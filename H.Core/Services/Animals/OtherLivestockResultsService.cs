using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;

namespace H.Core.Services.Animals
{
    /// <summary>
    /// The main class to calculate emissions from animals such as horses, deer, etc.
    /// </summary>
    public class OtherLivestockResultsService : AnimalResultsServiceBase, IOtherLivestockResultsService
    {
        #region Fields


        #endregion

        #region Constructors

        public OtherLivestockResultsService() : base()
        {
            _animalComponentCategory = ComponentCategory.OtherLivestock;
        }

        #endregion

        #region Public Methods

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

            this.InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            /*
             * Enteric methane (CH4)
             */

            // Equation 3.4.1-1
            dailyEmissions.EntericMethaneEmission = this.CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(
                entericMethaneEmissionRate: managementPeriod.ManureDetails.YearlyEntericMethaneRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Manure carbon (C) and methane (CH4)
             */

            var manureCompositionData = farm.GetManureCompositionData(ManureStateType.Pasture, managementPeriod.AnimalType);

            dailyEmissions.FecalCarbonExcretionRate = base.CalculateFecalCarbonExcretionRateForSheepPoultryAndOtherLivestock(
                manureExcretionRate: managementPeriod.ManureDetails.ManureExcretionRate,
                carbonFractionOfManure: manureCompositionData.CarbonFraction);

            dailyEmissions.FecalCarbonExcretion = base.CalculateAmountOfFecalCarbonExcreted(
                excretionRate: dailyEmissions.FecalCarbonExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = base.CalculateRateOfCarbonAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                carbonConcentrationOfBeddingMaterial: managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            dailyEmissions.CarbonAddedFromBeddingMaterial = base.CalculateAmountOfCarbonAddedFromBeddingMaterial(
                rateOfCarbonAddedFromBedding: dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            dailyEmissions.VolatileSolids = managementPeriod.ManureDetails.VolatileSolids;

            if (animalGroup.GroupType == AnimalType.Deer || animalGroup.GroupType == AnimalType.Elk || animalGroup.GroupType == AnimalType.Llamas || animalGroup.GroupType == AnimalType.Alpacas)
            {
                // Equation 4.1.2-4
                dailyEmissions.ManureMethaneEmissionRate = managementPeriod.ManureDetails.DailyManureMethaneEmissionRate;
            }
            else
            {
                dailyEmissions.ManureMethaneEmissionRate = this.CalculateManureMethaneEmissionRate(
                    volatileSolids: managementPeriod.ManureDetails.VolatileSolids,
                    methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);
            }

            dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            base.CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            dailyEmissions.NitrogenExcretionRate = managementPeriod.ManureDetails.NitrogenExretionRate;

            dailyEmissions.AmountOfNitrogenExcreted = base.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial = base.CalculateRateOfNitrogenAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                nitrogenConcentrationOfBeddingMaterial: managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            dailyEmissions.AmountOfNitrogenAddedFromBedding = base.CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                rateOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.ManureDirectN2ONEmissionRate = base.CalculateManureDirectNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                emissionFactor: managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            dailyEmissions.ManureDirectN2ONEmission = base.CalculateManureDirectNitrogenEmission(
                manureDirectNitrogenEmissionRate: dailyEmissions.ManureDirectN2ONEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Indirect manure N2O
             */

            base.CalculateIndirectEmissionsFromHousingAndStorage(dailyEmissions, managementPeriod);

            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage = base.CalculateNitrogenAvailableForLandApplicationFromSheepSwineAndOtherLivestock(
                nitrogenExcretion: dailyEmissions.AmountOfNitrogenExcreted,
                nitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                directN2ONEmission: dailyEmissions.ManureDirectN2ONEmission,
                ammoniaLostFromHousingAndStorage: dailyEmissions.TotalNitrogenLossesFromHousingAndStorage,
                leachingN2ONEmission: dailyEmissions.ManureN2ONLeachingEmission, 
                leachingNO3NEmission: dailyEmissions.ManureNitrateLeachingEmission);

            // Equation 4.5.2-22
            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay = dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage +
                (previousDaysEmissions == null ? 0 : previousDaysEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage);

            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay = dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage,
                nitrogenContentOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedVolume);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            base.GetEmissionsFromGrazingSheepSwineAndOtherLiveStock(
                managementPeriod: managementPeriod,
                groupEmissionsByDay: dailyEmissions);

            return dailyEmissions;
        }

        #endregion

        #region Equations

        /// <summary>
        ///    Equation 3.6.1-1
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
        /// Equation 3.6.2-1
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
        /// Equation 3.6.3-1
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
        /// Equation 3.6.3-2
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
        ///  Equation 3.6.3-3
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
        /// Equation 3.6.3-4
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
        ///   Equation 3.6.3-6
        /// </summary>
        /// <returns>Scenario manure available for land application (kg N)</returns>
        public double CalculateManureAvailableForLandApplication()
        {
            return 0.0;
        }

        /// <summary>
        /// Equation 3.6.4-1
        /// </summary>
        /// <param name="manureDirectNitrogenEmissionFromAnimals">Total manure direct N emission from other animals (kg N2O-N year^-1)</param>
        /// <returns>Total manure direct N2O emission from other animals (kg N2O year^-1)</returns>
        public double CalculateManureDirectNitrousEmissionFromAnimals(double manureDirectNitrogenEmissionFromAnimals)
        {
            return manureDirectNitrogenEmissionFromAnimals * 44.0 / 28.0;
        }

        /// <summary>
        /// Equation 3.6.4-2
        /// </summary>
        /// <param name="manureIndirectNitrogenEmissionFromAnimals">Total manure indirect N emission from other animals (kg N2O-N year^-1)</param>
        /// <returns>Total manure indirect N2O emission from other animals (kg N2O year^-1)</returns>
        public double CalculateManureIndirectNitrousEmissionFromAnimals(double manureIndirectNitrogenEmissionFromAnimals)
        {
            return manureIndirectNitrogenEmissionFromAnimals * 44.0 / 28.0;
        }

        #endregion
    }
}
