using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using H.CLI.UserInput;
using H.Core.Calculators.Infrastructure;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals.Sheep;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Rotation;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using Prism.Events;
using Prism.Interactivity.DefaultPopupWindows;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;

namespace H.Core.Test.Integration
{
    [TestClass]
    public class FieldComponentIntegrationTest : UnitTestBase
    {
        private GeographicDataProvider _geographicDataProvider;
        private GlobalSettings _globalSettings;
        private RotationComponentHelper _rotationComponentHelper;
        private ApplicationData _applicationData;
        private NasaClimateProvider _nasaClimateProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize the soil information provider
            _geographicDataProvider = new GeographicDataProvider();
            _geographicDataProvider.Initialize();

            // Initial system defaults
            _globalSettings = new GlobalSettings();

            // This class will create a rotation on our farm
            _rotationComponentHelper = new RotationComponentHelper();

            _applicationData = new ApplicationData();

            _nasaClimateProvider = new NasaClimateProvider();

            _fieldResultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipcc, _n2OEmissionFactorCalculator);
        }

        [TestMethod]
        public void CarbonModellingIntegrationTest()
        {
            /*
             * Note: The Visual Studio output window will display logging information from the various method calls and the associated successes/failures of those calls
             */

            // Create a new farm which is a container object for all components that can be added to a farm
            var farm = new Farm();

            farm.Name = "My Test Farm";

            // Choose the carbon model
            farm.Defaults = new Defaults()
            {
                CarbonModellingStrategy = CarbonModellingStrategies.IPCCTier2,
                //CarbonModellingStrategy = CarbonModellingStrategies.ICBM,
            };

            // Add location information for the farm
            farm.Province = Province.Alberta;

            // All farms need to have an SLC polygon number assigned (latitude & longitude coordinates not currently supported)
            farm.PolygonId = 793011; // Lethbridge, AB

            // Get soil data that is associated with this location
            farm.GeographicData = _geographicDataProvider.GetGeographicalData(farm.PolygonId);

            // Now can set or read various soil details for this location by reading the DefaultSoilData property. For example we can get or set the soil Ph
            var soilPh = farm.GeographicData.DefaultSoilData.SoilPh;

            // We will now get climate data from the NASA API. Note that if the latitude and longitude are known, we can get precise climate data for our location. If precise location
            // is not know, we call use slightly less accurate 30 year climate normals.

            farm.ClimateData = _climateProvider.Get(49.699498, -112.775813, TimeFrame.NineteenNinetyToTwoThousand); // Lethbridge research station coordinates

            // We now have daily climate data going back to 1981 for this location and can access precipitation, evapotranspiration, temperature, etc.
            var climateDataFromLastMonth = farm.ClimateData.DailyClimateData.Single(data => data.Date.Date.Equals(DateTime.Now.Date.Subtract(TimeSpan.FromDays(30))));
            var precipitation30DaysAgo = climateDataFromLastMonth.MeanDailyPrecipitation;

            var crop1 = new CropViewItem() { CropType = CropType.Wheat };
            var crop2 = new CropViewItem() { CropType = CropType.Oats };
            var crop3 = new CropViewItem() { CropType = CropType.Fallow };

            // We'll create three fields with 3 different crops rotated over the three fields
            var cropsInRotation = new List<CropViewItem>()
            {
                crop1, crop2, crop3,
            };

            // Assign default properties to
            foreach (var cropViewItem in cropsInRotation)
            {
                _fieldResultsService.AssignSystemDefaults(cropViewItem, farm, _globalSettings);
            }

            // Override default properties for some crops (optional)
            crop2.NitrogenFertilizerRate = 60;

            // Create a new rotation component.
            var rotationComponent = new RotationComponent();
            rotationComponent.FieldSystemComponent = new FieldSystemComponent();
            rotationComponent.FieldSystemComponent.BeginOrderingAtStartYearOfRotation = true;
            rotationComponent.FieldSystemComponent.StartYear = 1980;
            rotationComponent.FieldSystemComponent.EndYear = 2022;
            rotationComponent.ShiftLeft = true;

            // Build the rotation and add it to the farm
            _rotationComponentHelper.CreateRotation(cropsInRotation, rotationComponent, _globalSettings, farm);

            // Set the yield assignment method
            farm.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;

            // Build the field history
            farm.StageStates.Add(new FieldSystemDetailsStageState());
            _fieldResultsService.CreateDetailViewItems(farm);

            var finalResults = _fieldResultsService.CalculateFinalResults(farm);

            // Create a directory to hold the outputs of the run
            const string OutputDirectory = "HOLOS_OUTPUT\\";
            Directory.CreateDirectory(OutputDirectory);

            _fieldResultsService.ExportResultsToFile(finalResults, OutputDirectory, CultureInfo.CurrentCulture, MeasurementSystemType.Metric, CLILanguageConstants.OutputLanguageAddOn, false, farm);
        }

        [TestMethod]
        public void FCC()
        {
            var farm = new Farm();



            // import data using Field API Client



            farm.Name = "test";
            farm.Province = Province.Saskatchewan;
            farm.PolygonId = 755002;
            farm.GeographicData = _geographicDataProvider.GetGeographicalData(farm.PolygonId);
            farm.Longitude = -103.867;
            farm.Latitude = 50.254;

            _nasaClimateProvider.EndDate = new DateTime(2022, 8, 18);
            var nasaClimateForDate = _nasaClimateProvider.GetCustomClimateData(50.254, -103.867);
            farm.ClimateData = _climateProvider.Get(nasaClimateForDate, TimeFrame.TwoThousandToCurrent);
            farm.ClimateAcquisition = Farm.ChosenClimateAcquisition.Custom;
            
            
            farm.Defaults.DefaultRunInPeriod = 20;

            var fieldComponent = new FieldSystemComponent()
            {
                FieldName = "Tony - East of Lane",
                FieldArea = 54.78,
                BeginOrderingAtStartYearOfRotation = false,
                StartYear = 1985,
                EndYear = 2022,
            };



            var cvi1 = new CropViewItem();
            cvi1.CropType = CropType.Barley;
            _fieldResultsService.AssignSystemDefaults(cvi1, farm, _applicationData.GlobalSettings);  // moisture content will be set here



            cvi1.Area = 54.78;
            cvi1.FieldName = "Tony - East of Lane";
            cvi1.Year = 2019;
            cvi1.TillageType = TillageType.NoTill;
            cvi1.HarvestMethod = HarvestMethods.CashCrop;
            cvi1.NumberOfPesticidePasses = 3;
            cvi1.Yield = 5380;                           // get from agexpert
            cvi1.MoistureContentOfCropPercentage = 12;
            // Need cover crop, if applicable



            // Need manure application, if applicable
            // This record doesn't have any manure, but commented code below shows how to add some
            //ManureApplicationViewItems = new System.Collections.ObjectModel.ObservableCollection<ManureApplicationViewItem>
            //{
            //    new ManureApplicationViewItem()
            //    {
            //        Amount = 0,
            //        AnimalType = AnimalType.Beef,
            //        DefaultManureCompositionData = _compositionProvider.GetManureCompositionDataByType(AnimalType.Beef, ManureStateType.LiquidSeparated)
            //    }
            //}
            cvi1.FertilizerApplicationViewItems = new System.Collections.ObjectModel.ObservableCollection<FertilizerApplicationViewItem>()
           {
               new FertilizerApplicationViewItem()
               {
                   SeasonOfApplication = Seasons.Spring,
                   FertilizerBlendData = new H.Core.Providers.Fertilizer.Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data()
                   {
                       FertilizerBlend = FertilizerBlends.Custom,
                       PercentagePhosphorus = 0,
                       PercentageNitrogen = 0,
                       PercentagePotassium = 0,
                       PercentageSulphur = 0,
                   },
                   AmountOfNitrogenApplied = 0,
                   AmountOfPhosphorusApplied = 0,
                   AmountOfPotassiumApplied = 0,
                   AmountOfSulphurApplied = 0,
                   FertilizerEfficiencyPercentage = 75,
                   FertilizerApplicationMethodology = FertilizerApplicationMethodologies.IncorporatedOrPartiallyInjected,
                   AmountOfBlendedProductApplied = 0
               }
           };
            // Not needed for carbon model
            cvi1.NitrogenDepositionAmount = 5;
            cvi1.NitrogenFixation = 0;



            cvi1.CalculateDryYield();                                                                // Will calculate dry weigh from wet weight



            var cvi2 = new CropViewItem();



            cvi2.CropType = CropType.Canola;
            _fieldResultsService.AssignSystemDefaults(cvi2, farm, _applicationData.GlobalSettings);



            cvi2.Area = 54.78;
            cvi2.FieldName = "Tony - East of Lane";
            cvi2.Year = 2020;
            cvi2.TillageType = TillageType.NoTill;
            cvi2.HarvestMethod = HarvestMethods.CashCrop;
            cvi2.Yield = 3295.25;
            cvi2.MoistureContentOfCropPercentage = 9;
            cvi2.NumberOfPesticidePasses = 3;
            cvi2.NitrogenDepositionAmount = 5;
            cvi2.NitrogenFixation = 0;



            cvi2.FertilizerApplicationViewItems = new System.Collections.ObjectModel.ObservableCollection<FertilizerApplicationViewItem>()
               {
                   new FertilizerApplicationViewItem()
                   {
                       SeasonOfApplication = Seasons.Spring,
                       FertilizerBlendData = new H.Core.Providers.Fertilizer.Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data()
                       {
                           FertilizerBlend = FertilizerBlends.Urea,
                           PercentagePhosphorus = 0,
                           PercentageNitrogen = 0,
                           PercentagePotassium = 0,
                           PercentageSulphur = 0,
                       },
                       AmountOfNitrogenApplied = 136.6,
                       AmountOfPhosphorusApplied = 0,
                       AmountOfPotassiumApplied = 0,
                       AmountOfSulphurApplied = 0,
                       FertilizerEfficiencyPercentage = 75,
                       FertilizerApplicationMethodology = FertilizerApplicationMethodologies.IncorporatedOrPartiallyInjected,
                       AmountOfBlendedProductApplied = 297
                   }
               };




            cvi2.CalculateDryYield();



            var cvi3 = new CropViewItem();



            cvi3.CropType = CropType.FieldPeas;
            _fieldResultsService.AssignSystemDefaults(cvi3, farm, _applicationData.GlobalSettings);



            cvi3.Area = 54.78;
            cvi3.FieldName = "Tony - East of Lane";
            cvi3.Year = 2021;
            cvi3.TillageType = TillageType.NoTill;
            cvi3.HarvestMethod = HarvestMethods.CashCrop;
            cvi3.Yield = 2353.75;
            cvi3.MoistureContentOfCropPercentage = 13;
            cvi3.NumberOfPesticidePasses = 5;
            cvi3.NitrogenDepositionAmount = 5;
            cvi3.NitrogenFixation = 0.7;
            cvi3.FertilizerApplicationViewItems = new System.Collections.ObjectModel.ObservableCollection<FertilizerApplicationViewItem>()
           {
               new FertilizerApplicationViewItem()
               {
                   SeasonOfApplication = Seasons.Spring,
                   FertilizerBlendData = new H.Core.Providers.Fertilizer.Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data()
                   {
                       FertilizerBlend = FertilizerBlends.Custom,
                       PercentagePhosphorus = 0,
                       PercentageNitrogen = 0,
                       PercentagePotassium = 0,
                       PercentageSulphur = 0,
                   },
                   AmountOfNitrogenApplied = 0,
                   AmountOfPhosphorusApplied = 0,
                   AmountOfPotassiumApplied = 0,
                   AmountOfSulphurApplied = 0,
                   FertilizerEfficiencyPercentage = 75,
                   FertilizerApplicationMethodology = FertilizerApplicationMethodologies.IncorporatedOrPartiallyInjected,
                   AmountOfBlendedProductApplied = 61.7
               }
           };



            cvi3.CalculateDryYield();



            var cvi4 = new CropViewItem();



            cvi4.CropType = CropType.Wheat;
            _fieldResultsService.AssignSystemDefaults(cvi4, farm, _applicationData.GlobalSettings);



            cvi4.Area = 54.78;
            cvi4.FieldName = "Tony - East of Lane";
            cvi4.Year = 2022;
            cvi4.TillageType = TillageType.NoTill;
            cvi4.HarvestMethod = HarvestMethods.CashCrop;
            cvi4.Yield = 4035;
            cvi4.MoistureContentOfCropPercentage = 12;
            cvi4.NumberOfPesticidePasses = 6;
            cvi4.NitrogenDepositionAmount = 5;
            cvi4.NitrogenFixation = 0;
            cvi4.FertilizerApplicationViewItems = new System.Collections.ObjectModel.ObservableCollection<FertilizerApplicationViewItem>()
           {
               new FertilizerApplicationViewItem()
               {
                   SeasonOfApplication = Seasons.Spring,



                   FertilizerBlendData = new H.Core.Providers.Fertilizer.Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data()
                   {
                       FertilizerBlend = FertilizerBlends.Urea,
                       PercentagePhosphorus = 0,
                       PercentageNitrogen = 0,
                       PercentagePotassium = 0,
                       PercentageSulphur = 0,
                   },
                   AmountOfNitrogenApplied = 0,
                   AmountOfPhosphorusApplied = 0,
                   AmountOfPotassiumApplied = 0,
                   AmountOfSulphurApplied = 0,
                   FertilizerEfficiencyPercentage = 75,
                   FertilizerApplicationMethodology = FertilizerApplicationMethodologies.IncorporatedOrPartiallyInjected,
                   AmountOfBlendedProductApplied = 280.3
               }
           };



            cvi4.CalculateDryYield();



            fieldComponent.CropViewItems.Add(cvi1);
            fieldComponent.CropViewItems.Add(cvi2);
            fieldComponent.CropViewItems.Add(cvi3);
            fieldComponent.CropViewItems.Add(cvi4);



            farm.Components.Add(fieldComponent);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;



            farm.StageStates.Add(new FieldSystemDetailsStageState());
            _fieldResultsService.CreateDetailViewItems(farm);



            var results = _fieldResultsService.CalculateFinalResults(farm);
            var soilC1985FieldPeas = results[0].SoilCarbon; // 36258

        }
    }
}
