using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Services.Animals
{
    public partial class AnimalResultsServiceBase
    {
        #region Public Methods

        /// <summary>
        /// Checks if dairy or beef cattle animals are grazing during the <see cref="ManagementPeriod"/> and calculates manure direct/indirect emissions.
        /// </summary>
        public void GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
            ManagementPeriod managementPeriod,
            double temperature,
            GroupEmissionsByDay groupEmissionsByDay)
        {
            if (managementPeriod.HousingDetails.HousingType == HousingType.Pasture)
            {
                groupEmissionsByDay.TotalAmountOfNitrogenForDay = groupEmissionsByDay.AccumulatedNitrogenAvailableForLandApplicationOnDay;

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

                groupEmissionsByDay.ManureDirectN2ONEmission = this.CalculateManureDirectNitrogenEmission(
                    manureDirectNitrogenEmissionRate: groupEmissionsByDay.ManureDirectN2ONEmissionRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                var temperatureAdjustmentForGrazing = GetAmbientTemperatureAdjustmentForGrazing(
                    temperature: temperature);

                var emissionFactorForGrazing = _defaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                    housingType: managementPeriod.HousingDetails.HousingType);

                var adjustedAmmoniaEmissionFactor = GetAdjustedEmissionFactorForGrazing(
                    emissionFactor: emissionFactorForGrazing,
                    temperatureAdjustment: temperatureAdjustmentForGrazing);

                var ammoniaEmissionRateFromGrazingAnimals = GetAmmoniaEmissionRateForGrazingAnimals(
                    tanExcretionRate: groupEmissionsByDay.TanExcretionRate,
                    adjustedEmissionFactor: adjustedAmmoniaEmissionFactor);

                var nH3NFromGrazingAnimals = GetTotalAmmoniaProductionFromGrazingAnimals(
                    ammoniaEmissionRate: ammoniaEmissionRateFromGrazingAnimals,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.NH3FromGrazingAnimals = CoreConstants.ConvertToNH3(nH3NFromGrazingAnimals);

                // Equation 5.4.3-1
                var volatilizationFraction = nH3NFromGrazingAnimals / groupEmissionsByDay.AmountOfNitrogenExcreted;

                groupEmissionsByDay.ManureVolatilizationRate = groupEmissionsByDay.ManureVolatilizationRate = CalculateManureVolatilizationEmissionRate(
                    nitrogenExcretionRate: groupEmissionsByDay.NitrogenExcretionRate,
                    beddingNitrogen: 0,
                    volatilizationFraction: volatilizationFraction,
                    volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

                groupEmissionsByDay.ManureVolatilizationN2ONEmission = this.CalculateManureVolatilizationNitrogenEmission(
                    volatilizationRate: groupEmissionsByDay.ManureVolatilizationRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.AdjustedNH3NFromHousing = CalculateAdjustedAmmoniaEmissionFromGrazingAnimals(
                    nH3NFromGrazingAnimals: nH3NFromGrazingAnimals,
                    volatilizationN2ON: groupEmissionsByDay.ManureVolatilizationN2ONEmission);

                // Equation 5.4.3-7
                var adjustedNH3FromHousing = CoreConstants.ConvertToNH3(groupEmissionsByDay.AdjustedNH3NFromHousing);

                groupEmissionsByDay.ManureNitrogenLeachingRate = this.CalculateManureLeachingNitrogenEmissionRate(
                    nitrogenExcretionRate: groupEmissionsByDay.NitrogenExcretionRate,
                    leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                    emissionFactorForLeaching: 0.011,
                    nitrogenBeddingRate: 0);

                groupEmissionsByDay.ManureNitrateLeachingEmission = this.CalculateNitrateLeaching(
                    nitrogenExcretionRate: groupEmissionsByDay.NitrogenExcretionRate,
                    nitrogenBeddingRate: groupEmissionsByDay.RateOfNitrogenAddedFromBeddingMaterial,
                    leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                    emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching
                    );
                
                groupEmissionsByDay.ManureN2ONLeachingEmission = this.CalculateManureLeachingNitrogenEmission(
                    leachingNitrogenEmissionRate: groupEmissionsByDay.ManureNitrogenLeachingRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.ManureIndirectN2ONEmission = this.CalculateManureIndirectNitrogenEmission(
                    manureLeachingNitrogenEmission: groupEmissionsByDay.ManureN2ONLeachingEmission,
                    manureVolatilizationNitrogenEmission: groupEmissionsByDay.ManureVolatilizationN2ONEmission);

                groupEmissionsByDay.ManureN2ONEmission = this.CalculateManureNitrogenEmission(
                    manureDirectNitrogenEmission: groupEmissionsByDay.ManureDirectN2ONEmission,
                    manureIndirectNitrogenEmission: groupEmissionsByDay.ManureIndirectN2ONEmission);
            }
        }

        /// <summary>
        /// Checks if sheep, swine, poultry, or other livestock animals are grazing during the <see cref="ManagementPeriod"/> and calculates ammonia emissions from grazing animals as required.
        /// </summary>
        public void GetEmissionsFromGrazingSheepSwineAndOtherLiveStock(
            ManagementPeriod managementPeriod,
            GroupEmissionsByDay groupEmissionsByDay)
        {
            if (managementPeriod.HousingDetails.HousingType == HousingType.Pasture &&
                managementPeriod.HousingDetails.PastureLocation != null)
            {
                // Equation 5.3.1-2
                groupEmissionsByDay.ManureDirectN2ONEmissionRate = groupEmissionsByDay.NitrogenExcretionRate *
                                                                   managementPeriod.ManureDetails.N2ODirectEmissionFactor;

                groupEmissionsByDay.ManureDirectN2ONEmission = this.CalculateManureDirectNitrogenEmission(
                    manureDirectNitrogenEmissionRate: groupEmissionsByDay.ManureDirectN2ONEmissionRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                var nH3NRateFromGrazingAnimals = GetAmmoniaEmissionRateFromAnimalsOnPasture(
                    nitrogenExcretionRate: groupEmissionsByDay.NitrogenExcretionRate,
                    volatilizationFraction: groupEmissionsByDay.FractionOfManureVolatilized);

                var nH3NFromGrazingAnimals = GetTotalAmmoniaProductionFromGrazingAnimals(
                    ammoniaEmissionRate: nH3NRateFromGrazingAnimals,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.NH3FromGrazingAnimals = CoreConstants.ConvertToNH3(nH3NFromGrazingAnimals);

                groupEmissionsByDay.ManureVolatilizationRate = CalculateManureVolatilizationEmissionRate(
                    nitrogenExcretionRate: groupEmissionsByDay.NitrogenExcretionRate,
                    beddingNitrogen: 0,
                    volatilizationFraction: groupEmissionsByDay.FractionOfManureVolatilized,
                    volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

                groupEmissionsByDay.ManureVolatilizationN2ONEmission = this.CalculateManureVolatilizationNitrogenEmission(
                    volatilizationRate: groupEmissionsByDay.ManureVolatilizationRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.AdjustedNH3NFromHousing = CalculateAdjustedAmmoniaEmissionFromGrazingAnimals(
                    nH3NFromGrazingAnimals: nH3NFromGrazingAnimals,
                    volatilizationN2ON: groupEmissionsByDay.ManureVolatilizationN2ONEmission);

                groupEmissionsByDay.ManureNitrogenLeachingRate = this.CalculateManureLeachingNitrogenEmissionRate(
                    nitrogenExcretionRate: groupEmissionsByDay.NitrogenExcretionRate,
                    leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                    emissionFactorForLeaching: 0.011,
                    nitrogenBeddingRate: 0);

                groupEmissionsByDay.ManureNitrateLeachingEmission = this.CalculateManureLeachingNitrogenEmissionRate(
                    nitrogenExcretionRate: groupEmissionsByDay.NitrogenExcretionRate,
                    leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                    emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching,
                    nitrogenBeddingRate: 0);

                groupEmissionsByDay.ManureN2ONLeachingEmission = this.CalculateManureLeachingNitrogenEmission(
                    leachingNitrogenEmissionRate: groupEmissionsByDay.ManureNitrogenLeachingRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                groupEmissionsByDay.ManureIndirectN2ONEmission = this.CalculateManureIndirectNitrogenEmission(
                    manureLeachingNitrogenEmission: groupEmissionsByDay.ManureN2ONLeachingEmission,
                    manureVolatilizationNitrogenEmission: groupEmissionsByDay.ManureVolatilizationN2ONEmission);

                groupEmissionsByDay.ManureN2ONEmission = this.CalculateManureNitrogenEmission(
                    manureDirectNitrogenEmission: groupEmissionsByDay.ManureDirectN2ONEmission,
                    manureIndirectNitrogenEmission: groupEmissionsByDay.ManureIndirectN2ONEmission);
            }
        }

        #endregion
    }
}
