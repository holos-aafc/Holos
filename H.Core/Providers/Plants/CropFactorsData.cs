using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class CropFactorsData
    {
        public CropType CropType { get; set; }
        public double MoistureContent { get; set; }
        public double AboveGroundResidueNitrogenConcentration { get; set; }
        public double BelowGroundResidueNitrogenConcentration { get; set; }
        public double YieldRatio { get; set; }
        public double AboveGroundResidueRatio { get; set; }
        public double BelowGroundResidueRatio { get; set; }
        public double Yield { get; set; }
    }
}