using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using CsvHelper;
using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Shelterbelt;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Climate
{
    public class ClimateProvider : IClimateProvider
    {
        #region Fields

        private readonly NasaClimateProvider _nasaClimateProvider;
        private readonly ISlcClimateProvider _slcClimateDataProvider;
        private readonly CustomFileClimateDataProvider _customFileClimateDataProvider;
        private readonly ClimateNormalCalculator _climateNormalCalculator;

        private readonly Table_63_Indoor_Temperature_Provider _indoorTemperatureProvider;

        #endregion

        #region Constructors

        public ClimateProvider(ISlcClimateProvider slcClimateDataProvider)
        {
            HTraceListener.AddTraceListener();

            if (slcClimateDataProvider != null)
            {
                _slcClimateDataProvider = slcClimateDataProvider;
            }

            _nasaClimateProvider = new NasaClimateProvider();
            _customFileClimateDataProvider = new CustomFileClimateDataProvider();
            _climateNormalCalculator = new ClimateNormalCalculator();
            _indoorTemperatureProvider = new Table_63_Indoor_Temperature_Provider();
        }

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

        public ClimateData Get(List<DailyClimateData> dailyClimateData, TimeFrame timeFrame)
        {
            if (dailyClimateData.Any() == false)
            {
                return null;
            }

            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(dailyClimateData, timeFrame);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(dailyClimateData, timeFrame);
            var evapotranspirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(dailyClimateData, timeFrame);

            return new ClimateData(dailyClimateData)
            {
                TemperatureData = temperatureNormals,
                PrecipitationData = precipitationNormals,
                EvapotranspirationData = evapotranspirationNormals,
            };
        }

        public ClimateData Get(double latitude, double longitude, TimeFrame climateNormalTimeFrame)
        {
            var dailyClimateData = _nasaClimateProvider.GetCustomClimateData(latitude, longitude);
            if (dailyClimateData.Any() == false)
            {                
                // This will happen when timeouts to the NASA API occur
                return null;
            }

            return this.Get(dailyClimateData, climateNormalTimeFrame);
        }

        public ClimateData Get(double latitude, double longitude, TimeFrame climateNormalTimeFrame, Farm farm)
        {
            var climateData = this.Get(latitude, longitude, climateNormalTimeFrame);
            if (climateData != null)
            {
                climateData.BarnTemperatureData = _indoorTemperatureProvider.GetIndoorTemperature(farm.Province);
            }

            return climateData;
        }

        public double GetMeanTemperatureForDay(Farm farm, DateTime dateTime)
        {
            return farm.ClimateData.GetMeanTemperatureForDay(dateTime);
        }

        public double GetAnnualPrecipitation(Farm farm, DateTime dateTime)
        {
            return this.GetAnnualPrecipitation(farm, dateTime.Year);
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
            return this.GetGrowingSeasonEvapotranspiration(farm, dateTime.Year);
        }

        public double GetGrowingSeasonEvapotranspiration(Farm farm, int year)
        {
            return farm.GetGrowingSeasonEvapotranspiration(year);
        }

        public double GetAnnualEvapotranspiration(Farm farm, DateTime dateTime)
        {
            return this.GetAnnualEvapotranspiration(farm, dateTime.Year);
        }

        public double GetAnnualEvapotranspiration(Farm farm, int year)
        {
            return farm.GetAnnualEvapotranspiration(year);
        }

        public ClimateData Get(string filepath, TimeFrame normalCalculationTimeFrame)
        {
            var dailyClimateData = _customFileClimateDataProvider.GetDailyClimateData(filepath);

            var temperatureNormals = _climateNormalCalculator.GetTemperatureDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);
            var precipitationNormals = _climateNormalCalculator.GetPrecipitationDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);
            var evapotranspirationNormals = _climateNormalCalculator.GetEvapotranspirationDataByDailyValues(dailyClimateData, normalCalculationTimeFrame);

            return new ClimateData()
            {
                DailyClimateData = new ObservableCollection<DailyClimateData>(dailyClimateData),
                TemperatureData = temperatureNormals,
                PrecipitationData = precipitationNormals,
                EvapotranspirationData = evapotranspirationNormals,
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

        public void OutputMonthlyClimateData(Farm farm, string outputPath)
        {
            var path = outputPath;

            var runInPeriodYears = farm.Defaults.DefaultRunInPeriod;
            var startYear = farm.GetStartYearOfEarliestRotation() - runInPeriodYears;
            var endYear = farm.GetEndYearOfEarliestRotation();

            string[] columnNames = {
                Properties.Resources.Year,
                Properties.Resources.LabelMonth,
                Properties.Resources.Temperature,
                Properties.Resources.LabelPrecipitation,
                Properties.Resources.LabelEvapotranspiration,
            };

            const string stringFormat = "F2";

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(",", columnNames));

                for (int year = startYear; year < endYear; year++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        var month = (Months) (j + 1);

                        var precipitation = farm.ClimateData.GetTotalPrecipitationForMonthAndYear(year, month);
                        var temperature = farm.ClimateData.GetAverageTemperatureForMonthAndYear(year, month);
                        var evapotranspiration = farm.ClimateData.GetTotalEvapotranspirationForMonthAndYear(year, month);

                        string[] rowData = { year.ToString(), month.GetDescription(), temperature.ToString(stringFormat), precipitation.ToString(stringFormat), evapotranspiration.ToString(stringFormat) };

                        writer.WriteLine(string.Join(",", rowData));
                    }
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

                string[] columnNames = {
                    Properties.Resources.Year,
                    Properties.Resources.JulianDay,
                    Properties.Resources.MeanDailyAirTemperature,
                    Properties.Resources.MeanDailyPrecipitation,
                    Properties.Resources.MeanDailyPET,
                    Properties.Resources.RelativeHumidity,
                    Properties.Resources.SolarRadiation,
                    Properties.Resources.Date,
};

                writer.WriteLine(string.Join(",", columnNames));

                foreach (var data in results)
                {
                    string year = Convert.ToString(data.Year);
                    string julianDay = Convert.ToString(data.JulianDay);
                    string meanDailyAirTemperature = Convert.ToString(data.MeanDailyAirTemperature);
                    string meanDailyPrecipitation = Convert.ToString(data.MeanDailyPrecipitation);
                    string meanDailyPET = Convert.ToString(data.MeanDailyPET);
                    string relativeHumidity = Convert.ToString(data.RelativeHumidity);
                    string solarRadiation = Convert.ToString(data.SolarRadiation);
                    string date = $"{Convert.ToString(data.Date.Month)}/{Convert.ToString(data.Date.Day)}/{Convert.ToString(data.Date.Year)}";
                    string[] rowData = { year, julianDay, meanDailyAirTemperature, meanDailyPrecipitation, meanDailyPET, relativeHumidity, solarRadiation, date };
                    writer.WriteLine(string.Join(",", rowData));
                }

                writer.WriteLine();
            }
        }

        #endregion
    }
}