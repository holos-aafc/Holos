using System;
using System.Collections.Generic;
using System.Diagnostics;
using H.CLI.Factorys;
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
        
        public AnimalComponentBase GetAnimalComponentFromAnimalType(AnimalType animalType)
        {
            switch (animalType)
            {
                //Swines
                case AnimalType.SwineStarter:
                    return new FarrowToWeanComponent();

                case AnimalType.SwineFinisher:
                    return new SwineFinishersComponent();

                case AnimalType.SwineGrower:
                    return new GrowerToFinishComponent();

                case AnimalType.SwineDrySow:
                    return new DrySowsComponent();

                case AnimalType.SwineLactatingSow:
                    return new LactatingSowsComponent();

                case AnimalType.SwineBoar:
                   return new BoarComponent();

                //Dairy - all of these animal group types belong to a component (for now). In the future, they may make up a set of differing components
                case AnimalType.DairyLactatingCow:
                case AnimalType.DairyCalves:
                case AnimalType.DairyHeifers:
                case AnimalType.DairyDryCow:
                    return new DairyComponent();

                //Beef Production
                case AnimalType.BeefBackgrounderHeifer:
                    return new BackgroundingComponent();

                case AnimalType.BeefBackgrounderSteer:
                    return new BackgroundingComponent();

                case AnimalType.CowCalf:
                case AnimalType.BeefCalf:
                case AnimalType.BeefCowDry:
                case AnimalType.BeefBulls:
                case AnimalType.BeefReplacementHeifers:
                case AnimalType.BeefCowLactating:
                    return new CowCalfComponent();   

                case AnimalType.BeefFinisher:
                case AnimalType.BeefFinishingHeifer:
                case AnimalType.BeefFinishingSteer:
                    return new FinishingComponent();

                //Poultry
                case AnimalType.Turkeys:
                    return new PoultryTurkeysComponent();

                case AnimalType.Ducks:
                    return new PoultryDucksComponent();

                case AnimalType.LayersDryPoultry:
                    return new PoultryLayersDryComponent();

                case AnimalType.LayersWetPoultry:
                    return new PoultryLayersWetComponent();

                case AnimalType.Broilers:
                    return new PoultryBroilersComponent();

                case AnimalType.Geese:
                    return new PoultryGeeseComponent();

                //Sheep
                case AnimalType.Sheep:
                    return new SheepComponent();

                case AnimalType.LambsAndEwes:
                case AnimalType.Lambs:
                case AnimalType.Ewes:
                case AnimalType.WeanedLamb:
                    return new EwesAndLambsComponent();

                case AnimalType.Ram:
                    return new RamsComponent(); 

                case AnimalType.SheepFeedlot:
                    return new SheepFeedlotComponent();

                //Other Livestock
                case AnimalType.Alpacas:
                    return new AlpacaComponent();

                case AnimalType.Goats:
                    return new GoatsComponent();

                case AnimalType.Deer:
                    return new DeerComponent();

                case AnimalType.Elk:
                    return new ElkComponent();

                case AnimalType.Horses:
                    return new HorsesComponent();

                case AnimalType.Mules:
                    return new MulesComponent();

                case AnimalType.Bison:
                    return new BisonComponent();

                case AnimalType.Llamas:
                    return new LlamaComponent();

                default:
                {
                    throw new Exception($"{nameof(ComponentConverterHandler)}.{nameof(GetAnimalComponentFromAnimalType)} unknown animal type: '{animalType}', unable to convert to an instance of a component");
                }
            }

        }
        #endregion
    }

}
    

