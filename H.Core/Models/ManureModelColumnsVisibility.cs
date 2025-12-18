namespace H.Core.Models
{
    /// <summary>
    ///     Class to specify the default visiblity of the ManureModelResultsView
    /// </summary>
    public class ManureModelColumnsVisibility : ColumnVisibilityBase
    {
        #region fields

        private bool _componentName;
        private bool _groupName;
        private bool _managementPeriod;
        private bool _month;
        private bool _daysInMonth;
        private bool _housingType;
        private bool _pastureLocation;
        private bool _dryMatterIntake;
        private bool _proteinIntake;
        private bool _amountCarbonFromBedding;
        private bool _amountNitrogenFromBedding;
        private bool _organicNitrogenInStoredManure;
        private bool _amountOfNitrogenExcreted;
        private bool _fecalNitrogenExcreted;
        private bool _urinaryNitrogenExcretion;
        private bool _fecalCarbonExcretion;
        private bool _ambientAirTemperatureAdjustmentForHousing;
        private bool _adjustedAmmoniaEmissionFactorForHousing;
        private bool _monthlyTanEnteringStorageSystem;
        private bool _adjustedAmountOfTanStoredInManure;
        private bool _ammoniaConcentrationInHousing;
        private bool _ammoniaEmissionFromHousing;
        private bool _ambientAirTemperatureAdjustedForStorage;
        private bool _adjustedAmmoniaEmissionFactorForStoredManure;
        private bool _fractionOfManureVolatilized;
        private bool _ammoniaConcentrationInStorage;
        private bool _ammoniaEmissionsFromStorage;
        private bool _manureCarbonToNitrogenRatio;
        private bool _totalCarbonInStoredManure;
        private bool _monthlyTanAvailableForLandApplication;
        private bool _monthlyOrganicNitrogenAvailableForLandApplication;
        private bool _totalAvailableManureNitrogenInStoredManure;
        private bool _totalVolumeOfManure;
        private bool _totalIndirectEmissionsFromLandAppliedManure;

        #endregion

        #region constructors

        public ManureModelColumnsVisibility()
        {
            DefaultVisibility();
        }

        public void DefaultVisibility()
        {
            SetAllColumnsInvisible();
            //we can set them all visible for now
            foreach (var prop in GetType().GetProperties()) prop.SetValue(this, true);
        }

        #endregion

        #region properties

        public bool ComponentName
        {
            get => _componentName;
            set => SetProperty(ref _componentName, value);
        }

        public bool GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        public bool ManagementPeriod
        {
            get => _managementPeriod;
            set => SetProperty(ref _managementPeriod, value);
        }

        public bool Month
        {
            get => _month;
            set => SetProperty(ref _month, value);
        }

        public bool DaysInMonth
        {
            get => _daysInMonth;
            set => SetProperty(ref _daysInMonth, value);
        }

        public bool HousingType
        {
            get => _housingType;
            set => SetProperty(ref _housingType, value);
        }

        public bool PastureLocation
        {
            get => _pastureLocation;
            set => SetProperty(ref _pastureLocation, value);
        }

        public bool DryMatterIntake
        {
            get => _dryMatterIntake;
            set => SetProperty(ref _dryMatterIntake, value);
        }

        public bool ProteinIntake
        {
            get => _proteinIntake;
            set => SetProperty(ref _proteinIntake, value);
        }

        public bool AmountCarbonFromBedding
        {
            get => _amountCarbonFromBedding;
            set => SetProperty(ref _amountCarbonFromBedding, value);
        }

        public bool AmountNitrogenFromBedding
        {
            get => _amountNitrogenFromBedding;
            set => SetProperty(ref _amountNitrogenFromBedding, value);
        }

        public bool OrganicNitrogenInStoredManure
        {
            get => _organicNitrogenInStoredManure;
            set => SetProperty(ref _organicNitrogenInStoredManure, value);
        }

        public bool AmountOfNitrogenExcreted
        {
            get => _amountOfNitrogenExcreted;
            set => SetProperty(ref _amountOfNitrogenExcreted, value);
        }

        public bool FecalNitrogenExcreted
        {
            get => _fecalNitrogenExcreted;
            set => SetProperty(ref _fecalNitrogenExcreted, value);
        }

        public bool UrinaryNitrogenExcreted
        {
            get => _urinaryNitrogenExcretion;
            set => SetProperty(ref _urinaryNitrogenExcretion, value);
        }

        public bool FecalCarbonExcretion
        {
            get => _fecalCarbonExcretion;
            set => SetProperty(ref _fecalCarbonExcretion, value);
        }

        public bool AmbientAirTemperatureAdjustmentForHousing
        {
            get => _ambientAirTemperatureAdjustmentForHousing;
            set => SetProperty(ref _ambientAirTemperatureAdjustmentForHousing, value);
        }

        public bool AdjustedAmmoniaEmissionFactorForHousing
        {
            get => _adjustedAmmoniaEmissionFactorForHousing;
            set => SetProperty(ref _adjustedAmmoniaEmissionFactorForHousing, value);
        }

        public bool MonthlyTanEnteringStorageSystem
        {
            get => _monthlyTanEnteringStorageSystem;
            set => SetProperty(ref _monthlyTanEnteringStorageSystem, value);
        }

        public bool AdjustedAmountOfTanStoredInManure
        {
            get => _adjustedAmountOfTanStoredInManure;
            set => SetProperty(ref _adjustedAmountOfTanStoredInManure, value);
        }

        public bool AmmoniaConcentrationInHousing
        {
            get => _ammoniaConcentrationInHousing;
            set => SetProperty(ref _ammoniaConcentrationInHousing, value);
        }

        public bool AmmoniaEmissionFromHousing
        {
            get => _ammoniaEmissionFromHousing;
            set => SetProperty(ref _ammoniaEmissionFromHousing, value);
        }

        public bool AmbientAirTemperatureAdjustedForStorage
        {
            get => _ambientAirTemperatureAdjustedForStorage;
            set => SetProperty(ref _ambientAirTemperatureAdjustedForStorage, value);
        }

        public bool AdjustedAmmoniaEmissionFactorForStoredManure
        {
            get => _adjustedAmmoniaEmissionFactorForStoredManure;
            set => SetProperty(ref _adjustedAmmoniaEmissionFactorForStoredManure, value);
        }

        public bool FractionOfManureVolatilized
        {
            get => _fractionOfManureVolatilized;
            set => SetProperty(ref _fractionOfManureVolatilized, value);
        }

        public bool AmmoniaConcentrationInStorage
        {
            get => _ammoniaConcentrationInStorage;
            set => SetProperty(ref _ammoniaConcentrationInStorage, value);
        }

        public bool AmmoniaEmissionFromStorage
        {
            get => _ammoniaEmissionsFromStorage;
            set => SetProperty(ref _ammoniaEmissionsFromStorage, value);
        }

        public bool ManureCarbonToNitrogenRatio
        {
            get => _manureCarbonToNitrogenRatio;
            set => SetProperty(ref _manureCarbonToNitrogenRatio, value);
        }

        public bool TotalCarbonInStoredManure
        {
            get => _totalCarbonInStoredManure;
            set => SetProperty(ref _totalCarbonInStoredManure, value);
        }

        public bool MonthlyTanAvailableForLandApplication
        {
            get => _monthlyTanAvailableForLandApplication;
            set => SetProperty(ref _monthlyTanAvailableForLandApplication, value);
        }

        public bool MonthlyOrganiceNitrogenAvailableForLandApplication
        {
            get => _monthlyOrganicNitrogenAvailableForLandApplication;
            set => SetProperty(ref _monthlyOrganicNitrogenAvailableForLandApplication, value);
        }

        public bool TotalAvailableManureNitrogenInStoredManure
        {
            get => _totalAvailableManureNitrogenInStoredManure;
            set => SetProperty(ref _totalAvailableManureNitrogenInStoredManure, value);
        }

        public bool TotalVolumeOfManure
        {
            get => _totalVolumeOfManure;
            set => SetProperty(ref _totalVolumeOfManure, value);
        }

        public bool TotalIndirectEmissionsFromLandAppliedManure
        {
            get => _totalIndirectEmissionsFromLandAppliedManure;
            set => SetProperty(ref _totalIndirectEmissionsFromLandAppliedManure, value);
        }

        #endregion
    }
}