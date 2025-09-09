using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Temperature;
using H.Infrastructure;

namespace H.Core.Providers.Climate
{
    /// <summary>
    ///     A container class to hold climate data (including temperature, precipitation, and evapotranspiration according to
    ///     the farm location
    /// </summary>
    public class ClimateData : ModelBase
    {
        #region Fields

        private PrecipitationData _precipitationData;
        private TemperatureData _temperatureData;
        private EvapotranspirationData _evapotranspirationData;
        private TemperatureData _barnTemperatureData;

        private ObservableCollection<DailyClimateData> _dailyClimateData;

        /// <summary>
        ///     Use a dictionary to lookup daily values since using a list with so many items is expensive
        /// </summary>
        private readonly Dictionary<Tuple<int, int>, List<DailyClimateData>> _dataByYearAndMonth =
            new Dictionary<Tuple<int, int>, List<DailyClimateData>>();

        private readonly Dictionary<Tuple<int, int, int>, DailyClimateData> _dataByDate =
            new Dictionary<Tuple<int, int, int>, DailyClimateData>();

        private readonly Dictionary<int, List<DailyClimateData>> _dailyClimateByYear =
            new Dictionary<int, List<DailyClimateData>>();

        private readonly Dictionary<int, double> _evapotranspirationByYear = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _precipitationByYear = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _growingSeasonPrecipitationByYear = new Dictionary<int, double>();
        private readonly Dictionary<int, double> _growingSeasonEvapotranspirationByYear = new Dictionary<int, double>();

        private readonly Dictionary<Tuple<DateTime, DateTime>, List<double>> _temperaturesByDateRange =
            new Dictionary<Tuple<DateTime, DateTime>, List<double>>();

        #endregion

        #region Constructors

        public ClimateData() : this(new List<DailyClimateData>())
        {
            TemperatureData = new TemperatureData();
            PrecipitationData = new PrecipitationData();
            EvapotranspirationData = new EvapotranspirationData();
            BarnTemperatureData = new TemperatureData();
        }

        public ClimateData(List<DailyClimateData> dailyClimate)
        {
            DailyClimateData = new ObservableCollection<DailyClimateData>();
            DailyClimateData.CollectionChanged += DailyClimateData_CollectionChanged;

            foreach (var dailyClimateData in dailyClimate) DailyClimateData.Add(dailyClimateData);
        }

        private void DailyClimateData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItem = e.NewItems[0] as DailyClimateData;

                var monthAndYearKey = new Tuple<int, int>(addedItem.Year, addedItem.Date.Month);
                var monthYearAndDayKey =
                    new Tuple<int, int, int>(addedItem.Year, addedItem.Date.Month, addedItem.Date.Day);

                if (_dataByDate.ContainsKey(monthYearAndDayKey))
                    _dataByDate[monthYearAndDayKey] = addedItem;
                else
                    _dataByDate.Add(new Tuple<int, int, int>(addedItem.Year, addedItem.Date.Month, addedItem.Date.Day),
                        addedItem);

                if (_dataByYearAndMonth.ContainsKey(monthAndYearKey))
                    _dataByYearAndMonth[monthAndYearKey].Add(addedItem);
                else
                    _dataByYearAndMonth.Add(new Tuple<int, int>(addedItem.Year, addedItem.Date.Month),
                        new List<DailyClimateData> { addedItem });

                if (_dailyClimateByYear.ContainsKey(addedItem.Year))
                    _dailyClimateByYear[addedItem.Year].Add(addedItem);
                else
                    _dailyClimateByYear.Add(addedItem.Year, new List<DailyClimateData> { addedItem });
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Monthly precipitation normals
        /// </summary>
        public PrecipitationData PrecipitationData
        {
            get => _precipitationData;
            set => SetProperty(ref _precipitationData, value);
        }

        /// <summary>
        ///     Monthly temperature normals
        /// </summary>
        public TemperatureData TemperatureData
        {
            get => _temperatureData;
            set => SetProperty(ref _temperatureData, value);
        }

        /// <summary>
        ///     Monthly evapotranspiration normals
        /// </summary>
        public EvapotranspirationData EvapotranspirationData
        {
            get => _evapotranspirationData;
            set => SetProperty(ref _evapotranspirationData, value);
        }

        public ObservableCollection<DailyClimateData> DailyClimateData
        {
            get => _dailyClimateData;
            set => SetProperty(ref _dailyClimateData, value);
        }

        public TemperatureData BarnTemperatureData
        {
            get => _barnTemperatureData;
            set => SetProperty(ref _barnTemperatureData, value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Used in calculation of perennial C inputs.
        /// </summary>
        public double ProportionOfPrecipitationFallingInMayThroughSeptember(int year)
        {
            var dailyClimateDataForYear = DailyClimateData.Where(x => x.Year == year).ToList();
            if (dailyClimateDataForYear.Any())
            {
                var dailyClimateDataForPeriod = dailyClimateDataForYear.Where(x =>
                    x.JulianDay >= CoreConstants.GrowingSeasonJulianStartDay &&
                    x.JulianDay <= CoreConstants.GrowingSeasonJulianEndDaySeptember).ToList();
                var totalPrecipitationForPeriod = dailyClimateDataForPeriod.Sum(x => x.MeanDailyPrecipitation);

                var proportion = totalPrecipitationForPeriod /
                                 dailyClimateDataForYear.Sum(x => x.MeanDailyPrecipitation);

                return proportion;
            }
            else
            {
                var precipitationForPeriod = PrecipitationData.May +
                                             PrecipitationData.June +
                                             PrecipitationData.July +
                                             PrecipitationData.August +
                                             PrecipitationData.September;

                var annualPrecipitation = PrecipitationData.GetTotalAnnualPrecipitation();
                if (annualPrecipitation == 0)
                    // Don't divide by 0
                    annualPrecipitation = 1;

                var proportion = precipitationForPeriod / annualPrecipitation;

                return proportion;
            }
        }

        public double GetGrowingSeasonPrecipitation(int year)
        {
            if (_growingSeasonPrecipitationByYear.ContainsKey(year)) return _growingSeasonPrecipitationByYear[year];

            if (_dailyClimateByYear.ContainsKey(year))
            {
                var dailyDataForYear = _dailyClimateByYear[year];
                var count = dailyDataForYear.Count;

                /*
                 * Check for leap years
                 */
                if (count == 365 || count == 366)
                {
                    var dailyClimateDataForPeriod = dailyDataForYear.Where(x =>
                        x.JulianDay >= CoreConstants.GrowingSeasonJulianStartDay &&
                        x.JulianDay <= CoreConstants.GrowingSeasonJulianEndDaySeptember).ToList();
                    var growingSeasonPrecipitation = dailyClimateDataForPeriod.Sum(x => x.MeanDailyPrecipitation);

                    _growingSeasonPrecipitationByYear[year] = growingSeasonPrecipitation;

                    return growingSeasonPrecipitation;
                }
            }

            // Don't have enough daily data, return total using SLC normals
            return PrecipitationData.GrowingSeasonPrecipitation;
        }

        public double GetGrowingSeasonEvapotranspiration(int year)
        {
            if (_growingSeasonEvapotranspirationByYear.ContainsKey(year))
                return _growingSeasonEvapotranspirationByYear[year];

            if (_dailyClimateByYear.ContainsKey(year))
            {
                var dailyDataForYear = _dailyClimateByYear[year];

                var count = dailyDataForYear.Count;

                /*
                 * Check for leap years
                 */
                if (count == 365 || count == 366)
                {
                    var dailyClimateDataForPeriod = dailyDataForYear.Where(x =>
                        x.JulianDay >= CoreConstants.GrowingSeasonJulianStartDay &&
                        x.JulianDay <= CoreConstants.GrowingSeasonJulianEndDaySeptember).ToList();
                    var growingSeasonEvapotranspiration = dailyClimateDataForPeriod.Sum(x => x.MeanDailyPET);

                    _growingSeasonEvapotranspirationByYear[year] = growingSeasonEvapotranspiration;

                    return growingSeasonEvapotranspiration;
                }
            }

            // Don't have enough daily data, return total using SLC normals
            return EvapotranspirationData.GrowingSeasonEvapotranspiration;
        }

        /// <summary>
        ///     Returns the total annual precipitation for a particular year. Use daily data if available, otherwise uses climate
        ///     normals for the total.
        /// </summary>
        public double GetTotalPrecipitationForYear(int year)
        {
            if (_precipitationByYear.ContainsKey(year)) return _precipitationByYear[year];

            if (_dailyClimateByYear.ContainsKey(year))
            {
                var dailyDataForYear = _dailyClimateByYear[year];
                var count = dailyDataForYear.Count;

                /*
                 * Check for leap years
                 */
                if (count == 365 || count == 366)
                {
                    var totalPrecipitationForYear = dailyDataForYear.Sum(x => x.MeanDailyPrecipitation);
                    _precipitationByYear[year] = totalPrecipitationForYear;

                    return totalPrecipitationForYear;
                }
            }

            // Don't have enough daily data, return total using SLC normals
            return PrecipitationData.GetTotalAnnualPrecipitation();
        }

        public double GetTotalEvapotranspirationForYear(int year)
        {
            if (_evapotranspirationByYear.ContainsKey(year)) return _evapotranspirationByYear[year];

            if (_dailyClimateByYear.ContainsKey(year))
            {
                var dailyDataForYear = _dailyClimateByYear[year];
                var count = dailyDataForYear.Count;

                /*
                 * Check for leap years
                 */
                if (count == 365 || count == 366)
                {
                    var totalEvapotranspiration = dailyDataForYear.Sum(x => x.MeanDailyPET);
                    _evapotranspirationByYear[year] = totalEvapotranspiration;

                    return totalEvapotranspiration;
                }
            }

            // Don't have enough daily data, return total using SLC normals
            return EvapotranspirationData.GetTotalAnnualEvapotranspiration();
        }

        /// <summary>
        ///     Returns the total precipitation for the given year and month. Uses monthly normals if daily values are not
        ///     available.
        /// </summary>
        public double GetTotalPrecipitationForMonthAndYear(int year, Months month)
        {
            var targetMonth = (int)month;

            var key = new Tuple<int, int>(year, targetMonth);

            // We won't have a full years worth of data if we are looking up values for the current (now) year and so use monthly normals instead
            if (_dataByYearAndMonth.ContainsKey(key) && DateTime.Now.Year != year)
            {
                var data = _dataByYearAndMonth[key];
                if (data.Any(x => Math.Abs(x.MeanDailyPrecipitation - -999) < double.Epsilon) == false)
                    // NASA will return -999 for any unknown values
                    return data.Sum(x => x.MeanDailyPrecipitation);
            }

            return PrecipitationData.GetValueByMonth(month);
        }

        public Dictionary<Months, double> GetMonthlyPrecipitationsForYear(int year)
        {
            var monthlyTotals = new Dictionary<Months, double>();

            foreach (var month in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                var precipitation = GetTotalPrecipitationForMonthAndYear(
                    year,
                    month);

                monthlyTotals.Add(month, precipitation);
            }

            return monthlyTotals;
        }

        /// <summary>
        ///     Returns the average temperature for the given year and month. Uses monthly normals if daily values are not
        ///     available.
        /// </summary>
        public double GetAverageTemperatureForMonthAndYear(int year, Months month)
        {
            var targetMonth = (int)month;

            var key = new Tuple<int, int>(year, targetMonth);

            // We won't have a full years worth of data if we are looking up values for the current (now) year and so use monthly normals instead
            if (_dataByYearAndMonth.ContainsKey(key) && DateTime.Now.Year != year)
            {
                var data = _dataByYearAndMonth[key];
                if (data.Any(x => Math.Abs(x.MeanDailyAirTemperature - -999) < double.Epsilon) == false)
                    // NASA will return -999 for any unknown values
                    return data.Average(x => x.MeanDailyAirTemperature);
            }

            return TemperatureData.GetValueByMonth(month);
        }

        public Dictionary<Months, double> GetMonthlyTemperaturesForYear(int year)
        {
            var monthlyTotals = new Dictionary<Months, double>();

            foreach (var month in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                var temperature = GetAverageTemperatureForMonthAndYear(
                    year,
                    month);

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
                return _dataByYearAndMonth[key].Sum(x => x.MeanDailyPET);

            return EvapotranspirationData.GetValueByMonth(month);
        }

        public Dictionary<Months, double> GetMonthlyEvapotranspirationForYear(int year)
        {
            var monthlyTotals = new Dictionary<Months, double>();

            foreach (var month in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                var temperature = GetTotalEvapotranspirationForMonthAndYear(
                    year,
                    month);

                monthlyTotals.Add(month, temperature);
            }

            return monthlyTotals;
        }

        public double GetMeanTemperatureForDay(DateTime dateTime)
        {
            var key = new Tuple<int, int, int>(dateTime.Year, dateTime.Date.Month, dateTime.Date.Day);
            if (_dataByDate.ContainsKey(key)) return _dataByDate[key].MeanDailyAirTemperature;

            return GetAverageTemperatureForMonthAndYear(dateTime.Year, (Months)dateTime.Month);
        }

        public List<double> GetTemperatureByDateRange(DateTime start, DateTime end)
        {
            var result = new List<double>();

            var key = new Tuple<DateTime, DateTime>(start.Date, end.Date);
            if (_temperaturesByDateRange.ContainsKey(key)) return _temperaturesByDateRange[key];

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                var temperatureForDay = GetMeanTemperatureForDay(date);

                result.Add(temperatureForDay);
            }

            _temperaturesByDateRange[key] = result;

            return result;
        }

        #endregion
    }
}