﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;

namespace H.Core.Services.Animals
{
    public class PoultryResultsService : AnimalResultsServiceBase, IPoultryResultsService
    {
        #region Fields

        private static readonly Table_41_Poultry_NExcretionRate_Parameter_Values_Provider _poultryNExcretionRateValuesProvider;
        private readonly DefaultDailyTanExcretionRatesForPoultry _defaultDailyTanExcretionRatesForPoultry = new DefaultDailyTanExcretionRatesForPoultry();

        #endregion

        #region Constructors

        static PoultryResultsService()
        {
            _poultryNExcretionRateValuesProvider = new Table_41_Poultry_NExcretionRate_Parameter_Values_Provider();
        }

        public PoultryResultsService() : base()
        {
            _animalComponentCategory = ComponentCategory.Poultry;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override GroupEmissionsByDay CalculateDailyEmissions(
            AnimalComponentBase animalComponentBase,
            ManagementPeriod managementPeriod,
            DateTime dateTime,
            GroupEmissionsByDay previousDaysEmissions,
            AnimalGroup animalGroup,
            Farm farm)
        {
            var dailyEmissions = new GroupEmissionsByDay();
            dailyEmissions.DateTime = dateTime;

            var dailyAverageOutdoorTemperature = farm.ClimateData.GetMeanTemperatureForDay(dateTime);

            this.InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            if (animalGroup.GroupType.IsNewlyHatchedEggs() || animalGroup.GroupType.IsEggs())
            {
                // No CH4 or N2O emissions, but there is electricity used in the barn so we calculate those associated emissions

                return dailyEmissions;
            }

            /*
             * Enteric methane (CH4)
             */

            // Equation 3.4.1-1
            dailyEmissions.EntericMethaneEmission = this.CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(
                    entericMethaneEmissionRate: managementPeriod.ManureDetails.YearlyEntericMethaneRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Manure carbon (C)
             */

            var manureCompositionData = farm.GetManureCompositionData(ManureStateType.SolidStorageWithOrWithoutLitter, AnimalType.Poultry);

            dailyEmissions.FecalCarbonExcretionRate = base.CalculateFecalCarbonExcretionRateForSheepPoultryAndOtherLivestock(
                    manureExcretionRate: managementPeriod.ManureDetails.ManureExcretionRate,
                    carbonFractionOfManure: manureCompositionData.CarbonFraction);

            dailyEmissions.FecalCarbonExcretion = base.CalculateAmountOfFecalCarbonExcreted(
                excretionRate: dailyEmissions.FecalCarbonExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = base.CalculateRateOfCarbonAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                carbonConcentrationOfBeddingMaterial: managementPeriod.HousingDetails
                    .TotalCarbonKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            dailyEmissions.CarbonAddedFromBeddingMaterial = base.CalculateAmountOfCarbonAddedFromBeddingMaterial(
                rateOfCarbonAddedFromBedding: dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            /*
             * Manure methane (CH4)
             */

            if (animalGroup.GroupType.IsChickenType())
            {
                // For chickens, we have VS values used to calculate the manure CH4 emission rate
                dailyEmissions.VolatileSolids = managementPeriod.ManureDetails.VolatileSolids;

                dailyEmissions.ManureMethaneEmissionRate = this.CalculateManureMethaneEmissionRate(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);
            }
            else
            {
                // For turkeys, we use a constant manure CH4 emission rate
                dailyEmissions.ManureMethaneEmissionRate =
                    managementPeriod.ManureDetails.DailyManureMethaneEmissionRate;
            }

            dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            base.CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            if (managementPeriod.AnimalType.IsTurkeyType())
            {
                /*
                 * Turkeys groups have a constant rate defined by table 44
                 */

                // Equation 4.2.1-24
                dailyEmissions.NitrogenExcretionRate = managementPeriod.ManureDetails.NitrogenExretionRate;
            }
            else
            {
                /*
                 * Chickens have a calculated rate
                 */

                var poultryDietData = _poultryNExcretionRateValuesProvider.GetParameterValues(managementPeriod.AnimalType);

                dailyEmissions.DryMatterIntake = poultryDietData.DailyMeanIntake;

                dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                    dryMatterIntake: dailyEmissions.DryMatterIntake,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                dailyEmissions.TotalCarbonUptakeForGroup = base.CalculateDailyCarbonUptakeForGroup(
                    totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

                dailyEmissions.ProteinIntake = this.CalculateProteinIntakePoultry(
                    dryMatterIntake: dailyEmissions.DryMatterIntake,
                    crudeProtein: poultryDietData.CrudeProtein );

                if (managementPeriod.AnimalType == AnimalType.ChickenHens || managementPeriod.AnimalType == AnimalType.Layers)
                {
                    dailyEmissions.ProteinRetained = this.CalculateProteinRetainedLayers(
                        proteinInLiveWeight: poultryDietData.ProteinLiveWeight,
                        weightGain: poultryDietData.WeightGain,
                        proteinContentOfEggs: poultryDietData.ProteinContentEgg,
                        eggProduction: poultryDietData.EggProduction);
                }
                else
                {
                    dailyEmissions.ProteinRetained = this.CalculateProteinRetainedBroilers(
                        initialWeight: poultryDietData.InitialWeight,
                        finalWeight: poultryDietData.FinalWeight,
                        productionPeriod: poultryDietData.ProductionPeriod);
                }

                dailyEmissions.NitrogenExcretionRate = this.CalculateNitrogenExcretionRateChickens(
                    proteinIntake: dailyEmissions.ProteinIntake,
                    proteinRetained: dailyEmissions.ProteinRetained);
            }

            dailyEmissions.AmountOfNitrogenExcreted = base.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial =
                base.CalculateRateOfNitrogenAddedFromBeddingMaterial(
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
             * Ammonia (NH3) from housing
             */

            // TAN excretion rate is from lookup table and not calculated
            dailyEmissions.TanExcretionRate = managementPeriod.ManureDetails.DailyTanExcretion;

            var emissionFactorForHousing = _defaultDailyTanExcretionRatesForPoultry.GetAmmoniaEmissionFactorForHousing(managementPeriod.AnimalType);

            base.CalculateAmmoniaInHousing(dailyEmissions, managementPeriod, emissionFactorForHousing);

            dailyEmissions.TanExcretion = base.CalculateTANExcretion(
                tanExcretionRate: managementPeriod.ManureDetails.DailyTanExcretion,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

             dailyEmissions.TanEnteringStorageSystem = CalculateTanFlowingIntoStorageEachDay(
                tanExcretion: dailyEmissions.TanExcretion,
                ammoniaLostFromHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            /*
             * Ammonia (NH3) from storage
             */

            dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = this.CalculateAmbientTemperatureAdjustmentForStorage(
                dailyAverageOutdoorTemperature: dailyAverageOutdoorTemperature);

            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                ambientTemperatureAdjustmentStorage: dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                ammoniaEmissionFactorStorage: managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage);

            dailyEmissions.AmmoniaLostFromStorage = CalculateAmmoniaLossFromStoredManure(
                amountOfTANEnteringStorageDaily: dailyEmissions.TanEnteringStorageSystem,
                adjustedAmmoniaEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            dailyEmissions.AmmoniaEmissionsFromStorageSystem = CoreConstants.ConvertToNH3(
                amountOfNH3N: dailyEmissions.AmmoniaLostFromStorage);

            dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay = this.CalculateAdjustedAmountOfTANEnteringStorage(
                amountOfTANFlowingIntoStorageEachDay: dailyEmissions.TanEnteringStorageSystem,
                adjustedAmmoniaLossFromStorage: dailyEmissions.AmmoniaLostFromStorage);

            dailyEmissions.AccumulatedTanInStorageOnDay = CalculateAmountOfTanInStorageOnDay(
                tanInStorageOnPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedTanInStorageOnDay,
                flowOfTanEnteringStorage: dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay);

            dailyEmissions.AdjustedAmmoniaFromStorage = this.CalculateAdjustedAmmoniaFromStorage(dailyEmissions, managementPeriod);

            dailyEmissions.FecalNitrogenExcretionRate = base.CalculateFecalNitrogenExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                tanExcretionRate: dailyEmissions.TanExcretionRate);

            dailyEmissions.FecalNitrogenExcretion = base.CalculateFecalNitrogenExcretion(
                fecalNitrogenExcretionRate: dailyEmissions.FecalNitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.OrganicNitrogenInStoredManure = base.CalculateOrganicNitrogenInStoredManure(
                totalNitrogenExcretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

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

            /*
             * Volatilization
             */

            this.CalculateVolatilizationEmissions(dailyEmissions, managementPeriod);

            /*
             * Leaching
             */

            this.CalculateLeachingEmissions(dailyEmissions, managementPeriod);

            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay = base.CalculateAccumulatedTanAvailableForLandApplication(
                accumulatedTANEnteringStorageSystemOnDay: dailyEmissions.AccumulatedTanInStorageOnDay);

            dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage = CalculateNonAccumulatedNitrogenAvailableForLandApplicationOnDay(
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                directManureN2ONEmission: dailyEmissions.ManureDirectN2ONEmission,
                ammoniaConcentrationInHousing: dailyEmissions.AmmoniaConcentrationInHousing,
                ammoniaLostFromStorage: dailyEmissions.AmmoniaLostFromStorage,
                manureN2ONLeachingEmission: dailyEmissions.ManureN2ONLeachingEmission,
                dailyEmissions.ManureNitrateLeachingEmission);

            // Equation 4.5.2-16
            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay = dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage +
                    (previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            // Equation 4.5.2-13
            dailyEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay =
                dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay - dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay;

            dailyEmissions.OrganicNitrogenCreatedOnDay = dailyEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay - (previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                nitrogenFromManure: dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay = dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay + dailyEmissions.OrganicNitrogenCreatedOnDay;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication =
                base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                    totalNitrogenAvailableForLandApplication: dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay,
                    nitrogenContentOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedVolume);

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            base.GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
                managementPeriod: managementPeriod,
                groupEmissionsByDay: dailyEmissions,
                temperature: dailyAverageOutdoorTemperature);

            return dailyEmissions;
        }

        protected override void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm,
            AnimalComponentBase animalComponentBase)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsNewlyHatchedEggs() || 
                groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsEggs())
            {
                groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = this.CalculateEnergyEmissionsFromHatchery(groupEmissionsByMonth, farm, animalComponentBase);

                return;
            }

            var energyConversionFactor = 0d;
            if (farm.Defaults.UseCustomElectricityConversionFactor)
            {
                energyConversionFactor = farm.Defaults.ConversionOfElectricityToCo2;
            }
            else
            {
                energyConversionFactor = _energyConversionDefaultsProvider.GetElectricityConversionValue(groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
            }

            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = this.CalculateTotalEnergyCarbonDioxideEmissionsFromPoultryOperations(
                numberOfAnimals: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                numberOfDays: groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth,
                energyConversion: energyConversionFactor);
        }

        #endregion

        #region Equations

        /// <summary>
        /// Equation 3.5.1-1
        /// </summary>
        /// <param name="entericMethaneEmissionRate">Enteric CH4 emission rate (kg head^-1 year^-1)</param>
        /// <param name="numberOfPoultry">Number of poultry</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns>Enteric CH4 emission (kg CH4)</returns>
        public double CalculateEntericMethaneEmission(double entericMethaneEmissionRate,
            double numberOfPoultry,
            double numberOfDaysInMonth)
        {
            return entericMethaneEmissionRate * numberOfPoultry * numberOfDaysInMonth / CoreConstants.DaysInYear;
        }

        /// <summary>
        ///   Equation 3.5.2-1
        /// </summary>
        /// <param name="manureMethaneEmissionRate">Manure CH4 emission rate (kg head^-1 year^-1) </param>
        /// <param name="numberOfPoultry">Number of poultry</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns>Manure CH4 emission (kg CH4)</returns>
        public double CalculateManureMethaneEmission(double manureMethaneEmissionRate,
            double numberOfPoultry,
            double numberOfDaysInMonth)
        {
            return manureMethaneEmissionRate * numberOfPoultry * numberOfDaysInMonth / CoreConstants.DaysInYear;
        }

        /// <summary>
        /// Equation 4.2.1-25
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg head^-1 day^-1)</param>
        /// <param name="crudeProtein">Crude protein content in dietary dry matter (% of DM)</param>
        /// <returns>Protein intake (kg head^-1 day^-1)</returns>
        private double CalculateProteinIntakePoultry(
            double dryMatterIntake,
            double crudeProtein)
        {
            return dryMatterIntake * (crudeProtein / 100.0);
        }

        /// <summary>
        /// Equation 4.2.1-26
        /// </summary>
        /// <param name="proteinInLiveWeight">Average protein content in live weight (kg protein (kg head)^-1)</param>
        /// <param name="weightGain">Average daily weight gain for cohort (kg head^-1 day^-1)</param>
        /// <param name="proteinContentOfEggs">Average protein content of eggs (kg protein (kg egg)^-1)</param>
        /// <param name="eggProduction">Egg mass production (g egg head-1 day-1)</param>
        /// <returns>Protein retained in animal in cohort (kg head^-1 day^-1)</returns>
        private double CalculateProteinRetainedLayers(
            double proteinInLiveWeight,
            double weightGain,
            double proteinContentOfEggs,
            double eggProduction)
        {
            var result = (proteinInLiveWeight * weightGain) + ((proteinContentOfEggs * eggProduction) / 1000);

            return result;
        }

        /// <summary>
        /// Equation 4.2.1-27
        /// </summary>
        /// <param name="initialWeight">Live weight of the animal at the beginning of the stage (kg)</param>
        /// <param name="finalWeight">Live weight of the animal at the end of the stage (kg)</param>
        /// <param name="productionPeriod">Length of time from chick to slaughter</param>
        /// <returns>Protein retained in animal (kg head^-1 day^-1)</returns>
        private double CalculateProteinRetainedBroilers(
            double initialWeight,
            double finalWeight,
            double productionPeriod)
        {
            const double proteinRetainedForGain = 0.175;

            var result = ((finalWeight - initialWeight) * proteinRetainedForGain) / productionPeriod;

            return result;
        }

        /// <summary>
        /// Equation 4.2.1-28
        /// </summary>
        /// <param name="proteinIntake">Protein intake (kg head^-1 day^-1)</param>
        /// <param name="proteinRetained">Protein retained in animal in cohort (kg head^-1 day^-1)</param>
        /// <returns>N excretion rate (kg head-1 day-1)</returns>
        private double CalculateNitrogenExcretionRateChickens(
            double proteinIntake,
            double proteinRetained)
        {
            // Conversion from dietary protein to dietary N
            const double conversion = 6.25;

            var result = (proteinIntake / conversion) - (proteinRetained / conversion);

            return result;
        }

        /// <summary>
        /// Equation 4.3.3-8
        /// </summary>
        public double CalculateAmbientTemperatureAdjustmentForStorage(
            double dailyAverageOutdoorTemperature)
        {
            var result = 1 - 0.058 * (17 - dailyAverageOutdoorTemperature);
            return result;
        }

        /// <summary>
        /// Equation 4.5.2-15
        /// </summary>
        private double CalculateNonAccumulatedNitrogenAvailableForLandApplicationOnDay(
            double amountOfNitrogenExcreted,
            double amountOfNitrogenFromBedding,
            double directManureN2ONEmission,
            double ammoniaConcentrationInHousing,
            double ammoniaLostFromStorage,
            double manureN2ONLeachingEmission,
            double nitrateLeachingEmission)
        {
            return (amountOfNitrogenExcreted + amountOfNitrogenFromBedding)
                   - (directManureN2ONEmission + ammoniaConcentrationInHousing +
                      ammoniaLostFromStorage + manureN2ONLeachingEmission + nitrateLeachingEmission);
        }

        /// <summary>
        /// Equation 6.2.3-2
        /// </summary>
        /// <param name="numberOfAnimals">Barn capacity for poultry</param>
        /// <param name="numberOfDays">Number of days in month</param>
        /// <param name="energyConversion">kWh per poultry placement per year for electricity (kWh poultry placement^-1 year^-1) </param>
        /// <returns>Total CO2 emissions from poultry operations (kg CO2)</returns>
        public double CalculateTotalEnergyCarbonDioxideEmissionsFromPoultryOperations(
            double numberOfAnimals, 
            double numberOfDays, 
            double energyConversion)
        {
            const double poultryConversion = 2.88;

            return numberOfAnimals * (poultryConversion / CoreConstants.DaysInYear) * energyConversion * numberOfDays;
        }

        /// <summary>
        /// Equation 6.2.3-3
        ///
        /// Calculates the monthly energy emissions for a hatchery in a particular month
        ///
        /// (kg CO2)
        /// </summary>
        private double CalculateEnergyEmissionsFromHatchery(
            GroupEmissionsByMonth groupEmissionsByMonth, 
            Farm farm,
            AnimalComponentBase animalComponent)
        {
            // Get entire production cycle length in days
            var managementPeriods = animalComponent.Groups.SelectMany(x => x.ManagementPeriods).ToList();
            var totalDaysInProductionCycle = managementPeriods.Sum(x => x.Duration.TotalDays);
            if (totalDaysInProductionCycle == 0)
            {
                // Prevent division by zero
                totalDaysInProductionCycle = 1;
            }

            var energyConversionFactor = 0d;
            if (farm.Defaults.UseCustomElectricityConversionFactor)
            {
                energyConversionFactor = farm.Defaults.ConversionOfElectricityToCo2;
            }
            else
            {
                energyConversionFactor = _energyConversionDefaultsProvider.GetElectricityConversionValue(groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
            }

            var barnCapacity = groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals;
            var daysInMonth = (double) groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth;

            var emissionsForEntireProductionCycle = (barnCapacity / 1000.0) * 223.52 * energyConversionFactor;
            var emissionsPerDay = emissionsForEntireProductionCycle * (1.0 / totalDaysInProductionCycle);
            var result  = emissionsPerDay * daysInMonth;

            return result;
        }

        #endregion
    }
}
