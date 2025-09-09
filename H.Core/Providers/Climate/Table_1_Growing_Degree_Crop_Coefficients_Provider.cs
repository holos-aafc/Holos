using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Climate
{
    /// <summary>
    ///     Table 1. Values for the coefficients a, b, c, d, and e required to calculate growing degree day driven crop
    ///     coefficients (Kc) (from Martel et al. 2021)
    /// </summary>
    public class Table_1_Growing_Degree_Crop_Coefficients_Provider
    {
        private readonly List<Table_1_Growing_Degree_Crop_Coefficients_Data> _cache;

        public Table_1_Growing_Degree_Crop_Coefficients_Provider()
        {
            HTraceListener.AddTraceListener();

            _cache = BuildCache();
        }

        /// <summary>
        ///     Read the file once when the class/instance is constructed. When clients call to get values from the table/file, we
        ///     can go directly to the cached results instead of reading the file again.
        /// </summary>
        /// <returns></returns>
        private List<Table_1_Growing_Degree_Crop_Coefficients_Data> BuildCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var fileLines = CsvResourceReader.GetFileLines(CsvResourceNames.GrowingDegreeCoefficients).ToList();
            var converter = new CropTypeStringConverter();
            double parseResult = 0;

            var resultList = new List<Table_1_Growing_Degree_Crop_Coefficients_Data>();

            foreach (var line in fileLines.Skip(2))
            {
                var data = new Table_1_Growing_Degree_Crop_Coefficients_Data
                {
                    Crop = converter.Convert(line[0]),
                    A = double.TryParse(line[1], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    B = double.TryParse(line[2], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    C = double.TryParse(line[3], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    D = double.TryParse(line[4], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    E = double.TryParse(line[5], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0
                };
                resultList.Add(data);
            }

            return resultList;
        }

        public List<Table_1_Growing_Degree_Crop_Coefficients_Data> GetGrowingDegreeCoefficients()
        {
            return _cache;
        }

        public Table_1_Growing_Degree_Crop_Coefficients_Data GetByCropType(CropType cropType)
        {
            var result = _cache.FirstOrDefault(x => x.Crop == cropType);
            if (result != null) return result;
            if (cropType.IsPulseCrop()) return _cache.SingleOrDefault(x => x.Crop == CropType.FabaBeans);

            if (cropType.IsSmallGrains()) return _cache.SingleOrDefault(x => x.Crop == CropType.MaltBarley);

            if (cropType.IsOilSeed()) return _cache.SingleOrDefault(x => x.Crop == CropType.Canola);

            if (cropType.IsOtherFieldCrop()) return _cache.SingleOrDefault(x => x.Crop == CropType.SorghumSudanGrass);

            return new Table_1_Growing_Degree_Crop_Coefficients_Data();
        }
    }
}