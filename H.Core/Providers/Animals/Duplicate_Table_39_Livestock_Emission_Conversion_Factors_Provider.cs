using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Duplicate Class - This table is implemented directly in the other file.
    /// Table 39. Default methane conversion factors, direct N2O emission factors, volatilization fractions and
    /// emission factors and leaching fractions and emission factors, by livestock group and manure handling
    /// system. For beef cattle, dairy cattle and broilers, layers and turkeys, the Fracvolatilization values are
    /// estimated by Holos 
    /// </summary>
    public class Duplicate_Table_39_Livestock_Emission_Conversion_Factors_Provider
    {
        #region Fields
        private List<Table_39_Livestock_Emission_Conversion_Factors_Data> _cache;
        #endregion

        #region Constructors
        public Duplicate_Table_39_Livestock_Emission_Conversion_Factors_Provider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods

        public List<Table_39_Livestock_Emission_Conversion_Factors_Data> GetMethaneConversionFactorsAndNitrogenOxideEmissionsForBeefDairySwine()
        {
            return _cache;
        }
        #endregion

        #region Private Methods

        private List<Table_39_Livestock_Emission_Conversion_Factors_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.MethaneConversionFactorsAndNitrogenOxideEmissionsForBeefDairySwine;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<Table_39_Livestock_Emission_Conversion_Factors_Data>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new Table_39_Livestock_Emission_Conversion_Factors_Data();
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