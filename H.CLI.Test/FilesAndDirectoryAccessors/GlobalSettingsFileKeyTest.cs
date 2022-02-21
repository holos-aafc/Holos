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
    public class GlobalSettingsFileKeyTest
    {
        public static readonly GeographicDataProvider geographicDataProvider = new GeographicDataProvider();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            geographicDataProvider.Initialize();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }

        [TestMethod]
        public void TestGlobalSettingsFileKeyConstructorMetric_EnglishCanada()
        {

            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Metric;

            var lethbridgePolygonID = 793006;
            var climateProvider = new SlcClimateDataProvider();
            var climateData = climateProvider.GetClimateData(lethbridgePolygonID, TimeFrame.NineteenFiftyToNineteenEighty);
            var lethbridgeGeographicData = geographicDataProvider.GetGeographicalData(lethbridgePolygonID);

            var globalKey = new BuildSettingsFileString(new Farm() { GeographicData = lethbridgeGeographicData, ClimateData = climateData });
            globalKey.keys.RemoveAll(x => x.Contains("#"));

            var filteredGlobalKeys = globalKey.keys.GroupBy(x => x.Split('=')).ToDictionary(x => x.Key[0].Trim(), x => x.Key[1].Trim());

            //Metric values.
            Assert.AreEqual(filteredGlobalKeys.Count(), 92);
            Assert.AreEqual(filteredGlobalKeys["Alfa"], "0.7");
            Assert.AreEqual(filteredGlobalKeys["Decomposition Minimum Temperature  (°C)"], "-3.78");
        }

        [TestMethod]
        public void TestGlobalSettingsFileKeyConstructorImperial_EnglishCanada()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;

            var lethbridgePolygonID = 793006;
            var climateProvider = new SlcClimateDataProvider();
            var climateData = climateProvider.GetClimateData(lethbridgePolygonID, TimeFrame.NineteenFiftyToNineteenEighty);
            var lethbridgeGeographicData = geographicDataProvider.GetGeographicalData(lethbridgePolygonID);

            var globalKey = new BuildSettingsFileString(new Farm() { GeographicData = lethbridgeGeographicData, ClimateData = climateData });
            globalKey.keys.RemoveAll(x => x.Contains("#"));

            var filteredGlobalKeys = globalKey.keys.GroupBy(x => x.Split('=')).ToDictionary(x => x.Key[0].Trim(), x => x.Key[1].Trim());

            //Imperial values.  Assert.AreEqual(filteredGlobalKeys[], "");
            Assert.AreEqual(filteredGlobalKeys.Count(), 92);
            Assert.AreEqual(filteredGlobalKeys["Alfa"], "0.7");
            Assert.AreEqual(filteredGlobalKeys["Decomposition Minimum Temperature  (°F)"], "25.196");

        }

        [TestMethod]
        public void TestGlobalSettingsFileKeyConstructorFrench_FrenchCanada()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("fr-CA"));
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Metric;

            var lethbridgePolygonID = 793006;
            var climateProvider = new SlcClimateDataProvider();
            var climateData = climateProvider.GetClimateData(lethbridgePolygonID, TimeFrame.NineteenFiftyToNineteenEighty);
            var lethbridgeGeographicData = geographicDataProvider.GetGeographicalData(lethbridgePolygonID);

            var globalKey = new BuildSettingsFileString(new Farm() { GeographicData = lethbridgeGeographicData, ClimateData = climateData});
            globalKey.keys.RemoveAll(x => x.Contains("#"));

            var filteredGlobalKeys = globalKey.keys.GroupBy(x => x.Split('=')).ToDictionary(x => x.Key[0].Trim(), x => x.Key[1].Trim());

            //Metric values. French => Commas Instead of Decimal Points
            Assert.AreEqual(filteredGlobalKeys.Count(), 92);
            Assert.AreEqual(filteredGlobalKeys["Alfa"], "0,7");
            Assert.AreEqual(filteredGlobalKeys["Decomposition Minimum Temperature  (°C)"], "-3,78");
        }

        [TestMethod]
        public void TestGlobalSettingsFileKeyConstructorImperialAndFrench()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("fr-CA"));
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;

            var lethbridgePolygonID = 793006;
            var climateProvider = new SlcClimateDataProvider();
            var climateData = climateProvider.GetClimateData(lethbridgePolygonID, TimeFrame.NineteenFiftyToNineteenEighty);

            var lethbridgeGeographicData = geographicDataProvider.GetGeographicalData(lethbridgePolygonID);
            var globalKey = new BuildSettingsFileString(new Farm() { GeographicData = lethbridgeGeographicData, ClimateData = climateData });            

            globalKey.keys.RemoveAll(x => x.Contains("#"));

            var filteredGlobalKeys = globalKey.keys.GroupBy(x => x.Split('=')).ToDictionary(x => x.Key[0].Trim(), x => x.Key[1].Trim());
            Assert.AreEqual(filteredGlobalKeys.Count(), 92);
            Assert.AreEqual(filteredGlobalKeys["Alfa"], "0,7");
            Assert.AreEqual(filteredGlobalKeys["Decomposition Minimum Temperature  (°F)"], "25,196");

        }

        [TestMethod]
        public void TestTemplateFarmHandler()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestTemplateFarmHandler\Farms");
            var pathToFarmsDirectory = @"H.CLI.TestFiles\TestTemplateFarmHandler\Farms";
            var templateFarmHandler = new TemplateFarmHandler();

            templateFarmHandler.CreateTemplateFarmIfNotExists(pathToFarmsDirectory, geographicDataProvider);
            var oneFarmMade = Directory.GetDirectories(pathToFarmsDirectory).Count();
            Assert.AreEqual(oneFarmMade, 1);
            var numberOfComponents = Directory.GetDirectories(pathToFarmsDirectory + @"\HolosExampleFarm");
            Assert.AreEqual(numberOfComponents.Count(), 8);
            Assert.IsTrue(File.Exists(pathToFarmsDirectory + @"\HolosExampleFarm\farm.settings"));
        }
    }
}
