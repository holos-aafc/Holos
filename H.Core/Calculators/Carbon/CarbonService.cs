﻿using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using System.Collections.Generic;
using System.Linq;
using H.Core.Services.Animals;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using H.Core.Providers.AnaerobicDigestion;
using H.Core.Services;
using H.Core.Services.Initialization.Crops;
using H.Core.Services.LandManagement;

namespace H.Core.Calculators.Carbon
{
    /// <summary>
    /// A service class to calculate inputs related to C. This will route calculations to the IPCC Tier 2 or ICBM methodology as required
    /// </summary>
    public class CarbonService : ICarbonService
    {
        #region Fields

        private readonly IIPCCTier2CarbonInputCalculator _ipccTier2CarbonInputCalculator;
        private readonly IICBMCarbonInputCalculator _icbmCarbonInputCalculator;
        private readonly IAnimalService _animalService;
        private readonly IFieldComponentHelper _fieldComponentHelper;
        private readonly ICropInitializationService _cropInitializationService;
        

        #endregion

        #region Constructors

        public CarbonService()
        {
            _cropInitializationService = new CropInitializationService();
            _ipccTier2CarbonInputCalculator = new IPCCTier2CarbonInputCalculator();
            _icbmCarbonInputCalculator = new ICBMCarbonInputCalculator();
            _animalService = new AnimalResultsService();
            _fieldComponentHelper = new FieldComponentHelper();
        }

        #endregion

        #region Public Methods

        public bool CanCalculateInputsUsingIpccTier2(CropViewItem cropViewItem)
        {
            return _ipccTier2CarbonInputCalculator.CanCalculateInputsForCrop(cropViewItem);
        }

        public void AssignInputsAndLosses(AdjoiningYears tuple, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults)
        {
            this.AssignInputsAndLosses(tuple.PreviousYearViewItem, tuple.CurrentYearViewItem, tuple.NextYearViewItem, farm, animalResults);
        }

        public void AssignInputsAndLosses(CropViewItem previousYear, CropViewItem viewItem, CropViewItem nextYear,
            Farm farm, List<AnimalComponentEmissionsResults> animalResults)
        {
            this.AssignInputs(previousYear, viewItem, nextYear, farm, animalResults);
            this.CalculateLosses(viewItem, farm);
        }

        public void AssignInputsAndLosses(List<CropViewItem> viewItems, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults)
        {
            this.AssignInputs(viewItems, farm, animalResults);
            this.CalculateLosses(viewItems, farm);
        }

        public void CalculateLosses(List<CropViewItem> viewItems, Farm farm)
        {
            var orderedItems = viewItems.OrderBy(x => x.Year).ToList();

            for (int index = 0; index < orderedItems.Count; index++)
            {
                CropViewItem currentYearItem = orderedItems[index];

                this.CalculateLosses(currentYearItem, farm);
            }
        }

        public void AssignInputs(List<CropViewItem> cropViewItems, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults)
        {
            var orderedItems = cropViewItems.OrderBy(x => x.Year).ToList();

            for (int index = 0; index < orderedItems.Count; index++)
            {
                var itemAtIndex = orderedItems.ElementAt(index);
                var year = itemAtIndex.Year;
                var tuple = _fieldComponentHelper.GetAdjoiningYears(orderedItems, year);

                this.AssignInputs(
                    previousYear: tuple.PreviousYearViewItem,
                    viewItem: tuple.CurrentYearViewItem,
                    nextYear: tuple.NextYearViewItem,
                    farm: farm, animalResults: animalResults);
            }
        }

        public void AssignInputs(CropViewItem previousYear, CropViewItem viewItem, CropViewItem nextYear, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults)
        {
            var isCliMode = farm.IsCommandLineMode;
            var residueInputMethod = farm.Defaults.ResidueInputCalculationMethod;
            var canCalculateInputsUsingIpccTier2 = this.CanCalculateInputsUsingIpccTier2(viewItem);

            /*
             * Currently the CLI supports the option to override the default carbon input calculation method
             */

            if (isCliMode)
            {
                // Check for 0 yield in inputs files

                if (residueInputMethod == ResidueInputCalculationMethod.ICBM)
                {
                    // If user specifies ICBM input calculation mode, use ICBM
                    _icbmCarbonInputCalculator.AssignInputs(previousYear, viewItem, nextYear, farm, animalResults);
                }
                else if (residueInputMethod == ResidueInputCalculationMethod.IPCCTier2 && canCalculateInputsUsingIpccTier2)
                {
                    // If user specifies IPCC Tier 2 input calculation mode, use IPCC Tier 2 but only if the crop is supported by IPCC Tier 2
                    _ipccTier2CarbonInputCalculator.AssignInputs(viewItem, farm, animalResults);
                }
                else
                {
                    // Default method (fallback)
                    if (canCalculateInputsUsingIpccTier2)
                    {
                        _ipccTier2CarbonInputCalculator.AssignInputs(viewItem, farm, animalResults);
                    }
                    else
                    {
                        _icbmCarbonInputCalculator.AssignInputs(previousYear, viewItem, nextYear, farm, animalResults);
                    }
                }
            }
            else
            {
                // GUI mode
                if (canCalculateInputsUsingIpccTier2)
                {
                    _ipccTier2CarbonInputCalculator.AssignInputs(viewItem, farm, animalResults);
                }
                else
                {
                    _icbmCarbonInputCalculator.AssignInputs(previousYear, viewItem, nextYear, farm, animalResults);
                }
            }
        }

        public void CalculateLosses(CropViewItem cropViewItem, Farm farm)
        {
            this.CalculateCarbonLostFromHayExports(farm, cropViewItem);
        }

        public double SumTotalAbovegroundCarbonInput(List<CropViewItem> viewItems)
        {
            return viewItems.Sum(x => x.AboveGroundCarbonInput);
        }

        public double SumTotalBelowgroundCarbonInput(List<CropViewItem> viewItems)
        {
            return viewItems.Sum(x => x.BelowGroundCarbonInput);
        }

        public double SumTotalManureCarbonInput(List<CropViewItem> viewItems)
        {
            return viewItems.Sum(x => x.ManureCarbonInputsPerHectare);
        }

        public double SumTotalDigestateCarbonInput(List<CropViewItem> viewItems)
        {
            return viewItems.Sum(x => x.DigestateCarbonInputsPerHectare);
        }

        /// <summary>
        /// (kg C ha^-1)
        /// </summary>
        public double CalculateManureCarbonInputFromGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            CropViewItem cropViewItem,
            List<AnimalComponentEmissionsResults> results, Farm farm)
        {
            _cropInitializationService.InitializeGrazingViewItems(farm, cropViewItem, fieldSystemComponent);

            var result = 0d;

            var grazingViewItems = cropViewItem.GrazingViewItems.ToList();

            var grazingItems = grazingViewItems.Where(x => x.Start.Year == cropViewItem.Year).ToList();

            foreach (var grazingViewItem in grazingItems)
            {
                var emissionsFromGrazingAnimals = _animalService.GetGroupEmissionsFromGrazingAnimals(results, grazingViewItem).ToList();
                foreach (var groupEmissionsByMonth in emissionsFromGrazingAnimals.ToList())
                {
                    result += (groupEmissionsByMonth.MonthlyFecalCarbonExcretion -
                               groupEmissionsByMonth.MonthlyManureMethaneEmission) / cropViewItem.Area;
                }
            }

            return result < 0 ? 0 : result;
        }

        /// <summary>
        /// Equation 5.6.1-1
        ///
        /// (kg C ha^-1)
        /// </summary>
        public void CalculateManureCarbonInputByGrazingAnimals(FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> results,
            List<CropViewItem> cropViewItems, Farm farm)
        {
            foreach (var cropViewItem in cropViewItems)
            {
                cropViewItem.TotalCarbonInputFromManureFromAnimalsGrazingOnPasture = this.CalculateManureCarbonInputFromGrazingAnimals(fieldSystemComponent, cropViewItem, results.ToList(), farm);
            }
        }

        public double CalculateInputsFromSupplementalHayFedToGrazingAnimals(CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem, CropViewItem nextYearViewItems, Farm farm)
        {
            return _icbmCarbonInputCalculator.CalculateInputsFromSupplementalHayFedToGrazingAnimals(previousYearViewItem, currentYearViewItem, nextYearViewItems, farm);
        }

        /// <summary>
        /// (kg C)
        /// </summary>
        public double GetSupplementalLosses(
            CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem,
            CropViewItem nextYearViewItems,
            Farm farm)
        {
            return _icbmCarbonInputCalculator.GetSupplementalLosses(previousYearViewItem, currentYearViewItem, nextYearViewItems, farm);
        }

        /// <summary>
        /// Calculates how much carbon was lost due to bales being exported off field.
        /// </summary>
        public void CalculateCarbonLostFromHayExports(Farm farm, CropViewItem cropViewItem)
        {
            var dryMatter = this.CalculateTotalDryMatterLossFromResidueExports(cropViewItem, farm);

            cropViewItem.TotalDryMatterLostFromBaleExports = dryMatter;
            cropViewItem.TotalCarbonLossFromBaleExports = dryMatter * farm.Defaults.CarbonConcentration;
        }

        /// <summary>
        /// Calculates the total dry matter lost from a field once reductions have been made to account for other fields on the farm may be using that dry matter
        /// as supplemental feed.
        ///
        /// (kg DM)
        /// </summary>
        /// <returns>Total dry matter lost from exports off farm (kg DM)</returns>
        public double CalculateTotalDryMatterLossFromResidueExports(CropViewItem cropViewItem,
            Farm farm)
        {
            var result = 0d;
            var targetYear = cropViewItem.Year;

            var field = farm.GetFieldSystemComponent(cropViewItem.FieldSystemComponentGuid);
            if (field == null || field.HasHayHarvestInYear(cropViewItem.Year) == false)
            {
                return result;
            }

            // Get all hay harvests by year
            var hayHarvestsByYear = cropViewItem.GetHayHarvestsByYear(targetYear);

            // Get all hay imports for entire farm
            var hayImportsByYear = farm.GetHayImportsUsingImportedHayFromSourceFieldByYear(cropViewItem.FieldSystemComponentGuid, targetYear);

            // Get total harvested mass
            var dryMatterHarvested = hayHarvestsByYear.Sum(x => x.AboveGroundBiomassDryWeight);
            
            // Get total imported dry matter that have used 
            var dryMatterImported = hayImportsByYear.Sum(x => x.AboveGroundBiomassDryWeight);

            result = dryMatterHarvested - dryMatterImported;
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Equation 11.3.2-4
        /// </summary>
        public void CalculateCarbonLostByGrazingAnimals(
            Farm farm,
            FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> animalComponentEmissionsResults,
            List<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
            {
                if (cropViewItem.HarvestMethod == HarvestMethods.StubbleGrazing)
                {
                    continue;
                }

                var totalCarbonLossesForField = 0d;
                var totalCarbonUptakeForField = 0d;

                foreach (var componentResults in animalComponentEmissionsResults.ToList())
                {
                    if (componentResults.Component is AnimalComponentBase animalComponentBase)
                    {
                        var totalLostForAllGroups = 0d;
                        var totalCarbonUptakeForAllGroups = 0d;

                        var animalGroups = animalComponentBase.Groups;
                        foreach (var animalGroup in animalGroups)
                        {
                            // managementPeriod.Start.Year == currentYearResults.Year
                            var grazingManagementPeriodsByGroup = _animalService.GetGrazingManagementPeriods(animalGroup, fieldSystemComponent).Where(x => x.Start.Year == cropViewItem.Year).ToList();
                            var totalCarbonLostForAllManagementPeriods = this.CalculateUptakeByGrazingAnimals(grazingManagementPeriodsByGroup, cropViewItem, animalGroup, fieldSystemComponent, farm, animalComponentBase);

                            totalLostForAllGroups += totalCarbonLostForAllManagementPeriods.Item1;
                            totalCarbonUptakeForAllGroups += totalCarbonLostForAllManagementPeriods.Item2;
                        }

                        totalCarbonLossesForField += totalLostForAllGroups;
                        totalCarbonUptakeForField += totalCarbonUptakeForAllGroups;
                    }
                }

                var amountsNotEaten = GetSupplementalLosses(null, cropViewItem, null, farm);

                cropViewItem.TotalCarbonLossesByGrazingAnimals = totalCarbonLossesForField - amountsNotEaten;
                cropViewItem.TotalCarbonUptakeByAnimals = totalCarbonUptakeForField;
            }
        }

        /// <summary>
        /// Once C inputs have been determined for all crops, this method will check if there are one or more crops grown in the same year. If there is, the total C
        /// inputs from the crops being grown in the same year will be combined and assigned to the main crop for that year since it is the inputs from the main crop
        /// that are used in the C models
        /// </summary>
        public void CombineCarbonInputs(
            Farm farm,
            List<CropViewItem> viewItems)
        {
            var distinctYears = viewItems.GetDistinctYears();
            foreach (var year in distinctYears)
            {
                var viewItemsForYear = viewItems.GetItemsByYear(year);
                var mainCropForYear = _fieldComponentHelper.GetMainCropForYear(viewItemsForYear, year);
                var secondaryCropsForYear = viewItemsForYear.GetSecondaryCrops(mainCropForYear);

                var totalAbovegroundCarbonFromCoverCrops = this.SumTotalAbovegroundCarbonInput(secondaryCropsForYear);
                var totalBelowgroundCarbonFromCoverCrops = this.SumTotalBelowgroundCarbonInput(secondaryCropsForYear);
                var totalManureCarbonFromCoverCrops = this.SumTotalManureCarbonInput(secondaryCropsForYear);
                var totalDigestateCarbonFromCoverCrops = this.SumTotalDigestateCarbonInput(secondaryCropsForYear);

                /*
                 * Sum up the main crop and cover crop C inputs
                 */

                mainCropForYear.CombinedAboveGroundInput = mainCropForYear.AboveGroundCarbonInput + totalAbovegroundCarbonFromCoverCrops;
                mainCropForYear.CombinedBelowGroundInput = mainCropForYear.BelowGroundCarbonInput + totalBelowgroundCarbonFromCoverCrops;
                mainCropForYear.CombinedManureInput = mainCropForYear.ManureCarbonInputsPerHectare + totalManureCarbonFromCoverCrops;
                mainCropForYear.CombinedDigestateInput = mainCropForYear.DigestateCarbonInputsPerHectare + totalDigestateCarbonFromCoverCrops;
                mainCropForYear.TotalCarbonInputs = mainCropForYear.CombinedAboveGroundInput + mainCropForYear.CombinedBelowGroundInput + mainCropForYear.CombinedManureInput + mainCropForYear.CombinedDigestateInput;
            }
        }

        #endregion

        #region Private Methods

        public Tuple<double, double> CalculateUptakeByGrazingAnimals(List<ManagementPeriod> managementPeriods, CropViewItem viewItem, AnimalGroup animalGroup, FieldSystemComponent fieldSystemComponent, Farm farm, AnimalComponentBase animalComponent)
        {
            if (managementPeriods.Any() == false)
            {
                return new Tuple<double, double>(0, 0);
            }

            if (managementPeriods.Count == 1)
            {
                // Equation 11.3.2-4
                // Equation 11.3.2-5

                /*
                 * Note: value is reduced by area in Equation 11.3.2-9
                 */

                var resultsForManagementPeriod = _animalService.GetResultsForManagementPeriod(animalGroup, farm, animalComponent, managementPeriods.Single());
                var totalCarbonUptake = resultsForManagementPeriod.TotalCarbonUptakeByAnimals();
                var averageUtilizationRate = viewItem.GrazingViewItems.Any() ? viewItem.GrazingViewItems.Average(x => x.Utilization) : 0;
                var utilizationRate = (averageUtilizationRate / 100.0);
                if (utilizationRate <= 0)
                {
                    utilizationRate = 1;
                }

                var result = totalCarbonUptake / utilizationRate;

                return new Tuple<double, double>(result, totalCarbonUptake);
            }
            else
            {
                var totalCarbonUptake = 0d;
                var averageUtilizationRate = viewItem.GrazingViewItems.Any() ? viewItem.GrazingViewItems.Average(x => x.Utilization) : 0;

                var lastPeriod = managementPeriods.OrderBy(x => x.Start).Last();
                var resultsForLastManagementPeriod = _animalService.GetResultsForManagementPeriod(animalGroup, farm, animalComponent, lastPeriod);
                var totalCarbonUptakeForLastPeriod = resultsForLastManagementPeriod.TotalCarbonUptakeByAnimals();
                totalCarbonUptake += totalCarbonUptakeForLastPeriod;
                var denominator = averageUtilizationRate / 100;
                if (denominator <= 0)
                {
                    denominator = 1;
                }

                var carbonUptakeForLastPeriod = totalCarbonUptakeForLastPeriod / denominator;

                // Equation 11.3.2-6
                var carbonUpdateForOtherPeriods = 0d;
                for (int i = 0; i < managementPeriods.Count - 1; i++)
                {
                    var managementPeriod = managementPeriods[i];
                    var resultsForPeriod = _animalService.GetResultsForManagementPeriod(animalGroup, farm, animalComponent, managementPeriod);
                    var totalCarbonUptakeForPeriod = resultsForPeriod.TotalCarbonUptakeByAnimals();
                    totalCarbonUptake += totalCarbonUptakeForPeriod;

                    var carbonUptakeForPeriod = totalCarbonUptakeForPeriod;
                    carbonUpdateForOtherPeriods += carbonUptakeForPeriod;
                }

                // Equation 11.3.2-7
                var result = (carbonUpdateForOtherPeriods + carbonUptakeForLastPeriod);

                return new Tuple<double, double>(result, totalCarbonUptake);
            }
        }

        public void ProcessCommandLineItems(List<CropViewItem> viewItems, Farm farm,
            List<AnimalComponentEmissionsResults> animalResults)
        {
            if (farm.IsCommandLineMode == false)
            {
                return;
            }

            for (int i = 0; i < viewItems.Count; i++)
            {
                var itemAtIndex = viewItems.ElementAt(i);
                var year = itemAtIndex.Year;
                var tupleForYear = _fieldComponentHelper.GetAdjoiningYears(viewItems, year);
                var currentYearViewItem = tupleForYear.CurrentYearViewItem;

                if (currentYearViewItem.Yield == 0)
                {
                    _cropInitializationService.InitializeYield(currentYearViewItem, farm);
                }
                
                if (currentYearViewItem.LigninContent == 0)
                {
                    _cropInitializationService.InitializeLigninContent(currentYearViewItem, farm);
                }

                if (currentYearViewItem.MoistureContentOfCropPercentage == 0)
                {
                    _cropInitializationService.InitializeMoistureContent(currentYearViewItem, farm);
                }

                /*
                 * If the CLI user has not entered a value for aboveground or belowground crop residue inputs, we will need to assign C inputs now before the C model runs. The user
                 * can specify that the CLI should recalculate inputs by setting these two columns to 0. C models can't run without these two inputs (as a minimum) so the must be set
                 * to non-zero values before running the C models
                 */

                var aboveGroundResidueCarbon = currentYearViewItem.AboveGroundCarbonInput;
                var belowGroundResidueCarbon = currentYearViewItem.BelowGroundCarbonInput;

                var cropResidueInputsNeedRecalculating = aboveGroundResidueCarbon == 0 && belowGroundResidueCarbon == 0;
                
                 if (cropResidueInputsNeedRecalculating)
                {
                    // If biomass fractions are 0, assign defaults
                    if (currentYearViewItem.BiomassCoefficientProduct == 0)
                    {
                        _cropInitializationService.InitializeBiomassCoefficientProduct(currentYearViewItem, farm);
                    }

                    if (currentYearViewItem.BiomassCoefficientStraw == 0)
                    {
                        _cropInitializationService.InitializeBiomassCoefficientStraw(currentYearViewItem, farm);
                    }

                    if (currentYearViewItem.BiomassCoefficientRoots == 0)
                    {
                        _cropInitializationService.InitializeBiomassCoefficientRoots(currentYearViewItem, farm);
                    }

                    if (currentYearViewItem.BiomassCoefficientExtraroot == 0)
                    {
                        _cropInitializationService.InitializeBiomassCoefficientExtraroots(currentYearViewItem, farm);
                    }

                    // Recalculate residue inputs
                    this.AssignInputsAndLosses(tupleForYear, farm, animalResults);
                }

                var needsManureInputsRecalculated = (currentYearViewItem.ManureCarbonInputsPerHectare == 0) && (currentYearViewItem.ManureApplicationViewItems.Count > 0);
                if (needsManureInputsRecalculated)
                {
                    var residueInputMethod = farm.Defaults.ResidueInputCalculationMethod;

                    if (residueInputMethod == ResidueInputCalculationMethod.ICBM)
                    {
                        _icbmCarbonInputCalculator.AssignManureInputs(tupleForYear.PreviousYearViewItem, tupleForYear.CurrentYearViewItem, currentYearViewItem, farm, animalResults);

                    }
                    else
                    {
                        _ipccTier2CarbonInputCalculator.AssignManureCarbonInputs(currentYearViewItem, farm, animalResults);
                    }

                    currentYearViewItem.TotalCarbonInputs += currentYearViewItem.ManureCarbonInputsPerHectare;
                }

                var digestateCarbonInput = currentYearViewItem.DigestateCarbonInputsPerHectare;
            }
        }

        #endregion
    }
}