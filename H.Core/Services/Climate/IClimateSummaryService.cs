using System.Collections.Generic;
using H.Core.Models;

namespace H.Core.Services.Climate
{
    public interface IClimateSummaryService
    {
        /// <summary>
        /// The farm's daily climate totalled by year and month, each year set against the long-term normal
        /// precipitation for the farm's soil polygon. Ordered by year.
        /// </summary>
        List<ClimateYearSummary> Summarize(Farm farm);

        /// <summary>
        /// The long-term normal annual precipitation for the farm's soil polygon, or null when there is none. (mm)
        /// </summary>
        double? GetReferencePrecipitation(Farm farm);
    }
}
