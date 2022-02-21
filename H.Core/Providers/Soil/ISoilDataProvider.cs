using System.Collections.Generic;

namespace H.Core.Providers.Soil
{
    public interface ISoilDataProvider
    {
        void Initialize();
        SoilData GetPredominantSoilDataByPolygonId(int polygonId);
        IEnumerable<SoilData> GetAllSoilDataForAllComponentsWithinPolygon(int polygonId);
        IEnumerable<int> GetPolygonIdList();
        string GetEcodistrictName(int polygonId);
        bool DataExistsForPolygon(int polygonId);
    }
}