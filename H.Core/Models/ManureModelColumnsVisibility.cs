using Prism.Mvvm;

namespace H.Core.Models
{
    /// <summary>
    /// Class to specify the default visiblity of the ManureModelResultsView
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

        #endregion

        #region constructors

        public ManureModelColumnsVisibility()
        {
            this.DefaultVisibility();
        }
        
        public void DefaultVisibility()
        {
            base.SetAllColumnsInvisible();
            //we can set them all visible for now
            foreach (var prop in this.GetType().GetProperties())
            {
                prop.SetValue(this, true);
            }
        }

        #endregion

        #region properties
        public bool ComponentName
        {
            get { return _componentName; }
            set { SetProperty(ref _componentName, value); }
        }
        public bool GroupName
        {
            get { return _groupName; }
            set { SetProperty(ref _groupName, value); }
        }
        public bool ManagementPeriod
        {
            get { return _managementPeriod; }
            set { SetProperty(ref _managementPeriod, value); }
        }
        public bool Month
        {
            get { return _month; }
            set { SetProperty(ref _month, value); }
        }
        public bool DaysInMonth
        {
            get { return _daysInMonth; }
            set { SetProperty(ref _daysInMonth, value); }
        }
        public bool HousingType
        {
            get { return _housingType; }
            set { SetProperty(ref _housingType, value); }
        }
        public bool PastureLocation
        {
            get { return _pastureLocation; }
            set { SetProperty(ref _pastureLocation, value); }
        }
        public bool DryMatterIntake
        {
            get { return _dryMatterIntake; }
            set { SetProperty(ref _dryMatterIntake, value); }
        }
        public bool ProteinIntake
        {
            get { return _proteinIntake; }
            set { SetProperty(ref _proteinIntake, value); }
        }
        public bool AmountCarbonFromBedding
        {
            get { return _amountCarbonFromBedding; }
            set { SetProperty(ref _amountCarbonFromBedding, value); }
        }
        public bool AmountNitrogenFromBedding
        {
            get { return _amountNitrogenFromBedding; }
            set { SetProperty(ref _amountNitrogenFromBedding, value); }
        }
        public bool OrganicNitrogenInStoredManure 
        {
            get { return _organicNitrogenInStoredManure; }
            set { SetProperty(ref _organicNitrogenInStoredManure, value); }
        }
        public bool AmountOfNitrogenExcreted 
        {
            get { return _amountOfNitrogenExcreted; }
            set { SetProperty(ref _amountOfNitrogenExcreted, value); }
        }
        public bool FecalNitrogenExcreted 
        {
            get { return _fecalNitrogenExcreted; }
            set { SetProperty(ref _fecalNitrogenExcreted, value); }
        }
        public bool UrinaryNitrogenExcreted 
        {
            get { return _urinaryNitrogenExcretion; }
            set { SetProperty(ref _urinaryNitrogenExcretion, value); }
        }
        public bool FecalCarbonExcretion
        {
            get { return _fecalCarbonExcretion; }
            set { SetProperty(ref _fecalCarbonExcretion, value); }
        }
        public bool AmbientAirTemperatureAdjustmentForHousing
        {
            get { return _ambientAirTemperatureAdjustmentForHousing; }
            set { SetProperty(ref _ambientAirTemperatureAdjustmentForHousing, value); }
        }
        public bool AdjustedAmmoniaEmissionFactorForHousing
        {
            get { return _adjustedAmmoniaEmissionFactorForHousing; }
            set { SetProperty(ref _adjustedAmmoniaEmissionFactorForHousing, value); }
        }
        public bool MonthlyTanEnteringStorageSystem
        {
            get { return _monthlyTanEnteringStorageSystem; }
            set { SetProperty(ref _monthlyTanEnteringStorageSystem, value); }
        }
        public bool AdjustedAmountOfTanStoredInManure
        {
            get { return _adjustedAmountOfTanStoredInManure; }
            set { SetProperty(ref _adjustedAmountOfTanStoredInManure, value); }
        }
        public bool AmmoniaConcentrationInHousing
        {
            get { return _ammoniaConcentrationInHousing; }
            set { SetProperty(ref _ammoniaConcentrationInHousing, value); }
        }
        public bool AmmoniaEmissionFromHousing
        {
            get { return _ammoniaEmissionFromHousing; }
            set { SetProperty(ref _ammoniaEmissionFromHousing, value); }
        }
        public bool AmbientAirTemperatureAdjustedForStorage
        {
            get { return _ambientAirTemperatureAdjustedForStorage; }
            set { SetProperty(ref _ambientAirTemperatureAdjustedForStorage, value); }
        }
        public bool AdjustedAmmoniaEmissionFactorForStoredManure
        {
            get { return _adjustedAmmoniaEmissionFactorForStoredManure; }
            set { SetProperty(ref _adjustedAmmoniaEmissionFactorForStoredManure, value); }
        }
        public bool FractionOfManureVolatilized
        {
            get { return _fractionOfManureVolatilized; }
            set { SetProperty(ref _fractionOfManureVolatilized, value); }
        }
        public bool AmmoniaConcentrationInStorage
        {
            get { return _ammoniaConcentrationInStorage; }
            set { SetProperty(ref _ammoniaConcentrationInStorage, value); }
        }
        public bool AmmoniaEmissionFromStorage
        {
            get { return _ammoniaEmissionsFromStorage; }
            set { SetProperty(ref _ammoniaEmissionsFromStorage, value); }
        }
        public bool ManureCarbonToNitrogenRatio
        {
            get { return _manureCarbonToNitrogenRatio; }
            set { SetProperty(ref _manureCarbonToNitrogenRatio, value); }
        }
        public bool TotalCarbonInStoredManure
        {
            get { return _totalCarbonInStoredManure; }
            set { SetProperty(ref _totalCarbonInStoredManure, value); }
        }
        public bool MonthlyTanAvailableForLandApplication
        {
            get { return _monthlyTanAvailableForLandApplication; }
            set { SetProperty(ref _monthlyTanAvailableForLandApplication, value); }
        }
        public bool MonthlyOrganiceNitrogenAvailableForLandApplication
        {
            get { return _monthlyOrganicNitrogenAvailableForLandApplication; }
            set { SetProperty(ref _monthlyOrganicNitrogenAvailableForLandApplication, value); }
        }
        public bool TotalAvailableManureNitrogenInStoredManure
        {
            get { return _totalAvailableManureNitrogenInStoredManure; }
            set { SetProperty(ref _totalAvailableManureNitrogenInStoredManure, value); }
        }
        public bool TotalVolumeOfManure 
        {
            get { return _totalVolumeOfManure; }
            set { SetProperty(ref _totalVolumeOfManure, value); }
        }

        #endregion
    }
}
