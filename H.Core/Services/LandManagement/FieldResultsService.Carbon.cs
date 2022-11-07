using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        public class AdjoiningYears
        {
            public CropViewItem PreviousYearViewItem { get; set; }
            public CropViewItem CurrentYearViewItem { get; set; }
            public CropViewItem NextYearViewItem { get; set; }
        }

        #region Public Methods

        /// <summary>
        /// A method to get the previous year from a perennial stand when undersown crops are used. Getting the previous year and next year for an
        /// item is required since the carbon input calculations for perennials need to look back/forward in time to calculate inputs for any year that has a missing yield.
        /// 
        /// For annuals, there is no need to get the previous/next years but for consistency, this method is called for annual crops regardless.
        /// </summary>
        public AdjoiningYears GetAdjoiningYears(
            IEnumerable<CropViewItem> viewItems,
            int year,
            FieldSystemComponent fieldSystemComponent)
        {
            var previousYear = year - 1;
            var nextYear = year + 1;

            // Get all items from the same year
            var viewItemsForYear = viewItems.Where(x => x.Year == year).OrderBy(x => x.Year).ToList();

            var mainCropForCurrentYear = this.GetMainCropForYear(viewItemsForYear, year, fieldSystemComponent);
            if (mainCropForCurrentYear.CropType.IsPerennial())
            {
                // Items with same stand id
                var perennialItemsInStand = viewItems.Where(x => x.CropType.IsPerennial() && x.PerennialStandGroupId.Equals(mainCropForCurrentYear.PerennialStandGroupId));
                var previousItemInStand = perennialItemsInStand.SingleOrDefault(x => x.Year == previousYear);
                var nextItemInStand = perennialItemsInStand.SingleOrDefault(x => x.Year == nextYear);

                return new AdjoiningYears()
                {
                    PreviousYearViewItem = previousItemInStand,
                    CurrentYearViewItem = mainCropForCurrentYear,
                    NextYearViewItem = nextItemInStand
                };
            }
            else
            {
                return new AdjoiningYears()
                {
                    PreviousYearViewItem = null,
                    CurrentYearViewItem = mainCropForCurrentYear,
                    NextYearViewItem = null,
                };
            }
        }

        /// <summary>
        /// Before carbon change can be calculated, all view items must have yields assigned so that we can determine the total carbon inputs from all crops, manure applications, supplemental 
        /// hay applications, etc. Then we can proceed to the actual carbon change calculations.
        /// </summary>
        public void AssignCarbonInputs(
            IEnumerable<CropViewItem> viewItems,
            Farm farm,
            FieldSystemComponent fieldSystemComponent)
        {
            // Yields must be assigned to all items before we can loop over each year and calculate plant carbon in agricultural product (C_p)
            this.AssignYieldToAllYears(
                cropViewItems: viewItems,
                farm: farm);

            // After yields have been set, we must consider perennial years in which there is 0 for the yield input (from user or by default yield provider)
            this.UpdatePercentageReturnsForPerennials(
                viewItems: viewItems);

            var mainCrops = viewItems.OrderBy(x => x.Year).Where(x => x.IsSecondaryCrop == false).ToList();
            var distinctYears = mainCrops.Select(x => x.Year).Distinct().OrderBy(x => x).ToList();

            // Consider the main crops for each year in the sequence
            foreach (var year in distinctYears)
            {
                // Get the previous, current, and next year view items for the year being considered
                var adjoiningYears = this.GetAdjoiningYears(mainCrops, year, fieldSystemComponent);

                var previousYearViewItem = adjoiningYears.PreviousYearViewItem;
                var currentYearViewItem = adjoiningYears.CurrentYearViewItem;
                var nextYearViewItem = adjoiningYears.NextYearViewItem;

                /*
                 * Assign carbon inputs based on selected strategy
                 */

                if (farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2 &&
                    _tier2SoilCarbonCalculator.CanCalculateInputsForCrop(currentYearViewItem))
                {
                    // If IPCC Tier 2 is the selected strategy and we can calculate inputs for the specified crop, then use the IPCC methodology for calculating inputs
                    _tier2SoilCarbonCalculator.CalculateInputs(
                        viewItem: currentYearViewItem, farm: farm);
                }
                else
                {
                    // Use ICBM approach to assign inputs if the Tier 2 approach cannot be used

                    // Some crop types (currently only perennials, need access to the previous year's crop and also next year's crop in order to calculate inputs in some scenarios)
                    _icbmSoilCarbonCalculator.SetCarbonInputs(
                        previousYearViewItem: previousYearViewItem,
                        currentYearViewItem: currentYearViewItem,
                        nextYearViewItem: nextYearViewItem,
                        farm: farm);
                }

                // Although climate/management factors are not used in the Tier 2 carbon modelling, they are used in the N budget and so must be calculated when user specifies Tier 2 or ICBM modelling
                currentYearViewItem.ClimateParameter = this.CalculateClimateParameter(currentYearViewItem, farm);
                currentYearViewItem.TillageFactor = this.CalculateTillageFactor(currentYearViewItem, farm);
                currentYearViewItem.ManagementFactor = this.CalculateManagementFactor(currentYearViewItem.ClimateParameter, currentYearViewItem.TillageFactor);
            }

            // Consider the secondary crops
            var secondaryCrops = viewItems.OrderBy(x => x.Year).Where(x => x.IsSecondaryCrop).ToList();
            foreach (var secondaryCrop in secondaryCrops)
            {
                if (farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2 &&
                    _tier2SoilCarbonCalculator.CanCalculateInputsForCrop(secondaryCrop))
                {
                    // If IPCC Tier 2 is the selected strategy and we can calculate inputs for the specified crop, then use the IPCC methodology for calculating inputs
                    _tier2SoilCarbonCalculator.CalculateInputs(
                        viewItem: secondaryCrop, farm: farm);
                }
                else
                {
                    // Use ICBM approach to assign inputs

                    _icbmSoilCarbonCalculator.SetCarbonInputs(
                        previousYearViewItem: null,
                        currentYearViewItem: secondaryCrop,
                        nextYearViewItem: null,
                        farm: farm);
                }
            }
        }

        /// <summary>
        /// If there is a year where a perennial crop has a 0 yield, it means it wasn't harvested that year. Therefore, when a perennial year has a 0 yield,
        /// we must set the percentage of product returned to soil to 100% (instead of the default 35% for perennials) since everything stayed in the field that year.
        ///
        /// This method has to be called after we assign yields.
        /// </summary>
        public void UpdatePercentageReturnsForPerennials(IEnumerable<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
            {
                if (cropViewItem.CropType.IsPerennial())
                {
                    if (cropViewItem.Yield == 0)
                    {
                        cropViewItem.PercentageOfProductYieldReturnedToSoil = 100; // Now C inputs will be calculated correctly for this year
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void CalculateCarbonAtInterval(
            CropViewItem previousYearResults,
            CropViewItem currentYearResults,
            Farm farm,
            FieldSystemComponent fieldSystemComponent)
        {
            // The user can choose to use either the climate parameter or the management factor in the calculations
            var climateParameterOrManagementFactor = farm.Defaults.UseClimateParameterInsteadOfManagementFactor ? currentYearResults.ClimateParameter : currentYearResults.ManagementFactor;

            currentYearResults.YoungPoolSoilCarbonAboveGround = _icbmSoilCarbonCalculator.CalculateYoungPoolAboveGroundCarbonAtInterval(
                youngPoolAboveGroundCarbonAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonAboveGround,
                aboveGroundCarbonAtPreviousInterval: previousYearResults.AboveGroundCarbonInput,
                youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                climateParameter: climateParameterOrManagementFactor);

            currentYearResults.YoungPoolSoilCarbonBelowGround = _icbmSoilCarbonCalculator.CalculateYoungPoolBelowGroundCarbonAtInterval(
                youngPoolBelowGroundCarbonAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonBelowGround,
                belowGroundCarbonAtPreviousInterval: previousYearResults.BelowGroundCarbonInput,
                youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                climateParameter: climateParameterOrManagementFactor);

            currentYearResults.YoungPoolManureCarbon = _icbmSoilCarbonCalculator.CalculateYoungPoolManureCarbonAtInterval(
                youngPoolManureCarbonAtPreviousInterval: previousYearResults.YoungPoolManureCarbon,
                manureCarbonInputAtPreviousInterval: previousYearResults.ManureCarbonInputsPerHectare,
                youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                climateParameter: climateParameterOrManagementFactor);

            currentYearResults.OldPoolSoilCarbon = _icbmSoilCarbonCalculator.CalculateOldPoolSoilCarbonAtInterval(
                oldPoolSoilCarbonAtPreviousInterval: previousYearResults.OldPoolSoilCarbon,
                aboveGroundHumificationCoefficient: farm.Defaults.HumificationCoefficientAboveGround,
                belowGroundHumificationCoefficient: farm.Defaults.HumificationCoefficientBelowGround,
                youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                oldPoolDecompositionRate: farm.Defaults.DecompositionRateConstantOldPool,
                youngPoolAboveGroundOrganicCarbonAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonAboveGround,
                youngPoolBelowGroundOrganicCarbonAtPreviousInterval: previousYearResults.YoungPoolSoilCarbonBelowGround,
                aboveGroundCarbonResidueAtPreviousInterval: previousYearResults.AboveGroundCarbonInput,
                belowGroundCarbonResidueAtPreviousInterval: previousYearResults.BelowGroundCarbonInput,
                climateParameter: climateParameterOrManagementFactor,
                youngPoolManureAtPreviousInterval: previousYearResults.YoungPoolManureCarbon,
                manureHumificationCoefficient: farm.Defaults.HumificationCoefficientManure,
                manureCarbonInputAtPreviousInterval: previousYearResults.ManureCarbonInputsPerHectare);

            currentYearResults.SoilCarbon = _icbmSoilCarbonCalculator.CalculateSoilCarbonAtInterval(
                youngPoolSoilCarbonAboveGroundAtInterval: currentYearResults.YoungPoolSoilCarbonAboveGround,
                youngPoolSoilCarbonBelowGroundAtInterval: currentYearResults.YoungPoolSoilCarbonBelowGround,
                oldPoolSoilCarbonAtInterval: currentYearResults.OldPoolSoilCarbon,
                youngPoolManureAtInterval: currentYearResults.YoungPoolManureCarbon);

            currentYearResults.ChangeInCarbon = _icbmSoilCarbonCalculator.CalculateChangeInSoilCarbonAtInterval(
                soilOrganicCarbonAtInterval: currentYearResults.SoilCarbon,
                soilOrganicCarbonAtPreviousInterval: previousYearResults.SoilCarbon);
        }

        /// <summary>
        /// Calculates the average soil organic carbon value for all fields on the farm.
        /// </summary>
        private void CalculateAverageSoilOrganicCarbonForFields(
            IEnumerable<CropViewItem> viewItems)
        {
            var distinctYears = viewItems.Select(x => x.Year).Distinct();
            foreach (var year in distinctYears)
            {
                // Ge all view items that have the same year.
                var viewItemsByYear = viewItems.Where(x => x.Year == year);

                // Get the average soil organic carbon from items.
                var averageSoilOrganicCarbon = viewItemsByYear.Average(x => x.SoilCarbon);

                // Assign this common value to each item.
                foreach (var viewItem in viewItemsByYear)
                {
                    viewItem.AverageSoilCarbonAcrossAllFieldsInFarm = averageSoilOrganicCarbon;
                }
            }
        }

        public CropViewItem CalculateEquilibriumYear(
            List<CropViewItem> detailViewItems,
            Farm farm,
            Guid componentId)
        {
            var fieldSystemComponent = farm.GetFieldSystemComponent(componentId);

            // Get the field system component that will be used to calculate the equilibrium year            
            var sizeOfFirstRotationForField = detailViewItems.OrderBy(viewItem => viewItem.Year).First().SizeOfFirstRotationForField;
            if (sizeOfFirstRotationForField == 0)
            {
                // Was not set during creation of old farm                
                sizeOfFirstRotationForField = fieldSystemComponent.SizeOfFirstRotationInField();
            }

            // Get the detail view items that represent the crops that define the first rotation
            var viewItemsInRotation = detailViewItems.Take(sizeOfFirstRotationForField).ToList();

            /*
             * Calculate averages for the crops that define the first rotation - this is used to calculate the equilibrium state
             *
             * Use a specified strategy to calculate these starting values
             */

            var equilibriumAboveGroundInput = 0d;
            var equilibriumBelowGroundInput = 0d;
            var equilibriumManureInput = 0d;
            var equilibriumClimateParameter = 0d;
            var equilibriumManagementFactor = 0d;

            var equilibriumCarbonInputFromProduct = 0d;
            var equilibriumCarbonInputFromStraw = 0d;
            var equilibriumCarbonInputFromRoots = 0d;
            var equilibriumCarbonInputFromExtraroots = 0d;

            var strategy = farm.Defaults.EquilibriumCalculationStrategy;
            if (strategy == EquilibriumCalculationStrategies.CarMultipleYearAverage)
            {

            }
            else
            {
                // At this point, the detail view items have had their C inputs calculated

                // Equation 2.1.3-1
                equilibriumAboveGroundInput = viewItemsInRotation.Average(x => x.AboveGroundCarbonInput);

                // Equation 2.1.3-2
                equilibriumBelowGroundInput = viewItemsInRotation.Average(x => x.BelowGroundCarbonInput);

                // Equation 2.1.3-3
                equilibriumManureInput = viewItemsInRotation.Average(x => x.ManureCarbonInputsPerHectare);
                equilibriumClimateParameter = viewItemsInRotation.Average(x => x.ClimateParameter);
                equilibriumManagementFactor = viewItemsInRotation.Average(x => x.ManagementFactor);

                
                equilibriumCarbonInputFromProduct = viewItemsInRotation.Average(x => x.CarbonInputFromProduct);
                equilibriumCarbonInputFromStraw = viewItemsInRotation.Average(x => x.CarbonInputFromStraw);
                equilibriumCarbonInputFromRoots = viewItemsInRotation.Average(x => x.CarbonInputFromRoots);
                equilibriumCarbonInputFromExtraroots = viewItemsInRotation.Average(x => x.CarbonInputFromExtraroots);
            }

            // This is the equilibrium year result
            var result = new CropViewItem();

            result.AboveGroundCarbonInput = equilibriumAboveGroundInput;
            result.BelowGroundCarbonInput = equilibriumBelowGroundInput;
            result.ManureCarbonInputsPerHectare = equilibriumManureInput;
            result.ClimateParameter = equilibriumClimateParameter;
            result.ManagementFactor = equilibriumManagementFactor;

            var averageNitrogenConcentrationInProduct = viewItemsInRotation.Select(x => x.NitrogenContentInProduct).Average();
            var averageNitrogenConcentrationInStraw = viewItemsInRotation.Select(x => x.NitrogenContentInStraw).Average();
            var averageNitrogenConcentrationInRoots = viewItemsInRotation.Select(x => x.NitrogenContentInRoots).Average();
            var averageNitrogenConcentrationInExtraroots = viewItemsInRotation.Select(x => x.NitrogenContentInExtraroot).Average();

            result.NitrogenContentInProduct = averageNitrogenConcentrationInProduct;
            result.NitrogenContentInStraw = averageNitrogenConcentrationInStraw;
            result.NitrogenContentInRoots = averageNitrogenConcentrationInRoots;
            result.NitrogenContentInExtraroot = averageNitrogenConcentrationInExtraroots;

            result.CarbonInputFromProduct = equilibriumCarbonInputFromProduct;
            result.CarbonInputFromStraw = equilibriumCarbonInputFromStraw;
            result.CarbonInputFromRoots = equilibriumCarbonInputFromRoots;
            result.CarbonInputFromExtraroots = equilibriumCarbonInputFromExtraroots;

            /*
             * Carbon
             */

            // The user can choose to use either the climate parameter or the management factor in the calculations
            var climateOrManagementFactor = farm.Defaults.UseClimateParameterInsteadOfManagementFactor ? result.ClimateParameter : result.ManagementFactor;

            if (farm.UseCustomStartingSoilOrganicCarbon == false)
            {
                result.YoungPoolSoilCarbonAboveGround = _icbmSoilCarbonCalculator.CalculateYoungPoolSteadyStateAboveGround(
                    averageAboveGroundCarbonInput: result.AboveGroundCarbonInput,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateOrManagementFactor);
            }
            else
            {

                // Equation 2.1.3-8
                var youngPoolAboveGround = result.AboveGroundCarbonInput / (climateOrManagementFactor * farm.Defaults.DecompositionRateConstantYoungPool);



                result.YoungPoolSoilCarbonAboveGround = youngPoolAboveGround;
            }

            if (farm.UseCustomStartingSoilOrganicCarbon == false)
            {
                result.YoungPoolSoilCarbonBelowGround = _icbmSoilCarbonCalculator.CalculateYoungPoolSteadyStateBelowGround(
                    averageBelowGroundCarbonInput: result.BelowGroundCarbonInput,
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    climateParameter: climateOrManagementFactor);
            }
            else
            {
                // Equation 2.1.3-9
                var youngPoolBelowGround = result.BelowGroundCarbonInput / (climateOrManagementFactor * farm.Defaults.DecompositionRateConstantYoungPool);

                result.YoungPoolSoilCarbonBelowGround = youngPoolBelowGround;
            }

            result.YoungPoolManureCarbon = _icbmSoilCarbonCalculator.CalculateYoungPoolSteadyStateManure(
                averageManureCarbonInput: result.ManureCarbonInputsPerHectare,
                youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                climateParameter: climateOrManagementFactor);

            if (farm.UseCustomStartingSoilOrganicCarbon == false)
            {
                result.OldPoolSoilCarbon = _icbmSoilCarbonCalculator.CalculateOldPoolSteadyState(
                    youngPoolDecompositionRate: farm.Defaults.DecompositionRateConstantYoungPool,
                    oldPoolDecompositionRate: farm.Defaults.DecompositionRateConstantOldPool,
                    climateParameter: climateOrManagementFactor,
                    aboveGroundHumificationCoefficient: farm.Defaults.HumificationCoefficientAboveGround,
                    belowGroundHumificationCoefficient: farm.Defaults.HumificationCoefficientBelowGround,
                    averageAboveGroundCarbonInputOfRotation: result.AboveGroundCarbonInput,
                    averageBelowGroundCarbonInputOfRotation: result.BelowGroundCarbonInput,
                    aboveGroundYoungPoolSteadyState: result.YoungPoolSoilCarbonAboveGround,
                    belowGroundYoungPoolSteadyState: result.YoungPoolSoilCarbonBelowGround,
                    manureYoungPoolSteadyState: result.YoungPoolSteadyStateManure,
                    averageManureCarbonInputOfRotation: result.ManureCarbonInputsPerHectare,
                    manureHumificationCoefficient: farm.Defaults.HumificationCoefficientManure);
            }
            else
            {
                // Equation 2.1.3-10
                result.OldPoolSoilCarbon = farm.StartingSoilOrganicCarbon - (result.YoungPoolSoilCarbonAboveGround + result.YoungPoolSoilCarbonBelowGround);
            }

            result.SoilCarbon = _icbmSoilCarbonCalculator.CalculateSoilCarbonAtInterval(
                youngPoolSoilCarbonAboveGroundAtInterval: result.YoungPoolSoilCarbonAboveGround,
                youngPoolSoilCarbonBelowGroundAtInterval: result.YoungPoolSoilCarbonBelowGround,
                oldPoolSoilCarbonAtInterval: result.OldPoolSoilCarbon,
                youngPoolManureAtInterval: result.YoungPoolManureCarbon);

            return result;
        }

        /// <summary>
        /// Calculates final results for one field. Results will be assigned to view items
        /// </summary>
        private void CalculateFinalResultsForField(
            List<CropViewItem> viewItemsForField,
            Farm farm,
            Guid fieldSystemGuid)
        {
            

            var fieldSystemComponent = farm.GetFieldSystemComponent(fieldSystemGuid);

            // Check if user specified ICBM or Tier 2 carbon modelling
            if (farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2)
            {
                _tier2SoilCarbonCalculator.AnimalComponentEmissionsResults = this.AnimalResults;

                foreach (var runInPeriodItem in fieldSystemComponent.RunInPeriodItems)
                {
                    if (_tier2SoilCarbonCalculator.CanCalculateInputsForCrop(runInPeriodItem))
                    {
                        _tier2SoilCarbonCalculator.CalculateInputs(runInPeriodItem, farm);
                    }
                    else
                    {
                        _icbmSoilCarbonCalculator.SetCarbonInputs(null, runInPeriodItem, null, farm);
                    }
                }

                _tier2SoilCarbonCalculator.CalculateResults(
                    farm: farm,
                    viewItemsByField: viewItemsForField,
                    fieldSystemComponent: fieldSystemComponent);
            }
            else
            {
                _icbmSoilCarbonCalculator.AnimalComponentEmissionsResults = this.AnimalResults;

                // Create the item with the steady state (equilibrium) values
                var equilibriumYearResults = this.CalculateEquilibriumYear(viewItemsForField, farm, fieldSystemGuid);

                for (int i = 0; i < viewItemsForField.Count; i++)
                {
                    var currentYearResults = viewItemsForField.ElementAt(i);

                    // Get previous year results, if there is no previous year (i.e. t = 0), then use equilibrium year values
                    var previousYearResults = i == 0 ? equilibriumYearResults : viewItemsForField.ElementAt(i - 1);

                    // Carbon must be calculated before nitrogen
                    this.CalculateCarbonAtInterval(
                        previousYearResults: previousYearResults,
                        currentYearResults: currentYearResults,
                        farm: farm,
                        fieldSystemComponent: fieldSystemComponent);

                     _icbmSoilCarbonCalculator.CalculateNitrogenAtInterval(
                        previousYearResults: previousYearResults,
                        currentYearResults: currentYearResults,
                        nextYearResults: null,
                        farm: farm,
                        yearIndex: i);
                }
            }

            foreach (var cropViewItem in viewItemsForField)
            {
                var energyResults = this.CalculateCropEnergyResults(cropViewItem, farm);
                cropViewItem.CropEnergyResults = energyResults;
                cropViewItem.EstimatesOfProductionResultsViewItem = this.CalculateEstimateOfProduction(cropViewItem, fieldSystemComponent);
            }
        }

        /// <summary>
        /// Calculates how much carbon was lost due to bales being exported off field.
        /// </summary>
        public void CalculateCarbonLostFromHayExports(FieldSystemComponent fieldSystemComponent, Farm farm)
        {
            // Get hay exports from component, find other components dependent on exports, assign losses to view item
            foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
            {
                if (cropViewItem.HasHarvestViewItems)
                {
                    var totalHarvestedBales = cropViewItem.HarvestViewItems.Sum(x => x.TotalNumberOfBalesHarvested);
                    var totalBalesImportedFromOtherFields = 0d;
                    var moistureContentOfImportedBales = new List<double>();

                    // Check all other fields to see if anyone is using these harvested bales
                    foreach (var component in farm.FieldSystemComponents)
                    {
                        foreach (var viewItem in component.CropViewItems)
                        {
                            foreach (var hayImportViewItem in viewItem.HayImportViewItems.Where(x => x.FieldSourceGuid.Equals(fieldSystemComponent.Guid) && x.Date.Year == cropViewItem.Year))
                            {
                                if (hayImportViewItem.SourceOfBales == ResourceSourceLocation.OffFarm)
                                {
                                    continue;
                                }

                                totalBalesImportedFromOtherFields += hayImportViewItem.NumberOfBales;
                                moistureContentOfImportedBales.Add(hayImportViewItem.MoistureContentAsPercentage);
                            }
                        }
                    }

                    if (totalBalesImportedFromOtherFields == 0)
                    {
                        continue;
                    }

                    var totalWetWeight = totalHarvestedBales - totalBalesImportedFromOtherFields;
                    if (totalWetWeight > 0)
                    {
                        // Get average moisture content to calculate the dry matter
                        var averageMoistureContentPercentage = moistureContentOfImportedBales.Average();

                        // Calculate dry matter of the bales
                        var totalDryMatter = totalWetWeight * (1 - (averageMoistureContentPercentage / 100.0));

                        // Calculate total carbon in dry matter
                        var totalCarbonInExportedBales = totalDryMatter * CoreConstants.CarbonConcentration;

                        cropViewItem.TotalCarbonLossFromBaleExports = this.CalculateTotalCarbonLossFromBaleExports(
                            percentageOfProductReturnedToSoil: cropViewItem.PercentageOfProductYieldReturnedToSoil,
                            totalCarbonInExprtedBales: totalCarbonInExportedBales);
                    }
                }
            }
        }

        /// <summary>
        /// Equation 12.3.2-4
        /// </summary>
        /// <param name="percentageOfProductReturnedToSoil">Amount of product yield returned to soil (%)</param>
        /// <param name="totalCarbonInExprtedBales">Total amount of carbon in bales (kg C)</param>
        /// <returns>Total amount of carbon lost from exported bales (kg C)</returns>
        private double CalculateTotalCarbonLossFromBaleExports(
            double percentageOfProductReturnedToSoil,
            double totalCarbonInExprtedBales)
        {
            var result = totalCarbonInExprtedBales / (1 - (percentageOfProductReturnedToSoil / 100.0));

            return result;
        }

        /// <summary>
        /// Calculates how much carbon added from manure of animals grazing on the field.
        /// </summary>
        public void CalculateManureCarbonInputByGrazingAnimals(FieldSystemComponent fieldSystemComponent, Farm farm)
        {
            var animalComponentEmissionResults = _animalResultsService.GetAnimalResults(farm);

            this.CalculateManureCarbonInputByGrazingAnimals(
                fieldSystemComponent: fieldSystemComponent,
                results: animalComponentEmissionResults);
        }

        public void CalculateCarbonLostByGrazingAnimals(FieldSystemComponent fieldSystemComponent, IEnumerable<AnimalComponentEmissionsResults> results)
        {
            foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
            {
                var totalCarbonUptakeByAnimals = 0d;

                foreach (var grazingViewItem in cropViewItem.GrazingViewItems)
                {
                    var animalComponentEmissionsResults = results.SingleOrDefault(x => x.Component.Guid == grazingViewItem.AnimalComponentGuid);
                    if (animalComponentEmissionsResults != null)
                    {
                        var groupEmissionResults = animalComponentEmissionsResults.EmissionResultsForAllAnimalGroupsInComponent.SingleOrDefault(x => x.AnimalGroup.Guid == grazingViewItem.AnimalGroupGuid);
                        if (groupEmissionResults != null)
                        {
                            totalCarbonUptakeByAnimals += groupEmissionResults.TotalCarbonUptakeByAnimals();
                        }
                    }
                }

                cropViewItem.TotalCarbonLossesByGrazingAnimals = this.CalculateTotalCarbonLossFromGrazingAnimals(
                    forageUtilizationRate: cropViewItem.ForageUtilizationRate,
                    totalCarbonUptakeByGrazingAnimals: totalCarbonUptakeByAnimals);
            }
        }

        public void CalculateManureCarbonInputByGrazingAnimals(FieldSystemComponent fieldSystemComponent, IEnumerable<AnimalComponentEmissionsResults> results)
        {
            foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
            {
                var totalCarbonExcretedByAnimals = 0d;
                var manureMethaneEmissions = 0d;
                var totalVolumeOfManure = 0d;

                foreach (var grazingViewItem in cropViewItem.GrazingViewItems)
                {
                    var animalComponentEmissionsResults = results.SingleOrDefault(x => x.Component.Guid == grazingViewItem.AnimalComponentGuid);
                    if (animalComponentEmissionsResults != null)
                    {
                        var groupEmissionResults = animalComponentEmissionsResults.EmissionResultsForAllAnimalGroupsInComponent.SingleOrDefault(x => x.AnimalGroup.Guid == grazingViewItem.AnimalGroupGuid);
                        if (groupEmissionResults != null)
                        {
                            totalCarbonExcretedByAnimals += groupEmissionResults.TotalCarbonExcreted;
                            manureMethaneEmissions += groupEmissionResults.TotalManureMethane;
                            totalVolumeOfManure += groupEmissionResults.TotalVolumeOfManure;
                        }
                    }
                }

                if (totalVolumeOfManure == 0)
                {
                    continue;
                }

                // Equation 5.5.1-1
                cropViewItem.TotalCarbonInputFromManureFromAnimalsGrazingOnPasture = (totalCarbonExcretedByAnimals - manureMethaneEmissions) / totalVolumeOfManure;
            }
        }

        /// <summary>
        /// Equation 12.3.2-4
        /// </summary>
        /// <param name="forageUtilizationRate">Utilization rate for the particular forage (%)</param>
        /// <param name="totalCarbonUptakeByGrazingAnimals">Total carbon uptake from grazing animals (kg C)</param>
        /// <returns>Total carbon lost from grazing animals (kg C)</returns>
        private double CalculateTotalCarbonLossFromGrazingAnimals(
            double forageUtilizationRate,
            double totalCarbonUptakeByGrazingAnimals)
        {
            var result = totalCarbonUptakeByGrazingAnimals / (1 - (forageUtilizationRate / 100.0));

            return result;
        }



        #endregion
    }
}