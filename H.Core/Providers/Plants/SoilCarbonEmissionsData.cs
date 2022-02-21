namespace H.Core.Providers.Plants
{
    public class SoilCarbonEmissionsData
    {
        public double MaximumCarbonProducedFromIntensiveToReducedTillage { get; set; }
        public double RateConstantFromIntensiveToReducedTillage { get; set; }
        public double MaximumCarbonProducedFromReducedToNoTillage { get; set; }
        public double RateConstantFromReducedToNoTillage { get; set; }
        public double MaximumCarbonProducedFromIntensiveToNoTillage { get; set; }
        public double RateConstantFromIntensiveToNoTillage { get; set; }
        public double MaximumCarbonProducedFromReducedToIntensiveTillage { get; set; }
        public double RateConstantFromReducedToIntensiveTillage { get; set; }
        public double MaximumCarbonProducedFromNoToReducedTillage { get; set; }
        public double RateConstantFromNoToReducedTillage { get; set; }
        public double MaximumCarbonProducedFromNoToIntensiveTillage { get; set; }
        public double RateConstantFromNoToIntensiveTillage { get; set; }
        public double MaximumCarbonProducedFromFallowToContinuousCropping { get; set; }
        public double RateConstantFromFallowToContinuousCropping { get; set; }
        public double MaximumCarbonProducedFromContinuousToFallowCropping { get; set; }
        public double RateConstantFromContinuousToFallowCropping { get; set; }
        public double MaximumCarbonProducedFromIncreaseInPerennialCropArea { get; set; }
        public double RateConstantFromIncreaseInPerennialCropArea { get; set; }
        public double MaximumCarbonProducedFromDecreaseInPerannialCropArea { get; set; }
        public double RateConstantFromDecreaseInPerennialCropArea { get; set; }
    }
}