using System;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Services
{
    public interface ITimePeriodHelper
    {
        List<Tuple<DateTime, DateTime>> GetTimePeriodOpenings(IEnumerable<ITimePeriodItem> itemsInTimePeriod,
                                                              DateTime minimumDate, DateTime maximumDate);

        Tuple<DateTime, DateTime> GetFirstTimePeriodOpening(IEnumerable<ITimePeriodItem> itemsInTimePeriod,
                                                            DateTime minimumDate, DateTime maximumDate);

        bool TimePeriodHasOpenings(IEnumerable<ITimePeriodItem> itemsInTimePeriod, DateTime minimumDate,
                                   DateTime maximumDate);

        IEnumerable<TimePeriodHelper.MonthlyObject> GetMonthsBetweenDates(DateTime start, TimeSpan duration);
        IEnumerable<TimePeriodHelper.MonthlyObject> GetMonthsBetweenDates(DateTime start, DateTime end);
        int GetNumberOfDaysOccupyingMonth(DateTime start, TimeSpan duration, int month, int year);

        string GetMonthString(int month);
        string GetMonthString(Months month);
    }
}