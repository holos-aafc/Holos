using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using System.Collections.Generic;
using System.Linq;
using H.Core.Services.Animals;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using System;
using System.Runtime.CompilerServices;
using H.Core.Providers.AnaerobicDigestion;

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

        #endregion

        #region Constructors

        public CarbonService()
        {
            _ipccTier2CarbonInputCalculator = new IPCCTier2CarbonInputCalculator();
            _icbmCarbonInputCalculator = new ICBMCarbonInputCalculator();
            _animalService = new AnimalResultsService();
        }

        #endregion

        #region Public Methods

        public bool CanCalculateInputsUsingIpccTier2(CropViewItem cropViewItem)
        {
            return _ipccTier2CarbonInputCalculator.CanCalculateInputsForCrop(cropViewItem);
        }

        public void CalculateInputsAndLosses(CropViewItem previousYear, CropViewItem viewItem, CropViewItem nextYear, Farm farm)
        {
            if (this.CanCalculateInputsUsingIpccTier2(viewItem))
            {
                _ipccTier2CarbonInputCalculator.CalculateInputs(viewItem, farm);
            }
            else
            {
                _icbmCarbonInputCalculator.CalculateInputs(previousYear, viewItem, nextYear, farm);
            }

            this.CalculateLosses(viewItem, farm);
        }

        public void CalculateLosses(CropViewItem cropViewItem, Farm farm)
        {
            this.CalculateCarbonLostFromHayExports(farm, cropViewItem);
        }

        /// <summary>
        /// (kg C ha^-1)
        /// </summary>
        public double CalculateManureCarbonInputFromGrazingAnimals(
            FieldSystemComponent fieldSystemComponent,
            CropViewItem cropViewItem,
            List<AnimalComponentEmissionsResults> results)
        {
            var result = 0d;

            var grazingViewItems = fieldSystemComponent.CropViewItems.Where(y => y.CropType == cropViewItem.CropType).SelectMany(x => x.GrazingViewItems).ToList();

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
        public void CalculateManureCarbonInputByGrazingAnimals(
            FieldSystemComponent fieldSystemComponent,
            IEnumerable<AnimalComponentEmissionsResults> results,
            List<CropViewItem> cropViewItems)
        {
            foreach (var cropViewItem in cropViewItems)
            {
                cropViewItem.TotalCarbonInputFromManureFromAnimalsGrazingOnPasture = this.CalculateManureCarbonInputFromGrazingAnimals(fieldSystemComponent, cropViewItem, results.ToList());
            }
        }

        public double CalculateInputsFromSupplementalHayFedToGrazingAnimals(CropViewItem previousYearViewItem,
            CropViewItem currentYearViewItem, CropViewItem nextYearViewItems, Farm farm)
        {
            return _icbmCarbonInputCalculator.CalculateInputsFromSupplementalHayFedToGrazingAnimals(previousYearViewItem, currentYearViewItem, nextYearViewItems, farm);
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

                cropViewItem.TotalCarbonLossesByGrazingAnimals = totalCarbonLossesForField;
                cropViewItem.TotalCarbonUptakeByAnimals = totalCarbonUptakeForField;
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

                var result = (carbonUpdateForOtherPeriods + carbonUptakeForLastPeriod);

                return new Tuple<double, double>(result, totalCarbonUptake);
            }
        }

        #endregion
    }
}