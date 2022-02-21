using System;

namespace H.Core.Models
{
    /// <summary>
    /// Default soil N2O breakdown. User can override but total must always equal 100%. User must specify how to spread out the emissions over the year.
    /// </summary>
    public class AnnualSoilN2OBreakdown : MonthlyValueBase<double>
    {
        #region Constructors

        public AnnualSoilN2OBreakdown()
        {
            base.January = 0;
            base.February = 0;
            base.March = 5;
            base.April = 30;
            base.May = 20;
            base.June = 15;
            base.July = 5;
            base.August = 5;
            base.September = 15;
            base.October = 5;
            base.November = 0;
            base.December = 0;
        }

        #endregion

        #region Public Methods

        public bool AllMonthsTotalOneHundred()
        {
            return Math.Abs((this.January + this.February + this.March + this.April + this.May + this.June + this.July +
                             this.August + this.September + this.October + this.November + this.December) - 100) < double.Epsilon;
        }

        #endregion
    }
}
