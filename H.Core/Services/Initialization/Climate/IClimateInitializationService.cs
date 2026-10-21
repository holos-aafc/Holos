using System.Collections.Generic;
using H.Core.Models;
using H.Core.Providers.Climate;

namespace H.Core.Services.Initialization.Climate
{
    public interface IClimateInitializationService
    {
        void InitializeClimate(Farm farm);

        /// <summary>
        /// Replaces a farm's climate with a fresh download, discarding what the provider has kept for its location.
        /// Returns false, leaving the farm as it was, when the download came back empty.
        /// </summary>
        bool ReDownloadClimate(Farm farm);

        /// <summary>
        /// A fresh copy of the climate for a farm's location, without changing the farm. Empty when the service could
        /// not be reached.
        /// </summary>
        List<DailyClimateData> DownloadClimateData(Farm farm);

        /// <summary>
        /// Replaces a farm's daily climate with the series given and recalculates its normals. Returns false, leaving
        /// the farm as it was, when the series is empty.
        /// </summary>
        bool ReplaceClimate(Farm farm, List<DailyClimateData> dailyClimateData);
        void InitializeClimate(Farm farm, int startYear, int endYear);
        void InitializeClimate(Farm farm, IEnumerable<DailyClimateData> dailyClimateData);
        void SetClimateNormals(Farm farm, IEnumerable<DailyClimateData> climateForPeriod);
    }
}