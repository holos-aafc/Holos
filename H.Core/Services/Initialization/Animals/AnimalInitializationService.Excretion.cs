using H.Core.Models.Animals;
using H.Core.Models;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize the default manure excretion rate for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeManureExcretionRate(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeManureExcretionRate(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the default <see cref="ManureDetails.ManureExcretionRate"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> to initialize with a default <see cref="ManureDetails.ManureExcretionRate"/></param>
        public void InitializeManureExcretionRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null)
            {
                managementPeriod.ManureDetails.ManureExcretionRate = _defaultManureExcretionRateProvider.GetManureExcretionRate(managementPeriod.AnimalType);
            }
        }

        #endregion
    }
}