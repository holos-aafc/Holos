using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using H.Content;
using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Climate
{
    [TestClass]
    public class NasaClimateProviderTest
    {

        #region Fields

        private NasaClimateProvider _nasaClimateProvider;

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
            _nasaClimateProvider = new NasaClimateProvider();
            //Thread.CurrentThread.CurrentCulture = Infrastructure.InfrastructureConstants.FrenchCultureInfo;
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void IsCachedReturnsTrue()
        {
            const double latitude = 49.6;
            const double longitude = 112.8;

            _nasaClimateProvider.GetCustomClimateData(latitude, longitude);

            Assert.IsTrue(_nasaClimateProvider.IsCached(latitude, longitude));
        }

        [TestMethod]
        public void GetGrowingSeasonPrecipitationFromOntarioReturnsMoreThan400()
        {
            //central ontario
            const double centralLatitude = 49.837;
            const double centralLongitude = -84.535;

            //north ontario
            const double northLatitude = 56.470;
            const double northLongitude = -88.846;

            //east ontario
            const double eastLatitude = 45.354;
            const double eastLongitude = -74.769;

            //south ontario
            const double southLatitude = 42.904;
            const double southLongitude = -80.612;

            //west ontario
            const double westLatitude = 49.533;
            const double westLongitude = -94.866;


            var northData = _nasaClimateProvider.GetCustomClimateData(northLatitude, northLongitude);
            var eastData = _nasaClimateProvider.GetCustomClimateData(eastLatitude, eastLongitude);
            var southData = _nasaClimateProvider.GetCustomClimateData(southLatitude, southLongitude);
            var westData = _nasaClimateProvider.GetCustomClimateData(westLatitude, westLongitude);
            var centralData = _nasaClimateProvider.GetCustomClimateData(centralLatitude, centralLongitude);

            // We will get empty collections if Nasa service is offline. Return from test in this case since we need data to calculate growing season values
            if (northData.Any() == false || eastData.Any() == false || southData.Any() == false || centralData.Any() == false)
            {
                return;
            }

            // the growing season
            // May 1 = 122nd day of year
            // October 31 = 305 day of year

            // present data
            var northGrowingSeasonValues = northData.Where(x => x.Year == 2019 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);
            var eastGrowingSeasonValues = eastData.Where(x => x.Year == 2019 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);
            var southGrowingSeasonValues = southData.Where(x => x.Year == 2019 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);
            var westGrowingSeasonValues = westData.Where(x => x.Year == 2019 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);
            var centralGrowingSeasonValues = centralData.Where(x => x.Year == 2019 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);

            var northResult = northGrowingSeasonValues.Sum();
            var eastResult = eastGrowingSeasonValues.Sum();
            var southResult = southGrowingSeasonValues.Sum();
            var westResult = westGrowingSeasonValues.Sum();
            var centralResult = centralGrowingSeasonValues.Sum();
            var average = (northResult + eastResult + westResult + southResult + centralResult) / 5;

            // 1985 data
            var northGrowingSeasonValues85 = northData.Where(x => x.Year == 1985 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);
            var eastGrowingSeasonValues85 = eastData.Where(x => x.Year == 1985 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);
            var southGrowingSeasonValues85 = southData.Where(x => x.Year == 1985 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);
            var westGrowingSeasonValues85 = westData.Where(x => x.Year == 1985 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);
            var centralGrowingSeasonValues85 = centralData.Where(x => x.Year == 1985 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation);

            var northResult85 = northGrowingSeasonValues85.Sum();
            var eastResult85 = eastGrowingSeasonValues85.Sum();
            var southResult85 = southGrowingSeasonValues85.Sum();
            var westResult85 = westGrowingSeasonValues85.Sum();
            var centralResult85 = centralGrowingSeasonValues85.Sum();
            var average85 = (northResult85 + eastResult85 + westResult85 + southResult85 + centralResult85) / 5;

            // present assertions
            Assert.IsTrue(northResult > 450);
            Assert.IsTrue(eastResult > 450);
            Assert.IsTrue(southResult > 400);
            Assert.IsTrue(westResult > 450);
            Assert.IsTrue(centralResult > 400);
            Assert.IsTrue(average > 450);

            // 1985 assertions
            Assert.IsTrue(northResult85 > 450);
            Assert.IsTrue(eastResult85 > 450);
            Assert.IsTrue(southResult85 > 400);
            Assert.IsTrue(westResult85 > 500);
            Assert.IsTrue(centralResult85 > 450);
            Assert.IsTrue(average85 > 450);


        }

        /// <summary>
        /// Was an integration test to see if potential evapotranspiration was being calculated correctly
        /// </summary>
        [TestMethod]
        public void CompareAgainstSLCDailyValues()
        {
            var data = _nasaClimateProvider.GetCustomClimateData(50.259197, -107.734873);
            var dataFor2000 = data.Where(x => x.Year == 2000);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Julian Day,Temperature,Precipitation,PET");

            foreach (var item in dataFor2000)
            {
                stringBuilder.Append(item.JulianDay);
                stringBuilder.Append(",");
                stringBuilder.Append(item.MeanDailyAirTemperature);
                stringBuilder.Append(",");
                stringBuilder.Append(item.MeanDailyPrecipitation);
                stringBuilder.Append(",");
                stringBuilder.Append(item.MeanDailyPET);
                stringBuilder.AppendLine(",");
            }

            File.WriteAllText("Nasa_Daily_Climate_Swift_Current_Year_2000.csv", stringBuilder.ToString());
        }

        [TestMethod]
        public void GetClimateData()
        {
            var lines = CsvResourceReader.SplitFileIntoLinesUsingRegex(File.ReadAllText("Climate\\Missing Climate Data_perennials.csv"));

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Sub_location");
            stringBuilder.Append(",");
            stringBuilder.Append("Latitude (decimal degrees)");
            stringBuilder.Append(",");
            stringBuilder.Append("Longitude (decimal degrees)");
            stringBuilder.Append(",");
            stringBuilder.Append("Date(s) of study/measurement");
            stringBuilder.Append(",");
            stringBuilder.Append("PPT (mm)");
            stringBuilder.Append(",");
            stringBuilder.Append("PET (mm)");
            stringBuilder.Append(",");
            stringBuilder.Append("MAY_SEP (PPT (mm))");
            stringBuilder.Append(",");
            stringBuilder.AppendLine();

            foreach (var line in lines.Skip(2))
            {
                var city = line[0];
                if (string.IsNullOrWhiteSpace(city))
                {
                    stringBuilder.AppendLine();
                    continue;
                }

                city = city.Replace(",", " ");

                var latitude = double.Parse(line[1]);
                var longitude = double.Parse(line[2]);
                var year = int.Parse(line[3]);

                var data = _nasaClimateProvider.GetCustomClimateData(latitude, longitude);
                var dataForYear = data.Where(x => x.Year == year);

                double totalAnnualPrecipitation = 0;
                double totalAnnualEvapotranspiration = 0;
                double totalGrowingSeasonPrecipitation = 0;
                if (dataForYear.Any())
                {
                    totalAnnualPrecipitation = dataForYear.Sum(x => x.MeanDailyPrecipitation);
                    totalAnnualEvapotranspiration = dataForYear.Sum(x => x.MeanDailyPET);

                    var growingSeasonDays = dataForYear.Where(x => x.JulianDay >= CoreConstants.GrowingSeasonJulianStartDay && x.JulianDay <= CoreConstants.GrowingSeasonJulianEndDaySeptember);

                    totalGrowingSeasonPrecipitation = growingSeasonDays.Sum(x => x.MeanDailyPrecipitation);
                }

                stringBuilder.Append(city);
                stringBuilder.Append(",");
                stringBuilder.Append(latitude);
                stringBuilder.Append(",");
                stringBuilder.Append(longitude);
                stringBuilder.Append(",");
                stringBuilder.Append(year);
                stringBuilder.Append(",");
                stringBuilder.Append(totalAnnualPrecipitation);
                stringBuilder.Append(",");
                stringBuilder.Append(totalAnnualEvapotranspiration);
                stringBuilder.Append(",");
                stringBuilder.Append(totalGrowingSeasonPrecipitation);
                stringBuilder.Append(",");
                stringBuilder.AppendLine();
            }

            File.WriteAllText($"Climate\\Output.csv", stringBuilder.ToString());
        }
    }
}
