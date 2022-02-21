using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.CLI.Converters;
using H.CLI.Parser;
using H.Core.Models;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class DataInputHandler
    {
        #region Fields
        private ParserHandler parserHandler = new ParserHandler();
        private DirectoryHandler _directoryHandler = new DirectoryHandler();
        private ComponentConverterHandler _componentConverter = new ComponentConverterHandler();
        //public Farm Farm { get; set; }
        public Dictionary<string, string> GlobalSettingsDictionary { get; set; } = new Dictionary<string, string>(); 
        #endregion

        #region Public Methods
        /// <summary>
        /// Takes in a path to a farm. For each component directory in that farm prioritize the list of directories and then create a key and value by getting the component type
        /// from the component's directory path. Pass this new string[] into the ParserHandler's InitializeParser method where all the 
        /// files for that component will be parsed
        /// If there is nothing in the parsed list, that means there is nothing to convert, so continue with the next component
        /// Otherwise, start the conversion of the list of parsed components to the appropriate concrete objects and add it to the list of 
        /// components in our Farm object.
        /// After all the files have been parsed and the components have been converted and added to our Farm, process each of these components and perform
        /// the appropriate calculations and output the results 
        /// </summary>
        public Farm ProcessDataInputFiles(string farmDirectoryPath)
        {
                var farm = new Farm()
                {
                    Guid = Guid.NewGuid()
                };

                var componentDirectoryList = Directory.GetDirectories(farmDirectoryPath).ToList();
                _directoryHandler.checkForInvalidComponentDirectoryNames(componentDirectoryList, farmDirectoryPath);

                //Fields and Shelterbelts are prioritized. Priority is set in the Directory Keys Class by number weight
                componentDirectoryList = _directoryHandler.prioritizeDirectoryKeys(componentDirectoryList);
                foreach (var componentDirectoryPath in componentDirectoryList)
                {
                    var componentCategoryType = Path.GetFileName(componentDirectoryPath);
                    var files = Directory.GetFiles(componentDirectoryPath);
            
                    //Parse
                    parserHandler.InitializeParser(componentCategoryType);
                    var parsedComponentList = parserHandler.StartParsing(files);
                
                    //If there is no data in the excel files, then there is nothing to convert, continue with the next component
                    if (parsedComponentList.Count() < 1)
                    {
                        continue;
                    }

                    //Convert the list of parsed components and store them in the farm and then continue on to the next component
                    //The Field and Shelterbelts are converted first.
                    else
                    {
                        var listOfComponents = _componentConverter.StartComponentConversion(componentCategoryType, farm, parsedComponentList);
                        foreach (var component in listOfComponents)
                        {  
                            farm.Components.Add(component);
                        }
                    }

                }
                Console.WriteLine(Properties.Resources.ParsingFinished);
                Console.WriteLine(Properties.Resources.ConversionFinished);
                return farm;
            
        }
        #endregion
    }
}


