using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        ///     Initialize the default manure excretion rate for all <see cref="ManagementPeriod" />s associated with this
        ///     <see cref="Farm" />.
        /// </summary>
        /// <param name="farm">The <see cref="Farm" /> containing the <see cref="ManagementPeriod" />s to initialize</param>
        public void InitializeManureExcretionRate(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeManureExcretionRate(managementPeriod);
        }

        /// <summary>
        ///     Initialize the default <see cref="ManureDetails.ManureExcretionRate" /> for the <see cref="ManagementPeriod" />.
        /// </summary>
        /// <param name="managementPeriod">
        ///     The <see cref="ManagementPeriod" /> to initialize with a default
        ///     <see cref="ManureDetails.ManureExcretionRate" />
        /// </param>
        public void InitializeManureExcretionRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
                managementPeriod.ManureDetails.ManureExcretionRate =
                    _defaultManureExcretionRateProvider.GetManureExcretionRate(managementPeriod.AnimalType);
        }

        public void InitializeNitrogenExcretionRate(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeNitrogenExcretionRate(managementPeriod);
        }

        public void InitializeNitrogenExcretionRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
                managementPeriod.ManureDetails.NitrogenExretionRate =
                    _poultryOtherLivestockDefaultNExcretionRatesProvider.GetNitrogenExcretionRateValue(managementPeriod
                        .AnimalType);
        }

        #endregion
    }
}