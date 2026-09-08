using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        #region Public Methods

        public void CalculateFactors(AdjoiningYears adjoiningYears, Farm farm)
        {
            var viewItem = adjoiningYears.CurrentYearViewItem;

            this.CalculateFactors(viewItem, farm);
        }

        public void CalculateFactors(List<CropViewItem> viewItems, Farm farm)
        {
            foreach (var cropViewItem in viewItems)
            {
                this.CalculateFactors(cropViewItem, farm);
            }
        }

        /// <summary>
        /// Although climate/management factors are not used in the Tier 2 carbon modelling, they are used in the N budget and so must be calculated when user specifies Tier 2 or ICBM modelling
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
        /// Before carbon change can be calculated, all view items must have yields assigned so that we can determine the total carbon inputs from all crops, manure applications, supplemental 
        /// hay applications, etc. Then we can proceed to the actual carbon change calculations.
        /// </summary>
        public void AssignCarbonInputs(IEnumerable<CropViewItem> viewItems,
            Farm farm)
        {
            var mainCrops = viewItems.GetMainCrops();
            var secondaryCrops = viewItems.GetSecondaryCrops();

            var animalResults = _animalService.GetAnimalResults(farm);

            _carbonService.AssignInputsAndLosses(mainCrops, farm, animalResults);
            _carbonService.AssignInputsAndLosses(secondaryCrops, farm, animalResults);

            this.CalculateFactors(mainCrops, farm);
        }

        public void AssignNitrogenInputs(List<CropViewItem> viewItems, Farm farm)
        {
            var mainCrops = viewItems.GetMainCrops();
            var secondaryCrops = viewItems.GetSecondaryCrops();

            _nitrogenService.AssignNitrogenInputs(mainCrops, farm);
            _nitrogenService.AssignNitrogenInputs(secondaryCrops, farm);
        }

        /// <summary>
        /// When the yield assignment method is Custom, a perennial hay/forage field's yield is taken from the biomass the
        /// user actually harvested rather than a separately-typed or modelled value. This makes the entered harvest the
        /// single source of truth - it drives C_p and everything derived from it. Non-Custom (modelled) methods keep
        /// their estimated yield; grain crops and fields without hayed harvests are left untouched.
        ///
        /// The two biomass figures are on different moisture bases and must be reconciled: a bale has been dried (around
        /// 15% moisture) while Yield means the standing crop as it grew (around 80% for forage), and C_p multiplies Yield
        /// by 1 - the CROP's moisture. Feeding the bale's wet weight in directly therefore strips ~80% of the mass off
        /// material holding only ~15% water, understating the field's carbon several-fold. So we take the harvest's dry
        /// matter and re-express it on the crop's basis:
        ///
        ///     Yield = Σ(hayed harvest dry matter) / (1 - crop moisture) / area
        ///
        /// which leaves Yield x (1 - crop moisture) equal to the dry matter actually baled off, as the pipeline expects.
        ///
        /// Grazed fields are excluded. Under a custom yield with grazing animals present, the algorithm document takes
        /// the entered yield to be the total aboveground biomass produced - what the animals ate plus what they left -
        /// which is why the harvest-loss gross-up is suppressed for that case (note under Eq. 2.1.2-1). Deriving the
        /// yield from the baled hay alone would discard the grazed portion and understate the field's production.
        ///
        /// Must run after yields are assigned and before carbon inputs are calculated.
        /// </summary>
        public void UpdateYieldFromHarvestForCustomPerennials(IEnumerable<CropViewItem> viewItems, Farm farm, FieldSystemComponent fieldSystemComponent)
        {
            if (farm == null || farm.GetYieldAssignmentMethod(fieldSystemComponent) != YieldAssignmentMethod.Custom)
            {
                return;
            }

            foreach (var cropViewItem in viewItems)
            {
                if (cropViewItem.DoNotRecalculateYield ||
                    cropViewItem.CropType.IsPerennial() == false ||
                    cropViewItem.Area <= 0 ||
                    cropViewItem.HasGrazingItemsForTheCurrentYear())
                {
                    // A manually-set (frozen) yield is not overwritten by the harvest derivation, and a grazed field's
                    // yield already represents the total biomass grown, not just the part that was baled.
                    continue;
                }

                // The derivation itself lives on the view item so the component selection screen and this pipeline
                // cannot drift apart; everything above is the policy about when it may run.
                cropViewItem.CalculateYieldFromHayHarvests();
            }
        }

        /// <summary>
        /// Sets a perennial's "percentage of product returned to soil" from the field's harvest / grazing state, per the
        /// algorithm document's perennial handling:
        ///
        /// - No harvest and no grazing: nothing is removed from the field, so all of the product stays and the return is
        ///   100% (rather than the 35% perennial default). A zero yield is one instance of this, but a modelled or custom
        ///   non-zero yield with no harvest and no grazing is equally an all-returned year, so we key off the harvest /
        ///   grazing state directly rather than only off a zero yield.
        /// - Hayed (not grazed): the user-entered "Harvest loss %" on the hayed harvest(s) drives the return, since harvest
        ///   loss is exactly the fraction of product left on the field. Only hayed harvests are wired here - silage / swath
        ///   harvests keep their own established returns (their harvest-loss default is a placeholder that does not
        ///   represent a returned-to-soil fraction).
        /// - Grazed: what remains is what the animals did not eat, so the return is 1 - utilization (note under
        ///   Eq. 2.1.2-20). This applies whether or not the field was also hayed, because the utilization already
        ///   describes the whole standing crop.
        ///
        /// This method has to be called after we assign yields.
        /// </summary>
        public void UpdatePercentageReturnsForPerennials(IEnumerable<CropViewItem> viewItems)
        {
            foreach (var cropViewItem in viewItems)
            {
                if (cropViewItem.CropType.IsPerennial() == false ||
                    cropViewItem.DoNotRecalculatePercentageReturnedToSoil)
                {
                    // A manually-set (frozen) percentage returned to soil is not overwritten.
                    continue;
                }

                var isHarvested = cropViewItem.IsHarvested();
                var isGrazed = cropViewItem.HasGrazingItemsForTheCurrentYear();

                if (cropViewItem.Yield == 0 || (isHarvested == false && isGrazed == false))
                {
                    cropViewItem.PercentageOfProductYieldReturnedToSoil =
                        100; // Nothing removes product from the field this year, so all of it stays
                }
                else if (isGrazed == false)
                {
                    // Harvested but not grazed: let the hayed harvest's "Harvest loss %" (the fraction left on the field)
                    // set how much product is returned to soil. A non-positive result means there were no hayed harvests
                    // or their loss values were uninitialized, so we leave the existing (default) return in place.
                    var harvestLoss = cropViewItem.GetHayedHarvestLossPercentage();
                    if (harvestLoss > 0)
                    {
                        cropViewItem.PercentageOfProductYieldReturnedToSoil = harvestLoss;
                    }
                }
                else
                {
                    // Grazed: what stays on the field is what the animals did not eat, i.e. 1 - utilization (note under
                    // Eq. 2.1.2-20). The carbon calculator derived this privately and never wrote it back, so the value
                    // shown on the details screen stayed at the crop-type default and disagreed with the calculation.
                    // A non-positive utilization means the grazing items are uninitialized, so the default is kept.
                    var utilization = GetAverageUtilizationForTheYear(cropViewItem);
                    if (utilization > 0)
                    {
                        cropViewItem.PercentageOfProductYieldReturnedToSoil = 100 - utilization;
                    }
                }
            }
        }

        /// <summary>
        /// The average grazing utilization rate for the view item's own year. Scoped to the year to match
        /// <see cref="CropViewItem.HasGrazingItemsForTheCurrentYear"/>, which decides whether the grazed branch applies -
        /// <see cref="CropViewItem.GetAverageUtilizationFromGrazingAnimals"/> averages every grazing item regardless of year.
        /// </summary>
        private static double GetAverageUtilizationForTheYear(CropViewItem cropViewItem)
        {
            var grazingForTheYear = cropViewItem.GrazingViewItems
                .Where(grazing => grazing.Start.Year == cropViewItem.Year)
                .ToList();

            return grazingForTheYear.Any() ? grazingForTheYear.Average(grazing => grazing.Utilization) : 0;
        }

        /// <summary>
        /// Combines the "Harvest loss %" of a perennial's hayed harvests for the year into a single percentage of product
        /// returned to soil. Cuts are weighted by the biomass they removed
        /// (<see cref="HarvestViewItem.AboveGroundBiomassDryWeight"/>) so a larger cut dominates the field's overall
        /// returned fraction; falls back to a simple mean when biomass weights are unavailable. Returns 0 when there are
        /// no hayed harvests (silage / swath harvests are excluded), signalling the caller to leave the existing return.
        /// </summary>

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

                /*
                 * Process run in period items
                 */

                _carbonService.AssignInputsAndLosses(runInPeriodItems, farm, this.AnimalResults);
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

                    _carbonService.ProcessCommandLineItems(viewItemsForField.ToList(), farm, this.AnimalResults);
                    _nitrogenService.ProcessCommandLineItems(viewItemsForField.ToList(), farm);
                    this.CalculateFactors(viewItemsForField.ToList(), farm);
                    this.CombineInputsForAllCropsInSameYear(farm, viewItemsForField.ToList());
                }

                // Combine inputs now that we have C and N inputs set for all items
                this.CombineInputsForAllCropsInSameYear(farm, runInPeriodItems);

                // Merge all run in period items
                var mergedRunInItems = this.MergeDetailViewItems(runInPeriodItems, leftMost);

                // Combine inputs for run in period
                this.CombineInputsForAllCropsInSameYear(farm, mergedRunInItems);

                _tier2SoilCarbonCalculator.CalculateResults(
                    farm: farm,
                    viewItemsByField: viewItemsForField,
                    fieldSystemComponent: leftMost,
                    runInPeriodItems: mergedRunInItems);
            }
            else
            {
                _icbmSoilCarbonCalculator.AnimalComponentEmissionsResults = this.AnimalResults;

                 if (farm.IsCommandLineMode)
                {
                    /*
                     * When in GUI mode, the inputs for the main view items will already have been assigned at AssignCarbonInputs() and AssignNitrogenInputs().
                     * When in CLI mode, we need to check if there are missing values and process any missing input values before calculating final results
                     */

                    _carbonService.ProcessCommandLineItems(viewItemsForField.ToList(), farm, this.AnimalResults);
                    _nitrogenService.ProcessCommandLineItems(viewItemsForField.ToList(), farm);
                    this.CalculateFactors(viewItemsForField.ToList(), farm);
                    this.CombineInputsForAllCropsInSameYear(farm, viewItemsForField.ToList());
                }

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