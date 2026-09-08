using System;
using System.ComponentModel;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// A class to hold harvest information so total yield calculations can be made.
    /// </summary>
    public class HarvestViewItem : BaleActivityBase, IRepeatableFieldActivity
    {
        #region Fields

        private int _totalNumberOfBalesHarvested;
        private double _harvestLossPercentage;
        private bool _repeatsInEveryYear;

        private Guid _fieldGuid;

        #endregion

        #region Constructors

        public HarvestViewItem()
        {
            base.Start = new DateTime(DateTime.Now.Year - 1, 8, 31);
            base.End = new DateTime(DateTime.Now.Year - 1, 8, 31);
            base.ForageActivity = ForageActivities.Hayed;

            // https://hayforks.com/blog/how-much-does-a-bale-of-hay-weigh
            this.BaleWeight = 500;

            // Repeating the entered management across the simulation is the algorithm document's model - the historical
            // period is built from the rotation the user specifies once - so this is the default, and "this year only"
            // is the exception a user chooses for a cut that happened in a single year. Farms saved before this property existed deserialize
            // without it and so keep this default, which is deliberate: they get the corrected behaviour.
            this.RepeatsInEveryYear = true;

            base.PropertyChanged -= OnPropertyChanged;
            base.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The total number of bales that were created from the harvest.
        /// </summary>
        public int TotalNumberOfBalesHarvested
        {
            get => _totalNumberOfBalesHarvested;
            set => SetProperty(ref _totalNumberOfBalesHarvested, value, OnTotalNumberOFBalesHarvestedChanged);
        }

        /// <summary>
        /// Whether this harvest describes management that recurs whenever the crop is grown, and so is carried into
        /// every year of the simulation, or a cut that happened in a single year only.
        /// </summary>
        public bool RepeatsInEveryYear
        {
            get => _repeatsInEveryYear;
            set => SetProperty(ref _repeatsInEveryYear, value);
        }

        /// <summary>
        /// The percentage of the yield lost during harvest
        ///
        /// (%)
        /// </summary>
        public double HarvestLossPercentage
        {
            get => _harvestLossPercentage;
            set => SetProperty(ref _harvestLossPercentage, value);
        }

        /// <summary>
        /// The <see cref="FieldSystemComponent"/> GUID that produced this harvest.
        /// </summary>
        public Guid FieldGuid
        {
            get => _fieldGuid;
            set => SetProperty(ref _fieldGuid, value);
        }

        #endregion

        #region Public Methods

        public bool BaleHasExpiredLifespan()
        {
            var dateHarvested = this.Start;
            var elapsedTime = DateTime.Now.Subtract(dateHarvested);

            // Bales of hay last for 5 years, straw bales last for 2 years.
            if (this.ForageActivity == ForageActivities.Hayed)
            {
                return elapsedTime.Days > (5 * 365);
            }
            else
            {
                return elapsedTime.Days > (2 * 365);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the wet weight of all bales harvested
        /// </summary>
        private void CalculateYield()
        {
            this.AboveGroundBiomass = this.TotalNumberOfBalesHarvested * this.BaleWeight;
        }

        private void CalculateDryWeightYield()
        {
            this.AboveGroundBiomassDryWeight = this.TotalNumberOfBalesHarvested * this.BaleWeight * (1 - (MoistureContentAsPercentage / 100.0));
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Need to update the total yield for the field if the weight of the bales changes.
            if (e.PropertyName.Equals(nameof(MoistureContentAsPercentage)) || e.PropertyName.Equals(nameof(BaleWeight)))
            {
                this.CalculateYield();
                this.CalculateDryWeightYield();
            }
        }

        /// <summary>
        /// Need to update the total yield for the field if the number of bales changes.
        /// </summary>
        private void OnTotalNumberOFBalesHarvestedChanged()
        {
            this.CalculateYield();
            this.CalculateDryWeightYield();
        }

        #endregion
    }
}