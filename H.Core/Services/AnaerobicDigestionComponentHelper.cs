using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using H.Core.Models.Infrastructure;
using H.Core.Models;

namespace H.Core.Services
{
    /// <summary>
    /// Helper class that inherits and implements <see cref="IAnaerobicDigestionComponentHelper"/>. The class handles functions like AD component initialization, getting unique names for components etc.
    /// </summary>
    public class AnaerobicDigestionComponentHelper : IAnaerobicDigestionComponentHelper
    {
        #region Fields

        private readonly IMapper _anaerobicDigestionComponentMapper;

        #endregion

        #region Constructors

        public AnaerobicDigestionComponentHelper()
        {
            var anaerobicDigestionComponentMapperConfiguration = new MapperConfiguration(x =>
                x.CreateMap<AnaerobicDigestionComponent, AnaerobicDigestionComponent>()
                    .ForMember(y => y.Guid, z => z.Ignore()));

            _anaerobicDigestionComponentMapper = anaerobicDigestionComponentMapperConfiguration.CreateMapper();
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

        public AnaerobicDigestionComponent Replicate(AnaerobicDigestionComponent component)
        {
            var replicatedComponent = (AnaerobicDigestionComponent)Activator.CreateInstance(component.GetType());

            return this.Replicate(component, replicatedComponent);
        }

        public AnaerobicDigestionComponent Replicate(AnaerobicDigestionComponent source, AnaerobicDigestionComponent destination)
        {
            _anaerobicDigestionComponentMapper.Map(source, destination);

            throw new NotImplementedException();
        } 

        #endregion
    }
}
