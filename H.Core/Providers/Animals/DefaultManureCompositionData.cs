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
        private double _moistureContent;
        private double _carbonToNitrogenRatio;
        private double _nitrogenConcentrationOfManure;

        private ManureStateType _manureStateType;
        private AnimalType _animalType;

        #endregion

        #region Properties

        public ManureStateType ManureStateType
        {
            get
            {
                return _manureStateType;
            }
            set
            {
                SetProperty(ref _manureStateType, value);
            }
        }

        public string ManureStateTypeString
        {
            get { return this.ManureStateType.GetDescription(); }
        }

        /// <summary>
        /// %
        /// </summary>
        public double MoistureContent 
        {
            get
            {
                return _moistureContent;
            }
            set
            {
                SetProperty(ref _moistureContent, value);
            }
        }

        /// <summary>
        /// (% fraction wet weight)
        /// </summary>
        public double NitrogenFraction 
        {
            get
            {
                return _nitrogenFraction;
            }
            set
            {
                SetProperty(ref _nitrogenFraction, value);
            }
        }

        /// <summary>
        /// (% fraction wet weight)
        /// </summary>
        public double CarbonFraction 
        {
            get
            {
                return _carbonFraction;
            }
            set
            {
                SetProperty(ref _carbonFraction, value);
            }
        }

        /// <summary>
        /// (% fraction wet weight)
        /// </summary>
        public double PhosphorusFraction 
        {
            get
            {
                return _phosphorusFraction;
            }
            set
            {
                SetProperty(ref _phosphorusFraction, value);
            }
        }

        public double CarbonToNitrogenRatio
        {
            get
            {
                return _carbonToNitrogenRatio;
            }
            set
            {
                SetProperty(ref _carbonToNitrogenRatio, value);
            }
        }

        /// <summary>
        /// A value returned for solid or liquid concentrations will have the same unit of measurement
        /// 
        /// (kg N (1000 L)^-1)
        /// </summary>
        public double NitrogenConcentrationOfManure {
            get
            {
                return _nitrogenConcentrationOfManure;
            }
            set
            {
                SetProperty(ref _nitrogenConcentrationOfManure, value);
            } }

        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
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