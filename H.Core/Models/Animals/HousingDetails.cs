using System;
using System.ComponentModel;
using System.Diagnostics;
using AutoMapper;
using H.Core.Converters;
using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Infrastructure;

namespace H.Core.Models.Animals
{
    public class HousingDetails : ModelBase
    {
        #region Fields

        private FieldSystemComponent _pastureLocation;

        private HousingType _housingType;
        private BeddingMaterialType _beddingMaterialType;
        private GrazingSystemTypes? _grazingSystemType;

        private double _userDefinedBeddingRate;
        private double _maintenanceCoefficientModifiedByTemperature;
        private double _activityCeofficientOfFeedingSituation;
        private double _baselineMaintenanceCoefficient;
        private double _ammoniaEmissionFactorForHousing;

        private double _totalNitrogenKilogramsDryMatterForBedding;
        private double _totalCarbonKilogramsDryMatterForBedding;
        private double _totalPhosphorusKilogramsDryMatterForBedding;
        private double _carbonToNitrogenRatioOfBeddingMaterial;
        private double _moistureContentOfBeddingMaterial;

        private string _nameOfPastureLocation;

        private static readonly IMapper _housingDetailsMapper;

        #endregion

        #region Constructors

        static HousingDetails()
        {
            var housingDetailsConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<HousingDetails, HousingDetails>()
                    .ForMember(housingDetails => housingDetails.Guid, opt => opt.Ignore())
                    .ForMember(housingDetails => housingDetails.Name, opt => opt.Ignore());
            });

            _housingDetailsMapper = housingDetailsConfiguration.CreateMapper();
        }
        public HousingDetails()
        {
            
            this.GrazingSystemType = null; // No grazing by default

            this.PropertyChanged -= OnPropertyChanged;
            this.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        public HousingType HousingType
        {
            get { return _housingType; }
            set { this.SetProperty(ref _housingType, value); }
        }

        public string HousingTypeString
        {
            get { return this.HousingType.GetDescription(); }
        }

        /// <summary>
        /// Will be set the name of the field component when animals are housed on a pasture.
        /// </summary>
        public string NameOfPastureLocation
        {
            get
            {
                if (this.PastureLocation != null)
                {
                    return this.PastureLocation.Name;
                }
                else
                {
                    return CoreConstants.NotApplicableOutputString;
                }
            }
        }

        /// <summary>
        /// CA
        ///
        /// (MJ day^-1 kg^-1)
        /// </summary>
        public double ActivityCeofficientOfFeedingSituation
        {
            get { return _activityCeofficientOfFeedingSituation; }
            set { this.SetProperty(ref _activityCeofficientOfFeedingSituation, value); }
        }

        /// <summary>
        /// C_f
        ///
        /// (MJ day^-1 kg^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerDayPerKilogram)]
        public double BaselineMaintenanceCoefficient
        {
            get { return _baselineMaintenanceCoefficient; }
            set { SetProperty(ref _baselineMaintenanceCoefficient, value); }
        }

        /// <summary>
        /// C_f_adjusted
        ///
        /// (MJ day⁻¹ kg⁻¹)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerDayPerKilogram)]
        public double MaintenanceCoefficientModifiedByTemperature
        {
            get { return _maintenanceCoefficientModifiedByTemperature; }
            set { this.SetProperty(ref _maintenanceCoefficientModifiedByTemperature, value); }
        }

        /// <summary>
        /// A reference to the <see cref="FieldSystemComponent"/> when animals are housed on pasture.
        /// </summary>
        public FieldSystemComponent PastureLocation
        {
            get { return _pastureLocation; }
            set { this.SetProperty(ref _pastureLocation, value); }
        }

        /// <summary>
        /// The type of bedding material being used (if any).
        /// </summary>
        public BeddingMaterialType BeddingMaterialType
        {
            get { return _beddingMaterialType; }
            set { SetProperty(ref _beddingMaterialType, value); }
        }

        /// <summary>
        /// The rate of bedding material added to the housing system
        ///
        /// (kg head-1 day-1) 
        /// </summary>
        [Units(MetricUnitsOfMeasurement.KilogramPerHeadPerDay)]
        public double UserDefinedBeddingRate
        {
            get { return _userDefinedBeddingRate; }
            set { SetProperty(ref _userDefinedBeddingRate, value); }
        }

        /// <summary>
        /// Used to specify the type of grazing system when animals are housed on a pasture.
        /// </summary>
        public GrazingSystemTypes? GrazingSystemType
        {
            get => _grazingSystemType;
            set => SetProperty(ref _grazingSystemType, value);
        }

        /// <summary>
        /// (kg N/kg DM)
        /// </summary>
        public double TotalNitrogenKilogramsDryMatterForBedding
        {
            get => _totalNitrogenKilogramsDryMatterForBedding;
            set => SetProperty(ref _totalNitrogenKilogramsDryMatterForBedding, value);
        }

        /// <summary>
        /// (kg C/kg DM)
        /// </summary>
        public double TotalCarbonKilogramsDryMatterForBedding
        {
            get => _totalCarbonKilogramsDryMatterForBedding;
            set => SetProperty(ref _totalCarbonKilogramsDryMatterForBedding, value);
        }

        /// <summary>
        /// (kg P/kg DM)
        /// </summary>
        public double TotalPhosphorusKilogramsDryMatterForBedding
        {
            get => _totalPhosphorusKilogramsDryMatterForBedding;
            set => SetProperty(ref _totalPhosphorusKilogramsDryMatterForBedding, value);
        }

        /// <summary>
        /// (unitless)
        /// </summary>
        public double CarbonToNitrogenRatioOfBeddingMaterial
        {
            get => _carbonToNitrogenRatioOfBeddingMaterial;
            set => SetProperty(ref _carbonToNitrogenRatioOfBeddingMaterial,value );
        }

        public double AmmoniaEmissionFactorForHousing
        {
            get => _ammoniaEmissionFactorForHousing;
            set => SetProperty(ref _ammoniaEmissionFactorForHousing, value);
        }

        /// <summary>
        /// (%)
        /// </summary>
        public double MoistureContentOfBeddingMaterial
        {
            get => _moistureContentOfBeddingMaterial;
            set => SetProperty(ref _moistureContentOfBeddingMaterial, value);
        }

        #endregion

        #region Public Methods

        public static HousingDetails CopyHousingDetails(HousingDetails sourceHousingDetails)
        {
            var copyHousingDetails = _housingDetailsMapper.Map<HousingDetails>(sourceHousingDetails);

            return copyHousingDetails;
        }

        public static HousingDetails GetSystemHousingDetailsFromBinding(HousingDetails bindingHousingDetails)
        {
            var systemCopy = CopyHousingDetails(bindingHousingDetails);
            var converter = new PropertyConverter<HousingDetails>(bindingHousingDetails);
            var attrProps = converter.PropertyInfos;
            foreach (var propertyInfo in attrProps)
            {
                //system value in metric
                var systemValue = converter.GetSystemValueFromBinding(propertyInfo);
                //set the systemCopy to contain that metric value 
                propertyInfo.SetValue(systemCopy, systemValue);
            }
            return systemCopy;
        }

        public static HousingDetails GetBindingHousingDetailsFromSystem(HousingDetails systemHousingDetails)
        {
            var bindingHousingDetails = CopyHousingDetails(systemHousingDetails);
            var converter = new PropertyConverter<HousingDetails>(bindingHousingDetails);
            var attrProps = converter.PropertyInfos;
            foreach (var propertyInfo in attrProps)
            {
                //system value in metric
                var bindingValue = converter.GetBindingValueFromSystem(propertyInfo);
                //set the systemCopy to contain that metric value 
                propertyInfo.SetValue(bindingHousingDetails, bindingValue);
            }
            return bindingHousingDetails;
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is HousingDetails housingDetails)
            {
            }
        }

        #endregion
    }
}