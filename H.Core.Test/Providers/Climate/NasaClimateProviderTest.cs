using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
        public void IsValidDailyDataReturnsFalseDuring2025()
        {
            var dailyData = new DailyClimateData();
            dailyData.Year = 2025; // Year NASA API was shutdown in October due to US government funding issues
            dailyData.SolarRadiation = -999;

            Assert.IsFalse(_nasaClimateProvider.IsValidDailyData(dailyData));
        }

        [TestMethod]
        public void IsValidDailyDataReturnsTrue()
        {
            var dailyData = new DailyClimateData();
            dailyData.Year = 2024;
            dailyData.SolarRadiation = 100;

            Assert.IsTrue(_nasaClimateProvider.IsValidDailyData(dailyData));
        }

        [TestMethod]
        public void IsCachedReturnsTrue()
        {
            const double latitude = 49.6;
            const double longitude = 112.8;

            var data = _nasaClimateProvider.GetCustomClimateData(latitude, longitude);

            // data will be an empty list if an exception was thrown
            // (i.e. 502 Gateway Error) or etc. during the NASA Api Call
            if (!data.Any())
            {
                return;
            }

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
            if (northData.Any() == false || eastData.Any() == false || southData.Any() == false || westData.Any() == false || centralData.Any() == false)
            {
                return;
            }

            // the growing season
            // May 1 = 122nd day of year
            // October 31 = 305 day of year

            var a = northData.Where(x => x.Year == 1985 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.SolarRadiation);
            var b = northData.Where(x => x.Year == 1984 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.SolarRadiation);
            var c = northData.Where(x => x.Year == 1983 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.SolarRadiation); 
            var d = northData.Where(x => x.Year == 1982 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.SolarRadiation);

            var i = northData.Where(x => x.Year == 2025 && x.JulianDay >= 122 && x.JulianDay <= 364).Select(x => new { Date = x.Date, t = x.MeanDailyAirTemperature, precip = x.MeanDailyPrecipitation, pet = x.MeanDailyPET, solar = x.SolarRadiation, minTemp = x.MinimumAirTemperature, maxTemp = x.MaximumAirTemperature});
            var j = northData.Where(x => x.Year == 2024 && x.JulianDay >= 122 && x.JulianDay <= 364).Select(x => new { Date = x.Date, t = x.MeanDailyAirTemperature, precip = x.MeanDailyPrecipitation, pet = x.MeanDailyPET, solar = x.SolarRadiation, minTemp = x.MinimumAirTemperature, maxTemp = x.MaximumAirTemperature });

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
        [Ignore]
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

        #region Precipitation scale

        /// <summary>
        /// The precipitation in a response has come back multiplied when the request reaches into the months the
        /// service has not finished reanalysing. The factor is measured against a window from the settled part of the
        /// record rather than assumed, so that nothing is corrected while the service answers consistently.
        /// </summary>
        [TestMethod]
        public void AUniformlyScaledResponseIsMeasuredAtItsScale()
        {
            var calibration = BuildCalibration(0.77, 1.26, 3.63, 0.38);
            var response = BuildResponse(0.77 * 24, 1.26 * 24, 3.63 * 24, 0.38 * 24);

            Assert.AreEqual(24, NasaClimateProvider.MeasurePrecipitationScalingFactor(calibration, response).Value, 0.0001);
        }

        [TestMethod]
        public void AResponseThatAgreesWithTheCalibrationIsLeftAlone()
        {
            var calibration = BuildCalibration(0.77, 1.26, 3.63, 0.38);
            var response = BuildResponse(0.77, 1.26, 3.63, 0.38);

            Assert.AreEqual(1, NasaClimateProvider.MeasurePrecipitationScalingFactor(calibration, response).Value, 0.0001,
                "a response at the right scale was going to be divided");
        }

        /// <summary>
        /// Every day of a scaled response is scaled alike. Days that disagree mean something other than a uniform
        /// scaling, which is not something this is entitled to correct, so the scale cannot be measured.
        /// </summary>
        [TestMethod]
        public void DaysThatDisagreeWithEachOtherCannotBeMeasured()
        {
            var calibration = BuildCalibration(0.77, 1.26, 3.63, 0.38);
            var response = BuildResponse(0.77 * 24, 1.26 * 24, 3.63 * 11, 0.38 * 24);

            Assert.IsNull(NasaClimateProvider.MeasurePrecipitationScalingFactor(calibration, response));
        }

        /// <summary>
        /// A dry day says nothing about the scale, so there have to be enough wet ones to compare.
        /// </summary>
        [TestMethod]
        public void TooFewWetDaysToCompareCannotBeMeasured()
        {
            var calibration = BuildCalibration(0.77, 0, 0, 0);
            var response = BuildResponse(0.77 * 24, 0, 0, 0);

            Assert.IsNull(NasaClimateProvider.MeasurePrecipitationScalingFactor(calibration, response));
        }

        /// <summary>
        /// Correcting only ever divides. A response smaller than the calibration is not something this understands,
        /// and multiplying it up would invent rainfall.
        /// </summary>
        [TestMethod]
        public void AResponseSmallerThanTheCalibrationIsLeftAlone()
        {
            var calibration = BuildCalibration(0.77, 1.26, 3.63, 0.38);
            var response = BuildResponse(0.77 / 24, 1.26 / 24, 3.63 / 24, 0.38 / 24);

            Assert.AreEqual(1, NasaClimateProvider.MeasurePrecipitationScalingFactor(calibration, response).Value, 0.0001);
        }

        [TestMethod]
        public void NoCalibrationWindowCannotBeMeasured()
        {
            var response = BuildResponse(0.77 * 24, 1.26 * 24, 3.63 * 24, 0.38 * 24);

            Assert.IsNull(NasaClimateProvider.MeasurePrecipitationScalingFactor(new Dictionary<DateTime, double>(), response));
            Assert.IsNull(NasaClimateProvider.MeasurePrecipitationScalingFactor(null, response));
        }

        [TestMethod]
        public void ParsePrecipitationByDateReadsTheDaysAndSkipsTheUndefinedOnes()
        {
            var content = "{\"properties\":{\"parameter\":{\"PRECTOTCORR\":" +
                          "{\"20200601\":0.77,\"20200602\":-999.0,\"20200603\":1.26}}}}";

            var result = NasaClimateProvider.ParsePrecipitationByDate(content);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0.77, result[new DateTime(2020, 6, 1)], 0.0001);
            Assert.AreEqual(1.26, result[new DateTime(2020, 6, 3)], 0.0001);
            Assert.IsFalse(result.ContainsKey(new DateTime(2020, 6, 2)), "the placeholder for a missing day was read as rainfall");
        }

        [TestMethod]
        public void ParsePrecipitationByDateHandlesAResponseWithNothingInIt()
        {
            Assert.AreEqual(0, NasaClimateProvider.ParsePrecipitationByDate(string.Empty).Count);
            Assert.AreEqual(0, NasaClimateProvider.ParsePrecipitationByDate("{}").Count);
        }

        private static Dictionary<DateTime, double> BuildCalibration(params double[] values)
        {
            var result = new Dictionary<DateTime, double>();
            for (var i = 0; i < values.Length; i++)
            {
                result[new DateTime(2020, 6, 1).AddDays(i)] = values[i];
            }

            return result;
        }

        private static List<DailyClimateData> BuildResponse(params double[] values)
        {
            var result = new List<DailyClimateData>();
            for (var i = 0; i < values.Length; i++)
            {
                result.Add(new DailyClimateData()
                {
                    Date = new DateTime(2020, 6, 1).AddDays(i),
                    Year = 2020,
                    MeanDailyPrecipitation = values[i],
                });
            }

            return result;
        }

        /// <summary>
        /// The scale check could not be made, so nothing shows whether the precipitation is at its true size. The response
        /// is treated as a failed download, and the check is tried once more before giving up.
        /// </summary>
        [TestMethod]
        public void AResponseIsRejectedWhenItsScaleCannotBeChecked()
        {
            var latitude = 21.1357;
            var longitude = -111.2468;
            var calibrationRequests = 0;
            var provider = BuildProviderHolding(latitude, longitude, BuildFirstHalfOf2020Response(1), url =>
            {
                calibrationRequests++;
                throw new WebException("Unable to connect to the remote server");
            });

            try
            {
                Assert.AreEqual(0, provider.GetCustomClimateData(latitude, longitude).Count,
                    "a response whose precipitation scale was never checked reached the farm");
                Assert.AreEqual(2, calibrationRequests, "a failed scale check should be tried once more");
            }
            finally
            {
                provider.ClearCachedData(latitude, longitude);
            }
        }

        /// <summary>
        /// A calibration request that comes back without precipitation - an error page or a maintenance notice - is a
        /// failed request, not a measurement.
        /// </summary>
        [TestMethod]
        public void ACalibrationResponseWithNoPrecipitationIsAFailedCheck()
        {
            var latitude = 22.1357;
            var longitude = -112.2468;
            var provider = BuildProviderHolding(latitude, longitude, BuildFirstHalfOf2020Response(1), url => "{\"messages\":[\"Service unavailable\"]}");

            try
            {
                Assert.AreEqual(0, provider.GetCustomClimateData(latitude, longitude).Count);
            }
            finally
            {
                provider.ClearCachedData(latitude, longitude);
            }
        }

        /// <summary>
        /// A failed check is not an answer, so it is not remembered for the location: once the service answers, the next
        /// request checks again and the response is corrected.
        /// </summary>
        [TestMethod]
        public void AFailedScaleCheckIsNotRememberedForTheLocation()
        {
            var latitude = 23.1357;
            var longitude = -113.2468;
            var serviceIsReachable = false;
            var provider = BuildProviderHolding(latitude, longitude, BuildFirstHalfOf2020Response(24), url =>
            {
                if (serviceIsReachable == false)
                {
                    throw new WebException("Unable to connect to the remote server");
                }

                return June2020Calibration;
            });

            try
            {
                Assert.AreEqual(0, provider.GetCustomClimateData(latitude, longitude).Count);

                serviceIsReachable = true;
                var result = provider.GetCustomClimateData(latitude, longitude);

                Assert.AreEqual(FirstHalfOf2020Days, result.Count, "the second request was answered from the failed check");
                var juneTenth = result.Single(x => x.Date == new DateTime(2020, 6, 10));
                Assert.AreEqual(PrecipitationOn(juneTenth.Date), juneTenth.MeanDailyPrecipitation, 0.0001,
                    "the response was not divided back down");
            }
            finally
            {
                provider.ClearCachedData(latitude, longitude);
            }
        }

        [TestMethod]
        public void AResponseAtItsTrueScaleIsKeptAsIs()
        {
            var latitude = 24.1357;
            var longitude = -114.2468;
            var provider = BuildProviderHolding(latitude, longitude, BuildFirstHalfOf2020Response(1), url => June2020Calibration);

            try
            {
                var result = provider.GetCustomClimateData(latitude, longitude);

                Assert.AreEqual(FirstHalfOf2020Days, result.Count);
                var juneTenth = result.Single(x => x.Date == new DateTime(2020, 6, 10));
                Assert.AreEqual(PrecipitationOn(juneTenth.Date), juneTenth.MeanDailyPrecipitation, 0.0001);
            }
            finally
            {
                provider.ClearCachedData(latitude, longitude);
            }
        }

        /// <summary>
        /// January 1 to June 30, 2020. A response starts on January 1, as a real one does, since the provider numbers
        /// its days from the start of each year.
        /// </summary>
        private const int FirstHalfOf2020Days = 182;

        private static double PrecipitationOn(DateTime date) => date.DayOfYear % 3 == 0 ? 0 : 1.5 + (date.DayOfYear % 10) * 0.1;

        private static string June2020Calibration =>
            "{\"properties\":{\"parameter\":{\"PRECTOTCORR\":{" + BuildDays(new DateTime(2020, 6, 1), 30, PrecipitationOn) + "}}}}";

        private static string BuildFirstHalfOf2020Response(double precipitationScale)
        {
            string Parameter(string name, Func<DateTime, double> value) =>
                $"\"{name}\":{{{BuildDays(new DateTime(2020, 1, 1), FirstHalfOf2020Days, value)}}}";

            return "{\"properties\":{\"parameter\":{" +
                   Parameter("PRECTOTCORR", date => PrecipitationOn(date) * precipitationScale) + "," +
                   Parameter("T2M", date => 5) + "," +
                   Parameter("RH2M", date => 60) + "," +
                   Parameter("ALLSKY_SFC_SW_DWN", date => 15) + "," +
                   Parameter("T2M_MIN", date => -1) + "," +
                   Parameter("T2M_MAX", date => 11) +
                   "}}}";
        }

        private static string BuildDays(DateTime start, int days, Func<DateTime, double> value)
        {
            return string.Join(",", Enumerable.Range(0, days).Select(day => start.AddDays(day)).Select(date =>
                $"\"{date:yyyyMMdd}\":{value(date).ToString(CultureInfo.InvariantCulture)}"));
        }

        /// <summary>
        /// A provider holding the given main response for the location, as it would after downloading it, with every
        /// further request answered by <paramref name="download"/>.
        /// </summary>
        private static NasaClimateProvider BuildProviderHolding(double latitude, double longitude, string response, Func<string, string> download)
        {
            var provider = new NasaClimateProvider(download);
            provider.ClearCachedData(latitude, longitude);

            File.WriteAllText(Path.GetTempPath() + "\\" + $"nasa_climate_data_lat_{latitude}_long_{longitude}", response);

            return provider;
        }

        #endregion

        #region Clearing what is held for a location

        /// <summary>
        /// The response for a location is kept on disk for an hour and every read puts that hour back, so a location in
        /// use never expires on its own. Asking for the data again therefore cannot replace it - what is held has to be
        /// thrown away, which is what the re-download on the farm relies on.
        /// </summary>
        [TestMethod]
        public void ClearCachedDataDiscardsWhatIsHeldForThatLocation()
        {
            var provider = new NasaClimateProvider();

            var latitude = 12.3456;
            var longitude = -98.7654;

            var path = Path.GetTempPath() + "\\" + $"nasa_climate_data_lat_{latitude}_long_{longitude}";
            File.WriteAllText(path, "{}");

            Assert.IsTrue(provider.IsCached(latitude, longitude), "this test needs something held for the location");

            provider.ClearCachedData(latitude, longitude);

            Assert.IsFalse(provider.IsCached(latitude, longitude),
                "the response held for the location survived, so the next request would be answered from it");
        }

        /// <summary>
        /// Nothing held for the location is the ordinary case the first time one is used.
        /// </summary>
        [TestMethod]
        public void ClearCachedDataWithNothingHeldDoesNotThrow()
        {
            var provider = new NasaClimateProvider();

            provider.ClearCachedData(11.1111, -22.2222);
        }

        #endregion
    }
}
