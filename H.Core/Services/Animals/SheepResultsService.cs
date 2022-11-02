using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Sheep;
using H.Core.Providers.Animals;
using H.Core.Providers.Feed;

namespace H.Core.Services.Animals
{
    public class SheepResultsService : AnimalResultsServiceBase, ISheepResultsService
    {
        #region Fields

        private readonly Table_27_Lamb_Daily_Weight_Gain_Provider _lambDailyWeightGainProvider;
        private readonly Table_28_Pregnancy_Coefficients_For_Sheep_Provider _pregnancyCoefficientProvider;

        #endregion

        #region Constructor

        public SheepResultsService() : base()
        {
            _pregnancyCoefficientProvider = new Table_28_Pregnancy_Coefficients_For_Sheep_Provider();
            _lambDailyWeightGainProvider = new Table_27_Lamb_Daily_Weight_Gain_Provider();

            _animalComponentCategory = ComponentCategory.Sheep;
        }

        #endregion

        #region Overrides

       
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

            this.InitializeDailyEmissions(dailyEmissions, managementPeriod);

            // Weaning lambs don't produce any emissions.
            if (managementPeriod.ProductionStage == ProductionStages.Weaning)
            {
                return dailyEmissions;
            }

            dailyEmissions.DateTime = dateTime;

            var temperature = farm.ClimateData.GetAverageTemperatureForMonthAndYear(dateTime.Year, (Months)dateTime.Month);

            /*
             * Enteric methane (CH4)
             */

            if (managementPeriod.PeriodDailyGain > 0)
            {
                dailyEmissions.AverageDailyGain = managementPeriod.PeriodDailyGain;
            }
            else
            {
                // Equation 3.1.1-7
                dailyEmissions.AverageDailyGain = base.CalculateAverageDailyWeightGain(
                    initialWeight: managementPeriod.StartWeight,
                    finalWeight: managementPeriod.EndWeight,
                    numberOfDays: managementPeriod.Duration.TotalDays);
            }

            // Equation 3.3.1-1
            dailyEmissions.AnimalWeight = base.GetCurrentAnimalWeight(
                startWeight: managementPeriod.StartWeight,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                startDate: managementPeriod.Start,
                currentDate: dailyEmissions.DateTime);

            var totalNumberOfYoungAnimalsOnDate = animalComponentBase.GetTotalNumberOfYoungAnimalsByDate(
                dateTime: dateTime,
                parentGroup: animalGroup,
                childGroupType: AnimalType.Lambs);

            if (animalGroup.GroupType == AnimalType.Ewes)
            {
                // Equation 3.3.1-2
                dailyEmissions.LambEweRatio = this.CalculateLambRatio(
                    numberOfLambs: totalNumberOfYoungAnimalsOnDate,
                    numberOfEwes: managementPeriod.NumberOfAnimals);
            }

            // Equation 3.3.1-3
            dailyEmissions.NetEnergyForMaintenance = this.CalculateNetEnergyForMaintenance(
                maintenanceCoefficient: managementPeriod.HousingDetails.BaselineMaintenanceCoefficient,
                weight: dailyEmissions.AnimalWeight);

            // Equation 3.3.1-4
            dailyEmissions.NetEnergyForActivity = this.CalculateNetEnergyForActivity(
                feedingActivityCoefficient: managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation,
                weight: dailyEmissions.AnimalWeight);

            var isLactatingAnimalGroup = totalNumberOfYoungAnimalsOnDate > 0;
            if (isLactatingAnimalGroup)
            {
                var dailyGainForLambs = _lambDailyWeightGainProvider.GetDailyWeightGain(
                    lambRatio: dailyEmissions.LambEweRatio);

                if (managementPeriod.UseCustomMilkProductionValue == false)
                {
                    // Equation 3.3.1-5
                    dailyEmissions.NetEnergyForLactation = this.CalculateNetEnergyForLactation(
                        dailyWeightGainOfLambs: dailyGainForLambs,
                        energyRequiredToProduceAKilogramOfMilk: managementPeriod.EnergyRequiredForMilk);
                }
                else
                {
                    // Equation 3.3.1-6
                    dailyEmissions.NetEnergyForLactation = this.CalculateNetEnergyForLactationUsingMilkProduction(
                        milkProduction: managementPeriod.MilkProduction,
                        energyRequiredToProduceAKilogramOfMilk: managementPeriod.EnergyRequiredForMilk);
                }
            }

            if (animalGroup.GroupType.IsPregnantType())
            {
                var pregnancyCoefficient = _pregnancyCoefficientProvider.GetPregnancyCoefficient(
                    lambRatio: dailyEmissions.LambEweRatio);

                // Equation 3.3.1-6
                dailyEmissions.NetEnergyForPregnancy = this.CalculateNetEnergyForPregnancy(
                    pregnancyCoefficient: pregnancyCoefficient,
                    netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);
            }

            // Equation 3.3.1-7
            dailyEmissions.NetEnergyForWoolProduction = this.CalculateNetEnergyForWoolProduction(
                energyValueOfAKilogramOfWool: managementPeriod.EnergyRequiredForWool,
                woolProduction: managementPeriod.WoolProduction);

            if (animalGroup.GroupType == AnimalType.SheepFeedlot)
            {
                dailyEmissions.AverageDailyGain = managementPeriod.PeriodDailyGain;
            }
            else
            {
                // Equation 3.3.1-8
                dailyEmissions.AverageDailyGain = base.CalculateAverageDailyWeightGain(
                    initialWeight: managementPeriod.StartWeight,
                    finalWeight: managementPeriod.EndWeight,
                    numberOfDays: managementPeriod.Duration.TotalDays);
            }

            // Equation 3.3.1-9
            dailyEmissions.NetEnergyForGain = this.CalculateNetEnergyForGain(
                averageDailyGain: dailyEmissions.AverageDailyGain,
                weight: dailyEmissions.AnimalWeight,
                coefficientA: managementPeriod.GainCoefficientA,
                coefficientB: managementPeriod.GainCoefficientB);

            // Equation 3.3.1-10
            dailyEmissions.RatioOfEnergyAvailableForMaintenance = this.CalculateRatioOfEnergyAvailableForMaintenance(
                percentTotalDigestibleEnergyInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            // Equation 3.3.1-11
            dailyEmissions.RatioOfEnergyAvailableForGain = this.CalculateRatioEnergyAvailableForGain(
                percentTotalDigestibleEnergyInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            // Equation 3.3.1-12
            dailyEmissions.GrossEnergyIntake = this.CalculateGrossEnergyIntake(
                netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance,
                netEnergyForActivity: dailyEmissions.NetEnergyForActivity,
                netEnergyForLactation: dailyEmissions.NetEnergyForLactation,
                netEnergyForPregnancy: dailyEmissions.NetEnergyForPregnancy,
                netEnergyForGain: dailyEmissions.NetEnergyForGain,
                netEnergyForWoolProduction: dailyEmissions.NetEnergyForWoolProduction,
                ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed: dailyEmissions.RatioOfEnergyAvailableForMaintenance,
                ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed: dailyEmissions.RatioOfEnergyAvailableForGain,
                percentTotalDigestibleEnergyInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            // Equation 3.3.1-13
            dailyEmissions.EntericMethaneEmissionRate = this.CalculateEntericMethaneEmissionRateForSheep(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                methaneConversionFactor: managementPeriod.SelectedDiet.MethaneConversionFactor);

            // Equation 3.3.1-14
            dailyEmissions.EntericMethaneEmission = this.CalculateEntericMethaneEmissions(
                entericMethaneEmissionRate: dailyEmissions.EntericMethaneEmissionRate,
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
            if (animalGroup.GroupType == AnimalType.Lambs)
            {
                dailyEmissions.CarbonAddedFromBeddingMaterial = 0;
            }
            else
            {
                dailyEmissions.CarbonAddedFromBeddingMaterial = base.CalculateAmountOfCarbonAddedFromBeddingMaterial(
                    rateOfCarbonAddedFromBedding: dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            // Equation 4.1.2-1
            dailyEmissions.VolatileSolids = base.CalculateVolatileSolids(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                percentTotalDigestibleNutrientsInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                ashContentOfFeed: managementPeriod.SelectedDiet.Ash);

            // Equation 4.1.2-4
            dailyEmissions.ManureMethaneEmissionRate = base.CalculateManureMethaneEmissionRate(
                volatileSolids: dailyEmissions.VolatileSolids,
                methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);

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

            // Equation 4.2.1-16
            dailyEmissions.ProteinIntake = base.CalculateProteinIntake(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                crudeProtein: managementPeriod.SelectedDiet.CrudeProteinContent);

            // Equation 4.2.1-17
            dailyEmissions.ProteinRetained = 0.1;

            // Equation 4.2.1-18
            dailyEmissions.NitrogenExcretionRate = this.CalculateNitrogenExcretionRate(
                proteinIntake: dailyEmissions.ProteinIntake,
                proteinRetained: dailyEmissions.ProteinRetained);

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
            if (animalGroup.GroupType == AnimalType.Lambs)
            {
                dailyEmissions.AmountOfNitrogenAddedFromBedding = 0;
            }
            else
            {
                dailyEmissions.AmountOfNitrogenAddedFromBedding = base.CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                    rateOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

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

            // No equation for this when considering sheep (or beef) as it is a lookup table in algorithm document
            dailyEmissions.FractionOfNitrogenExcretedInUrine = base.GetFractionOfNitrogenExcretedInUrine(
                crudeProteinInDiet: managementPeriod.SelectedDiet.CrudeProteinContent);

            // Equation 4.3.4-1
            dailyEmissions.TanExcretionRate = base.CalculateTANExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                fractionOfNitrogenExcretedInUrine: dailyEmissions.FractionOfNitrogenExcretedInUrine);

            // Equation 4.3.4-2
            dailyEmissions.TanExcretion = base.CalculateTANExcretion(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.4-3
            dailyEmissions.FecalNitrogenExcretionRate = base.CalculateFecalNitrogenExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                tanExcretionRate: dailyEmissions.TanExcretionRate);

            // Equation 4.3.4-4
            dailyEmissions.FecalNitrogenExcretion = base.CalculateFecalNitrogenExcretion(
                fecalNitrogenExcretionRate: dailyEmissions.FecalNitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.4-5
            dailyEmissions.OrganicNitrogenInStoredManure = base.CalculateOrganicNitrogenInStoredManure(
                totalNitrogenExcretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

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

            // Equation 4.3.4-2
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
             * Adjusted ammonia emissions
             */

            // Equation 4.3.5-12
            dailyEmissions.AdjustedTotalNitrogenEmissionsFromHousingAndStorage = base.CalculateAmmoniaAdjustmentFromHousingAndStorage(
                totalAmmoniaLossFromHousingAndStorage: dailyEmissions.TotalNitrogenLossesFromHousingAndStorage,
                manureVolatilizationEmissions: dailyEmissions.ManureVolatilizationN2ONEmission);

            // Equation 4.3.5-13
            dailyEmissions.AdjustedAmmoniaEmissionsFromHousingAndStorage = base.CalculateTotalAdjustedAmmoniaFromHousingAndStorage(
                adjustedTotalNitrogenEmissionsFromStorageAndHousing: dailyEmissions.AdjustedTotalNitrogenEmissionsFromHousingAndStorage);

            /*
             * Leaching
             */

            // Equation 4.3.6-1
            dailyEmissions.ManureNitrogenLeachingRate = base.CalculateManureLeachingNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

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

            // Equation 4.3.5-1
            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.3.7-1
            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.5.2-13
            dailyEmissions.NitrogenAvailableForLandApplication = base.CalculateNitrogenAvailableForLandApplicationFromSheepSwineAndOtherLivestock(
                nitrogenExcretion: dailyEmissions.AmountOfNitrogenExcreted,
                nitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                directN2ONEmission: dailyEmissions.ManureDirectN2ONEmission, 
                ammoniaLostFromHousingAndStorage: dailyEmissions.TotalNitrogenLossesFromHousingAndStorage, 
                leachingN2ONEmission: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.5.3-1
            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.NitrogenAvailableForLandApplication);

            // Equation 4.5.3-2
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                nitrogenContentOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            // Equation 5.2.5-8
            dailyEmissions.AmmoniaEmissionsFromGrazingAnimals = base.GetAmmoniaEmissionsFromGrazingSheepSwinePoultryAndOtherLiveStock(
                managementPeriod: managementPeriod,
                groupEmissionsByDay: dailyEmissions);

            return dailyEmissions;
        }

        protected override void CalculateEstimatesOfProduction(
            GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType == AnimalType.SheepFeedlot)
            {
                groupEmissionsByMonth.MonthlyLambProduced = this.CalculateLambProducedPerMonth(
                    averageDailyGain: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.PeriodDailyGain, 
                    numberOfAnimals: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals, 
                    numberOfDaysInMonth: groupEmissionsByMonth.DaysInMonth); 
            }
        }

        #endregion

        #region Equations

        /// <summary>
        /// Equation 3.3.1-3
        /// </summary>
        /// <param name="numberOfLambs">Number of lambs</param>
        /// <param name="numberOfEwes">Number of ewes</param>
        /// <returns>Lamb:ewe ratio</returns>
        public double CalculateLambRatio(double numberOfLambs, double numberOfEwes)
        {
            if (numberOfEwes == 0)
            {
                return 0;
            }

            return numberOfLambs / numberOfEwes;
        }

        /// <summary>
        /// Equation 3.3.1-4
        /// </summary>
        /// <param name="feedingActivityCoefficient">Feeding activity coefficient (MJ kg^-1)</param>
        /// <param name="weight">Weight (kg head^-1)</param>
        /// <returns>Net energy for activity (MJ head^-1 day^-1)</returns>
        public new double CalculateNetEnergyForActivity(
            double feedingActivityCoefficient, 
            double weight)
        {
            return feedingActivityCoefficient * weight;
        }

        /// <summary>
        /// Equation 3.3.1-5
        /// </summary>
        /// <param name="dailyWeightGainOfLambs">Daily weight gain of lamb(s) </param>
        /// <param name="energyRequiredToProduceAKilogramOfMilk">Energy required to produce 1 kg of milk (MJ kg^-1)</param>
        /// <returns>Net energy for lactation (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForLactation(
            double dailyWeightGainOfLambs,
            double energyRequiredToProduceAKilogramOfMilk)
        {
            return 5.0 * dailyWeightGainOfLambs * energyRequiredToProduceAKilogramOfMilk;
        }

        /// <summary>
        /// Equation 3.4.1-6
        /// </summary>
        /// <param name="milkProduction">Amount of milk produced (kg milk head^-1 day^-1)</param>
        /// <param name="energyRequiredToProduceAKilogramOfMilk">Energy required to produce 1 kg of milk (MJ kg^-1)</param>
        /// <returns>Net energy for lactation (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForLactationUsingMilkProduction(
            double milkProduction,
            double energyRequiredToProduceAKilogramOfMilk)
        {
            return milkProduction * energyRequiredToProduceAKilogramOfMilk;
        }

        /// <summary>
        /// Equation 3.3.1-6
        /// </summary>
        /// <param name="pregnancyCoefficient">Pregnancy constant</param>
        /// <param name="netEnergyForMaintenance">Net energy for maintenance (MJ head^-1 day^-1)</param>
        /// <returns>Net energy for pregnancy (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForPregnancy(double pregnancyCoefficient, double netEnergyForMaintenance)
        {
            return netEnergyForMaintenance * pregnancyCoefficient;
        }

        /// <summary>
        /// Equation 3.3.1-7
        /// </summary>
        /// <param name="energyValueOfAKilogramOfWool">Energy value of 1 kg of wool (MJ kg^-1)</param>
        /// <param name="woolProduction">Wool production (kg year^-1)</param>
        /// <returns>Net energy for wool production (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForWoolProduction(double energyValueOfAKilogramOfWool, double woolProduction)
        {
            return (energyValueOfAKilogramOfWool * woolProduction) / CoreConstants.DaysInYear;
        }

        /// <summary>
        /// Equation 3.3.1-9
        /// </summary>
        /// <param name="averageDailyGain">Average daily gain (kg head^-1 day^-1)</param>
        /// <param name="weight">Weight (kg head^-1)</param>
        /// <param name="coefficientA">Coefficient a (MJ kg^-1)</param>
        /// <param name="coefficientB">Coefficient b (MJ kg^-2)</param>
        /// <returns>Net energy for gain (MJ head^-1 day^-1)</returns>
        public new double CalculateNetEnergyForGain(
            double averageDailyGain, 
            double weight, 
            double coefficientA,
            double coefficientB)
        {
            return averageDailyGain * (coefficientA + coefficientB * weight);
        }

        /// <summary>
        /// Equation 3.3.1-10
        /// </summary>
        /// <param name="percentTotalDigestibleEnergyInFeed">Percent digestible energy in feed</param>
        /// <returns>Ratio of net energy available in diet for maintenance to digestible energy consumed (unitless)</returns>
        public double CalculateRatioOfEnergyAvailableForMaintenance(
            double percentTotalDigestibleEnergyInFeed)
        {
            var a = 1.123;
            var b = 0.004092;
            var c = 0.00001126;
            var d = 25.4;
            return a - b * percentTotalDigestibleEnergyInFeed +
                   c * percentTotalDigestibleEnergyInFeed * percentTotalDigestibleEnergyInFeed -
                   d / percentTotalDigestibleEnergyInFeed;
        }

        /// <summary>
        /// Equation 3.4.1-11
        /// </summary>
        /// <param name="percentTotalDigestibleEnergyInFeed">Percent digestible energy in feed</param>
        /// <returns>Ratio of net energy available in diet for gain to digestible energy consumed (unitless)</returns>
        public double CalculateRatioEnergyAvailableForGain(
            double percentTotalDigestibleEnergyInFeed)
        {
            var a = 1.164;
            var b = 0.005160;
            var c = 0.00001308;
            var d = 37.4;
            return a - b * percentTotalDigestibleEnergyInFeed +
                   c * percentTotalDigestibleEnergyInFeed * percentTotalDigestibleEnergyInFeed -
                   d / percentTotalDigestibleEnergyInFeed;
        }

        /// <summary>
        /// Equation 3.3.1-12
        /// </summary>
        /// <param name="netEnergyForMaintenance">Net energy for maintenance (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForActivity">Net energy for activity (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForLactation">Net energy for lactation (MJ head -1 day^-1)</param>
        /// <param name="netEnergyForPregnancy">Net energy for pregnancy (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForGain">Net energy for gain (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForWoolProduction">Net energy for wool production (MJ head^-1 day^-1)</param>
        /// <param name="ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed">Ratio of net energy available in diet for maintenance to digestible energy consumed</param>
        /// <param name="ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed">Ratio of net energy available in diet for gain to digestible energy consumed</param>
        /// <param name="percentTotalDigestibleEnergyInFeed">Percent digestible energy in feed</param>
        /// <returns>Gross energy intake (MJ head^-1 day^-1)</returns>
        public double CalculateGrossEnergyIntake(double netEnergyForMaintenance, double netEnergyForActivity,
                                                 double netEnergyForLactation, double netEnergyForPregnancy, double netEnergyForGain,
                                                 double netEnergyForWoolProduction,
                                                 double ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed,
                                                 double ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed,
                                                 double percentTotalDigestibleEnergyInFeed)
        {
            var a = netEnergyForPregnancy + netEnergyForMaintenance + netEnergyForLactation + netEnergyForActivity;
            a /= ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed;
            var b = netEnergyForGain + netEnergyForWoolProduction;
            b /= ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed;
            return (a + b) * 100.0 / percentTotalDigestibleEnergyInFeed;
        }

        /// <summary>
        /// Equation 3.3.1-13
        /// </summary>
        /// <param name="grossEnergyIntake">Gross energy intake (MJ head^-1 day^-1)</param>
        /// <param name="methaneConversionFactor">Methane conversion factor</param>
        /// <returns>Enteric CH4 emission rate (kg head^-1 day^-1)</returns>
        public double CalculateEntericMethaneEmissionRateForSheep(double grossEnergyIntake, double methaneConversionFactor)
        {
            return grossEnergyIntake * methaneConversionFactor / 55.65;
        }

        /// <summary>
        /// Equation 3.4.2-3
        /// </summary>
        /// <param name="manureMethaneEmissionRate">Manure CH4 emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfSheep">Number of sheep</param>
        /// <param name="numberOfDays">Number of days</param>
        /// <returns>Manure CH4 emission (kg CH4)</returns>
        public double CalculateManureMethaneEmission(double manureMethaneEmissionRate, double numberOfSheep,
                                                     double numberOfDays)
        {
            return manureMethaneEmissionRate * numberOfSheep * numberOfDays;
        }

        /// <summary>
        /// Equation 3.4.3-2
        /// </summary>
        /// <returns>Protein retained</returns>
        public double CalculateProteinRetained()
        {
            return 0.10;
        }

        /// <summary>
        /// Equation 4.2.1-18
        /// </summary>
        /// <param name="proteinIntake">Protein intake (kg head^-1 day^-1)</param>
        /// <param name="proteinRetained">Protein Retained</param>
        /// <returns>N excretion rate (kg head^-1 day^-1)</returns>
        public double CalculateNitrogenExcretionRate(double proteinIntake, double proteinRetained)
        {
            return ((1.0 - proteinRetained) * proteinIntake) / 6.25;
        }

        /// <summary>
        /// Equation 3.4.3-6
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="volatilizationFraction">Volatilization Fraction</param>
        /// <param name="emissionFactorForVolatilization">Emission factor for volatilization [kg N2O-N (kg N)^-1] </param>
        /// <returns>Manure volatilization N emission rate (kg head^-1 day^-1)</returns>
        public double CalculateManureVolatilizationNitrogenEmissionRate(double nitrogenExcretionRate,
                                                                        double volatilizationFraction, double emissionFactorForVolatilization)
        {
            return nitrogenExcretionRate * volatilizationFraction * emissionFactorForVolatilization;
        }

        /// <summary>
        ///  Equation 3.4.3-8
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="leachingFraction">Leaching fraction</param>
        /// <param name="emissionFactorForLeaching">Emission factor for leaching [kg N2O-N (kg N)^-1]</param>
        /// <returns>Manure leaching N emission rate (kg head^-1 day^-1)</returns>
        public double CalculateManureLeachingNitrogemEmissionRate(double nitrogenExcretionRate, double leachingFraction,
                                                                  double emissionFactorForLeaching)
        {
            return nitrogenExcretionRate * leachingFraction * emissionFactorForLeaching;
        }

        /// <summary>
        /// Equation 3.4.3-12
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfSheep">Number of sheep</param>
        /// <param name="numberOfDays">Number of days in month</param>
        /// <param name="volatilizationFraction">Volatilization Fraction</param>
        /// <param name="leachingFraction">Leaching fraction</param>
        /// <returns>Manure available for land application (kg N)</returns>
        public double CalculateManureAvailableForLandApplication(double nitrogenExcretionRate, double numberOfSheep,
                                                                 double numberOfDays, double volatilizationFraction, double leachingFraction)
        {
            var factor = 1 - (leachingFraction + volatilizationFraction);
            return nitrogenExcretionRate * numberOfSheep * numberOfDays * factor;
        }

        /// <summary>
        /// Equation 3.4.5-1
        /// </summary>
        /// <param name="manureTotalDirectNitrogenEmissionFromSheep">Total manure direct N emission from sheep (kg N2O-N year^-1)</param>
        /// <returns>Total manure direct N2O emission from sheep (kg N2O year^-1)</returns>
        public double CalculateTotalManureDirectNitrousOxideEmissionFromSheep(double manureTotalDirectNitrogenEmissionFromSheep)
        {
            return manureTotalDirectNitrogenEmissionFromSheep * 44.0 / 28.0;
        }

        /// <summary>
        ///  Equation 3.4.5-2
        /// </summary>
        /// <param name="manureTotalIndirectNitrogenEmissionFromSheep">Total manure indirect N emission from sheep (kg N2O-N year^-1)</param>
        /// <returns>Total manure indirect N2O emission from sheep (kg N2O year^-1)</returns>
        public double CalculateTotalManureIndirectNitrousOxideEmissionFromSheep(
            double manureTotalIndirectNitrogenEmissionFromSheep)
        {
            return manureTotalIndirectNitrogenEmissionFromSheep * 44.0 / 28.0;
        }

        /// <summary>
        ///  Equation 3.4.5-3
        /// </summary>
        /// <param name="totalManureNitrogenEmission">Total manure N emission from sheep (kg N2O-N year^-1)</param>
        /// <returns>Total manure N2O emission from sheep (kg N2O year^-1)</returns>
        public double CalculateTotalManureNitrousOxideEmissionFromSheep(
            double totalManureNitrogenEmission)
        {
            return totalManureNitrogenEmission * 44 / 28;
        }

        /// <summary>
        /// Equation 9.4-3
        /// </summary>
        /// <param name="averageDailyGain">Total manure N2O emission from sheep (kg N2O year^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns></returns>
        public double CalculateLambProducedPerMonth(double averageDailyGain, double numberOfAnimals, double numberOfDaysInMonth)
        {
            return averageDailyGain * numberOfDaysInMonth * numberOfAnimals;
        }

        #endregion
    }
}
