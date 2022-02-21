using System.Collections.Generic;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Plants
{
    public class TillageRatioFactorsForDirectSoilNitrousOxideEmissionsProvider_Table_8
    {
        #region Fields
        private readonly TillageTypeStringConverter _tillageTypeStringConverter = new TillageTypeStringConverter();
        private readonly ProvinceStringConverter _provinceStringConverter = new ProvinceStringConverter();
        private readonly List<TillageRatioFactosForDirectSoilNitrousOxideEmissionsData> _cache;
        #endregion

        #region Ctor
        public TillageRatioFactorsForDirectSoilNitrousOxideEmissionsProvider_Table_8()
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
        private List<TillageRatioFactosForDirectSoilNitrousOxideEmissionsData>
            GetTillageRatioFactosForDirectSoilNitrousOxideEmissions()
        {
            return _cache;
        }

        private List<TillageRatioFactosForDirectSoilNitrousOxideEmissionsData>
            BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filename = CsvResourceNames.TillageRatioFactosForDirectSoilNitrousOxideEmissions;
            var filelines = CsvResourceReader.GetFileLines(filename);
            var result = new List<TillageRatioFactosForDirectSoilNitrousOxideEmissionsData>();
            foreach (var line in filelines.Skip(1))
            {
                var entry = new TillageRatioFactosForDirectSoilNitrousOxideEmissionsData();
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