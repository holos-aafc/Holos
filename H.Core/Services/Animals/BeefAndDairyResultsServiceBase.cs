using System;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    public abstract class BeefAndDairyResultsServiceBase : AnimalResultsServiceBase
    {
        protected void CalculateIndirectManureNitrousOxide(GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod,
            AnimalGroup animalGroup,
            DateTime dateTime,
            GroupEmissionsByDay previousDaysEmissions,
            double temperature,
            Farm farm)
        {
            if (animalGroup.GroupType.IsDairyCattleType())
                dailyEmissions.FractionOfNitrogenExcretedInUrine = GetFractionOfNitrogenExcretedInUrineForDairy(
                    managementPeriod.SelectedDiet.CrudeProteinContent,
                    animalGroup.GroupType);
            else
                // No equation for this when considering beef cattle as it is a lookup table in algorithm document
                dailyEmissions.FractionOfNitrogenExcretedInUrine = GetFractionOfNitrogenExcretedInUrine(
                    managementPeriod.SelectedDiet.CrudeProteinContent);

            dailyEmissions.TanExcretionRate = CalculateTANExcretionRate(
                dailyEmissions.NitrogenExcretionRate,
                dailyEmissions.FractionOfNitrogenExcretedInUrine);

            dailyEmissions.TanExcretion = CalculateTANExcretion(
                dailyEmissions.TanExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.FecalNitrogenExcretionRate = CalculateFecalNitrogenExcretionRate(
                dailyEmissions.NitrogenExcretionRate,
                dailyEmissions.TanExcretionRate);

            dailyEmissions.FecalNitrogenExcretion = CalculateFecalNitrogenExcretion(
                dailyEmissions.FecalNitrogenExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.OrganicNitrogenInStoredManure = CalculateOrganicNitrogenInStoredManure(
                dailyEmissions.FecalNitrogenExcretion,
                dailyEmissions.AmountOfNitrogenAddedFromBedding);

            /*
             * Ammonia (NH3) from housing
             */

            if (managementPeriod.HousingDetails.HousingType.IsIndoorHousing())
            {
                var housingTemperature = temperature;
                if (managementPeriod.AnimalType.IsDairyCattleType())
                {
                    if (managementPeriod.HousingDetails.UseCustomIndoorHousingTemperature == false &&
                        farm.ClimateData.BarnTemperatureData != null)
                    {
                        var month = (Months)dateTime.Month;
                        housingTemperature = farm.ClimateData.BarnTemperatureData.GetValueByMonth(month);
                    }
                    else
                    {
                        housingTemperature = managementPeriod.HousingDetails.IndoorHousingTemperature;
                    }
                }

                var isIndoorDairyHousing = managementPeriod.HousingDetails.HousingType.IsIndoorHousing() &&
                                           managementPeriod.AnimalType.IsDairyCattleType();
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing =
                    CalculateAmbientTemperatureAdjustmentForIndoorHousing(
                        housingTemperature,
                        isIndoorDairyHousing);
            }
            else
            {
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustmentNoBarn(
                    temperature);
            }

            var ammoniaEmissionFactorForHousingType = _defaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                managementPeriod.HousingDetails.HousingType);

            dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing = CalculateAdjustedEmissionFactorHousing(
                ammoniaEmissionFactorForHousingType,
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing);

            CalculateAmmoniaInHousing(dailyEmissions, managementPeriod,
                dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing);

            dailyEmissions.TanEnteringStorageSystem = CalculateTanFlowingIntoStorageEachDay(
                dailyEmissions.TanExcretion,
                dailyEmissions.AmmoniaConcentrationInHousing);

            dailyEmissions.AdjustedAmountOfTanFlowingIntoStorageEachDay =
                CalculateAdjustedAmountOfTanFlowingIntoStorageEachDay(
                    dailyEmissions.TanEnteringStorageSystem,
                    managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized,
                    managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified,
                    dailyEmissions.FecalNitrogenExcretion,
                    managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized,
                    dailyEmissions.AmountOfNitrogenAddedFromBedding);

            if (managementPeriod.ManureDetails.StateType.IsSolidManure())
            {
                if (animalGroup.GroupType.IsBeefCattleType())
                    dailyEmissions.AmbientAirTemperatureAdjustmentForStorage =
                        CalculateTemperatureAdjustmentForBeefCattleSolidStoredManure(
                            temperature);
                else
                    dailyEmissions.AmbientAirTemperatureAdjustmentForStorage =
                        CalculateTemperatureAdjustmentForDairyCattleSolidStoredManure(
                            temperature);
            }
            else
            {
                dailyEmissions.AmbientAirTemperatureAdjustmentForStorage =
                    CalculateStorageTemperatureAdjustmentForLiquidManure(
                        temperature);
            }

            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage);

            dailyEmissions.AmmoniaLostFromStorage = CalculateAmmoniaLossFromStoredManure(
                dailyEmissions.AdjustedAmountOfTanFlowingIntoStorageEachDay,
                dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            dailyEmissions.AmmoniaEmissionsFromStorageSystem = CoreConstants.ConvertToNH3(
                dailyEmissions.AmmoniaLostFromStorage);

            dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay = CalculateAdjustedAmountOfTANEnteringStorage(
                dailyEmissions.AdjustedAmountOfTanFlowingIntoStorageEachDay,
                dailyEmissions.AmmoniaLostFromStorage);

            dailyEmissions.AccumulatedTanInStorageOnDay = CalculateAmountOfTanInStorageOnDay(
                previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedTanInStorageOnDay,
                dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay);

            dailyEmissions.AdjustedAmmoniaFromStorage =
                CalculateAdjustedAmmoniaFromStorage(dailyEmissions, managementPeriod);

            /*
             * Volatilization
             */

            if (managementPeriod.ManureDetails.UseCustomVolatilizationFraction)
                dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;
            else
                dailyEmissions.FractionOfManureVolatilized = CalculateFractionOfManureVolatilized(
                    dailyEmissions.AmmoniaConcentrationInHousing,
                    dailyEmissions.AmmoniaLostFromStorage,
                    dailyEmissions.AmountOfNitrogenExcreted,
                    dailyEmissions.AmountOfNitrogenAddedFromBedding);

            if (managementPeriod.HousingDetails.HousingType.IsPasture()) dailyEmissions.FractionOfManureVolatilized = 0;

            CalculateVolatilizationEmissions(dailyEmissions, managementPeriod);

            /*
             * Leaching
             */

            CalculateLeachingEmissions(dailyEmissions, managementPeriod);
        }

        /// <summary>
        ///     Equation 4.3.1-1
        ///     Equation 4.3.1-2
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
                result = 3.296 * crudeProteinContent + 0.0084;
            else
                result = -19.26 * Math.Pow(crudeProteinContent, 2) + 6.62 * crudeProteinContent + 0.022;

            if (result < 0) return 0;

            if (result > 1) return 1;

            return result;
        }

        /// <summary>
        ///     Equation 4.3.2-3
        /// </summary>
        /// <param name="temperature">Average daily outdoor temperature (°C)</param>
        /// <returns>
        ///     Ambient temperature-based adjustments used to correct default NH3 emission factors for manure storage
        ///     (compost, stockpile/deep bedding)
        /// </returns>
        public double CalculateTemperatureAdjustmentForBeefCattleSolidStoredManure(double temperature)
        {
            return Math.Pow(1.041, temperature + 2) / Math.Pow(1.041, 15);
        }

        /// <summary>
        ///     Equation 4.3.2-4
        /// </summary>
        /// <param name="temperature">Average daily outdoor temperature (°C)</param>
        /// <returns>Temperature adjustment for solid manure</returns>
        public double CalculateTemperatureAdjustmentForDairyCattleSolidStoredManure(double temperature)
        {
            return 1 - 0.058 * (17 - temperature);
        }

        /// <summary>
        ///     Equation 4.3.2-5
        /// </summary>
        /// <param name="temperature">Average daily outdoor temperature (°C)</param>
        /// <returns>Temperature adjustment for liquid manure</returns>
        public double CalculateStorageTemperatureAdjustmentForLiquidManure(double temperature)
        {
            return 1 - 0.058 * (15 - temperature);
        }
    }
}