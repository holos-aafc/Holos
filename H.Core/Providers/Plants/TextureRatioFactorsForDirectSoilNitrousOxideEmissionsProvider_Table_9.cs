using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{

    public class TextureRatioFactorsForDirectSoilNitrousOxideEmissionsProvider_Table_9
    {
        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();
        private readonly SoilTextureStringConverter _soilTextureStringConverter = new SoilTextureStringConverter();
        private readonly List<TextureRatioFactorsForDirectSoilNitrousOxideEmissionsData> _cache;

        public TextureRatioFactorsForDirectSoilNitrousOxideEmissionsProvider_Table_9()
        {
            _cache = BuildCache();
        }

        public List<TextureRatioFactorsForDirectSoilNitrousOxideEmissionsData>
            GetTextureRatioFactorsForDirectSoilNitrousOxideEmissions()
        {
            return _cache;
        }

        private List<TextureRatioFactorsForDirectSoilNitrousOxideEmissionsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.TextureRatioFactorsForDirectSoilNitrousOxideEmissions;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<TextureRatioFactorsForDirectSoilNitrousOxideEmissionsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new TextureRatioFactorsForDirectSoilNitrousOxideEmissionsData();
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