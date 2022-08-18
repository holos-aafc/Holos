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
            var temperature = farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month);

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

            // Equation 4.1.1-3
            dailyEmissions.FecalCarbonExcretionRate = base.CalculateFecalCarbonExcretionRateForSheepPoultryAndOtherLivestock(
                manureExcretionRate: managementPeriod.ManureDetails.ManureExcretionRate,
                carbonFractionOfManure: managementPeriod.ManureDetails.FractionOfCarbonInManure);

            // Equation 4.1.1-4
            dailyEmissions.FecalCarbonExcretion = base.CalculateAmountOfFecalCarbonExcreted(
                excretionRate: dailyEmissions.FecalCarbonExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-5
            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = base.CalculateRateOfCarbonAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                carbonConcentrationOfBeddingMaterial: managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.1.1-6
            dailyEmissions.CarbonAddedFromBeddingMaterial = base.CalculateAmountOfCarbonAddedFromBeddingMaterial(
                rateOfCarbonAddedFromBedding: dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            if (animalGroup.GroupType == AnimalType.Deer || animalGroup.GroupType == AnimalType.Elk || animalGroup.GroupType == AnimalType.Llamas || animalGroup.GroupType == AnimalType.Alpacas)
            {
                // Equation 4.1.2-4
                dailyEmissions.ManureMethaneEmissionRate = managementPeriod.ManureDetails.DailyManureMethaneEmissionRate;
            }
            else
            {
                // Equation 4.1.2-4
                dailyEmissions.ManureMethaneEmissionRate = this.CalculateManureMethaneEmissionRate(
                    volatileSolids: managementPeriod.ManureDetails.VolatileSolids,
                    methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);
            }

            // Equation 4.1.2-5
            dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.3-13
            dailyEmissions.AmountOfCarbonLostAsMethaneDuringManagement = base.CalculateCarbonLostAsMethaneDuringManagement(
                monthlyManureMethaneEmission: dailyEmissions.ManureMethaneEmission);

            // Equation 4.1.3-14
            dailyEmissions.AmountOfCarbonInStoredManure = base.CalculateAmountOfCarbonInStoredManure(
                monthlyFecalCarbonExcretion: dailyEmissions.FecalCarbonExcretion,
                monthlyAmountOfCarbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial,
                monthlyAmountOfCarbonLostAsMethaneDuringManagement: dailyEmissions.AmountOfCarbonLostAsMethaneDuringManagement);

            /*
             * Direct manure N2O
             */

            // Equation 4.2.1-24
            dailyEmissions.NitrogenExcretionRate = managementPeriod.ManureDetails.NitrogenExretionRate;

            // Equation 4.2.1-29 (used in volatilization calculation)
            dailyEmissions.AmountOfNitrogenExcreted = base.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-30
            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial = base.CalculateRateOfNitrogenAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                nitrogenConcentrationOfBeddingMaterial: managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.2.1-31
            dailyEmissions.AmountOfNitrogenAddedFromBedding = base.CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                rateOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.2-1
            dailyEmissions.ManureDirectN2ONEmissionRate = base.CalculateManureDirectNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                emissionFactor: managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            // Equation 4.2.2-2
            dailyEmissions.ManureDirectN2ONEmission = base.CalculateManureDirectNitrogenEmission(
                manureDirectNitrogenEmissionRate: dailyEmissions.ManureDirectN2ONEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Ammonia emissions
             */

            // Equation 4.3.3-1
            dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;

            // Equation 4.3.4-6
            dailyEmissions.AmmoniaEmissionRateFromHousingAndStorage = base.CalculateAmmoniaEmissionRateFromHousingAndStorage(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                rateOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized);

            // Equation 4.3.4-7
            dailyEmissions.TotalNitrogenLossesFromHousingAndStorage = base.CalculateTotalNitrogenLossFromHousingAndStorage(
                ammoniaEmissionRate: dailyEmissions.AmmoniaEmissionRateFromHousingAndStorage,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.4-8
            dailyEmissions.AmmoniaEmissionsFromHousingAndStorage = base.CalculateAmmoniaLossFromHousingAndStorage(
                totalNitrogenLossFromHousingAndStorage: dailyEmissions.TotalNitrogenLossesFromHousingAndStorage);

            /*
             * Indirect manure N2O
             */

            /*
             * Volatilization
             */

            // Equation 4.3.3-1
            dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;

            dailyEmissions.ManureVolatilizationRate = base.CalculateManureVolatilizationEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized,
                volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            dailyEmissions.ManureVolatilizationRate = base.CalculateManureVolatilizationEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized,
                volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            // Equation 4.3.3-4
            dailyEmissions.ManureVolatilizationN2ONEmission = base.CalculateManureVolatilizationNitrogenEmission(
                volatilizationRate: dailyEmissions.ManureVolatilizationRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Leaching
             */

            // Equation 4.3.4-1
            dailyEmissions.ManureNitrogenLeachingRate = base.CalculateManureLeachingNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            // Equation 4.3.4-2
            dailyEmissions.ManureN2ONLeachingEmission = this.CalculateManureLeachingNitrogenEmission(
                leachingNitrogenEmissionRate: dailyEmissions.ManureNitrogenLeachingRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.5-1
            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.3.7-1
            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.5.2-6
            dailyEmissions.NitrogenAvailableForLandApplication = base.CalculateTotalAvailableManureNitrogenInStoredManureForSheepSwinePoultryAndOtherLivestock(
                nitrogenExcretion: dailyEmissions.AmountOfNitrogenExcreted,
                nitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                directN2ONEmission: dailyEmissions.ManureDirectN2ONEmission,
                indirectN2ONEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.5.3-1
            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.NitrogenAvailableForLandApplication);

            // Equation 4.5.3-2
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                nitrogenFractionOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            // Equation 4.6.2-2
            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = base.CalculateTotalAmmoniaEmissionsFromLandAppliedManure(
                farm: farm,
                dateTime: dateTime,
                dailyEmissions: dailyEmissions,
                animalType: animalGroup.GroupType,
                temperature: temperature,
                managementPeriod: managementPeriod);

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
