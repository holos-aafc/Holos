using System;
using System.ComponentModel;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Providers.Fertilizer;
using H.Infrastructure;

namespace H.Core.Models.LandManagement.Fields
{
    public class FertilizerApplicationViewItem : ModelBase
    {
        #region Fields

        private FertilizerApplicationMethodologies _fertilizerApplicationMethodology;
        private Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data _carbonFootprintForFertilizerBlendsData;
        private Seasons _seasonOfApplication;


        private double _amountOfBlendedProductApplied;
        private double _amountOfNitrogenApplied;
        private double _amountOfPhosphorusApplied;
        private double _amountOfPotassiumApplied;
        private double _amountOfSulphurApplied;

        private double _fertilizerEfficiencyPercentage;

        #endregion

        #region Constructors

        public FertilizerApplicationViewItem()
        {
            this.FertilizerEfficiencyPercentage = 75;

            this.FertilizerBlendData = new Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data();
            this.FertilizerBlendData.PropertyChanged += FertilizerBlendDataOnPropertyChanged;
            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        public FertilizerApplicationMethodologies FertilizerApplicationMethodology
        {
            get => _fertilizerApplicationMethodology;
            set => SetProperty(ref _fertilizerApplicationMethodology, value);
        }

        public Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data FertilizerBlendData
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
        /// Amount of fertilizer applied
        ///
        /// (kg product ha^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramsPerHectare)]
        public double AmountOfBlendedProductApplied
        {
            get => _amountOfBlendedProductApplied;
            set => SetProperty(ref _amountOfBlendedProductApplied, value);
        }

        /// <summary>
        /// Amount of N applied based on the percentage composition of N in the fertilizer blend
        ///
        /// (kg N ha^-1)
        /// </summary>
        public double AmountOfNitrogenApplied
        {
            get => _amountOfNitrogenApplied;
            set => SetProperty(ref _amountOfNitrogenApplied, value);
        }

        /// <summary>
        /// Amount of P applied based on the percentage composition of P in the fertilizer blend
        ///
        /// (kg P ha^-1)
        /// </summary>
        public double AmountOfPhosphorusApplied
        {
            get => _amountOfPhosphorusApplied;
            set => SetProperty(ref _amountOfPhosphorusApplied, value);
        }

        /// <summary>
        /// Amount of K applied based on the percentage composition of K in the fertilizer blend
        ///
        /// (kg K ha^-1)
        /// </summary>
        public double AmountOfPotassiumApplied
        {
            get => _amountOfPotassiumApplied;
            set => SetProperty(ref _amountOfPotassiumApplied, value);
        }

        /// <summary>
        /// Amount of sulphur applied
        ///
        /// (kg S ha^-1)
        /// </summary>
        public double AmountOfSulphurApplied
        {
            get => _amountOfSulphurApplied;
            set => SetProperty(ref _amountOfSulphurApplied, value);
        }

        /// <summary>
        /// (%)
        /// </summary>
        public double FertilizerEfficiencyPercentage
        {
            get => _fertilizerEfficiencyPercentage;
            set => SetProperty(ref _fertilizerEfficiencyPercentage, value);
        }

        /// <summary>
        /// (Fraction)
        /// </summary>
        public double FertilizerEfficiencyFraction
        {
            get => this.FertilizerEfficiencyPercentage / 100;
        }

        #endregion

        #region Private Methods

        private void UpdateAmountOfNitrogenApplied()
        {
            this.AmountOfNitrogenApplied = (this.FertilizerBlendData.PercentageNitrogen / 100) * this.AmountOfBlendedProductApplied;
        }

        private void UpdateAmountOfPhosphorusApplied()
        {
            this.AmountOfPhosphorusApplied = (this.FertilizerBlendData.PercentagePhosphorus / 100) * this.AmountOfBlendedProductApplied;
        }

        private void UpdateAmountOfPotassiumApplied()
        {
            this.AmountOfPotassiumApplied = (this.FertilizerBlendData.PercentagePotassium / 100) * this.AmountOfBlendedProductApplied;
        }

        private void UpdateAmountOfSulphurApplied()
        {
            this.AmountOfSulphurApplied = (this.FertilizerBlendData.PercentageSulphur / 100) * this.AmountOfBlendedProductApplied;
        }

        #endregion

        #region Event Handlers

        private void FertilizerBlendDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data fertilizerBlendData)
            {
                if (e.PropertyName.Equals(nameof(FertilizerBlendData.PercentageNitrogen)))
                {
                    // When the percentage of N is changed, the amount of nitrogen applied from the amount of product applied will change
                    this.UpdateAmountOfNitrogenApplied();
                }

                if (e.PropertyName.Equals(nameof(FertilizerBlendData.PercentagePhosphorus)))
                {
                    // When the percentage of P is changed, the amount of phosphorus applied from the amount of product applied will change
                    this.UpdateAmountOfPhosphorusApplied();
                }

                if (e.PropertyName.Equals(nameof(FertilizerBlendData.PercentagePotassium)))
                {
                    // When the percentage of K is changed, the amount of potassium applied from the amount of product applied will change
                    this.UpdateAmountOfPotassiumApplied();
                }

                if (e.PropertyName.Equals(nameof(FertilizerBlendData.PercentageSulphur)))
                {
                    // When the percentage of S is changed, the amount of sulphur applied from the amount of product applied will change
                    this.UpdateAmountOfSulphurApplied();
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(AmountOfBlendedProductApplied)))
            {
                this.UpdateAmountOfNitrogenApplied();
                this.UpdateAmountOfPhosphorusApplied();
                this.UpdateAmountOfPotassiumApplied();
                this.UpdateAmountOfSulphurApplied();
            }
        }

        #endregion
    }
}