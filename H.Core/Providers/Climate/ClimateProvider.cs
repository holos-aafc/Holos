using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Properties;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Climate
{
    public class ClimateProvider : IClimateProvider
    {
        #region Constructors

        public ClimateProvider(ISlcClimateProvider slcClimateDataProvider)
        {
            HTraceListener.AddTraceListener();

            if (slcClimateDataProvider != null) _slcClimateDataProvider = slcClimateDataProvider;

            _nasaClimateProvider = new NasaClimateProvider();
            _customFileClimateDataProvider = new CustomFileClimateDataProvider();
            _climateNormalCalculator = new ClimateNormalCalculator();
            _indoorTemperatureProvider = new Table_63_Indoor_Temperature_Provider();
        }

        #endregion

        #region Fields

        private readonly NasaClimateProvider _nasaClimateProvider;
        private readonly ISlcClimateProvider _slcClimateDataProvider;
        private readonly CustomFileClimateDataProvider _customFileClimateDataProvider;
        private readonly ClimateNormalCalculator _climateNormalCalculator;

        private readonly Table_63_Indoor_Temperature_Provider _indoorTemperatureProvider;

        private readonly Dictionary<Tuple<int, TimeFrame>, ClimateData> _cacheByPolygon =
            new Dictionary<Tuple<int, TimeFrame>, ClimateData>();

        private readonly Dictionary<Tuple<double, double, TimeFrame>, ClimateData> _cacheByPosition =
            new Dictionary<Tuple<double, double, TimeFrame>, ClimateData>();

        #endregion

        #region Public Methods

        public ClimateData Get(int polygonId, TimeFrame timeFrame)
        {
            return _slcClimateDataProvider.GetClimateData(polygonId, timeFrame);
        }

        public ClimateData Get(int polygonId, TimeFrame timeFrame, Farm farm)
        {
            var climateData = _slcClimateDataProvider.GetClimateData(polygonId, timeFrame);
            climateData.BarnTemperatureData = _indoorTemperatureProvider.GetIndoorTemperature(farm.Province);

            return climateData;
        }

        public ClimateData Get(string filepath, TimeFrame normalCalculationTimeFrame, Farm farm)
        {
            var dailyClimateData = _customFileClimateDataProvider.GetDailyClimateData(filepath);

            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);
            var evapotranspirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);

            var climateData = new ClimateData(dailyClimateData)
            {
                TemperatureData = temperatureNormals,
                PrecipitationData = precipitationNormals,
                EvapotranspirationData = evapotranspirationNormals,
            };

            climateData.BarnTemperatureData = _indoorTemperatureProvider.GetIndoorTemperature(farm.Province);

            return climateData;
        }

        public ClimateData Get(List<DailyClimateData> dailyClimateData, TimeFrame timeFrame, Farm farm)
        {
            if (dailyClimateData.Any() == false)
            {
                return null;
            }

            var temperatureNormals =
                _climateNormalCalculator.GetTemperatureDataByDailyValues(dailyClimateData, timeFrame);
            var precipitationNormals =
                _climateNormalCalculator.GetPrecipitationDataByDailyValues(dailyClimateData, timeFrame);
            var evapotranspirationNormals =
                _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(dailyClimateData, timeFrame);

            var climateData = new ClimateData(dailyClimateData)
            {
                TemperatureData = temperatureNormals,
                PrecipitationData = precipitationNormals,
                EvapotranspirationData = evapotranspirationNormals
            };

            climateData.BarnTemperatureData = _indoorTemperatureProvider.GetIndoorTemperature(farm.Province);

            return climateData;
        }

        public ClimateData Get(double latitude, double longitude, TimeFrame climateNormalTimeFrame, Farm farm)
        {
            // Cache here since climate normal calculations are expensive
            var key = new Tuple<double, double, TimeFrame>(latitude, longitude, climateNormalTimeFrame);
            if (_cacheByPosition.ContainsKey(key)) return _cacheByPosition[key];

            var dailyClimateData = _nasaClimateProvider.GetCustomClimateData(latitude, longitude);
            if (dailyClimateData.Any() == false)
            {
                // This happens if exceptions are thrown when calling the nasa api (i.e. 502 Gateway Error)
                // as well as if there are timeouts or empty responses
                return null;

            var result = this.Get(dailyClimateData, climateNormalTimeFrame, farm);
            _cacheByPosition.Add(key, result);

            return result;
        }

        public double GetMeanTemperatureForDay(Farm farm, DateTime dateTime)
        {
            return farm.ClimateData.GetMeanTemperatureForDay(dateTime);
        }

        public double GetAnnualPrecipitation(Farm farm, DateTime dateTime)
        {
            return GetAnnualPrecipitation(farm, dateTime.Year);
        }


        public double GetAnnualPrecipitation(Farm farm, int year)
        {
            return farm.GetAnnualPrecipitation(year);
        }

        public double GetGrowingSeasonPrecipitation(Farm farm, DateTime dateTime)
        {
            return farm.GetGrowingSeasonPrecipitation(dateTime.Year);
        }

        public double GetGrowingSeasonPrecipitation(Farm farm, int year)
        {
            return farm.GetGrowingSeasonPrecipitation(year);
        }

        public double GetGrowingSeasonEvapotranspiration(Farm farm, DateTime dateTime)
        {
            return GetGrowingSeasonEvapotranspiration(farm, dateTime.Year);
        }

        public double GetGrowingSeasonEvapotranspiration(Farm farm, int year)
        {
            return farm.GetGrowingSeasonEvapotranspiration(year);
        }

        public double GetAnnualEvapotranspiration(Farm farm, DateTime dateTime)
        {
            return GetAnnualEvapotranspiration(farm, dateTime.Year);
        }

        public double GetAnnualEvapotranspiration(Farm farm, int year)
        {
            return farm.GetAnnualEvapotranspiration(year);
        }

        public ClimateData GetClimateData(int polygonId, TimeFrame timeFrame)
        {
            // Cache here since climate normal calculations are expensive
            var key = new Tuple<int, TimeFrame>(polygonId, timeFrame);
            if (_cacheByPolygon.ContainsKey(key)) return _cacheByPolygon[key];

            var result = _slcClimateDataProvider.GetClimateData(polygonId, timeFrame);

            _cacheByPolygon.Add(key, result);

            return result;
        }

        /// <summary>
        ///     Called when user changes the time frame in user settings. Normals must be recalculated in this situation.
        /// </summary>
        public ClimateData AdjustClimateNormalsForTimeFrame(IEnumerable<DailyClimateData> dailyClimateDatas,
            TimeFrame timeFrame)
        {
            var listDailyClimateData = new List<DailyClimateData>(dailyClimateDatas);
            var temperatureNormals =
                _climateNormalCalculator.GetTemperatureDataByDailyValues(listDailyClimateData, timeFrame);
            var precipitationNormals =
                _climateNormalCalculator.GetPrecipitationDataByDailyValues(listDailyClimateData, timeFrame);
            var evapotranspirationNormals =
                _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(listDailyClimateData, timeFrame);

            return new ClimateData
            {
                DailyClimateData = new ObservableCollection<DailyClimateData>(dailyClimateDatas),
                TemperatureData = temperatureNormals,
                PrecipitationData = precipitationNormals,
                EvapotranspirationData = evapotranspirationNormals
            };
        }

        public void OutputMonthlyClimateData(Farm farm, string outputPath)
        {
            var path = outputPath;

            var runInPeriodYears = farm.Defaults.DefaultRunInPeriod;
            var startYear = farm.GetStartYearOfEarliestRotation() - runInPeriodYears;
            var endYear = farm.GetEndYearOfEarliestRotation();

            string[] columnNames =
            {
                Resources.Year,
                Resources.LabelMonth,
                Resources.Temperature,
                Resources.LabelPrecipitation,
                Resources.LabelEvapotranspiration
            };

            const string stringFormat = "F2";

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(",", columnNames));

                for (var year = startYear; year <= endYear; year++)
                for (var j = 0; j < 12; j++)
                {
                    var month = (Months)(j + 1);

                    var precipitation = farm.ClimateData.GetTotalPrecipitationForMonthAndYear(year, month);
                    var temperature = farm.ClimateData.GetAverageTemperatureForMonthAndYear(year, month);
                    var evapotranspiration = farm.ClimateData.GetTotalEvapotranspirationForMonthAndYear(year, month);

                    string[] rowData =
                    {
                        year.ToString(), month.GetDescription(), temperature.ToString(stringFormat),
                        precipitation.ToString(stringFormat), evapotranspiration.ToString(stringFormat)
                    };

                    writer.WriteLine(string.Join(",", rowData));
                }

                writer.WriteLine();
            }
        }

        public void OutputDailyClimateData(Farm farm, string outputPath)
        {
            var path = outputPath;

            using (var writer = new StreamWriter(path))
            {
                var results = farm.ClimateData.DailyClimateData;

                string[] columnNames =
                {
                    Resources.Year,
                    Resources.JulianDay,
                    Resources.MeanDailyAirTemperature,
                    Resources.MeanDailyPrecipitation,
                    Resources.MeanDailyPET,
                    Resources.RelativeHumidity,
                    Resources.SolarRadiation,
                    Resources.Date
                };

                writer.WriteLine(string.Join(",", columnNames));

                foreach (var data in results)
                {
                    var year = Convert.ToString(data.Year);
                    var julianDay = Convert.ToString(data.JulianDay);
                    var meanDailyAirTemperature = Convert.ToString(data.MeanDailyAirTemperature);
                    var meanDailyPrecipitation = Convert.ToString(data.MeanDailyPrecipitation);
                    var meanDailyPET = Convert.ToString(data.MeanDailyPET);
                    var relativeHumidity = Convert.ToString(data.RelativeHumidity);
                    var solarRadiation = Convert.ToString(data.SolarRadiation);
                    var date =
                        $"{Convert.ToString(data.Date.Month)}/{Convert.ToString(data.Date.Day)}/{Convert.ToString(data.Date.Year)}";
                    string[] rowData =
                    {
                        year, julianDay, meanDailyAirTemperature, meanDailyPrecipitation, meanDailyPET,
                        relativeHumidity, solarRadiation, date
                    };
                    writer.WriteLine(string.Join(",", rowData));
                }

                writer.WriteLine();
            }
        }

        #endregion
    }
}