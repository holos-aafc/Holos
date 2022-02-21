using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    public class MethaneConversionFactorsAndNitrogenOxideEmissionsForBeefDairySwineProvider
    {
        #region Fields
        private List<EmissionData> _cache;
        #endregion

        #region Constructors
        public MethaneConversionFactorsAndNitrogenOxideEmissionsForBeefDairySwineProvider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods

        public List<EmissionData> GetMethaneConversionFactorsAndNitrogenOxideEmissionsForBeefDairySwine()
        {
            return _cache;
        }
        #endregion

        #region Private Methods

        private List<EmissionData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.MethaneConversionFactorsAndNitrogenOxideEmissionsForBeefDairySwine;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<EmissionData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new EmissionData();
                var methaneConversionFactor = double.Parse(line[2], cultureInfo);
                var emissionFactor = double.Parse(line[3], cultureInfo);
                var fractionalVolatilization = double.Parse(line[4], cultureInfo);
                entry.MethaneConversionFactor = methaneConversionFactor;
                entry.N20DirectEmissionFactor = emissionFactor;
                entry.VolatilizationFraction = fractionalVolatilization;
                result.Add(entry);
            }

            return result;
        }

        #endregion
    }
}