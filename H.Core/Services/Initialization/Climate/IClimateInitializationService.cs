using System.Collections.Generic;
using H.Core.Models;
using H.Core.Providers.Climate;

namespace H.Core.Services.Initialization.Climate
{
    public interface IClimateInitializationService
    {
        void InitializeClimate(Farm farm);
        void InitializeClimate(Farm farm, int startYear, int endYear);
        void InitializeClimate(Farm farm, IEnumerable<DailyClimateData> dailyClimateData);
        void SetClimateNormals(Farm farm, IEnumerable<DailyClimateData> climateForPeriod);
    }
}