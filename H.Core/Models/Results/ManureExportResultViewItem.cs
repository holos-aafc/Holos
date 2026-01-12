using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Models.Results
{
    public class ManureExportResultViewItem : ExportResultViewItemBase
    {
        #region Fields

        private double _nitrateLeachedEmissions;
        private double _volatilizationEmissions;
        private double _adjustedVolatilizationEmissions;

        #endregion

        #region Properties


        /// <summary>
        /// Direct N2O-N from exported manure
        /// 
        /// (kg NO3-N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsNO3NPerHectare)]
        public double NitrateLeachedEmissions
        {
            get => _nitrateLeachedEmissions;
            set => SetProperty(ref _nitrateLeachedEmissions, value);
        }

        /// <summary>
        /// NH3-N from exported manure
        ///
        /// (kg NH3-N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsNH3NPerHectare)]
        public double VolatilizationEmissions
        {
            get => _volatilizationEmissions;
            set => SetProperty(ref _volatilizationEmissions, value);
        }

        /// <summary>
        /// Adjusted NH3-N from exported manure
        ///
        /// (kg NH3-N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsNH3NPerHectare)]
        public double AdjustedVolatilizationEmissions
        {
            get => _adjustedVolatilizationEmissions;
            set => SetProperty(ref _adjustedVolatilizationEmissions, value);
        }

        #endregion
    }
}