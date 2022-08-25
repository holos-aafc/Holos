#region Imports

#endregion

using System;
using System.Transactions;

namespace H.Core
{
    /// <summary>
    /// </summary>
    public class CoreConstants
    {
        #region Constructors

        static CoreConstants()
        {
            DefaultMaximumYear = DateTime.Now.Year; 
        }

        #endregion

        #region Fields

        #endregion

        #region Properties


        public static int GrowingSeasonJulianStartDay { get; } = 121;

        public static int GrowingSeasonJulianEndDayOctober { get; } = 305;

        public static int GrowingSeasonJulianEndDaySeptember { get; } = 273;

        public static double EntericManureCH4UncertaintyEstimate = 1;
        public static double ManureCH4UncertaintyEstimate = 1;
        public static double DirectN2OUncertaintyEstimate = 2;
        public static double IndirectN2OUncertaintyEstimate = 4;
        public static double EnergyCO2UncertaintyEstimate = 2;
        public static double CarbonConcentration = 0.45;

        /// <summary>
        /// Carbon concentration of trees used in shelterbelt component
        /// </summary>
        public static double CarbonConcentrationOfTrees = 0.5;

        public static int ShelterbeltCarbonTablesMaximumAge = 60;

        public static int IcbmProjectionPeriod = 30;
        public static int IcbmEquilibriumYear = 1985;

        public static double ValueNotDetermined = 0;
        public static double NotApplicable = -100000;
        public static double DaysInYear = 365.0;

        /// <summary>
        /// A sensible default for inputs that require a minimum allowable year (i.e. year of tillage change, year of seeding, etc.).
        ///
        /// Note: use a double since rad controls can't use an int.
        /// </summary>
        public static double DefaultMinimumYear = 1800;

        /// <summary>
        /// Carbon calculations are not made for land use changes (conversions) prior to 1910.
        ///
        /// Note: use a double since rad controls can't use an int.
        /// </summary>
        public static double MinimumCarbonModellingYear = 1910;

        /// <summary>
        /// Note that this value must account for any projected components that the user adds to the farm.
        /// </summary>
        public static double DefaultMaximumYear { get; }

        /// <summary>
        /// Converts from CO2 to CO2e
        /// </summary>
        public static double CO2ToCO2eConversionFactor = 1;

        /// <summary>
        /// Converts from CH4 to CO2e
        /// </summary>
        public static double CH4ToCO2eConversionFactor = 28;

        /// <summary>
        /// Converts N2O to CO2e
        /// </summary>
        public static double N2OToCO2eConversionFactor = 265;        

        /// <summary>
        /// Converts from NH3-N to NH3
        /// </summary>
        public static double ConvertNH3NToNH3 = 17.0 / 14.0;

        /// <summary>
        /// Converts from N2O-N to N2O
        /// </summary>
        public static double ConvertN2ONToN2O = 44.0 / 28.0;

        /// <summary>
        /// Converts from C to CO2
        /// </summary>
        public static double ConvertFromCToCO2 = 44.0 / 12.0;

        public static string NotApplicableOutputString
        {
            get { return H.Core.Properties.Resources.NotApplicable; } // Don't use a '-' character as it confuses when there are negative numbers in a grid cell
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}