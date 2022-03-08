using System;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// Used on fields in the dry season to add forage for animals grazing on a field
    /// </summary>
    public class HayImportViewItem : FieldActivityBase
    {
        #region Fields

        private DateTime _date;
        private ResourceSourceLocation _sourceOfBales;
        private double _amount;
        private int _numberOfBales;
        private double _moistureContentAsPercentage;
        private double _baleWeight;
        private Guid _fieldSourceGuid;

        #endregion

        #region Constructors

        public HayImportViewItem()
        {
            this.Date = DateTime.Now;
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
        /// User can specify a dry matter amount
        /// </summary>
        public double Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
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
        /// When user imports bales, we need bale weight (wet weight)
        /// 
        /// (kg)
        /// </summary>
        public double BaleWeight
        {
            get => _baleWeight;
            set => SetProperty(ref _baleWeight, value);
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

        /// <summary>
        /// Equation 12.1.1-1
        /// </summary>
        public double GetTotalDryMatterWeightOfAllBales()
        {
            return (this.BaleWeight * (1 - (this.MoistureContentAsPercentage / 100))) * this.NumberOfBales;
        }

        /// <summary>
        /// Equation 12.1.1-2
        /// </summary>
        public double GetTotalWetWeightOfAllBales()
        {
            return this.GetTotalDryMatterWeightOfAllBales() * (1 + (this.MoistureContentAsPercentage / 100));
        }

        #endregion
    }
}