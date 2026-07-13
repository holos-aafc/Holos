using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Mappers;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Economics;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Soil;
using H.Core.Services.Initialization.Crops;
using static H.Core.Services.LandManagement.FieldResultsService;

namespace H.Core.Services
{
    public class FieldComponentHelper : IFieldComponentHelper
    {
        #region Fields

        private readonly ModelMapper<CropViewItem> _cropViewItemMapper;
        private readonly ModelMapper<CropEconomicData> _cropEconomicDataMapper;
        private readonly ModelMapper<GrazingViewItem> _grazingPeriodMapper;
        private readonly ModelMapper<HarvestViewItem> _harvestPeriodMapper;
        private readonly ModelMapper<ManureApplicationViewItem> _manureApplicationViewItemMapper;
        private readonly ModelMapper<FertilizerApplicationViewItem> _fertilizerApplicationViewItemMapper;
        private readonly ModelMapper<HayImportViewItem> _hayImportViewItemMapper;
        private readonly ModelMapper<DigestateApplicationViewItem> _digestateViewItemMapper;
        private readonly ModelMapper<SoilData> _soilDataMapper;

        ICropInitializationService _cropInitializationService;

        #endregion

        #region Constructors

        public FieldComponentHelper()
        {
            _cropInitializationService = new CropInitializationService();

            _cropViewItemMapper = new ModelMapper<CropViewItem>(
                nameof(CropViewItem.Guid),
                nameof(CropViewItem.HarvestViewItems),
                nameof(CropViewItem.GrazingViewItems),
                nameof(CropViewItem.ManureApplicationViewItems),
                nameof(CropViewItem.CropEconomicData),
                nameof(CropViewItem.FertilizerApplicationViewItems));

            _cropEconomicDataMapper = new ModelMapper<CropEconomicData>();

            _harvestPeriodMapper = new ModelMapper<HarvestViewItem>(nameof(HarvestViewItem.Guid));

            _grazingPeriodMapper = new ModelMapper<GrazingViewItem>(nameof(GrazingViewItem.Guid));

            _manureApplicationViewItemMapper = new ModelMapper<ManureApplicationViewItem>();

            _hayImportViewItemMapper = new ModelMapper<HayImportViewItem>();

            _digestateViewItemMapper = new ModelMapper<DigestateApplicationViewItem>();

            // The fertilizer item map was bundled with Table_48 so FertilizerBlendData was deep-copied; Replicate()
            // now reproduces that with an explicit clone (the shallow mapper alone would share the blend reference).
            _fertilizerApplicationViewItemMapper = new ModelMapper<FertilizerApplicationViewItem>();

            _soilDataMapper = new ModelMapper<SoilData>();
        }

        #endregion

        #region Public Methods

        public void InitializeComponent(FieldSystemComponent component, Farm farm)
        {
            component.StartYear = farm.CarbonModellingEquilibriumYear;
            component.EndYear = DateTime.Now.Year;
            component.YearOfObservation = DateTime.Now.Year;

            var fieldName = this.GetUniqueFieldName(farm.FieldSystemComponents);
            component.Name = fieldName;
            component.ComponentDescriptionString = fieldName;
            component.GroupPath = fieldName;

            component.IsInitialized = true;

            // Assign soil types to field
            _cropInitializationService.InitializeAvailableSoilTypes(farm, component);
        }

        public string GetUniqueFieldName(IEnumerable<FieldSystemComponent> components)
        {
            var i = 1;
            var fieldSystemComponents = components;

            var totalCount = fieldSystemComponents.Count();
            var proposedName = string.Format(Properties.Resources.InterpolatedFieldNumber, i);

            //while proposedName isn't unique create a uniqe name for this component so we don't have duplicate named components
            while (fieldSystemComponents.Any(x => x.Name == proposedName))
            {
                proposedName = string.Format(Properties.Resources.InterpolatedFieldNumber, ++i);
            }

            return proposedName;
        }

        public FieldSystemComponent Replicate(FieldSystemComponent component)
        {
            var fieldSystemComponent = (FieldSystemComponent)Activator.CreateInstance(typeof(FieldSystemComponent));

            this.Replicate(component, fieldSystemComponent);

            fieldSystemComponent.Name = component.Name;

            /*
             * Must be true else Holos will reinitialize the component 
             * causing field components in the components view to behave oddly
             * when clicked upon.
             */
            fieldSystemComponent.IsInitialized = true;

            return fieldSystemComponent;
        }

        public void Replicate(ComponentBase copyFrom, ComponentBase copyTo)
        {
            var to = copyTo as FieldSystemComponent;
            var from = copyFrom as FieldSystemComponent;

            to.FieldArea = from.FieldArea;
            to.StartYear = from.StartYear;
            to.EndYear = from.EndYear;

            foreach (var cropViewItem in from.CropViewItems)
            {
                var copiedViewItem = new CropViewItem();

                _cropViewItemMapper.Map(cropViewItem, copiedViewItem);

                _cropEconomicDataMapper.Map(cropViewItem.CropEconomicData, copiedViewItem.CropEconomicData);

                to.CropViewItems.Add(copiedViewItem);

                foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
                {
                    var copiedManureApplicationViewItem = new ManureApplicationViewItem();

                    _manureApplicationViewItemMapper.Map(manureApplicationViewItem, copiedManureApplicationViewItem);
                    copiedViewItem.ManureApplicationViewItems.Add(copiedManureApplicationViewItem);
                }

                foreach (var harvestViewItem in cropViewItem.HarvestViewItems)
                {
                    var copiedHarvestViewItem = new HarvestViewItem();

                    _harvestPeriodMapper.Map(harvestViewItem, copiedHarvestViewItem);
                    copiedViewItem.HarvestViewItems.Add(copiedHarvestViewItem);
                }

                foreach (var hayImportViewItem in cropViewItem.HayImportViewItems)
                {
                    var copiedHayImportItem = new HayImportViewItem();
                }

                foreach (var grazingViewItem in cropViewItem.GrazingViewItems)
                {
                    var copiedGrazingViewItem = new GrazingViewItem();

                    _grazingPeriodMapper.Map(grazingViewItem, copiedGrazingViewItem);
                    copiedViewItem.GrazingViewItems.Add(copiedGrazingViewItem);
                }

                foreach (var fertilizerApplicationViewItem in cropViewItem.FertilizerApplicationViewItems)
                {
                    var copiedFertilizerApplicationViewItem = new FertilizerApplicationViewItem();

                    _fertilizerApplicationViewItemMapper.Map(fertilizerApplicationViewItem, copiedFertilizerApplicationViewItem);

                    // Reproduce the deep-copy the previous bundled mapper performed for the blend data.
                    copiedFertilizerApplicationViewItem.FertilizerBlendData =
                        PropertyMapper.Clone(fertilizerApplicationViewItem.FertilizerBlendData);

                    copiedViewItem.FertilizerApplicationViewItems.Add(copiedFertilizerApplicationViewItem);
                }
            }
        }

        /// <summary>
        /// Determines which view item is the main crop for a particular year. Will use the boolean <see cref="H.Core.Models.LandManagement.Fields.CropViewItem.IsSecondaryCrop"/> to determine which view item
        /// is the main crop for the particular year.
        /// </summary>
        public CropViewItem GetMainCropForYear(IEnumerable<CropViewItem> viewItems,
            int year)
        {
            var viewItemsForYear = viewItems.GetByYear(year);
            if (viewItemsForYear.Any() == false)
            {
                return null;
            }

            if (viewItemsForYear.Count() == 1)
            {
                // There is only one crop grown, return it as the main crop
                return viewItemsForYear.Single();
            }

            var mainCrop = viewItemsForYear.FirstOrDefault(x => x.IsSecondaryCrop == false);
            if (mainCrop != null)
            {
                return mainCrop;
            }
            else
            {
                // Old farms won't have this boolean set, so return first item or have user rebuild stage state
                return viewItemsForYear.First();
            }
        }

        public CropViewItem GetCoverCropForYear(
            IEnumerable<CropViewItem> viewItems,
            int year)
        {
            var viewItemsForYear = viewItems.GetByYear(year);
            if (viewItemsForYear.Any() == false)
            {
                return null;
            }

            if (viewItemsForYear.Count() == 1 && viewItemsForYear.Single().IsSecondaryCrop == false)
            {
                // There is only one crop grown and it is not a cover crop
                return null;
            }

            var coverCrop = viewItemsForYear.FirstOrDefault(x => x.IsSecondaryCrop == true);
            if (coverCrop != null)
            {
                return coverCrop;
            }
            else
            {
                // Old farms won't have this boolean set, so return first item or have user rebuild stage state
                return null;
            }
        }

        public AdjoiningYears GetAdjoiningYears(
            IEnumerable<CropViewItem> viewItems,
            int year)
        {
            var previousYear = year - 1;
            var nextYear = year + 1;

            // Get all items from the same year
            var viewItemsForYear = viewItems.GetItemsByYear(year);

            var mainCropForCurrentYear = this.GetMainCropForYear(viewItemsForYear, year);
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
                var previousYearViewItem = viewItems.SingleOrDefault(x => x.Year == previousYear);
                var nextYearViewItem = viewItems.SingleOrDefault(x => x.Year == nextYear);

                return new AdjoiningYears()
                {
                    PreviousYearViewItem = previousYearViewItem,
                    CurrentYearViewItem = mainCropForCurrentYear,
                    NextYearViewItem = nextYearViewItem,
                };
            }
        }

        #endregion
    }
}