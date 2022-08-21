using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Economics;
using H.Core.Providers.Fertilizer;

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

        #endregion

        #region Constructors

        public FieldComponentHelper()
        {
            var cropViewItemMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<CropViewItem, CropViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore())
                    .ForMember(y => y.HarvestViewItems, z => z.Ignore())
                    .ForMember(y => y.GrazingViewItems, z => z.Ignore())
                    .ForMember(y => y.ManureApplicationViewItems, z => z.Ignore())
                    .ForMember(y => y.CropEconomicData, z => z.Ignore())
                    .ForMember(y => y.FertilizerApplicationViewItems, z => z.Ignore());
            });

            _cropViewItemMapper = cropViewItemMapperConfiguration.CreateMapper();

            var cropEconomicDataMapperConfiguration =
                new MapperConfiguration(x => x.CreateMap<CropEconomicData, CropEconomicData>());
            _cropEconomicDataMapper = cropEconomicDataMapperConfiguration.CreateMapper();

            var harvestPeriodMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<HarvestViewItem, HarvestViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore());
            });

            _harvestPeriodMapper = harvestPeriodMapperConfiguration.CreateMapper();

            var grazingPeriodMapperConfiguration = new MapperConfiguration(x =>
            {
                x.CreateMap<GrazingViewItem, GrazingViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore());
            });

            _grazingPeriodMapper = grazingPeriodMapperConfiguration.CreateMapper();

            var manureApplicationViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<ManureApplicationViewItem, ManureApplicationViewItem>();
            });

            _manureApplicationViewItemMapper = manureApplicationViewItemMapper.CreateMapper();

            var fertilizerApplicationViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data, Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data>();
                x.CreateMap<FertilizerApplicationViewItem, FertilizerApplicationViewItem>();
                
            });
            
            _fertilizerApplicationViewItemMapper = fertilizerApplicationViewItemMapper.CreateMapper();
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

            // When comparing farms, these two values have to be the same otherwise details stage state items won't match up to an existing field component
            fieldSystemComponent.Guid = component.Guid;

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

                foreach (var harvestViewItem in cropViewItem.HarvestViewItems)
                {
                    var copiedHarvestViewItem = new HarvestViewItem();

                    _harvestPeriodMapper.Map(harvestViewItem, copiedHarvestViewItem);
                    copiedViewItem.HarvestViewItems.Add(copiedHarvestViewItem);
                }

                foreach (var grazingViewItem in cropViewItem.GrazingViewItems)
                {
                    var copiedGrazingViewItem = new GrazingViewItem();

                    _grazingPeriodMapper.Map(grazingViewItem, copiedGrazingViewItem);
                    copiedViewItem.GrazingViewItems.Add(copiedGrazingViewItem);
                }

                foreach (var manureApplicationViewItem in cropViewItem.ManureApplicationViewItems)
                {
                    var copiedManureApplicationViewItem = new ManureApplicationViewItem();

                    _manureApplicationViewItemMapper.Map(manureApplicationViewItem, copiedManureApplicationViewItem);
                    copiedViewItem.ManureApplicationViewItems.Add(copiedManureApplicationViewItem);
                }

                foreach (var fertilizerApplicationViewItem in cropViewItem.FertilizerApplicationViewItems)
                {
                    var copiedFertilizerApplicationViewItem = new FertilizerApplicationViewItem();

                    _fertilizerApplicationViewItemMapper.Map(fertilizerApplicationViewItem, copiedFertilizerApplicationViewItem);
                    copiedViewItem.FertilizerApplicationViewItems.Add(copiedFertilizerApplicationViewItem);
                }
            }
        }

        #endregion
    }
}