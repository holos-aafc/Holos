using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    /// <summary>
    /// Duplicate: Soil nitrous oxide emission factors (N2O EF) as influenced by source of nitrogen, soil texture, tillage practice and crop type in Canada (adapted from Liang et al., 2020)
    ///
    /// <para>This provider contains the RF_Till values from table 20.</para>
    /// </summary>
    public class DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Tillage_Practice_Provider
    {
        #region Fields
        private readonly TillageTypeStringConverter _tillageTypeStringConverter = new TillageTypeStringConverter();
        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();
        private readonly List<DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Tillage_Practice_Data> _cache;
        #endregion

        #region Ctor
        public DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Tillage_Practice_Provider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods
        public double GetRatioFactor(Province province, TillageType tillageType)
        {
            var result = this.GetTillageRatioFactosForDirectSoilNitrousOxideEmissions().SingleOrDefault(x => x.Province == province &&
                                                                                                             x.TillageType == tillageType);

            if (result != null)
            {
                return result.RatioFactor;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Private Methods
        private List<DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Tillage_Practice_Data>
            GetTillageRatioFactosForDirectSoilNitrousOxideEmissions()
        {
            return _cache;
        }

        private List<DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Tillage_Practice_Data>
            BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SoilN2OEmissionFactorsInfluencedByTillPractice;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Tillage_Practice_Data>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Tillage_Practice_Data();
                var province = _provinceStringConverter.Convert(line[0]);
                var tillageType = _tillageTypeStringConverter.Convert(line[1]);
                var ratioFactor = double.Parse(line[2], cultureInfo);
                entry.Province = province;
                entry.TillageType = tillageType;
                entry.RatioFactor = ratioFactor;
                result.Add(entry);
            }

            return result;
        }

        #endregion
    }
}