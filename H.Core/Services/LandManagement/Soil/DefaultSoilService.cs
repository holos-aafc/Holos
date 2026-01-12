using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers;
using H.Core.Providers.Soil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Core.Services.LandManagement.Soil
{
    public class DefaultSoilService : ISoilService
    {
        #region Fields

        private readonly IGeographicDataProvider _geographicDataProvider;
        private readonly Dictionary<int, GeographicData> _cache;

        #endregion

        #region Constructors

        public DefaultSoilService(IGeographicDataProvider geographicDataProvider)
        {
            if (geographicDataProvider != null)
            {
                _geographicDataProvider = geographicDataProvider;
            }
            else
            {
                throw new ArgumentNullException(nameof(geographicDataProvider));
            }

            _cache = new Dictionary<int, GeographicData>();
            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a list of all available soil types for the <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> with the <see cref="Farm.PolygonId"/> set to non-zero</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="SoilData"/> for the <see cref="Farm"/>'s location</returns>
        public List<SoilData> GetSoilData(Farm farm)
        {
            var geographicData = this.GetGeographicalData(farm);

            // Filter by distinct soil great group types
            var filtered = this.SelectValidSoilData(geographicData.SoilDataForAllComponentsWithinPolygon);

            return filtered;
        }

        /// <summary>
        /// Filters out soil types that cannot currently be modelled.
        /// </summary>
        /// <param name="soils">A <see cref="List{T}"/> of <see cref="SoilData"/> that will be filtered</param>
        /// <returns>A filtered <see cref="List{T}"/> of <see cref="SoilData"/></returns>
        public List<SoilData> SelectValidSoilData(IEnumerable<SoilData> soils)
        {
            var result = new List<SoilData>();

            foreach (var soilData in soils)
            {
                // Add this type of soil if it is not already in the list
                if (result.FirstOrDefault(x => x.SoilGreatGroup == soilData.SoilGreatGroup) == null)
                {
                    // We don't model organic soil at this point
                    if (soilData.SoilFunctionalCategory != SoilFunctionalCategory.Organic)
                    {
                        result.Add(soilData);
                    }
                }
            }

            return result;
        }

        public GeographicData GetGeographicalData(int polygonId)
        {
            if (_cache.ContainsKey(polygonId))
            {
                return _cache[polygonId];
            }

            var geographicData = _geographicDataProvider.GetGeographicalData(polygonId);
            _cache.Add(polygonId, geographicData);

            return geographicData;
        }

        public GeographicData GetGeographicalData(Farm farm)
        {
            return this.GetGeographicalData(farm.PolygonId);
        }

        public void SetGeographicalData(Farm farm)
        {
            var geographicData = this.GetGeographicalData(farm.PolygonId);
            var soilData = this.GetSoilData(farm);
            if (soilData.Count == 0)
            {
                var emptySoil = this.CreateDefaultSoilDataForEmptyPolygon(geographicData, farm);
                geographicData.SoilDataForAllComponentsWithinPolygon.Add(emptySoil);
                soilData.Add(emptySoil);
            }

            var defaultSoilData = soilData.First();
            geographicData.DefaultSoilData = defaultSoilData;

            farm.GeographicData = geographicData;
            farm.Province = geographicData.DefaultSoilData.Province;
        }

        public bool HasValidSoilData(Farm farm)
        {
            var soilData = this.GetSoilData(farm);

            return soilData.Count > 0;
        }

        /// <summary>
        /// Create a default soil data for a polygon that doesn't contain any valid soil types. This method
        /// allows us to populate the various information that appears in the map view when a user selects a polygon
        /// including polygon id, province etc.
        /// </summary>
        /// <param name="geographicData">The geographic data information of the polygon</param>
        /// <param name="farm">The farm with the soil data</param>
        /// <returns>An instance of <see cref="SoilData"/> that serves as the default soil for the current polygon.</returns>
        public SoilData CreateDefaultSoilDataForEmptyPolygon(GeographicData geographicData, Farm farm)
        {
            var soilData = new SoilData();
            soilData.PolygonId = farm.PolygonId;
            soilData.Province = farm.Province;
            soilData.IsDefaultSoilData = true;

            var hasSoilData = geographicData.SoilDataForAllComponentsWithinPolygon.Count > 0;
            if (hasSoilData)
            {
                var soil = geographicData.SoilDataForAllComponentsWithinPolygon.First();
                soilData.EcodistrictId = soil.EcodistrictId;
                soilData.EcodistrictName = soil.EcodistrictName;
                soilData.Ecozone = soil.Ecozone;
            }
            else
            {
                soilData.EcodistrictId = 0;
                soilData.EcodistrictName = "N/A";
                soilData.Ecozone = Ecozone.Unknown;
            }

            return soilData;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}