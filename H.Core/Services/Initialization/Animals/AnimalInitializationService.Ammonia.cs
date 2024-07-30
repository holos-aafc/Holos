using H.Core.Models.Animals;
using H.Core.Models;
using H.Core.Providers.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize the default manure mineralization fractions for all <see cref="ManagementPeriod"/>s associated with this <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="ManagementPeriod"/>s to initialize</param>
        public void InitializeManureMineralizationFractions(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var fractions = _fractionOrganicNMineralizedAsTanProvider.GetByStorageType(managementPeriod.ManureDetails.StateType, managementPeriod.AnimalType);
                    this.InitializeManureMineralizationFractions(managementPeriod, fractions);
                }
            }
        }

        /// <summary>
        /// Initialize the manure fractions for the selected <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will have it's values reset to system defaults</param>
        /// <param name="mineralizationFractions">The <see cref="FractionOfOrganicNitrogenMineralizedData"/> containing the new default values to use</param>
        public void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod, FractionOfOrganicNitrogenMineralizedData mineralizationFractions)
        {
            if (managementPeriod != null &&
                managementPeriod.ManureDetails != null &&
                mineralizationFractions != null)
            {
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized = mineralizationFractions.FractionImmobilized;
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified = mineralizationFractions.FractionNitrified;
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized = mineralizationFractions.FractionMineralized;
            }
        }

        #endregion
    }
}