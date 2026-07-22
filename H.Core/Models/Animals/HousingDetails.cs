using System;
using System.ComponentModel;
using System.Diagnostics;
using H.Core.Mappers;
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
        private Guid _pastureLocationGuid;

        /// <summary>
        /// Defaults to true so the field is written unless the farm has confirmed the reference can be resolved from
        /// its own components. Anything the farm cannot resolve - notably a pasture belonging to a different farm,
        /// which happens when a farm is copied - would otherwise be lost on save.
        /// </summary>
        private bool _writePastureLocationInline = true;

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

        private double _indoorHousingTemperature;

        private string _nameOfPastureLocation;

        private static readonly ModelMapper<HousingDetails> _housingDetailsMapper =
            new ModelMapper<HousingDetails>(nameof(HousingDetails.Guid), nameof(HousingDetails.Name));

        private bool _useCustomIndoorHousingTemperature;

        #endregion

        #region Constructors

        public HousingDetails()
        {
            this.GrazingSystemType = null; // No grazing by default
            this.UseCustomIndoorHousingTemperature = false;

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
        /// The <see cref="ModelBase.Guid"/> of <see cref="PastureLocation"/>. The pasture is a field already held in
        /// the farm's components, so persisting the field itself stored a complete extra copy of it - including all of
        /// its crop view items - in every management period that referenced it.
        /// </summary>
        public Guid PastureLocationGuid
        {
            get { return _pastureLocation != null ? _pastureLocation.Guid : _pastureLocationGuid; }
            set { _pastureLocationGuid = value; }
        }

        /// <summary>
        /// Writes the field only when the owning farm could not resolve it from its own components. Reading is always
        /// allowed, so files written before <see cref="PastureLocationGuid"/> existed keep resolving their pasture.
        /// </summary>
        public bool ShouldSerializePastureLocation()
        {
            return _writePastureLocationInline;
        }

        /// <summary>
        /// Called by the owning farm before serializing. When the pasture is one of that farm's own fields only the
        /// guid needs to be written; otherwise the field is written inline so the reference is not lost.
        /// </summary>
        internal void SetPastureLocationIsResolvable(bool isResolvable)
        {
            _writePastureLocationInline = isResolvable == false;
        }

        /// <summary>
        /// The guid used to restore <see cref="PastureLocation"/> after loading. The
        /// <see cref="PastureLocationGuid"/> getter cannot be used because it prefers the in-memory field, which is
        /// null until the reference is restored.
        /// </summary>
        internal Guid GetPersistedPastureLocationGuid()
        {
            if (_pastureLocationGuid != Guid.Empty)
            {
                return _pastureLocationGuid;
            }

            return _pastureLocation != null ? _pastureLocation.Guid : Guid.Empty;
        }

        /// <summary>
        /// Re-points <see cref="PastureLocation"/> at the field held in the farm's components. Assigns the field
        /// directly because <see cref="ModelBase.Equals(object)"/> compares by <see cref="ModelBase.Guid"/>, so a copy
        /// read from an older file compares equal to the real field and SetProperty would skip the assignment.
        /// </summary>
        internal void RestorePastureLocation(FieldSystemComponent pastureLocation)
        {
            _pastureLocation = pastureLocation;
            _pastureLocationGuid = pastureLocation != null ? pastureLocation.Guid : Guid.Empty;

            this.RaisePropertyChanged(nameof(this.PastureLocation));
            this.RaisePropertyChanged(nameof(this.NameOfPastureLocation));
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

        /// <summary>
        /// (degrees celsius)
        /// </summary>
        public double IndoorHousingTemperature
        {
            get => _indoorHousingTemperature;
            set => SetProperty(ref _indoorHousingTemperature, value);
        }

        public bool UseCustomIndoorHousingTemperature
        {
            get => _useCustomIndoorHousingTemperature;
            set => SetProperty(ref _useCustomIndoorHousingTemperature, value);
        }

        #endregion

        #region Public Methods

        public static HousingDetails CopyHousingDetails(HousingDetails sourceHousingDetails)
        {
            var copyHousingDetails = _housingDetailsMapper.Map(sourceHousingDetails);

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