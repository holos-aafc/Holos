using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Calculators.Climate;
using H.Core.Models;
using H.Core.Providers.Climate;

namespace H.Core.Services.Initialization.Climate
{
    public class ClimateInitializationService : IClimateInitializationService
    {
        #region Fields

        private readonly NasaClimateProvider _nasaClimateProvider;
        private readonly ClimateNormalCalculator _climateNormalCalculator;

        #endregion

        #region Constructors

        public ClimateInitializationService()
        {
            _nasaClimateProvider = new NasaClimateProvider();
            _climateNormalCalculator = new ClimateNormalCalculator();
        }

        #endregion

        #region Public Methods

        public void InitializeClimate(Farm farm)
        {
            var dailyClimateData = _nasaClimateProvider.GetCustomClimateData(farm.Latitude, farm.Longitude);

            var startYear = dailyClimateData.Min(x => x.Date.Year);
            var endYear = dailyClimateData.Max(x => x.Date.Year);

            this.InitializeClimate(farm, startYear, endYear);
        }

        public void InitializeClimate(Farm farm, int startYear, int endYear)
        {
            var dailyClimateData = _nasaClimateProvider.GetCustomClimateData(farm.Latitude, farm.Longitude);
            var climateForPeriod = dailyClimateData.Where(x => x.Date.Year >= startYear && x.Date.Year <= endYear).ToList();
            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(climateForPeriod, startYear, endYear);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(climateForPeriod, startYear, endYear);
            var evapotranspirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(climateForPeriod, startYear, endYear);

            farm.ClimateData.DailyClimateData.AddRange(climateForPeriod);
            farm.ClimateData.EvapotranspirationData = evapotranspirationNormals;
            farm.ClimateData.PrecipitationData = precipitationNormals;
            farm.ClimateData.TemperatureData = temperatureNormals;
        }

        #endregion
    }
}