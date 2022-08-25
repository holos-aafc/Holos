using System.ComponentModel;
using System.Diagnostics;
using AutoMapper;
using H.Core.Converters;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models.Animals
{
    public class ManureDetails : ModelBase
    {
        #region Fields

        private ManureStateType _manureStateType;

        private double _methaneConversionFactor;
        private double _n2ODirectEmissionFactor;
        private double _volatilizationFraction;
        private double _ashContentOfManure;
        private double _leachingFraction;
        private double _methaneProducingCapacityOfManure;
        private double _emissionFactorLeaching;
        private double _emissionFactorVolatilization;
        private double _volatileSolidExcretion;
        private double _volatileSolids;
        private double _yearlyEntericMethaneRate;
        private double _dailyManureMethaneEmissionRate;
        private double _yearlyManureMethaneRate;
        private double _nitrogenExretionRate;
        private double _yearlyTanExcretion;
        private double _ammoniaEmissionFactorForManureStorage;
        private double _ammoniaEmissionFactorForLandApplication;
        private double _fractionOfOrganicNitrogenImmobilized;
        private double _fractionOfOrganicNitrogenNitrified;
        private double _fractionOfOrganicNitrogenMineralized;
        private double _manureExcretionRate;
        private double _fractionOfNitrogenInManure;
        private double _fractionOfCarbonInManure;
        private double _fractionOfPhosphorusInManure;

        private bool _useCustomVolatilizationFraction;
        private bool _useCustomMethaneConversionFactor;

        private static IMapper _manureDetailsMapper;
        #endregion

        #region Constructors

        static ManureDetails()
        {
            var manureDetailsMapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<ManureDetails, ManureDetails>()
                    .ForMember(manureDetails => manureDetails.Guid, opt => opt.Ignore())
                    .ForMember(manureDetails => manureDetails.Name, opt => opt.Ignore());
            });
            _manureDetailsMapper = manureDetailsMapperConfiguration.CreateMapper();
        }

        public ManureDetails()
        {
            this.AshContentOfManure = 8;

            this.PropertyChanged -= OnPropertyChanged;
            this.PropertyChanged += OnPropertyChanged;

            this.UseCustomVolatilizationFraction = false;
        }

        #endregion

        #region Properties

        public bool UseCustomVolatilizationFraction
        {
            get => _useCustomVolatilizationFraction;
            set => SetProperty(ref _useCustomVolatilizationFraction, value);
        }

        /// <summary>
        /// The manure storage/handling system being used.
        /// </summary>
        public ManureStateType StateType
        {
            get { return _manureStateType; }
            set { this.SetProperty(ref _manureStateType, value); }
        }

        /// <summary>
        /// Methane conversion factor of manure
        ///
        /// MCF
        /// 
        /// (kg kg^-1)
        /// </summary>
        public double MethaneConversionFactor
        {
            get { return _methaneConversionFactor; }
            set { this.SetProperty(ref _methaneConversionFactor, value); }
        }

        /// <summary>
        /// N2O direct emission factor
        /// 
        /// EF Direct
        ///
        /// [kg N2O-N (kg N)^1]
        /// </summary>
        public double N2ODirectEmissionFactor
        {
            get { return _n2ODirectEmissionFactor; }
            set { this.SetProperty(ref _n2ODirectEmissionFactor, value); }
        }

        /// <summary>
        /// Volatilization fraction
        ///
        /// Some animal types do not have a methodology for calculating this fraction and so default values are used in those situations (e.g. other animals category)
        ///
        /// [kg NH3-N (kg N)^-1]
        /// </summary>
        public double VolatilizationFraction
        {
            get { return _volatilizationFraction; }
            set { this.SetProperty(ref _volatilizationFraction, value); }
        }

        /// <summary>
        /// Ash content of manure
        ///
        /// (%)
        /// </summary>
        public double AshContentOfManure
        {
            get { return _ashContentOfManure; }
            set { this.SetProperty(ref _ashContentOfManure, value); }
        }

        /// <summary>
        /// Methane producing capacity of manure
        /// 
        /// B_o
        ///
        /// (m^3 CH4 kg^-1 VS)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.CubicMetersMethanePerKilogramVolatileSolids)]
        public double MethaneProducingCapacityOfManure
        {
            get { return _methaneProducingCapacityOfManure; }
            set { this.SetProperty(ref _methaneProducingCapacityOfManure, value); }
        }

        /// <summary>
        /// Frac_leach
        ///
        /// (unitless)
        /// </summary>
        public double LeachingFraction
        {
            get { return _leachingFraction; }
            set { SetProperty(ref _leachingFraction, value); }
        }

        /// <summary>
        /// Emission factor for leaching
        /// 
        /// EF_leach
        ///
        /// [kg N2O-N (kg N)^-1]
        /// </summary>
        public double EmissionFactorLeaching
        {
            get { return _emissionFactorLeaching; }
            set { SetProperty(ref _emissionFactorLeaching, value); }
        }

        /// <summary>
        /// Emission factor for volatilization
        /// 
        /// EF_volatilization
        ///
        /// [kg N2O-N (kg N)^-1]
        /// </summary>
        public double EmissionFactorVolatilization
        {
            get { return _emissionFactorVolatilization; }
            set { SetProperty(ref _emissionFactorVolatilization, value); }
        }

        /// <summary>
        /// (kg VS kg^-1 feed)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerDay)]
        public double VolatileSolidExcretion
        {
            get { return _volatileSolidExcretion; }
            set { this.SetProperty(ref _volatileSolidExcretion, value); }
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerDay)]
        public double VolatileSolids
        {
            get => _volatileSolids;
            set => SetProperty(ref _volatileSolids, value);
        }

        /// <summary>
        /// (kg CH4 head^-1 year^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerYear)]
        public double YearlyEntericMethaneRate
        {
            get { return _yearlyEntericMethaneRate; }
            set { this.SetProperty(ref _yearlyEntericMethaneRate, value); }
        }

        /// <summary>
        /// (kg CH4 head^-1 year^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerYear)]
        public double YearlyManureMethaneRate
        {
            get { return _yearlyManureMethaneRate; }
            set { SetProperty(ref _yearlyManureMethaneRate, value); }
        }

        /// <summary>
        /// Some animals have this value calculated, other animal groups have this set to a constant value (i.e. chickens use a non-calculated rate)
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerYear)]
        public double NitrogenExretionRate
        {
            get { return _nitrogenExretionRate; }
            set { SetProperty(ref _nitrogenExretionRate, value); }
        }

        /// <summary>
        /// The default ammonia emission factor for housing
        ///
        /// (kg NH3-N kg^-1 TAN)
        /// </summary>
        public double AmmoniaEmissionFactorForManureStorage
        {
            get => _ammoniaEmissionFactorForManureStorage;
            set => SetProperty(ref _ammoniaEmissionFactorForManureStorage, value);
        }
        
        /// <summary>
        /// (unitless)
        /// </summary>
        public double FractionOfOrganicNitrogenImmobilized
        {
            get => _fractionOfOrganicNitrogenImmobilized;
            set => SetProperty(ref _fractionOfOrganicNitrogenImmobilized, value);
        }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double FractionOfOrganicNitrogenNitrified
        {
            get => _fractionOfOrganicNitrogenNitrified;
            set => SetProperty(ref _fractionOfOrganicNitrogenNitrified, value);
        }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double FractionOfOrganicNitrogenMineralized
        {
            get => _fractionOfOrganicNitrogenMineralized;
            set => SetProperty(ref _fractionOfOrganicNitrogenMineralized, value);
        }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double ManureExcretionRate
        {
            get => _manureExcretionRate;
            set => SetProperty(ref _manureExcretionRate, value);
        }

        /// <summary>
        /// User can indicate if a custom methane conversion factor should be used.
        /// </summary>
        public bool UseCustomMethaneConversionFactor
        {
            get => _useCustomMethaneConversionFactor;
            set => SetProperty(ref _useCustomMethaneConversionFactor, value);
        }

        /// <summary>
        /// (% wet weight)
        /// </summary>
        public double FractionOfNitrogenInManure 
        { 
            get => _fractionOfNitrogenInManure;
            set => SetProperty(ref _fractionOfNitrogenInManure, value);
        }

        /// <summary>
        /// (% wet weight)
        /// </summary>
        public double FractionOfCarbonInManure 
        { 
            get => _fractionOfCarbonInManure; 
            set => SetProperty(ref _fractionOfCarbonInManure, value); 
        }

        /// <summary>
        /// (% wet weight)
        /// </summary>
        public double FractionOfPhosphorusInManure 
        { 
            get => _fractionOfPhosphorusInManure; 
            set => SetProperty(ref _fractionOfPhosphorusInManure, value); 
        }

        /// <summary>
        /// Turkeys, deer, etc. have a non-calculated manure methane rate
        /// 
        /// (kg head^-1 day^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerDay)]
        public double DailyManureMethaneEmissionRate
        {
            get => _dailyManureMethaneEmissionRate;
            set => SetProperty(ref _dailyManureMethaneEmissionRate, value);
        }

        public double YearlyTanExcretion
        {
            get => _yearlyTanExcretion;
            set => SetProperty(ref _yearlyTanExcretion, value);
        }

        /// <summary>
        /// Currently used by poultry groups only
        ///
        /// (kg TAN head^-1 day^-1)
        /// </summary>
        public double DailyTanExcretion { get; set; }

        public double AmmoniaEmissionFactorForLandApplication
        {
            get => _ammoniaEmissionFactorForLandApplication;
            set => SetProperty(ref _ammoniaEmissionFactorForLandApplication, value);
        }

        #endregion

        #region Public Methods

        public static ManureDetails CopyManureDetails(ManureDetails manureDetailsToCopy)
        {
            return _manureDetailsMapper.Map<ManureDetails>(manureDetailsToCopy);
        }

        /// <summary>
        /// Get a metric value for system converted from the GUI binding instance (metric/imperial)
        /// </summary>
        /// <param name="bindingManureDetails">the GUI binding manure details</param>
        /// <returns>manure details converted to metric</returns>
        public static ManureDetails GetSystemManureDetailsFromBinding(ManureDetails bindingManureDetails)
        {
            var systemManureDetails= CopyManureDetails(bindingManureDetails);
            var systemPropertyConverter = new PropertyConverter<ManureDetails>(systemManureDetails);
            var attrProps = systemPropertyConverter.PropertyInfos;

            foreach (var propertyInfo in attrProps)
            {
                var systemValue = systemPropertyConverter.GetSystemValueFromBinding(propertyInfo);
                propertyInfo.SetValue(systemManureDetails, systemValue);
            }

            return systemManureDetails;
        }

        /// <summary>
        /// Get appropriate binding manure details (in metric/imperial) for GUI converted from the system (metric)
        /// </summary>
        /// <param name="systemManureDetails">the system manure details</param>
        /// <returns>metric or imperial converted manure details</returns>
        public static ManureDetails GetBindingManureDetailsFromSystem(ManureDetails systemManureDetails)
        {
            var bindingManureDetails = CopyManureDetails(systemManureDetails);
            var bindingPropertyConverter = new PropertyConverter<ManureDetails>(bindingManureDetails);
            var attrProps = bindingPropertyConverter.PropertyInfos;

            foreach (var propertyInfo in attrProps)
            {
                var bindingValue = bindingPropertyConverter.GetBindingValueFromSystem(propertyInfo);
                propertyInfo.SetValue(bindingManureDetails, bindingValue);
            }

            return bindingManureDetails;
        }

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ManureDetails manureDetails)
            {
            }
        }

        #endregion
    }
}