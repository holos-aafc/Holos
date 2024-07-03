using System.Collections;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Climate;
using H.Core.Providers.Temperature;

namespace H.Core.Services
{
    public class InitializationService : IInitializationService
    {
        #region Fields

        private readonly IIndoorTemperatureProvider _indoorTemperatureProvider;

        #endregion

        #region Constructors

        public InitializationService()
        {
            _indoorTemperatureProvider = new Table_63_Indoor_Temperature_Provider();
        }

        #endregion

        #region Public Methods

        public void CheckInitialization(Farm farm)
        {
            if (farm == null)
            {
                return;
            }

            if (farm.DefaultSoilData == null)
            {
                return;
            }

            var soilData = farm.DefaultSoilData;

            if (farm.ClimateData == null)
            {
                return;
            }

            var climateData = farm.ClimateData;

            var barnTemperature = climateData.BarnTemperatureData;
            if (barnTemperature == null || barnTemperature.IsInitialized == false)
            {
                this.InitializeBarnTemperature(farm.ClimateData, soilData.Province);
            }
        }

        public void ReInitializeFarms(IEnumerable<Farm> farms)
        {
            foreach (var farm in farms)
            {
                // Table 63
                this.InitializeBarnTemperature(farm.ClimateData, farm.DefaultSoilData.Province);
            }
        }

        #endregion

        #region Private Methods

        private void InitializeBarnTemperature(ClimateData climateData, Province province)
        {
            climateData.BarnTemperatureData = _indoorTemperatureProvider.GetIndoorTemperature(province);
            climateData.BarnTemperatureData.IsInitialized = true;
        }

        #endregion
    }
}