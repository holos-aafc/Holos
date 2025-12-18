using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Climate;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Event Handlers

        private void DigestateApplicationViewItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasDigestateApplications = DigestateApplicationViewItems.Count > 0;
            RaisePropertyChanged(nameof(HasDigestateApplications));
        }

        #endregion

        #region Fields

        private ObservableCollection<DigestateApplicationViewItem> _digestateApplicationViewItems;

        private double _digestateCarbonInputsPerHectare;
        private double _digestateCarbonInputsPerHectareFromApplicationsOnly;

        private BiogasAndMethaneProductionParametersData _biogasAndMethaneProductionParametersData;

        #endregion

        #region Properties

        /// <summary>
        ///     (kg C ha^-1)
        ///     Total digestate C from all digestate applications, remaining digestate, etc.
        /// </summary>
        public double DigestateCarbonInputsPerHectare
        {
            get => _digestateCarbonInputsPerHectare;
            set => SetProperty(ref _digestateCarbonInputsPerHectare, value);
        }

        /// <summary>
        ///     (kg C ha^-1)
        ///     Total digestate C from all digestate applications. Does not inlcude remaining amounts
        /// </summary>
        public double DigestateCarbonInputsPerHectareFromApplicationsOnly
        {
            get => _digestateCarbonInputsPerHectareFromApplicationsOnly;
            set => SetProperty(ref _digestateCarbonInputsPerHectareFromApplicationsOnly, value);
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
        ///     (kg C year^-1)
        /// </summary>
        public double GetTotalCarbonFromAppliedDigestate(ManureLocationSourceType location)
        {
            var digestateApplications = GetDigestateApplicationsInYear(location);
            var result = CalculateTotalCarbonFromDigestateApplications(digestateApplications);

            return result;
        }

        /// <summary>
        ///     Equation 4.7.1-1
        ///     Equation 4.7.1-2
        ///     (kg C year^-1)
        /// </summary>
        public double CalculateTotalCarbonFromDigestateApplications(
            IEnumerable<DigestateApplicationViewItem> manureApplicationViewItems)
        {
            var result = 0d;

            foreach (var manureApplication in manureApplicationViewItems)
            {
                var amountOfCarbonAppliedPerHectare = manureApplication.AmountOfCarbonAppliedPerHectare;
                var area = Area;

                result += amountOfCarbonAppliedPerHectare * area;
            }

            return result;
        }

        public IEnumerable<DigestateApplicationViewItem> GetDigestateApplicationsInYear(
            ManureLocationSourceType locationSourceType)
        {
            return DigestateApplicationViewItems.Where(digestateApplicationViewItem =>
                digestateApplicationViewItem.ManureLocationSourceType == locationSourceType &&
                digestateApplicationViewItem.DateCreated.Year == Year);
        }

        public IEnumerable<DigestateApplicationViewItem> GetDigestateApplicationViewItems(DateTime date,
            DigestateState state,
            ManureLocationSourceType source)
        {
            return DigestateApplicationViewItems.Where(x =>
                x.DateCreated.Date == date && x.DigestateState == state && x.ManureLocationSourceType == source);
        }

        #endregion
    }
}