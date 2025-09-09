using H.Core.Models;
using H.Core.Providers;
using H.Core.Providers.Soil;

namespace H.Core.Services.Initialization.Geography
{
    public class GeographyInitializationService : IGeographyInitializationService
    {
        #region Fields

        private readonly IGeographicDataProvider _geographicDataProvider;

        #endregion

        #region Constructors

        public GeographyInitializationService()
        {
            _geographicDataProvider = new GeographicDataProvider();
            _geographicDataProvider.Initialize();
        }

        #endregion

        #region Public Methods

        public void InitializeGeography(Farm farm)
        {
            var geographicData = _geographicDataProvider.GetGeographicalData(farm.PolygonId);
            if (geographicData.DefaultSoilData == null) geographicData.DefaultSoilData = new SoilData();

            farm.GeographicData = geographicData;
        }

        #endregion
    }
}