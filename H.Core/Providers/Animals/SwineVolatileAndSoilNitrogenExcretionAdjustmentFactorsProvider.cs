using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    public class SwineVolatileAndSoilNitrogenExcretionAdjustmentFactorsProvider
    {
        #region Fields
        private readonly List<SwineVolatileAndSoilNitrogenExcretionAdjustmentFactorsData> _cache;
        #endregion

        #region Constructors
        public SwineVolatileAndSoilNitrogenExcretionAdjustmentFactorsProvider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods
        public List<SwineVolatileAndSoilNitrogenExcretionAdjustmentFactorsData> GetSwineVolatileAndSoilNitrogenExcretionAdjustmentFactors()
        {
            return _cache;
        }
        #endregion

        #region Private Methods
        private List<SwineVolatileAndSoilNitrogenExcretionAdjustmentFactorsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SwineVolatileAndSoilNitrogenExcretionAdjustmentFactors;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<SwineVolatileAndSoilNitrogenExcretionAdjustmentFactorsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new SwineVolatileAndSoilNitrogenExcretionAdjustmentFactorsData();
                var volatileSolidExcretion = double.Parse(line[1], cultureInfo);
                var nitrogenExcretionAdjustmentFactor = double.Parse(line[2], cultureInfo);
                entry.VolatileSolidExcretion = volatileSolidExcretion;
                entry.NitrogenExcretionAdjustmentFactor = nitrogenExcretionAdjustmentFactor;
                result.Add(entry);
            }

            return result;
        }
        #endregion
    }
}