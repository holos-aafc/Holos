using System;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Fertilizer
{
    public class Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data : ModelBase
    {
        #region Fields

        private FertilizerBlends _fertilizerBlend;

        private DateTime _applicationDate;

        private double _percentageNitrogen;
        private double _percentagePhosphorus;
        private double _percentagePotassium;
        private double _percentageSulfur;
        private double _carbonDioxideEmissionsAtTheGate;
        private double _applicationEmissions;

        #endregion

        #region Contructors

        public Table_51_Carbon_Footprint_For_Fertilizer_Blends_Data()
        {
            this.ApplicationDate = DateTime.Now;
        }

        #endregion

        #region Properties

        public DateTime ApplicationDate
        {
            get => _applicationDate;
            set => SetProperty(ref _applicationDate, value);
        }

        /// <summary>
        /// Percentage of N contained in the blend
        /// 
        /// (%)
        /// </summary>
        public double PercentageNitrogen
        {
            get => _percentageNitrogen;
            set => SetProperty(ref _percentageNitrogen, value);
        }

        /// <summary>
        /// Percentage of P contained in the blend
        /// 
        /// (%)
        /// </summary>
        public double PercentagePhosphorus
        {
            get => _percentagePhosphorus;
            set => SetProperty(ref _percentagePhosphorus, value);
        }

        /// <summary>
        /// Percentage of K contained in the blend
        /// 
        /// (%)
        /// </summary>
        public double PercentagePotassium
        {
            get => _percentagePotassium;
            set => SetProperty(ref _percentagePotassium, value);
        }

        /// <summary>
        /// Percentage of S contained in the blend
        /// 
        /// (%)
        /// </summary>
        public double PercentageSulphur
        {
            get => _percentageSulfur;
            set => SetProperty(ref _percentageSulfur, value);
        }

        /// <summary>
        /// These are upstream emissions - not from farm
        /// 
        /// (kg CO2eq kg product^-1)
        /// </summary>
        public double CarbonDioxideEmissionsAtTheGate
        {
            get => _carbonDioxideEmissionsAtTheGate;
            set => SetProperty(ref _carbonDioxideEmissionsAtTheGate, value);
        }

        /// <summary>
        /// These are emission from the farm
        /// 
        /// (kg CO2eq kg product^-1)
        /// </summary>
        public double ApplicationEmissions
        {
            get => _applicationEmissions;
            set => SetProperty(ref _applicationEmissions, value);
        }

        public FertilizerBlends FertilizerBlend
        {
            get => _fertilizerBlend;
            set => SetProperty(ref _fertilizerBlend, value);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(ApplicationDate)}: {ApplicationDate}, {nameof(FertilizerBlend)}: {FertilizerBlend}, {nameof(PercentageNitrogen)}: {PercentageNitrogen}, {nameof(PercentagePhosphorus)}: {PercentagePhosphorus}, {nameof(PercentagePotassium)}: {PercentagePotassium}";
        }

        #endregion
    }
}