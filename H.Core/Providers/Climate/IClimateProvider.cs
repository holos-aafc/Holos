using H.Core.Models;

namespace H.Core.Providers.Climate
{
    public interface IClimateProvider
    {
        void OutputDailyClimateData(Farm farm, string outputPath);
    }
}