using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace H.Core.Test.Providers.Climate
{
    [TestClass]
    public class ClimateDataTest
    {

        [TestMethod]
        public void GetTotalPrecipitationForYearReturnsTotalOfNormalsWhenNoDailyDataExists()
        {
            var climateData = new ClimateData();
            climateData.PrecipitationData.January = 10;
            climateData.PrecipitationData.February = 40;

            var result = climateData.GetTotalPrecipitationForYear(2020);

            Assert.AreEqual(50, result);
        }

        [TestMethod]
        public void GetTotalEvapotranspirationForYearReturnsTotalOfNormalsWhenNoDailyDataExists()
        {
            var climateData = new ClimateData();
            climateData.EvapotranspirationData.January = 10;
            climateData.EvapotranspirationData.February = 40;

            var result = climateData.GetTotalEvapotranspirationForYear(2020);

            Assert.AreEqual(50, result);
        }

        [TestMethod]
        public void ProportionOfPrecipitationFallingInMayThroughSeptemberReturnsProportionUsingDailyValues()
        {
            var climateData = new ClimateData();
            climateData.DailyClimateData.Add(new DailyClimateData() { MeanDailyPrecipitation = 10, Year = 2020, JulianDay = 122});
            climateData.DailyClimateData.Add(new DailyClimateData() { MeanDailyPrecipitation = 20, Year = 2020, JulianDay = 123,});
            climateData.DailyClimateData.Add(new DailyClimateData() { MeanDailyPrecipitation = 50, Year = 2020, JulianDay = 19});

            var result = climateData.ProportionOfPrecipitationFallingInMayThroughSeptember(2020);

            Assert.AreEqual((10.0 + 20.0) / 80.0, result);
        }

        [TestMethod]
        public void ProportionOfPrecipitationFallingInMayThroughSeptemberReturnsProportionUsingNormals()
        {
            var climateData = new ClimateData();
            climateData.PrecipitationData.May = 10;
            climateData.PrecipitationData.June = 20;
            climateData.PrecipitationData.December = 50;

            var result = climateData.ProportionOfPrecipitationFallingInMayThroughSeptember(2020);

            Assert.AreEqual((10.0 + 20.0)/80.0, result);
        }

        [TestMethod]
        public void GetAverageTemperatureForMonthAndYear()
        {
            var climateData = new ClimateData();
            
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 1) , Year = 2020, MeanDailyAirTemperature = 10, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 2), Year = 2020 , MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 2, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 3, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 4, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 5, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 6, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 7, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 8, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 9, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 10, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 11, 2), Year = 2020, MeanDailyAirTemperature = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 12, 2), Year = 2020, MeanDailyAirTemperature = 20, });


            var result = climateData.GetAverageTemperatureForMonthAndYear(2020, Enumerations.Months.January);

            Assert.AreEqual((10 + 20) / 2, result);
        }

        [TestMethod]
        public void GetMonthlyTemperaturesForYear()
        {
            var climateData = new ClimateData();

            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 1), Year = 2020, MeanDailyAirTemperature = 10, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 2), Year = 2020, MeanDailyAirTemperature = 20, });            
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 2, 1), Year = 2020, MeanDailyAirTemperature = 30 });

            var result = climateData.GetMonthlyTemperaturesForYear(2020);

            Assert.AreEqual(2, result.Values.Where(x => x > 0).Count());
        }

        [TestMethod]
        public void GetTotalPrecipitationForMonthAndYear()
        {
            var climateData = new ClimateData();

            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 1), Year = 2020, MeanDailyPrecipitation = 10, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 2), Year = 2020, MeanDailyPrecipitation = 20, });

            // Has a different month and so should not be included in total
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 2, 1), Year = 2020, MeanDailyPrecipitation = 30 });

            var result = climateData.GetTotalPrecipitationForMonthAndYear(2020, Enumerations.Months.January);

            Assert.AreEqual(10 + 20, result);
        }

        [TestMethod]
        public void GetMonthlyPrecipitationsForYear()
        {
            var climateData = new ClimateData();

            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 1), Year = 2020, MeanDailyPrecipitation = 10, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 2), Year = 2020, MeanDailyPrecipitation = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 2, 1), Year = 2020, MeanDailyPrecipitation = 30 });

            var result = climateData.GetMonthlyPrecipitationsForYear(2020);

            Assert.AreEqual(2, result.Values.Where(x => x > 0).Count());
        }

        [TestMethod]
        public void GetTotalEvapotranspirationsForMonthAndYear()
        {
            var climateData = new ClimateData();

            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 1), Year = 2020, MeanDailyPET = 10, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 2), Year = 2020, MeanDailyPET = 20, });

            // Has a different month and so should not be included in total
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 2, 1), Year = 2020, MeanDailyPET = 30 });

            var result = climateData.GetTotalEvapotranspirationForMonthAndYear(2020, Enumerations.Months.January);

            Assert.AreEqual(10 + 20, result);
        }

        [TestMethod]
        public void GetMonthlyEvapostranspirationsForYear()
        {
            var climateData = new ClimateData();

            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 1), Year = 2020, MeanDailyPET = 10, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 2), Year = 2020, MeanDailyPET = 20, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 2, 1), Year = 2020, MeanDailyPET = 30 });

            var result = climateData.GetMonthlyEvapotranspirationsForYear(2020);

            Assert.AreEqual(2, result.Values.Where(x => x > 0).Count());
        }
    }
}