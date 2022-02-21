using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class NitrogenApplicationRatesForSpringWheatStubbleAndFallowCropsData
    {
        public Province Province { get; set; }
        public SoilFunctionalCategory SoilFunctionalCategory { get; set; }
        public double NitrogenApplicationRateForWheatStubble { get; set; }
        public double NitrogenApplicationRateForFallow { get; set; }
        public double NitrogenMineralizedStubbleFallow { get; set; }
    }
}