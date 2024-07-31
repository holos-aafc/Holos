using H.Core.Enumerations;
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
                    this.InitializeManureMineralizationFractions(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Initialize the manure fractions for the selected <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will have it's values reset to system defaults</param>
        public void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null &&
                managementPeriod.ManureDetails != null)
            {
                var fractions = _fractionOrganicNMineralizedAsTanProvider.GetByStorageType(managementPeriod.ManureDetails.StateType, managementPeriod.AnimalType);

                managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized = fractions.FractionImmobilized;
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified = fractions.FractionNitrified;
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized = fractions.FractionMineralized;
            }
        }

        public void InitializeLeachingFraction(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeLeachingFraction(farm, managementPeriod);
                }
            }
        }

        public void InitializeLeachingFraction(Farm farm, ManagementPeriod managementPeriod)
        {
            if (farm != null && managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                var emissionData = _livestockEmissionConversionFactorsProvider.GetFactors(manureStateType: managementPeriod.ManureDetails.StateType,
                    meanAnnualPrecipitation: farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation(),
                    meanAnnualTemperature: farm.ClimateData.TemperatureData.GetMeanAnnualTemperature(),
                    meanAnnualEvapotranspiration: farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration(),
                    beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                    animalType: managementPeriod.AnimalType,
                    farm: farm,
                    year: managementPeriod.Start.Date.Year);

                if (managementPeriod.ManureDetails.StateType.IsGrazingArea())
                {
                    managementPeriod.ManureDetails.LeachingFraction = _nitrogenInputCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                        growingSeasonPrecipitation: farm.ClimateData.PrecipitationData.CalculateGrowingSeasonPrecipitation(),
                        growingSeasonEvapotranspiration: farm.ClimateData.EvapotranspirationData.CalculateGrowingSeasonEvapotranspiration());
                }
                else
                {
                    managementPeriod.ManureDetails.LeachingFraction = emissionData.LeachingFraction;
                }
            }
        }

        #endregion
    }
}