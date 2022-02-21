using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    public class SwineVolatileExcretionProvider : ProviderBase
    {
        #region Fields
        private List<SwineVolatileExcretionData> _cache;
        #endregion
        #region Constructors
        public SwineVolatileExcretionProvider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods
        public List<SwineVolatileExcretionData> GetSwineVolatileExcretion()
        {
            return _cache;
        }
        #endregion

        #region Private Methods
        private List<SwineVolatileExcretionData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SwineVolatileExcretion;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<SwineVolatileExcretionData>();

            foreach (var line in filelines.Skip(1))
            {
                var entry = new SwineVolatileExcretionData();
                var animalType = animalTypeStringConverter.Convert(line[0]);
                var province = provinceStringConverter.Convert(line[1]);
                var volatileSolidExcretion = double.Parse(line[2], cultureInfo);

                entry.AnimalType = animalType;
                entry.Province = province;
                entry.VolatileSolidExcretion = volatileSolidExcretion;

                result.Add(entry);
            }

            return result;
        }

        #endregion
    }
}