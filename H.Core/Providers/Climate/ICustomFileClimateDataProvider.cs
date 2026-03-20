using System.Collections.Generic;

namespace H.Core.Providers.Climate
{
    public interface ICustomFileClimateDataProvider 
    {        
        List<DailyClimateData> GetDailyClimateData(string filePath);
        bool HasExpectedInputFormat(string fileName);
        List<string> GetExpectedFileHeaderList();
    }
}