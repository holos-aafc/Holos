using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class TillageRatioFactosForDirectSoilNitrousOxideEmissionsData
    {
        public TillageType TillageType{ get; set; }
        public Province Province { get; set; }
        public double RatioFactor { get; set; }
    }
}