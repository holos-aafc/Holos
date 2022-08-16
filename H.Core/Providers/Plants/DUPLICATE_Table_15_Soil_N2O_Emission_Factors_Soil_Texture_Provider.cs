using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    /// <summary>
    /// Table 15: Soil nitrous oxide emission factors (N2O EF) as influenced by source of nitrogen, soil texture, tillage practice and crop type in Canada (adapted from Liang et al., 2020)
    ///
    /// <para>This provider contains the RF_TX values from table 20.</para>
    /// </summary>
    public class DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Soil_Texture_Provider
    {
        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();
        private readonly SoilTextureStringConverter _soilTextureStringConverter = new SoilTextureStringConverter();
        private readonly List<DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Soil_Texture_Data> _cache;

        public DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Soil_Texture_Provider()
        {
            _cache = BuildCache();
        }

        public List<DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Soil_Texture_Data>
            GetTextureRatioFactorsForDirectSoilNitrousOxideEmissions()
        {
            return _cache;
        }

        private List<DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Soil_Texture_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.SoilN2OEmissionFactorsInfluencedBySoilTexture;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Soil_Texture_Data>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new DUPLICATE_Table_15_Soil_N2O_Emission_Factors_Soil_Texture_Data();
                var province = _provinceStringConverter.Convert(line[0]);
                var soilTexture = _soilTextureStringConverter.Convert(line[1]);
                var ratioFactor = double.Parse(line[2], cultureInfo);
                entry.Province = province;
                entry.SoilTexture = soilTexture;
                entry.RatioFactor = ratioFactor;
                result.Add(entry);
            }

            return result;
        }
    }
}