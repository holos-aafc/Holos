using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    public class NitrogenApplicationRatesForSpringWheatStubbleAndFallowCropsProvider
    {
        #region Fields
        private readonly List<NitrogenApplicationRatesForSpringWheatStubbleAndFallowCropsData> _cache;
        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();
        private readonly SoilFunctionalCategoryStringConverter _soilFunctionalCategoryStringConverter = new SoilFunctionalCategoryStringConverter();
        #endregion

        #region Constructors
        public NitrogenApplicationRatesForSpringWheatStubbleAndFallowCropsProvider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Functions

        public List<NitrogenApplicationRatesForSpringWheatStubbleAndFallowCropsData>
            GetNitrogenApplicationRatesForSpringWheatStubbleAndFallowCrops()
        {
            return _cache;
        }
        #endregion

        #region Private Functions
        private List<NitrogenApplicationRatesForSpringWheatStubbleAndFallowCropsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.NitrogenApplicationRatesForSpringWheatStubbleAndFallowCrops;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<NitrogenApplicationRatesForSpringWheatStubbleAndFallowCropsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new NitrogenApplicationRatesForSpringWheatStubbleAndFallowCropsData();
                var province = _provinceStringConverter.Convert(line[0]);
                var soilFunctionalCategory = _soilFunctionalCategoryStringConverter.Convert(line[1]);
                var nitrogenApplicationRateForWheatStubble = double.Parse(line[2], cultureInfo);
                var nitrogenApplicationRateForFallow = double.Parse(line[3], cultureInfo);
                var nitrogenMineralizedStubbleFallow = double.Parse(line[4], cultureInfo);
                entry.Province = province;
                entry.SoilFunctionalCategory = soilFunctionalCategory;
                entry.NitrogenApplicationRateForWheatStubble = nitrogenApplicationRateForWheatStubble;
                entry.NitrogenApplicationRateForFallow = nitrogenApplicationRateForFallow;
                entry.NitrogenMineralizedStubbleFallow = nitrogenMineralizedStubbleFallow;

                result.Add(entry);
            }

            return result;
        }
        #endregion  
    }
}