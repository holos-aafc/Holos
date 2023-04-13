using H.Core.Calculators.Infrastructure;
using H.Core.Models.Animals.Beef;
using H.Core.Models;
using H.Core.Providers.Feed;
using H.Core.Providers;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using H.Core.Services;
using H.Core;
using H.Helpers;
using H.Views.ComponentViews.Beef.Backgrounders;
using H.Views.ComponentViews.Beef.CowCalf;
using H.Views.ComponentViews.Beef.Finishers;
using H.Views.DetailViews.Animals.BeefCattle.CowCalf;
using H.Views.ResultViews.DetailedEmissionReport;
using H.Views.SupportingViews.Menu;
using H.Views.TimelineViews.Animals.BeefCattle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.Infrastructure;
using System.Reflection;
using H.Content;
using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Providers.Climate;

namespace H.Integration.Hay_LCI
{
    [TestClass]
    [Ignore]
    public class HayLCIIntegrationTest
    {
        #region Internal Classes

        class Table3Item
        {
            public int Polygon { get; set; }
            public int Ecodistrict { get; set; }
            public double Area { get; set; }
            public double EcodistrictArea { get; set; }
        }

        #endregion

        #region Fields

        private static Defaults _defaults;
        private static ClimateNormalCalculator _climateNormalCalculator;
        private static NasaClimateProvider _nasaClimateProvider;
        private static GeographicDataProvider _geograhicDataProvider;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _defaults = new Defaults();

            _climateNormalCalculator = new ClimateNormalCalculator();
            _nasaClimateProvider = new NasaClimateProvider();
            _geograhicDataProvider = new GeographicDataProvider();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void HayLCI()
        {
            var slcList = this.ReadSlcs();
            var farms = this.BuldFarms(slcList);

            this.AssignClimateData(farms);
            this.AssignGeographicData(farms);
        }

        #endregion

        #region Private Methods

        private List<Table3Item> ReadSlcs()
        {
            var result = new List<Table3Item>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            var a = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            Stream resourceStream = assembly.GetManifestResourceStream("H.Integration.Resources.table_3_polygons.csv");

            var reader = CsvResourceReader.SplitFileIntoLinesUsingRegex(resourceStream);
            foreach (var row in reader.Skip(2))
            {
                var item = new Table3Item();
                item.Polygon = int.Parse(row[0]);
                item.Ecodistrict = int.Parse(row[1]);
                item.Area = double.Parse(row[2]);
                item.EcodistrictArea = double.Parse(row[3]);

                result.Add(item);
            }

            return result;
        }

        private List<Farm> BuldFarms(List<Table3Item> items)
        {
            var result = new List<Farm>();

            foreach (var table3Item in items)
            {
                var farm = new Farm();
                farm.PolygonId = table3Item.Polygon;

                result.Add(farm);
            }

            return result;
        }

        private void AssignClimateData(List<Farm> farms)
        {
            var startYear = 2009;
            var endYear = 2018;

            foreach (var farm in farms)
            {
                var nasaClimate = _nasaClimateProvider.GetCustomClimateData(farm.Latitude, farm.Longitude);
                var climateForPeriod = nasaClimate.Where(x => x.Date.Year >= startYear && x.Date.Year <= endYear).ToList();
                var temperatureNormalsForPeriod = _climateNormalCalculator.GetTemperatureDataByDailyValues(climateForPeriod, startYear, endYear);
                var precipitationNormalsForPeriod = _climateNormalCalculator.GetPrecipitationDataByDailyValues(climateForPeriod, startYear, endYear);
                var evapotranspirationNormalsForPeriod = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(climateForPeriod, startYear, endYear);

                farm.ClimateData.EvapotranspirationData = evapotranspirationNormalsForPeriod;
                farm.ClimateData.PrecipitationData = precipitationNormalsForPeriod;
                farm.ClimateData.TemperatureData = temperatureNormalsForPeriod;
            }
        }

        private void AssignGeographicData(List<Farm> farms)
        {
            foreach (var farm in farms)
            {
                var geographicData = _geograhicDataProvider.GetGeographicalData(farm.PolygonId);
                farm.GeographicData = geographicData;
            }
        }

        #endregion
    }
}
