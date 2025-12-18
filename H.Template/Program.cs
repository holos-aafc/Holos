using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Results;
using H.Core;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Climate;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Animals.Table_69;
using H.Core.Providers.Climate;
using H.Core.Providers.Feed;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.Initialization;
using H.Core.Services.Initialization.Animals;
using H.Core.Services.Initialization.Climate;
using H.Core.Services.Initialization.Crops;
using H.Core.Services.Initialization.Geography;
using H.Core.Services.LandManagement;
using H.Infrastructure;
using Prism.Events;

namespace H.TemplateT
{
    internal class Program
    {
        #region Fields

        private static readonly Storage _storage;
        private static readonly ApplicationData _applicationData;

        private static readonly IClimateInitializationService _climateInitializationService;
        private static readonly IGeographyInitializationService _geographyInitializationService;
        private static readonly ICropInitializationService _cropInitializationService;
        private static readonly IAnimalService _animalService;
        private static readonly ICBMSoilCarbonCalculator _icbmsoilCarbonCalculator;
        private static readonly IPCCTier2SoilCarbonCalculator _ipccTier2SoilCarbonCalculator;
        private static readonly N2OEmissionFactorCalculator _n2OEmissionFactorCalculator;
        private static readonly IInitializationService _initializationService;
        private static readonly FieldResultsService _fieldResultsService;
        private static readonly IClimateProvider _climateProvider;
        private static readonly ISlcClimateProvider _sliClimateProvider;
        private static readonly IDietProvider _dietProvider;
        private static readonly ComponentResultsProcessor _componentResultsProcessor;
        private static readonly ITimePeriodHelper _timePeriodHelper;
        private static readonly IFarmResultsService _farmResultsService;
        private static readonly IEventAggregator _eventAggregator;
        private static readonly IADCalculator _adCalculator;
        private static readonly IManureService _manureService;
        private static readonly IAnimalInitializationService _animalInitializationService;

        #endregion

        static Program()
        {
            _storage = new Storage();
            _applicationData = new ApplicationData();
            _storage.ApplicationData = _applicationData;

            _adCalculator = new ADCalculator();
            _manureService = new ManureService();
            _timePeriodHelper = new TimePeriodHelper();
            _eventAggregator = new EventAggregator();
            _climateInitializationService = new ClimateInitializationService();
            _geographyInitializationService = new GeographyInitializationService();
            _cropInitializationService = new CropInitializationService();
            _sliClimateProvider = new SlcClimateDataProvider();
            _initializationService = new InitializationService();
            _dietProvider = new DietProvider();
            _climateProvider = new ClimateProvider(_sliClimateProvider);
            _n2OEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            _icbmsoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            _ipccTier2SoilCarbonCalculator = new IPCCTier2SoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            _fieldResultsService = new FieldResultsService(_icbmsoilCarbonCalculator, _ipccTier2SoilCarbonCalculator, _n2OEmissionFactorCalculator, _initializationService);
            _animalService = new AnimalResultsService();
            _componentResultsProcessor = new ComponentResultsProcessor(_storage, _timePeriodHelper, _fieldResultsService, _n2OEmissionFactorCalculator);
            _farmResultsService = new FarmResultsService(_eventAggregator, _fieldResultsService, _adCalculator, _manureService, _animalService, _n2OEmissionFactorCalculator);
            _animalInitializationService = new AnimalInitializationService();
        }

        static void Main(string[] args)
        {
            CroppingExample();
            //AnimalExample();
        }

        static void AnimalExample()
        {
            // An object to hold settings that can be referenced from all farms created by the user
            var globalSettings = new GlobalSettings();

            // Create a farm
            var farm = new Farm();
            farm.Name = "Template_Test_Farm_Animals";

            // Add this farm to the system
            _applicationData.Farms.Clear();
            _applicationData.Farms.Add(farm);

            /*
             * All farms need to have their location set.
             */

            // Place the farm within an SLC polygon (St-Julien, QC)
            farm.PolygonId = 482019;

            // Specify coordinates
            farm.Latitude = 46.005;
            farm.Longitude = -71.520;

            /*
             * Set climate data according to location
             */

            _climateInitializationService.InitializeClimate(farm);

            /*
             * Set geographic data (soil properties, etc.) according to location
             */

            _geographyInitializationService.InitializeGeography(farm);

            /*
             * Choose components to add to the farm
             */

            var beefCattleFinishingComponent = new FinishingComponent();
            beefCattleFinishingComponent.Name = "My Quebec beef cattle operation";
            farm.Components.Add(beefCattleFinishingComponent);

            // All animal components must have at least one group of animals
            var groupOfHeifers = new AnimalGroup();
            groupOfHeifers.Name = "Group of 10 heifers";
            groupOfHeifers.GroupType = AnimalType.BeefFinishingHeifer;

            // Associate this group with the animal component
            beefCattleFinishingComponent.Groups.Add(groupOfHeifers);

            // All animal groups must have at least one management period. To estimate emissions for a period of time, the start weight, end weight and duration of the period should be defined.
            var managementPeriod = new ManagementPeriod();
            managementPeriod.Name = "Summer management";
            managementPeriod.AnimalType = AnimalType.BeefFinishingHeifer;
            managementPeriod.NumberOfAnimals = 10;
            managementPeriod.StartWeight = 100;
            managementPeriod.EndWeight = 200;
            managementPeriod.Start = new DateTime(2024, 1, 1);
            managementPeriod.Duration = TimeSpan.FromDays(30);

            // Choose a diet for the animals - all management periods must have diet defined
            var availableDiets = _dietProvider.GetDiets().Where(x => x.AnimalType == AnimalType.BeefFinisher);
            var diet = availableDiets.First();
            managementPeriod.SelectedDiet = diet;

            // Associate this management period with the animal group
            groupOfHeifers.ManagementPeriods.Add(managementPeriod);

            // Initialize defaults
            _animalInitializationService.InitializeFarm(farm);

            // We are done configuring our farm and are ready to calculate results
            var results = _farmResultsService.CalculateFarmEmissionResults(farm);

            // Use results...
            foreach (var animalComponentEmissionsResults in results.AnimalComponentEmissionsResults)
            {
                foreach (var groupEmissions in animalComponentEmissionsResults.EmissionResultsForAllAnimalGroupsInComponent)
                {
                    foreach (var groupEmissionsGroupEmissionsByMonth in groupEmissions.GroupEmissionsByMonths)
                    {
                        Console.WriteLine($"Enteric methane emissions for 10 heifers in {groupEmissionsGroupEmissionsByMonth.MonthString} is {groupEmissionsGroupEmissionsByMonth.MonthlyEntericMethaneEmission} kg CH4");
                    }
                }
            }
        }

        static void CroppingExample()
        {
            // An object to hold settings that can be referenced from all farms created by the user
            var globalSettings = new GlobalSettings();

            // Create a farm
            var farm = new Farm();
            farm.Name = "Template_Test_Farm_Cropping";

            // Add this farm to the system
            _applicationData.Farms.Clear();
            _applicationData.Farms.Add(farm);

            /*
             * All farms need to have their location set.
             */

            // Place the farm within an SLC polygon (Lethbridge, AB)
            farm.PolygonId = 793011;

            // Specify coordinates
            farm.Latitude = 49.682;
            farm.Longitude = -112.682;

            /*
             * Set climate data according to location
             */

            _climateInitializationService.InitializeClimate(farm);

            /*
             * Set geographic data (soil properties, etc.) according to location
             */

            _geographyInitializationService.InitializeGeography(farm);

            /*
             * Choose components to add to the farm
             */

            // Add one field component. Choose a start year and an end year with at least a couple of decades in between
            var fieldComponent = new FieldSystemComponent
            {
                Name = "My wheat and barley field",
                StartYear = 1985,
                EndYear = 2020
            };

            // Many crops exist in the system (Enumeration file) but only a subset are currently supported for carbon modelling. Choose from the list of supported crop types
            var validCropTypes = CropTypeExtensions.GetValidCropTypes().ToList();
            var barley = validCropTypes.Single(x => x == CropType.Barley);
            var wheat = validCropTypes.Single(x => x == CropType.Wheat);

            // Grow wheat in one year
            var wheatYear = new CropViewItem
            {
                CropType = wheat,
                Year = 2020,
            };

            // Grow barley in the previous year
            var barleyYear = new CropViewItem()
            {
                CropType = barley,
                Year = 2019,
            };

            /*
             * We have now specified the rotation that will be used for this field. Starting in 2020 we grew wheat, then in 2019 we grew barley. Holos will
             * use this as the crop rotation sequence going back to our start year for this field (1985). It is not necessary to manually code this sequence going all the way
             * back to the start year - Holos will do this on behalf of the user as long as the crop sequence is minimally described (i.e. Wheat-Barley)
             *
             * Holos will then back-populate the field history
             *
             * 2020 wheat
             * 2019 barley
             * 2018 wheat
             * 2017 barley
             * ...
             */

            // Associate the cropping data with the field
            wheatYear.FieldSystemComponentGuid = fieldComponent.Guid;
            barleyYear.FieldSystemComponentGuid = fieldComponent.Guid;

            fieldComponent.CropViewItems.Add(wheatYear);
            fieldComponent.CropViewItems.Add(barleyYear);

            // Set defaults for each year (yield, irrigation, pesticide passes, etc.
            _cropInitializationService.InitializeCrop(wheatYear, farm, globalSettings);
            _cropInitializationService.InitializeCrop(barleyYear, farm, globalSettings);

            farm.Components.Add(fieldComponent);

            // Create the field history
            _fieldResultsService.InitializeStageState(farm);

            // Holos has now back-populated our field with historical data. Finally adjustments can be made to each individual year if need
            foreach (var cropViewItem in farm.GetFieldSystemDetailsStageState().DetailsScreenViewCropViewItems)
            {
                if (cropViewItem.Year == 1999)
                {
                    // Very high yield in this year
                    cropViewItem.Yield = 10000;
                }
            }

            // Farm setup is complete - calculate final emission results
            var finalResults = _fieldResultsService.CalculateFinalResults(farm);

            // Print results to file
            const string outputDirectory = "Holos_Template_Run_Output";
            Directory.CreateDirectory(outputDirectory);
            _fieldResultsService.ExportResultsToFile(finalResults, outputDirectory + Path.DirectorySeparatorChar, InfrastructureConstants.EnglishCultureInfo, MeasurementSystemType.Metric, string.Empty, false, farm);
        }
    }
}
