using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Soil
{
    public class CanadianAgriculturalRegionIdToSlcIdProvider
    {
        public const int DefaultValueForBadPolygonInput = 0;

        private static readonly ProvinceStringConverter _provinceConverter = new ProvinceStringConverter();
        private List<CanadianAgriculturalRegionIdToSlcIdData> _cachedData;

        public CanadianAgriculturalRegionIdToSlcIdProvider()
        {
            HTraceListener.AddTraceListener();
            this._cachedData = this.GetData();
        }

        private List<CanadianAgriculturalRegionIdToSlcIdData> GetData()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var files = new List<CsvResourceNames>
            {
                CsvResourceNames.AB_CAR_to_SLC,
                CsvResourceNames.SK_CAR_to_SLC
            };
            var result = new List<CanadianAgriculturalRegionIdToSlcIdData>();

            foreach (var filename in files)
            {
                var filelines = CsvResourceReader.GetFileLines(filename);                
                int i;

                foreach (var line in filelines.Skip(1))
                {
                    var entry = new CanadianAgriculturalRegionIdToSlcIdData()
                    {
                        PrId = int.Parse(line[0], cultureInfo),
                        PrName = _provinceConverter.Convert(line[1]),
                        CarId = int.Parse(line[2], cultureInfo),
                        CarName = line[3],
                        SplitPolys = int.TryParse(line[4], System.Globalization.NumberStyles.Integer, cultureInfo, out i) ? i : -99,
                        PolygonId = int.Parse(line[5], cultureInfo),
                        EcodistrictId = int.Parse(line[6], cultureInfo),
                    };
                    result.Add(entry);
                }
            }
            return result;
        }

        public int GetCarId(int polyId)
        {
            var result = _cachedData.FirstOrDefault(x => x.PolygonId.Equals(polyId));
            if (result != null)
            {
                return result.CarId;
            }            

            Trace.TraceError($"{nameof(CanadianAgriculturalRegionIdToSlcIdProvider)}.{nameof(CanadianAgriculturalRegionIdToSlcIdProvider.GetCarId)} unable to get car id for {polyId}. Returning default value of {DefaultValueForBadPolygonInput}.");

            return DefaultValueForBadPolygonInput;
        }

        public int GetCarId(int polyId, int SplitPolys)
        {
            var result = _cachedData.FirstOrDefault(x => x.PolygonId.Equals(polyId) && x.SplitPolys.Equals(SplitPolys));
            if (result != null)
            {
                return result.CarId;
            }

            Trace.TraceError($"{nameof(CanadianAgriculturalRegionIdToSlcIdProvider)}{nameof(CanadianAgriculturalRegionIdToSlcIdProvider.GetCarId)} unable to get car id for {polyId} and {SplitPolys} Returning default value of {DefaultValueForBadPolygonInput}.");

            return DefaultValueForBadPolygonInput;
        }
    }
}
