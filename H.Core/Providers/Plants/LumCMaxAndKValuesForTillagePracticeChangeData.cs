using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class LumCMaxAndKValuesForTillagePracticeChangeData
    {
        public Ecozone Ecozone { get; set; }
        public SoilTexture SoilTexture { get; set; }
        public TillagePracticeChangeType TillagePracticeChangeType { get; set; }
        public double LumCMax { get; set; }
        public double kValue { get; set; }
    }
}
