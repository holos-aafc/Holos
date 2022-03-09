using System;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// A class to hold harvest information so total yield calculations can be made.
    /// </summary>
    public class HarvestViewItem : FieldActivityBase
    {
        #region Fields

        private int _totalNumberOfBalesHarvested;
        private int _totalNumberOfBalesExported;

        private double _baleWeight;
        private double _harvestLossPercentage;

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

            this.HarvestLossPercentage = 35;
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
        /// The total weight of each bale. This is the wet bale weight.
        ///
        /// (kg)
        /// </summary>
        public double BaleWeight
        {
            get => _baleWeight;
            set => SetProperty(ref _baleWeight, value, OnBaleWeightChanged);
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

        /// <summary>
        /// Allows for accounting of bales that have been used for supplemental feeding or exporting
        /// </summary>
        public int TotalNumberOfBalesExported
        {
            get => _totalNumberOfBalesExported;
            set => SetProperty(ref _totalNumberOfBalesExported, value);
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

        private void CalculateYield()
        {
            this.AboveGroundBiomass = this.TotalNumberOfBalesHarvested * this.BaleWeight;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Need to update the total yield for the field if the number of bales changes.
        /// </summary>
        private void OnTotalNumberOFBalesHarvestedChanged()
        {
            this.CalculateYield();
        }

        /// <summary>
        /// Need to update the total yield for the field if the weight of the bales changes.
        /// </summary>
        private void OnBaleWeightChanged()
        {
            this.CalculateYield();
        }

        #endregion
    }
}