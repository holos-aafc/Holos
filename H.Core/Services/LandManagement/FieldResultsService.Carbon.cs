using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        #region Public Methods

        public void CalculateFactors(AdjoiningYears adjoiningYears, Farm farm)
        {
            var viewItem = adjoiningYears.CurrentYearViewItem;

            CalculateFactors(viewItem, farm);
        }

        public void CalculateFactors(List<CropViewItem> viewItems, Farm farm)
        {
            foreach (var cropViewItem in viewItems) CalculateFactors(cropViewItem, farm);
        }

        /// <summary>
        ///     Although climate/management factors are not used in the Tier 2 carbon modelling, they are used in the N budget and
        ///     so must be calculated when user specifies Tier 2 or ICBM modelling
        /// </summary>
        public void CalculateFactors(CropViewItem viewItem, Farm farm)
        {
            if (farm.IsCommandLineMode)
            {
                /*
                 * In CLI
                 *
                 * These factors can never be zero.
                 *
                 * If they are not zero, don't overwrite them so that we use what was in the input files.
                 */

                if (viewItem.ClimateParameter == 0)
                {
                    viewItem.ClimateParameter = _climateService.CalculateClimateParameter(viewItem, farm);
                }

                if (viewItem.TillageFactor == 0)
                {
                    viewItem.TillageFactor = this.CalculateTillageFactor(viewItem, farm);
                }

                if (viewItem.ManagementFactor == 0)
                {
                    viewItem.ManagementFactor = this.CalculateManagementFactor(viewItem.ClimateParameter, viewItem.TillageFactor);
                }
            }
            else
            {
                /*
                 * In GUI
                 */

                viewItem.ClimateParameter = _climateService.CalculateClimateParameter(viewItem, farm);
                viewItem.TillageFactor = this.CalculateTillageFactor(viewItem, farm);
                viewItem.ManagementFactor = this.CalculateManagementFactor(viewItem.ClimateParameter, viewItem.TillageFactor);
            }
        }

        /// <summary>
        ///     Before carbon change can be calculated, all view items must have yields assigned so that we can determine the total
        ///     carbon inputs from all crops, manure applications, supplemental
        ///     hay applications, etc. Then we can proceed to the actual carbon change calculations.
        /// </summary>
        public void AssignCarbonInputs(IEnumerable<CropViewItem> viewItems,
            Farm farm)
        {
            var mainCrops = viewItems.GetMainCrops();
            var secondaryCrops = viewItems.GetSecondaryCrops();

            var animalResults = _animalService.GetAnimalResults(farm);

            _carbonService.AssignInputsAndLosses(mainCrops, farm, animalResults);
            _carbonService.AssignInputsAndLosses(secondaryCrops, farm, animalResults);

            CalculateFactors(mainCrops, farm);
        }

        public void AssignNitrogenInputs(List<CropViewItem> viewItems, Farm farm)
        {
            var mainCrops = viewItems.GetMainCrops();
            var secondaryCrops = viewItems.GetSecondaryCrops();

            _nitrogenService.AssignNitrogenInputs(mainCrops, farm);
            _nitrogenService.AssignNitrogenInputs(secondaryCrops, farm);
        }

        /// <summary>
        ///     If there is a year where a perennial crop has a 0 yield, it means it wasn't harvested that year. Therefore, when a
        ///     perennial year has a 0 yield,
        ///     we must set the percentage of product returned to soil to 100% (instead of the default 35% for perennials) since
        ///     everything stayed in the field that year.
        ///     This method has to be called after we assign yields.
        /// </summary>
        public void UpdatePercentageReturnsForPerennials(IEnumerable<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
                if (cropViewItem.CropType.IsPerennial())
                    if (cropViewItem.Yield == 0)
                        cropViewItem.PercentageOfProductYieldReturnedToSoil =
                            100; // Now C inputs will be calculated correctly for this year
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Calculates the average soil organic carbon value for all fields on the farm.
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
                    viewItem.AverageSoilCarbonAcrossAllFieldsInFarm = averageSoilOrganicCarbon;
            }
        }

        private FieldSystemComponent GetLeftMostComponent(FieldSystemComponent fieldSystemComponent, Farm farm)
        {
            var currentFieldComponent = new FieldSystemComponent();

            var currentComponentId = fieldSystemComponent.CurrentPeriodComponentGuid;
            if (currentComponentId.Equals(Guid.Empty))
                currentFieldComponent = fieldSystemComponent;
            else
                currentFieldComponent = farm.GetFieldSystemComponent(currentComponentId);

            if (currentFieldComponent.HistoricalComponents.Any())
                return currentFieldComponent.HistoricalComponents.Cast<FieldSystemComponent>().OrderBy(x => x.StartYear)
                    .First();

            return currentFieldComponent;
        }

        /// <summary>
        ///     Calculates final results for one field. Results will be assigned to view items
        /// </summary>
        private void CalculateFinalResultsForField(
            List<CropViewItem> viewItemsForField,
            Farm farm,
            Guid fieldSystemGuid)
        {
            var fieldSystemComponent = farm.GetFieldSystemComponent(fieldSystemGuid);

            // Need to get leftmost component here
            var leftMost = GetLeftMostComponent(fieldSystemComponent, farm);

            // Create run in period items
            var runInPeriodItems = GetRunInPeriodItems(farm, leftMost.CropViewItems, leftMost.StartYear,
                viewItemsForField, leftMost);

            _initializationService.InitializeYieldForAllYears(runInPeriodItems, farm, leftMost);

            // Check if user specified ICBM or Tier 2 carbon modelling
            if (farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2)
            {
                _tier2SoilCarbonCalculator.AnimalComponentEmissionsResults = AnimalResults;

                /*
                 * Process run in period items
                 */

                _carbonService.AssignInputsAndLosses(runInPeriodItems, farm, AnimalResults);
                _nitrogenService.AssignNitrogenInputs(runInPeriodItems, farm);

                /*
                 * Process main view items
                 */

                if (farm.IsCommandLineMode)
                {
                    /*
                     * When in GUI mode, the inputs for the main view items will already have been assigned at AssignCarbonInputs() and AssignNitrogenInputs().
                     * When in CLI mode, we need to check if there are missing values and process any missing input values before calculating final results
                     */

                    _carbonService.ProcessCommandLineItems(viewItemsForField.ToList(), farm, AnimalResults);
                    _nitrogenService.ProcessCommandLineItems(viewItemsForField.ToList(), farm);
                    this.CalculateFactors(viewItemsForField.ToList(), farm);
                    this.CombineInputsForAllCropsInSameYear(farm, viewItemsForField.ToList());
                }

                // Combine inputs now that we have C and N inputs set for all items
                CombineInputsForAllCropsInSameYear(farm, runInPeriodItems);

                // Merge all run in period items
                var mergedRunInItems = MergeDetailViewItems(runInPeriodItems, leftMost);

                // Combine inputs for run in period
                CombineInputsForAllCropsInSameYear(farm, mergedRunInItems);

                _tier2SoilCarbonCalculator.CalculateResults(
                    farm,
                    viewItemsForField,
                    leftMost,
                    mergedRunInItems);
            }
            else
            {
                _icbmSoilCarbonCalculator.AnimalComponentEmissionsResults = AnimalResults;

                 if (farm.IsCommandLineMode)
                {
                    /*
                     * When in GUI mode, the inputs for the main view items will already have been assigned at AssignCarbonInputs() and AssignNitrogenInputs().
                     * When in CLI mode, we need to check if there are missing values and process any missing input values before calculating final results
                     */

                    _carbonService.ProcessCommandLineItems(viewItemsForField.ToList(), farm, AnimalResults);
                    _nitrogenService.ProcessCommandLineItems(viewItemsForField.ToList(), farm);
                    this.CalculateFactors(viewItemsForField.ToList(), farm);
                    this.CombineInputsForAllCropsInSameYear(farm, viewItemsForField.ToList());
                }

                // Create the item with the steady state (equilibrium) values
                var equilibriumYearResults =
                    _icbmSoilCarbonCalculator.CalculateEquilibriumYear(viewItemsForField, farm, fieldSystemGuid);

                for (var i = 0; i < viewItemsForField.Count; i++)
                {
                    var currentYearResults = viewItemsForField.ElementAt(i);

                    // Get previous year results, if there is no previous year (i.e. t = 0), then use equilibrium (or custom measured) values for the pools
                    var previousYearResults = i == 0 ? equilibriumYearResults : viewItemsForField.ElementAt(i - 1);

                    // Carbon must be calculated before nitrogen
                    _icbmSoilCarbonCalculator.CalculateCarbonAtInterval(
                        previousYearResults,
                        currentYearResults,
                        farm);

                    _icbmSoilCarbonCalculator.CalculateNitrogenAtInterval(
                        previousYearResults,
                        currentYearResults,
                        null,
                        farm,
                        i);
                }
            }

            foreach (var cropViewItem in viewItemsForField)
            {
                var energyResults = CalculateCropEnergyResults(cropViewItem, farm);
                cropViewItem.CropEnergyResults = energyResults;
                cropViewItem.EstimatesOfProductionResultsViewItem =
                    CalculateEstimateOfProduction(cropViewItem, fieldSystemComponent);
            }
        }

        #endregion
    }
}