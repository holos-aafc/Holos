using System;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    public class ManureExportViewItem : ManureItemBase
    {
        #region Fields

        private double _amount;
        private DateTime _dateOfExport;

        #endregion

        #region Constructors

        public ManureExportViewItem()
        {
            this.DateOfExport = DateTime.Now;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The amount of manure exported
        ///
        /// (kg)
        /// </summary>
        public double Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public DateTime DateOfExport
        {
            get => _dateOfExport;
            set => SetProperty(ref _dateOfExport, value);
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Amount)}: {Amount}, {nameof(DateOfExport)}: {DateOfExport}";
        }

        #endregion
    }
}