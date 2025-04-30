#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Polygon;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using H.Core.Providers.Temperature;
using H.Core.Tools;

#endregion

namespace H.Core.Providers
{
    /// <summary>
    /// Provides soil data for a given location within Canada
    /// </summary>
    public class GeographicDataProvider : GeographicDataProviderBase, IGeographicDataProvider, IHolosMapPolygonIdListProvider
    {
        #region Fields

        private List<int> _polygonIdsCache = new List<int>();
        private readonly ISoilDataProvider _soilDataProvider;

        #endregion

        #region Constructors

        public GeographicDataProvider()
        {
            HTraceListener.AddTraceListener();            

            _soilDataProvider = new NationalSoilDataBaseProvider();
        }

        #endregion

        #region Properties

        public void Initialize()
        {
            if (this.IsInitialized)
            {
                return;
            }

            _soilDataProvider.Initialize();

            this.IsInitialized = true;

            Trace.TraceInformation($"{nameof(GeographicDataProvider)} has been initialized.");
        }

        public SoilData GetPredominantSoilDataByPolygonId(int polygonId)
        {
            return _soilDataProvider.GetPredominantSoilDataByPolygonId(polygonId);
        }

        public IEnumerable<SoilData> GetAllSoilDataForAllComponentsWithinPolygon(int polygonId)
        {
            return _soilDataProvider.GetAllSoilDataForAllComponentsWithinPolygon(polygonId);
        }

        public List<int> GetPolygonIdList()
        {
            if (_polygonIdsCache.Any())
            {
                return _polygonIdsCache;
            }

            var result =  _soilDataProvider.GetPolygonIdList();

            _polygonIdsCache.AddRange(result);

            return result;
        }

        public string GetEcodistrictName(int polygonId)
        {
            return _soilDataProvider.GetEcodistrictName(polygonId);
        }

        public bool DataExistsForPolygon(int polygonId)
        {
            return _soilDataProvider.DataExistsForPolygon(polygonId);
        }

        public GeographicData GetGeographicalData(int polygonId)
        {
            var result = new GeographicData();

            var predominantSoilDataByPolygonId = _soilDataProvider.GetPredominantSoilDataByPolygonId(polygonId);
            var allSoilData = _soilDataProvider.GetAllSoilDataForAllComponentsWithinPolygon(polygonId);

            result = new GeographicData
            {
                DefaultSoilData = predominantSoilDataByPolygonId,
                SoilDataForAllComponentsWithinPolygon = allSoilData.ToList(),           
            };

            return result;            
        }      

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}