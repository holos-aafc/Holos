using System.Collections.Generic;
using H.Core.Models;
using H.Core.Providers;
using H.Core.Providers.Soil;

namespace H.Core.Services.LandManagement.Soil
{
    public interface ISoilService
    {
        GeographicData GetGeographicalData(int polygonId);
        SoilData CreateDefaultSoilDataForEmptyPolygon(GeographicData geographicData, Farm farm);
        bool HasValidSoilData(Farm farm);
        List<SoilData> SelectValidSoilData(IEnumerable<SoilData> soils);

        #region Public Methods

        List<SoilData> GetSoilData(Farm farm);

        GeographicData GetGeographicalData(Farm farm);
        void SetGeographicalData(Farm farm);

        #endregion
    }
}