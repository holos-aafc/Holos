using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core
{
    public class MonthlyValueBase<T> : ModelBase
    {
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

        #region Constructors

        public MonthlyValueBase()
        {
            this.January = default(T);
            this.February = default(T);
            this.March = default(T);
            this.April = default(T);
            this.May = default(T);
            this.June = default(T);
            this.July = default(T);
            this.August = default(T);
            this.September = default(T);
            this.October = default(T);
            this.November = default(T);
            this.December = default(T);

            _monthNames = new List<string>()
            {
                nameof(this.January),
                nameof(this.February),
                nameof(this.March),
                nameof(this.April),
                nameof(this.May),
                nameof(this.June),
                nameof(this.July),
                nameof(this.August),
                nameof(this.September),
                nameof(this.October),
                nameof(this.November),
                nameof(this.December)
            };
        }

        #endregion

        #region Properties

        public T January
        {
            get { return _january; }
            set { SetProperty(ref _january, value); }
        }

        public T February
        {
            get { return _february; }
            set { SetProperty(ref _february, value); } 
        }
        public T March
        {
            get { return _march; }
            set { SetProperty(ref _march, value); } 
        }
        public T April
        {
            get { return _april; }
            set { SetProperty(ref _april, value); } 
        }
        public T May
        {
            get { return _may; }
            set { SetProperty(ref _may, value); } 
        }
        public T June
        {
            get { return _june; }
            set { SetProperty(ref _june, value); } 
        }
        public T July
        {
            get { return _july; }
            set { SetProperty(ref _july, value); } 
        }
        public T August
        {
            get { return _august; }
            set { SetProperty(ref _august, value); } 
        }
        public T September
        {
            get { return _september; }
            set { SetProperty(ref _september, value); } 
        }
        public T October
        {
            get { return _october; }
            set { SetProperty(ref _october, value); } 
        }
        public T November
        {
            get { return _november; }
            set { SetProperty(ref _november, value); } 
        }
        public T December
        {
            get { return _december; }
            set { SetProperty(ref _december, value); } 
        }

    
        #endregion

        #region Public Methods

        public T GetValueByMonth(Months month)
        {
            switch (month)
            {
                case Months.January:
                    return this.January;
                case Months.February:
                    return this.February;
                case Months.March:
                    return this.March;
                case Months.April:
                    return this.April;
                case Months.May:
                    return this.May;
                case Months.June:
                    return this.June;
                case Months.July:
                    return this.July;
                case Months.August:
                    return this.August;
                case Months.September:
                    return this.September;
                case Months.October:
                    return this.October;
                case Months.November:
                    return this.November;
                default:
                    return this.December;
            }
        }

        public void AssignMonthlyValues(List<T> valuesByMonth)
        {
            this.January = valuesByMonth.ElementAtOrDefault(0);
            this.February = valuesByMonth.ElementAtOrDefault(1);
            this.March = valuesByMonth.ElementAtOrDefault(2);
            this.April = valuesByMonth.ElementAtOrDefault(3);
            this.May = valuesByMonth.ElementAtOrDefault(4);
            this.June = valuesByMonth.ElementAtOrDefault(5);
            this.July = valuesByMonth.ElementAtOrDefault(6);
            this.August = valuesByMonth.ElementAtOrDefault(7);
            this.September = valuesByMonth.ElementAtOrDefault(8);
            this.October = valuesByMonth.ElementAtOrDefault(9);
            this.November = valuesByMonth.ElementAtOrDefault(10);
            this.December = valuesByMonth.ElementAtOrDefault(11);
        }

        public void AssignValueByMonth(T value, Months month)
        {
            switch (month)
            {
                case Months.January:
                    this.January = value; 
                    break;
                case Months.February:
                    this.February = value;
                    break;
                case Months.March:
                    this.March = value;
                    break;
                case Months.April:
                    this.April = value;
                    break;
                case Months.May:
                    this.May = value;
                    break;
                case Months.June:
                    this.June = value;
                    break;
                case Months.July:
                    this.July = value;
                    break;
                case Months.August:
                    this.August = value;
                    break;
                case Months.September:
                    this.September = value;
                    break;
                case Months.October:
                    this.October = value;
                    break;
                case Months.November:
                    this.November = value;
                    break;
                default:
                    this.December = value;
                    break;
            }
        }

        public bool IsNameOfMonth(string propertyName)
        {
            foreach (var monthName in _monthNames)
            {
                if (propertyName.IndexOf(monthName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public Months GetMonthFromPropertyUpdateChange(string propertyName)
        {
            if (propertyName.IndexOf(nameof(this.January), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.January;
            }
            else if (propertyName.IndexOf(nameof(this.February), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.February;
            }
            else if (propertyName.IndexOf(nameof(this.March), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.March;
            }
            else if (propertyName.IndexOf(nameof(this.April), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.April;
            }
            else if (propertyName.IndexOf(nameof(this.May), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.May;
            }
            else if (propertyName.IndexOf(nameof(this.June), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.June;
            }
            else if (propertyName.IndexOf(nameof(this.July), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.July;
            }
            else if (propertyName.IndexOf(nameof(this.August), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.August;
            }
            else if (propertyName.IndexOf(nameof(this.September), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.September;
            }
            else if (propertyName.IndexOf(nameof(this.October), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.October;
            }
            else if (propertyName.IndexOf(nameof(this.November), StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return Months.November;
            }
            else
            {
                return Months.December;
            }
        }

        public override string ToString()
        {
            return $"{nameof(this.January)}: {this.January}, {nameof(this.February)}: {this.February}, {nameof(this.March)}: {this.March}, {nameof(this.April)}: {this.April}, {nameof(this.May)}: {this.May}, {nameof(this.June)}: {this.June}, {nameof(this.July)}: {this.July}, {nameof(this.August)}: {this.August}, {nameof(this.September)}: {this.September}, {nameof(this.October)}: {this.October}, {nameof(this.November)}: {this.November}, {nameof(this.December)}: {this.December}";
        }

        #endregion
    }
}