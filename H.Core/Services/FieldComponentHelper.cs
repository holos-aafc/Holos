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

            var hayImportViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<HayImportViewItem, HayImportViewItem>();
            });

            _hayImportViewItemMapper = hayImportViewItemMapper.CreateMapper();

            var digestateViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<DigestateApplicationViewItem, DigestateApplicationViewItem>();
            });

            _digestateViewItemMapper = digestateViewItemMapper.CreateMapper();


            var fertilizerApplicationViewItemMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data, Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data>();
                x.CreateMap<FertilizerApplicationViewItem, FertilizerApplicationViewItem>();
            });
            
            _fertilizerApplicationViewItemMapper = fertilizerApplicationViewItemMapper.CreateMapper();

            var soilDataMapper = new MapperConfiguration(x =>
            {
                x.CreateMap<SoilData, SoilData>();
            });

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
            this.InitializeSoilAvailableSoilTypes(farm, component);
        }

        private void InitializeSoilAvailableSoilTypes(Farm farm, FieldSystemComponent component)
        {
            foreach (var soilData in farm.GeographicData.SoilDataForAllComponentsWithinPolygon)
            {
                // Add this type of soil if it does not already exist
                if (component.SoilDataAvailableForField.FirstOrDefault(x => x.SoilGreatGroup == soilData.SoilGreatGroup) == null)
                {
                    // We don't model organic soil at this time
                    if (soilData.SoilFunctionalCategory != SoilFunctionalCategory.Organic)
                    {
                        var copiedSoil = new SoilData();
                        _soilDataMapper.Map(soilData, copiedSoil);

                        component.SoilDataAvailableForField.Add(copiedSoil);
                    }
                }
            }
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

        #endregion
    }
}