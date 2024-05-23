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