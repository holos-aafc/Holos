using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core
{
    public class MonthlyValueBase<T> : ModelBase
    {
        #region Constructors

        public MonthlyValueBase()
        {
            January = default;
            February = default;
            March = default;
            April = default;
            May = default;
            June = default;
            July = default;
            August = default;
            September = default;
            October = default;
            November = default;
            December = default;

            _monthNames = new List<string>
            {
                nameof(January),
                nameof(February),
                nameof(March),
                nameof(April),
                nameof(May),
                nameof(June),
                nameof(July),
                nameof(August),
                nameof(September),
                nameof(October),
                nameof(November),
                nameof(December)
            };
        }

        #endregion

        #region Fields

        private T _january;
        private T _february;
        private T _march;
        private T _april;
        private T _may;
        private T _june;
        private T _july;
        private T _august;
        private T _september;
        private T _october;
        private T _november;
        private T _december;

        private readonly List<string> _monthNames;

        #endregion

        #region Properties

        public T January
        {
            get => _january;
            set => SetProperty(ref _january, value);
        }

        public T February
        {
            get => _february;
            set => SetProperty(ref _february, value);
        }

        public T March
        {
            get => _march;
            set => SetProperty(ref _march, value);
        }

        public T April
        {
            get => _april;
            set => SetProperty(ref _april, value);
        }

        public T May
        {
            get => _may;
            set => SetProperty(ref _may, value);
        }

        public T June
        {
            get => _june;
            set => SetProperty(ref _june, value);
        }

        public T July
        {
            get => _july;
            set => SetProperty(ref _july, value);
        }

        public T August
        {
            get => _august;
            set => SetProperty(ref _august, value);
        }

        public T September
        {
            get => _september;
            set => SetProperty(ref _september, value);
        }

        public T October
        {
            get => _october;
            set => SetProperty(ref _october, value);
        }

        public T November
        {
            get => _november;
            set => SetProperty(ref _november, value);
        }

        public T December
        {
            get => _december;
            set => SetProperty(ref _december, value);
        }

        #endregion

        #region Public Methods

        public T GetValueByMonth(Months month)
        {
            switch (month)
            {
                case Months.January:
                    return January;
                case Months.February:
                    return February;
                case Months.March:
                    return March;
                case Months.April:
                    return April;
                case Months.May:
                    return May;
                case Months.June:
                    return June;
                case Months.July:
                    return July;
                case Months.August:
                    return August;
                case Months.September:
                    return September;
                case Months.October:
                    return October;
                case Months.November:
                    return November;
                default:
                    return December;
            }
        }

        public void AssignMonthlyValues(List<T> valuesByMonth)
        {
            January = valuesByMonth.ElementAtOrDefault(0);
            February = valuesByMonth.ElementAtOrDefault(1);
            March = valuesByMonth.ElementAtOrDefault(2);
            April = valuesByMonth.ElementAtOrDefault(3);
            May = valuesByMonth.ElementAtOrDefault(4);
            June = valuesByMonth.ElementAtOrDefault(5);
            July = valuesByMonth.ElementAtOrDefault(6);
            August = valuesByMonth.ElementAtOrDefault(7);
            September = valuesByMonth.ElementAtOrDefault(8);
            October = valuesByMonth.ElementAtOrDefault(9);
            November = valuesByMonth.ElementAtOrDefault(10);
            December = valuesByMonth.ElementAtOrDefault(11);
        }

        public void AssignValueByMonth(T value, Months month)
        {
            switch (month)
            {
                case Months.January:
                    January = value;
                    break;
                case Months.February:
                    February = value;
                    break;
                case Months.March:
                    March = value;
                    break;
                case Months.April:
                    April = value;
                    break;
                case Months.May:
                    May = value;
                    break;
                case Months.June:
                    June = value;
                    break;
                case Months.July:
                    July = value;
                    break;
                case Months.August:
                    August = value;
                    break;
                case Months.September:
                    September = value;
                    break;
                case Months.October:
                    October = value;
                    break;
                case Months.November:
                    November = value;
                    break;
                default:
                    December = value;
                    break;
            }
        }

        public bool IsNameOfMonth(string propertyName)
        {
            foreach (var monthName in _monthNames)
                if (propertyName.IndexOf(monthName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    return true;

            return false;
        }

        public Months GetMonthFromPropertyUpdateChange(string propertyName)
        {
            if (propertyName.IndexOf(nameof(January), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.January;

            if (propertyName.IndexOf(nameof(February), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.February;

            if (propertyName.IndexOf(nameof(March), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.March;

            if (propertyName.IndexOf(nameof(April), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.April;

            if (propertyName.IndexOf(nameof(May), StringComparison.InvariantCultureIgnoreCase) >= 0) return Months.May;

            if (propertyName.IndexOf(nameof(June), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.June;

            if (propertyName.IndexOf(nameof(July), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.July;

            if (propertyName.IndexOf(nameof(August), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.August;

            if (propertyName.IndexOf(nameof(September), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.September;

            if (propertyName.IndexOf(nameof(October), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.October;

            if (propertyName.IndexOf(nameof(November), StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Months.November;

            return Months.December;
        }

        public override string ToString()
        {
            return
                $"{nameof(January)}: {January}, {nameof(February)}: {February}, {nameof(March)}: {March}, {nameof(April)}: {April}, {nameof(May)}: {May}, {nameof(June)}: {June}, {nameof(July)}: {July}, {nameof(August)}: {August}, {nameof(September)}: {September}, {nameof(October)}: {October}, {nameof(November)}: {November}, {nameof(December)}: {December}";
        }

        #endregion
    }
}