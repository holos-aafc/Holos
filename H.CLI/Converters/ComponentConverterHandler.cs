using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using H.CLI.Factories;
using H.CLI.Handlers;
using H.CLI.Interfaces;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.Animals.OtherAnimals;
using H.Core.Models.Animals.Poultry;
using H.Core.Models.Animals.Sheep;
using H.Core.Models.Animals.Swine;

namespace H.CLI.Converters
{
    /// <summary>
    /// This class is responsible for setting the proper concrete converter class for the component we are parsing and has two methods: one to start the conversion based on
    /// the concrete converter returned from the ComponentConverterFactory and the second which sets the Row Guids of the component (For example, with a Shelterbelt we have
    /// a Row ID that corresponds with a bunch of Tree Group data or for a Field, if the CropType is a perennial, it will have a PerennialStandGroupId.
    /// </summary>
    public class ComponentConverterHandler
    {
        #region Fields
        public ConvertingComponentStrategy _converterStrat { get; set; } = new ConvertingComponentStrategy();
        private ComponentConverterFactory _converterFactory = new ComponentConverterFactory();
        private GuidComponentHandler guidHandler = new GuidComponentHandler();  
      
        #endregion

        #region Constructors
        public ComponentConverterHandler() { }
        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the conversion of the components after the components files have been parsed. The parsed components results
        /// are passed into the converter's SetComponentListGuid, which will return a new list with the appropriate GUID's set
        /// Then, the conversion will begin by passing in the new GUID list to the StartComponentConversion method and the 
        /// resulting component(s) will be added to a list of farm components (of type ComponentBase).
        /// </summary>

        public List<ComponentBase> StartComponentConversion(string componentType, Farm farm, List<List<IComponentTemporaryInput>> tempComponentList)
        {
            var componentConverter = new ComponentConverterHandler();
            _converterStrat.SetConvertingComponentStrategy(_converterFactory.GetComponentConverter(componentType));

            var parsedComponentResultsWithGuidSet = componentConverter.SetComponentListGuid(tempComponentList);
            return _converterStrat.ConvertComponent(parsedComponentResultsWithGuidSet, farm);
           
        }
      
        /// <summary>
        /// Takes in a list of parsed components that do not have their GUIDs properly set as a List<List<IComponentTemporaryInput>>
        /// For each component (ie. Field1.csv), we will iterate through its rows and set the appropriate GUID's and add that to a 
        /// temporary list of parsed components that have their GUID's set. Then, we will return the temporary list.
        /// </summary>
        public List<List<IComponentTemporaryInput>> SetComponentListGuid(List<List<IComponentTemporaryInput>> parsedComponents)
        {
          
            var tempGUIDList = new List<List<IComponentTemporaryInput>>();
            foreach (var component in parsedComponents)
            {
                var GuidHasBeenSetComponentList = guidHandler.GenerateComponentGUIDs(component);
                tempGUIDList.Add(GuidHasBeenSetComponentList);
            }

            return tempGUIDList;
        }
        
        public AnimalComponentBase GetAnimalComponentFromComponentTypeString(string componentTypeString)
        {
            var assembly = Assembly.GetAssembly(typeof(ComponentBase)).FullName.Split(new char[]{','})[0];

            var type = Type.GetType($"{componentTypeString}, {assembly}");

            var component = (AnimalComponentBase)Activator.CreateInstance(type);

            return component;
        }
        #endregion
    }

}
    

