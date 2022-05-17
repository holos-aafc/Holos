using H.Core.Enumerations;
using H.Infrastructure;

namespace H.Core.Models
{
    /// <summary>
    /// A storage tank for a particular type of manure (beef, dairy, etc.)
    /// </summary>
    public class ManureTank : ModelBase
    {
        #region Fields

        private AnimalType _animalType;

        private double _volumeRemainingInTank;
        private double _totalOrganicNitrogenAvailableForLandApplication;
        private double _totalTanAvailableForLandApplication;
        private double _totalAmountOfCarbonInStoredManure;
        private double _totalAvailableManureNitrogenAvailableForLandApplication;
        private double _totalAvailableManureNitrogenAvailableForLandApplicationAfterAllLandApplications;
        private double _nitrogenSumOfAllManureApplicationsMade;

        #endregion

        #region Properties

        /// <summary>
        /// When user defines multiple years for a field history, there will need to be a tank for each year of the history
        /// </summary>
        public int Year { get; set; }

        public AnimalType AnimalType
        {
            get => _animalType;
            set => SetProperty(ref _animalType, value);
        }

        /// <summary>
        /// The volume of manure remaining in the storage tank.
        ///
        /// (1000 kg wet weight for solid manure, 1000 L for liquid manure)
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

        public double TotalTanAvailableForLandApplication
        {
            get => _totalTanAvailableForLandApplication;
            set => SetProperty(ref _totalTanAvailableForLandApplication, value);
        }

        public double TotalAmountOfCarbonInStoredManure
        {
            get => _totalAmountOfCarbonInStoredManure;
            set => SetProperty(ref _totalAmountOfCarbonInStoredManure, value);
        }

        public double TotalAvailableManureNitrogenAvailableForLandApplication
        {
            get => _totalAvailableManureNitrogenAvailableForLandApplication;
            set => SetProperty(ref _totalAvailableManureNitrogenAvailableForLandApplication, value);
        }

        public double TotalAvailableManureNitrogenAvailableForLandApplicationAfterAllLandApplications
        {
            get => _totalAvailableManureNitrogenAvailableForLandApplicationAfterAllLandApplications;
            set => SetProperty(ref _totalAvailableManureNitrogenAvailableForLandApplicationAfterAllLandApplications, value);
        }

        public double NitrogenSumOfAllManureApplicationsMade
        {
            get => _nitrogenSumOfAllManureApplicationsMade;
            set => SetProperty(ref _nitrogenSumOfAllManureApplicationsMade, value, OnSumOfAllManureApplicationsMade);
        }

        #endregion

        #region Event Handlers

        private void OnSumOfAllManureApplicationsMade()
        {
            this.TotalAvailableManureNitrogenAvailableForLandApplicationAfterAllLandApplications = this.TotalAvailableManureNitrogenAvailableForLandApplication - this.NitrogenSumOfAllManureApplicationsMade;           
        }

        #endregion
    }
}