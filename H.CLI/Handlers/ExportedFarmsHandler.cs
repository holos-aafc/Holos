using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using H.CLI.ComponentKeys;
using H.CLI.Converters;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Processors;
using H.CLI.TemporaryComponentStorage;
using H.CLI.UserInput;
using H.Core;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Climate;
using H.Core.Calculators.Tillage;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.LandManagement;

namespace H.CLI.Handlers
{
    public class ExportedFarmsHandler
    {
        #region Fields

        private readonly InputHelper _inputHelper = new InputHelper();
        private readonly Storage _storage = new Storage();
        private readonly FieldProcessor _fieldProcessor = new FieldProcessor();
        private readonly DirectoryHandler _directoryHandler = new DirectoryHandler();
        private readonly ExcelInitializer _excelInitializer = new ExcelInitializer();
        private readonly DirectoryKeys _directoryKeys = new DirectoryKeys();
        private readonly SettingsHandler _settingsHandler = new SettingsHandler();

        private readonly BeefConverter _beefConverter = new BeefConverter();
        private readonly DairyConverter _dairyConverter = new DairyConverter();
        private readonly SwineConverter _swineConverter = new SwineConverter();
        private readonly SheepConverter _sheepConverter = new SheepConverter();
        private readonly PoultryConverter _poultryConverter = new PoultryConverter();
        private readonly OtherLiveStockConverter _otherLiveStockConverter = new OtherLiveStockConverter();

        private readonly IFieldResultsService _fieldResultsService = new FieldResultsService();

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="farmsFolderPath">The root directory that will contain all of the farms</param>
        public List<Farm> Initialize(string farmsFolderPath, CLIArguments argValues)
        {
            // Check if exported farm is given via command line argument
            if (argValues.FileName != "")
            {
                if (InitializeWithCLArguements(farmsFolderPath, argValues))
                {
                    return null;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Properties.Resources.InputFileNotFound, argValues.FileName);
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }
            
            var pathToExportedFarms = this.PromptUserForLocationOfExportedFarms(farmsFolderPath);
            if (string.IsNullOrWhiteSpace(pathToExportedFarms))
            {
                // User specified no farms to import
                return new List<Farm>();
            }

            var farms = this.GetExportedFarmsFromUserSpecifiedLocation(pathToExportedFarms);

            var inputFilesForAllFarms = new List<string>();
            foreach (var farm in farms)
            {
                // Create input files for all components in farm
                var componentFilesForFarm = this.CreateInputFilesForFarm(farmsFolderPath, farm, null);

                inputFilesForAllFarms.AddRange(componentFilesForFarm);
            }

            Console.WriteLine();
            Console.WriteLine(string.Format(Properties.Resources.InterpolatedTotalFarmsSuccessfullyImported, farms.Count));
            Console.WriteLine(string.Format(Properties.Resources.LabelInputFilesHaveBeenCreatedAndStoredInYourFarmsDirectory, inputFilesForAllFarms.Count));

            return farms;
        }

        public bool InitializeWithCLArguements(string farmsFolderPath, CLIArguments argValues)
        {
            bool isExportedFarmFound = false;
            var files = Directory.GetFiles(farmsFolderPath);
            string path = string.Empty;
            foreach (var myFile in files)
            {
                if (argValues.FileName == Path.GetFileName(myFile))
                {
                    path = myFile;
                    isExportedFarmFound = true;
                    break;
                }
            }
            if (isExportedFarmFound)
            {
                var farms = _storage.GetFarmsFromExportFile(path);
                var farmsList = farms.ToList();
                _ = this.CreateInputFilesForFarm(farmsFolderPath, farmsList[0], argValues);
            }

            return isExportedFarmFound;
        }

        /// <summary>
        /// Create a list of input files based on the components that make up the farm.
        /// </summary>
        public List<string> CreateInputFilesForFarm(string pathToFarmsDirectory, Farm farm, CLIArguments argValues)
        {
            // A list of all the created input files
            var createdFiles = new List<string>();

            // Create a directory for the farm
            var farmDirectoryPath = this.CreateDirectoryStructureForImportedFarm(pathToFarmsDirectory, farm);

            // Create a settings file for this farm
            bool isSettingsFileFound = false;
            if (argValues != null && argValues.Settings != "")
            {
                var files = Directory.GetFiles(pathToFarmsDirectory);
                string filePath = string.Empty;
                foreach (var file in files)
                {
                    if (argValues.Settings == Path.GetFileName(file))
                    {
                        filePath = file;
                        isSettingsFileFound = true;
                    }
                }
                if (isSettingsFileFound)
                {
                    string newFilePath = Path.Combine(farmDirectoryPath, Path.GetFileName(filePath));
                    File.Move(filePath, newFilePath);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Properties.Resources.SettingsFileNotFound, argValues.Settings);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine(Properties.Resources.LabelCreatingSettingsFile);
                    this.CreateSettingsFileForFarm(farmDirectoryPath, farm);
                }
            }
            // Move the settings file from pathToFarmsDirectory to the farmDirectoryPath
            else
            {
                Console.WriteLine();
                Console.WriteLine(Properties.Resources.LabelCreatingSettingsFile);
                this.CreateSettingsFileForFarm(farmDirectoryPath, farm);
            }

            /*
             * Field components
             */

            if (farm.FieldSystemComponents.Any())
            {
                Console.WriteLine(Properties.Resources.LabelCreatingFieldInputFiles);

                var pathToFieldComponents = farmDirectoryPath + @"\" + Properties.Resources.DefaultFieldsInputFolder;
                var fieldKeys = new FieldKeys();
                foreach (var fieldSystemComponent in farm.FieldSystemComponents)
                {
                    // The input file that gets built based on the field component
                    string inputFile = string.Empty;

                    if (farm.EnableCarbonModelling == false)
                    {
                        // Single year mode field - create input file based on single year view item
                        inputFile = _fieldProcessor.SetTemplateCSVFileBasedOnExportedField(farm: farm,
                            path: pathToFieldComponents,
                            componentKeys: fieldKeys.Keys,
                            fieldSystemComponent: fieldSystemComponent);
                    }
                    else
                    {
                        var detailViewItemsForField = new List<CropViewItem>();

                        // Multi-year mode field, create input based on multiple detail view items
                        var stageState = farm.StageStates.OfType<FieldSystemDetailsStageState>().SingleOrDefault();
                        if (stageState == null)
                        {
                            _fieldResultsService.InitializeStageState(farm);
                            stageState = _fieldResultsService.GetStageState(farm);
                        }

                        detailViewItemsForField = stageState.DetailsScreenViewCropViewItems.Where(x => x.FieldSystemComponentGuid == fieldSystemComponent.Guid).ToList();

                        inputFile = _fieldProcessor.SetTemplateCSVFileBasedOnExportedField(farm: farm,
                            path: pathToFieldComponents,
                            componentKeys: fieldKeys.Keys,
                            fieldSystemComponent: fieldSystemComponent,
                            detailsScreenViewCropViewItems: detailViewItemsForField);
                    }

                    createdFiles.Add(inputFile);

                    Console.WriteLine($@"{farm.Name}: {fieldSystemComponent.Name}");
                }
            }

            /*
             * Beef
             */

            if (farm.BeefCattleComponents.Any())
            {
                Console.WriteLine(Properties.Resources.LabelCreatingBeefCattleInputFiles);

                var pathToBeefCattleComponents = farmDirectoryPath + @"\" + Properties.Resources.DefaultBeefInputFolder;
                foreach (var beefCattleComponent in farm.BeefCattleComponents)
                {
                    var createdInputFile = _beefConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToBeefCattleComponents,
                        component: beefCattleComponent,
                        writeToPath: true);

                    createdFiles.Add(createdInputFile);

                    Console.WriteLine($@"{farm.Name}: {beefCattleComponent.Name}");
                }
            }

            /*
             * Dairy
             */

            if (farm.DairyComponents.Any())
            {
                Console.WriteLine(Properties.Resources.LabelCreatingDairyCattleInputFiles);

                var pathToDairyCattleComponents = farmDirectoryPath + @"\" + Properties.Resources.DefaultDairyInputFolder;
                foreach (var dairyComponent in farm.DairyComponents)
                {
                    var createdInputFile = _dairyConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToDairyCattleComponents,
                        component: dairyComponent,
                        writeToPath: true);

                    createdFiles.Add(createdInputFile);

                    Console.WriteLine($@"{farm.Name}: {dairyComponent.Name}");
                }
            }

            /*
             * Swine
             */

            if (farm.SwineComponents.Any())
            {
                Console.WriteLine(Properties.Resources.LabelCreatingSwineInputFiles);

                var pathToSwineComponents = farmDirectoryPath + @"\" + Properties.Resources.DefaultSwineInputFolder;
                foreach (var swineComponent in farm.SwineComponents)
                {
                    var createdInputFile = _swineConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToSwineComponents,
                        component: swineComponent,
                        writeToPath: true);

                    createdFiles.Add(createdInputFile);

                    Console.WriteLine($@"{farm.Name}: {swineComponent.Name}");
                }
            }

            /*
             * Sheep
             */

            if (farm.SheepComponents.Any())
            {
                Console.WriteLine(Properties.Resources.LabelCreatingSheepInputFiles);

                var pathToSheepComponents = farmDirectoryPath + @"\" + Properties.Resources.DefaultSheepInputFolder;
                foreach (var sheepComponent in farm.SheepComponents)
                {
                    var createdInputFile = _sheepConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToSheepComponents,
                        component: sheepComponent,
                        writeToPath: true);

                    createdFiles.Add(createdInputFile);

                    Console.WriteLine($@"{farm.Name}: {sheepComponent.Name}");
                }
            }

            /*
             * Poultry
             */

            if (farm.PoultryComponents.Any())
            {
                Console.WriteLine(Properties.Resources.LabelCreatingPoultryInputFiles);

                var pathToPoultryComponents = farmDirectoryPath + @"\" + Properties.Resources.DefaultPoultryInputFolder;
                foreach (var poultryComponent in farm.PoultryComponents)
                {
                    var createdInputFile = _poultryConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToPoultryComponents,
                        component: poultryComponent,
                        writeToPath: true);

                    createdFiles.Add(createdInputFile);

                    Console.WriteLine($@"{farm.Name}: {poultryComponent.Name}");
                }
            }

            /*
             * Other animals
             */

            if (farm.OtherLivestockComponents.Any())
            {
                Console.WriteLine(Properties.Resources.LabelCreatingOtherAnimalsInputFiles);

                var pathToOtherAnimalComponents = farmDirectoryPath + @"\" + Properties.Resources.DefaultOtherLivestockInputFolder;
                foreach (var otherLivestockComponent in farm.OtherLivestockComponents)
                {
                    var createdInputFile = _otherLiveStockConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToOtherAnimalComponents,
                        component: otherLivestockComponent,
                        writeToPath: true);

                    createdFiles.Add(createdInputFile);

                    Console.WriteLine($@"{farm.Name}: {otherLivestockComponent.Name}");
                }
            }

            return createdFiles;
        }

        public string PromptUserForLocationOfExportedFarms(string farmsFolderPath)
        {
            // Ask the user if they have farms that they would like to import from the GUI (they must have already exported the farms to a .json file)
            Console.WriteLine();
            Console.Write(Properties.Resources.LabelWouldYouLikeToImportFarmsFromTheGui);
            Console.WriteLine(Properties.Resources.LabelYesNo);

            var response = Console.ReadLine();
            if (_inputHelper.IsYesResponse(response))
            {
                // Prompt the user for the location of their exported farms
                Console.WriteLine();
                Console.Write(Properties.Resources.LabelAreYourExportedFarmsInCurrentFarmDirectory);
                Console.WriteLine(Properties.Resources.LabelYesNo);
                var response2 = Console.ReadLine();
                if (_inputHelper.IsYesResponse(response2))
                {
                    return farmsFolderPath;
                }
                Console.WriteLine();
                Console.WriteLine(Properties.Resources.LabelWhatIsThePathToYourExportedFarms);
                var pathToExportedFarms = Console.ReadLine();

                return pathToExportedFarms;
            }
            else
            {
                return string.Empty;
            }
        }

        public Farm GetExportedFarmFromUserSpecifiedLocation(string path)
        {
            return new Farm();
        }

        public List<Farm> GetExportedFarmsFromUserSpecifiedLocation(string path)
        {
            Console.WriteLine();
            Console.WriteLine(Properties.Resources.LabelGettingExportedFarms);
            var farms = _storage.GetExportedFarmsFromDirectoryRecursively(path);

            Console.WriteLine(string.Format(Properties.Resources.InterpolatedTotalNumberOfFarmsFound, farms.Count()));

            return farms.ToList();
        }

        #endregion

        #region Private Methods

        private void CreateSettingsFileForFarm(string farmDirectoryPath, Farm farm)
        {
            // Create a settings file based on the default object found in the imported farm
            _directoryHandler.GenerateGlobalSettingsFile(farmDirectoryPath, farm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToFarmsDirectory"></param>
        /// <param name="farm"></param>
        /// <returns>The full path to the directory created for the farm</returns>
        private string CreateDirectoryStructureForImportedFarm(string pathToFarmsDirectory, Farm farm)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

            // Remove illegal characters from farm name since it will be used to create a folder.
            var cleanedFarmName = r.Replace(farm.Name, "");

            var farmDirectoryPath = pathToFarmsDirectory + @"\" + cleanedFarmName;
            if (Directory.Exists(farmDirectoryPath))
            {
                // Directory already exists, append timestamp to folder to differentiate between existing folder and new folder
                var timestamp = $"__{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}";

                farmDirectoryPath += timestamp;
            }

            Directory.CreateDirectory(farmDirectoryPath);
            _directoryHandler.ValidateComponentDirectories(farmDirectoryPath);

            return farmDirectoryPath;
        }

        #endregion
    }
}
