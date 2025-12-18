using H.Content;
using H.Infrastructure;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace H.Core.Providers.Soil
{
    public class LethbridgeSOCDataProvider
    {
        private List<SOCData> _cache;

        public LethbridgeSOCDataProvider()
        {
            _cache = BuildLethbridgeSOCDataCache();
        }

        public List<SOCData> GetLethbridgeSOCDatas()
        {
            return _cache;
        }

        private List<SOCData> BuildLethbridgeSOCDataCache()
        {
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;
            var filelines = CsvResourceReader.GetFileLines(CsvResourceNames.CondensedLethbridgeSOCData).ToList();
            double parseResult = 0;
            int parseResultInt = 0;

            var result = new List<SOCData>();
            foreach (var line in filelines.Skip(1))
            {
                var data = new SOCData()
                {
                    Year = int.TryParse(line[0], NumberStyles.Any, cultureInfo, out parseResultInt) ? parseResultInt : 0,
                    W_N0P0 = double.TryParse(line[1], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    W_N45P0 = double.TryParse(line[2], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    W_N0P20 = double.TryParse(line[3], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    W_N45P20 = double.TryParse(line[4], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    FW_N0P0 = double.TryParse(line[5], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    FW_N45P0 = double.TryParse(line[6], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    FW_N0P20 = double.TryParse(line[7], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    FW_N45P20 = double.TryParse(line[8], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    FWW_N0P0 = double.TryParse(line[9], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    FWW_N45P0 = double.TryParse(line[10], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    FWW_N0P20 = double.TryParse(line[11], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                    FWW_N45P20 = double.TryParse(line[12], NumberStyles.Any, cultureInfo, out parseResult) ? parseResult : 0,
                };

                result.Add(data);
            }

            return result;
        }

    }
}
