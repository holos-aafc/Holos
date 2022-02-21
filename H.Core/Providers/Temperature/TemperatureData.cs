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
            return new List<double>()
            {
                this.January,
                this.February,
                this.March,
                this.April,
                this.May,
                this.June,
                this.July,
                this.August,
                this.September,
                this.October,
                this.November,
                this.December,

            }.Average();
        }

        public double GetMeanTemperatureForMonth(int month)
        {
            if (month == 1)
            {
                return this.January;
            }

            if (month == 2)
            {
                return this.February;
            }

            if (month == 3)
            {
                return this.March;
            }

            if (month == 4)
            {
                return this.April;
            }

            if (month == 5)
            {
                return this.May;
            }

            if (month == 6)
            {
                return this.June;
            }

            if (month == 7)
            {
                return this.July;
            }

            if (month == 8)
            {
                return this.August;
            }

            if (month == 9)
            {
                return this.September;
            }

            if (month == 10)
            {
                return this.October;
            }

            if (month == 11)
            {
                return this.November;
            }

            return this.December;
        }

        public override string ToString()
        {
            return
                $"{nameof(this.PolygonId)}: {this.PolygonId}, {nameof(this.January)}: {this.January}, {nameof(this.February)}: {this.February}, {nameof(this.March)}: {this.March}, {nameof(this.April)}: {this.April}, {nameof(this.May)}: {this.May}, {nameof(this.June)}: {this.June}, {nameof(this.July)}: {this.July}, {nameof(this.August)}: {this.August}, {nameof(this.September)}: {this.September}, {nameof(this.October)}: {this.October}, {nameof(this.November)}: {this.November}, {nameof(this.December)}: {this.December}";
        }

        public List<double> GetAveragedYearlyValues()
        {
            var list = new List<double>
            {
                this.January,
                this.February,
                this.March,
                this.April,
                this.May,
                this.June,
                this.July,
                this.August,
                this.September,
                this.October,
                this.November,
                this.December
            };

            var yearlyAverages = new List<double>();
            var numberOfDaysInEachMonthList = new List<int>
            {
                (int) DaysInMonth.January,
                (int) DaysInMonth.February,
                (int) DaysInMonth.March,
                (int) DaysInMonth.April,
                (int) DaysInMonth.May,
                (int) DaysInMonth.June,
                (int) DaysInMonth.July,
                (int) DaysInMonth.August,
                (int) DaysInMonth.September,
                (int) DaysInMonth.October,
                (int) DaysInMonth.November,
                (int) DaysInMonth.December
            };

            for (int i = 0; i < numberOfDaysInEachMonthList.Count; i++)
            {
                var currentMonthAndDayCount = numberOfDaysInEachMonthList.ElementAt(i);
                var currentInputValue = list.ElementAt(i);
                for (var j = 0; j < currentMonthAndDayCount; j++)
                {
                    yearlyAverages.Add(currentInputValue);
                }
            }

            return yearlyAverages;
        }

        #endregion
    }
}