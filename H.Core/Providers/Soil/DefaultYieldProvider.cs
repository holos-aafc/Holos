using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using H.Content;
using H.Core.Converters;
using H.Core.Enumerations;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Soil
{
    /// <summary>
    /// Taken from worksheet 'AB Each CAR by YR all CROPs' of file
    /// </summary>
    public class DefaultYieldProvider
    {
        public DefaultYieldTableData DefaultValueForGetRowByCarIdYearAndCropType = new DefaultYieldTableData()
        {
            PrId = 0,
            CarId = 0,
            PrSad = 0,
            CropType = CropType.Barley,
            Seeded = 0,
            ESeed = 0,
            Harv = 0,
            EHarv = 0,
            PerHarv = 0,
            Yield = 0,
            YldLbs = 0,
            EYield = 0,
            ProdN = 0,
            PrdLbs = 0,
            EProdN = 0,
            NYield = 0,
            NYldLbs = 0,
            NEYield = 0,
            PPYield = 0
        };

        private static readonly CropTypeStringConverter _cropTypeConverter = new CropTypeStringConverter();
        private List<DefaultYieldTableData> _cachedData;

        public DefaultYieldProvider()
        {
            HTraceListener.AddTraceListener();
            this._cachedData = this.GetData();
        }

        private List<DefaultYieldTableData> GetData()
        {
            //return new List<DefaultYieldTableData>();

            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var files = new List<CsvResourceNames>
            {
                CsvResourceNames.AB_Default_Yields,
                CsvResourceNames.SK_Default_Yields,
                CsvResourceNames.BC_Default_Yields,
                CsvResourceNames.MB_Default_Yields,
                CsvResourceNames.ON_Default_Yields,
                CsvResourceNames.QC_Default_Yields
            };
            var result = new List<DefaultYieldTableData>();

            foreach (var filename in files)
            {
                var filelines = CsvResourceReader.GetFileLines(filename);
                var totalcols = filelines.First().Count();
                double i;

                foreach (var line in filelines.Skip(1))
                {
                    if (line[1] == "all" || line[2] == "all") // if CARID or PRSAD equals "all" then skip the rows
                        continue;

                    var entry = new DefaultYieldTableData();

                    var prId = int.Parse(line[0], cultureInfo);
                    var carId = int.Parse(line[1], cultureInfo);
                    var prSad = int.Parse(line[2], cultureInfo);
                    var year = int.Parse(line[3], cultureInfo);
                    var cropName = _cropTypeConverter.Convert(line[4]);
                    var seeded = double.Parse(line[5], cultureInfo);
                    var eSeed = double.Parse(line[6], cultureInfo);
                    var harv = double.Parse(line[7], cultureInfo);
                    var eHarv = double.Parse(line[8], cultureInfo);
                    var perHarv = double.Parse(line[9].Replace("%", ""), cultureInfo);
                    var yield = double.Parse(line[10], cultureInfo);
                    var yldLbs = double.Parse(line[11], cultureInfo);
                    var eYield = double.Parse(line[12], cultureInfo);
                    var prodN = double.TryParse(line[13], System.Globalization.NumberStyles.Float, cultureInfo, out i) ? i : -99;
                    var prdLbs = double.TryParse(line[14], System.Globalization.NumberStyles.Float, cultureInfo, out i) ? i : -99;
                    var eProdN = double.Parse(line[15], cultureInfo);
                    var nYield = double.TryParse(line[16], System.Globalization.NumberStyles.Float, cultureInfo, out i) ? i : -99;
                    var nYldLbs = double.TryParse(line[17], System.Globalization.NumberStyles.Float, cultureInfo, out i) ? i : -99;
                    var nEYield = double.Parse(line[18], cultureInfo);
                    var pPYield = double.Parse(line[19].Replace("%", ""), cultureInfo);

                    // Column number check has to be performed as the column is missing in some of the files
                    if (totalcols == 21)
                    {
                        var cSad = line[20];
                        entry.CSad = cSad;
                    }

                    entry.PrId = prId;
                    entry.CarId = carId;
                    entry.PrSad = prSad;
                    entry.Year = year;
                    entry.CropType = cropName;
                    entry.Seeded = seeded;
                    entry.ESeed = eSeed;
                    entry.Harv = harv;
                    entry.EHarv = eHarv;
                    entry.PerHarv = perHarv;
                    entry.Yield = yield;
                    entry.YldLbs = yldLbs;
                    entry.EYield = eYield;
                    entry.ProdN = prodN;
                    entry.PrdLbs = prdLbs;
                    entry.EProdN = eProdN;
                    entry.NYield = nYield;
                    entry.NYldLbs = nYldLbs;
                    entry.NEYield = nEYield;
                    entry.PPYield = pPYield;

                    result.Add(entry);
                }
            }

            return result;
        }

        public DefaultYieldTableData GetRowByCarIdYearAndCropType(int carId, int year, CropType cropType)
        {
            var listByCarId = _cachedData.Where(x => x.CarId == carId);
            if (listByCarId.Any() == false)
            {
                Trace.TraceError($"{nameof(DefaultYieldProvider)}.{nameof(DefaultYieldProvider.GetRowByCarIdYearAndCropType)}" +
                    $" unable to get row for carID: {carId}, year: {year}, crop type: {cropType}." +
                    $" Returning default value of {DefaultValueForGetRowByCarIdYearAndCropType}.");
                return DefaultValueForGetRowByCarIdYearAndCropType;
            }

            var listByYear = listByCarId.Where(x => x.Year == year);
            if (listByYear.Any() == false)
            {
                Trace.TraceError($"{nameof(DefaultYieldProvider)}.{nameof(DefaultYieldProvider.GetRowByCarIdYearAndCropType)}" +
                    $" unable to get row for carID: {carId}, year: {year}, crop type: {cropType}." +
                    $" Returning default value of {DefaultValueForGetRowByCarIdYearAndCropType}.");
                return DefaultValueForGetRowByCarIdYearAndCropType;
            }

            var listByCrop = listByCarId.Where(x => x.CropType == cropType);
            if (listByCrop.Any() == false)
            {
                Trace.TraceError($"{nameof(DefaultYieldProvider)}.{nameof(DefaultYieldProvider.GetRowByCarIdYearAndCropType)}" +
                    $" unable to get row for carID: {carId}, year: {year}, crop type: {cropType}." +
                    $" Returning default value of {DefaultValueForGetRowByCarIdYearAndCropType}.");
                return DefaultValueForGetRowByCarIdYearAndCropType;
            }

            return _cachedData.FirstOrDefault(x => x.CarId.Equals(carId) && x.Year.Equals(year) && x.CropType.Equals(cropType));
        }
    }
}

