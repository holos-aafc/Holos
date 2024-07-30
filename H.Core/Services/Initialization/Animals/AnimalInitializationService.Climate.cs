using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Climate;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize all <see cref="ClimateData.BarnTemperatureData"/> for all <see cref="ManagementPeriod"/>s in the <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> to initialize with new defaults</param>
        public void InitializeBarnTemperature(Farm farm)
        {
            if (farm != null && farm.ClimateData != null)
            {
                var climateData = farm.ClimateData;
                var province = farm.DefaultSoilData.Province;

                climateData.BarnTemperatureData = _indoorTemperatureProvider.GetIndoorTemperature(province);
                climateData.BarnTemperatureData.IsInitialized = true;
            }
        }

        #endregion
    }
}