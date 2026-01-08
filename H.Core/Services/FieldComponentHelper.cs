using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Economics;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Soil;
using H.Core.Services.Initialization.Crops;
using Microsoft.Extensions.Logging.Abstractions;

namespace H.Core.Services
{
    public class FieldComponentHelper : IFieldComponentHelper
    {
        #region Fields

        private readonly IMapper _cropViewItemMapper;
        private readonly IMapper _cropEconomicDataMapper;
        private readonly IMapper _grazingPeriodMapper;
        private readonly IMapper _harvestPeriodMapper;
        private readonly IMapper _manureApplicationViewItemMapper;
        private readonly IMapper _fertilizerApplicationViewItemMapper;
        private readonly IMapper _hayImportViewItemMapper;
        private readonly IMapper _digestateViewItemMapper;
        private readonly IMapper _soilDataMapper;

        ICropInitializationService _cropInitializationService;

        #endregion

        #region Constructors

        public FieldComponentHelper()
        {
            _cropInitializationService = new CropInitializationService();

            var cropViewItemMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<CropViewItem, CropViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore())
                    .ForMember(y => y.HarvestViewItems, z => z.Ignore())
                    .ForMember(y => y.GrazingViewItems, z => z.Ignore())
                    .ForMember(y => y.ManureApplicationViewItems, z => z.Ignore())
                    .ForMember(y => y.CropEconomicData, z => z.Ignore())
                    .ForMember(y => y.FertilizerApplicationViewItems, z => z.Ignore());
            }, new NullLoggerFactory());

            _cropViewItemMapper = cropViewItemMapperConfiguration.CreateMapper();

            var cropEconomicDataMapperConfiguration =
                new MapperConfiguration(x => x.CreateMap<CropEconomicData, CropEconomicData>(), new NullLoggerFactory());

            _cropEconomicDataMapper = cropEconomicDataMapperConfiguration.CreateMapper();

            var harvestPeriodMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<HarvestViewItem, HarvestViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore());
            }, new NullLoggerFactory());

            _harvestPeriodMapper = harvestPeriodMapperConfiguration.CreateMapper();

            var grazingPeriodMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<GrazingViewItem, GrazingViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore());
            }, new NullLoggerFactory());

            _grazingPeriodMapper = grazingPeriodMapperConfiguration.CreateMapper();

            var manureApplicationViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<ManureApplicationViewItem, ManureApplicationViewItem>();
            }, new NullLoggerFactory());

            _manureApplicationViewItemMapper = manureApplicationViewItemMapper.CreateMapper();

            var hayImportViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<HayImportViewItem, HayImportViewItem>();
            }, new NullLoggerFactory());

            _hayImportViewItemMapper = hayImportViewItemMapper.CreateMapper();

            var digestateViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<DigestateApplicationViewItem, DigestateApplicationViewItem>();
            }, new NullLoggerFactory());

            _digestateViewItemMapper = digestateViewItemMapper.CreateMapper();


            var fertilizerApplicationViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data, Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data>();
                x.CreateMap<FertilizerApplicationViewItem, FertilizerApplicationViewItem>();
            }, new NullLoggerFactory());
            
            _fertilizerApplicationViewItemMapper = fertilizerApplicationViewItemMapper.CreateMapper();

            var soilDataMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<SoilData, SoilData>();
            }, new NullLoggerFactory());

            _soilDataMapper = soilDataMapper.CreateMapper();
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