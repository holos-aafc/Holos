using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class FertilizerApplicationRatesData
    {
        public SoilFunctionalCategory SoilFunctionalCategory { get; set; }
        public CropType CropType { get; set; }
        public Province Province { get; set; }
        public double NitrogenApplicationRate { get; set; }
        public double PhosphorusApplicationRate { get; set; }
    }
}