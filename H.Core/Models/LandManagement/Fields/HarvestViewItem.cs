using System;
using System.ComponentModel;
using H.Core.Enumerations;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    ///     A class to hold harvest information so total yield calculations can be made.
    /// </summary>
    public class HarvestViewItem : BaleActivityBase
    {
        #region Constructors

        public HarvestViewItem()
        {
            Start = new DateTime(DateTime.Now.Year - 1, 8, 31);
            End = new DateTime(DateTime.Now.Year - 1, 8, 31);
            ForageActivity = ForageActivities.Hayed;

            // https://hayforks.com/blog/how-much-does-a-bale-of-hay-weigh
            BaleWeight = 500;

            PropertyChanged -= OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Public Methods

        public bool BaleHasExpiredLifespan()
        {
            var dateHarvested = Start;
            var elapsedTime = DateTime.Now.Subtract(dateHarvested);

            // Bales of hay last for 5 years, straw bales last for 2 years.
            if (ForageActivity == ForageActivities.Hayed) return elapsedTime.Days > 5 * 365;

            return elapsedTime.Days > 2 * 365;
        }

        #endregion

        #region Fields

        private int _totalNumberOfBalesHarvested;
        private double _harvestLossPercentage;

        private Guid _fieldGuid;

        #endregion

        #region Properties

        /// <summary>
        ///     The total number of bales that were created from the harvest.
        /// </summary>
        public int TotalNumberOfBalesHarvested
        {
            get => _totalNumberOfBalesHarvested;
            set => SetProperty(ref _totalNumberOfBalesHarvested, value, OnTotalNumberOFBalesHarvestedChanged);
        }

        /// <summary>
        ///     The percentage of the yield lost during harvest
        ///     (%)
        /// </summary>
        public double HarvestLossPercentage
        {
            get => _harvestLossPercentage;
            set => SetProperty(ref _harvestLossPercentage, value);
        }

        /// <summary>
        ///     The <see cref="FieldSystemComponent" /> GUID that produced this harvest.
        /// </summary>
        public Guid FieldGuid
        {
            get => _fieldGuid;
            set => SetProperty(ref _fieldGuid, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Returns the wet weight of all bales harvested
        /// </summary>
        private void CalculateYield()
        {
            AboveGroundBiomass = TotalNumberOfBalesHarvested * BaleWeight;
        }

        private void CalculateDryWeightYield()
        {
            AboveGroundBiomassDryWeight =
                TotalNumberOfBalesHarvested * BaleWeight * (1 - MoistureContentAsPercentage / 100.0);
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Need to update the total yield for the field if the weight of the bales changes.
            if (e.PropertyName.Equals(nameof(MoistureContentAsPercentage)) || e.PropertyName.Equals(nameof(BaleWeight)))
            {
                CalculateYield();
                CalculateDryWeightYield();
            }
        }

        /// <summary>
        ///     Need to update the total yield for the field if the number of bales changes.
        /// </summary>
        private void OnTotalNumberOFBalesHarvestedChanged()
        {
            CalculateYield();
            CalculateDryWeightYield();
        }

        #endregion
    }
}