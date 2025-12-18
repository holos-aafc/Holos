using System.Collections.Generic;
using H.Core.Models;
using H.Core.Providers.Temperature;

namespace H.Core.Providers.Climate
{
    public interface ICustomFileClimateDataProvider 
    {        
        List<DailyClimateData> GetDailyClimateData(string filePath);
        bool HasExpectedInputFormat(string fileName);
        List<string> GetExpectedFileHeaderList();
    }
}