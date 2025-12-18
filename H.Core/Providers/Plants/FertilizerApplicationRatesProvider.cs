using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Infrastructure;
using H.Core.Converters;

namespace H.Core.Providers.Plants
{
    /// <summary>
    /// 
    /// </summary>
    public class FertilizerApplicationRatesProvider
    {
        #region Fields
        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();
        private readonly CropTypeStringConverter _cropTypeStringConverter = new CropTypeStringConverter();
        private readonly SoilFunctionalCategoryStringConverter _soilFunctionalCategoryStringConverter = new SoilFunctionalCategoryStringConverter();
        private readonly List<FertilizerApplicationRatesData> _cache;
        #endregion

        #region Constructors
        public FertilizerApplicationRatesProvider()
        {
            _cache = BuildCache();
        }
        #endregion

        #region Public Methods
        public List<FertilizerApplicationRatesData> GetFertilizerApplicationRates()
        {
            return _cache;
        }
        #endregion

        #region Private Methos
        private List<FertilizerApplicationRatesData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.FertilizerApplicationRates;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<FertilizerApplicationRatesData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new FertilizerApplicationRatesData();
                var cropType = _cropTypeStringConverter.Convert(line[0]);
                var province = _provinceStringConverter.Convert(line[1]);
                var soilType = _soilFunctionalCategoryStringConverter.Convert(line[2]);
                var nitrogenApplicationRate = double.Parse(line[3], cultureInfo);
                var phosphorusApplicationRate = double.Parse(line[4], cultureInfo);
                entry.CropType = cropType;
                entry.Province = province;
                entry.SoilFunctionalCategory = soilType;
                entry.NitrogenApplicationRate = nitrogenApplicationRate;
                entry.PhosphorusApplicationRate = phosphorusApplicationRate;
                result.Add(entry);
            }

            return result;
        }
        #endregion
    }
}