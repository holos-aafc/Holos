using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    /// <summary>
    /// A class to describe the various concentrations of N, C, and P found in manure. Nitrogen concentrations will be in (kg N (1000 L)^-1) for both
    /// solid and liquid manure. Class represents one row from table 9.
    /// </summary>
    public class DefaultManureCompositionData : ModelBase
    {
        #region Fields

        private double _nitrogenFraction;
        private double _carbonFraction;
        private double _phosphorusFraction;

        private double _nitrogenContent;
        private double _carbonContent;
        private double _phosphorusContent;

        private double _moistureContent;
        private double _carbonToNitrogenRatio;
        private double _nitrogenConcentrationOfManure;
        private double _volatileSolidsContent;

        private ManureStateType _manureStateType;
        private AnimalType _animalType;

        #endregion

        #region Properties

        public ManureStateType ManureStateType
        {
            get => _manureStateType;
            set => SetProperty(ref _manureStateType, value);
        }

        public string ManureStateTypeString => this.ManureStateType.GetDescription();

        /// <summary>
        /// %
        /// </summary>
        public double MoistureContent 
        {
            get => _moistureContent;
            set => SetProperty(ref _moistureContent, value);
        }

        /// <summary>
        /// (% wet weight)
        ///
        /// Expressed as a percentage
        ///
        /// TODO: rename this to NitrogenPercentage
        /// </summary>
        public double NitrogenFraction 
        {
            get => _nitrogenFraction;
            set => SetProperty(ref _nitrogenFraction, value, () => { this.NitrogenContent = value / 100.0;});
        }

        /// <summary>
        /// Proportion of N in manure (fraction)
        /// </summary>
        public double NitrogenContent
        {
            get => _nitrogenContent;
            set => SetProperty(ref _nitrogenContent, value);
        }

        /// <summary>
        /// (% wet weight)
        ///
        /// Expressed as a percentage
        ///
        /// TODO: rename this to CarbonPercentage
        /// </summary>
        public double CarbonFraction 
        {
            get => _carbonFraction;
            set => SetProperty(ref _carbonFraction, value, () => { this.CarbonContent = value / 100.0;});
        }

        /// <summary>
        /// Proportion of C in manure (fraction)
        /// </summary>
        public double CarbonContent
        {
            get => _carbonContent;
            set => SetProperty(ref _carbonContent, value);
        }

        /// <summary>
        /// (% wet weight)
        ///
        /// Expressed as a percentage
        ///
        /// TODO: rename this to PhosphorusPercentage
        /// </summary>
        public double PhosphorusFraction 
        {
            get => _phosphorusFraction;
            set => SetProperty(ref _phosphorusFraction, value, () => { this.PhosphorusContent = value / 100.0;});
        }

        /// <summary>
        /// Proportion of P in manure (fraction)
        /// </summary>
        public double PhosphorusContent
        {
            get => _phosphorusContent;
            set => SetProperty(ref _phosphorusContent, value);
        }

        public double CarbonToNitrogenRatio
        {
            get => _carbonToNitrogenRatio;
            set => SetProperty(ref _carbonToNitrogenRatio, value);
        }

        /// <summary>
        /// A value returned for solid or liquid concentrations will have the same unit of measurement
        /// 
        /// (kg N (1000 L)^-1)
        /// </summary>
        public double NitrogenConcentrationOfManure {
            get => _nitrogenConcentrationOfManure;
            set => SetProperty(ref _nitrogenConcentrationOfManure, value);
        }

        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
        }

        /// <summary>
        /// VS content (% wet weight)
        /// </summary>
        public double VolatileSolidsContent
        {
            get => _volatileSolidsContent;
            set => SetProperty(ref _volatileSolidsContent, value);
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{nameof(ManureStateType)}: {ManureStateType}, {nameof(AnimalType)}: {AnimalType}";
        }

        #endregion
    }
}