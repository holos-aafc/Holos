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
    public class GeographicDataProvider : GeographicDataProviderBase, IGeographicDataProvider
    {
        #region Fields

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

        public IEnumerable<int> GetPolygonIdList()
        {
            return _soilDataProvider.GetPolygonIdList();
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