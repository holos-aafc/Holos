using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Soil;

namespace H.Core.Providers
{
    public interface IGeographicDataProvider : ISoilDataProvider
    {
        GeographicData GetGeographicalData(int polygonId);   
    }
}