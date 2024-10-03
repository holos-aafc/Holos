using System;
using System.ComponentModel;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// Used on fields in the dry season to add forage for animals grazing on a field
    /// </summary>
    public class HayImportViewItem : BaleActivityBase
    {
        #region Fields

        private DateTime _date;
        private ResourceSourceLocation _sourceOfBales;
        private int _numberOfBales;
        private Guid _fieldSourceGuid;

        #endregion

        #region Constructors

        public HayImportViewItem()
        {
            this.Date = DateTime.Now;

            this.PropertyChanged -= OnPropertyChanged;
            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The date the hay was added to the field
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        /// <summary>
        /// User can specify a number of bales
        /// </summary>
        public int NumberOfBales
        {
            get => _numberOfBales;
            set => SetProperty(ref _numberOfBales, value);
        }

        /// <summary>
        /// Indicates if the bales where from the farm or had to be imported from off site
        /// </summary>
        public ResourceSourceLocation SourceOfBales
        {
            get => _sourceOfBales;
            set => SetProperty(ref _sourceOfBales, value);
        }

        /// <summary>
        /// The field from where these bases came from (if sourced on farm).
        /// </summary>
        public Guid FieldSourceGuid
        {
            get => _fieldSourceGuid;
            set => SetProperty(ref _fieldSourceGuid, value);
        }

        #endregion

        #region Public Methods

        public double GetTotalDryMatterWeightOfAllBales()
        {
            return (this.BaleWeight * (1 - (this.MoistureContentAsPercentage / 100.0))) * this.NumberOfBales;
        }

        public double GetTotalWetWeightOfAllBales()
        {
            return this.GetTotalDryMatterWeightOfAllBales() * (1 + (this.MoistureContentAsPercentage / 100.0));
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(BaleWeight)) || e.PropertyName.Equals(nameof(MoistureContentAsPercentage)) || e.PropertyName.Equals(nameof(NumberOfBales)))
            {
                base.AboveGroundBiomassDryWeight = this.GetTotalWetWeightOfAllBales();
            }
        }

        #endregion
    }
}