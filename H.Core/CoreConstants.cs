#region Imports

using System;
using System.Security.RightsManagement;
using System.Transactions;
using H.Core.Properties;
using H.Infrastructure;

#endregion

namespace H.Core
{
    /// <summary>
    /// </summary>
    public class CoreConstants
    {
        #region Constants

        public const int DefaultNumberOfDecimalPlaces = 3;

        /// <summary>
        /// Converts from N2O-N to N2O
        /// </summary>
        private const double ConvertN2ONToN2O = 44.0 / 28.0;

        /// <summary>
        /// Converts from NO-N to NO
        /// </summary>
        private const double ConvertNONToNO = 30.0 / 14.0;

        /// <summary>
        /// Converts from NO3-N to NO3
        /// </summary>
        private const double ConvertNO3NToNO3 = 62.0 / 14.0;

        /// <summary>
        /// Converts from NH3-N to NH3
        /// </summary>
        private const double ConvertNH3NToNH3 = 17.0 / 14.0;

        /// <summary>
        /// Converts from N2N to N2
        /// </summary>
        private const double ConvertN2NToN2 = 14.0 / 28.0;

        /// <summary>
        /// Converts from NH3 to NH3-N
        /// </summary>
        private const double ConvertNH3ToNH3N = 14.0 / 17.0;

        private const double ConvertN2OToN = 28.0 / 44.0;

        private const double ConvertNH3ToN = 14.0 / 17.0;

        private const double ConvertCH4ToC = 12.0 / 16.0;

        #endregion

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

        /// <summary>
        /// (kg kg^-1)
        /// </summary>
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
        public static double N2OToCO2eConversionFactor = 273;

        /// <summary>
        /// Converts from C to CO2
        /// </summary>
        public static double ConvertFromCToCO2 = 44.0 / 12.0;

        public static string NotApplicableOutputString
        {
            get { return Resources.NotApplicable; } // Don't use a '-' character as it confuses when there are negative numbers in a grid cell
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Equation 2.6.9-27
        /// </summary>
        public static double ConvertToN2O(double amountOfN2ON)
        {
            return amountOfN2ON * ConvertN2ONToN2O;
        }

        /// <summary>
        /// Equation 2.6.9-28
        /// </summary>
        public static double ConvertToNO(double amountOfNON)
        {
            return amountOfNON * ConvertNONToNO;
        }

        /// <summary>
        /// Equation 2.6.9-29
        /// Equation 4.9.6-3 (verify)
        /// </summary>
        public static double ConvertToNO3(double amountOfNO3N)
        {
            return amountOfNO3N * ConvertNO3NToNO3;
        }

        /// <summary>
        /// Equation 2.6.9-30
        /// Equation 4.9.6-2
        /// </summary>
        public static double ConvertToNH3(double amountOfNH3N)
        {
            return amountOfNH3N * ConvertNH3NToNH3;
        }

        public static double ConvertToNH3N(double amountOfNH3)
        {
            return amountOfNH3 * ConvertNH3ToNH3N;
        }

        /// <summary>
        /// Equation 2.6.9-31
        /// Equation 4.8.6-1
        /// </summary>
        public static double ConvertToN(double amountOfN2O)
        {
            return amountOfN2O * ConvertN2OToN;
        }

        public static double ConvertToC(double amountOfCH4)
        {
            return amountOfCH4 * ConvertCH4ToC;
        }

        public static bool IsWetClimate(double precipitation, double evapotranspiration)
        {
            return precipitation >= evapotranspiration;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion
    }
}