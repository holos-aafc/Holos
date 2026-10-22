using H.Core.Models;
using H.Core.Providers.Climate;
using H.Core.Services.Initialization.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Core.Test.Services.Initialization
{
    [TestClass]
    public class ClimateInitializationServiceTest : UnitTestBase
    {
        #region Fields

        private ClimateInitializationService _sut;

        #endregion

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new ClimateInitializationService();
        }

        #endregion

        #region Tests

        /// <summary>
        /// A farm's daily records are added to by InitializeClimate rather than replaced, so a replacement that did not
        /// clear them first would leave two of every day - and twice the rainfall, which looks exactly like the inflated
        /// data the replacement exists to remove.
        /// </summary>
        [TestMethod]
        public void ReplaceClimateReplacesTheDailyRecordsRatherThanAddingToThem()
        {
            var farm = new Farm();
            farm.ClimateData.DailyClimateData.Add(new DailyClimateData() { Year = 1999, JulianDay = 1, Date = new DateTime(1999, 1, 1) });

            var replacement = BuildYears(2020, 2021, precipitation: 1.5);

            var replaced = _sut.ReplaceClimate(farm, replacement);

            Assert.IsTrue(replaced);
            Assert.AreEqual(replacement.Count, farm.ClimateData.DailyClimateData.Count,
                "the old records were kept alongside the new ones");
            Assert.IsFalse(farm.ClimateData.DailyClimateData.Any(x => x.Year == 1999), "a record from the old climate survived");
        }

        /// <summary>
        /// The normals are recalculated from the new series, not left from the old one.
        /// </summary>
        [TestMethod]
        public void ReplaceClimateRecalculatesTheNormalsFromTheNewSeries()
        {
            var farm = new Farm();

            _sut.ReplaceClimate(farm, BuildYears(2020, 2021, precipitation: 24.0));
            var inflatedJanuary = farm.ClimateData.PrecipitationData.January;

            _sut.ReplaceClimate(farm, BuildYears(2020, 2021, precipitation: 1.0));
            var correctedJanuary = farm.ClimateData.PrecipitationData.January;

            Assert.AreEqual(inflatedJanuary / 24.0, correctedJanuary, 0.0001,
                "the normals did not follow the replacement series");
        }

        /// <summary>
        /// A download that fails comes back empty. Clearing the farm before finding that out would leave it with no
        /// climate at all, so nothing is touched.
        /// </summary>
        [TestMethod]
        public void ReplaceClimateWithNothingLeavesTheFarmAsItWas()
        {
            var farm = new Farm();
            var original = BuildYears(2020, 2021, precipitation: 1.0);
            _sut.ReplaceClimate(farm, original);

            var januaryBefore = farm.ClimateData.PrecipitationData.January;

            Assert.IsFalse(_sut.ReplaceClimate(farm, new List<DailyClimateData>()));
            Assert.IsFalse(_sut.ReplaceClimate(farm, null));

            Assert.AreEqual(original.Count, farm.ClimateData.DailyClimateData.Count, "a failed download emptied the farm's climate");
            Assert.AreEqual(januaryBefore, farm.ClimateData.PrecipitationData.January, 0.0001);
        }

        /// <summary>
        /// The farm's yearly figures - what the N2O and carbon calculations read for each year - follow the replacement
        /// for every year, not only a partial year that falls back to the normals. Each year is read before the
        /// replacement, as it would have been when the farm's results were first shown.
        /// </summary>
        [TestMethod]
        public void ReplaceClimateReplacesTheYearlyFiguresForEveryYear()
        {
            var farm = new Farm();
            _sut.ReplaceClimate(farm, BuildYears(2019, 2021, precipitation: 24.0));

            var inflated = new[] {2019, 2020, 2021}.ToDictionary(year => year, year => farm.GetGrowingSeasonPrecipitation(year));

            _sut.ReplaceClimate(farm, BuildYears(2019, 2021, precipitation: 1.0));

            foreach (var year in inflated.Keys)
            {
                Assert.AreEqual(inflated[year] / 24.0, farm.GetGrowingSeasonPrecipitation(year), 0.0001,
                    $"{year} still reports the growing season precipitation of the climate that was replaced");
                Assert.AreEqual(DateTime.IsLeapYear(year) ? 366 : 365, farm.ClimateData.GetTotalPrecipitationForYear(year), 0.0001,
                    $"{year}'s annual precipitation is not the new series'");
            }
        }

        [TestMethod]
        public void ReplaceClimateWithNoFarmReportsNothingReplaced()
        {
            Assert.IsFalse(_sut.ReplaceClimate(null, BuildYears(2020, 2020, precipitation: 1.0)));
        }

        #endregion

        #region Helpers

        private static List<DailyClimateData> BuildYears(int firstYear, int lastYear, double precipitation)
        {
            var result = new List<DailyClimateData>();
            for (var year = firstYear; year <= lastYear; year++)
            {
                var days = DateTime.IsLeapYear(year) ? 366 : 365;
                for (var julianDay = 1; julianDay <= days; julianDay++)
                {
                    result.Add(new DailyClimateData()
                    {
                        Year = year,
                        JulianDay = julianDay,
                        Date = new DateTime(year, 1, 1).AddDays(julianDay - 1),
                        MeanDailyPrecipitation = precipitation,
                        MeanDailyAirTemperature = 5,
                        MeanDailyPET = 1,
                    });
                }
            }

            return result;
        }

        #endregion
    }
}
