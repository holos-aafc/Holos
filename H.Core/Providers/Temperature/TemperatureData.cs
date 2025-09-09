#region Imports

using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;

#endregion

namespace H.Core.Providers.Temperature
{
    /// <summary>
    /// </summary>
    public class TemperatureData : MonthlyValueBase<double>
    {
        #region Properties

        public int PolygonId { get; set; }

        #endregion

        #region Public Methods

        public double GetMeanAnnualTemperature()
        {
            return new List<double>
            {
                January,
                February,
                March,
                April,
                May,
                June,
                July,
                August,
                September,
                October,
                November,
                December
            }.Average();
        }

        public double GetMeanTemperatureForMonth(int month)
        {
            if (month == 1) return January;

            if (month == 2) return February;

            if (month == 3) return March;

            if (month == 4) return April;

            if (month == 5) return May;

            if (month == 6) return June;

            if (month == 7) return July;

            if (month == 8) return August;

            if (month == 9) return September;

            if (month == 10) return October;

            if (month == 11) return November;

            return December;
        }

        public override string ToString()
        {
            return
                $"{nameof(PolygonId)}: {PolygonId}, {nameof(January)}: {January}, {nameof(February)}: {February}, {nameof(March)}: {March}, {nameof(April)}: {April}, {nameof(May)}: {May}, {nameof(June)}: {June}, {nameof(July)}: {July}, {nameof(August)}: {August}, {nameof(September)}: {September}, {nameof(October)}: {October}, {nameof(November)}: {November}, {nameof(December)}: {December}";
        }

        public List<double> GetAveragedYearlyValues()
        {
            var list = new List<double>
            {
                January,
                February,
                March,
                April,
                May,
                June,
                July,
                August,
                September,
                October,
                November,
                December
            };

            var yearlyAverages = new List<double>();
            var numberOfDaysInEachMonthList = new List<int>
            {
                (int)DaysInMonth.January,
                (int)DaysInMonth.February,
                (int)DaysInMonth.March,
                (int)DaysInMonth.April,
                (int)DaysInMonth.May,
                (int)DaysInMonth.June,
                (int)DaysInMonth.July,
                (int)DaysInMonth.August,
                (int)DaysInMonth.September,
                (int)DaysInMonth.October,
                (int)DaysInMonth.November,
                (int)DaysInMonth.December
            };

            for (var i = 0; i < numberOfDaysInEachMonthList.Count; i++)
            {
                var currentMonthAndDayCount = numberOfDaysInEachMonthList.ElementAt(i);
                var currentInputValue = list.ElementAt(i);
                for (var j = 0; j < currentMonthAndDayCount; j++) yearlyAverages.Add(currentInputValue);
            }

            return yearlyAverages;
        }

        #endregion
    }
}