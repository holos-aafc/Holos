using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public void GetAverageTemperatureForMonthAndYearReturnsNormalsWhenNasaDataReturns999Values()
        {
            var climateData = new ClimateData();

            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 1), Year = 2020, MeanDailyAirTemperature = -999, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 2), Year = 2020, MeanDailyAirTemperature = 20, });
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
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 12, 2), Year = 2020, MeanDailyAirTemperature = 20 });

            climateData.TemperatureData.January = 50;

            var result = climateData.GetAverageTemperatureForMonthAndYear(2020, Enumerations.Months.January);

            Assert.AreEqual(50, result);
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
        public void GetTotalPrecipitationForMonthAndYearReturnsNormalsWhenNasaReturnsNegative999()
        {
            var climateData = new ClimateData();

            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 1), Year = 2020, MeanDailyPrecipitation = -999, });
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 1, 2), Year = 2020, MeanDailyPrecipitation = 20, });

            // Has a different month and so should not be included in total
            climateData.DailyClimateData.Add(new DailyClimateData() { Date = new System.DateTime(2020, 2, 1), Year = 2020, MeanDailyPrecipitation = 30 });

            climateData.PrecipitationData.January = 777;

            var result = climateData.GetTotalPrecipitationForMonthAndYear(2020, Enumerations.Months.January);

            Assert.AreEqual(777, result);
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

            var result = climateData.GetMonthlyEvapotranspirationForYear(2020);

            Assert.AreEqual(2, result.Values.Where(x => x > 0).Count());
        }
        /// <summary>
        /// Re-downloading a farm's climate clears the daily records and adds the new ones. Every per-year figure has to
        /// come from the new records, including a year that had already been read - a figure kept from the old records
        /// leaves every complete year on the climate being replaced, while a partial year, which falls back to the
        /// recalculated normals, appears fixed.
        /// </summary>
        [TestMethod]
        public void ClearingAndReplacingTheDailyRecordsReplacesEveryFigureForTheYear()
        {
            var climateData = new ClimateData();
            foreach (var day in BuildYear(2020, precipitation: 24, evapotranspiration: 2))
            {
                climateData.DailyClimateData.Add(day);
            }

            var before = ReadEveryFigure(climateData, 2020);

            climateData.DailyClimateData.Clear();
            foreach (var day in BuildYear(2020, precipitation: 1, evapotranspiration: 2))
            {
                climateData.DailyClimateData.Add(day);
            }

            var after = ReadEveryFigure(climateData, 2020);

            foreach (var figure in new[] {"growing season precipitation", "annual precipitation", "January precipitation", "precipitation on 1 July"})
            {
                Assert.AreEqual(before[figure] / 24.0, after[figure], 1e-9, figure + " still comes from the records that were replaced");
            }

            foreach (var figure in new[] {"growing season evapotranspiration", "annual evapotranspiration", "January evapotranspiration"})
            {
                Assert.AreEqual(before[figure], after[figure], 1e-9, figure + " was counted from both sets of records");
            }
        }

        [TestMethod]
        public void ReplacingTheDailyRecordsCollectionIndexesTheNewRecords()
        {
            var climateData = new ClimateData();

            climateData.DailyClimateData = new ObservableCollection<DailyClimateData>(BuildYear(2020, precipitation: 1, evapotranspiration: 2));

            Assert.AreEqual(366, climateData.GetTotalPrecipitationForYear(2020), 1e-9, "the new records were not used");
            Assert.AreEqual(1, climateData.GetMeanPrecipitationForDay(new DateTime(2020, 7, 1)), 1e-9);

            // Records added afterwards are indexed too.
            climateData.DailyClimateData.Clear();
            foreach (var day in BuildYear(2020, precipitation: 2, evapotranspiration: 2))
            {
                climateData.DailyClimateData.Add(day);
            }

            Assert.AreEqual(732, climateData.GetTotalPrecipitationForYear(2020), 1e-9, "records added to the new collection were not used");
        }

        private static Dictionary<string, double> ReadEveryFigure(ClimateData climateData, int year)
        {
            return new Dictionary<string, double>()
            {
                {"growing season precipitation", climateData.GetGrowingSeasonPrecipitation(year)},
                {"annual precipitation", climateData.GetTotalPrecipitationForYear(year)},
                {"January precipitation", climateData.GetTotalPrecipitationForMonthAndYear(year, Enumerations.Months.January)},
                {"precipitation on 1 July", climateData.GetMeanPrecipitationForDay(new DateTime(year, 7, 1))},
                {"growing season evapotranspiration", climateData.GetGrowingSeasonEvapotranspiration(year)},
                {"annual evapotranspiration", climateData.GetTotalEvapotranspirationForYear(year)},
                {"January evapotranspiration", climateData.GetTotalEvapotranspirationForMonthAndYear(year, Enumerations.Months.January)},
            };
        }

        private static List<DailyClimateData> BuildYear(int year, double precipitation, double evapotranspiration)
        {
            var days = DateTime.IsLeapYear(year) ? 366 : 365;

            return Enumerable.Range(1, days).Select(julianDay => new DailyClimateData()
            {
                Year = year,
                JulianDay = julianDay,
                Date = new DateTime(year, 1, 1).AddDays(julianDay - 1),
                MeanDailyPrecipitation = precipitation,
                MeanDailyPET = evapotranspiration,
                MeanDailyAirTemperature = 5,
            }).ToList();
        }
    }
}