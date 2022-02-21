using H.Content;
using H.Core.Converters;
using H.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Core.Tools;

namespace H.Core.Providers.Climate
{
    public class GrowingDegreeCoefficientsProvider_Table_1
    {
        private readonly List<GrowingDegreeCoefficientsData> _cache;

        public GrowingDegreeCoefficientsProvider_Table_1()
        {
            HTraceListener.AddTraceListener();

            _cache = this.BuildCache();
        }

        /// <summary>
        /// Read the file once when the class/instance is constructed. When clients call to get values from the table/file, we can go directly to the cached results instead of reading the file again.
        /// </summary>
        /// <returns></returns>
        private List<GrowingDegreeCoefficientsData> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.GrowingDegreeCoefficients).ToList();
            var converter = new CropTypeStringConverter();
            double parseResult = 0;

            var resultList = new List<GrowingDegreeCoefficientsData>();

            foreach (var line in fileLines.Skip(1))
            {
                var data = new GrowingDegreeCoefficientsData()
                {
                    Crop = converter.Convert(line[0]),
                    A = double.TryParse(line[1], System.Globalization.NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    B = double.TryParse(line[2], System.Globalization.NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C = double.TryParse(line[3], System.Globalization.NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    D = double.TryParse(line[4], System.Globalization.NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    E = double.TryParse(line[5], System.Globalization.NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                };
                resultList.Add(data);
            }
            return resultList;
        }

        public List<GrowingDegreeCoefficientsData> GetGrowingDegreeCoefficients()
        {
            return _cache;
        }

        public GrowingDegreeCoefficientsData GetByCropType(CropType cropType)
        {
            var result = _cache.FirstOrDefault(x => x.Crop == cropType);
            if (result != null)
            {
                return result;
            }
            if (cropType.IsPulseCrop())
            {
                return _cache.SingleOrDefault(x => x.Crop == CropType.FabaBeans);
            }
            else if (cropType.IsSmallGrains())
            {
                return _cache.SingleOrDefault(x => x.Crop == CropType.MaltBarley);
            }
            else if (cropType.IsOilSeed())
            {
                return _cache.SingleOrDefault(x => x.Crop == CropType.Canola);
            }
            else if (cropType.IsOtherFieldCrop())
            {
                return _cache.SingleOrDefault(x => x.Crop == CropType.SorghumSudanGrass);
            }
            else
            {
                return new GrowingDegreeCoefficientsData();
            }

        }
    }
}
