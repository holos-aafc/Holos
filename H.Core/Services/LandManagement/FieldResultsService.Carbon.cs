using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using AutoMapper.Configuration.Annotations;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
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
                var perennialItemsInStand = viewItems.Where(x =>
                    x.CropType.IsPerennial() &&
                    x.PerennialStandGroupId.Equals(mainCropForCurrentYear.PerennialStandGroupId));
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
            _initializationService.InitializeYieldForAllYears(
                cropViewItems: viewItems,
                farm: farm, fieldSystemComponent: fieldSystemComponent);

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

                _carbonService.CalculateInputsAndLosses(previousYearViewItem, currentYearViewItem, nextYearViewItem, farm);

                // Although climate/management factors are not used in the Tier 2 carbon modelling, they are used in the N budget and so must be calculated when user specifies Tier 2 or ICBM modelling
                currentYearViewItem.ClimateParameter = _climateService.CalculateClimateParameter(currentYearViewItem, farm);
                currentYearViewItem.TillageFactor = this.CalculateTillageFactor(currentYearViewItem, farm);
                currentYearViewItem.ManagementFactor = this.CalculateManagementFactor(currentYearViewItem.ClimateParameter, currentYearViewItem.TillageFactor);
            }

            // Consider the secondary crops
            var secondaryCrops = viewItems.OrderBy(x => x.Year).Where(x => x.IsSecondaryCrop).ToList();
            foreach (var secondaryCrop in secondaryCrops)
            {
                _carbonService.CalculateInputsAndLosses(null, secondaryCrop, null, farm);
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
                        cropViewItem.PercentageOfProductYieldReturnedToSoil =
                            100; // Now C inputs will be calculated correctly for this year
                    }
                }
            }
        }

        #endregion

        #region Private Methods

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

        private FieldSystemComponent GetLeftMostComponent(FieldSystemComponent fieldSystemComponent, Farm farm)
        {
            var currentFieldComponent = new FieldSystemComponent();

            var currentComponentId = fieldSystemComponent.CurrentPeriodComponentGuid;
            if (currentComponentId.Equals(Guid.Empty))
            {
                currentFieldComponent = fieldSystemComponent;
            }
            else
            {
                currentFieldComponent = farm.GetFieldSystemComponent(currentComponentId);
            }

            if (currentFieldComponent.HistoricalComponents.Any())
            {
                return currentFieldComponent.HistoricalComponents.Cast<FieldSystemComponent>().OrderBy(x => x.StartYear)
                    .First();
            }
            else
            {
                return currentFieldComponent;
            }
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

            // Need to get leftmost component here
            var leftMost = this.GetLeftMostComponent(fieldSystemComponent, farm);

            // Create run in period items
            var runInPeriodItems = this.GetRunInPeriodItems(farm, leftMost.CropViewItems, leftMost.StartYear,
                viewItemsForField, leftMost);

            _initializationService.InitializeYieldForAllYears(runInPeriodItems, farm, leftMost);

            // Check if user specified ICBM or Tier 2 carbon modelling
            if (farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2)
            {
                _tier2SoilCarbonCalculator.AnimalComponentEmissionsResults = this.AnimalResults;

                foreach (var runInPeriodItem in runInPeriodItems)
                {
                    _carbonService.CalculateInputsAndLosses(null, runInPeriodItem, null, farm);
                }

                // Combine inputs now that we have C set on cover crops
                this.CombineInputsForAllCropsInSameYear(runInPeriodItems, leftMost);

                // Merge all run in period items
                var mergedRunInItems = this.MergeDetailViewItems(runInPeriodItems, leftMost);

                // Combine inputs for run in period
                this.CombineInputsForAllCropsInSameYear(mergedRunInItems, leftMost);

                _tier2SoilCarbonCalculator.CalculateResults(
                    farm: farm,
                    viewItemsByField: viewItemsForField,
                    fieldSystemComponent: leftMost,
                    runInPeriodItems: mergedRunInItems);
            }
            else
            {
                _icbmSoilCarbonCalculator.AnimalComponentEmissionsResults = this.AnimalResults;

                // Create the item with the steady state (equilibrium) values
                var equilibriumYearResults = _icbmSoilCarbonCalculator.CalculateEquilibriumYear(viewItemsForField, farm, fieldSystemGuid);

                for (int i = 0; i < viewItemsForField.Count; i++)
                {
                    var currentYearResults = viewItemsForField.ElementAt(i);

                    // Get previous year results, if there is no previous year (i.e. t = 0), then use equilibrium (or custom measured) values for the pools
                    var previousYearResults = i == 0 ? equilibriumYearResults : viewItemsForField.ElementAt(i - 1);

                    // Carbon must be calculated before nitrogen
                    _icbmSoilCarbonCalculator.CalculateCarbonAtInterval(
                        previousYearResults: previousYearResults,
                        currentYearResults: currentYearResults,
                        farm: farm);

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

        #endregion
    }
}