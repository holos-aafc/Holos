using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Tools;

namespace H.Core.Providers.Climate
{
    public class ClimateProvider
    {
        #region Fields

        private readonly NasaClimateProvider _nasaClimateProvider;
        private readonly SlcClimateDataProvider _slcClimateDataProvider;
        private readonly CustomFileClimateDataProvider _customFileClimateDataProvider;
        private readonly ClimateNormalCalculator _climateNormalCalculator;

        #endregion

        #region Constructors

        public ClimateProvider()
        {
            HTraceListener.AddTraceListener();

            _nasaClimateProvider = new NasaClimateProvider();
            _slcClimateDataProvider = new SlcClimateDataProvider();
            _customFileClimateDataProvider = new CustomFileClimateDataProvider();
            _climateNormalCalculator = new ClimateNormalCalculator();
        }

        #endregion

        #region Public Methods

        

        public ClimateData Get(int polygonId, TimeFrame timeFrame)
        {
            return _slcClimateDataProvider.GetClimateData(polygonId, timeFrame);
        }

        public ClimateData Get(double latitude, double longitude, TimeFrame climateNormalTimeFrame)
        {
            var dailyClimateData = _nasaClimateProvider.GetCustomClimateData(latitude, longitude);
            if (dailyClimateData.Any() == false)
            {                
                // This will happen when timeouts to the NASA API occur
                return null;
            }

            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(dailyClimateData, climateNormalTimeFrame);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(dailyClimateData, climateNormalTimeFrame);
            var evapotranspirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(dailyClimateData, climateNormalTimeFrame);

            return new ClimateData()
            {
                DailyClimateData = new ObservableCollection<DailyClimateData>(dailyClimateData),
                TemperatureData = temperatureNormals,
                PrecipitationData = precipitationNormals,
                EvapotranspirationData = evapotranspirationNormals,
            };
        }

        public ClimateData Get(string filepath, TimeFrame normalCalculationTimeFrame)
        {
            var dailyClimateData = _customFileClimateDataProvider.GetDailyClimateData(filepath);

            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);
            var evapotranpirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);

            return new ClimateData()
            {
                DailyClimateData = new ObservableCollection<DailyClimateData>(dailyClimateData),
                TemperatureData = temperatureNormals,
                PrecipitationData = precipitationNormals,
                EvapotranspirationData = evapotranpirationNormals,
            };
        }

        /// <summary>
        /// Called when user changes the time frame in user settings. Normals must be recalculated in this situation.
        /// </summary>
        public ClimateData AdjustClimateNormalsForTimeFrame(IEnumerable<DailyClimateData> dailyClimateDatas, TimeFrame timeFrame)
        {
            var listDailyClimateData = new List<DailyClimateData>(dailyClimateDatas);
            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(listDailyClimateData, timeFrame);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(listDailyClimateData, timeFrame);
            var evapotranspirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(listDailyClimateData, timeFrame);

            return new ClimateData()
            {
                DailyClimateData = new ObservableCollection<DailyClimateData>(dailyClimateDatas),
                TemperatureData = temperatureNormals,
                PrecipitationData = precipitationNormals,
                EvapotranspirationData = evapotranspirationNormals,
            };
        }
        #endregion
    }
}