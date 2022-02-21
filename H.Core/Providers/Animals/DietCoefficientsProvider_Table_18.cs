using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// Table 18
    /// </summary>
    public class DietCoefficientsProvider_Table_18
    {
        #region Fields

        private DietTypeStringConverter _dietConverter = new DietTypeStringConverter();
        private readonly List<DietCoefficientsData> _cache;

        #endregion

        #region Constructors

        public DietCoefficientsProvider_Table_18()
        {
            _cache = BuildCache();
        }

        #endregion

        #region Public Methods
        public List<DietCoefficientsData> GetDietCoefficientsForDairyBeefSheep()
        {
            return _cache;
        }
        #endregion

        #region Private Methods
        private List<DietCoefficientsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.DietCoefficientsForDairyBeefSheep;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<DietCoefficientsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new DietCoefficientsData();
                var dietType = _dietConverter.Convert(line[0]);
                var percentDigestibleEnergyInFeed = double.Parse(line[1], cultureInfo);
                var crudeProteinContent = double.Parse(line[2], cultureInfo);
                var methaneConversionFactor = double.Parse(line[3], cultureInfo);
                entry.DietType = dietType;
                entry.PercentDigestibleEnergyInFeed = percentDigestibleEnergyInFeed;
                entry.CrudeProteinContent = crudeProteinContent;
                entry.MethaneConversionFactor = methaneConversionFactor;
                result.Add(entry);
            }

            return result;
        }
        #endregion
    }
}