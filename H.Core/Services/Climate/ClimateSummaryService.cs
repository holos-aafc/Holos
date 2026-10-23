using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Providers.Precipitation;

namespace H.Core.Services.Climate
{
    /// <summary>
    /// Totals a farm's daily climate by year and month so the user can check it against what they know of their
    /// location, and flags years whose precipitation is implausible.
    ///
    /// The reference is the long-term normal for the farm's soil polygon from Holos's own Soil Landscapes of Canada
    /// climate data. It is held locally and does not come from the service that supplied the daily records, so a fault
    /// in that service - NASA POWER returning precipitation at 24 times its value (holos-aafc/Holos#499) - shows up
    /// against it rather than in it.
    /// </summary>
    public class ClimateSummaryService : IClimateSummaryService
    {
        #region Fields

        /// <summary>
        /// A year at this multiple of the normal or more is flagged. Contaminated years run at about 24 times; a real wet
        /// year rarely reaches 2.
        /// </summary>
        public const double ImplausibleRatio = 3;

        /// <summary>
        /// The normal the years are set against: the standard 30-year period the polygon data covers.
        /// </summary>
        public const TimeFrame ReferenceTimeFrame = TimeFrame.NineteenEightyToTwoThousandTen;

        /// <summary>
        /// NASA POWER's marker for an unknown value.
        /// </summary>
        private const double UnknownValue = -999;

        private readonly ISlcClimateProvider _slcClimateProvider;

        #endregion

        #region Constructors

        public ClimateSummaryService(ISlcClimateProvider slcClimateProvider)
        {
            _slcClimateProvider = slcClimateProvider ?? throw new ArgumentNullException(nameof(slcClimateProvider));
        }

        #endregion

        #region Public Methods

        public double? GetReferencePrecipitation(Farm farm)
        {
            var normals = this.GetReferenceNormals(farm);
            if (normals == null)
            {
                return null;
            }

            var annual = normals.GetTotalAnnualPrecipitation();

            return annual > 0 ? annual : (double?)null;
        }

        public List<ClimateYearSummary> Summarize(Farm farm)
        {
            var result = new List<ClimateYearSummary>();
            if (farm?.ClimateData?.DailyClimateData == null)
            {
                return result;
            }

            var normals = this.GetReferenceNormals(farm);
            var referenceAnnual = this.GetReferencePrecipitation(farm);

            foreach (var year in farm.ClimateData.DailyClimateData.GroupBy(day => day.Year).OrderBy(grouping => grouping.Key))
            {
                var days = year.ToList();
                var summary = new ClimateYearSummary()
                {
                    Year = year.Key,
                    Days = days.Count,
                    MissingDays = days.Count(IsMissing),
                    Precipitation = SumKnown(days, day => day.MeanDailyPrecipitation),
                    GrowingSeasonPrecipitation = SumKnown(
                        days.Where(day => day.JulianDay >= CoreConstants.GrowingSeasonJulianStartDay &&
                                          day.JulianDay <= CoreConstants.GrowingSeasonJulianEndDaySeptember),
                        day => day.MeanDailyPrecipitation),
                    Evapotranspiration = SumKnown(days, day => day.MeanDailyPET),
                    MeanTemperature = AverageKnown(days, day => day.MeanDailyAirTemperature),
                    ReferencePrecipitation = referenceAnnual,
                };

                if (referenceAnnual.HasValue && summary.Days > 0)
                {
                    // A partial year is scaled to a full one, so the current year can be judged before it ends.
                    var daysInYear = DateTime.IsLeapYear(summary.Year) ? 366.0 : 365.0;
                    var annualized = summary.Precipitation * daysInYear / summary.Days;

                    summary.RatioToReference = annualized / referenceAnnual.Value;
                    // The tolerance keeps a year summed to exactly the threshold from falling just short of it.
                    summary.IsImplausible = summary.RatioToReference >= ImplausibleRatio - 1e-9;
                }

                foreach (var month in days.GroupBy(day => day.Date.Month).OrderBy(grouping => grouping.Key))
                {
                    var monthDays = month.ToList();
                    var monthOfYear = (Months)month.Key;

                    summary.Months.Add(new ClimateMonthSummary()
                    {
                        Month = monthOfYear,
                        Days = monthDays.Count,
                        MissingDays = monthDays.Count(IsMissing),
                        Precipitation = SumKnown(monthDays, day => day.MeanDailyPrecipitation),
                        Evapotranspiration = SumKnown(monthDays, day => day.MeanDailyPET),
                        MeanTemperature = AverageKnown(monthDays, day => day.MeanDailyAirTemperature),
                        ReferencePrecipitation = normals != null && referenceAnnual.HasValue
                            ? normals.GetValueByMonth(monthOfYear)
                            : (double?)null,
                    });
                }

                result.Add(summary);
            }

            return result;
        }

        #endregion

        #region Private Methods

        private PrecipitationData GetReferenceNormals(Farm farm)
        {
            if (farm == null || farm.PolygonId <= 0 || _slcClimateProvider.HasDataForPolygonId(farm.PolygonId, ReferenceTimeFrame) == false)
            {
                return null;
            }

            return _slcClimateProvider.GetPrecipitationDataByPolygonId(farm.PolygonId, ReferenceTimeFrame);
        }

        private static bool IsUnknown(double value)
        {
            return value <= UnknownValue + 1;
        }

        /// <summary>
        /// Solar radiation is included because evapotranspiration is calculated from it: a day without it has no
        /// evapotranspiration either.
        /// </summary>
        private static bool IsMissing(DailyClimateData day)
        {
            return IsUnknown(day.MeanDailyPrecipitation) || IsUnknown(day.MeanDailyAirTemperature) || IsUnknown(day.SolarRadiation);
        }

        private static double SumKnown(IEnumerable<DailyClimateData> days, Func<DailyClimateData, double> value)
        {
            return days.Select(value).Where(x => IsUnknown(x) == false).Sum();
        }

        private static double AverageKnown(IEnumerable<DailyClimateData> days, Func<DailyClimateData, double> value)
        {
            var known = days.Select(value).Where(x => IsUnknown(x) == false).ToList();

            return known.Any() ? known.Average() : 0;
        }

        #endregion
    }
}
