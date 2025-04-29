using H.CLI.Properties;
using System;
using System.Deployment;
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
using H.CLI.Interfaces;
using H.Core;
using H.Core.Models;
using H.Core.Services;
using System.Collections.Generic;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Providers.Climate;
using H.Core.Services.Initialization;
using H.Core.Services.LandManagement;

namespace H.CLI
{
    class Program
    {
        #region Fields

        private static Storage _storage;
        private static ClimateProvider _climateProvider;
        private static SlcClimateDataProvider _slcClimateDataProvider;
        private static N2OEmissionFactorCalculator _n2OEmissionFactorCalculator;
        private static ICBMSoilCarbonCalculator _iCbmSoilCarbonCalculator;
        private static IPCCTier2SoilCarbonCalculator _ipccSoilCarbonCalculator;
        private static InitializationService _initializationService;
        private static FieldResultsService _fieldResultsService;
        private static ExportedFarmsHandler _exportedFarmsHandler;
        private static SettingsHandler _globalSettingsHandler;
        private static ProcessorHandler _processorHandler;
        private static DataInputHandler _dataInputHandler;

        #endregion

        #region Constructors

        static Program()
        {
            ShowBanner();

            _storage = new Storage();
            _slcClimateDataProvider = new SlcClimateDataProvider();
            _climateProvider = new ClimateProvider(_slcClimateDataProvider);
            _n2OEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            _iCbmSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator); 
            _ipccSoilCarbonCalculator = new IPCCTier2SoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            _initializationService = new InitializationService();
            _fieldResultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipccSoilCarbonCalculator, _n2OEmissionFactorCalculator, _initializationService);
            _exportedFarmsHandler = new ExportedFarmsHandler(_fieldResultsService, _climateProvider, _storage);
            _globalSettingsHandler = new SettingsHandler(_climateProvider);
            _processorHandler = new ProcessorHandler(_fieldResultsService);
            _dataInputHandler = new DataInputHandler();
        }

        #endregion

        #region Methods

        public static string GetVersionString()
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                return "1.0";
            }
        }

        static void Main(string[] args)
        {
            // CLI arguments access 
            CLIArguments argValues = new CLIArguments();
            argValues.ParseArgs(args);

            // CLI Language 
            var userCulture = CultureInfo.CurrentCulture;
            CLILanguageConstants.SetCulture(userCulture);

            // Farm directory access
            var directoryHandler = new DirectoryHandler();
            directoryHandler.GetUsersFarmsPath(args);
            var farmsFolderPath = Directory.GetCurrentDirectory();

            // Farms exported from GUI access
            List<string> generatedFarmFolders = new List<string>();

            // Separate initialize functions with and without CLI arguments
            if (argValues.FileName != string.Empty || argValues.FolderName != string.Empty)
            {
                generatedFarmFolders = _exportedFarmsHandler.InitializeWithCLArguements(farmsFolderPath, argValues);
            }
            else
            {
                _ = _exportedFarmsHandler.Initialize(farmsFolderPath);
            }

            // Ensure output directory is not a network drive 
            DriveInfoWrapper givenPathDriveInfoWrapper = null;
            if (!string.IsNullOrEmpty(argValues.OutputPath))
            {
                DriveInfo givenPathDriveInfo = new DriveInfo(argValues.OutputPath);
                givenPathDriveInfoWrapper = new DriveInfoWrapper(givenPathDriveInfo);
            }
            InfrastructureConstants.CheckOutputDirectoryPath(argValues.OutputPath, givenPathDriveInfoWrapper, farmsFolderPath);

            // Units of measurement access
            CLIUnitsOfMeasurementConstants.PromptUserForUnitsOfMeasurement(argValues.Units);

            var applicationData = new ApplicationData();
            
            var templateFarmHandler = new TemplateFarmHandler();

            // Get The Directories in the "Farms" folder
            var listOfFarmPaths = directoryHandler.GetListOfFarms(farmsFolderPath, argValues, _exportedFarmsHandler.pathToExportedFarm, generatedFarmFolders);

            // Set up the geographic data provider only once to speed up processing.
            var geographicDataProvider = new GeographicDataProvider();
            geographicDataProvider.Initialize();

            // If the directory exists and there are directories in the Farms folder - meaning there should be at least one farm folder.
            if (Directory.Exists(farmsFolderPath) && listOfFarmPaths.Any())
            {
                //if there is no template farm folder, create one
                //templateFarmHandler.CreateTemplateFarmIfNotExists(farmsFolderPath, geographicDataProvider);

                _globalSettingsHandler.InitializePolygonIDList(geographicDataProvider);

                foreach (var farmDirectoryPath in listOfFarmPaths)
                {
                    //Are there any settings files?
                    var settingsFilePathsInFarmDirectory = Directory.GetFiles(farmDirectoryPath, "*.settings").ToList();

                    if (!settingsFilePathsInFarmDirectory.Any())
                    {
                        _globalSettingsHandler.GetUserSettingsMenuChoice(farmDirectoryPath, geographicDataProvider);

                        // This will be the default name for the farm settings file. The user can change the name of the settings file in the Farm folder if they want to.
                        var defaultFarmSettingsFilePath = farmDirectoryPath + @"\" + Properties.Resources.NameOfSettingsFile + ".settings";

                        // We add it to our list of settings files so we can continue processing with the new settings file
                        settingsFilePathsInFarmDirectory.Add(defaultFarmSettingsFilePath);
                    }

                    //For every farm, we go through each settings file - in case the user wants to test different temperatures or other factors for that specific farm. We will output results
                    //using each settings file in each Farm
                    foreach (var settingsFilePath in settingsFilePathsInFarmDirectory)
                    {
                        if (File.Exists(settingsFilePath))
                        {
                            // Initialize And Validate Directories - Make sure all directories are valid (spelled correctly, created if not there, template files made)
                            directoryHandler.InitializeDirectoriesAndFilesForComponents(farmDirectoryPath);

                            var farmName = Path.GetFileName(farmDirectoryPath);
                            var farmSettingsFileName = Path.GetFileNameWithoutExtension(settingsFilePath);
                            var reader = new ReadSettingsFile();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(String.Format(Environment.NewLine + Properties.Resources.StartingConversion, Path.GetFileName(farmDirectoryPath)));

                            // Parse And Convert Raw Input Files Into Components and add them to a Farm
                            var farm = _dataInputHandler.ProcessDataInputFiles(farmDirectoryPath);
                            farm.IsCommandLineMode = true;
                            farm.CliInputPath = farmDirectoryPath;
                            farm.SettingsFileName = farmSettingsFileName;
                            farm.Name = farmName;
                            farm.MeasurementSystemType = CLIUnitsOfMeasurementConstants.measurementSystem;
                            applicationData.DisplayUnitStrings.SetStrings(farm.MeasurementSystemType);
                            farm.MeasurementSystemSelected = true;

                            // Read Global Settings (ONLY SET ONCE) and other settings (Temperature, Precipitation, Evapotranspiration, Soil) which are specific
                            // for each Farm and therefore, are set for each farm
                            Console.WriteLine(Properties.Resources.ReadingSettingsFile);
                            reader.ReadGlobalSettings(settingsFilePath);
                            _globalSettingsHandler.ApplySettingsFromUserFile(ref applicationData, ref farm, reader.GlobalSettingsDictionary);

                            // Create a KeyValuePair which takes in the farmDirectoryPath and the Farm itself
                            if (farm.Components.Any())
                            {
                                applicationData.Farms.Add(farm);

                                // Set up Output Directories For The Land Management Components In The Farm
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
                    _storage.ApplicationData = applicationData;
                    // Start Processing Farms
                    Console.WriteLine();
                    Console.WriteLine(Properties.Resources.StartingProcessing);

                    // Overall Results For All the Farms
                    var componentResults = new ComponentResultsProcessor(_storage, new TimePeriodHelper(), _fieldResultsService, _n2OEmissionFactorCalculator);

                    // Get base directory of user entered path to create Total Results For All Farms folder
                    Directory.CreateDirectory(InfrastructureConstants.BaseOutputDirectoryPath + @"\" + Properties.Resources.Outputs + @"\" + Properties.Resources.TotalResultsForAllFarms);

                    // Output Individual Results For Each Farm's Land Management Components (list of components is filtered inside method)
                    // Slowest section because we initialize view models for every component
                    _processorHandler.InitializeComponentProcessing(_storage.ApplicationData);

                    // Calculate emissions for all farms
                    componentResults.ProcessFarms(_storage);

                    // Output all results files
                    componentResults.WriteEmissionsToFiles(applicationData);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Properties.Resources.LabelProcessingComplete);
                    Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Environment.Exit(1);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Properties.Resources.NoFarmsToProcess);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadLine();
                    Environment.Exit(1);
                }
            }
            else
            {
                /*
                 * There is nothing in the Farms Folder, create a Template Farm and instruct user to populate their folders with data files
                 */

                templateFarmHandler.CreateTemplateFarmIfNotExists(farmsFolderPath, geographicDataProvider);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(String.Format(Properties.Resources.InitialMessageAfterInstallation, farmsFolderPath));
                _ = Console.ReadKey();
                Environment.Exit(1);
            }
        }

        static void ShowBanner()
        {
            Console.WriteLine();
            Console.WriteLine("HOLOS CLI " + "(" + GetVersionString() + ")");
            Console.WriteLine("Loading data please wait...");
            Console.WriteLine();
            Console.WriteLine();
        } 
        #endregion
    }
}




