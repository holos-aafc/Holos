using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core
{
    public class MonthlyValueBase<T> : ModelBase
    {
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

        public override string ToString()
        {
            return $"{nameof(this.January)}: {this.January}, {nameof(this.February)}: {this.February}, {nameof(this.March)}: {this.March}, {nameof(this.April)}: {this.April}, {nameof(this.May)}: {this.May}, {nameof(this.June)}: {this.June}, {nameof(this.July)}: {this.July}, {nameof(this.August)}: {this.August}, {nameof(this.September)}: {this.September}, {nameof(this.October)}: {this.October}, {nameof(this.November)}: {this.November}, {nameof(this.December)}: {this.December}";
        }

        #endregion
    }
}