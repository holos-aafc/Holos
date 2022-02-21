using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using H.CLI.TemporaryComponentStorage;
using H.Core.Models;
using H.Core.Providers;

namespace H.CLI.FileAndDirectoryAccessors
{
    public class DirectoryHandler
    {
        #region Fields

        private readonly InputHelper _inputHelper = new InputHelper();
        private readonly DirectoryKeys _directoryKeys = new DirectoryKeys();
        private readonly TemplateFileHandler _templateFileHandler = new TemplateFileHandler();
        private readonly KeyConverter.KeyConverter _keyConverter = new KeyConverter.KeyConverter();

        #endregion

        #region Constructors
        public DirectoryHandler() { }
        #endregion

        #region Public Methods

        /// <summary>
        /// Takes in a farmDirectoryPath, if the directory does not exist, throw a DirectoryNotFoundException
        /// Otherwise, inside the farmDirectory, we will validate that the appropriate folders are present (such as a folder for Shelterbelts, Fields, etc...)
        /// After the validation of directories, we will validate if each directory has a valid template file.
        /// </summary>
        public void InitializeDirectoriesAndFilesForComponents(string farmDirectoryPath)
        {

            if (!Directory.Exists(farmDirectoryPath))
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                throw new DirectoryNotFoundException(String.Format(Properties.Resources.InvalidDirectory, farmDirectoryPath));
            }

            ValidateComponentDirectories(farmDirectoryPath);
            var componentDirectories = Directory.GetDirectories(farmDirectoryPath).ToList();
            var validComponentDirectories = prioritizeDirectoryKeys(componentDirectories);
            _templateFileHandler.validateTemplateFiles(validComponentDirectories);

        }

        public void checkForInvalidComponentDirectoryNames(List<string> componentDirectoryPathsInAFarm, string farmDirectoryPath)
        {
            var invalidDirectories = componentDirectoryPathsInAFarm.Where(x => !_directoryKeys.directoryWeights.Select(y => y.Key.ToUpper()).Contains(Path.GetFileName(x).ToUpper())).ToList();
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (invalidDirectories.Any())
            {
                foreach (var invalidDirectoryName in invalidDirectories)
                {
                    Console.WriteLine(String.Format(Properties.Resources.InvalidComponentDirectoryName, Path.GetFileName(invalidDirectoryName), farmDirectoryPath));
                }

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(Properties.Resources.ListOfValidDirectories);
                foreach (var validDirectory in _directoryKeys.directoryWeights.Keys)
                {

                    Console.WriteLine(validDirectory);
                }

                //Currently Pauses so you can run the program. When fully finished, throw an exception instead
                System.Threading.Thread.Sleep(500);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// This method takes in a list of componentDirectoryPathsInAFarm and orders them by weight - which is specified in the
        /// DirectoryKeys class. Shelterbelts and Fields are a higher weight because they do not depend on
        /// other Components to be able to perform calculations. However, some other components such as
        /// Swines, if the user selects their HousingType to be pasture, they will potentially need to be refer to one of their
        /// created Field Components, which needs to be processed first in order to use it in that Swine component.
        /// </summary>
        public List<string> prioritizeDirectoryKeys(List<string> componentDirectoryPathsInAFarm)
        {
            var validDirectories = componentDirectoryPathsInAFarm.Where(x => _directoryKeys.directoryWeights.Select(y => y.Key.ToUpper()).Contains(Path.GetFileName(x).ToUpper())).ToList();
            return validDirectories.OrderByDescending(x => _directoryKeys.directoryWeights.ContainsKey(Path.GetFileName(x))).ToList();
        }

        /// <summary>
        /// Takes in a farmDirectoryPath. For each key in the list of Directory Keys, if the folder does not exist corresponding to that key,
        /// create a new Directory using that Directory Path. For example, if C:/Holos/Farms\Farm1\Shelterbelts does not exist, it will make
        /// a new directory using that path. Otherwise if the path exists, continue iterating through the next key.
        /// </summary>
        public void ValidateComponentDirectories(string root)
        {
            foreach (var key in _directoryKeys.directoryWeights.Keys)
            {
                var componentDirectoryPath = Path.GetFullPath(String.Format(@"{0}\{1}", root, key));
                if (!Directory.Exists(componentDirectoryPath))
                {
                    Directory.CreateDirectory(componentDirectoryPath);
                }
                else continue;
            }
        }


        public void ValidateAndCreateLandManagementDirectories(string baseOutputDirectory, string farmName)
        {
            var listOfLandManagements = _directoryKeys.directoryWeights.Keys.Where(x => x == Properties.Resources.DefaultShelterbeltInputFolder || x == Properties.Resources.DefaultFieldsInputFolder);
            var pathToFarmDirectory = baseOutputDirectory + @"\" + Properties.Resources.Outputs + @"\" + farmName + Properties.Resources.Results;
            foreach (var key in listOfLandManagements)
            {
                var landManagementComponentPath = Path.GetFullPath(String.Format(@"{0}\{1}", pathToFarmDirectory, key));
                if (!Directory.Exists(landManagementComponentPath))
                {
                    Directory.CreateDirectory(landManagementComponentPath);
                }
            }
        }

        /// <summary>
        /// Takes in a farmDirectoryPath where the farm does not have a settings file present. This method will create a new
        /// global settings file for the farm based on a key where the list of global settings is stored. If you want to add a 
        /// new global setting, please modify the BuildSettingsFileString class.
        /// </summary>
        public void GenerateGlobalSettingsFile(string farmDirectoryPath, Farm farm)
        {
            BuildSettingsFileString settingsFileString = new BuildSettingsFileString(farm);

            var farmSettingsFilePath = farmDirectoryPath + @"\" + Properties.Resources.LabelFarm + ".settings";

            var stringBuilder = new StringBuilder();
            foreach (var key in settingsFileString.keys)
            {
                if (key.StartsWith("#"))
                {
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append(String.Format("\"{0}\"", key));
                    stringBuilder.Append(Environment.NewLine);
                    continue;
                }

                var keyAndValue = key.Split('=');
                stringBuilder.Append(keyAndValue[0].Trim() + " = " + keyAndValue[1].Replace(" ", "").Trim());
                stringBuilder.Append(Environment.NewLine);
            }

            File.WriteAllText(farmSettingsFilePath, stringBuilder.ToString().Replace("\"", "").Trim());
        }

        /// <summary>
        /// Read the txt file located in @"FarmsPathFile" + @"\" + "UserFarmsPath.txt" which stores
        ///the users previously entered Farms Directory Path
        /// </summary>
        public string ReadUserFarmsPath(string usersFarmsPath)
        {
            using (FileStream fs = new FileStream(usersFarmsPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    if (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        return line;
                    }

                    else
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Prompts the user for the location of their Farms Directory
        /// </summary>  
        public string PromptForUsersFarmsDirectory()
        {
            string farmsFolderPath = string.Empty;
            Console.ForegroundColor = ConsoleColor.Green;
            do
            {
                Console.WriteLine(Properties.Resources.PromptForFarmsFolderLocation);
                farmsFolderPath = Console.ReadLine();
            } while (!Directory.Exists(farmsFolderPath));
            File.WriteAllText(@"FarmsPathFile" + @"\" + "UserFarmsPath.txt", farmsFolderPath);
            return farmsFolderPath;
        }

        /// <summary>
        /// Contains the logic to get the users Farms Directory Path and creates our txt file to store the users Farms Directory Path
        /// </summary>
        public void GetUsersFarmsPath(string[] args)
        {
            //Check if our data file that stores the Users Farms Path is made or not, if it isn't make the directory.
            Directory.CreateDirectory("FarmsPathFile");

            //Check if our data file directory contains our file to store the users results. If not, create the file
            if (!Directory.GetFiles("FarmsPathFile").Any())
            {
                File.Create(@"FarmsPathFile" + @"\" + "UserFarmsPath.txt").Close();
            }

            //Read the data file which stores the users previous directory.
            var previousFarmsFolderPath = ReadUserFarmsPath(@"FarmsPathFile" + @"\" + "UserFarmsPath.txt");

            //If they have entered a command line argument when running the HCLI
            if (args.Any())
            {
                Directory.SetCurrentDirectory(args[0]);
            }

            //If there is no command line argument and there is no previous directory (first time running), 
            //prompt the user for the location of their Farms directory 
            if (!args.Any() && previousFarmsFolderPath == string.Empty)
            {
                var farmsFolderPath = PromptForUsersFarmsDirectory();
                Directory.SetCurrentDirectory(farmsFolderPath);
            }

            //If there is no command line argument and there is a previous Farms directory entered
            if (!args.Any() && previousFarmsFolderPath != string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                var usePreviousDirectory = string.Empty;
                do
                {
                    Console.Write(String.Format(Properties.Resources.PromptUserToUsePreviousDirectory, previousFarmsFolderPath));
                    Console.WriteLine(Properties.Resources.LabelYesNo);

                    usePreviousDirectory = Console.ReadLine();
                    if (_inputHelper.IsYesResponse(usePreviousDirectory))
                    {
                        Directory.SetCurrentDirectory(previousFarmsFolderPath);
                        break;
                    }

                    else if (_inputHelper.IsNoResponse(usePreviousDirectory))
                    {
                        var farmsFolderPath = PromptForUsersFarmsDirectory();
                        Directory.SetCurrentDirectory(farmsFolderPath);
                        break;
                    }

                } while (_inputHelper.IsYesResponse(usePreviousDirectory) == false || _inputHelper.IsNoResponse(usePreviousDirectory) == false);

                Console.WriteLine();
            }

        }
        #endregion
    }

}

