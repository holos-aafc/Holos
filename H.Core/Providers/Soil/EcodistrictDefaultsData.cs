using H.Core.Enumerations;

namespace H.Core.Providers.Soil
{
    public class EcodistrictDefaultsData
    {
        public int EcodistrictId { get; set; }
        public Ecozone Ecozone { get; set; }
        public Province Province { get; set; }
        public int PMayToOct { get; set; }
        public int PEMayToOct { get; set; }
        public double FTopo { get; set; }
        public SoilFunctionalCategory SoilFunctionalCategory { get; set; }
        public SoilTexture SoilTexture { get; set; }
    }
}