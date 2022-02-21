using System.Collections.Generic;

namespace H.Core.Providers.Soil
{
    public interface ICustomFileYieldProvider
    {
        bool HasExpectedInputFormat(string filename);
        List<CustomUserYieldData> GetYieldData(string filePath);
    }
}