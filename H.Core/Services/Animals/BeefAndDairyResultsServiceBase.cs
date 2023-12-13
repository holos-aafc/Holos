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
                if (managementPeriod.AnimalType.IsDairyCattleType())
                {
                    if (managementPeriod.HousingDetails.UseCustomIndoorHousingTemperature == false)
                    {
                        temperature = _indoorTemperatureProvider.GetTemperature(dateTime);
                    }
                    else
                    {
                        temperature = managementPeriod.HousingDetails.IndoorHousingTemperature;
                    }
                }

                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustmentForIndoorHousing(
                    dailyTemperature: temperature);
            }
            else
            {
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustmentNoBarn(
                    averageMonthlyTemperature: temperature);
            }

            var ammoniaEmissionFactorForHousingType = _defaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                housingType: managementPeriod.HousingDetails.HousingType);

            dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing = CalculateAdjustedEmissionFactorHousing(
                emissionFactor: ammoniaEmissionFactorForHousingType,
                temperatureAdjustment: dailyEmissions.AmbientAirTemperatureAdjustmentForHousing);

            base.CalculateAmmoniaInHousing(dailyEmissions, managementPeriod, dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing);

            dailyEmissions.TanEnteringStorageSystem = CalculateTanFlowingIntoStorageEachDay(
                tanExcretion: dailyEmissions.TanExcretion,
                ammoniaLostFromHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            var adjustedAmountOfTanFlowingIntoStorageEachDay = CalculateAdjustedAmountOfTanFlowingIntoStorageEachDay(
                tanEnteringStorageSystem: dailyEmissions.TanEnteringStorageSystem,
                fractionOfTanImmobilizedToOrganicNitrogen: managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized,
                fractionOfTanNitrifiedDuringManureStorage: managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified,
                nitrogenExcretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage: managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding);

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

            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = base.CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                ambientTemperatureAdjustmentStorage: dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                ammoniaEmissionFactorStorage: managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage);

            dailyEmissions.AmmoniaLostFromStorage = base.CalculateAmmoniaLossFromStoredManure(
                adjustedAmountOfTanFlowingIntoStorageEachDay,
                dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            dailyEmissions.AmmoniaEmissionsFromStorageSystem = CoreConstants.ConvertToNH3(
                amountOfNH3N: dailyEmissions.AmmoniaLostFromStorage);

            dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay = this.CalculateAdjustedAmountOfTANEnteringStorage(
                amountOfTANFlowingIntoStorageEachDay: adjustedAmountOfTanFlowingIntoStorageEachDay, 
                adjustedAmmoniaLossFromStorage: dailyEmissions.AmmoniaLostFromStorage);

            dailyEmissions.AccumulatedTanInStorageOnDay = CalculateAmountOfTanInStorageOnDay(
                tanInStorageOnPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedTanInStorageOnDay,
                flowOfTanEnteringStorage: dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay);

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

            if (managementPeriod.HousingDetails.HousingType.IsPasture())
            {
                dailyEmissions.FractionOfManureVolatilized = 0;
            }

            base.CalculateVolatilizationEmissions(dailyEmissions, managementPeriod);

            /*
             * Leaching
             */

            base.CalculateLeachingEmissions(dailyEmissions, managementPeriod);
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
        /// Equation 4.3.2-3
        /// </summary>
        /// <param name="averageMonthlyTemperature">Average monthly temperature (°C)</param>
        /// <returns>Ambient temperature-based adjustments used to correct default NH3 emission factors for manure storage (compost, stockpile/deep bedding)</returns>
        public double CalculateTemperatureAdjustmentForBeefCattleSolidStoredManure(double averageMonthlyTemperature)
        {
            return Math.Pow(1.041, averageMonthlyTemperature + 2) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 4.3.2-4
        /// </summary>
        /// <param name="temperature">Average daily temperature (degrees Celsius)</param>
        /// <returns>Temperature adjustment for solid manure</returns>
        public double CalculateTemperatureAdjustmentForDairyCattleSolidStoredManure(double temperature)
        {
            return 1 - 0.058 * (17 - temperature);
        }

        /// <summary>
        /// Equation 4.3.2-5
        /// </summary>
        /// <param name="temperature">Average daily temperature (degrees Celsius)</param>
        /// <returns>Temperature adjustment for liquid manure</returns>
        public double CalculateStorageTemperatureAdjustmentForLiquidManure(double temperature)
        {
            return 1 - 0.058 * (15 - temperature);
        }
    }
}