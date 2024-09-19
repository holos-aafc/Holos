using System;
using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Models.Results
{
    public class ManureExportResultViewItem : ResultsViewItemBase
    {
        #region Fields

        private DateTime _dateOfExport;
        private double _n2ON;

        #endregion

        #region Propeties
        
        public DateTime DateOfExport
        {
            get => _dateOfExport;
            set => SetProperty(ref _dateOfExport, value);
        }

        /// <summary>
        /// (kg N2O-N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsN2ONPerHectare)]
        public double N2ON
        {
            get => _n2ON;
            set => SetProperty(ref _n2ON, value);
        }

        #endregion
    }
}