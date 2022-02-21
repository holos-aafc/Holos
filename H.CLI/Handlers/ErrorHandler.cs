using System;
using System.Collections.Generic;
using System.Linq;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Interfaces;
using H.CLI.TemporaryComponentStorage;

namespace H.CLI.Handlers
{
    #region Structs
    public class TempComponent
    {
        public string Name;
        public string FileName;
        public bool Displayed { get; set; }
    };

    #endregion

    public class ErrorHandler
    {
        #region Fields
        private TemplateFileHandler templateFileHandler = new TemplateFileHandler();
        #endregion

        #region Public Methods
        /// <summary>
        /// This method checks if Component Files are referring to the same Component based on the Component Name. If the 
        /// Component Names are not unique,, an error will be thrown, otherwise the program will continue
        /// The method takes in a dictionary that consists of the file path and a match to the associated List<ComponentTemporaryInput> for
        /// all the files in a Component Directory (i.e. Shelterbelt1.csv, Shelterbelt2.csv ...)
        /// The list is the container for the objects that have been parsed from one file in the Component Directory - meaning
        /// it represents one of the Excel files in a Component Directory (i.e. A Shelterbelt1.csv file in the Shelterbelts Component Directory)
        /// The Algorithm is as follows:
        /// 1. Iterate through the key/value pairs in the dictionary
        /// 2. For every key/value pair, determine if the name of that Component is present in the uniqueName list.
        /// 3. If the current Component's List Name matches a Name in the uniqueName list, retrieve the Name in the uniqueName list
        /// and output an exception that displays the current Component's List file path and the uniqueName Name that it has been compared to
        /// If the unique Name has already been outputed, we do not output the error message again and set the TempComponents Displayed field
        /// to true because it has already been displayed once.
        /// 4. Otherwise, create a new TempComponent that refers to the current Components Name and FilePath and set the Displayed field to false
        /// and add it to the list of uniqueNames and continue processing the key/value pairs.
        /// </summary>
        /// <param name="fileToComponentList"></param>
        public void CheckIfComponentsHaveTheSameName(Dictionary<string, List<IComponentTemporaryInput>> fileToComponentList)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;

            var uniqueName = new List<TempComponent>();
            var counter = 0;

            string typeOfComponent = string.Empty;
            foreach (var componentList in fileToComponentList)
            {
                typeOfComponent = componentList.Value[0].GetType().ToString();
                if (uniqueName.Exists(x => x.Name == componentList.Value[0].Name))
                {
                    var indexOfUniqueNameUsedForComparison = uniqueName.FindIndex(x => x.Name == componentList.Value[0].Name);
                    Console.WriteLine(String.Format(Properties.Resources.SameNameError, typeOfComponent, componentList.Key, uniqueName[indexOfUniqueNameUsedForComparison].Name));
                    Console.WriteLine(Environment.NewLine);
                    counter++;

                    if (!uniqueName[indexOfUniqueNameUsedForComparison].Displayed)
                    {
                        counter++;
                        Console.WriteLine(String.Format(Properties.Resources.SameNameError, typeOfComponent, uniqueName[indexOfUniqueNameUsedForComparison].FileName, uniqueName[indexOfUniqueNameUsedForComparison].Name));
                        Console.WriteLine(Environment.NewLine);
                        uniqueName[indexOfUniqueNameUsedForComparison].Displayed = true;
                    }
                }
                else
                {
                    var tempComponent = new TempComponent();
                    tempComponent.Name = componentList.Value[0].Name;
                    tempComponent.FileName = componentList.Key;
                    tempComponent.Displayed = false;
                    uniqueName.Add(tempComponent);
                }
            }
            if (counter > 0)
            {
                throw new FormatException(String.Format(Properties.Resources.SameNameErrorExceptionMessage, counter, typeOfComponent));
            }

        }
        
        public void CheckIfEachGroupRefersToOneAnimalGroupType(KeyValuePair<string, List<IComponentTemporaryInput>> fileToListOfDataRows)
        {
            if (fileToListOfDataRows.Value[0].GetType() == typeof(FieldTemporaryInput) ||
               fileToListOfDataRows.Value[0].GetType() == typeof(ShelterBeltTemporaryInput))
            {
                return;
            }
         
            var splitByGroupName = fileToListOfDataRows.Value
                                                 //Group list of results by Group Name
                                                 .GroupBy(data => data.GroupName)
                                                 //Check if there is more than one row of data referring to that group name
                                                 .Where(groupedByName => groupedByName.Count() > 1).ToList();

            if (splitByGroupName.Any())
            {
                foreach (var group in splitByGroupName)
                {
                    var checkForMultipleGroupTypesInARow = group.GroupBy(x => x.GroupType)
                                               //If there are more than two groups referring to a row, it is invalid
                                               .Where(y => y.Count() < 2)
                                               .ToList();
                    if (checkForMultipleGroupTypesInARow.Any())
                    {
                        var combineInvalidGroupTypesString = string.Join(",", checkForMultipleGroupTypesInARow.SelectMany(x => x.Select(y => y.GroupType.ToString())));
                        Console.WriteLine(String.Format(Properties.Resources.DuplicateAnimalGroupTypesReferringToSameAnimalGroup, combineInvalidGroupTypesString, group.Key, fileToListOfDataRows.Key));
                        Console.WriteLine(Environment.NewLine);
                        throw new Exception(String.Format(Properties.Resources.DuplicateAnimalGroupTypesReferringToSameAnimalGroup, combineInvalidGroupTypesString, group.Key, fileToListOfDataRows.Key));
                    }
                }
            }
           
        }

       
        #endregion

    }
}


 

