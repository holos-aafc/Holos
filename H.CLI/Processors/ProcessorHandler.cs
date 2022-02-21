using H.CLI.Factorys;
using H.CLI.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models;

namespace H.CLI.Processors
{
    public class ProcessorHandler
    {
        #region Fields
        private ComponentProcessorFactory processorFactory = new ComponentProcessorFactory();
        public IProcessor _processor { get; set; }
        #endregion

        #region Public Methods

        public void SetProccessor(IProcessor processor)
        {
            _processor = processor;
        }

        /// <summary>
        /// Takes in a list of KeyValuePairs in the format: farmDirectoryPath, Farm
        /// For each farm, we will create a seenComponentTypes list which will store the first
        /// encounter of any unique Component type in that list.
        /// For each Land Management component in our farm, we get the type of Component and determine if it is a unique Component (does it exist in our
        /// seen components list?), if it does not, that means those components have not been processed,
        /// then group together all the components of that type and process those components and then
        /// add the current Component Type to the seen Component list so we do not continue processing any instance of that Component again.
        /// </summary>

        public void InitializeComponentProcessing(ApplicationData applicationData)
        {
            foreach (var currentFarm in applicationData.Farms)
            {
                var seenComponentTypes = new List<Type>();
                Console.WriteLine(String.Format(Properties.Resources.StartingIndividualComponentProcessing, currentFarm.Name));
                foreach (var component in currentFarm.Components)
                {
                    //Later on if more Components have different output files, add their categories here or filter the list somewhere else
                    if (component.ComponentCategory == ComponentCategory.LandManagement)
                    {
                        var currentType = component.GetType();
                        var componentTypeAlreadyExists = seenComponentTypes.Where(x => x == currentType);

                        //If a Component has not been "seen" (added to our seenComponentTypes list, group all Components in the Farm that 
                        //are of the same type as our current Component and process them. Then, add the current Component's type to the
                        //seenComponentTypes list so we do not keep processing components of the same type.
                        if (!componentTypeAlreadyExists.Any())
                        {
                            SetProccessor(processorFactory.GetComponentProcessor(component.GetType()));
                            var componentList = currentFarm.Components.Where(x => x.GetType() == component.GetType()).ToList();
                            _processor.ProcessComponent(currentFarm, componentList, applicationData);
                            seenComponentTypes.Add(currentType);
                        }
                    }
                }
            }
        } 
            #endregion
        }
    }

