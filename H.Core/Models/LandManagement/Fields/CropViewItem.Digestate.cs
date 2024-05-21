using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using H.Core.Providers.Climate;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Fields

        private ObservableCollection<DigestateApplicationViewItem> _digestateApplicationViewItems;

        private double _digestateCarbonInputsPerHectare;

        private BiogasAndMethaneProductionParametersData _biogasAndMethaneProductionParametersData;

        #endregion

        #region Properties

        /// <summary>
        /// (kg C ha^-1)
        ///
        /// Total digestate C from all digestate applications
        /// </summary>
        public double DigestateCarbonInputsPerHectare
        {
            get { return _digestateCarbonInputsPerHectare;}
            set { SetProperty(ref _digestateCarbonInputsPerHectare, value); }
        }

        public ObservableCollection<DigestateApplicationViewItem> DigestateApplicationViewItems
        {
            get => _digestateApplicationViewItems;
            set => SetProperty(ref _digestateApplicationViewItems, value);
        }

        public bool HasDigestateApplications { get; set; }

        public BiogasAndMethaneProductionParametersData BiogasAndMethaneProductionParametersData
        {
            get => _biogasAndMethaneProductionParametersData;
            set => SetProperty(ref _biogasAndMethaneProductionParametersData, value);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the fraction of digestate used (unitless)
        /// </summary>
        public double GetFractionOfDigestateUsed(DigestateApplicationViewItem applicationViewItem)
        {
            var totalApplied = applicationViewItem.AmountAppliedPerHectare * this.Area;

            var totalDigestateProducedBySystem = 0d;
            if (this.DigestateApplicationViewItems.Any())
            {
                totalDigestateProducedBySystem = this.DigestateApplicationViewItems.First().TotalDigestateProducedBySystem;
            }
            else
            {
                totalDigestateProducedBySystem = 0d;
            }

            var result = 0d;
            if (totalDigestateProducedBySystem > 0)
            {
                result = totalApplied / totalDigestateProducedBySystem;
            }

            return result;
        }

        public double GetRemainingNitrogenFromDigestateAtEndOfYear()
        {
            var itemsByYear = this.DigestateApplicationViewItems.Where(x => x.DateCreated.Year == this.Year);
            if (itemsByYear.Any())
            {
                // All digestate view items have the amount of digestate remaining at end of year when detail view items are created. Since all items from
                // the same year will have the same value for amount remaining, we return the amount from the first item.
                var firstItem = itemsByYear.First();
                var amount = itemsByYear.First().AmountOfNitrogenRemainingAtEndOfYear;
                if (amount >= 0)
                {
                    return amount;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region Event Handlers

        private void DigestateApplicationViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.HasDigestateApplications = this.DigestateApplicationViewItems.Count > 0;
            this.RaisePropertyChanged(nameof(this.HasDigestateApplications));
        }

        #endregion
    }
}