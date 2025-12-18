#region Imports

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers;
using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace H.Core.Test.Providers
{
    [TestClass]
    public class SlcClimateNormalsDataProviderTest
    {
        #region Fields

        private static SlcClimateDataProvider _sut;

        private const int ProblemPolygon = 500025;

        //for comparing the climate normals new and old
        private static string path = Directory.GetCurrentDirectory();
        private static string extension = ".csv";
        private static string header = "month,old,new,difference";
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _sut = new SlcClimateDataProvider();
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
        public void GetTemperatureDataByPolygonIdReturnsTemperatureData()
        {
            var result = _sut.GetTemperatureDataByPolygonId(684007, TimeFrame.NineteenEightyToNineteenNinety);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Just want to see the variance between using SLC normals and NASA
        /// </summary>
        [TestMethod]
        public void ComparePrecipitationDataFromPolygonIDandDailyValueRanges()
        {
            var precipResult = _sut.GetPrecipitationDataByPolygonId(684007, TimeFrame.NineteenEightyToNineteenNinety);

            var yearlyPrecipAverages = precipResult.GetAveragedYearlyValues();

            double[] january = new double[31];
            yearlyPrecipAverages.CopyTo(0, january, 0, 31);
            var slcNormalValue = january.Average();

            var nasaClimateApiValue = 0.8228;

            Assert.AreNotEqual(nasaClimateApiValue, slcNormalValue);
        }

        /// <summary>
        /// Just want to see the variance between using SLC normals and NASA
        /// </summary>
        [TestMethod]
        public void CompareNasaSpecificTemperatureValueToSlcNormal()
        {
            var slcNormalsForPolygon = _sut.GetTemperatureDataByPolygonId(684007, TimeFrame.NineteenEightyToNineteenNinety);

            var nasaClimateApiValue = -12.6024;

            Assert.AreNotEqual(nasaClimateApiValue, slcNormalsForPolygon.GetMeanTemperatureForMonth(1));
        }

        [TestMethod]
        public void CheckGetPrecipitationDataByPolygonReadsCorrectFile()
        {
            var polygonId1 = 1001009;

            var actualJanuary1950 = 49.7130466285412;
            var actualJanuary1960 = 41.9644803133222;
            var actualJanuary1970 = 36.925483886967;
            var actualJanuary1980 = 36.9449462211527;
            var actualJanuary1990 = 40.4295635032024;
            var actualJanuaryOriginal = 57.77868896;

            var precip1950data = _sut.GetPrecipitationDataByPolygonId(polygonId1, TimeFrame.NineteenFiftyToNineteenEighty);
            var precip1960data = _sut.GetPrecipitationDataByPolygonId(polygonId1, TimeFrame.NineteenSixtyToNineteenNinety);
            var precip1970data = _sut.GetPrecipitationDataByPolygonId(polygonId1, TimeFrame.NineteenSeventyToTwoThousand);
            var precip1980data = _sut.GetPrecipitationDataByPolygonId(polygonId1, TimeFrame.NineteenEightyToTwoThousandTen);
            var precip1990data = _sut.GetPrecipitationDataByPolygonId(polygonId1, TimeFrame.NineteenNinetyToTwoThousandSeventeen);
            var originalPrecip = _sut.GetPrecipitationDataByPolygonId(polygonId1, TimeFrame.ProjectionPeriod);

            Assert.AreEqual(actualJanuary1950, precip1950data.January);
            Assert.AreEqual(actualJanuary1960, precip1960data.January);
            Assert.AreEqual(actualJanuary1970, precip1970data.January);
            Assert.AreEqual(actualJanuary1980, precip1980data.January);
            Assert.AreEqual(actualJanuary1990, precip1990data.January);
            Assert.AreEqual(actualJanuaryOriginal, originalPrecip.January);
        }

        [TestMethod]
        public void CheckGetEvapotranspirationByPolygonReadsCorrectFile()
        {
            var polygonId1 = 1001009;

            var actualJanuary1950 = 0.00129291600890194;
            var actualJanuary1960 = 0.00216533256127214;
            var actualJanuary1970 = 0.00216533256127214;
            var actualJanuary1980 = 0.000872416552370201;
            var actualJanuary1990 = 0;
            var actualJanuaryOriginal = 1.753113733;

            var PET1950data = _sut.GetEvapotranspirationDataByPolygonId(polygonId1, TimeFrame.NineteenFiftyToNineteenEighty);
            var PET1960data = _sut.GetEvapotranspirationDataByPolygonId(polygonId1, TimeFrame.NineteenSixtyToNineteenNinety);
            var PET1970data = _sut.GetEvapotranspirationDataByPolygonId(polygonId1, TimeFrame.NineteenSeventyToTwoThousand);
            var PET1980data = _sut.GetEvapotranspirationDataByPolygonId(polygonId1, TimeFrame.NineteenEightyToTwoThousandTen);
            var PET1990data = _sut.GetEvapotranspirationDataByPolygonId(polygonId1, TimeFrame.NineteenNinetyToTwoThousandSeventeen);
            var PETOriginalData = _sut.GetEvapotranspirationDataByPolygonId(polygonId1, TimeFrame.ProjectionPeriod);

            Assert.AreEqual(actualJanuary1950, PET1950data.January);
            Assert.AreEqual(actualJanuary1960, PET1960data.January);
            Assert.AreEqual(actualJanuary1970, PET1970data.January);
            Assert.AreEqual(actualJanuary1980, PET1980data.January);
            Assert.AreEqual(actualJanuary1990, PET1990data.January);
            Assert.AreEqual(actualJanuaryOriginal, PETOriginalData.January);

        }
        [TestMethod]
        public void CheckGetTemperatureByPolgyonReadsCorrectFileTest()
        {
            var polygonId1 = 1001009;

            var actualJanuary1950 = -8.47763148137817;
            var actualJanuary1960 = -6.60514259481136;
            var actualJanuary1970 = -5.99114321207558;
            var actualJanuary1980 = -4.43572407259617;
            var actualJanuary1990 = -3.96657036612799;
            var actualJanuaryOriginal = -4.503495576;

            var temp1950data = _sut.GetTemperatureDataByPolygonId(polygonId1, TimeFrame.NineteenFiftyToNineteenEighty);
            var temp1960data = _sut.GetTemperatureDataByPolygonId(polygonId1, TimeFrame.NineteenSixtyToNineteenNinety);
            var temp1970data = _sut.GetTemperatureDataByPolygonId(polygonId1, TimeFrame.NineteenSeventyToTwoThousand);
            var temp1980data = _sut.GetTemperatureDataByPolygonId(polygonId1, TimeFrame.NineteenEightyToTwoThousandTen);
            var temp1990data = _sut.GetTemperatureDataByPolygonId(polygonId1, TimeFrame.NineteenNinetyToTwoThousandSeventeen);
            var tempOriginaldata = _sut.GetTemperatureDataByPolygonId(polygonId1, TimeFrame.ProjectionPeriod);

            Assert.AreEqual(actualJanuary1950, temp1950data.January);
            Assert.AreEqual(actualJanuary1960, temp1960data.January);
            Assert.AreEqual(actualJanuary1970, temp1970data.January);
            Assert.AreEqual(actualJanuary1980, temp1980data.January);
            Assert.AreEqual(actualJanuary1990, temp1990data.January);
            Assert.AreEqual(actualJanuaryOriginal, tempOriginaldata.January);

        }


        [TestMethod]
        //for lethbridge
        public void CompareOldTemperatureDataToNewTemperatureData()
        {
            var lethbridgePolygon = 793011;
            // get the data from the new climate norms
            var newTemperatureData = _sut.GetTemperatureDataByPolygonId(lethbridgePolygon, TimeFrame.NineteenEightyToTwoThousandTen);

            //get the data from the old climate by passing in an unhandled timeframe
            var oldTemperatureData = _sut.GetTemperatureDataByPolygonId(lethbridgePolygon, TimeFrame.ProjectionPeriod);

            var lines = new List<string>();
            lines.Add(header);
            foreach (var month in Enum.GetNames(typeof(Months)).Cast<string>())
            {
                var newTempsMonth = (double)newTemperatureData.GetType().GetProperty(month).GetValue(newTemperatureData);
                var oldTempsMonth = (double)oldTemperatureData.GetType().GetProperty(month).GetValue(oldTemperatureData);
                var result = oldTempsMonth - newTempsMonth;
                lines.Add($"{month},{oldTempsMonth},{newTempsMonth},{result}");
                Assert.AreNotEqual(newTempsMonth, oldTempsMonth);
            }
            File.WriteAllLines($"{path}\\TemperatureComparisonLethbridge1980-2010{extension}", lines);
        }

        [TestMethod]
        //for lethbridge
        public void CompareOldPrecipitationDataToNewPrecipitationData()
        {
            var lethbridgePolygon = 793011;
            // get the data from the new climate norms
            var newPrecipitationData = _sut.GetPrecipitationDataByPolygonId(lethbridgePolygon, TimeFrame.NineteenEightyToTwoThousandTen);

            //get the data from the old climate by passing in an unhandled timeframe
            var oldPrecipitationData = _sut.GetPrecipitationDataByPolygonId(lethbridgePolygon, TimeFrame.ProjectionPeriod);

            var lines = new List<string>();
            lines.Add(header);
            foreach (var month in Enum.GetNames(typeof(Months)).Cast<string>())
            {
                var newMonth = (double)newPrecipitationData.GetType().GetProperty(month).GetValue(newPrecipitationData);
                var oldMonth = (double)oldPrecipitationData.GetType().GetProperty(month).GetValue(oldPrecipitationData);
                var result = oldMonth - newMonth;
                lines.Add($"{month},{oldMonth},{newMonth},{result}");
                Assert.AreNotEqual(newMonth, oldMonth);
            }
            File.WriteAllLines($"{path}\\PrecipitationComparisionLethbridge1980-2010{extension}", lines);
        }

        [TestMethod]
        //for lethbridge
        public void CompareOldPETwithNewPET()
        {
            var lethbridgePolygon = 793011;
            // get the data from the new climate norms
            var newEvapotranspirationData = _sut.GetEvapotranspirationDataByPolygonId(lethbridgePolygon, TimeFrame.NineteenEightyToTwoThousandTen);

            //get the data from the old climate by passing in an unhandled timeframe
            var oldEvapotranspirationData = _sut.GetEvapotranspirationDataByPolygonId(lethbridgePolygon, TimeFrame.ProjectionPeriod);


            var months = new List<string>()
            {
                "January",
                "February",
                "March",
                "April",
                "May",
                "June",
                "July",
                "August",
                "September",
                "October",
                "November",
                "December",
            };

            var lines = new List<string>();
            lines.Add(header);
            foreach (var month in months)
            {
                var newMonth = (double)newEvapotranspirationData.GetType().GetProperty(month).GetValue(newEvapotranspirationData);
                var oldMonth = (double)oldEvapotranspirationData.GetType().GetProperty(month).GetValue(oldEvapotranspirationData);
                var result = oldMonth - newMonth;
                lines.Add($"{month},{oldMonth},{newMonth},{result}");
                Assert.AreNotEqual(newMonth, oldMonth);
            }
            File.WriteAllLines($"{path}\\PETComparisonLethbridge1980-2010{extension}", lines);

        }

        //want to make sure our SLC daily data is pretty close to the NASA data
        [TestMethod]
        [Ignore]
        public void CompareDailyTemperatureFromNasaVsSLC()
        {
            var lethbridgeLong = -112.833333;
            var lethbridgeLat = 49.7;

            var slcAndNasaTuple = GetDailyClimateDataForComparisonSlcNasa(lethbridgeLong, lethbridgeLat);
            var slcLethbridge1990DailyData = slcAndNasaTuple.Item1;
            var nasaLethbridge1990DailyData = slcAndNasaTuple.Item2;

            var headerline = "Julian Day,SLC,NASA,Difference";
            var lines = new List<string>();
            lines.Add(headerline);
            for (int i = 1; i <= 365; i++)
            {
                double slcMeanTemp = slcLethbridge1990DailyData[i - 1].MeanDailyAirTemperature;
                double nasaMeanTemp = nasaLethbridge1990DailyData[i - 1].MeanDailyAirTemperature;
                double difference = slcMeanTemp - nasaMeanTemp;
                lines.Add($"{i},{slcMeanTemp},{nasaMeanTemp},{difference}");
            }
            File.WriteAllLines(path + $"\\Compare_NASA_SLC_Temperature_Daily_Data{extension}", lines);
        }

        [TestMethod]
        [Ignore]
        public void CompareDailyPrecipFromNasaVsSLC()
        {
            var lethbridgeLong = -112.833333;
            var lethbridgeLat = 49.7;

            var slcAndNasaTuple = GetDailyClimateDataForComparisonSlcNasa(lethbridgeLong, lethbridgeLat);
            var slcLethbridge1990DailyData = slcAndNasaTuple.Item1;
            var nasaLethbridge1990DailyData = slcAndNasaTuple.Item2;

            var headerline = "Julian Day,SLC,NASA,Difference";
            var lines = new List<string>();
            lines.Add(headerline);
            for (int i = 1; i <= 365; i++)
            {
                double slcMeanPrecip = slcLethbridge1990DailyData[i - 1].MeanDailyPrecipitation;
                double nasaMeanPrecip = nasaLethbridge1990DailyData[i - 1].MeanDailyPrecipitation;
                double difference = slcMeanPrecip - nasaMeanPrecip;
                lines.Add($"{i},{slcMeanPrecip},{nasaMeanPrecip},{difference}");
            }
            File.WriteAllLines(path + $"\\Compare_NASA_SLC_Precipitation_Daily_Data{extension}", lines);
        }

        [TestMethod]
        [Ignore]
        public void CompareDailyPETFromNasaVsSLC()
        {
            var lethbridgeLong = -112.833333;
            var lethbridgeLat = 49.7;

            var slcAndNasaTuple = GetDailyClimateDataForComparisonSlcNasa(lethbridgeLong, lethbridgeLat);
            var slcLethbridge1990DailyData = slcAndNasaTuple.Item1;
            var nasaLethbridge1990DailyData = slcAndNasaTuple.Item2;

            var headerline = "Julian Day,SLC,NASA,Difference";
            var lines = new List<string>();
            lines.Add(headerline);
            for (int i = 1; i <= 365; i++)
            {
                double slcMeanPET = slcLethbridge1990DailyData[i - 1].MeanDailyPET;
                double nasaMeanPET = nasaLethbridge1990DailyData[i - 1].MeanDailyPET;
                double difference = slcMeanPET - nasaMeanPET;
                lines.Add($"{i},{slcMeanPET},{nasaMeanPET},{difference}");
            }
            File.WriteAllLines(path + $"\\Compare_NASA_SLC_PET_Daily_Data{extension}", lines);
        }

        private Tuple<List<DailyClimateData>, List<DailyClimateData>> GetDailyClimateDataForComparisonSlcNasa(double lethbridgeLong, double lethbridgeLat)
        {
            var nasaClimateProvider = new NasaClimateProvider();
            List<DailyClimateData> nasaLethbridge1990DailyData, slcLethbridge1990DailyData;

            //all the data from 1985 to today
            var nasaLethbridgeDailyData = nasaClimateProvider.GetCustomClimateData(lethbridgeLat, lethbridgeLong);

            //lethbridge nasa data for 1990
            nasaLethbridge1990DailyData = nasaLethbridgeDailyData.Where(x => x.Year == 1990).ToList();
            var lethbridgeSLCPath = @"C:\Users\bigbe\Holos\slcDaily\roland_2\793011.csv";

            //lethbridge polygon slc data for 1950 to 2017
            var slcLethbridgeDailyData = MakeDailyClimateData(lethbridgeSLCPath);

            //lethbridge slc data for 1990
            slcLethbridge1990DailyData = slcLethbridgeDailyData.Where(x => x.Year == 1990).ToList();

            var slcAndNasaTuple = new Tuple<List<DailyClimateData>, List<DailyClimateData>>(slcLethbridge1990DailyData, nasaLethbridge1990DailyData);

            return slcAndNasaTuple;

        }

        private List<DailyClimateData> MakeDailyClimateData(string path)
        {
            var fileLines = File.ReadAllLines(path);
            List<DailyClimateData> listOfDailyClimateData = new List<DailyClimateData>();
            foreach (var line in fileLines.Skip(1))
            {
                var splitLine = line.Split(',');
                var dailyClimateData = new DailyClimateData()
                {
                    Year = int.Parse(splitLine[0]),
                    JulianDay = int.Parse(splitLine[1]),
                    MeanDailyAirTemperature = double.Parse(splitLine[2]),
                    MeanDailyPrecipitation = double.Parse(splitLine[3]),
                    MeanDailyPET = double.Parse(splitLine[4]),
                };
                listOfDailyClimateData.Add(dailyClimateData);
            }
            return listOfDailyClimateData;
        }
        #endregion
    }
}