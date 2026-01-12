using System.Collections.Generic;
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

        private readonly NasaClimateProvider _nasaClimateProvider = new NasaClimateProvider();
        private readonly CustomFileClimateDataProvider _customFileClimateProvider = new CustomFileClimateDataProvider();
        private readonly ClimateNormalCalculator _climateNormalCalculator = new ClimateNormalCalculator();

        #endregion

        #region Constructors

        #endregion

        #region Public Methods

        public void InitializeClimate(Farm farm)
        {
            var acquisition = farm.ClimateAcquisition;
            List<DailyClimateData> dailyClimateData;

            if (acquisition == Farm.ChosenClimateAcquisition.InputFile)
            {
                dailyClimateData = _customFileClimateProvider.GetDailyClimateData(farm.ClimateDataFileName);
            }
            else
            {
                dailyClimateData = _nasaClimateProvider.GetCustomClimateData(farm.Latitude, farm.Longitude);
            }

            this.InitializeClimate(farm, dailyClimateData);
        }

        public void InitializeClimate(Farm farm, int startYear, int endYear)
        {
            var dailyClimateData = _nasaClimateProvider.GetCustomClimateData(farm.Latitude, farm.Longitude);
            var climateForPeriod = dailyClimateData.Where(x => x.Date.Year >= startYear && x.Date.Year <= endYear).ToList();

            farm.ClimateData.DailyClimateData.AddRange(climateForPeriod);

            this.SetClimateNormals(farm, climateForPeriod);
        }

        public void InitializeClimate(Farm farm, IEnumerable<DailyClimateData> dailyData)
        {
            var dailyDataList = dailyData.ToList();

            farm.ClimateData.DailyClimateData.AddRange(dailyDataList);

            this.SetClimateNormals(farm, dailyDataList);
        }

        public void SetClimateNormals(Farm farm, IEnumerable<DailyClimateData> climateForPeriod)
        {
            var climateList = climateForPeriod.ToList();

            var startYear = climateList.Min(x => x.Date.Year);
            var endYear = climateList.Max(x => x.Date.Year);

            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(climateList, startYear, endYear);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(climateList, startYear, endYear);
            var evapotranspirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(climateList, startYear, endYear);

            farm.ClimateData.EvapotranspirationData = evapotranspirationNormals;
            farm.ClimateData.PrecipitationData = precipitationNormals;
            farm.ClimateData.TemperatureData = temperatureNormals;
        }

        #endregion

        #region Private Methods



        #endregion
    }
}