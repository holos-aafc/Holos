#region Imports

using System.Collections.Generic;
using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using H.Core.Providers.Temperature;
using H.Infrastructure;

#endregion

namespace H.Core.Providers
{
    /// <summary>
    /// A class to hold information related to the soil types, custom yield information, and hardiness zone information for a farm.
    /// </summary>
    public class GeographicData : ModelBase
    {
        #region Fields

        private SoilData _defaultSoilData;
        private List<CustomUserYieldData> _customYieldData;
        private List<SoilData> _soilDataForAllComponentsWithinPolygon;
        private HardinessZone _hardinessZone;

        #endregion

        #region Constructors

        public GeographicData()
        {            
            this.DefaultSoilData = new SoilData();
            this.SoilDataForAllComponentsWithinPolygon = new List<SoilData>();            
            this.HardinessZone = HardinessZone.NotAvailable;
            this.CustomYieldData = new List<CustomUserYieldData>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// A farm can have multiple soil types in a location. This property will hold the default assigned (or selected by the user from a list all available types) once the location
        /// is chosen on the map view.
        /// </summary>
        public SoilData DefaultSoilData
        {
            get {  return _defaultSoilData; }
            set { SetProperty(ref _defaultSoilData, value); }
        }

        /// <summary>
        /// A farm can have multiple soil types in a location.
        /// </summary>
        public List<SoilData> SoilDataForAllComponentsWithinPolygon
        {
            get { return _soilDataForAllComponentsWithinPolygon; }
            set { SetProperty(ref _soilDataForAllComponentsWithinPolygon, value); }
        }

        public HardinessZone HardinessZone
        {
            get { return _hardinessZone;}
            set { SetProperty(ref _hardinessZone, value); }
        }

        public string HardinessZoneString
        {
            get { return this.HardinessZone.GetDescription(); }
        }

        /// <summary>
        /// A list of custom yield data entered by user
        /// </summary>
        public List<CustomUserYieldData> CustomYieldData
        {
            get { return _customYieldData; }
            set {SetProperty(ref _customYieldData, value); }
        }

        #endregion
    }
}