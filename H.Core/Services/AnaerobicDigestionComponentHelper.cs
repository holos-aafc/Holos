using AutoMapper;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace H.Core.Services
{
    /// <summary>
    /// Helper class that inherits and implements <see cref="IAnaerobicDigestionComponentHelper"/>. The class handles functions like AD component initialization, getting unique names for components etc.
    /// </summary>
    public class AnaerobicDigestionComponentHelper : IAnaerobicDigestionComponentHelper
    {
        #region Fields

        private readonly IMapper _anaerobicDigestionComponentMapper;
        private readonly IMapper _anaerobicDigestionViewItemMapper;
        private readonly IMapper _animalGroupMapper;
        private readonly IMapper _cropResiduesSubstrateViewItemMapper;
        private readonly IMapper _farmResiduesSubstrateViewItemMapper;
        private readonly IMapper _manureSubstrateViewItemMapper;
        private readonly IMapper _managementPeriodViewItemsMapper;

        #endregion

        #region Constructors

        public AnaerobicDigestionComponentHelper()
        {
            var anaerobicDigestionComponentMapperConfiguration = new MapperConfiguration(x =>
                x.CreateMap<AnaerobicDigestionComponent, AnaerobicDigestionComponent>()
                    .ForMember(y => y.Guid, z => z.Ignore())
                    .ForMember(y => y.CurrentPeriodComponentGuid, z => z.Ignore())
                    .ForMember(y => y.AnaerobicDigestionViewItem, z => z.Ignore())
                    .ForMember(y => y.ManagementPeriodViewItems, z => z.Ignore()));
            _anaerobicDigestionComponentMapper = anaerobicDigestionComponentMapperConfiguration.CreateMapper();

            var anaerobicDigestionViewItemMapperConfiguration = new MapperConfiguration(x =>
                    x.CreateMap<AnaerobicDigestionViewItem, AnaerobicDigestionViewItem>()
                .ForMember(y => y.Guid, z => z.Ignore())
                .ForMember(y => y.CropResiduesSubstrateViewItems, z => z.Ignore())
                .ForMember(y => y.FarmResiduesSubstrateViewItems, z => z.Ignore())
                .ForMember(y => y.ManureSubstrateViewItems, z => z.Ignore()));
            _anaerobicDigestionViewItemMapper = anaerobicDigestionViewItemMapperConfiguration.CreateMapper();

            var managementPeriodViewItemsMapperConfiguration = new MapperConfiguration(x =>
                x.CreateMap<ADManagementPeriodViewItem, ADManagementPeriodViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore())
                    .ForMember(y => y.AnimalComponent, z => z.Ignore())
                    .ForMember(y => y.AnimalGroup, z => z.Ignore())
                    .ForMember(y => y.ManagementPeriod, z => z.Ignore()));
            _managementPeriodViewItemsMapper = managementPeriodViewItemsMapperConfiguration.CreateMapper();

            var animalGroupMapperConfiguration = new MapperConfiguration(x => x.CreateMap<AnimalGroup, AnimalGroup>()
                .ForMember(y => y.ManagementPeriods, z => z.Ignore()));
            _animalGroupMapper = animalGroupMapperConfiguration.CreateMapper();

            var cropResiduesSubstrateViewItemMapperConfiguration = new MapperConfiguration(x =>
                x.CreateMap<CropResidueSubstrateViewItem, CropResidueSubstrateViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore()));
            _cropResiduesSubstrateViewItemMapper = cropResiduesSubstrateViewItemMapperConfiguration.CreateMapper();

            var farmResiduesSubstrateViewItemMapperConfiguration = new MapperConfiguration(x =>
                x.CreateMap<FarmResiduesSubstrateViewItem, FarmResiduesSubstrateViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore()));
            _farmResiduesSubstrateViewItemMapper = farmResiduesSubstrateViewItemMapperConfiguration.CreateMapper();

            var manureSubstrateViewItemMapperConfiguration = new MapperConfiguration(x =>
                x.CreateMap<ManureSubstrateViewItem, ManureSubstrateViewItem>()
                    .ForMember(y => y.Guid, z => z.Ignore()));
            _manureSubstrateViewItemMapper = manureSubstrateViewItemMapperConfiguration.CreateMapper();
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Initializes a new component. The method gives each component a unique name and sets the <see cref="AnaerobicDigestionComponent.IsInitialized"/> flag.
        /// </summary>
        /// <param name="component">The component that needs to be initialized</param>
        /// <param name="activeFarm">The current farm that contains the AD component</param>
        public void InitializeAnaerobicDigestionComponent(AnaerobicDigestionComponent component, Farm activeFarm)
        {
            component.Name = GetUniqueAnaerobicDigestionName(activeFarm.AnaerobicDigestionComponents);
            component.IsInitialized = true;

            component.NumberOfReactors = 1;
            component.HydraulicRetentionTimeInDays = 25;
        }

        /// <summary>
        /// Sets a unique name for each AD component. It takes in a collection of components as a parameter. The method then sets a unique name based on whether
        /// a name currently exists in the collection or not. The method starts with a base name and keeps on adding a number to the end, comparing it to the collection, until a unique name is found. When a unique name is found,
        /// the method exits the loop and returns that unique name.
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public string GetUniqueAnaerobicDigestionName(IEnumerable<AnaerobicDigestionComponent> components)
        {
            int componentNumber = 1;
            int totalCount = components.Count();

            string uniqueName = Properties.Resources.EnumAnaerobicDigestion;
            while (components.Any(x => x.Name == uniqueName))
            {
                uniqueName = $"{Properties.Resources.EnumAnaerobicDigestion} #{++componentNumber}";
            }
            return uniqueName;
        }

        public AnaerobicDigestionComponent Replicate(AnaerobicDigestionComponent component, IEnumerable<AnimalComponentBase> replicatedAnimalComponents)
        {
            // Guids from the replicatedAnimalComponents must be mapped correctly in order for Holos to correctly link the ADComponent to the existing animal data. 
            // 
            var replicatedComponent = (AnaerobicDigestionComponent)Activator.CreateInstance(component.GetType());
            Replicate(component, replicatedComponent, replicatedAnimalComponents);
            replicatedComponent.Name = component.Name;
            replicatedComponent.IsInitialized = true;

            return replicatedComponent;
        }

        public void Replicate(ComponentBase source, ComponentBase destination, IEnumerable<AnimalComponentBase> replicatedAnimalComponents)
        {
            var from = source as AnaerobicDigestionComponent;
            var to = destination as AnaerobicDigestionComponent;

            var aDManagementPeriodViewItems = new Collection<ADManagementPeriodViewItem>();

            // Map original AD component to replicated AD component excluding the Guid, CurrentPeriodComponentGuid, AnaerobicDigestionViewItem and ManagementPeriodViewItems
            _anaerobicDigestionComponentMapper.Map(from, to);

            // Step through each ManagementPeriod in each AnimalGroup in each replicated AnimalComponent that can have an AD management period, match these management period and animal group names to original AD component and map them to new AD component
            // Use names of Groups and ManagementPeriods as Guid matching isn't viable 
            foreach (var animalComponent in replicatedAnimalComponents.Where(x => x.ComponentCategory != ComponentCategory.Sheep))
            {
                foreach (var animalGroup in animalComponent.Groups)
                {
                    foreach (var managementPeriod in animalGroup.ManagementPeriods.Where(x => x.ManureDetails.StateType != ManureStateType.Pasture))
                    {
                        foreach (var managementPeriodViewItem in from.ManagementPeriodViewItems)
                        {
                            if (managementPeriodViewItem.AnimalGroup.Name == animalGroup.Name && managementPeriodViewItem.ManagementPeriod.Name == managementPeriod.Name)
                            {
                                // Map original ManagementPeriodViewItem to new ManagementPeriodViewItem excluding the Guid, AnimalComponent, AnimalGroup and ManagementPeriod
                                var copiedManagementPeriodViewItem = new ADManagementPeriodViewItem();
                                _managementPeriodViewItemsMapper.Map(managementPeriodViewItem, copiedManagementPeriodViewItem);

                                // Assign replicated AnimalComponent to replicate ManagementPeriodViewItem
                                copiedManagementPeriodViewItem.AnimalComponent = animalComponent;

                                // Map replicated AnimalGroup to replicate ManagementPeriodViewItem excluding the ManagementPeriods (source ADComponent doesn't include ManagementPeriods in AnimalGroup)
                                var copiedAnimalGroup = new AnimalGroup();
                                _animalGroupMapper.Map(animalGroup, copiedAnimalGroup);
                                copiedManagementPeriodViewItem.AnimalGroup = copiedAnimalGroup;

                                // Assign replicated ManagementPeriod to replicate ManagementPeriodViewItem
                                copiedManagementPeriodViewItem.ManagementPeriod = managementPeriod;

                                aDManagementPeriodViewItems.Add(copiedManagementPeriodViewItem);
                            }
                        }
                    }
                }
            }

            to.ManagementPeriodViewItems = new ObservableCollection<ADManagementPeriodViewItem>(aDManagementPeriodViewItems);

            // Map original AnaerobicDigestionViewItem to replicated AnaerobicDigestionViewItem excluding the Guid, CropResiduesSubstrateViewItems, FarmResiduesSubstrateViewItems and ManureSubstrateViewItems
            _anaerobicDigestionViewItemMapper.Map(from.AnaerobicDigestionViewItem, to.AnaerobicDigestionViewItem);

            foreach (var cropResidueSubstrateViewItem in from.AnaerobicDigestionViewItem.CropResiduesSubstrateViewItems)
            {
                // Map original CropResidueSubstrateViewItem to new CropResidueSubstrateViewItem excluding the Guid
                var copiedCropResidueSubstrateViewItem = new CropResidueSubstrateViewItem();
                _cropResiduesSubstrateViewItemMapper.Map(cropResidueSubstrateViewItem, copiedCropResidueSubstrateViewItem);
                to.AnaerobicDigestionViewItem.CropResiduesSubstrateViewItems.Add(copiedCropResidueSubstrateViewItem);
            }

            foreach (var farmResidueSubstrateViewItem in from.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems)
            {
                // map original FarmResiduesSubstrateViewItem to new FarmResiduesSubstrateViewItem excluding the Guid
                var copiedFarmResiduesSubstrateViewItem = new FarmResiduesSubstrateViewItem();
                _farmResiduesSubstrateViewItemMapper.Map(farmResidueSubstrateViewItem, copiedFarmResiduesSubstrateViewItem);
                to.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(copiedFarmResiduesSubstrateViewItem);
            }

            foreach (var manureSubstrateViewItem in from.AnaerobicDigestionViewItem.ManureSubstrateViewItems)
            {
                // Map original ManureSubstrateViewItem to new ManureSubstrateViewItem excluding the Guid
                var copiedManureSubstrateViewItem = new ManureSubstrateViewItem();
                _manureSubstrateViewItemMapper.Map(manureSubstrateViewItem, copiedManureSubstrateViewItem);
                to.AnaerobicDigestionViewItem.ManureSubstrateViewItems.Add(copiedManureSubstrateViewItem);
            }
        }
        #endregion
    }
}
