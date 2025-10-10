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
                        growingSeasonPrecipitation: farm.ClimateData.GetGrowingSeasonPrecipitation(managementPeriod.Start.Year),
                        growingSeasonEvapotranspiration: farm.ClimateData.GetGrowingSeasonEvapotranspiration(managementPeriod.Start.Year));
                }
                else
                {
                    managementPeriod.ManureDetails.LeachingFraction = emissionData.LeachingFraction;
                }
            }
        }

        public void InitializeDailyTanExcretion(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeDailyTanExcretion(managementPeriod);
                }
            }
        }

        public void InitializeDailyTanExcretion(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                if (managementPeriod.AnimalType.IsPoultryType())
                {
                    managementPeriod.ManureDetails.DailyTanExcretion = _defaultDailyTanExcretionRatesForPoultry.GetDailyTanExcretionRate(animalType: managementPeriod.AnimalType);
                }
            }
        }

        public void InitializeAmmoniaEmissionFactorForManureStorage(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeAmmoniaEmissionFactorForManureStorage(managementPeriod);
                }
            }
        }

        public void InitializeAmmoniaEmissionFactorForManureStorage(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                if (managementPeriod.AnimalType.IsPoultryType())
                {
                    managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage = _defaultAmmoniaEmissionFactorsForPoultryManureStorageProvider.GetAmmoniaEmissionFactorForStorage(animalType: managementPeriod.AnimalType);
                }
                else if (managementPeriod.AnimalType.IsBeefCattleType() || managementPeriod.AnimalType.IsDairyCattleType())
                {
                    managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage = _beefDairyDefaultEmissionFactorsProvider.GetByManureStorageType(managementPeriod.ManureDetails.StateType);
                }
            }
        }

        public void InitializeAmmoniaEmissionFactorForHousing(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeAmmoniaEmissionFactorForHousing(managementPeriod);
                }
            }
        }

        public void InitializeAmmoniaEmissionFactorForHousing(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null)
            {
                if (managementPeriod.AnimalType.IsPoultryType())
                {
                    managementPeriod.HousingDetails.AmmoniaEmissionFactorForHousing = _defaultDailyTanExcretionRatesForPoultry.GetAmmoniaEmissionFactorForHousing(animalType: managementPeriod.AnimalType);
                }
            }
        }

        public void InitializeAmmoniaEmissionFactorForLandApplication(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeAmmoniaEmissionFactorForLandApplication(managementPeriod);
                }
            }
        }

        public void InitializeAmmoniaEmissionFactorForLandApplication(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                if (managementPeriod.AnimalType.IsPoultryType())
                {
                    managementPeriod.ManureDetails.AmmoniaEmissionFactorForLandApplication = _defaultAmmoniaEmissionFactorForPoultryLandAppliedManure.GetAmmoniaEmissionFactorForLandAppliedManure(animalType: managementPeriod.AnimalType);
                }
            }
        }

        #endregion
    }
}