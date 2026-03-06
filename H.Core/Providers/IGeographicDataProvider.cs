using H.Core.Providers.Soil;

namespace H.Core.Providers
{
    public interface IGeographicDataProvider : ISoilDataProvider
    {
        GeographicData GetGeographicalData(int polygonId);   
    }
}