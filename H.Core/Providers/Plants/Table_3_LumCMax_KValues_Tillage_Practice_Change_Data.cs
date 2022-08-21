using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class Table_3_LumCMax_KValues_Tillage_Practice_Change_Data
    {
        public Ecozone Ecozone { get; set; }
        public SoilTexture SoilTexture { get; set; }
        public TillagePracticeChangeType TillagePracticeChangeType { get; set; }
        public double LumCMax { get; set; }
        public double kValue { get; set; }
    }
}
