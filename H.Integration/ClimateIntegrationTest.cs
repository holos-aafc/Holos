using H.CLI.Results;
using H.Core;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Climate;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Providers.Feed;
using H.Core.Providers.Soil;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.Initialization;
using H.Core.Services.Initialization.Animals;
using H.Core.Services.Initialization.Climate;
using H.Core.Services.Initialization.Crops;
using H.Core.Services.Initialization.Geography;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using H.Infrastructure;

namespace H.Integration
{
    [TestClass]
    public class ClimateIntegrationTest
    {
        #region Fields

        private Storage _storage;
        private ApplicationData _applicationData;

        private IClimateInitializationService _climateInitializationService;
        private IGeographyInitializationService _geographyInitializationService;
        private ICropInitializationService _cropInitializationService;
        private IAnimalService _animalService;
        private ICBMSoilCarbonCalculator _icbmsoilCarbonCalculator;
        private IPCCTier2SoilCarbonCalculator _ipccTier2SoilCarbonCalculator;
        private N2OEmissionFactorCalculator _n2OEmissionFactorCalculator;
        private IInitializationService _initializationService;
        private FieldResultsService _fieldResultsService;
        private IClimateProvider _climateProvider;
        private ISlcClimateProvider _sliClimateProvider;
        private IDietProvider _dietProvider;
        private ComponentResultsProcessor _componentResultsProcessor;
        private ITimePeriodHelper _timePeriodHelper;
        private IFarmResultsService _farmResultsService;
        private IEventAggregator _eventAggregator;
        private IADCalculator _adCalculator;
        private IManureService _manureService;
        private IAnimalInitializationService _animalInitializationService;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
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

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        /// <summary>
        /// Ensure the daily climate data attained from a NASA call is the same as that attained from a user-input climate file. In this scenario, the climate
        /// file should contain the exact same data as that returned from NASA for the given coordinates.
        /// </summary>
        [TestMethod]
        [Ignore] // Ignore since tests are failing after NASA API shutdown. After API was restored, tests are failing since they have probably recalculated some of their data points and now the file 
        // bing used for testing is out of date.
        public void TestDataFromClimateFileIsTheSameAsFromNasaApiCall()
        {
            // Get final Holos GHG calculations
            var nasaResults = GetResultsUsingNasaApi();
            var fileResults = GetResultsUsingClimateFile();

            // Get the daily climate data from each farm
            var nasaDaily = nasaResults.Item1.ClimateData.DailyClimateData;
            var fileDaily = fileResults.Item1.ClimateData.DailyClimateData;

            var count = fileResults.Item1.ClimateData.DailyClimateData.Count;

            // Compare each day's data
            for (int i = 0; i < count; i++)
            {
               var fileClimate = fileDaily.ElementAt(i);
               var nasaClimate = nasaDaily.ElementAt(i);


               Assert.AreEqual(fileClimate.MeanDailyPrecipitation, nasaClimate.MeanDailyPrecipitation, 0.00001);
               Assert.AreEqual(fileClimate.MeanDailyAirTemperature, nasaClimate.MeanDailyAirTemperature, 0.00001);
               Assert.AreEqual(fileClimate.MeanDailyPET, nasaClimate.MeanDailyPET, 0.00001);
            }

            _climateInitializationService.InitializeClimate(nasaResults.Item1, nasaResults.Item1.ClimateData.DailyClimateData.Take(count).ToList());
            _climateInitializationService.InitializeClimate(fileResults.Item1, fileResults.Item1.ClimateData.DailyClimateData.Take(count).ToList());

            // Compare monthly normal calculations as well
            foreach (Months value in Enum.GetValues(typeof(Months)))
            {
                var apiPrecip = nasaResults.Item1.ClimateData.PrecipitationData.GetValueByMonth(value);
                var filePrecip = fileResults.Item1.ClimateData.PrecipitationData.GetValueByMonth(value);

                var apiEvapo = nasaResults.Item1.ClimateData.EvapotranspirationData.GetValueByMonth(value);
                var fileEvapo = fileResults.Item1.ClimateData.EvapotranspirationData.GetValueByMonth(value);

                var apiTemp = nasaResults.Item1.ClimateData.TemperatureData.GetValueByMonth(value);
                var fileTemp = fileResults.Item1.ClimateData.TemperatureData.GetValueByMonth(value);

                Assert.AreEqual(filePrecip, apiPrecip, 0.00001);
                Assert.AreEqual(fileEvapo, apiEvapo, 0.00001);
                Assert.AreEqual(fileTemp, apiTemp, 0.00001);
            }
        }

        #region Private Methods

        private Tuple<Farm, List<CropViewItem>> GetResultsUsingClimateFile()
        {
            // An object to hold settings that can be referenced from all farms created by the user
            var globalSettings = new GlobalSettings();

            // Create a farm
            var farm = new Farm();
            farm.Name = "Template_Test_Farm_Cropping_FILE";

            /*
             * All farms need to have their location set.
             */

            // Place the farm within an SLC polygon (Lethbridge, AB)
            farm.PolygonId = 793011;

            farm.ClimateAcquisition = Farm.ChosenClimateAcquisition.InputFile;
            farm.ClimateDataFileName = "Climate 2 - Climate Data_FOR_INTEGRATION_TESTING.csv";

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

            return new Tuple<Farm, List<CropViewItem>>(farm, finalResults);
        }

        private Tuple<Farm, List<CropViewItem>> GetResultsUsingNasaApi()
        {
            // An object to hold settings that can be referenced from all farms created by the user
            var globalSettings = new GlobalSettings();

            // Create a farm
            var farm = new Farm();
            farm.Name = "Template_Test_Farm_Cropping_NASA";

            // Add this farm to the system
            _applicationData.Farms.Clear();
            _applicationData.Farms.Add(farm);

            /*
             * All farms need to have their location set.
             */

            // Place the farm within an SLC polygon (Lethbridge, AB)
            farm.PolygonId = 793011;

            // Specify coordinates
            farm.Latitude = 49.244954;
            farm.Longitude = -109.731445;

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

            return new Tuple<Farm, List<CropViewItem>>(farm, finalResults);
        }

        #endregion
    }
}
