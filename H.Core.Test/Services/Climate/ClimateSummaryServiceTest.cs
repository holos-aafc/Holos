using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Providers.Precipitation;
using H.Core.Services.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace H.Core.Test.Services.Climate
{
    /// <summary>
    /// The climate summary lets a user check their farm's climate against what they know of their location. Its one
    /// judgement - a year implausibly far above the polygon's normal - has to catch the 24-fold precipitation NASA POWER
    /// has been returning (holos-aafc/Holos#499) without flagging ordinary wet years.
    /// </summary>
    [TestClass]
    public class ClimateSummaryServiceTest
    {
        #region Fields

        private const int PolygonId = 851003;
        private const double NormalPerMonth = 40;

        private ClimateSummaryService _sut;

        #endregion

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            var normals = new PrecipitationData()
            {
                January = NormalPerMonth, February = NormalPerMonth, March = NormalPerMonth, April = NormalPerMonth,
                May = NormalPerMonth, June = NormalPerMonth, July = NormalPerMonth, August = NormalPerMonth,
                September = NormalPerMonth, October = NormalPerMonth, November = NormalPerMonth, December = NormalPerMonth,
            };

            var provider = new Mock<ISlcClimateProvider>();
            provider.Setup(x => x.HasDataForPolygonId(PolygonId, ClimateSummaryService.ReferenceTimeFrame)).Returns(true);
            provider.Setup(x => x.GetPrecipitationDataByPolygonId(PolygonId, ClimateSummaryService.ReferenceTimeFrame)).Returns(normals);

            _sut = new ClimateSummaryService(provider.Object);
        }

        #endregion

        #region Tests

        [TestMethod]
        public void AYearTwentyFourTimesTheNormalIsFlaggedAndAWetYearIsNot()
        {
            var farm = FarmWith(Year(2019, precipitation: 480.0 / 365 * 1.8), Year(2020, precipitation: 480.0 / 366 * 24));

            var summaries = _sut.Summarize(farm);

            var wet = summaries.Single(x => x.Year == 2019);
            Assert.AreEqual(480 * 1.8, wet.Precipitation, 1e-6);
            Assert.AreEqual(1.8, wet.RatioToReference.Value, 1e-6);
            Assert.IsFalse(wet.IsImplausible, "a year at 1.8 times the normal was flagged");

            var inflated = summaries.Single(x => x.Year == 2020);
            Assert.AreEqual(24, inflated.RatioToReference.Value, 1e-6);
            Assert.IsTrue(inflated.IsImplausible, "a year at 24 times the normal was not flagged");
        }

        [TestMethod]
        public void AYearAtExactlyThreeTimesTheNormalIsFlagged()
        {
            var summaries = _sut.Summarize(FarmWith(Year(2019, precipitation: 480.0 / 365 * 3)));

            Assert.IsTrue(summaries.Single().IsImplausible);
        }

        /// <summary>
        /// The current year is judged before it ends: its precipitation so far is scaled up to a full year.
        /// </summary>
        [TestMethod]
        public void APartialYearIsScaledToAFullOneBeforeItIsJudged()
        {
            var partial = Year(2026, precipitation: 480.0 / 365 * 24).Take(100).ToList();

            var summary = _sut.Summarize(FarmWith(partial)).Single();

            Assert.AreEqual(100, summary.Days);
            Assert.AreEqual(24, summary.RatioToReference.Value, 1e-6);
            Assert.IsTrue(summary.IsImplausible);
        }

        /// <summary>
        /// Unknown values (-999) are counted, and left out of the totals rather than subtracted from them. Missing
        /// solar radiation counts too: evapotranspiration is calculated from it.
        /// </summary>
        [TestMethod]
        public void UnknownValuesAreCountedAndLeftOutOfTheTotals()
        {
            var days = Year(2019, precipitation: 1);
            days[0].MeanDailyPrecipitation = -999;
            days[1].MeanDailyAirTemperature = -999;
            days[2].SolarRadiation = -999;

            var summary = _sut.Summarize(FarmWith(days)).Single();

            Assert.AreEqual(3, summary.MissingDays);
            Assert.AreEqual(364, summary.Precipitation, 1e-6);
            Assert.AreEqual(5, summary.MeanTemperature, 1e-6);
            Assert.AreEqual(3, summary.Months.Single(x => x.Month == Months.January).MissingDays);
        }

        [TestMethod]
        public void MonthsAreTotalledBesideTheirNormals()
        {
            var summary = _sut.Summarize(FarmWith(Year(2019, precipitation: 2))).Single();

            Assert.AreEqual(12, summary.Months.Count);

            var february = summary.Months.Single(x => x.Month == Months.February);
            Assert.AreEqual(28, february.Days);
            Assert.AreEqual(56, february.Precipitation, 1e-6);
            Assert.AreEqual(NormalPerMonth, february.ReferencePrecipitation.Value, 1e-6);

            // May to September, as the N2O calculation uses it.
            Assert.AreEqual(2 * (31 + 30 + 31 + 31 + 30), summary.GrowingSeasonPrecipitation, 1e-6);
        }

        /// <summary>
        /// Without a normal for the polygon there is nothing to judge against, so nothing is flagged.
        /// </summary>
        [TestMethod]
        public void WithoutANormalNothingIsFlagged()
        {
            var farm = FarmWith(Year(2020, precipitation: 480.0 / 366 * 24));
            farm.PolygonId = 1;

            var summary = _sut.Summarize(farm).Single();

            Assert.IsNull(summary.ReferencePrecipitation);
            Assert.IsNull(summary.RatioToReference);
            Assert.IsFalse(summary.IsImplausible);
            Assert.IsNull(summary.Months.First().ReferencePrecipitation);
        }

        #endregion

        #region Helpers

        private static Farm FarmWith(params List<DailyClimateData>[] years)
        {
            var farm = new Farm() { PolygonId = PolygonId };
            foreach (var day in years.SelectMany(x => x))
            {
                farm.ClimateData.DailyClimateData.Add(day);
            }

            return farm;
        }

        private static List<DailyClimateData> Year(int year, double precipitation)
        {
            var days = DateTime.IsLeapYear(year) ? 366 : 365;

            return Enumerable.Range(0, days).Select(index => new DailyClimateData()
            {
                Year = year,
                JulianDay = index + 1,
                Date = new DateTime(year, 1, 1).AddDays(index),
                MeanDailyPrecipitation = precipitation,
                MeanDailyAirTemperature = 5,
                MeanDailyPET = 2,
                SolarRadiation = 10,
            }).ToList();
        }

        #endregion
    }
}
