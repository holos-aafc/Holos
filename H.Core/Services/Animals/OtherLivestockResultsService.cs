﻿using System;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    /// <summary>
    ///     The main class to calculate emissions from animals such as horses, deer, etc.
    /// </summary>
    public class OtherLivestockResultsService : AnimalResultsServiceBase, IOtherLivestockResultsService
    {
        #region Constructors

        public OtherLivestockResultsService()
        {
            _animalComponentCategory = ComponentCategory.OtherLivestock;
        }

        #endregion

        #region Private Methods

        protected override GroupEmissionsByDay CalculateDailyEmissions(
            AnimalComponentBase animalComponentBase,
            ManagementPeriod managementPeriod,
            DateTime dateTime,
            GroupEmissionsByDay previousDaysEmissions,
            AnimalGroup animalGroup,
            Farm farm)
        {
            var dailyEmissions = new GroupEmissionsByDay();
            var temperature = farm.ClimateData.GetMeanTemperatureForDay(dateTime);

            InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            /*
             * Enteric methane (CH4)
             */

            // Equation 3.4.1-1
            dailyEmissions.EntericMethaneEmission = CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(
                managementPeriod.ManureDetails.YearlyEntericMethaneRate,
                managementPeriod.NumberOfAnimals);

            /*
             * Manure carbon (C) and methane (CH4)
             */

            var manureCompositionData =
                farm.GetManureCompositionData(ManureStateType.Pasture, managementPeriod.AnimalType);

            dailyEmissions.FecalCarbonExcretionRate = CalculateFecalCarbonExcretionRateForSheepPoultryAndOtherLivestock(
                managementPeriod.ManureDetails.ManureExcretionRate,
                manureCompositionData.CarbonFraction);

            dailyEmissions.FecalCarbonExcretion = CalculateAmountOfFecalCarbonExcreted(
                dailyEmissions.FecalCarbonExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = CalculateRateOfCarbonAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            dailyEmissions.CarbonAddedFromBeddingMaterial = CalculateAmountOfCarbonAddedFromBeddingMaterial(
                dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.CarbonFromManureAndBedding = CalculateAmountOfCarbonFromManureAndBedding(
                dailyEmissions.FecalCarbonExcretion,
                dailyEmissions.CarbonAddedFromBeddingMaterial);

            dailyEmissions.VolatileSolids = managementPeriod.ManureDetails.VolatileSolids;

            if (animalGroup.GroupType == AnimalType.Deer || animalGroup.GroupType == AnimalType.Elk ||
                animalGroup.GroupType == AnimalType.Llamas || animalGroup.GroupType == AnimalType.Alpacas)
                // Equation 4.1.2-4
                dailyEmissions.ManureMethaneEmissionRate =
                    managementPeriod.ManureDetails.DailyManureMethaneEmissionRate;
            else
                dailyEmissions.ManureMethaneEmissionRate = CalculateManureMethaneEmissionRate(
                    managementPeriod.ManureDetails.VolatileSolids,
                    managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    managementPeriod.ManureDetails.MethaneConversionFactor);

            dailyEmissions.ManureMethaneEmission = CalculateManureMethane(
                dailyEmissions.ManureMethaneEmissionRate,
                managementPeriod.NumberOfAnimals);

            CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            dailyEmissions.NitrogenExcretionRate = managementPeriod.ManureDetails.NitrogenExretionRate;

            dailyEmissions.AmountOfNitrogenExcreted = CalculateAmountOfNitrogenExcreted(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial = CalculateRateOfNitrogenAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            dailyEmissions.AmountOfNitrogenAddedFromBedding = CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.ManureDirectN2ONEmissionRate = CalculateManureDirectNitrogenEmissionRate(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            dailyEmissions.ManureDirectN2ONEmission = base.CalculateManureDirectNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmissionRate,
                managementPeriod.NumberOfAnimals);

            /*
             * Indirect manure N2O
             */

            CalculateIndirectEmissionsFromHousingAndStorage(dailyEmissions, managementPeriod);

            dailyEmissions.ManureN2ONEmission = CalculateManureNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmission,
                dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage =
                CalculateNitrogenAvailableForLandApplicationFromSheepSwineAndOtherLivestock(
                    dailyEmissions.AmountOfNitrogenExcreted,
                    dailyEmissions.AmountOfNitrogenAddedFromBedding,
                    dailyEmissions.ManureDirectN2ONEmission,
                    dailyEmissions.TotalNitrogenLossesFromHousingAndStorage,
                    dailyEmissions.ManureN2ONLeachingEmission,
                    dailyEmissions.ManureNitrateLeachingEmission);

            // Equation 4.5.2-22
            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay =
                dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage +
                (previousDaysEmissions == null
                    ? 0
                    : previousDaysEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.ManureCarbonNitrogenRatio = CalculateManureCarbonToNitrogenRatio(
                dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay =
                dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication =
                CalculateTotalVolumeOfManureAvailableForLandApplication(
                    dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage,
                    managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null
                                                   ? 0
                                                   : previousDaysEmissions.AccumulatedVolume);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            GetEmissionsFromGrazingSheepSwineAndOtherLiveStock(
                managementPeriod,
                dailyEmissions);

            return dailyEmissions;
        }

        #endregion

        #region Equations

        /// <summary>
        ///     Equation 3.6.1-1
        /// </summary>
        /// <param name="entericMethaneEmissionRate">Enteric CH4 emission rate (kg head^-1 year^-1) </param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns>Enteric CH4 emission (kg CH4) </returns>
        public double CalculateEntericMethaneEmission(double entericMethaneEmissionRate,
            double numberOfAnimals,
            double numberOfDaysInMonth)
        {
            return entericMethaneEmissionRate * numberOfAnimals * numberOfDaysInMonth / CoreConstants.DaysInYear;
        }

        /// <summary>
        ///     Equation 3.6.2-1
        /// </summary>
        /// <param name="manureMethaneEmissionRate">Manure CH4 emission rate (kg head^-1 year^-1) </param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns>Manure CH4 emission (kg CH4) </returns>
        public double CalculateManureMethaneEmission(double manureMethaneEmissionRate,
            double numberOfAnimals,
            double numberOfDaysInMonth)
        {
            return manureMethaneEmissionRate * numberOfAnimals * numberOfDaysInMonth / CoreConstants.DaysInYear;
        }

        /// <summary>
        ///     Equation 3.6.3-1
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 year^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Manure N (kg N)</returns>
        public double CalculateManureNitrogen(double nitrogenExcretionRate,
            double numberOfAnimals)
        {
            return nitrogenExcretionRate * numberOfAnimals / CoreConstants.DaysInYear;
        }

        /// <summary>
        ///     Equation 3.6.3-2
        /// </summary>
        /// <param name="manureNitrogen">Manure N (kg N)</param>
        /// <param name="emissionFactor">Emission factor [kg N2O-N (kg N)^-1] </param>
        /// <param name="numberOfDays">Number of days in month</param>
        /// <returns>Manure direct N emission (kg N2O-N)</returns>
        public new double CalculateManureDirectNitrogenEmission(double manureNitrogen,
            double emissionFactor,
            double numberOfDays)
        {
            return manureNitrogen * emissionFactor * numberOfDays;
        }

        /// <summary>
        ///     Equation 3.6.3-3
        /// </summary>
        /// <param name="manureNitrogen">Manure N (kg N)</param>
        /// <param name="volatilizationFraction">Volatilization fraction</param>
        /// <param name="emissionFactorForVolatilization">Emission factor for volatilization [kg N2O-N (kg N)^-1]</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns>Manure volatilization N emission (kg N2O-N year^-1)</returns>
        public double CalculateManureVolatilizationNitrogenEmission(double manureNitrogen,
            double volatilizationFraction,
            double emissionFactorForVolatilization,
            double numberOfDaysInMonth)
        {
            return manureNitrogen * volatilizationFraction * emissionFactorForVolatilization * numberOfDaysInMonth;
        }

        /// <summary>
        ///     Equation 3.6.3-4
        /// </summary>
        /// <param name="manureNitrogen">Manure N (kg N)</param>
        /// <param name="leachingFraction">Leaching fraction - calculated in soil N2O emissions</param>
        /// <param name="emissionFactorForLeaching">Emission factor for leaching [kg N2O-N (kg N)^-1]</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns>Manure leaching N emission (kg N2O-N year^-1)</returns>
        public double CalculateManureLeachingNitrogenEmission(double manureNitrogen,
            double leachingFraction,
            double emissionFactorForLeaching,
            double numberOfDaysInMonth)
        {
            return manureNitrogen * leachingFraction * emissionFactorForLeaching * numberOfDaysInMonth;
        }

        /// <summary>
        ///     Equation 3.6.3-6
        /// </summary>
        /// <returns>Scenario manure available for land application (kg N)</returns>
        public double CalculateManureAvailableForLandApplication()
        {
            return 0.0;
        }

        /// <summary>
        ///     Equation 3.6.4-1
        /// </summary>
        /// <param name="manureDirectNitrogenEmissionFromAnimals">
        ///     Total manure direct N emission from other animals (kg N2O-N
        ///     year^-1)
        /// </param>
        /// <returns>Total manure direct N2O emission from other animals (kg N2O year^-1)</returns>
        public double CalculateManureDirectNitrousEmissionFromAnimals(double manureDirectNitrogenEmissionFromAnimals)
        {
            return manureDirectNitrogenEmissionFromAnimals * 44.0 / 28.0;
        }

        /// <summary>
        ///     Equation 3.6.4-2
        /// </summary>
        /// <param name="manureIndirectNitrogenEmissionFromAnimals">
        ///     Total manure indirect N emission from other animals (kg N2O-N
        ///     year^-1)
        /// </param>
        /// <returns>Total manure indirect N2O emission from other animals (kg N2O year^-1)</returns>
        public double CalculateManureIndirectNitrousEmissionFromAnimals(
            double manureIndirectNitrogenEmissionFromAnimals)
        {
            return manureIndirectNitrogenEmissionFromAnimals * 44.0 / 28.0;
        }

        #endregion
    }
}