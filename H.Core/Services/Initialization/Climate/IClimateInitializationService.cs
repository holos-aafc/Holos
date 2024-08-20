using H.Core.Models;

namespace H.Core.Services.Initialization.Climate
{
    public interface IClimateInitializationService
    {
        void InitializeClimate(Farm farm);
        void InitializeClimate(Farm farm, int startYear, int endYear);
    }
}