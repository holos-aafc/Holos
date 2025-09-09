using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    public partial class AnimalResultsServiceBase
    {
        #region Public Methods

        /// <summary>
        ///     Checks if dairy or beef cattle animals are grazing during the <see cref="ManagementPeriod" /> and calculates manure
        ///     direct/indirect emissions.
        /// </summary>
        public void GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
            ManagementPeriod managementPeriod,
            double temperature,
            GroupEmissionsByDay groupEmissionsByDay)
        {
            if (managementPeriod.HousingDetails.HousingType == HousingType.Pasture)
            {
                groupEmissionsByDay.TotalAmountOfNitrogenForDay =
                    groupEmissionsByDay.AccumulatedNitrogenAvailableForLandApplicationOnDay;

                groupEmissionsByDay.TanEnteringStorageSystem = 0;
                groupEmissionsByDay.AdjustedAmmoniaEmissionFactorForHousing = 0;
                groupEmissionsByDay.AmbientAirTemperatureAdjustmentForStorage = 0;
                groupEmissionsByDay.AdjustedAmmoniaEmissionFactorForStorage = 0;
                groupEmissionsByDay.AmmoniaEmissionsFromStorageSystem = 0;
                groupEmissionsByDay.AdjustedAmountOfTanInStoredManureOnDay = 0;
                groupEmissionsByDay.AccumulatedTanInStorageOnDay = 0;
                groupEmissionsByDay.AccumulatedTANAvailableForLandApplicationOnDay = 0;
                groupEmissionsByDay.AdjustedAmmoniaFromStorage = 0;
                groupEmissionsByDay.AccumulatedNitrogenAvailableForLandApplicationOnDay = 0;
                groupEmissionsByDay.ManureCarbonNitrogenRatio = 0;
                groupEmissionsByDay.TotalVolumeOfManureAvailableForLandApplication = 0;
                groupEmissionsByDay.AccumulatedVolume = 0;

                // Equation 5.3.1-1
                groupEmissionsByDay.ManureDirectN2ONEmissionRate = groupEmissionsByDay.NitrogenExcretionRate *
                                                                   managementPeriod.ManureDetails
                                                                       .N2ODirectEmissionFactor;

                groupEmissionsByDay.ManureDirectN2ONEmission = CalculateManureDirectNitrogenEmission(
                    groupEmissionsByDay.ManureDirectN2ONEmissionRate,
                    managementPeriod.NumberOfAnimals);

                var temperatureAdjustmentForGrazing = GetAmbientTemperatureAdjustmentForGrazing(
                    temperature);

                var emissionFactorForGrazing = _defaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                    managementPeriod.HousingDetails.HousingType);

                var adjustedAmmoniaEmissionFactor = GetAdjustedEmissionFactorForGrazing(
                    emissionFactorForGrazing,
                    temperatureAdjustmentForGrazing);

                var ammoniaEmissionRateFromGrazingAnimals = GetAmmoniaEmissionRateForGrazingAnimals(
                    groupEmissionsByDay.TanExcretionRate,
                    adjustedAmmoniaEmissionFactor);

                var nH3NFromGrazingAnimals = GetTotalAmmoniaProductionFromGrazingAnimals(
                    ammoniaEmissionRateFromGrazingAnimals,
                    managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.NH3FromGrazingAnimals = CoreConstants.ConvertToNH3(nH3NFromGrazingAnimals);

                // Equation 5.4.3-1
                // Will be zero if weight entered by user is 0
                var volatilizationFraction = nH3NFromGrazingAnimals / (groupEmissionsByDay.AmountOfNitrogenExcreted <= 0
                    ? 1
                    : groupEmissionsByDay.AmountOfNitrogenExcreted);

                groupEmissionsByDay.ManureVolatilizationRate = groupEmissionsByDay.ManureVolatilizationRate =
                    CalculateManureVolatilizationEmissionRate(
                        groupEmissionsByDay.NitrogenExcretionRate,
                        0,
                        volatilizationFraction,
                        managementPeriod.ManureDetails.EmissionFactorVolatilization);

                groupEmissionsByDay.ManureVolatilizationN2ONEmission = CalculateManureVolatilizationNitrogenEmission(
                    groupEmissionsByDay.ManureVolatilizationRate,
                    managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.AdjustedNH3NFromHousing = CalculateAdjustedAmmoniaEmissionFromGrazingAnimals(
                    nH3NFromGrazingAnimals,
                    groupEmissionsByDay.ManureVolatilizationN2ONEmission);

                // Equation 5.4.3-7
                var adjustedNH3FromHousing = CoreConstants.ConvertToNH3(groupEmissionsByDay.AdjustedNH3NFromHousing);

                groupEmissionsByDay.ManureNitrogenLeachingRate = CalculateManureLeachingNitrogenEmissionRate(
                    groupEmissionsByDay.NitrogenExcretionRate,
                    managementPeriod.ManureDetails.LeachingFraction,
                    0.011,
                    0);

                groupEmissionsByDay.ManureNitrateLeachingEmission = CalculateNitrateLeaching(
                    groupEmissionsByDay.NitrogenExcretionRate,
                    groupEmissionsByDay.RateOfNitrogenAddedFromBeddingMaterial,
                    managementPeriod.ManureDetails.LeachingFraction,
                    managementPeriod.ManureDetails.EmissionFactorLeaching
                );

                groupEmissionsByDay.ManureN2ONLeachingEmission = CalculateManureLeachingNitrogenEmission(
                    groupEmissionsByDay.ManureNitrogenLeachingRate,
                    managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.ManureIndirectN2ONEmission = CalculateManureIndirectNitrogenEmission(
                    manureLeachingNitrogenEmission: groupEmissionsByDay.ManureN2ONLeachingEmission,
                    manureVolatilizationNitrogenEmission: groupEmissionsByDay.ManureVolatilizationN2ONEmission);

                groupEmissionsByDay.ManureN2ONEmission = CalculateManureNitrogenEmission(
                    groupEmissionsByDay.ManureDirectN2ONEmission,
                    groupEmissionsByDay.ManureIndirectN2ONEmission);
            }
        }

        /// <summary>
        ///     Checks if sheep, swine, poultry, or other livestock animals are grazing during the <see cref="ManagementPeriod" />
        ///     and calculates ammonia emissions from grazing animals as required.
        /// </summary>
        public void GetEmissionsFromGrazingSheepSwineAndOtherLiveStock(
            ManagementPeriod managementPeriod,
            GroupEmissionsByDay groupEmissionsByDay)
        {
            if (managementPeriod.HousingDetails.HousingType == HousingType.Pasture)
            {
                groupEmissionsByDay.TotalAmountOfNitrogenForDay =
                    groupEmissionsByDay.AccumulatedNitrogenAvailableForLandApplicationOnDay;

                groupEmissionsByDay.TanEnteringStorageSystem = 0;
                groupEmissionsByDay.AdjustedAmmoniaEmissionFactorForHousing = 0;
                groupEmissionsByDay.AmbientAirTemperatureAdjustmentForStorage = 0;
                groupEmissionsByDay.AdjustedAmmoniaEmissionFactorForStorage = 0;
                groupEmissionsByDay.AmmoniaEmissionsFromStorageSystem = 0;
                groupEmissionsByDay.AdjustedAmountOfTanInStoredManureOnDay = 0;
                groupEmissionsByDay.AccumulatedTanInStorageOnDay = 0;
                groupEmissionsByDay.AccumulatedTANAvailableForLandApplicationOnDay = 0;
                groupEmissionsByDay.AdjustedAmmoniaFromStorage = 0;
                groupEmissionsByDay.AccumulatedNitrogenAvailableForLandApplicationOnDay = 0;
                groupEmissionsByDay.ManureCarbonNitrogenRatio = 0;
                groupEmissionsByDay.TotalVolumeOfManureAvailableForLandApplication = 0;
                groupEmissionsByDay.AccumulatedVolume = 0;

                // Equation 5.3.1-2
                groupEmissionsByDay.ManureDirectN2ONEmissionRate = groupEmissionsByDay.NitrogenExcretionRate *
                                                                   managementPeriod.ManureDetails
                                                                       .N2ODirectEmissionFactor;

                groupEmissionsByDay.ManureDirectN2ONEmission = CalculateManureDirectNitrogenEmission(
                    groupEmissionsByDay.ManureDirectN2ONEmissionRate,
                    managementPeriod.NumberOfAnimals);

                var nH3NRateFromGrazingAnimals = GetAmmoniaEmissionRateFromAnimalsOnPasture(
                    groupEmissionsByDay.NitrogenExcretionRate,
                    groupEmissionsByDay.FractionOfManureVolatilized);

                var nH3NFromGrazingAnimals = GetTotalAmmoniaProductionFromGrazingAnimals(
                    nH3NRateFromGrazingAnimals,
                    managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.NH3FromGrazingAnimals = CoreConstants.ConvertToNH3(nH3NFromGrazingAnimals);

                groupEmissionsByDay.ManureVolatilizationRate = CalculateManureVolatilizationEmissionRate(
                    groupEmissionsByDay.NitrogenExcretionRate,
                    0,
                    groupEmissionsByDay.FractionOfManureVolatilized,
                    managementPeriod.ManureDetails.EmissionFactorVolatilization);

                groupEmissionsByDay.ManureVolatilizationN2ONEmission = CalculateManureVolatilizationNitrogenEmission(
                    groupEmissionsByDay.ManureVolatilizationRate,
                    managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.AdjustedNH3NFromHousing = CalculateAdjustedAmmoniaEmissionFromGrazingAnimals(
                    nH3NFromGrazingAnimals,
                    groupEmissionsByDay.ManureVolatilizationN2ONEmission);

                groupEmissionsByDay.ManureNitrogenLeachingRate = CalculateManureLeachingNitrogenEmissionRate(
                    groupEmissionsByDay.NitrogenExcretionRate,
                    managementPeriod.ManureDetails.LeachingFraction,
                    0.011,
                    0);

                groupEmissionsByDay.ManureNitrateLeachingEmission = CalculateManureLeachingNitrogenEmissionRate(
                    groupEmissionsByDay.NitrogenExcretionRate,
                    managementPeriod.ManureDetails.LeachingFraction,
                    managementPeriod.ManureDetails.EmissionFactorLeaching,
                    0);

                groupEmissionsByDay.ManureN2ONLeachingEmission = CalculateManureLeachingNitrogenEmission(
                    groupEmissionsByDay.ManureNitrogenLeachingRate,
                    managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.ManureIndirectN2ONEmission = CalculateManureIndirectNitrogenEmission(
                    manureLeachingNitrogenEmission: groupEmissionsByDay.ManureN2ONLeachingEmission,
                    manureVolatilizationNitrogenEmission: groupEmissionsByDay.ManureVolatilizationN2ONEmission);

                groupEmissionsByDay.ManureN2ONEmission = CalculateManureNitrogenEmission(
                    groupEmissionsByDay.ManureDirectN2ONEmission,
                    groupEmissionsByDay.ManureIndirectN2ONEmission);
            }
        }

        #endregion
    }
}