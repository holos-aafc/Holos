using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        ///     Initialize the default manure mineralization fractions for all <see cref="ManagementPeriod" />s associated with
        ///     this <see cref="Farm" />.
        /// </summary>
        /// <param name="farm">The <see cref="Farm" /> containing the <see cref="ManagementPeriod" />s to initialize</param>
        public void InitializeManureMineralizationFractions(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeManureMineralizationFractions(managementPeriod);
        }

        /// <summary>
        ///     Initialize the manure fractions for the selected <see cref="ManagementPeriod" />.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod" /> that will have it's values reset to system defaults</param>
        public void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null &&
                managementPeriod.ManureDetails != null)
            {
                var fractions =
                    _fractionOrganicNMineralizedAsTanProvider.GetByStorageType(managementPeriod.ManureDetails.StateType,
                        managementPeriod.AnimalType);

                managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized = fractions.FractionImmobilized;
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified = fractions.FractionNitrified;
                managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized = fractions.FractionMineralized;
            }
        }

        public void InitializeLeachingFraction(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeLeachingFraction(farm, managementPeriod);
        }

        public void InitializeLeachingFraction(Farm farm, ManagementPeriod managementPeriod)
        {
            if (farm != null && managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                var emissionData = _livestockEmissionConversionFactorsProvider.GetFactors(
                    managementPeriod.ManureDetails.StateType,
                    farm.ClimateData.PrecipitationData.GetTotalAnnualPrecipitation(),
                    farm.ClimateData.TemperatureData.GetMeanAnnualTemperature(),
                    farm.ClimateData.EvapotranspirationData.GetTotalAnnualEvapotranspiration(),
                    managementPeriod.HousingDetails.UserDefinedBeddingRate,
                    managementPeriod.AnimalType,
                    farm,
                    managementPeriod.Start.Date.Year);

                if (managementPeriod.ManureDetails.StateType.IsGrazingArea())
                    managementPeriod.ManureDetails.LeachingFraction =
                        _nitrogenInputCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                            farm.ClimateData.PrecipitationData.CalculateGrowingSeasonPrecipitation(),
                            farm.ClimateData.EvapotranspirationData.CalculateGrowingSeasonEvapotranspiration());
                else
                    managementPeriod.ManureDetails.LeachingFraction = emissionData.LeachingFraction;
            }
        }

        public void InitializeDailyTanExcretion(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeDailyTanExcretion(managementPeriod);
        }

        public void InitializeDailyTanExcretion(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
                if (managementPeriod.AnimalType.IsPoultryType())
                    managementPeriod.ManureDetails.DailyTanExcretion =
                        _defaultDailyTanExcretionRatesForPoultry.GetDailyTanExcretionRate(managementPeriod.AnimalType);
        }

        public void InitializeAmmoniaEmissionFactorForManureStorage(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeAmmoniaEmissionFactorForManureStorage(managementPeriod);
        }

        public void InitializeAmmoniaEmissionFactorForManureStorage(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
            {
                if (managementPeriod.AnimalType.IsPoultryType())
                    managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage =
                        _defaultAmmoniaEmissionFactorsForPoultryManureStorageProvider
                            .GetAmmoniaEmissionFactorForStorage(managementPeriod.AnimalType);
                else if (managementPeriod.AnimalType.IsBeefCattleType() ||
                         managementPeriod.AnimalType.IsDairyCattleType())
                    managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage =
                        _beefDairyDefaultEmissionFactorsProvider.GetByManureStorageType(managementPeriod.ManureDetails
                            .StateType);
            }
        }

        public void InitializeAmmoniaEmissionFactorForHousing(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeAmmoniaEmissionFactorForHousing(managementPeriod);
        }

        public void InitializeAmmoniaEmissionFactorForHousing(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null)
                if (managementPeriod.AnimalType.IsPoultryType())
                    managementPeriod.HousingDetails.AmmoniaEmissionFactorForHousing =
                        _defaultDailyTanExcretionRatesForPoultry.GetAmmoniaEmissionFactorForHousing(managementPeriod
                            .AnimalType);
        }

        public void InitializeAmmoniaEmissionFactorForLandApplication(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeAmmoniaEmissionFactorForLandApplication(managementPeriod);
        }

        public void InitializeAmmoniaEmissionFactorForLandApplication(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.ManureDetails != null)
                if (managementPeriod.AnimalType.IsPoultryType())
                    managementPeriod.ManureDetails.AmmoniaEmissionFactorForLandApplication =
                        _defaultAmmoniaEmissionFactorForPoultryLandAppliedManure
                            .GetAmmoniaEmissionFactorForLandAppliedManure(managementPeriod.AnimalType);
        }

        #endregion
    }
}