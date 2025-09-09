#region Imports

using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Providers.Soil;
using H.Infrastructure;

#endregion

namespace H.Core.Providers
{
    /// <summary>
    ///     A class to hold information related to the soil types, custom yield information, and hardiness zone information for
    ///     a farm.
    /// </summary>
    public class GeographicData : ModelBase
    {
        #region Constructors

        public GeographicData()
        {
            DefaultSoilData = new SoilData();
            SoilDataForAllComponentsWithinPolygon = new List<SoilData>();
            HardinessZone = HardinessZone.NotAvailable;
            CustomYieldData = new List<CustomUserYieldData>();
        }

        #endregion

        #region Fields

        private SoilData _defaultSoilData;
        private List<CustomUserYieldData> _customYieldData;
        private List<SoilData> _soilDataForAllComponentsWithinPolygon;
        private HardinessZone _hardinessZone;

        #endregion

        #region Properties

        /// <summary>
        ///     A farm can have multiple soil types in a location. This property will hold the default assigned (or selected by the
        ///     user from a list all available types) once the location
        ///     is chosen on the map view.
        /// </summary>
        public SoilData DefaultSoilData
        {
            get => _defaultSoilData;
            set => SetProperty(ref _defaultSoilData, value);
        }

        /// <summary>
        ///     A farm can have multiple soil types in a location.
        /// </summary>
        public List<SoilData> SoilDataForAllComponentsWithinPolygon
        {
            get => _soilDataForAllComponentsWithinPolygon;
            set => SetProperty(ref _soilDataForAllComponentsWithinPolygon, value);
        }

        public HardinessZone HardinessZone
        {
            get => _hardinessZone;
            set => SetProperty(ref _hardinessZone, value);
        }

        public string HardinessZoneString => HardinessZone.GetDescription();

        /// <summary>
        ///     A list of custom yield data entered by user
        /// </summary>
        public List<CustomUserYieldData> CustomYieldData
        {
            get => _customYieldData;
            set => SetProperty(ref _customYieldData, value);
        }

        #endregion
    }
}