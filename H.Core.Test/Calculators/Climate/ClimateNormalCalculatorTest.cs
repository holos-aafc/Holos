using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Calculators.Climate
{
    [TestClass]
    public class ClimateNormalCalculatorTest
    {
        #region Fields

        private ClimateNormalCalculator _calculator;
        private NasaClimateProvider _nasaClimateProvider;
        private List<DailyClimateData> _dailyClimateData;

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
            _calculator = new ClimateNormalCalculator();
            _nasaClimateProvider = new NasaClimateProvider();
            _dailyClimateData = _nasaClimateProvider.GetCustomClimateData(53.9125034103343, -114.08203125);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetNormalsForAllTwelveMonthsReturnsCorrectValue()
        {
            // _dailyClimateData will be an empty list if an exception was thrown (i.e. 502 Gateway Error) or etc. during the NASA Api Call
            // return early in these situations to prevent unhandled exceptions or errors that lead to a failed test
            if (!_dailyClimateData.Any())
            {
                return; 
            }

            var result = _calculator.GetNormalsForAllTwelveMonths(_dailyClimateData, TimeFrame.TwoThousandToCurrent);

            var temperatureNormals = result[MonthlyNormalTypes.temperature];
            var januaryTemperatureNormal = temperatureNormals[Months.January];

            var precipNorms = result[MonthlyNormalTypes.precipitation];
            var januaryPrecipNormal = precipNorms[Months.January];

            var petNorms = result[MonthlyNormalTypes.evapotranspiration];
            var januaryPETNormal = petNorms[Months.January];

            Assert.AreEqual(-13.1799, Math.Round(januaryTemperatureNormal, 4));
            Assert.AreEqual(24.8168, Math.Round(januaryPrecipNormal, 4));
            Assert.AreEqual(0.1474, Math.Round(januaryPETNormal, 4));
        }

        [TestMethod]
        public void GetTemperatureDataByDailyValueNotReturnNull()
        {
            // _dailyClimateData will be an empty list if an exception was thrown (i.e. 502 Gateway Error) or etc. during the NASA Api Call
            // return early in these situations to prevent unhandled exceptions or errors that lead to a failed test
            if (!_dailyClimateData.Any())
            {
                return;
            }

            var result = _calculator.GetTemperatureDataByDailyValues(_dailyClimateData, Enumerations.TimeFrame.TwoThousandToCurrent);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetPrecipitationDataByDailyValueNotReturnNull()
        {
            // _dailyClimateData will be an empty list if an exception was thrown (i.e. 502 Gateway Error) or etc. during the NASA Api Call
            // return early in these situations to prevent unhandled exceptions or errors that lead to a failed test
            if (!_dailyClimateData.Any())
            {
                return;
            }

            var result = _calculator.GetPrecipitationDataByDailyValues(_dailyClimateData, Enumerations.TimeFrame.TwoThousandToCurrent);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Ontario typically has > 500 mm in the growing season
        /// </summary>
        [TestMethod]
        public void GetGrowingSeasonPrecipitationNormalsForOntario()
        {
            const double latitude = 43.325;
            const double longitude = -80.564;

            var customClimateData = _nasaClimateProvider.GetCustomClimateData(latitude, longitude);

            // customClimateData will be an empty list if an exception was thrown (i.e. 502 Gateway Error) or etc. during the NASA Api Call
            // return early in these situations to prevent unhandled exceptions or errors that lead to a failed test
            if (!customClimateData.Any())
            {
                return;
            }

            var normalsFor1990To2000 = _calculator.GetPrecipitationDataByDailyValues(customClimateData, TimeFrame.TwoThousandToCurrent);

            var growingSeasonPrecipitation1990To2000 = normalsFor1990To2000.CalculateGrowingSeasonPrecipitation();
            var growingSeasonUsingDailyValues = customClimateData.Where(x => x.Year == 2019 && x.JulianDay >= 122 && x.JulianDay <= 305).Select(x => x.MeanDailyPrecipitation).Sum();

            var normalsFor1980To1990 = _calculator.GetPrecipitationDataByDailyValues(customClimateData, TimeFrame.TwoThousandToCurrent);

            var growingSeasonPrecipitation1980To1990 = normalsFor1990To2000.CalculateGrowingSeasonPrecipitation();

            Assert.IsTrue(growingSeasonPrecipitation1980To1990 > 0);
        }

        #endregion
    }
}
