#region Imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Tools;

#endregion

namespace H.Core.Providers
{
    /// <summary>
    /// </summary>
    public static class YearlyAverageCalculator
    {
        #region Public Methods

        public static List<double> GetAverages(this List<double> values)
        {
            HTraceListener.AddTraceListener();
            if (values.Count != 12)
            {
                var message = $"{nameof(YearlyAverageCalculator)}.{nameof(GetAverages)}: expected a list of 12 values but got {values.Count()} values instead.";
                Trace.WriteLine(message);

                throw new InvalidOperationException();
            }

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

            for (var i = 0; i < numberOfDaysInEachMonthList.Count; i++)
            {
                var currentMonthAndDayCount = numberOfDaysInEachMonthList.ElementAt(i);
                var currentInputValue = values.ElementAt(i);
                var average = currentInputValue / currentMonthAndDayCount;
                //var average = currentInputValue;
                for (var j = 0; j < currentMonthAndDayCount; j++)
                {
                    yearlyAverages.Add(average);
                }
            }

            return yearlyAverages;
        }

        #endregion

        #region Constructors

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}