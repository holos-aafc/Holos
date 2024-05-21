using System;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.Infrastructure
{
    public class SubstrateViewItemBase : ModelBase
    {
        #region Fields
        
        private double _biomethanePotential;
        private double _methaneFraction;
        private double _volatileSolids;
        private double _totalSolids;
        private double _totalNitrogen;
        private double _flowRate;
        private double _totalCarbon;
        private double _organicNitrogenConcentration;
        private double _tan;
        private DateTime _startDate;
        private DateTime _endDate;
        private ManureLocationSourceType _sourceType;

        #endregion

        #region Constructors

        public SubstrateViewItemBase()
        {
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now.AddDays(1);
        }

        #endregion

        #region Properties

        /// <summary>
        /// (Nm^3 kg VS^-1)
        /// </summary>
        public double BiomethanePotential
        {
            get => _biomethanePotential;
            set => this.SetProperty(ref _biomethanePotential, value);
        }

        public double MethaneFraction
        {
            get => _methaneFraction;
            set => this.SetProperty(ref _methaneFraction, value);
        }

        /// <summary>
        /// (kg kg^-1)
        /// </summary>
        public double VolatileSolids
        {
            get => _volatileSolids;
            set => this.SetProperty(ref _volatileSolids, value);
        }

        /// <summary>
        /// This is the VS content of the substrate - not the same as the <see cref="VolatileSolids"/> which is the flow rate of the VS as entered by the user
        ///
        /// (fraction wet weight)
        /// </summary>
        public double VolatileSolidsContent { get; set; }

        /// <summary>
        /// (fraction wet weight)
        /// </summary>
        public double NitrogenContent { get; set; }

        /// <summary>
        /// (fraction wet weight)
        /// </summary>
        public double CarbonContent { get; set; }

        /// <summary>
        /// (kg kg^-1)
        /// </summary>
        public double TotalSolids
        {
            get => _totalSolids;
            set => this.SetProperty(ref _totalSolids, value);
        }

        /// <summary>
        /// (kg kg^-1)
        /// </summary>
        public double TotalNitrogen
        {
            get => _totalNitrogen;
            set => this.SetProperty(ref _totalNitrogen, value);
        }

        /// <summary>
        /// (kg day^-1)
        /// </summary>
        public double FlowRate
        {
            get => _flowRate;
            set => SetProperty(ref _flowRate, value);
        }

        public double OrganicNitrogenConcentration
        {
            get => _organicNitrogenConcentration;
            set => SetProperty(ref _organicNitrogenConcentration, value);
        }

        public double Tan
        {
            get => _tan;
            set => SetProperty(ref _tan, value);
        }

        /// <summary>
        /// (kg kg^-1)
        /// </summary>
        public double TotalCarbon
        {
            get => _totalCarbon;
            set => SetProperty(ref _totalCarbon, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        /// <summary>
        /// The source of the digestate material (local/imported)
        /// </summary>
        public ManureLocationSourceType SourceType
        {
            get => _sourceType;
            set => SetProperty(ref _sourceType, value);
        }

        #endregion
    }
}