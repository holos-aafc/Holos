using H.Infrastructure;

namespace H.Core.Models
{
    public abstract class StorageTankBase : ModelBase
    {
        #region Public Methods

        public virtual void ResetTank()
        {
            VolumeRemainingInTank = 0;
            TotalOrganicNitrogenAvailableForLandApplication = 0;
            TotalTanAvailableForLandApplication = 0;
            TotalAmountOfCarbonInStoredManure = 0;
            TotalNitrogenAvailableForLandApplication = 0;
            TotalNitrogenAvailableAfterAllLandApplications = 0;
            NitrogenSumOfAllManureApplicationsMade = 0;
            VolumeSumOfAllManureApplicationsMade = 0;
            VolumeOfManureAvailableForLandApplication = 0;
        }

        #endregion

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Year)}: {Year}";
        }

        #region Fields

        private double _volumeRemainingInTank;
        private double _volumeOfManureAvailableForLandApplication;
        private double _totalOrganicNitrogenAvailableForLandApplication;
        private double _totalTanAvailableForLandApplication;
        private double _totalAmountOfCarbonInStoredManure;
        private double _totalNitrogenAvailableForLandApplication;
        private double _totalNitrogenAvailableAfterAllLandApplications;
        private double _nitrogenSumOfAllManureApplicationsMade;
        private double _volumeSumOfAllManureApplicationsMade;

        #endregion

        #region Properties

        /// <summary>
        ///     When user defines multiple years for a field history, there will need to be a tank for each year of the history
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        ///     The volume of manure or digestate remaining in the storage tank
        ///     (1000 kg wet weight for solid manure, 1000 L for liquid manure)
        /// </summary>
        public double VolumeRemainingInTank
        {
            get => _volumeRemainingInTank;
            set => SetProperty(ref _volumeRemainingInTank, value);
        }

        public double TotalOrganicNitrogenAvailableForLandApplication
        {
            get => _totalOrganicNitrogenAvailableForLandApplication;
            set => SetProperty(ref _totalOrganicNitrogenAvailableForLandApplication, value);
        }

        /// <summary>
        ///     (kg)
        /// </summary>
        public double TotalTanAvailableForLandApplication
        {
            get => _totalTanAvailableForLandApplication;
            set => SetProperty(ref _totalTanAvailableForLandApplication, value);
        }

        /// <summary>
        ///     (kg C)
        /// </summary>
        public double TotalAmountOfCarbonInStoredManure
        {
            get => _totalAmountOfCarbonInStoredManure;
            set => SetProperty(ref _totalAmountOfCarbonInStoredManure, value);
        }

        public double TotalNitrogenAvailableForLandApplication
        {
            get => _totalNitrogenAvailableForLandApplication;
            set => SetProperty(ref _totalNitrogenAvailableForLandApplication, value);
        }

        public double TotalNitrogenAvailableAfterAllLandApplications
        {
            get => _totalNitrogenAvailableAfterAllLandApplications;
            set => SetProperty(ref _totalNitrogenAvailableAfterAllLandApplications, value);
        }

        /// <summary>
        ///     Total amount of nitrogen applied to fields from manure produced on farm (does not include imports)
        /// </summary>
        public double NitrogenSumOfAllManureApplicationsMade
        {
            get => _nitrogenSumOfAllManureApplicationsMade;
            set
            {
                SetProperty(ref _nitrogenSumOfAllManureApplicationsMade, value);

                // This needs to be called if the value is changed or not and keep the call outside of SetProperty()
                OnSumOfAllManureApplicationsMade();
            }
        }

        public double VolumeSumOfAllManureApplicationsMade
        {
            get => _volumeSumOfAllManureApplicationsMade;
            set => SetProperty(ref _volumeSumOfAllManureApplicationsMade, value,
                OnVolumeSumOfAllManureApplicationsMade);
        }

        /// <summary>
        ///     (kg)
        /// </summary>
        public double VolumeOfManureAvailableForLandApplication
        {
            get => _volumeOfManureAvailableForLandApplication;
            set => SetProperty(ref _volumeOfManureAvailableForLandApplication, value);
        }

        #endregion

        #region Private Methods

        private void OnSumOfAllManureApplicationsMade()
        {
            TotalNitrogenAvailableAfterAllLandApplications =
                TotalNitrogenAvailableForLandApplication - NitrogenSumOfAllManureApplicationsMade;
        }

        private void OnVolumeSumOfAllManureApplicationsMade()
        {
            VolumeRemainingInTank = VolumeOfManureAvailableForLandApplication - VolumeSumOfAllManureApplicationsMade;
        }

        #endregion
    }
}