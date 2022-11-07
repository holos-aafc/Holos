using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Core.Models;
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
        /// <param name="viewItem">The <see cref="CropViewItem"/> to copy from.</param>
        /// <param name="year">When creating historical view items, this value will be used to set the year of any associated <see cref="ManureApplicationViewItem"/>s etc.</param>
        /// <returns>A duplicated <see cref="CropViewItem"/></returns>
        public CropViewItem MapDetailsScreenViewItemFromComponentScreenViewItem(CropViewItem viewItem, int year)
        {
            var result = _detailViewItemMapper.Map<CropViewItem, CropViewItem>(viewItem);

            // Copy all manure applications to the detail view item
            foreach (var manureApplicationViewItem in viewItem.ManureApplicationViewItems)
            {
                var copiedManureApplicationViewItem = _manureApplicationViewItemMapper.Map<ManureApplicationViewItem, ManureApplicationViewItem>(manureApplicationViewItem);

                // We need to update the year so that the current years' manure applications are copied back in time
                copiedManureApplicationViewItem.DateOfApplication = new DateTime(year, manureApplicationViewItem.DateOfApplication.Month, manureApplicationViewItem.DateOfApplication.Day);

                result.ManureApplicationViewItems.Add(copiedManureApplicationViewItem);
            }

            foreach (var harvestViewItem in viewItem.HarvestViewItems)
            {
                var copiedHarvestViewItem = _harvestViewItemMapper.Map<HarvestViewItem, HarvestViewItem>(harvestViewItem);

                // We need to update the year so that the current years' harvest items are copied back in time
                copiedHarvestViewItem.DateCreated = new DateTime(year, harvestViewItem.DateCreated.Month, harvestViewItem.DateCreated.Day);

                result.HarvestViewItems.Add(copiedHarvestViewItem);
            }

            foreach (var hayImportViewItem in viewItem.HayImportViewItems)
            {
                var copiedHayImportViewItem = _hayImportViewItemMapper.Map<HayImportViewItem, HayImportViewItem>(hayImportViewItem);

                // We need to update the year so that the current years' hay import items are copied back in time
                copiedHayImportViewItem.Date = new DateTime(year, copiedHayImportViewItem.DateCreated.Month, copiedHayImportViewItem.DateCreated.Day);

                result.HayImportViewItems.Add(copiedHayImportViewItem);
            }

            return result;
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
            }
        }

        /// <summary>
        /// Determines which view item is the main crop for a particular year. Will use the boolean <see cref="CropViewItem.IsSecondaryCrop"/> to determine which view item
        /// is the main crop for the particular year.
        /// </summary>
        public CropViewItem GetMainCropForYear(
            IEnumerable<CropViewItem> viewItems, 
            int year, 
            FieldSystemComponent fieldSystemComponent)
        {
            var detailViewItemsForYear = viewItems.Where(x => x.Year == year).ToList();
            if (detailViewItemsForYear.Any() == false)
            {
                return null;
            }

            if (detailViewItemsForYear.Count() == 1)
            {
                // There is only one crop grown, return it as the main crop
                return detailViewItemsForYear.Single();
            }

            var mainCrop = detailViewItemsForYear.FirstOrDefault(x => x.IsSecondaryCrop == false);
            if (mainCrop != null)
            {
                return mainCrop;
            }
            else
            {
                // Old farms won't have this boolean set, so return first item or have user rebuild stage state
                return detailViewItemsForYear.First();
            }
        }

        /// <summary>
        /// Combines input for a particular year. This would combine the inputs from the main crop grown that year plus the cover crop (if specified by user). This must be called
        /// after all inputs from crops, manure, etc. have been calculated for each detail view item since we simply add up the total above and below ground inputs for each
        /// year. Inputs from the secondary crop are added to the main crop since the main crop view item will be used in the final ICBM calculations
        /// </summary>
        public void CombineInputsForAllCropsInSameYear(
            IEnumerable<CropViewItem> viewItems, 
            FieldSystemComponent fieldSystemComponent)
        {
            // Add up all residues for each year and combine into the main crop

            var years = viewItems.Select(x => x.Year).Distinct().OrderBy(x => x);
            foreach (var year in years)
            {
                // Get the main crop view item and the secondary crop item (if user specified a secondary crop)
                var viewItemsForYear = viewItems.Where(x => x.Year == year);
                
                // Get the main crop
                var mainCrop = this.GetMainCropForYear(viewItemsForYear, year, fieldSystemComponent);

                // Combine inputs from other crops grown in the same year (undersown, cover crop, etc.)

                var coverCropAboveGroundInput = 0d;
                var coverCropBelowGroundInput = 0d;
                var coverCropManureInput = 0d;
                var cropsOtherThanMainCrop = viewItemsForYear.Except(new List<CropViewItem>() {mainCrop});
                foreach (var cropViewItem in cropsOtherThanMainCrop)
                {
                    coverCropAboveGroundInput += cropViewItem.AboveGroundCarbonInput;
                    coverCropBelowGroundInput += cropViewItem.BelowGroundCarbonInput;
                    coverCropManureInput += cropViewItem.ManureCarbonInputsPerHectare;
                }

                mainCrop.CombinedAboveGroundInput = mainCrop.AboveGroundCarbonInput + coverCropAboveGroundInput;
                mainCrop.CombinedBelowGroundInput = mainCrop.BelowGroundCarbonInput + coverCropBelowGroundInput;
                mainCrop.CombinedManureInput = mainCrop.ManureCarbonInputsPerHectare + coverCropManureInput;

                mainCrop.TotalCarbonInputs = mainCrop.CombinedAboveGroundInput + mainCrop.CombinedBelowGroundInput + mainCrop.CombinedManureInput;
            }
        }

        /// <summary>
        /// Combine multiple view items for same year back into a single view item for that same year so ICBM works with one view item per year as expected.
        /// </summary>
        public List<CropViewItem> MergeDetailViewItems(IEnumerable<CropViewItem> viewItems,
                                             FieldSystemComponent fieldSystemComponent)
        {
            var result = new List<CropViewItem>();

            var years = viewItems.Select(x => x.Year).Distinct().OrderBy(x => x);
            foreach (var year in years)
            {
                var itemsForYear = viewItems.Where(x => x.Year == year);

                // Get the main crop
                var mainCropForYear = this.GetMainCropForYear(itemsForYear, year, fieldSystemComponent);

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
            if (farm.YieldAssignmentMethod == YieldAssignmentMethod.InputFile)
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

            // Before creating view items for each year, calculate carbon uptake by grazing animals
            this.CalculateCarbonLostByGrazingAnimals(
                fieldSystemComponent: fieldSystemComponent,
                results: this.AnimalResults);

            // Before creating view items for each year, calculate carbon deposited from manure of animals grazing on pasture
            //this.CalculateManureCarbonInputByGrazingAnimals(fieldSystemComponent, farm);

            // Before creating view items for each year, calculate carbon lost from bale exports
            this.CalculateCarbonLostFromHayExports(fieldSystemComponent, farm);

            // Create a view item for each year (and also create additional items for each cover crop in the same year)
            var viewItems = this.CreateItems(fieldSystemComponent, farm).ToList();

            // Assign a default name, time period category, etc. to the view item
            this.AssignInitialProperties(fieldSystemComponent, viewItems);

            // Once the view items have been created, additional properties that relate to perennials only need to be assigned (stand IDs, positional years, etc.)
            this.ProcessPerennials(viewItems, fieldSystemComponent);

            // Similarly with cover crops, the view items need to be adjusted
            this.ProcessCoverCrops(viewItems, fieldSystemComponent);

            // Add in a details view message for the undersown year(s). Note that perennials must be processed before this call
            this.ProcessUndersownCrops(viewItems, fieldSystemComponent);

            // Assign carbon inputs for each view item
            this.AssignCarbonInputs(
                viewItems: viewItems,
                farm: farm, 
                fieldSystemComponent: fieldSystemComponent);

            var stageState = this.GetStageState(farm);

            // Save the view items to the farm which can then be edited by the user on the details view
            stageState.DetailsScreenViewCropViewItems.AddRange(viewItems.OrderBy(x => x.Year).ThenBy(x => x.IsSecondaryCrop));
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

            if (farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2)
            {
                var runInPeriodItems = this.GetRunInPeriodItems(
                    farm: farm,
                    viewItemsForRotation: viewItems,
                    startYearOfField: startYear,
                    viewItemsForField: result);

                fieldSystemComponent.RunInPeriodItems.Clear();
                fieldSystemComponent.RunInPeriodItems.AddRange(runInPeriodItems);
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
        /// <returns>A collection of run in period items</returns>
        public List<CropViewItem> GetRunInPeriodItems(
            Farm farm, 
            IEnumerable<CropViewItem> viewItemsForRotation, 
            int startYearOfField,
            IEnumerable<CropViewItem> viewItemsForField)
        {
            var moduloCounter = 0;
            var runInPeriodItems = new List<CropViewItem>();

            // Add in additional years for the run-in period. The additional years should not be displayed to the user as they are just for internal calculations                
            var runInPeriod = farm.Defaults.DefaultRunInPeriod;
            var runInPeriodStartYear = startYearOfField - runInPeriod;

            // Take the first phase of the rotation from the view items and use that as the basis for the run in period since these items will differ depending on
            // which field is being considered in the rotation.
            var items = viewItemsForField.OrderBy(x => x.Year).Take(viewItemsForRotation.Count()).ToList();

            // Reverse items so we can use indexes from the beginning of collection
            items.Reverse();

            // Start at the year before the user-defined start year and work backwards towards the start year of the run in period
            for (int indexYear = startYearOfField - 1; indexYear >= runInPeriodStartYear; indexYear--, moduloCounter++)
            {
                var moduloIndex = moduloCounter % viewItemsForRotation.Count();
                var itemAtYear = items.ElementAt(moduloIndex);

                // Need to get all items at year, not just main crop?

                var createdViewItem = this.MapDetailsScreenViewItemFromComponentScreenViewItem(itemAtYear, indexYear);
                createdViewItem.Year = indexYear;
                createdViewItem.DetailViewItemToComponentSelectionViewItemMap = itemAtYear.Guid;

                // Add to list of run in period items
                runInPeriodItems.Add(createdViewItem);
            }

            return runInPeriodItems.OrderBy(x => x.Year).ToList();
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
            IEnumerable<CropViewItem> viewItems)
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
            }
        }

        #endregion
    }
}
