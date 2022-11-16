using H.Core.Emissions.Results;
using H.Core.Models.Animals;
using H.Core.Models;
using System;
using H.Core.Enumerations;

namespace H.Core.Services.Animals
{
    public abstract class BeefAndDairyResultsServiceBase : AnimalResultsServiceBase
    {
        protected void CalculateIndirectManureNitrousOxide(GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod,
            AnimalGroup animalGroup,
            Farm farm,
            DateTime dateTime,
            GroupEmissionsByDay previousDaysEmissions,
            double temperature)
        {
            if (animalGroup.GroupType.IsDairyCattleType())
            {
                dailyEmissions.FractionOfNitrogenExcretedInUrine = this.GetFractionOfNitrogenExcretedInUrineForDairy(
                    crudeProteinContent: managementPeriod.SelectedDiet.CrudeProteinContent,
                    animalType: animalGroup.GroupType);
            }
            else
            {
                // No equation for this when considering beef cattle as it is a lookup table in algorithm document
                dailyEmissions.FractionOfNitrogenExcretedInUrine = base.GetFractionOfNitrogenExcretedInUrine(
                    crudeProteinInDiet: managementPeriod.SelectedDiet.CrudeProteinContent);
            }

            dailyEmissions.TanExcretionRate = CalculateTANExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                fractionOfNitrogenExcretedInUrine: dailyEmissions.FractionOfNitrogenExcretedInUrine);

            dailyEmissions.TanExcretion = CalculateTANExcretion(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.FecalNitrogenExcretionRate = CalculateFecalNitrogenExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                tanExcretionRate: dailyEmissions.TanExcretionRate);

            dailyEmissions.FecalNitrogenExcretion = CalculateFecalNitrogenExcretion(
                fecalNitrogenExcretionRate: dailyEmissions.FecalNitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.OrganicNitrogenInStoredManure = CalculateOrganicNitrogenInStoredManure(
                totalNitrogenExcretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            /*
             * Ammonia (NH3) from housing
             */

            if (managementPeriod.HousingDetails.HousingType.IsIndoorHousing())
            {
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustmentForBarn(
                    averageMonthlyTemperature: temperature);
            }
            else
            {
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustmentNoBarn(
                    averageMonthlyTemperature: temperature);
            }

            var ammoniaEmissionFactorForHousingType = _beefDairyDefaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                housingType: managementPeriod.HousingDetails.HousingType);

            dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing = CalculateAdjustedEmissionFactorHousing(
                emissionFactor: ammoniaEmissionFactorForHousingType,
                temperatureAdjustment: dailyEmissions.AmbientAirTemperatureAdjustmentForHousing);

            dailyEmissions.AmmoniaEmissionRateFromHousing = CalculateAmmoniaEmissionRateFromHousing(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                adjustedEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing);

            dailyEmissions.AmmoniaConcentrationInHousing = CalculateAmmoniaConcentrationInHousing(
                emissionRate: dailyEmissions.AmmoniaEmissionRateFromHousing,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.AmmoniaEmissionsFromHousingSystem = CalculateTotalAmmoniaEmissionsFromHousing(
                ammoniaConcentrationInHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            dailyEmissions.AdjustedAmmoniaFromHousing = this.CalculateAdjustedAmmoniaFromHousing(dailyEmissions, managementPeriod);

            /*
             * Ammonia (NH3) from storage
             */

            if (managementPeriod.ManureDetails.StateType.IsSolidManure())
            {
                if (animalGroup.GroupType.IsBeefCattleType())
                {
                    dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = this.CalculateTemperatureAdjustmentForBeefCattleSolidStoredManure(
                        averageMonthlyTemperature: temperature);
                }
                else
                {
                    dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = this.CalculateTemperatureAdjustmentForDairyCattleSolidStoredManure(
                        temperature: temperature);
                }
            }
            else
            {
                dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = this.CalculateStorageTemperatureAdjustmentForLiquidManure(
                    temperature: temperature);
            }
            
            dailyEmissions.TanEnteringStorageSystem = CalculateTanFlowingIntoStorage(
                tanExcretion: dailyEmissions.TanExcretion,
                ammoniaLostFromHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            dailyEmissions.AdjustedAmountOfTanInStoredManure = CalculateAdjustedAmountOfTanFlowingIntoStorage(
                tanEnteringStorageSystem: dailyEmissions.TanEnteringStorageSystem,
                fractionOfTanImmoblizedToOrganicNitrogen: managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized,
                fractionOfTanNitrifiedDuringManureStorage: managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified,
                nitrogenExretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage: managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            dailyEmissions.TanInStorageOnDay = CalculateAmountOfTanInStorageOnDay(
                tanInStorageOnPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.TanInStorageOnDay,
                flowOfTanIntoStorage: dailyEmissions.AdjustedAmountOfTanInStoredManure);

            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                ambientTemperatureAdjustmentStorage: dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                ammoniaEmissionFactorStorage: managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage);

            dailyEmissions.AmmoniaLostFromStorage = CalculateAmmoniaLossFromStoredManure(
                tanInStoredManure: dailyEmissions.AdjustedAmountOfTanInStoredManure,
                adjustedAmmoniaEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            dailyEmissions.AmmoniaEmissionsFromStorageSystem = CalculateAmmoniaEmissionsFromStoredManure(
                ammoniaNitrogenLossFromStoredManure: dailyEmissions.AmmoniaLostFromStorage);

            dailyEmissions.AdjustedAmmoniaFromStorage = this.CalculateAdjustedAmmoniaFromStorage(dailyEmissions, managementPeriod);

            /*
             * Volatilization
             */

            if (managementPeriod.ManureDetails.UseCustomVolatilizationFraction)
            {
                dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;
            }
            else
            {
                dailyEmissions.FractionOfManureVolatilized = this.CalculateFractionOfManureVolatilized(
                    ammoniaEmissionsFromHousing: dailyEmissions.AmmoniaConcentrationInHousing,
                    ammoniaEmissionsFromStorage: dailyEmissions.AmmoniaLostFromStorage,
                    amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                    amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);
            }

            dailyEmissions.ManureVolatilizationRate = CalculateManureVolatilizationEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized,
                volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            dailyEmissions.ManureVolatilizationN2ONEmission = CalculateManureVolatilizationNitrogenEmission(
                volatilizationRate: dailyEmissions.ManureVolatilizationRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Leaching
             */

            // Equation 4.3.6-1
            dailyEmissions.ManureNitrogenLeachingRate = CalculateManureLeachingNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            // Equation 4.3.6-2
            dailyEmissions.ManureN2ONLeachingEmission = CalculateManureLeachingNitrogenEmission(
                leachingNitrogenEmissionRate: dailyEmissions.ManureNitrogenLeachingRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.6-3
            dailyEmissions.ManureNitrateLeachingEmission = CalculateNitrateLeaching(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                nitrogenBeddingRate: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching);
        }

        /// <summary>
        /// Equation 4.3.1-1
        /// Equation 4.3.1-2
        /// </summary>
        /// <param name="crudeProteinContent">The crude protein of the diet (fraction)</param>
        /// <param name="animalType">The type of dairy animal</param>
        /// <returns>Fraction of excreted N in the urine (urinary-N or urea-N fraction) (kg TAN (kg manure-N)-1)</returns>
        public double GetFractionOfNitrogenExcretedInUrineForDairy(
            double crudeProteinContent,
            AnimalType animalType)
        {
            var result = 0.0;

            if (animalType.IsLactatingType())
            {
                result = 3.296 * crudeProteinContent + 0.0084;
            }
            else
            {
                result = -19.26 * Math.Pow(crudeProteinContent, 2) + 6.62 * crudeProteinContent + 0.022;
            }

            if (result < 0)
            {
                return 0;
            }

            if (result > 1)
            {
                return 1;
            }

            return result;
        }

        /// <summary>
        /// Equation 4.3.2-5
        /// </summary>
        /// <param name="averageMonthlyTemperature">Average monthly temperature (°C)</param>
        /// <returns>Ambient temperature-based adjustments used to correct default NH3 emission factors for manure storage (compost, stockpile/deep bedding)</returns>
        public double CalculateTemperatureAdjustmentForBeefCattleSolidStoredManure(double averageMonthlyTemperature)
        {
            return Math.Pow(1.041, averageMonthlyTemperature + 2) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 4.3.2-6
        /// </summary>
        /// <param name="temperature">Average daily temperature (degrees Celsius)</param>
        /// <returns>Temperature adjustment for solid manure</returns>
        public double CalculateTemperatureAdjustmentForDairyCattleSolidStoredManure(double temperature)
        {
            return 1 - 0.058 * (17 - temperature);
        }

        /// <summary>
        /// Equation 4.3.2-7
        /// </summary>
        /// <param name="temperature">Average daily temperature (degrees Celsius)</param>
        /// <returns>Temperature adjustment for liquid manure</returns>
        public double CalculateStorageTemperatureAdjustmentForLiquidManure(double temperature)
        {
            return 1 - 0.058 * (15 - temperature);
        }
    }
}