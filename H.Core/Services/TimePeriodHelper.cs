#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Infrastructure;

#endregion

namespace H.Core.Services
{
    /// <summary>
    /// </summary>
    public class TimePeriodHelper : ITimePeriodHelper
    {
        #region Inner Classes

        public class MonthlyObject
        {
            public int Month { get; set; }
            public int Year { get; set; }
        } 

        #endregion

        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public bool TimePeriodHasOpenings(IEnumerable<ITimePeriodItem> itemsInTimePeriod, DateTime minimumDate,
                                          DateTime maximumDate)
        {
            return this.GetFirstTimePeriodOpening(itemsInTimePeriod, minimumDate, maximumDate) != null;
        }

        public IEnumerable<MonthlyObject> GetMonthsBetweenDates(DateTime start, TimeSpan duration)
        {
            var end = start.Add(duration);

            return this.GetMonthsBetweenDates(start, end);
        }

        public IEnumerable<MonthlyObject> GetMonthsBetweenDates(DateTime start, DateTime end)
        {
            var result = new List<MonthlyObject>();

            for (var i = start; i < end; i = i.AddDays(1))
            {
                var month = i.Month;
                var year = i.Year;

                if (result.Any(x => x.Month == month && x.Year == year) == false)
                {
                    result.Add(new MonthlyObject {Month = month, Year = year});
                }
            }

            return result.OrderBy(x => x.Year).ThenBy(x => x.Month);
        }

        /// <summary>
        /// Returns the number of days a time period occupies, (e.g. April 15 - April 30 = 15)
        /// </summary>
        public int GetNumberOfDaysOccupyingMonth(DateTime start, TimeSpan duration, int month, int year)
        {
            var days = new List<int>();

            for (var i = start; i < start.Add(duration); i = i.AddDays(1))
            {
                if (i.Month == month && i.Year == year)
                {
                    days.Add(i.Day);
                }
            }

            return days.Count;
        }

        public string GetMonthString(int month)
        {
            switch (month)
            {
                case 1:
                    return Months.January.GetDescription();

                case 2:
                    return Months.February.GetDescription();

                case 3:
                    return Months.March.GetDescription();

                case 4:
                    return Months.April.GetDescription();

                case 5:
                    return Months.May.GetDescription();

                case 6:
                    return Months.June.GetDescription();

                case 7:
                    return Months.July.GetDescription();

                case 8:
                    return Months.August.GetDescription();

                case 9:
                    return Months.September.GetDescription();

                case 10:
                    return Months.October.GetDescription();

                case 11:
                    return Months.November.GetDescription();

                case 12:
                    return Months.December.GetDescription();

                default:
                    return Months.January.GetDescription();
            }
        }

        public string GetMonthString(Months month)
        {
            return this.GetMonthString((int) month);
        }

        public Tuple<DateTime, DateTime> GetFirstTimePeriodOpening(IEnumerable<ITimePeriodItem> itemsInTimePeriod,
                                                                   DateTime minimumDate, DateTime maximumDate)
        {
            return this.GetTimePeriodOpenings(itemsInTimePeriod, minimumDate, maximumDate)
                       .FirstOrDefault();
        }

        public List<Tuple<DateTime, DateTime>> GetTimePeriodOpenings(IEnumerable<ITimePeriodItem> itemsInTimePeriod,
                                                                     DateTime minimumDate, DateTime maximumDate)
        {
            itemsInTimePeriod = itemsInTimePeriod.OrderBy(item => item.Start);

            // If there are no items in the time period, then the entire time period is open
            if (itemsInTimePeriod.Any() == false)
            {
                return new List<Tuple<DateTime, DateTime>> {new Tuple<DateTime, DateTime>(minimumDate, maximumDate)};
            }

            var result = new List<Tuple<DateTime, DateTime>>();

            var firstItemInTimePeriod = itemsInTimePeriod.First();
            if (itemsInTimePeriod.Count() == 1)
            {
                // Check if there is an opening between the start of the time period and the start date of the single item
                if (minimumDate < firstItemInTimePeriod.Start)
                {
                    result.Add(new Tuple<DateTime, DateTime>(minimumDate,
                                                             firstItemInTimePeriod.Start.Subtract(TimeSpan.FromDays(1))));
                }

                // Check if there is an opening between the end date of the single item and the end of the time period
                if (itemsInTimePeriod.First()
                                     .End < maximumDate)
                {
                    result.Add(new Tuple<DateTime, DateTime>(firstItemInTimePeriod.End.Add(TimeSpan.FromDays(1)),
                                                             maximumDate));
                }

                return result;
            }

            // Check if there is an opening between the start of the time period and the start date of the first item
            if (minimumDate < firstItemInTimePeriod.Start)
            {
                result.Add(new Tuple<DateTime, DateTime>(minimumDate,
                                                         firstItemInTimePeriod.Start.Subtract(TimeSpan.FromDays(1))));
            }

            for (var i = 0; i < itemsInTimePeriod.Count() - 1; i++)
            {
                var itemInTimePeriodAtIndex = itemsInTimePeriod.ElementAt(i);
                var itemInTimePeriodAtNextIndex = itemsInTimePeriod.ElementAt(i + 1);

                if (itemInTimePeriodAtIndex.End < itemInTimePeriodAtNextIndex.Start)
                {
                    if (Math.Abs(itemInTimePeriodAtIndex.End.Subtract(itemInTimePeriodAtNextIndex.Start)
                                                        .TotalDays) > 1)
                    {
                        result.Add(new Tuple<DateTime, DateTime>(itemInTimePeriodAtIndex.End.AddDays(1),
                                                                 itemInTimePeriodAtNextIndex.Start.Subtract(TimeSpan.FromDays(1))));
                    }
                }
            }

            var lastItem = itemsInTimePeriod.Last();
            if (lastItem.End < maximumDate)
            {
                result.Add(new Tuple<DateTime, DateTime>(lastItem.End.AddDays(1), maximumDate));
            }

            return result;
        }

        #endregion
    }
}