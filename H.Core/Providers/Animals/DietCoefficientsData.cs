using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DietCoefficientsData
    {
        public AnimalType AnimalType { get; set; }
        public DietType DietType { get; set; }
        public double PercentDigestibleEnergyInFeed { get; set; }
        public double CrudeProteinContent { get; set; }
        public double MethaneConversionFactor { get; set; }
    }
}