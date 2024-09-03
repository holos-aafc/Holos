using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using System.Collections.Generic;
using System.Linq;
using H.Core.Services.Animals;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using System;

namespace H.Core.Calculators.Carbon
{
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

        public void CalculateInputs(CropViewItem previousYear, CropViewItem viewItem, CropViewItem nextYear, Farm farm)
        {
            if (_ipccTier2CarbonInputCalculator.CanCalculateInputsForCrop(viewItem))
            {
                _ipccTier2CarbonInputCalculator.CalculateInputsForCrop(viewItem, farm);
            }
            else
            {
                _icbmCarbonInputCalculator.SetCarbonInputs(previousYear, viewItem, nextYear, farm);
            }
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
        public void CalculateCarbonLostFromHayExports(
            FieldSystemComponent fieldSystemComponent,
            Farm farm)
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
                            foreach (var hayImportViewItem in viewItem.HayImportViewItems.Where(x =>
                                         x.FieldSourceGuid.Equals(fieldSystemComponent.Guid) &&
                                         x.Date.Year == cropViewItem.Year))
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
                            totalCarbonInExportedBales: totalCarbonInExportedBales);
                    }
                }
            }
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

        /// <summary>
        /// Equation 12.3.2-4
        /// </summary>
        /// <param name="percentageOfProductReturnedToSoil">Amount of product yield returned to soil (%)</param>
        /// <param name="totalCarbonInExportedBales">Total amount of carbon in bales (kg C)</param>
        /// <returns>Total amount of carbon lost from exported bales (kg C)</returns>
        private double CalculateTotalCarbonLossFromBaleExports(
            double percentageOfProductReturnedToSoil,
            double totalCarbonInExportedBales)
        {
            var result = totalCarbonInExportedBales / (1 - (percentageOfProductReturnedToSoil / 100.0));

            return result;
        }

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