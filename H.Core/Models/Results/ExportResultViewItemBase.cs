using System;
using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Models.Results
{
    /// <summary>
    ///     A base class for <see cref="ManureExportResultViewItem" />s and <see cref="CropResidueExportResultViewItem" />s.
    /// </summary>
    public abstract class ExportResultViewItemBase : ResultsViewItemBase
    {
        #region Fields

        private DateTime _dateOfExport;
        private double _directN2ON;
        private double _indirectN2ON;

        #endregion

        #region Propeties

        public DateTime DateOfExport
        {
            get => _dateOfExport;
            set => SetProperty(ref _dateOfExport, value);
        }

        /// <summary>
        ///     Direct N2O-N from exports
        ///     (kg N2O-N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsN2ONPerHectare)]
        public double DirectN2ON
        {
            get => _directN2ON;
            set => SetProperty(ref _directN2ON, value);
        }

        /// <summary>
        ///     Indirect N2O-N from exports
        ///     (kg N2O-N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsN2ONPerHectare)]
        public double IndirectN2ON
        {
            get => _indirectN2ON;
            set => SetProperty(ref _indirectN2ON, value);
        }

        #endregion
    }
}