using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    public class SoilCarbonEmissionsProvider
    {
        #region Fields
        private List<SoilCarbonEmissionsData> _cache;
        #endregion

        #region Constructors
        public SoilCarbonEmissionsProvider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods
        public List<SoilCarbonEmissionsData> GetSoilCarbonEmissions()
        {
            return _cache;
        }
        #endregion

        #region Private Methods
        private List<SoilCarbonEmissionsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SoilCarbonEmissions;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<SoilCarbonEmissionsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new SoilCarbonEmissionsData();
                var maximumCarbonProducedFromIntensiveToReducedTillage = double.Parse(line[2], cultureInfo);
                var rateConstantFromIntensiveToReducedTillage = double.Parse(line[3], cultureInfo);
                var maximumCarbonProducedFromReducedToNoTillage = double.Parse(line[4], cultureInfo);
                var rateConstantFromReducedToNoTillage = double.Parse(line[5], cultureInfo);
                var maximumCarbonProducedFromIntensiveToNoTillage = double.Parse(line[6], cultureInfo);
                var rateConstantFromIntensiveToNoTillage = double.Parse(line[7], cultureInfo);
                var maximumCarbonProducedFromReducedToIntensiveTillage = double.Parse(line[8], cultureInfo);
                var rateConstantFromReducedToIntensiveTillage = double.Parse(line[9], cultureInfo);
                var maximumCarbonProducedFromNoToReducedTillage = double.Parse(line[10], cultureInfo);
                var rateConstantFromNoToReducedTillage = double.Parse(line[11], cultureInfo);
                var maximumCarbonProducedFromNoToIntensiveTillage = double.Parse(line[12], cultureInfo);
                var rateConstantFromNoToIntensiveTillage = double.Parse(line[13], cultureInfo);
                var maximumCarbonProducedFromFallowToContinuousCropping = double.Parse(line[14], cultureInfo);
                var rateConstantFromFallowToContinuousCropping = double.Parse(line[15], cultureInfo);
                var maximumCarbonProducedFromContinuousToFallowCropping = double.Parse(line[16], cultureInfo);
                var rateConstantFromContinuousToFallowCropping = double.Parse(line[17], cultureInfo);
                var maximumCarbonProducedFromIncreaseInPerennialCropArea = double.Parse(line[18], cultureInfo);
                var rateConstantFromIncreaseInPerennialCropArea = double.Parse(line[19], cultureInfo);
                var maximumCarbonProducedFromDecreaseInPerannialCropArea = double.Parse(line[20], cultureInfo);
                var rateConstantFromDecreaseInPerennialCropArea = double.Parse(line[21], cultureInfo);
                entry.MaximumCarbonProducedFromIntensiveToReducedTillage =
                    maximumCarbonProducedFromIntensiveToReducedTillage;
                entry.RateConstantFromIntensiveToReducedTillage = rateConstantFromIntensiveToReducedTillage;
                entry.MaximumCarbonProducedFromReducedToNoTillage = maximumCarbonProducedFromReducedToNoTillage;
                entry.RateConstantFromReducedToNoTillage = rateConstantFromReducedToNoTillage;
                entry.MaximumCarbonProducedFromIntensiveToNoTillage = maximumCarbonProducedFromIntensiveToNoTillage;
                entry.RateConstantFromIntensiveToNoTillage = rateConstantFromIntensiveToNoTillage;
                entry.MaximumCarbonProducedFromReducedToIntensiveTillage =
                    maximumCarbonProducedFromReducedToIntensiveTillage;
                entry.RateConstantFromReducedToIntensiveTillage = rateConstantFromReducedToIntensiveTillage;
                entry.MaximumCarbonProducedFromNoToReducedTillage = maximumCarbonProducedFromNoToReducedTillage;
                entry.RateConstantFromNoToReducedTillage = rateConstantFromNoToReducedTillage;
                entry.MaximumCarbonProducedFromNoToIntensiveTillage = maximumCarbonProducedFromNoToIntensiveTillage;
                entry.RateConstantFromNoToIntensiveTillage = rateConstantFromNoToIntensiveTillage;
                entry.MaximumCarbonProducedFromFallowToContinuousCropping =
                    maximumCarbonProducedFromFallowToContinuousCropping;
                entry.RateConstantFromFallowToContinuousCropping = rateConstantFromFallowToContinuousCropping;
                entry.MaximumCarbonProducedFromContinuousToFallowCropping =
                    maximumCarbonProducedFromContinuousToFallowCropping;
                entry.RateConstantFromContinuousToFallowCropping = rateConstantFromContinuousToFallowCropping;
                entry.MaximumCarbonProducedFromIncreaseInPerennialCropArea =
                    maximumCarbonProducedFromIncreaseInPerennialCropArea;
                entry.RateConstantFromIncreaseInPerennialCropArea = rateConstantFromIncreaseInPerennialCropArea;
                entry.MaximumCarbonProducedFromDecreaseInPerannialCropArea =
                    maximumCarbonProducedFromDecreaseInPerannialCropArea;
                entry.RateConstantFromDecreaseInPerennialCropArea = rateConstantFromDecreaseInPerennialCropArea;
                result.Add(entry);
            }

            return result;
        }
        #endregion
    }
}