using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Temperature;
using H.Infrastructure;

namespace H.Core.Providers.Climate
{
    /// <summary>
    /// A container class to hold climate data (including temperature, precipitation, and evapotranspiration according to the farm location
    /// </summary>
    public class ClimateData : ModelBase
    {
        #region Fields

        private PrecipitationData _precipitationData;
        private TemperatureData _temperatureData;
        private EvapotranspirationData _evapotranspirationData;

        private ObservableCollection<DailyClimateData> _dailyClimateData;

        /// <summary>
        /// Use a dictionary to lookup daily values since using a list with so many items is expensive
        /// </summary>
        private Dictionary<Tuple<int, int>, List<DailyClimateData>> _dataByYearAndMonth = new Dictionary<Tuple<int, int>, List< DailyClimateData>>();

        #endregion

        #region Constructors

        public ClimateData()
        {
            this.TemperatureData = new TemperatureData();
            this.PrecipitationData = new PrecipitationData();
            this.EvapotranspirationData = new EvapotranspirationData();

            this.DailyClimateData = new ObservableCollection<DailyClimateData>();
            this.DailyClimateData.CollectionChanged += DailyClimateData_CollectionChanged;
        }

        private void DailyClimateData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var addedItem = e.NewItems[0] as DailyClimateData;

                var key = new Tuple<int, int>(addedItem.Year, addedItem.Date.Month);

                if (_dataByYearAndMonth.ContainsKey(key))
                {
                    _dataByYearAndMonth[key].Add(addedItem);
                }
                else
                {
                    _dataByYearAndMonth.Add(new Tuple<int, int>(addedItem.Year, addedItem.Date.Month), new List<DailyClimateData>() { addedItem });
                }    
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Monthly precipitation normals
        /// </summary>
        public PrecipitationData PrecipitationData
        {
            get { return _precipitationData; }
            set { SetProperty(ref _precipitationData, value); }
        }

        /// <summary>
        /// Monthly temperature normals
        /// </summary>
        public TemperatureData TemperatureData
        {
            get { return _temperatureData; }
            set { SetProperty(ref _temperatureData, value); }
        }

        /// <summary>
        /// Monthly evapotranspiration normals
        /// </summary>
        public EvapotranspirationData EvapotranspirationData
        {
            get { return _evapotranspirationData; }
            set { SetProperty(ref _evapotranspirationData, value); }
        }

        public ObservableCollection<DailyClimateData> DailyClimateData
        {
            get { return _dailyClimateData; }
            set { SetProperty(ref _dailyClimateData, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Used in calculation of perennial C inputs.
        /// </summary>
        public double ProportionOfPrecipitationFallingInMayThroughSeptember(int year)
        {
            var dailyClimateDataForYear = this.DailyClimateData.Where(x => x.Year == year).ToList();
            if (dailyClimateDataForYear.Any())
            {
                const int julianDayStart = 121;     // May 1
                const int julianDayEnd = 273;       // September 30

                var dailyClimateDataForPeriod = dailyClimateDataForYear.Where(x => x.JulianDay >= julianDayStart && x.JulianDay <= julianDayEnd).ToList();
                var totalPrecipitationForPeriod = dailyClimateDataForPeriod.Sum(x => x.MeanDailyPrecipitation);

                var proportion = totalPrecipitationForPeriod / dailyClimateDataForYear.Sum(x => x.MeanDailyPrecipitation);

                return proportion;
            }
            else
            {
                var precipitationForPeriod = this.PrecipitationData.May +
                                             this.PrecipitationData.June +
                                             this.PrecipitationData.July +
                                             this.PrecipitationData.August +
                                             this.PrecipitationData.September;

                var proportion = precipitationForPeriod / this.PrecipitationData.GetTotalAnnualPrecipitation();

                return proportion;
            }
        }

        /// <summary>
        /// Returns the total annual precipitation for a particular year. Use daily data if available, otherwise uses climate normals for the total.
        /// </summary>
        public double GetTotalPrecipitationForYear(int year)
        {
            var dailyClimateDataForYear = this.DailyClimateData.Where(x => x.Year == year).ToList();
            if (dailyClimateDataForYear.Count == 365)
            {
                // We have a full years' worth daily data for this year
                var totalPrecipitation = dailyClimateDataForYear.Sum(x => x.MeanDailyPrecipitation);

                return totalPrecipitation;
            }
            else
            {
                // Don't have enough daily data, return total using SLC normals
                return this.PrecipitationData.GetTotalAnnualPrecipitation();
            }
        }

        public double GetTotalEvapotranspirationForYear(int year)
        {
            var dailyClimateDataForYear = this.DailyClimateData.Where(x => x.Year == year).ToList();

            if (dailyClimateDataForYear.Count == 365)
            {
                // We have a full years' worth daily data for this year
                var totalEvapotranspiration = dailyClimateDataForYear.Sum(x => x.MeanDailyPET);

                return totalEvapotranspiration;
            }
            else
            {
                // Don't have enough daily data, return total using SLC normals
                return this.EvapotranspirationData.GetTotalAnnualEvapotranspiration();
            }
        }

        /// <summary>
        /// Returns the total precipitation for the given year and month. Uses monthly normals if daily values are not available.
        /// </summary>
        public double GetTotalPrecipitationForMonthAndYear(int year, Months month)
        {
            var targetMonth = (int)month;

            var key = new Tuple<int, int>(year, targetMonth);

            // We won't have a full years worth of data if we are looking up values for the current (now) year and so use monthly normals instead
            if (_dataByYearAndMonth.ContainsKey(key) && DateTime.Now.Year != year)
            {
                return _dataByYearAndMonth[key].Sum(x => x.MeanDailyPrecipitation);
            }
            else
            {
                return this.PrecipitationData.GetValueByMonth(month);
            }            
        }

        public Dictionary<Months, double> GetMonthlyPrecipitationsForYear(int year)
        {
            var monthlyTotals = new Dictionary<Months, double>();

            foreach (var month in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                var precipitation = this.GetTotalPrecipitationForMonthAndYear(
                    year: year,
                    month: month);

                monthlyTotals.Add(month, precipitation);
            }

            return monthlyTotals;
        }

        /// <summary>
        /// Returns the average temperature for the given year and month. Uses monthly normals if daily values are not available.
        /// </summary>
        public double GetAverageTemperatureForMonthAndYear(int year, Months month)
        {
            var targetMonth = (int)month;

            var key = new Tuple<int, int>(year, targetMonth);

            // We won't have a full years worth of data if we are looking up values for the current (now) year and so use monthly normals instead
            if (_dataByYearAndMonth.ContainsKey(key) && DateTime.Now.Year != year)
            {
                return _dataByYearAndMonth[key].Average(x => x.MeanDailyAirTemperature);
            }
            else
            {
                return this.TemperatureData.GetValueByMonth(month);
            }
        }

        public Dictionary<Months, double> GetMonthlyTemperaturesForYear(int year)
        {
            var monthlyTotals = new Dictionary<Months, double>();

            foreach (var month in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                var temperature = this.GetAverageTemperatureForMonthAndYear(
                    year: year,
                    month: month);

                monthlyTotals.Add(month, temperature);
            }

            return monthlyTotals;
        }

        public double GetTotalEvapotranspirationForMonthAndYear(int year, Months month)
        {
            var targetMonth = (int)month;

            var key = new Tuple<int, int>(year, targetMonth);

            // We won't have a full years worth of data if we are looking up values for the current (now) year and so use monthly normals instead
            if (_dataByYearAndMonth.ContainsKey(key) && DateTime.Now.Year != year)
            {
                return _dataByYearAndMonth[key].Sum(x => x.MeanDailyPET);
            }
            else
            {
                return this.EvapotranspirationData.GetValueByMonth(month);
            }
        }

        public Dictionary<Months, double> GetMonthlyEvapotranspirationsForYear(int year)
        {
            var monthlyTotals = new Dictionary<Months, double>();
            
            foreach (var month in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                var temperature = this.GetTotalEvapotranspirationForMonthAndYear(
                    year: year,
                    month: month);

                monthlyTotals.Add(month, temperature);
            }

            return monthlyTotals;
        }

        #endregion
    }
}