using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
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

        private Table_44_Poultry_NExcretionRate_Parameter_Values_Provider _poultryNExcretionRateValuesProvider = new Table_44_Poultry_NExcretionRate_Parameter_Values_Provider();
        private readonly DefaultDailyTanExcretionRatesForPoultry _defaultDailyTanExcretionRatesForPoultry = new DefaultDailyTanExcretionRatesForPoultry();

        #endregion

        #region Constructors

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

            var temperature = farm.ClimateData.GetAverageTemperatureForMonthAndYear(dateTime.Year, (Months)dateTime.Month);

            this.InitializeDailyEmissions(dailyEmissions, managementPeriod);

            if (animalGroup.GroupType.IsNewlyHatchedEggs() || animalGroup.GroupType.IsEggs())
            {
                return dailyEmissions;
            }

            /*
             * Enteric methane (CH4)
             */

            // Equation 3.4.1-1
            dailyEmissions.EntericMethaneEmission =
                this.CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(
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
                carbonConcentrationOfBeddingMaterial: managementPeriod.HousingDetails
                    .TotalCarbonKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.1.1-6
            dailyEmissions.CarbonAddedFromBeddingMaterial = base.CalculateAmountOfCarbonAddedFromBeddingMaterial(
                rateOfCarbonAddedFromBedding: dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            if (animalGroup.GroupType.IsChickenType())
            {
                // For chicken, we have VS values used to calculate the manure CH4 emission rate
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

            // Equation 4.1.2-5
            dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            base.CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions);

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

                dailyEmissions.TotalCarbonUptakeForGroup = base.CaclulateDailyCarbonUptakeForGroup(
                    totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

                dailyEmissions.ProteinIntake = this.CalculateProteinIntakePoultry(
                    dryMatterIntake: dailyEmissions.DryMatterIntake,
                    crudeProtein: poultryDietData.CrudeProtein );// Value in table is percentage - convert to fraction

                if (managementPeriod.AnimalType == AnimalType.ChickenHens)
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

            // Equation 4.2.1-29 (used in volatilization calculation)
            dailyEmissions.AmountOfNitrogenExcreted = base.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-30
            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial =
                base.CalculateRateOfNitrogenAddedFromBeddingMaterial(
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

            // Equation 4.3.3-1
            dailyEmissions.TanExcretion = base.CalculateTANExcretion(
                tanExcretionRate: managementPeriod.ManureDetails.DailyTanExcretion,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // TAN excretion rate is from lookup table and not calculated
            dailyEmissions.TanExcretionRate = managementPeriod.ManureDetails.DailyTanExcretion;

            // Equation 4.3.3-2
            dailyEmissions.FecalNitrogenExcretionRate = base.CalculateFecalNitrogenExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                tanExcretionRate: dailyEmissions.TanExcretionRate);

            // Equation 4.3.3-3
            dailyEmissions.FecalNitrogenExcretion = base.CalculateFecalNitrogenExcretion(
                fecalNitrogenExcretionRate: dailyEmissions.FecalNitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.3-4
            dailyEmissions.OrganicNitrogenInStoredManure = base.CalculateOrganicNitrogenInStoredManure(
                totalNitrogenExcretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            var emissionFactorForHousing = _defaultDailyTanExcretionRatesForPoultry.GetEmissionFactorForHousing(managementPeriod.AnimalType);

            // Equation 4.3.3-5
            dailyEmissions.AmmoniaEmissionRateFromHousing = CalculateAmmoniaEmissionRateFromHousing(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                adjustedEmissionFactor: emissionFactorForHousing);

            // Equation 4.3.3-6
            dailyEmissions.AmmoniaConcentrationInHousing = CalculateAmmoniaConcentrationInHousing(
                emissionRate: dailyEmissions.AmmoniaEmissionRateFromHousing,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.3-7
            dailyEmissions.AmmoniaEmissionsFromHousingSystem = CalculateTotalAmmoniaEmissionsFromHousing(
                ammoniaConcentrationInHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            // Equation 4.3.3-8
            dailyEmissions.TanEnteringStorageSystem = CalculateTanStorage(
                tanExcretion: dailyEmissions.TanExcretion,
                tanExcretionFromPreviousPeriod: previousDaysEmissions == null ? 0 : previousDaysEmissions.TanExcretion,
                ammoniaLostByHousingDuringPreviousPeriod: previousDaysEmissions == null ? 0 : previousDaysEmissions.AmmoniaConcentrationInHousing);

            // Equation 4.3.3-9
            dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = this.CalculateAmbientTemperatureAdjustmentForStorage(
                temperature: temperature);

            var emissionFactorForStorage = _defaultDailyTanExcretionRatesForPoultry.GetEmissionFactorForStorage(managementPeriod.AnimalType);

            // Equation 4.3.3-10
            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = this.CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                ambientTemperatureAdjustmentStorage: dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                ammoniaEmissionFactorStorage: emissionFactorForStorage);

            // Equation 4.3.3-11
            dailyEmissions.AmmoniaLostFromStorage = CalculateAmmoniaLossFromStoredManure(
                tanInStoredManure: dailyEmissions.TanEnteringStorageSystem,
                ammoniaEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            // Equation 4.3.3-12
            dailyEmissions.AmmoniaEmissionsFromStorageSystem = CalculateAmmoniaEmissionsFromStoredManure(
                ammoniaNitrogenLossFromStoredManure: dailyEmissions.AmmoniaLostFromStorage);

            // Equation 4.3.4-1
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

            // Equation 4.3.4-2
            dailyEmissions.ManureVolatilizationRate = this.CalculateManureVolatilizationEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                beddingNitrogen: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized,
                volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            // Equation 4.3.4-3
            dailyEmissions.ManureVolatilizationN2ONEmission = this.CalculateManureVolatilizationNitrogenEmission(
                volatilizationRate: dailyEmissions.ManureVolatilizationRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.6-1
            dailyEmissions.ManureNitrogenLeachingRate = base.CalculateManureLeachingNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching,
                amountOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial);

            // Equation 4.3.6-2
            dailyEmissions.ManureN2ONLeachingEmission = this.CalculateManureLeachingNitrogenEmission(
                leachingNitrogenEmissionRate: dailyEmissions.ManureNitrogenLeachingRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.6-3
            dailyEmissions.ManureNitrateLeachingEmission = base.CalculateNitrateLeaching(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                nitrogenBeddingRate: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching);

            // Equation 4.3.6-1
            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.3.7-1
            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.3.5-7
            dailyEmissions.AdjustedAmmoniaFromHousing = this.CalculateAdjustedAmmoniaFromHousing(dailyEmissions, managementPeriod);

            // Equation 4.3.4-11
            dailyEmissions.AdjustedAmmoniaFromStorage = this.CalculateAdjustedAmmoniaFromStorage(dailyEmissions, managementPeriod);

            // Equation 4.5.2-7
            dailyEmissions.TanAvailableForLandApplication = dailyEmissions.TanEnteringStorageSystem - dailyEmissions.AmmoniaLostFromStorage;

            // Equation 4.5.2-11
            dailyEmissions.NitrogenAvailableForLandApplication =
                (dailyEmissions.AmountOfNitrogenExcreted + dailyEmissions.AmountOfNitrogenAddedFromBedding)
                - (dailyEmissions.ManureDirectN2ONEmission + dailyEmissions.AmmoniaConcentrationInHousing +
                   dailyEmissions.AmmoniaLostFromStorage + dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.5.2-9
            dailyEmissions.OrganicNitrogenAvailableForLandApplication =
                dailyEmissions.NitrogenAvailableForLandApplication - dailyEmissions.TanAvailableForLandApplication;

            // Equation 4.5.3-1
            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.NitrogenAvailableForLandApplication);

            // Equation 4.5.3-2
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication =
                base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                    totalNitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                    nitrogenContentOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            return dailyEmissions;
        }

        protected override void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsNewlyHatchedEggs() || 
                groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsEggs())
            {
                return;
            }

            var energyConversionFactor  = _energyConversionDefaultsProvider.GetElectricityConversionValue(groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
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
        /// Equation 4.3.3-9
        /// </summary>
        public double CalculateAmbientTemperatureAdjustmentForStorage(
            double temperature)
        {
            var result = 1 - 0.058 * (17 - temperature);
            if (result > 1)
            {
                return 1;
            }

            if (result < 0)
            {
                return 0;
            }

            return result;
        }

        /// <summary>
        /// Equation 6.2.3-1
        /// </summary>
        /// <param name="numberOfAnimals">Barn capacity for poultry</param>
        /// <param name="numberOfDays">Number of days in month</param>
        /// <param name="energyConversion">kWh per poultry placement per year for electricity electricity (kWh poultry placement^-1 year^-1) </param>
        /// <returns>Total CO2 emissions from poultry operations (kg CO2)</returns>
        public double CalculateTotalEnergyCarbonDioxideEmissionsFromPoultryOperations(
            double numberOfAnimals, 
            double numberOfDays, 
            double energyConversion)
        {
            const double poultryConversion = 2.88;

            return numberOfAnimals * (poultryConversion / CoreConstants.DaysInYear) * energyConversion * numberOfDays;
        }

        #endregion
    }
}
