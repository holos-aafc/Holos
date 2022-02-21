using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Handlers;
using H.CLI.Interfaces;
using H.CLI.UserInput;

namespace H.CLI.Parser
{
    public class Parser
    {
        #region Fields
        public IComponentKeys ComponentKey { get; set; }
        public IComponentTemporaryInput ComponentTemporaryInput { get; set; }
        private KeyConverter.KeyConverter _converter = new KeyConverter.KeyConverter();
        private ErrorHandler _errorHandler = new ErrorHandler();
        private Dictionary<string, List<IComponentTemporaryInput>> fileToComponentPairList = new Dictionary<string, List<IComponentTemporaryInput>>();
        private KeyValuePair<string, List<IComponentTemporaryInput>> fileToComponentPair;

        #endregion

        #region Public Methods
        /// <summary>
        /// Takes in a list of files in a component's directory and iterates through each .csv file 
        /// (ie, in the Fields directory, we have files such as Field1.csv, Field2.csv,..., Fieldn.csv)
        /// For each file, we will read the contents and retrieve the headers of the .csv file
        /// (the headers will be converted to all UPPERCASE for easier use)
        /// If the filelines have less than 2 rows, that means there is no data to process, so continue with the next file.
        /// Otherwise, for each row in each file, we will create a temporary object based on the type of
        /// the ComponentTemporaryInput that we have set previously when we initialized our parser. This object
        /// is where we will store the data from the .csv file.
        /// Then, we will determine the columnIndex by iterating through our component's keys and searching for their
        /// position in our list of headers and access the appropriate data using the row and columnIndex.
        /// Once the parse has been completed for a row, the tempObject corresponding to that row will be added to a parsedFile.
        /// After each file has been completely parsed, the parsedFile list will be added to a parsedComponentList
        /// which stores a list of all the parsed component files (ie, parsedField1, parsedField2,...parsedFieldn)
        /// </summary>

        public List<List<IComponentTemporaryInput>> Parse(string[] fileList)
        {
            var excel = new ExcelInitializer();
            List<List<IComponentTemporaryInput>> parsedComponentList = new List<List<IComponentTemporaryInput>>();

            foreach (var file in fileList)
            {
                List<IComponentTemporaryInput> parsedFile = new List<IComponentTemporaryInput>();
                if (File.Exists(file))
                {
                    var filelines = excel.ReadExcelFile(file).ToArray();
                    var headers = filelines.First().Select(element => (string)element.ToUpper().Replace(" ", "").Clone()).ToArray();

                    //remove the units wrapped in parentheses
                    for (int i = 0; i < headers.Count(); i++)
                    {
                        if (headers[i].Contains('('))
                        {
                            headers[i] = headers[i].Substring(0, headers[i].IndexOf('('));
                        }
                    }

                    if (filelines.Count() < 2)
                    {
                        Console.WriteLine(String.Format(H.CLI.Properties.Resources.NoFileData, file));
                        continue;
                    }

                    for (int row = 1; row < filelines.Count(); row++)
                    {
                        var tempObject = (IComponentTemporaryInput)Activator.CreateInstance(Type.GetType(ComponentTemporaryInput.ToString()));

                        foreach (var keyValuePair in this.ComponentKey.keys)
                        {
                            try
                            {
                                var columnIndex = Array.FindIndex(headers, x => x.Equals(keyValuePair.Key.ToUpper().Replace(" ", "")));
                                if (columnIndex == -1) ComponentKey.MissingHeaders[keyValuePair.Key] = true;
                                if (CLILanguageConstants.culture.Name == "fr-CA")
                                {
                                    //Used to get English Headers from French Headers
                                    //This has a work around to get the english key probably should be better than the work around.
                                    var convertedToEnglishKey = Properties.Resources.ResourceManager.GetResourceName(keyValuePair.Key, CultureInfo.GetCultureInfo("fr-CA"));
                                    tempObject.ConvertToComponentProperties(convertedToEnglishKey.Replace(" ", ""), keyValuePair.Value, filelines[row][columnIndex].Replace(" ", "").Trim(), row, columnIndex, file);
                                }
                                else
                                {
                                    tempObject.ConvertToComponentProperties(keyValuePair.Key.Replace(" ", ""), keyValuePair.Value, filelines[row][columnIndex].Replace(" ", "").Trim(), row, columnIndex, file);
                                }
                            }

                            catch (IndexOutOfRangeException)
                            {
                                //if you add a header make sure to update this function for the appropriate componentKey
                                bool optional = ComponentKey.IsHeaderOptional(keyValuePair.Key);

                                //if the header isn't optional then throw
                                if (!optional)
                                {
                                    Console.WriteLine(String.Format(H.CLI.Properties.Resources.DataInputMistakeHeader, row, file, keyValuePair.Key));
                                    throw new IndexOutOfRangeException(String.Format(H.CLI.Properties.Resources.DataInputMistakeHeader, row, file, keyValuePair.Key));
                                }

                                //let the user know ONCE per missing header that he/she is missing an optional header and HOLOS is using a default value instead.
                                if (row == 1)
                                {
                                    Console.WriteLine(String.Format(H.CLI.Properties.Resources.DataMissingOptionalHeader, keyValuePair.Key, file));
                                }
                            }
                        }
                        tempObject.FinalSettings(ComponentKey);
                        parsedFile.Add(tempObject);
                    }

                    parsedComponentList.Add(parsedFile);
                    fileToComponentPair = new KeyValuePair<string, List<IComponentTemporaryInput>>(file, parsedFile);

                    //Checks just one file and its list of temporary component objects (rows)
                    _errorHandler.CheckIfEachGroupRefersToOneAnimalGroupType(fileToComponentPair);

                    Console.ResetColor();
                    Console.WriteLine(String.Format(H.CLI.Properties.Resources.FollowingFileHasBeenParsed, file));
                }

                else
                    continue;
            }
            //Checks all files and their list of temporary component objects
            _errorHandler.CheckIfComponentsHaveTheSameName(fileToComponentPairList);

            Console.ResetColor();

            return parsedComponentList;

        }
        #endregion
    }
}

