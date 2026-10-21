using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Services.LandManagement
{
    public partial class FieldResultsService
    {
        #region Public Methods

        /// <summary>
        /// Map properties from the component selection screen view item to the detail screen view item.
        /// </summary>
        /// <param name="viewItem">The <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/> to copy from.</param>
        /// <param name="year">When creating historical view items, this value will be used to set the year of any associated <see cref="ManureApplicationViewItem"/>s etc.</param>
        /// <returns>A duplicated <see cref="H.Core.Models.LandManagement.Fields.CropViewItem"/></returns>
        public CropViewItem MapDetailsScreenViewItemFromComponentScreenViewItem(CropViewItem viewItem, int year)
        {
            var result = _detailViewItemMapper.Map(viewItem);

            // Copy all manure applications to the detail view item
            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                if (BelongsToAnotherYearOnly(manureApplicationViewItem, manureApplicationViewItem.DateOfApplication.Year, year))
                {
                    continue;
                }

                var copiedManureApplicationViewItem = _manureApplicationViewItemMapper.Map(manureApplicationViewItem);

                // We need to update the year so that the current years' manure applications are copied back in time
                copiedManureApplicationViewItem.DateOfApplication = new DateTime(year, manureApplicationViewItem.DateOfApplication.Month, manureApplicationViewItem.DateOfApplication.Day);

                result.ManureApplicationViewItems.Add(copiedManureApplicationViewItem);
            }

            foreach (var harvestViewItem in viewItem.HarvestViewItems)
            {
                if (BelongsToAnotherYearOnly(harvestViewItem, harvestViewItem.Start.Year, year))
                {
                    continue;
                }

                var copiedHarvestViewItem = _harvestViewItemMapper.Map(harvestViewItem);

                // We need to update the year so that the current years' harvest items are copied back in time. The year
                // has to land on Start (and End), not only DateCreated: GetHayHarvestsByYear filters on Start.Year, so
                // stamping DateCreated alone left every copy matching the year the harvest was entered and no other,
                // which defeated the copy. Repeating the management across the simulation is what the algorithm document
                // describes - the historical period is built from the rotation the user specifies once.
                copiedHarvestViewItem.DateCreated = new DateTime(year, harvestViewItem.DateCreated.Month, harvestViewItem.DateCreated.Day);
                copiedHarvestViewItem.Start = new DateTime(year, harvestViewItem.Start.Month, harvestViewItem.Start.Day);
                copiedHarvestViewItem.End = new DateTime(year, harvestViewItem.End.Month, harvestViewItem.End.Day);

                result.HarvestViewItems.Add(copiedHarvestViewItem);
            }

            foreach (var grazingViewItem in viewItem.GrazingViewItems)
            {
                var copiedGrazingViewItem = _grazingViewItemMapper.Map(grazingViewItem);

                // We need to update the year so that the current years' harvest items are copied back in time
                copiedGrazingViewItem.DateCreated = new DateTime(year, grazingViewItem.DateCreated.Month, grazingViewItem.DateCreated.Day);

                result.GrazingViewItems.Add(copiedGrazingViewItem);
            }

            foreach (var hayImportViewItem in viewItem.HayImportViewItems)
            {
                if (BelongsToAnotherYearOnly(hayImportViewItem, hayImportViewItem.Date.Year, year))
                {
                    continue;
                }

                var copiedHayImportViewItem = _hayImportViewItemMapper.Map(hayImportViewItem);

                // We need to update the year so that the current years' hay import items are copied back in time
                copiedHayImportViewItem.Date = new DateTime(year, copiedHayImportViewItem.DateCreated.Month, copiedHayImportViewItem.DateCreated.Day);

                result.HayImportViewItems.Add(copiedHayImportViewItem);
            }

            foreach (var fertilizerApplicationViewItem in viewItem.FertilizerApplicationViewItems)
            {
                if (BelongsToAnotherYearOnly(fertilizerApplicationViewItem, viewItem.Year, year))
                {
                    continue;
                }

                var copiedFertilizerViewItem = _fertilizerViewItemMapper.Map(fertilizerApplicationViewItem);

                // We need to update the year so that the current years' fertilizer applications are copied back in time
                copiedFertilizerViewItem.DateCreated = new DateTime(year, fertilizerApplicationViewItem.DateCreated.Month, fertilizerApplicationViewItem.DateCreated.Day);

                result.FertilizerApplicationViewItems.Add(copiedFertilizerViewItem);
            }

            foreach (var digestateApplicationViewItem in viewItem.DigestateApplicationViewItems)
            {
                if (BelongsToAnotherYearOnly(digestateApplicationViewItem, viewItem.Year, year))
                {
                    continue;
                }

                var copiedDigestateViewItem = _digestateViewItemMapper.Map(digestateApplicationViewItem);

                // We need to update the year so that the current years' digestate applications are copied back in time
                copiedDigestateViewItem.DateCreated = new DateTime(year, digestateApplicationViewItem.DateCreated.Month, digestateApplicationViewItem.DateCreated.Day);

                result.DigestateApplicationViewItems.Add(copiedDigestateViewItem);
            }

            // The N-P-K-S rates are cached on the crop view item and are what the nitrogen calculations read - the
            // fertilizer collection itself is not summed at calculation time. The mapper copies those cached rates, so
            // without this a year that was denied a copy of an application still carried its full rate. Only recompute
            // when the source actually has applications: the CLI sets NitrogenFertilizerRate directly, with no
            // application items behind it, and summing an empty collection would erase it.
            if (viewItem.FertilizerApplicationViewItems.Any())
            {
                result.UpdateApplicationRateTotals();
            }

            // Same shape again: these flags are cached fields kept up to date by the collections' change handlers, and
            // the mapper copies them. A year denied a copy adds nothing, so no handler runs and the flag stays true
            // beside an empty collection - which makes CalculateHarvest (Eq 11.4.4-1) sum an empty table and report no
            // harvest instead of falling back to the year's yield.
            result.HasHarvestViewItems = result.HarvestViewItems.Count > 0;
            result.HasManureApplicationViewItems = result.ManureApplicationViewItems.Count > 0;
            result.HasHayImportViewItems = result.HayImportViewItems.Count > 0;

            return result;
        }


        /// <summary>
        /// True when an entry is marked as happening in a single year and this is not that year, so no copy of it is
        /// made. An empty collection is exactly what "this did not happen that year" means to everything downstream.
        ///
        /// Repeating is the default, matching the algorithm document's model of a historical period built from the
        /// management the user specifies once.
        ///
        /// Callers pass the year the entry belongs to, which differs by type. Harvests, manure applications and hay
        /// imports carry a management date the user sets. Fertilizer and digestate applications do not - their
        /// DateCreated is a creation timestamp, not a date in the field - so they belong to the year of the crop item
        /// they were entered against.
        /// </summary>
        private static bool BelongsToAnotherYearOnly(object activity, int yearItWasEntered, int yearBeingCreated)
        {
            return activity is IRepeatableFieldActivity repeatable
                   && repeatable.RepeatsInEveryYear == false
                   && yearItWasEntered != yearBeingCreated;
        }

        public FieldSystemDetailsStageState GetStageState(Farm farm)
        {
            var stageState = farm.StageStates.OfType<FieldSystemDetailsStageState>().SingleOrDefault();
            if (stageState == null)
            {
                // No stage state created yet, so make a new one now and add to farm
                stageState = new FieldSystemDetailsStageState();
                farm.StageStates.Add(stageState);
            }

            /*
             * Don't initialize here since we only want to return view items so calculations can use values, initialization should take place in
             * details view model or CLI
             */

            return stageState;
        }

        /// <summary>
        /// Clears every manual override on the details screen and puts the three overridable inputs back to their
        /// calculated values, in place on the items the screen is bound to.
        ///
        /// The results pipeline cannot do this. CalculateFinalResults merges the details items into copies and
        /// calculates over those, so clearing the flags and recalculating leaves the screen showing the number the
        /// user typed - no longer marked as an override, and so indistinguishable from a calculated value, while being
        /// free to change at the next recalculation.
        ///
        /// Rebuilding the stage state would also do it, and would throw away every other input on the screen along
        /// with the overrides, so each row is restored in place instead: back to the crop it was created from on the
        /// component selection screen, which is the state a rebuild would start from, and then derived again.
        ///
        /// Restoring before deriving is what makes this complete. The derivation does not reach every row - it leaves
        /// a Custom field's yields alone, and only perennials have a percentage returned to soil derived for them - so
        /// deriving alone would leave those rows holding the number the user typed with the flag that protected it now
        /// cleared, which is the state this method exists to get out of.
        /// </summary>
        public void ResetOverriddenInputs(Farm farm)
        {
            var stageState = this.GetStageState(farm);
            if (stageState == null)
            {
                return;
            }

            foreach (var fieldSystemComponent in farm.FieldSystemComponents)
            {
                var managementPeriods = GetManagementPeriods(fieldSystemComponent);

                // A field's rows all carry the current period's guid - AssignInitialProperties assigns it so the
                // results screen can group a field's rows together - so the period a row belongs to has to be resolved
                // from the crop it was created from. Each period has its own rotation and the yield a row gets can
                // depend on it (an Average yield is the mean of its own period's crops), so the periods are derived
                // separately here, as they are when the screen is first built.
                var itemsByPeriod = stageState.DetailsScreenViewCropViewItems
                    .Where(viewItem => viewItem.FieldSystemComponentGuid == fieldSystemComponent.Guid)
                    .GroupBy(viewItem => GetManagementPeriodForItem(managementPeriods, viewItem) ?? fieldSystemComponent);

                foreach (var grouping in itemsByPeriod)
                {
                    var managementPeriod = grouping.Key;
                    var viewItems = grouping.ToList();

                    var moistureToPutBack = new List<BorrowedMoisture>();
                    var restoredItems = new List<CropViewItem>();

                    foreach (var viewItem in viewItems)
                    {
                        var componentSelectionScreenCrop = GetComponentSelectionScreenCropForItem(managementPeriod, viewItem);
                        if (componentSelectionScreenCrop == null)
                        {
                            // There is nothing to put this row back to - the crop it was created from is gone, or the
                            // row was built before a secondary crop's row recorded a resolvable one. Its override is
                            // left standing: clearing the flag would leave the user's number in the calculation with
                            // nothing saying it was theirs, which is the state this method exists to prevent. The row
                            // becomes resettable again once the screen is rebuilt.
                            continue;
                        }

                        // Restored before the flags are cleared, since which inputs are put back depends on which
                        // ones the user had frozen.
                        RestoreOverridableInputs(componentSelectionScreenCrop, viewItem);

                        // A hayed row's yield is its harvested dry matter scaled up by the standing crop's moisture,
                        // and the moisture the row carries afterwards is the hay's - PostProcessPerennials assigns it
                        // once the yield has been derived. Repeating the derivation against the row as it stands would
                        // scale by the hay's moisture and change the yield of a row nobody overrode, so the entered
                        // moisture is borrowed for the derivation and the row is put back to what it was showing. The
                        // moisture is not one of the inputs being reset, so it has to end where it started.
                        //
                        // Both forms are carried. The percentage writes through to the fraction only when its value
                        // changes, so setting one of them alone leaves the row holding two moistures that disagree,
                        // and the passes below do not all read the same one.
                        if (YieldInputPolicy.HasHayedHarvest(viewItem))
                        {
                            moistureToPutBack.Add(BorrowedMoisture.From(viewItem));

                            SetMoisture(
                                viewItem,
                                componentSelectionScreenCrop.MoistureContentOfCropPercentage,
                                componentSelectionScreenCrop.MoistureContentOfCrop);
                        }

                        ClearOverrideFlags(viewItem);

                        restoredItems.Add(viewItem);
                    }

                    // The same order the items were derived in when the screen was first built. Each step reads what
                    // the one before it wrote, and each already honours the flags, which are now clear.
                    _initializationService.InitializeYieldForAllYears(
                        cropViewItems: viewItems,
                        farm: farm,
                        fieldSystemComponent: managementPeriod);

                    this.UpdateYieldFromHarvest(viewItems);

                    this.UpdatePercentageReturnsForPerennials(viewItems: viewItems);

                    this.AssignCarbonInputs(viewItems: viewItems, farm: farm);

                    // The nitrogen in a crop's residues is derived from its carbon inputs, and in GUI mode the final
                    // results are calculated on the understanding that both passes have already run here - the command
                    // line is the only caller that reprocesses them later. Without this the results would combine
                    // restored carbon with the nitrogen derived from the discarded values.
                    this.AssignNitrogenInputs(viewItems: viewItems, farm: farm);

                    foreach (var borrowed in moistureToPutBack)
                    {
                        SetMoisture(borrowed.ViewItem, borrowed.Percentage, borrowed.Fraction);
                    }

                    // The carbon pass freezes a plant carbon it estimated for a later year itself (Equation 2.1.2-25),
                    // so a row can come out of the derivation carrying a flag the user never set. Clearing them again
                    // leaves the screen showing nothing as overridden, which is what the button says it does. Only the
                    // rows that were restored are touched - a row whose override had to be left standing keeps it.
                    foreach (var restoredItem in restoredItems)
                    {
                        ClearOverrideFlags(restoredItem);
                    }

                    // The crops the rows were created from are what a rebuilt screen starts from, so an override
                    // marker left on one of them would pin the value again on the new rows. Nothing writes these from
                    // the details screen, so this is a sweep rather than a correction - and it is done here rather
                    // than by the caller, which cannot clear them before this runs without hiding from it which
                    // inputs the user had frozen.
                    foreach (var crop in managementPeriod.CropViewItems.Concat(managementPeriod.CoverCrops))
                    {
                        ClearOverrideFlags(crop);
                    }
                }
            }
        }

        private static void ClearOverrideFlags(CropViewItem viewItem)
        {
            viewItem.DoNotRecalculateYield = false;
            viewItem.DoNotRecalculatePlantCarbonInAgriculturalProduct = false;
            viewItem.DoNotRecalculatePercentageReturnedToSoil = false;
        }

        /// <summary>
        /// Both forms of a row's crop moisture, in the one order that leaves them agreeing: the percentage first,
        /// since assigning it writes a converted value into the fraction, and then the fraction itself.
        /// </summary>
        private static void SetMoisture(CropViewItem viewItem, double percentage, double fraction)
        {
            viewItem.MoistureContentOfCropPercentage = percentage;
            viewItem.MoistureContentOfCrop = fraction;
        }

        /// <summary>
        /// A row's crop moisture, held while the reset derives a yield from a different one.
        /// </summary>
        private class BorrowedMoisture
        {
            public CropViewItem ViewItem { get; private set; }

            public double Percentage { get; private set; }

            public double Fraction { get; private set; }

            public static BorrowedMoisture From(CropViewItem viewItem)
            {
                return new BorrowedMoisture()
                {
                    ViewItem = viewItem,
                    Percentage = viewItem.MoistureContentOfCropPercentage,
                    Fraction = viewItem.MoistureContentOfCrop,
                };
            }
        }

        /// <summary>
        /// A field's management periods, oldest first: its historical period(s), the current one, then its projected
        /// period(s). The same order CreateDetailViewItems builds them in.
        /// </summary>
        private static List<FieldSystemComponent> GetManagementPeriods(FieldSystemComponent fieldSystemComponent)
        {
            var result = new List<FieldSystemComponent>();

            result.AddRange(fieldSystemComponent.HistoricalComponents.OfType<FieldSystemComponent>());
            result.Add(fieldSystemComponent);
            result.AddRange(fieldSystemComponent.ProjectedComponents.OfType<FieldSystemComponent>());

            return result;
        }

        /// <summary>
        /// The management period holding the component selection screen crop a details screen row was created from,
        /// or null when that crop can no longer be found - a row created before the user changed the rotation, say.
        /// </summary>
        private static FieldSystemComponent GetManagementPeriodForItem(
            IEnumerable<FieldSystemComponent> managementPeriods,
            CropViewItem viewItem)
        {
            return managementPeriods.FirstOrDefault(
                managementPeriod => GetComponentSelectionScreenCropForItem(managementPeriod, viewItem) != null);
        }

        /// <summary>
        /// The component selection screen crop a details screen row was created from. A row's main crop is held in
        /// CropViewItems and a secondary crop in CoverCrops, so both are searched.
        /// </summary>
        private static CropViewItem GetComponentSelectionScreenCropForItem(
            FieldSystemComponent managementPeriod,
            CropViewItem viewItem)
        {
            return managementPeriod.CropViewItems
                .Concat(managementPeriod.CoverCrops)
                .FirstOrDefault(crop => crop.Guid == viewItem.DetailViewItemToComponentSelectionViewItemMap);
        }

        /// <summary>
        /// Puts the inputs the user froze back to the values the details screen row was created with. The mapper that
        /// creates a row copies all three from the component selection screen crop, so these are what a rebuilt screen
        /// would start from.
        ///
        /// Only an input that was actually frozen is put back. A yield under the Custom method and an annual crop's
        /// percentage of product returned to soil are ordinary editable inputs outside advanced input editing
        /// (see YieldInputPolicy), so a row where the user typed one of those and overrode nothing is not this
        /// method's business - resetting another row's override must not discard it.
        ///
        /// Plant carbon is the exception and is restored on every row. It is never editable outside advanced input
        /// editing, so there is no ordinary input to protect, and the carbon pass needs it: a perennial establishment
        /// year reads the following year's plant carbon to estimate its own, and the pass works forwards through the
        /// years, so a discarded override left in place on a later row would be read as though it had been calculated
        /// before that row's turn to be recalculated came.
        /// </summary>
        private static void RestoreOverridableInputs(
            CropViewItem componentSelectionScreenCrop,
            CropViewItem viewItem)
        {
            if (viewItem.DoNotRecalculateYield)
            {
                viewItem.Yield = componentSelectionScreenCrop.Yield;
            }

            if (viewItem.DoNotRecalculatePercentageReturnedToSoil)
            {
                viewItem.PercentageOfProductYieldReturnedToSoil = componentSelectionScreenCrop.PercentageOfProductYieldReturnedToSoil;
            }

            viewItem.PlantCarbonInAgriculturalProduct = componentSelectionScreenCrop.PlantCarbonInAgriculturalProduct;
        }

        public void InitializeStageState(Farm farm)
        {
            var stageState = this.GetStageState(farm);

            // Clear existing items because we want to reset values for view items.
            stageState.ClearState();

            // Initialize the stage state (create view items that will be needed to create result view items)
            this.CreateDetailViewItems(farm);

            stageState.IsInitialized = true;
        }

        /// <summary>
        /// Create the detail view items for all fields on the farm. Created items will be added to the stage state for the farm.
        /// </summary>
        public void CreateDetailViewItems(Farm farm)
        {
            // Create view items for the current component, and each historical, and projected component(s)
            var components = farm.FieldSystemComponents;
            foreach (var component in components)
            {
                // Create view items any historical component(s)
                foreach (var historicalComponent in component.HistoricalComponents)
                {
                    this.CreateDetailViewItems(historicalComponent as FieldSystemComponent, farm);
                }

                // Create view items for the current component
                this.CreateDetailViewItems(component, farm);

                // Create view items any projected component(s)
                foreach (var projectedComponent in component.ProjectedComponents)
                {
                    this.CreateDetailViewItems(projectedComponent as FieldSystemComponent, farm);
                }
                
                this.PostProcessPerennials(component, farm);
            }
        }

        /// <summary>
        /// Combines C and N input for all items by year. Does not calculate inputs for individual items but assumes inputs have already been calculated and combines inputs for
        /// the main crop and any secondary crops
        /// </summary>
        public void CombineInputsForAllCropsInSameYear(Farm farm,
            IEnumerable<CropViewItem> viewItems)
        {
            _carbonService.CombineCarbonInputs(farm, viewItems.ToList());
            _nitrogenService.CombineNitrogenInputs(farm, viewItems.ToList());
        }

        /// <summary>
        /// Combine multiple view items for same year back into a single view item for that same year so ICBM works with one view item per year as expected.
        /// </summary>
        public List<CropViewItem> MergeDetailViewItems(
            IEnumerable<CropViewItem> viewItems, 
            FieldSystemComponent fieldSystemComponent)
        {
            var result = new List<CropViewItem>();

            var years = viewItems.Select(x => x.Year).Distinct().OrderBy(x => x);
            foreach (var year in years)
            {
                var itemsForYear = viewItems.Where(x => x.Year == year);

                // Get the main crop
                var mainCropForYear = _fieldComponentHelper.GetMainCropForYear(itemsForYear, year);

                // Copy the item
                var copiedItem = this.MapDetailsScreenViewItemFromComponentScreenViewItem(mainCropForYear, year);
                copiedItem.Name = mainCropForYear.Name;

                result.Add(copiedItem);
            }

            return result;
        }

        public void CreateDetailViewItems(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            Trace.TraceInformation($"{nameof(FieldResultsService)}.{nameof(CreateItems)}: creating details view items for field: '{fieldSystemComponent.Name}' and farm: '{farm.Name}'");

            // When called from the CLI, and the user specifies a path to a custom yield file, read in data now and assign yield data to the farm
            if (farm.IsCommandLineMode && farm.YieldAssignmentMethod == YieldAssignmentMethod.InputFile)
            {
                if (_customFileYieldProvider.HasExpectedInputFormat(farm.PathToYieldInputFile))
                {
                    farm.GeographicData.CustomYieldData = _customFileYieldProvider.GetYieldData(farm.PathToYieldInputFile);
                }
                else
                {
                    Trace.TraceError($"{nameof(FieldResultsService)}.{nameof(CreateItems)}: yield input file was not in the expected format. Using default yield assignment method instead");

                    // Fall back to custom since we don't want to overwrite users entered yields (custom enum will skip assignment of yields)
                    farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
                }
            }

            // Create a view item for each year (and also create additional items for each cover crop in the same year)
            var viewItems = this.CreateItems(fieldSystemComponent, farm).ToList();

            // Assign a default name, time period category, etc. to the view item
            this.AssignInitialProperties(fieldSystemComponent, viewItems, farm);

            // Once the view items have been created, additional properties that relate to perennials only need to be assigned (stand IDs, positional years, etc.)
            this.ProcessPerennials(viewItems, fieldSystemComponent);

            // Similarly with cover crops, the view items need to be adjusted
            _initializationService.InitializeCoverCrops(viewItems);

            // Add in a details view message for the undersown year(s). Note that perennials must be processed before this call
            this.ProcessUndersownCrops(viewItems, fieldSystemComponent);

            // Before creating view items for each year, calculate carbon uptake by grazing animals
            _carbonService.CalculateCarbonLostByGrazingAnimals(
                farm: farm,
                fieldSystemComponent: fieldSystemComponent,
                animalComponentEmissionsResults: this.AnimalResults, viewItems: viewItems);

            var stageState = this.GetStageState(farm);

            // Save the view items to the farm which can then be edited by the user on the details view
            stageState.DetailsScreenViewCropViewItems.AddRange(viewItems.OrderBy(x => x.Year).ThenBy(x => x.IsSecondaryCrop));
            
            // Before creating view items for each year, calculate carbon deposited from manure of animals grazing on pasture
            _carbonService.CalculateManureCarbonInputByGrazingAnimals(fieldSystemComponent, this.AnimalResults, viewItems, farm);

            _nitrogenService.CalculateManureNitrogenInputByGrazingAnimals(fieldSystemComponent, this.AnimalResults, viewItems);

            // Yields must be assigned to all items before we can loop over each year and calculate plant carbon in agricultural product (C_p)
            _initializationService.InitializeYieldForAllYears(
                cropViewItems: viewItems,
                farm: farm, fieldSystemComponent: fieldSystemComponent);

            // A perennial hay/forage field's entered harvest is its yield, under every yield assignment method
            this.UpdateYieldFromHarvest(viewItems);

            // After yields have been set, we must consider perennial years in which there is 0 for the yield input (from user or by default yield provider)
            this.UpdatePercentageReturnsForPerennials(
                viewItems: viewItems);

            // Assign C inputs for each view item
            this.AssignCarbonInputs(
                viewItems: viewItems,
                farm: farm);

            // Assign N inputs for each view item
            this.AssignNitrogenInputs(
                viewItems,
                farm);
        }

        /// <summary>
        /// Create ordered view items allowing for duplicated years (e.g. main crop and undersown/cover crop in the same year will result in 2 view items created). Considers
        /// the user's specified ordering of items (i.e. sequence entered by user begins at start year of field or sequence works backwards from end year of field)
        /// </summary>
        public IEnumerable<CropViewItem> CreateOrderedViewItems(
            FieldSystemComponent fieldSystemComponent,
            IEnumerable<CropViewItem> viewItems, 
            Farm farm)
        {
            var result = new List<CropViewItem>();

            // We use modulo to loop over items, if there are no items we must return so no division by zero occurs
            if (viewItems.Any() == false)
            {
                return result;
            }

            var startYear = fieldSystemComponent.StartYear;
            var endYear = fieldSystemComponent.EndYear;

            var moduloIndex = 0;

            // All the years in which the user specified a crop was grown
            var years = viewItems.Select(x => x.Year);

            // The years in which a crop was grown but in ascending order
            var orderedYears = years.OrderBy(x => x).ToList();

            // There may be crops that were grown in the same year (main crop + cover crop), but we still need to determine how many distinct years exist to build the sequence
            var distinctYears = orderedYears.Distinct().Count();

            if (fieldSystemComponent.BeginOrderingAtStartYearOfRotation)
            {
                // The earliest year entered by the user
                var firstEnteredYear = orderedYears.ElementAt(0);

                // If user specifies an ordering from the starting year of the field, create items working toward the end year of the field
                for (int indexYear = startYear; indexYear <= endYear; indexYear++, moduloIndex++)
                {
                    var moduloYear = firstEnteredYear + (moduloIndex % distinctYears);
                    var itemsAtModuloYear = viewItems.Where(x => x.Year == moduloYear);
                    foreach (var cropViewItem in itemsAtModuloYear)
                    {
                        var createdViewItem = this.MapDetailsScreenViewItemFromComponentScreenViewItem(cropViewItem, indexYear);
                        createdViewItem.Year = indexYear;
                        createdViewItem.DetailViewItemToComponentSelectionViewItemMap = cropViewItem.Guid;

                        result.Add(createdViewItem);
                    }
                }
            }
            else
            {
                // The latest year entered by the user
                var lastEnteredYear = orderedYears.Last();

                // If user specifies an ordering starting from the last year of the field, create items working back to the starting year of the field
                for (int indexYear = endYear; indexYear >= startYear; indexYear--, moduloIndex++)
                {
                    var moduloYear = lastEnteredYear - (moduloIndex % distinctYears);
                    var itemsAtYear = viewItems.Where(x => x.Year == moduloYear);
                    foreach (var cropViewItem in itemsAtYear)
                    {
                        var createdViewItem = this.MapDetailsScreenViewItemFromComponentScreenViewItem(cropViewItem, indexYear);
                        createdViewItem.Year = indexYear;
                        createdViewItem.DetailViewItemToComponentSelectionViewItemMap = cropViewItem.Guid;
                        result.Add(createdViewItem);
                    }
                }
            }

            return result.OrderBy(x => x.Year).ThenByDescending(x => x.CropType.IsAnnual()).ThenByDescending(x => x.CropType.IsCoverCrop()).ThenByDescending(x => x.CropType.IsPerennial());
        }

        /// <summary>
        /// Returns a collection of run in period items that are needed to calculate a starting state for the IPCC Tier 2 carbon model
        /// </summary>
        /// <param name="farm"></param>
        /// <param name="viewItemsForRotation">The crops that define a rotation</param>
        /// <param name="startYearOfField"></param>
        /// <param name="viewItemsForField">The view items for the entire field history</param>
        /// <param name="fieldComponent"></param>
        /// <returns>A collection of run in period items</returns>
        public List<CropViewItem> GetRunInPeriodItems(Farm farm,
            IEnumerable<CropViewItem> viewItemsForRotation,
            int startYearOfField,
            IEnumerable<CropViewItem> viewItemsForField, 
            FieldSystemComponent fieldComponent)
        {
            var moduloCounter = 0;
            var runInPeriodItems = new List<CropViewItem>();

            if (viewItemsForRotation.Count() > viewItemsForField.Count())
            {
                return new List<CropViewItem>(viewItemsForRotation);
            }

            // Add in additional years for the run-in period. The additional years should not be displayed to the user as they are just for internal calculations                
            var runInPeriod = farm.Defaults.DefaultRunInPeriod == 0 ? 1 : farm.Defaults.DefaultRunInPeriod;
            var runInPeriodStartYear = startYearOfField - runInPeriod;

            int rotationSize = 0;
            if (viewItemsForRotation.Count() == 0)
            {
                var firstViewItem = viewItemsForField.First();
                if (firstViewItem.SizeOfFirstRotationForField != viewItemsForField.Count())
                {
                    // This will occur when the start and end year are the same (i.e. SizeOfFirstRotation could be 3 and count of detail view items would be 1)
                    rotationSize = viewItemsForField.Count();
                }
                else
                {
                    rotationSize = viewItemsForField.First().SizeOfFirstRotationForField;
                }
            }
            else
            {
                // Field component will have crop view items that can be used for the count
                rotationSize = viewItemsForRotation.Count();
            }

            // Take the first phase of the rotation from the view items and use that as the basis for the run in period since these items will differ depending on
            // which field is being considered in the rotation.
            var mainCrops = viewItemsForField.OrderBy(x => x.Year).Where(y => y.IsSecondaryCrop == false).Take(rotationSize).ToList();
            var coverCrops = viewItemsForField.OrderBy(x => x.Year).Where(y => y.IsSecondaryCrop == true).Take(rotationSize).ToList();

            // Reverse items so we can use indexes from the beginning of collection
            mainCrops.Reverse();
            coverCrops.Reverse();          

            // Start at the year before the user-defined start year and work backwards towards the start year of the run in period
            for (int indexYear = startYearOfField - 1; indexYear >= runInPeriodStartYear; indexYear--, moduloCounter++)
            {
                var moduloIndex = moduloCounter % rotationSize;
                var mainCropAtYear = mainCrops.ElementAt(moduloIndex);

                var createdMainCrop = this.MapDetailsScreenViewItemFromComponentScreenViewItem(mainCropAtYear, indexYear);
                UseEnteredMoisture(createdMainCrop, mainCropAtYear, fieldComponent);
                createdMainCrop.Year = indexYear;
                createdMainCrop.DetailViewItemToComponentSelectionViewItemMap = mainCropAtYear.Guid;
                createdMainCrop.FieldSystemComponentGuid = fieldComponent.Guid;

                // Add to list of run in period items;
                runInPeriodItems.Add(createdMainCrop);

                var coverCropAtYear = coverCrops.ElementAtOrDefault(moduloIndex);
                if (coverCropAtYear != null)
                {
                    var createdCoverCrop = this.MapDetailsScreenViewItemFromComponentScreenViewItem(coverCropAtYear, indexYear);
                    UseEnteredMoisture(createdCoverCrop, coverCropAtYear, fieldComponent);
                    createdCoverCrop.Year = indexYear;
                    createdCoverCrop.DetailViewItemToComponentSelectionViewItemMap = coverCropAtYear.Guid;
                    createdCoverCrop.FieldSystemComponentGuid = fieldComponent.Guid;

                    // Add to list of run in period items;
                    runInPeriodItems.Add(createdCoverCrop);
                }
            }

            this.SetRunInPeriodTillageType(mainCrops, runInPeriodItems, farm);

            // All of these items belong to the run-in period. The CLI will use this information
            foreach (var runInPeriodItem in runInPeriodItems)
            {
                runInPeriodItem.IsRunInPeriodItem = true;
            }

            return runInPeriodItems.OrderBy(x => x.Year).ToList();
        }

        /// <summary>
        /// Gives a run-in item the crop moisture the user entered rather than the one on the details screen row it was
        /// copied from.
        ///
        /// A hayed row's moisture is the hay's once PostProcessPerennials has run, while its yield is the standing crop
        /// as it grew. The run-in items calculate their carbon inputs afresh, so a copied row would take the dry matter
        /// of a standing crop to be that of hay - for forage at 80% moisture baled at 15%, over four times the carbon
        /// actually grown - and the starting soil carbon is built from those inputs. Left as it is when the entered crop
        /// cannot be found.
        /// </summary>
        private static void UseEnteredMoisture(CropViewItem runInItem, CropViewItem detailsScreenRow, FieldSystemComponent fieldComponent)
        {
            var enteredCrop = GetComponentSelectionScreenCropForItem(fieldComponent, detailsScreenRow);
            if (enteredCrop == null)
            {
                return;
            }

            SetMoisture(runInItem, enteredCrop.MoistureContentOfCropPercentage, enteredCrop.MoistureContentOfCrop);
        }

        public void SetRunInPeriodTillageType(List<CropViewItem> mainCrops, List<CropViewItem> runInPeriodItems, Farm farm)
        {
            if (farm.UseCustomRunInTillage)
            {
                foreach (var runInPeriodItem in runInPeriodItems)
                {
                    runInPeriodItem.TillageType = farm.Defaults.RunInPeriodTillageType;
                }

                return;
            }

            if (mainCrops.Select(x => x.TillageType).Distinct().Count() == 1)
            {
                // There is only one tillage type, use it for all run in period items
                var tillageType = mainCrops.First().TillageType;
                foreach (var runInPeriodItem in runInPeriodItems)
                {
                    runInPeriodItem.TillageType = tillageType;
                }
            }
            else
            {
                // Use the 'average' of reduced tillage
                foreach (var runInPeriodItem in runInPeriodItems)
                {
                    runInPeriodItem.TillageType = TillageType.Reduced;
                }
            }
        }

        /// <summary>
        /// We need to add additional items for each cover crop (if used). The <see cref="CreateOrderedViewItems"/> method will use the list
        /// returned from this method to create the entire sequence from start to end year for the <see cref="FieldSystemComponent"/>.
        /// </summary>
        public IEnumerable<CropViewItem> PreProcessViewItems(
            FieldSystemComponent fieldSystemComponent)
        {
            // A list that will contain a combined list of main crops and secondary crops (undersown/cover/winter)
            var result = new List<CropViewItem>();

            /*
             * A view item is added for each main crop
             */

            result.AddRange(fieldSystemComponent.CropViewItems);

            /*
             * A view item needs to be added for each additional secondary crop
             */

            // Check for any secondary crops. Note there will always be a secondary view item for each year but we only consider that year to have grown a secondary crop if the crop
            // type was specified.
            var secondaryCrops = fieldSystemComponent.CoverCrops.Where(x => x.CropType != CropType.None);
            foreach (var cropViewItem in secondaryCrops)
            {
                // Add in an extra view item for the secondary crop
                var secondaryCrop = this.MapDetailsScreenViewItemFromComponentScreenViewItem(cropViewItem, cropViewItem.Year);

                secondaryCrop.IsSecondaryCrop = true;

                // The mapper does not copy the guid, and this copy is thrown away once the details screen rows have
                // been created from it. Carrying the entered crop's guid is what lets a secondary row record which
                // crop it came from, the same way a main crop's row does.
                secondaryCrop.Guid = cropViewItem.Guid;

                // If the secondary crop is a perennial and the main crop is a perennial of the same type, do not add an extra view item for the perennial grown in the second slot of the same year.
                // This is confusing on the details screen because the user will see two entries for the same year (1985 = Tame Legume && 1985 Tame Legume (cover crop))
                if (secondaryCrop.CropType.IsPerennial())
                {
                    // Check if the main crop is the same
                    var mainCrop = fieldSystemComponent.CropViewItems.SingleOrDefault(x => x.Year == secondaryCrop.Year);
                    if (mainCrop == null)
                    {
                        // If main crop is null, then we are in a situation where we are considering a projected system since the year for the main crop will not be equal to the year for the cover crops
                        // in the context of a current rotation (as opposed to a future projection)
                        continue;
                    }

                    if (mainCrop.CropType == secondaryCrop.CropType)
                    {
                        continue;
                    }
                }

                // Year property will already have been set at the component selection view stage
                result.Add(secondaryCrop);
            }

            return result.OrderBy(x => x.Year);
        }

        /// <summary>
        /// Translates the view items that were added by the user at the component selection view stage into detail view items that will be displayed
        /// on the details view stage. The detail view items will also be used in the final ICBM calculations
        /// </summary>
        public IEnumerable<CropViewItem> CreateItems(
            FieldSystemComponent fieldSystemComponent, 
            Farm farm)
        {
            var viewItems = this.PreProcessViewItems(fieldSystemComponent);
            
            var orderedViewItems = this.CreateOrderedViewItems(
                fieldSystemComponent,
                viewItems, 
                farm);

            return orderedViewItems;
        }

        #endregion

        #region Private Methods

        private void AssignInitialProperties(
            FieldSystemComponent fieldSystemComponent,
            IEnumerable<CropViewItem> viewItems,
            Farm farm)
        {
            for (int i = 0; i < viewItems.Count(); i++)
            {
                var currentYearViewItem = viewItems.ElementAt(i);

                currentYearViewItem.TimePeriodCategoryString = fieldSystemComponent.TimePeriodCategory.GetDescription();
                currentYearViewItem.Name = fieldSystemComponent.Name;
                currentYearViewItem.FieldName = fieldSystemComponent.Name;
                currentYearViewItem.ManagementPeriodName = fieldSystemComponent.Name;
                currentYearViewItem.SizeOfFirstRotationForField = fieldSystemComponent.CropViewItems.Count();

                /*
                 * This needs to be set to the GUID of the current component since the results view model will group items by field using this GUID value. Since
                 * all historical, and projected components belong to the same field, this value needs to be the same for all historical and projected components
                 * contained within the "current" component.
                 */

                if (fieldSystemComponent.TimePeriodCategory == TimePeriodCategory.Past ||
                    fieldSystemComponent.TimePeriodCategory == TimePeriodCategory.Future)
                {
                    currentYearViewItem.FieldSystemComponentGuid = fieldSystemComponent.CurrentPeriodComponentGuid;
                }
                else
                {
                    currentYearViewItem.FieldSystemComponentGuid = fieldSystemComponent.Guid;
                }

                _initializationService.InitializeSoilProperties(currentYearViewItem, farm);
            }
        }

        #endregion
    }
}
