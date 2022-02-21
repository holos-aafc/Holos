using H.CLI.Properties;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.UserInput;
using H.CLI.Processors;
using H.Core.Providers;
using H.Core.Providers.Soil;
using H.CLI.Results;
using System.Globalization;
using H.CLI.Handlers;
using H.Core;
using H.Core.Models;
using H.Core.Services;

namespace H.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowBanner();

            var userCulture = CultureInfo.CurrentCulture;
            CLILanguageConstants.SetCulture(userCulture);

            var directoryHandler = new DirectoryHandler();
            //Farm directory access
            directoryHandler.GetUsersFarmsPath(args);
            var farmsFolderPath = Directory.GetCurrentDirectory();


            var exportedFarmsHandler = new ExportedFarmsHandler();
            
            // Ask for files to import
            var farmsImportedFromGUI = exportedFarmsHandler.Initialize(farmsFolderPath);

            InfrastructureConstants.BaseOutputDirectoryPath = Path.GetDirectoryName(farmsFolderPath);    

            CLIUnitsOfMeasurementConstants.PromptUserForUnitsOfMeasurement();

            var applicationData = new ApplicationData();

            var storage = new Storage();
            var templateFarmHandler = new TemplateFarmHandler();
            var processorHandler = new ProcessorHandler();

            InfrastructureConstants.BaseOutputDirectoryPath = Path.GetDirectoryName(farmsFolderPath);
            //Get The Directories in the "Farms" folder
            var listOfFarmPaths = Directory.GetDirectories(farmsFolderPath);

            //Set up the geographic data provider only once to speed up processing.
            var geographicDataProvider = new GeographicDataProvider();
            geographicDataProvider.Initialize();

            //If the directory exists and there are directories in the Farms folder - meaning there should be at least one farm folder.
            if (Directory.Exists(farmsFolderPath) && listOfFarmPaths.Any())
            {
                //if there is no template farm folder, create one
                templateFarmHandler.CreateTemplateFarmIfNotExists(farmsFolderPath, geographicDataProvider);

                var globalSettingsHandler = new SettingsHandler();
                globalSettingsHandler.InitializePolygonIDList(geographicDataProvider); 
 
                foreach (var farmDirectoryPath in listOfFarmPaths)
                {
                    //Are there any settings files?
                    var settingsFilePathsInFarmDirectory = Directory.GetFiles(farmDirectoryPath, "*.settings").ToList();

                    if (!settingsFilePathsInFarmDirectory.Any())
                    {
                        globalSettingsHandler.GetUserSettingsMenuChoice(farmDirectoryPath, geographicDataProvider);
  
                        //This will be the default name for the farm settings file. The user can change the name of the settings file in the Farm folder if they want to.
                        var defaultFarmSettingsFilePath = farmDirectoryPath + @"\" + Properties.Resources.NameOfSettingsFile + ".settings";

                        //We add it to our list of settings files so we can continue processing with the new settings file
                        settingsFilePathsInFarmDirectory.Add(defaultFarmSettingsFilePath);
                    }

                    //For every farm, we go through each settings file - in case the user wants to test different temperatures or other factors for that specific farm. We will output results
                    //using each settings file in each Farm
                    foreach (var settingsFilePath in settingsFilePathsInFarmDirectory)
                    {
                        if (File.Exists(settingsFilePath))
                        {
                            //Initialize And Validate Directories - Make sure all directories are valid (spelled correctly, created if not there, template files made)
                            directoryHandler.InitializeDirectoriesAndFilesForComponents(farmDirectoryPath);

                            var farmName = Path.GetFileName(farmDirectoryPath);
                            var farmSettingsFileName = Path.GetFileNameWithoutExtension(settingsFilePath);
                            var reader = new ReadSettingsFile();
                            var dataInputHandler = new DataInputHandler();
                            Console.WriteLine(String.Format(Environment.NewLine + Properties.Resources.StartingConversion, Path.GetFileName(farmDirectoryPath)));

                            //Parse And Convert Raw Input Files Into Components and add them to a Farm
                            var farm = dataInputHandler.ProcessDataInputFiles(farmDirectoryPath);
                            farm.CliInputPath = farmDirectoryPath;
                            farm.SettingsFileName = farmSettingsFileName;
                            farm.Name = farmName;
                            farm.MeasurementSystemType = CLIUnitsOfMeasurementConstants.measurementSystem;
                            applicationData.DisplayUnitStrings.SetStrings(farm.MeasurementSystemType);
                            farm.MeasurementSystemSelected = true;

                            //Read Global Settings (ONLY SET ONCE) and other settings (Temperature, Precipitation, Evapotranspiration, Soil) which are specific
                            //for each Farm and therefore, are set for each farm
                            Console.WriteLine(Properties.Resources.ReadingSettingsFile);
                            reader.ReadGlobalSettings(settingsFilePath);
                            globalSettingsHandler.ApplySettingsFromUserFile(ref applicationData, ref farm, reader.GlobalSettingsDictionary);
  
                            //Create a KeyValuePair which takes in the farmDirectoryPath and the Farm itself
                            if (farm.Components.Any())
                            { 
                                applicationData.Farms.Add(farm);

                                //Set up Output Directories For The Land Management Components In The Farm
                                directoryHandler.ValidateAndCreateLandManagementDirectories(InfrastructureConstants.BaseOutputDirectoryPath, farmName);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine(Properties.Resources.FarmDoesNotContainAnyData, farm.Name + "_" + farm.SettingsFileName);
                                System.Threading.Thread.Sleep(2000);
                            }
    
                        }
                        else
                        {
                            throw new Exception(Properties.Resources.InvalidSettingsFilePath);
                        }
                    }
                }

                if (applicationData.Farms.Any())
                {
                    storage.ApplicationData = applicationData;
                    //Start Processing Farms
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine(Properties.Resources.StartingProcessing);

                    //Overall Results For All the Farms
                    var componentResults = new ComponentResultsProcessor(storage, new TimePeriodHelper());

                    //Get base directory of user entered path to create Total Results For All Farms folder
                    Directory.CreateDirectory(InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + Properties.Resources.TotalResultsForAllFarms);

                    //Output Individual Results For Each Farm's Land Management Components (list of components is filtered inside method)
                    //Slowest section because we initialize view models for every component
                    processorHandler.InitializeComponentProcessing(storage.ApplicationData);

                    //Calculate emissions for all farms
                    componentResults.ProcessFarms(storage);

                    //Output all results files
                    componentResults.WriteEmissionsToFiles(applicationData);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Properties.Resources.LabelProcessingComplete);
                    Console.ReadLine();
                    Environment.Exit(1);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Properties.Resources.NoFarmsToProcess);
                    Console.ReadLine();
                    Environment.Exit(1);
                }
            } 

            //There is nothing in the Farms Folder, create a Template Farm and instruct user to populate their folders with data files
            else
            {
                templateFarmHandler.CreateTemplateFarmIfNotExists(farmsFolderPath, geographicDataProvider);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(String.Format(Properties.Resources.InitialMessageAfterInstallation, farmsFolderPath));
                var a = Console.ReadKey();
                Environment.Exit(1);
            }
        }        

        static void ShowBanner()
        {
            Console.WriteLine();
            Console.WriteLine("HOLOS CLI");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}



