﻿using System.ComponentModel;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Providers.Fertilizer;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    public class FertilizerApplicationViewItem : ModelBase
    {
        #region Constructors

        public FertilizerApplicationViewItem()
        {
            FertilizerEfficiencyPercentage = 75;

            FertilizerBlendData = new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data();
            FertilizerBlendData.PropertyChanged += FertilizerBlendDataOnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Fields

        private FertilizerApplicationMethodologies _fertilizerApplicationMethodology;
        private Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data _carbonFootprintForFertilizerBlendsData;
        private Seasons _seasonOfApplication;


        private double _amountOfBlendedProductApplied;
        private double _amountOfNitrogenApplied;
        private double _amountOfPhosphorusApplied;
        private double _amountOfPotassiumApplied;
        private double _amountOfSulphurApplied;

        private double _fertilizerEfficiencyPercentage;

        #endregion

        #region Properties

        public FertilizerApplicationMethodologies FertilizerApplicationMethodology
        {
            get => _fertilizerApplicationMethodology;
            set => SetProperty(ref _fertilizerApplicationMethodology, value);
        }

        public Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data FertilizerBlendData
        {
            get => _carbonFootprintForFertilizerBlendsData;
            set => SetProperty(ref _carbonFootprintForFertilizerBlendsData, value);
        }

        public Seasons SeasonOfApplication
        {
            get => _seasonOfApplication;
            set => SetProperty(ref _seasonOfApplication, value);
        }

        /// <summary>
        ///     Amount of fertilizer applied
        ///     (kg product ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double AmountOfBlendedProductApplied
        {
            get => _amountOfBlendedProductApplied;
            set => SetProperty(ref _amountOfBlendedProductApplied, value);
        }

        /// <summary>
        ///     Amount of N applied based on the percentage composition of N in the fertilizer blend
        ///     (kg N ha^-1)
        /// </summary>
        public double AmountOfNitrogenApplied
        {
            get => _amountOfNitrogenApplied;
            set => SetProperty(ref _amountOfNitrogenApplied, value);
        }

        /// <summary>
        ///     Amount of P applied based on the percentage composition of P in the fertilizer blend
        ///     (kg P ha^-1)
        /// </summary>
        public double AmountOfPhosphorusApplied
        {
            get => _amountOfPhosphorusApplied;
            set => SetProperty(ref _amountOfPhosphorusApplied, value);
        }

        /// <summary>
        ///     Amount of K applied based on the percentage composition of K in the fertilizer blend
        ///     (kg K ha^-1)
        /// </summary>
        public double AmountOfPotassiumApplied
        {
            get => _amountOfPotassiumApplied;
            set => SetProperty(ref _amountOfPotassiumApplied, value);
        }

        /// <summary>
        ///     Amount of sulphur applied
        ///     (kg S ha^-1)
        /// </summary>
        public double AmountOfSulphurApplied
        {
            get => _amountOfSulphurApplied;
            set => SetProperty(ref _amountOfSulphurApplied, value);
        }

        /// <summary>
        ///     (%)
        /// </summary>
        public double FertilizerEfficiencyPercentage
        {
            get => _fertilizerEfficiencyPercentage;
            set => SetProperty(ref _fertilizerEfficiencyPercentage, value);
        }

        /// <summary>
        ///     (Fraction)
        /// </summary>
        public double FertilizerEfficiencyFraction => FertilizerEfficiencyPercentage / 100;

        #endregion

        #region Private Methods

        private void UpdateAmountOfNitrogenApplied()
        {
            AmountOfNitrogenApplied = FertilizerBlendData.PercentageNitrogen / 100 * AmountOfBlendedProductApplied;
        }

        private void UpdateAmountOfPhosphorusApplied()
        {
            AmountOfPhosphorusApplied = FertilizerBlendData.PercentagePhosphorus / 100 * AmountOfBlendedProductApplied;
        }

        private void UpdateAmountOfPotassiumApplied()
        {
            AmountOfPotassiumApplied = FertilizerBlendData.PercentagePotassium / 100 * AmountOfBlendedProductApplied;
        }

        private void UpdateAmountOfSulphurApplied()
        {
            AmountOfSulphurApplied = FertilizerBlendData.PercentageSulphur / 100 * AmountOfBlendedProductApplied;
        }

        #endregion

        #region Event Handlers

        private void FertilizerBlendDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data fertilizerBlendData)
            {
                if (e.PropertyName.Equals(nameof(FertilizerBlendData.PercentageNitrogen)))
                    // When the percentage of N is changed, the amount of nitrogen applied from the amount of product applied will change
                    UpdateAmountOfNitrogenApplied();

                if (e.PropertyName.Equals(nameof(FertilizerBlendData.PercentagePhosphorus)))
                    // When the percentage of P is changed, the amount of phosphorus applied from the amount of product applied will change
                    UpdateAmountOfPhosphorusApplied();

                if (e.PropertyName.Equals(nameof(FertilizerBlendData.PercentagePotassium)))
                    // When the percentage of K is changed, the amount of potassium applied from the amount of product applied will change
                    UpdateAmountOfPotassiumApplied();

                if (e.PropertyName.Equals(nameof(FertilizerBlendData.PercentageSulphur)))
                    // When the percentage of S is changed, the amount of sulphur applied from the amount of product applied will change
                    UpdateAmountOfSulphurApplied();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(AmountOfBlendedProductApplied)))
            {
                UpdateAmountOfNitrogenApplied();
                UpdateAmountOfPhosphorusApplied();
                UpdateAmountOfPotassiumApplied();
                UpdateAmountOfSulphurApplied();
            }
        }

        #endregion
    }
}