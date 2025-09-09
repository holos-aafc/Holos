using System;

namespace H.Core.Providers.Soil
{
    /// <summary>
    ///     Table 15: Default soil N2O emission breakdown.
    ///     Default soil N2O breakdown. User can override but total must always equal 100%. User must specify how to spread out
    ///     the emissions over the year.
    /// </summary>
    public class Table_15_Default_Soil_N2O_Emission_BreakDown_Provider : MonthlyValueBase<double>
    {
        #region Constructors

        /// <summary>
        ///     Footnote: Values based on expert opinion (H. Janzen 2010).
        /// </summary>
        public Table_15_Default_Soil_N2O_Emission_BreakDown_Provider()
        {
            January = 0;
            February = 0;
            March = 5;
            April = 30;
            May = 20;
            June = 15;
            July = 5;
            August = 5;
            September = 15;
            October = 5;
            November = 0;
            December = 0;
        }

        #endregion

        #region Public Methods

        public bool AllMonthsTotalOneHundred()
        {
            return Math.Abs(January + February + March + April + May + June + July +
                August + September + October + November + December - 100) < double.Epsilon;
        }

        #endregion
    }
}