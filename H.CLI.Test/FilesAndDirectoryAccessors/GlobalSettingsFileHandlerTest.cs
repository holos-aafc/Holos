using H.CLI.FileAndDirectoryAccessors;
using H.CLI.UserInput;
using H.Core.Enumerations;
using H.Core.Providers;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Models;

namespace H.CLI.Test.FilesAndDirectoryAccessors
{
    [TestClass]
    public class GlobalSettingsFileHandlerTest
    {
       public static readonly GeographicDataProvider geographicDataProvider = new GeographicDataProvider();
        [ClassInitialize]
        public static void ClassInitialzie(TestContext context)
        {
            geographicDataProvider.Initialize();
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            
        }

        [TestMethod]
        public void TestGenerateGlobalSettingsFile_ThereIsNoSettingsFileInFarms_ExpectSettingsFileToBeMade()
        {
            var directoryKeys = new DirectoryKeys();
            var directoryHandler = new DirectoryHandler();
            var globalSettingsReader = new ReadSettingsFile();

            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestFarms");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestFarms\Farm2");

            var pathToSettingsFile = @"H.CLI.TestFiles\TestFarms\Farm2\Farm.settings";
            var pathToFarmWhereSettingsFileShouldBe = @"H.CLI.TestFiles\TestFarms\Farm2";

            if (File.Exists(pathToSettingsFile))
            {
                File.Delete(pathToSettingsFile);
            }

            var lethbridgeGeographicData = geographicDataProvider.GetGeographicalData(793006);
            directoryHandler.GenerateGlobalSettingsFile(pathToFarmWhereSettingsFileShouldBe, new Farm() { GeographicData = lethbridgeGeographicData});
            globalSettingsReader.ReadGlobalSettings(pathToSettingsFile);

            Assert.AreEqual(globalSettingsReader.GlobalSettingsDictionary["Emergence Day"], "141");

        }

        [TestMethod]
        public void TestSetGlobalSettingsSetsDefaultsProperly()
        {
       
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Metric;
            var farm = new Farm();
            var applicationData = new ApplicationData();
            var lethbridgeGeographicData = geographicDataProvider.GetGeographicalData(793006);
            var climateDataProvider = new SlcClimateDataProvider();
            var climatateData = climateDataProvider.GetClimateData(793006, TimeFrame.ProjectionPeriod);
            
            var globalSettingsHandler = new SettingsHandler();
            var reader = new ReadSettingsFile();
            var directoryHandler = new DirectoryHandler();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestGlobalSettingsHandler");
            var pathToSettingsFile = @"H.CLI.TestFiles\TestGlobalSettingsHandler";
            directoryHandler.GenerateGlobalSettingsFile(pathToSettingsFile, new Farm() { GeographicData = lethbridgeGeographicData, ClimateData = climatateData});
            reader.ReadGlobalSettings(pathToSettingsFile + @"\farm.settings");
            globalSettingsHandler.ApplySettingsFromUserFile(ref applicationData, ref farm, reader.GlobalSettingsDictionary);

            Assert.AreEqual(farm.Defaults.Alfa, 0.7);
            Assert.AreEqual(Math.Round(farm.Defaults.CarbonConcentration, 4), 0.45);
            Assert.AreEqual(farm.Defaults.EmergenceDay, 141);
            Assert.AreEqual(farm.Defaults.RipeningDay, 197);
            Assert.AreEqual(farm.Defaults.Variance, 300);
            Assert.AreEqual(farm.Defaults.DecompositionMinimumTemperature, -3.78);
            Assert.AreEqual(farm.Defaults.DecompositionMaximumTemperature, 30.0);
            Assert.AreEqual(farm.Defaults.MoistureResponseFunctionAtSaturation, 0.42);
            Assert.AreEqual(farm.Defaults.MoistureResponseFunctionAtWiltingPoint, 0.18);

            //Annual Crops
            Assert.AreEqual(farm.Defaults.PercentageOfProductReturnedToSoilForAnnuals, 2.0);
            Assert.AreEqual(farm.Defaults.PercentageOfStrawReturnedToSoilForAnnuals, 100.0);
            Assert.AreEqual(farm.Defaults.PercentageOfRootsReturnedToSoilForAnnuals, 100.0);

            //Silage Crops
            Assert.AreEqual(farm.Defaults.PercentageOfProductYieldReturnedToSoilForSilageCrops, 35.0);
            Assert.AreEqual(farm.Defaults.PercentageOfRootsReturnedToSoilForSilageCrops, 100.0);

            //Cover Crops
            Assert.AreEqual(farm.Defaults.PercentageOfProductYieldReturnedToSoilForCoverCrops, 100.0);
            Assert.AreEqual(farm.Defaults.PercentageOfProductYieldReturnedToSoilForCoverCropsForage, 35.0);
            Assert.AreEqual(farm.Defaults.PercentageOfProductYieldReturnedToSoilForCoverCropsProduce, 0.0);
            Assert.AreEqual(farm.Defaults.PercentageOfStrawReturnedToSoilForCoverCrops, 100.0);
            Assert.AreEqual(farm.Defaults.PercetageOfRootsReturnedToSoilForCoverCrops, 100.0);

            //Root Crops
            Assert.AreEqual(farm.Defaults.PercentageOfStrawReturnedToSoilForRootCrops, 100);
            Assert.AreEqual(farm.Defaults.PercentageOfProductReturnedToSoilForRootCrops, 0.0);

            //Perennial Crops
            Assert.AreEqual(farm.Defaults.PercentageOfProductReturnedToSoilForPerennials, 35.0);
            Assert.AreEqual(farm.Defaults.PercentageOfRootsReturnedToSoilForPerennials, 100.0);

            //Rangeland
            Assert.AreEqual(farm.Defaults.PercentageOfProductReturnedToSoilForRangelandDueToHarvestLoss, 35.0);
           //Assert.AreEqual(farm.Defaults.PercentageOfProductReturnedToSoilForRangelandDueToGrazingLoss, 0.0);
            Assert.AreEqual(farm.Defaults.PercentageOfRootsReturnedToSoilForRangeland, 100.0);
           
            //Fodder Corn
            Assert.AreEqual(farm.Defaults.PercentageOfProductReturnedToSoilForFodderCorn, 35.0);
            Assert.AreEqual(farm.Defaults.PercentageOfRootsReturnedToSoilForFodderCorn, 100.0);
            Assert.AreEqual(farm.Defaults.HumificationCoefficientAboveGround, 0.125);
            Assert.AreEqual(farm.Defaults.HumificationCoefficientBelowGround, 0.3);
            Assert.AreEqual(farm.Defaults.HumificationCoefficientManure, 0.31);
            Assert.AreEqual(farm.Defaults.DecompositionRateConstantOldPool, 0.00605);
            Assert.AreEqual(farm.Defaults.DecompositionRateConstantYoungPool, 0.8);
            Assert.AreEqual(farm.Defaults.OldPoolCarbonN, 0.1);
            Assert.AreEqual(farm.Defaults.NORatio, 0.1);
            Assert.AreEqual(farm.Defaults.EmissionFactorForLeachingAndRunoff, 0.011);
            Assert.AreEqual(farm.Defaults.EmissionFactorForVolatilization, 0.01);
            Assert.AreEqual(farm.Defaults.FractionOfNLostByVolatilization, 0.21);
            Assert.AreEqual(farm.Defaults.MicrobeDeath, 0.2);
            Assert.AreEqual(farm.Defaults.Denitrification, 0.5);

            //Precipitation Data
            Assert.AreEqual(farm.ClimateData.PrecipitationData.January, 13.7223);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.February, 11.9852);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.March, 20.6658);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.April, 27.8472);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.May, 48.1653);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.June, 75.8647);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.July, 36.6482);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.August, 35.9121);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.September, 37.3989);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.October, 17.814);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.November, 15.8361);
            Assert.AreEqual(farm.ClimateData.PrecipitationData.December, 14.5102);
            //Evanpotranspiration Data

            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.January, 4.8688);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.February, 9.8498);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.March, 37.3942);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.April, 74.7692);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.May, 111.9281);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.June, 129.2335);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.July, 135.6777);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.August, 113.1369);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.September, 71.7394);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.October, 38.6017);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.November, 13.2771);
            Assert.AreEqual(farm.ClimateData.EvapotranspirationData.December, 4.6077);

            //TemperatureData
            Assert.AreEqual(farm.ClimateData.TemperatureData.January, -6.4094);
            Assert.AreEqual(farm.ClimateData.TemperatureData.February, -4.3469);
            Assert.AreEqual(farm.ClimateData.TemperatureData.March, 0.2081);
            Assert.AreEqual(farm.ClimateData.TemperatureData.April, 6.1523);
            Assert.AreEqual(farm.ClimateData.TemperatureData.May, 11.4057);
            Assert.AreEqual(farm.ClimateData.TemperatureData.June, 15.3654);
            Assert.AreEqual(farm.ClimateData.TemperatureData.July, 18.2907);
            Assert.AreEqual(farm.ClimateData.TemperatureData.August, 17.7328);
            Assert.AreEqual(farm.ClimateData.TemperatureData.September, 12.5254);
            Assert.AreEqual(farm.ClimateData.TemperatureData.October, 6.7026);
            Assert.AreEqual(farm.ClimateData.TemperatureData.November, -0.9397);
            Assert.AreEqual(farm.ClimateData.TemperatureData.December, -5.8802);

            //DefaultSoilData
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.YearOfObservation, DateTime.Now.Year);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.EcodistrictId, 793);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.SoilGreatGroup, SoilGreatGroupType.Regosol);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.BulkDensity, 1.45);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.SoilTexture, SoilTexture.Medium);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.SoilPh, 7);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.TopLayerThickness, 1000);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.ProportionOfSandInSoil, 0.4);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.ProportionOfClayInSoil, 0.3);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.ProportionOfSoilOrganicCarbon, 1);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.ProportionOfSandInSoilAsPercentage, 40);
            Assert.AreEqual(farm.GeographicData.DefaultSoilData.ProportionOfClayInSoilAsPercentage, 30);
        }

        [TestMethod]
        public void TestInitializePolygonID_UserChoiceIs3_ExpectLethbridgeSettingsFileToBeMade()
        {
            CLILanguageConstants.SetCulture(CultureInfo.GetCultureInfo("en-CA"));
            var globalSettingsHandler = new SettingsHandler();
            var reader = new ReadSettingsFile();

            globalSettingsHandler.InitializePolygonIDList(geographicDataProvider);

            var userChoice = 3;
            var pathToUserChoice3SettingsFile = @"H.CLI.TestFiles\TestGlobalSettingsHandler\TestUserChoice3";
            Directory.CreateDirectory(pathToUserChoice3SettingsFile);
           globalSettingsHandler.ExecuteUserChoice(userChoice, geographicDataProvider, pathToUserChoice3SettingsFile);

            reader.ReadGlobalSettings(pathToUserChoice3SettingsFile + @"\Farm.settings");

            Assert.AreEqual(reader.GlobalSettingsDictionary["January Mean Temperature"], "-6.1703");

        }

    }
}
