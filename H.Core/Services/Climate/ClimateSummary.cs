using System.Collections.Generic;
using H.Core.Enumerations;

namespace H.Core.Services.Climate
{
    /// <summary>
    /// One year of a farm's daily climate, totalled, and set against the long-term normal precipitation for the farm's
    /// soil polygon so that a year far outside it stands out.
    /// </summary>
    public class ClimateYearSummary
    {
        public int Year { get; set; }

        /// <summary>
        /// Days with a record for this year. Fewer than 365 for the current year and for a series that starts or ends
        /// part way through one.
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Days on which the source marked precipitation, temperature or solar radiation as unknown (-999). Unknown
        /// values are left out of the totals and averages.
        /// </summary>
        public int MissingDays { get; set; }

        /// <summary>(mm)</summary>
        public double Precipitation { get; set; }

        /// <summary>
        /// May to September. (mm)
        /// </summary>
        public double GrowingSeasonPrecipitation { get; set; }

        /// <summary>(mm)</summary>
        public double Evapotranspiration { get; set; }

        /// <summary>(°C)</summary>
        public double MeanTemperature { get; set; }

        /// <summary>
        /// The long-term normal annual precipitation for the farm's soil polygon, or null when there is none. (mm)
        /// </summary>
        public double? ReferencePrecipitation { get; set; }

        /// <summary>
        /// The year's precipitation as a multiple of the normal, scaled up to a full year for a partial one. Null
        /// without a normal.
        /// </summary>
        public double? RatioToReference { get; set; }

        /// <summary>
        /// True when the year's precipitation is at least <see cref="ClimateSummaryService.ImplausibleRatio"/> times
        /// the normal.
        /// </summary>
        public bool IsImplausible { get; set; }

        public List<ClimateMonthSummary> Months { get; set; } = new List<ClimateMonthSummary>();
    }

    /// <summary>
    /// One month of a farm's daily climate, totalled, beside the long-term normal precipitation for that month.
    /// </summary>
    public class ClimateMonthSummary
    {
        public Months Month { get; set; }

        public int Days { get; set; }

        public int MissingDays { get; set; }

        /// <summary>(mm)</summary>
        public double Precipitation { get; set; }

        /// <summary>(mm)</summary>
        public double Evapotranspiration { get; set; }

        /// <summary>(°C)</summary>
        public double MeanTemperature { get; set; }

        /// <summary>
        /// The long-term normal precipitation for this month for the farm's soil polygon, or null. (mm)
        /// </summary>
        public double? ReferencePrecipitation { get; set; }
    }
}
