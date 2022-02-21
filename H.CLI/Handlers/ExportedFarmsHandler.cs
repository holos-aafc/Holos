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
        public List<Farm> Initialize(string farmsFolderPath)
        {
            var pathToExportedFarms = this.PromptUserForLocationOfExportedFarms();
            if (string.IsNullOrWhiteSpace(pathToExportedFarms))
            {
                // User specified no farms to import
                return new List<Farm>();
            }

            var farms = this.GetExportedFarmsFromUserSpeciedLocation(pathToExportedFarms);

            var inputFilesForAllFarms = new List<string>();
            foreach (var farm in farms)
            {
                // Create input files for all components in farm
                var componentFilesForFarm = this.CreateInputFilesForFarm(farmsFolderPath, farm);

                inputFilesForAllFarms.AddRange(componentFilesForFarm);
            }

            Console.WriteLine();
            Console.WriteLine(string.Format(Properties.Resources.InterpolatedTotalFarmsSuccessfullyImported, farms.Count));
            Console.WriteLine(string.Format(Properties.Resources.LabelInputFilesHaveBeenCreatedAndStoredInYourFarmsDirectory, inputFilesForAllFarms.Count));
            Console.WriteLine();

            return farms;
        }

        /// <summary>
        /// Create a list of input files based on the components that make up the farm.
        /// </summary>
        public List<string> CreateInputFilesForFarm(string pathToFarmsDirectory, Farm farm)
        {
            // A list of all the created input files
            var createdFiles = new List<string>();

            // Create a directory for the farm
            var farmDirectoryPath = this.CreateDirectoryStructureForImportedFarm(pathToFarmsDirectory, farm);

            // Create a settings file for this farm
            this.CreateSettingsFileForFarm(farmDirectoryPath, farm);

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
                            componentKeys: fieldKeys.keys,
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
                            componentKeys: fieldKeys.keys, 
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
                var beefCattleKeys = new BeefCattleKeys();
                var componentNumber = 1;
                foreach (var beefCattleComponent in farm.BeefCattleComponents)
                {                    
                    var createdInputFile = _beefConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToBeefCattleComponents,
                        componentKeys: beefCattleKeys.keys,
                        component: beefCattleComponent,
                        componentNumber: componentNumber,
                        writeToPath: true);

                    createdFiles.Add(createdInputFile);

                    componentNumber++;

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
                var dairyCattleKeys = new DairyCattleKeys();
                foreach (var dairyComponent in farm.DairyComponents)
                {
                    var createdInputFile = _dairyConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToDairyCattleComponents,
                        componentKeys: dairyCattleKeys.keys,
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
                var swineKeys = new SwineKeys();
                foreach (var swineComponent in farm.SwineComponents)
                {
                    var createdInputFile = _swineConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToSwineComponents,
                        componentKeys: swineKeys.keys,
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
                var sheepKeys = new SheepKeys();
                foreach (var sheepComponent in farm.SheepComponents)
                {
                    var createdInputFile = _sheepConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToSheepComponents,
                        componentKeys: sheepKeys.keys,
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
                var poultryKeys = new PoultryKeys();
                foreach (var poultryComponent in farm.PoultryComponents)
                {
                    var createdInputFile = _poultryConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToPoultryComponents,
                        componentKeys: poultryKeys.keys,
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
                var otherLiveStockKeys = new OtherLiveStockKeys();
                foreach (var otherLivestockComponent in farm.OtherLivestockComponents)
                {
                    var createdInputFile = _otherLiveStockConverter.SetTemplateCSVFileBasedOnExportedFarm(
                        path: pathToOtherAnimalComponents,
                        componentKeys: otherLiveStockKeys.keys,
                        component: otherLivestockComponent,
                        writeToPath: true);

                    createdFiles.Add(createdInputFile);

                    Console.WriteLine($@"{farm.Name}: {otherLivestockComponent.Name}");
                }
            }

            return createdFiles;
        }

        public string PromptUserForLocationOfExportedFarms()
        {
            // Ask the user if they have farms that they would like to import from the GUI (they must have already exported the farms to a .json file)            
            Console.Write(Properties.Resources.LabelWouldYouLikeToImportFarmsFromTheGui);            
            Console.WriteLine(Properties.Resources.LabelYesNo);

            var response = Console.ReadLine();
            if (_inputHelper.IsYesResponse(response))
            {
                // Prompt the user for the location of their exported farms
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

        public List<Farm> GetExportedFarmsFromUserSpeciedLocation(string path)
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
