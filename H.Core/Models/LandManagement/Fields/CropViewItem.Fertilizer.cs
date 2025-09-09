﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Models.LandManagement.Fields
{
    public partial class CropViewItem
    {
        #region Private Methods

        /// <summary>
        ///     Sums up the total individual fertilizer component (N-P-K-S) application rates for the entire year.
        /// </summary>
        public void UpdateApplicationRateTotals()
        {
            // Update total N fertilizer rate from all fertilizer applications
            NitrogenFertilizerRate = FertilizerApplicationViewItems.Sum(viewItem => viewItem.AmountOfNitrogenApplied);

            // Update total P fertilizer rate from all fertilizer applications
            PhosphorusFertilizerRate =
                FertilizerApplicationViewItems.Sum(viewItem => viewItem.AmountOfPhosphorusApplied);

            // Update total K fertilizer rate from all fertilizer applications
            PotassiumFertilizerRate = FertilizerApplicationViewItems.Sum(viewItem => viewItem.AmountOfPotassiumApplied);

            // Update total S fertilizer rate from all fertilizer applications
            SulphurFertilizerRate = FertilizerApplicationViewItems.Sum(viewItem => viewItem.AmountOfSulphurApplied);
        }

        #endregion

        #region Fields

        private double _nitrogenFertilizerRate;
        private double _phosphorusFertilizerRate;
        private double _potassiumFertilizerRate;
        private double _sulphurFertilizerRate;
        private double _customReductionFactor;

        private SoilReductionFactors _soilReductionFactor;
        private FertilizerBlends _fertilizerBlend;

        private ObservableCollection<FertilizerApplicationViewItem> _fertilizerApplicationViewItems;
        public double NO3NFromMineralizationLeaching;
        public double NH4FromSyntheticNitogenVolatilized;

        #endregion

        #region Properties

        public SoilReductionFactors SoilReductionFactor
        {
            get => _soilReductionFactor;
            set => SetProperty(ref _soilReductionFactor, value);
        }

        public bool HasFertilizerApplicationViewItems => FertilizerApplicationViewItems.Count > 0;

        /// <summary>
        ///     A collection to hold information about each fertilizer application the user applies to a field/crop.
        /// </summary>
        public ObservableCollection<FertilizerApplicationViewItem> FertilizerApplicationViewItems
        {
            get => _fertilizerApplicationViewItems;
            set => SetProperty(ref _fertilizerApplicationViewItems, value);
        }

        /// <summary>
        ///     The total amount of nitrogen fertilizer applied to a field
        ///     (kg N ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsNitrogenPerHectare)]
        public double NitrogenFertilizerRate
        {
            get => _nitrogenFertilizerRate > 0 ? _nitrogenFertilizerRate : 0;
            set => SetProperty(ref _nitrogenFertilizerRate, value);
        }

        /// <summary>
        ///     The total amount of phosphorus fertilizer applied to a field
        ///     (kg P ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPhosphorousPerHectare)]
        public double PhosphorusFertilizerRate
        {
            get => _phosphorusFertilizerRate > 0 ? _phosphorusFertilizerRate : 0;
            set => SetProperty(ref _phosphorusFertilizerRate, value);
        }

        /// <summary>
        ///     The total amount of potassium fertilizer applied to a field
        ///     (kg K ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPotassiumPerHectare)]
        public double PotassiumFertilizerRate
        {
            get => _potassiumFertilizerRate;
            set => SetProperty(ref _potassiumFertilizerRate, value);
        }

        /// <summary>
        ///     The total amount of sulphur fertilizer applied to a field
        ///     (kg S ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsSulphurPerHectare)]
        public double SulphurFertilizerRate
        {
            get => _sulphurFertilizerRate;
            set => SetProperty(ref _sulphurFertilizerRate, value);
        }

        public double CustomReductionFactor
        {
            get => _customReductionFactor;
            set => SetProperty(ref _customReductionFactor, value);
        }

        /// <summary>
        ///     Used by CLI to indicate which blend is used. Currently only one blend can be specified when using CLI. Amount of
        ///     blend
        ///     is taken from value in <see cref="NitrogenFertilizerRate" />
        /// </summary>
        public FertilizerBlends FertilizerBlend
        {
            get => _fertilizerBlend;
            set => SetProperty(ref _fertilizerBlend, value);
        }

        #endregion

        #region Event Handlers

        private void FertilizerApplicationViewItemsOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasFertilizerApplicationViewItems));

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems[0] is FertilizerApplicationViewItem addedItem)
                    addedItem.PropertyChanged += FertilizerApplicationViewItemOnPropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // When a user removes a fertilizer application we have to update total application rates
                UpdateApplicationRateTotals();
            }
        }

        private void FertilizerApplicationViewItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is FertilizerApplicationViewItem fertilizerApplicationViewItem)
                if (e.PropertyName.Equals(nameof(FertilizerApplicationViewItem.AmountOfNitrogenApplied)) ||
                    e.PropertyName.Equals(nameof(FertilizerApplicationViewItem.AmountOfPotassiumApplied)) ||
                    e.PropertyName.Equals(nameof(FertilizerApplicationViewItem.AmountOfSulphurApplied)) ||
                    e.PropertyName.Equals(nameof(FertilizerApplicationViewItem.AmountOfPhosphorusApplied)))
                    // When the component amounts of the fertilizer blend changes, we need to update the total application rates for the year (sum up individual rates)
                    UpdateApplicationRateTotals();
        }

        #endregion
    }
}