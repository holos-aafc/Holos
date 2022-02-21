using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class LumCMaxAndKValuesForFallowPracticeChangeData
    {
        public Ecozone Ecozone { get; set; }
        public SoilTexture SoilTexture { get; set; }
        public FallowPracticeChangeType FallowPracticeChangeType { get; set; }
        public double LumCMax { get; set; }
        public double kValue { get; set; }

    }
}
