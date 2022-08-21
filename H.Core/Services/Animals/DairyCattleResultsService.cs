using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.Animals.Dairy;

namespace H.Core.Services.Animals
{
    public class DairyCattleResultsService : AnimalResultsServiceBase, IDairyResultsService
    {
        #region Fields

        #endregion

        #region Constructors

        public DairyCattleResultsService() : base()
        {
            _animalComponentCategory = ComponentCategory.Dairy;
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
            GroupEmissionsByDay groupEmissionsByDay;

            if (animalGroup.GroupType == AnimalType.DairyCalves)
            {
                groupEmissionsByDay = this.CalculateDailyEmissionsForCalves(
                    managementPeriod: managementPeriod,
                    dateTime: dateTime,
                    dairyComponent: animalComponentBase,
                    previousDaysEmissions: previousDaysEmissions,
                    animalGroup: animalGroup,
                    farm: farm);
            }
            else
            {
                groupEmissionsByDay = this.CalculateDailyEmissionsForGroup(
                    dairyComponent: animalComponentBase,
                    managementPeriod: managementPeriod,
                    dateTime: dateTime,
                    previousDaysEmissions: previousDaysEmissions,
                    animalGroup: animalGroup,
                    farm: farm);
            }

            return groupEmissionsByDay;
        }

        protected GroupEmissionsByDay CalculateDailyEmissionsForCalves(
            ManagementPeriod managementPeriod, 
            DateTime dateTime, 
            AnimalComponentBase dairyComponent, 
            GroupEmissionsByDay previousDaysEmissions, 
            AnimalGroup animalGroup, 
            Farm farm)
        {
            var dailyEmissions = new GroupEmissionsByDay();

            dailyEmissions.DateTime = dateTime;

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

            // Equation 3.1.1-1
            dailyEmissions.AnimalWeight = base.GetCurrentAnimalWeight(
                startWeight: managementPeriod.StartWeight,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                startDate: managementPeriod.Start,
                currentDate: dailyEmissions.DateTime);

            // Equation 3.1.2-2
            dailyEmissions.DryMatterIntake = base.CalculateDryMatterIntakeForCalves(
                dietaryNetEnergyConcentration: 6,
                weight: dailyEmissions.AnimalWeight);

            dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = base.CaclulateDailyCarbonUptakeForGroup(
                totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

            // Equation 3.1.2-3
            dailyEmissions.GrossEnergyIntake = base.CalculateGrossEnergyIntakeForCalves(
                dryMatterIntake: dailyEmissions.DryMatterIntake);

            dailyEmissions.AdditiveReductionFactor = AdditiveReductionFactorsProvider.GetAdditiveReductionFactor(
                additiveType: managementPeriod.DietAdditive,
                numberOfDays: managementPeriod.Duration.TotalDays,
                fat: managementPeriod.SelectedDiet.Fat);

            // Equation 3.2.2-1
            dailyEmissions.EntericMethaneEmission = 0;

            /*
             * Manure carbon (C) and methane (CH4)
             */

            // Equation 4.1.1-1
            dailyEmissions.FecalCarbonExcretionRate = base.CalculateFecalCarbonExcretionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake);

            // Equation 4.1.1-4
            dailyEmissions.FecalCarbonExcretion = base.CalculateAmountOfFecalCarbonExcreted(
                excretionRate: dailyEmissions.FecalCarbonExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-6
            dailyEmissions.CarbonAddedFromBeddingMaterial = 0;

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            // Equation 4.1.2-1
            dailyEmissions.VolatileSolids = 1.42;

            /*
             * Manure methane calculations differ depending if the manure is stored as a liquid or as a solid
             */

            var temperature = farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month);

            if (managementPeriod.ManureDetails.StateType.IsSolidManure())
            {
                // Equation 4.1.2-4
                dailyEmissions.ManureMethaneEmissionRate = base.CalculateManureMethaneEmissionRate(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);

                // Equation 4.1.2-5
                dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                    emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }
            else
            {
                // Equation 4.1.3-8
                dailyEmissions.KelvinAirTemperature = base.CalculateDegreeKelvin(
                    degreesCelsius: temperature);

                // Equation 4.1.3-9
                dailyEmissions.ClimateFactor = base.CalculateClimateFactor(
                    kelvinAirTemperature: dailyEmissions.KelvinAirTemperature);

                // Equation 4.1.3-3
                dailyEmissions.VolatileSolidsProduced = base.CalculateVolatileSolidsProduced(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                // Equation 4.1.3-4
                dailyEmissions.VolatileSolidsLoaded = base.CalculateVolatileSolidsLoaded(
                    volatileSolidsProduced: dailyEmissions.VolatileSolidsProduced);

                // Equation 4.1.3-5
                dailyEmissions.VolatileSolidsAvailable = base.CalculateVolatileSolidsAvailable(
                    volatileSolidsLoaded: dailyEmissions.VolatileSolidsLoaded,
                    volatileSolidsAvailableFromPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsAvailable,
                    volatileSolidsConsumedFromPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsConsumed);

                // Equation 4.1.3-7
                dailyEmissions.VolatileSolidsConsumed = base.CalculateVolatileSolidsConsumed(
                    climateFactor: dailyEmissions.ClimateFactor,
                    volatileSolidsAvailable: dailyEmissions.VolatileSolidsAvailable);

                // Equation 4.1.3-8
                dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethane(
                    volatileSolidsConsumed: dailyEmissions.VolatileSolidsConsumed,
                    methaneProducingCapacityOfManure: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure);

                if (managementPeriod.ManureDetails.StateType.IsCoveredSystem())
                {
                    if (managementPeriod.ManureDetails.StateType == ManureStateType.LiquidWithNaturalCrust)
                    {
                        // Equation 4.1.3-9
                        dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethaneForCoveredSystem(
                            manureMethane: dailyEmissions.ManureMethaneEmission,
                            emissionReductionFactor: 0.4);
                    }
                    else
                    {
                        // Equation 4.1.3-9
                        dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethaneForCoveredSystem(
                            manureMethane: dailyEmissions.ManureMethaneEmission,
                            emissionReductionFactor: 0.5); // Not finalized at this time so going with 0.5 (range is 25-50%)
                    }
                }
            }

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

            // Equation 4.2.1-15
            dailyEmissions.NitrogenExcretionRate = 0.078;

            // Equation 4.2.1-29 (used in volatilization calculation)
            dailyEmissions.AmountOfNitrogenExcreted = base.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-31
            dailyEmissions.AmountOfNitrogenAddedFromBedding = 0;

            // Equation 4.2.2-1
            dailyEmissions.ManureDirectN2ONEmissionRate = base.CalculateManureDirectNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                emissionFactor: managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            // Equation 4.2.2-2
            dailyEmissions.ManureDirectN2ONEmission = base.CalculateManureDirectNitrogenEmission(
                manureDirectNitrogenEmissionRate: dailyEmissions.ManureDirectN2ONEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            /*
             * Indirect manure N2O
             */

            this.CalculateIndirectManureNitrousOxide(
                dailyEmissions: dailyEmissions,
                managementPeriod: managementPeriod,
                animalGroup: animalGroup,
                farm: farm,
                dateTime: dateTime,
                previousDaysEmissions: previousDaysEmissions);

            // Equation 4.3.5-1
            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.3.7-1
            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.5.2-1
            dailyEmissions.TanAvailableForLandApplication = base.CalculateMonthlyTanAvailableForLandApplication(
                monthlyTanEnteringStorageSystem: dailyEmissions.AdjustedAmountOfTanInStoredManure,
                ammoniaLostFromStoredManure: dailyEmissions.AmmoniaLostFromStorage);

            // Equation 4.5.2-3
            dailyEmissions.OrganicNitrogenAvailableForLandApplication = base.CalculateMonthlyOrganicNitrogenAvailableForLandApplication(
                fecalNitrogenExcretion: dailyEmissions.FecalNitrogenExcretion,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                fractionOfMineralizedNitrogen: managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized,
                directManureEmissions: dailyEmissions.ManureDirectN2ONEmission,
                leachingEmissions: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.5.2-5
            dailyEmissions.NitrogenAvailableForLandApplication = base.CalculateTotalAvailableManureNitrogenInStoredManure(
                tanAvailableForLandApplication: dailyEmissions.TanAvailableForLandApplication,
                organicNitrogenAvailableForLandApplication: dailyEmissions.OrganicNitrogenAvailableForLandApplication);

            // Equation 4.5.3-1
            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.NitrogenAvailableForLandApplication);

            // Equation 4.5.3-2
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                nitrogenFractionOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            // Equation 4.6.1-4
            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = base.CalculateTotalAmmoniaEmissionsFromLandAppliedManure(
                farm: farm,
                dateTime: dateTime,
                dailyEmissions: dailyEmissions,
                animalType: animalGroup.GroupType,
                temperature: temperature,
                managementPeriod: managementPeriod);

            base.GetEmissionsFromBeefAndDairyGrazingAnimals(
                managementPeriod: managementPeriod,
                temperature: temperature,
                groupEmissionsByDay: dailyEmissions);

            return dailyEmissions;
        }

        protected GroupEmissionsByDay CalculateDailyEmissionsForGroup(
            AnimalComponentBase dairyComponent,
            ManagementPeriod managementPeriod, 
            DateTime dateTime, 
            GroupEmissionsByDay previousDaysEmissions, 
            AnimalGroup animalGroup,
            Farm farm)
        {
            var dailyEmissions = new GroupEmissionsByDay();

            dailyEmissions.DateTime = dateTime;

            var temperature = farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month);

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

            // Equation 3.2.1-1
            dailyEmissions.AnimalWeight = base.GetCurrentAnimalWeight(
                startWeight: managementPeriod.StartWeight,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                startDate: managementPeriod.Start,
                currentDate: dailyEmissions.DateTime);

            // Equation 3.2.1-2
            dailyEmissions.NetEnergyForMaintenance = base.CalculateNetEnergyForMaintenance(
                maintenanceCoefficient: managementPeriod.HousingDetails.BaselineMaintenanceCoefficient,
                weight: dailyEmissions.AnimalWeight);

            // Equation 3.1.1-3
            dailyEmissions.NetEnergyForActivity = base.CalculateNetEnergyForActivity(
                feedingActivityCoefficient: managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation,
                netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);

            var totalNumberOfYoungAnimalsOnDate = dairyComponent.GetTotalNumberOfYoungAnimalsByDate(
                dateTime: dateTime,
                parentGroup: animalGroup,
                childGroupType: AnimalType.DairyCalves);

            var isLactatingAnimalGroup = totalNumberOfYoungAnimalsOnDate > 0;
            if (isLactatingAnimalGroup)
            {
                // Equation 3.2.1-4
                dailyEmissions.NetEnergyForLactation = this.CalculateNetEnergyForLactation(
                    milkProduction: managementPeriod.MilkProduction,
                    fatContent: managementPeriod.MilkFatContent);
            }

            if (animalGroup.GroupType.IsPregnantType())
            {
                // Equation 3.2.1-5
                dailyEmissions.NetEnergyForPregnancy = base.CalculateNetEnergyForPregnancy(
                    netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);
            }

            // Equation 3.2.1-7
            dailyEmissions.NetEnergyForGain = base.CalculateNetEnergyForGain(
                weight: dailyEmissions.AnimalWeight,
                gainCoefficient: managementPeriod.GainCoefficient,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                finalWeight: managementPeriod.EndWeight);

            // Equation 3.2.1-8
            dailyEmissions.RatioOfEnergyAvailableForMaintenance = base.CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy(
                totalDigestibleNutrient: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            // Equation 3.2.1-9
            dailyEmissions.RatioOfEnergyAvailableForGain = base.CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed(
                totalDigestibleNutrient: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            // Equation 3.2.1-10
            dailyEmissions.GrossEnergyIntake = base.CalculateGrossEnergyIntake(
                netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance,
                netEnergyForActivity: dailyEmissions.NetEnergyForActivity,
                netEnergyForLactation: dailyEmissions.NetEnergyForLactation,
                netEnergyForPregnancy: dailyEmissions.NetEnergyForPregnancy,
                netEnergyForGain: dailyEmissions.NetEnergyForGain,
                ratioOfEnergyAvailableForMaintenance: dailyEmissions.RatioOfEnergyAvailableForMaintenance,
                ratioOfEnergyAvailableForGain: dailyEmissions.RatioOfEnergyAvailableForGain,
                percentTotalDigestibleNutrientsInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            dailyEmissions.AdditiveReductionFactor = AdditiveReductionFactorsProvider.GetAdditiveReductionFactor(
                additiveType: managementPeriod.DietAdditive,
                numberOfDays: managementPeriod.Duration.TotalDays,
                fat: managementPeriod.SelectedDiet.Fat);


            // Equation 3.2.1-11
            dailyEmissions.EntericMethaneEmissionRate = base.CalculateEntericMethaneEmissionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                methaneConversionFactor: managementPeriod.SelectedDiet.MethaneConversionFactor,
                additiveReductionFactor: dailyEmissions.AdditiveReductionFactor);

            // Equation 3.2.1-12
            dailyEmissions.EntericMethaneEmission = base.CalculateEntericMethaneEmissions(
                entericMethaneEmissionRate: dailyEmissions.EntericMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 12.3.1-1
            dailyEmissions.DryMatterIntake = base.CalculateDryMatterIntake(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake);

            dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = base.CaclulateDailyCarbonUptakeForGroup(
                totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

            // Equation 12.3.1-5
            dailyEmissions.DryMatterIntakeMax = base.CalculateDryMatterMax(
                animalType: animalGroup.GroupType,
                finalWeightOfAnimal: managementPeriod.EndWeight);

            if (dailyEmissions.DryMatterIntake > dailyEmissions.DryMatterIntakeMax)
            {
                dailyEmissions.OptimumTdn = this.CalculateRequiredTdnSoThatMaxDmiIsNotExceeded(
                    netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance,
                    netEnergyForActivity: dailyEmissions.NetEnergyForActivity,
                    netEnergyForLactation: dailyEmissions.NetEnergyForLactation,
                    netEnergyForPregnancy: dailyEmissions.NetEnergyForPregnancy,
                    netEnergyForGain: dailyEmissions.NetEnergyForGain,
                    ratioOfEnergyForMaintenance: dailyEmissions.RatioOfEnergyAvailableForMaintenance,
                    ratioOfEnergyForGain: dailyEmissions.RatioOfEnergyAvailableForGain,
                    currentTdn: managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                    currentDmiMax: dailyEmissions.DryMatterIntakeMax);
            }

            #region Additional enteric methane (CH4) calculations

            dailyEmissions.NeutralDetergentFiberIntake = dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.NdfContent;
            dailyEmissions.AcidDetergentFiberIntake = dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.AdfContent;

            // Equation 3.2.1-13
            dailyEmissions.EntericMethaneRaminHuhtanenDairy = this.CalculateEntericMethaneEmissionsUsingRaminHuhtanenMethod(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 3.2.1-14
            dailyEmissions.EntericMethaneMillsEtAlDairy = this.CalculateEntericMethaneEmissionUsingMillsEtAl(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 3.2.1-15
            dailyEmissions.EntericMethaneEllisEtAlDairy = this.CalculateEntericMethaneEmissionUsingEllisEtAl(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                acidDetergentFiberIntake: dailyEmissions.AcidDetergentFiberIntake,
                neutralDetergentFiberIntake: dailyEmissions.NeutralDetergentFiberIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 3.2.1-16
            dailyEmissions.EntericMethaneNuiEtAlDairy = this.CalculateEntericMethaneEmissionUsingNuiEtAl(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                etherExtract: managementPeriod.SelectedDiet.Ee,
                dietaryNeutralDetergentFiber: managementPeriod.SelectedDiet.Ndf,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            #endregion

            /*
             * Manure carbon (C) and methane (CH4)
             */

            // Equation 4.1.1-1
            dailyEmissions.FecalCarbonExcretionRate = base.CalculateFecalCarbonExcretionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake);

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

            // Equation 4.1.2-1
            dailyEmissions.VolatileSolids = base.CalculateVolatileSolids(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                percentTotalDigestibleNutrientsInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                ashContentOfFeed: managementPeriod.SelectedDiet.Ash);

            /*
             * Manure methane calculations differ depending if the manure is stored as a liquid or as a solid
             *
             *  If user specifies custom a custom methane conversion factor, then skip liquid calculations (even if system is liquid, calculate manure methane using 2-4 and 2-5.)
             */

            if (managementPeriod.ManureDetails.StateType.IsSolidManure() || 
                managementPeriod.ManureDetails.UseCustomMethaneConversionFactor)
            {
                // Equation 4.1.2-4
                dailyEmissions.ManureMethaneEmissionRate = base.CalculateManureMethaneEmissionRate(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);

                // Equation 4.1.2-5
                dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                    emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }
            else
            {
                // Equation 4.1.3-8
                dailyEmissions.KelvinAirTemperature = base.CalculateDegreeKelvin(
                    degreesCelsius: temperature);

                // Equation 4.1.3-9
                dailyEmissions.ClimateFactor = base.CalculateClimateFactor(
                    kelvinAirTemperature: dailyEmissions.KelvinAirTemperature);

                // Equation 4.1.3-3
                dailyEmissions.VolatileSolidsProduced = base.CalculateVolatileSolidsProduced(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                // Equation 4.1.3-4
                dailyEmissions.VolatileSolidsLoaded = base.CalculateVolatileSolidsLoaded(
                    volatileSolidsProduced: dailyEmissions.VolatileSolidsProduced);

                // Equation 4.1.3-5
                dailyEmissions.VolatileSolidsAvailable = base.CalculateVolatileSolidsAvailable(
                    volatileSolidsLoaded: dailyEmissions.VolatileSolidsLoaded,
                    volatileSolidsAvailableFromPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsAvailable,
                    volatileSolidsConsumedFromPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsConsumed);

                // Equation 4.1.3-7
                dailyEmissions.VolatileSolidsConsumed = base.CalculateVolatileSolidsConsumed(
                    climateFactor: dailyEmissions.ClimateFactor,
                    volatileSolidsAvailable: dailyEmissions.VolatileSolidsAvailable);

                // Equation 4.1.3-8
                dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethane(
                    volatileSolidsConsumed: dailyEmissions.VolatileSolidsConsumed,
                    methaneProducingCapacityOfManure: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure);

                if (managementPeriod.ManureDetails.StateType.IsCoveredSystem())
                {
                    if (managementPeriod.ManureDetails.StateType == ManureStateType.LiquidWithNaturalCrust)
                    {
                        // Equation 4.1.3-9
                        dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethaneForCoveredSystem(
                            manureMethane: dailyEmissions.ManureMethaneEmission,
                            emissionReductionFactor: 0.4);
                    }
                    else
                    {
                        // Equation 4.1.3-9
                        dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethaneForCoveredSystem(
                            manureMethane: dailyEmissions.ManureMethaneEmission,
                            emissionReductionFactor: 0.5); // Not finalized at this time so going with 0.5 (range is 25-50%)
                    }
                }
            }

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

            // Equation 4.2.1-1
            dailyEmissions.ProteinIntake = base.CalculateProteinIntake(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                crudeProtein: managementPeriod.SelectedDiet.CrudeProteinContent);

            if (animalGroup.GroupType.IsPregnantType())
            {
                // Equation 4.2.1-2
                dailyEmissions.ProteinRetainedForPregnancy = base.CalculateProteinRetainedForPregnancy();
            }

            if (isLactatingAnimalGroup)
            {
                // Equation 4.2.1-3
                dailyEmissions.ProteinRetainedForLactation = base.CalculateProteinRetainedForLactation(
                    milkProduction: managementPeriod.MilkProduction,
                    proteinContentOfMilk: managementPeriod.MilkProteinContent,
                    numberOfYoungAnimals: totalNumberOfYoungAnimalsOnDate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

            if (managementPeriod.HasGrowingAnimals)
            {
                // Equation 4.2.1-4
                dailyEmissions.EmptyBodyWeight = base.CalculateEmptyBodyWeight(
                    weight: dailyEmissions.AnimalWeight);

                // Equation 4.2.1-5
                dailyEmissions.EmptyBodyGain = base.CalculateEmptyBodyGain(
                    averageDailyGain: dailyEmissions.AverageDailyGain);

                // Equation 4.2.1-6
                dailyEmissions.RetainedEnergy = base.CalculateRetainedEnergy(
                    emptyBodyWeight: dailyEmissions.EmptyBodyWeight,
                    emptyBodyGain: dailyEmissions.EmptyBodyGain);

                // Equation 4.2.1-7
                dailyEmissions.ProteinRetainedForGain = base.CalculateProteinRetainedForGain(
                    averageDailyGain: dailyEmissions.AverageDailyGain,
                    retainedEnergy: dailyEmissions.RetainedEnergy);
            }

            // Equation 4.2.1-8
            dailyEmissions.NitrogenExcretionRate = base.CalculateNitrogenExcretionRate(
                proteinIntake: dailyEmissions.ProteinIntake,
                proteinRetainedForPregnancy: dailyEmissions.ProteinRetainedForPregnancy,
                proteinRetainedForLactation: dailyEmissions.ProteinRetainedForLactation,
                proteinRetainedForGain: dailyEmissions.ProteinRetainedForGain);

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
             * Indirect manure N2O
             */

            this.CalculateIndirectManureNitrousOxide(
                dailyEmissions: dailyEmissions,
                managementPeriod: managementPeriod,
                animalGroup: animalGroup,
                farm: farm,
                dateTime: dateTime,
                previousDaysEmissions: previousDaysEmissions);

            // Equation 4.3.5-1
            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.3.7-1
            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            // Equation 4.5.2-1
            dailyEmissions.TanAvailableForLandApplication = base.CalculateMonthlyTanAvailableForLandApplication(
                monthlyTanEnteringStorageSystem: dailyEmissions.AdjustedAmountOfTanInStoredManure,
                ammoniaLostFromStoredManure: dailyEmissions.AmmoniaLostFromStorage);

            // Equation 4.5.2-3
            dailyEmissions.OrganicNitrogenAvailableForLandApplication = base.CalculateMonthlyOrganicNitrogenAvailableForLandApplication(
                fecalNitrogenExcretion: dailyEmissions.FecalNitrogenExcretion,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                fractionOfMineralizedNitrogen: managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized,
                directManureEmissions: dailyEmissions.ManureDirectN2ONEmission,
                leachingEmissions: dailyEmissions.ManureN2ONLeachingEmission);

            // Equation 4.5.2-5
            dailyEmissions.NitrogenAvailableForLandApplication = base.CalculateTotalAvailableManureNitrogenInStoredManure(
                tanAvailableForLandApplication: dailyEmissions.TanAvailableForLandApplication,
                organicNitrogenAvailableForLandApplication: dailyEmissions.OrganicNitrogenAvailableForLandApplication);

            // Equation 4.5.3-1
            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.NitrogenAvailableForLandApplication);

            // Equation 4.5.3-2
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                nitrogenFractionOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            // Equation 4.6.1-4
            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = base.CalculateTotalAmmoniaEmissionsFromLandAppliedManure(
                farm: farm,
                dateTime: dateTime,
                dailyEmissions: dailyEmissions,
                animalType: animalGroup.GroupType,
                temperature: temperature, 
                managementPeriod: managementPeriod);

            base.GetEmissionsFromBeefAndDairyGrazingAnimals(
                managementPeriod: managementPeriod,
                temperature: temperature,
                groupEmissionsByDay: dailyEmissions);

            return dailyEmissions;
        }

        protected override void CalculateEnergyEmissions(
            GroupEmissionsByMonth groupEmissionsByMonth, 
            Farm farm)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType != AnimalType.DairyLactatingCow)
            {
                return;
            }

            var energyConversionFactor = _energyConversionDefaultsProvider.GetElectricityConversionValue(groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = this.CalculateTotalCarbonDioxideEmissionsFromDairyHousing(
                numberOfLactatingDairyCows: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                numberOfDaysInMonth: groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth, 
                energyConversionFactor: energyConversionFactor);
        }

        protected override void CalculateEstimatesOfProduction(
            GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsLactatingType())
            {
                groupEmissionsByMonth.MonthlyMilkProduction = this.CalculateMilkProductionPerMonthFromDairyCattle(
                    milkProductionPerDay: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.MilkProduction, 
                    numberOfAnimals: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals, 
                    numberOfDaysInMonth: groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth); 

                groupEmissionsByMonth.MonthlyFatAndProteinCorrectedMilkProduction = this.FatAndProteinCorrectedMilkProductionPerMonth(
                    milkProductionForMonth: groupEmissionsByMonth.MonthlyMilkProduction,
                    fatContent: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.MilkFatContent,
                    milkProtein: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.MilkFatContent);
            }
        }

        #endregion

        #region Equations

        private void CalculateIndirectManureNitrousOxide(
            GroupEmissionsByDay dailyEmissions, 
            ManagementPeriod managementPeriod, 
            AnimalGroup animalGroup, 
            Farm farm, 
            DateTime dateTime, 
            GroupEmissionsByDay previousDaysEmissions)
        {
            // Equation 4.3.1-1
            // Equation 4.3.1-2
            dailyEmissions.FractionOfNitrogenExcretedInUrine = this.GetFractionOfNitrogenExcretedInUrineForDairy(
                crudeProteinContent: managementPeriod.SelectedDiet.CrudeProteinContent,
                animalType: animalGroup.GroupType);

            // Equation 4.3.1-3
            dailyEmissions.TanExcretionRate = base.CalculateTANExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                fractionOfNitrogenExcretedInUrine: dailyEmissions.FractionOfNitrogenExcretedInUrine);

            // Equation 4.3.1-4
            dailyEmissions.TanExcretion = base.CalculateTANExcretion(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.1-5
            dailyEmissions.FecalNitrogenExcretionRate = base.CalculateFecalNitrogenExcretionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                tanExcretionRate: dailyEmissions.TanExcretionRate);

            // Equation 4.3.1-6
            dailyEmissions.FecalNitrogenExcretion = base.CalculateFecalNitrogenExcretion(
                fecalNitrogenExcretionRate: dailyEmissions.FecalNitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.1-7
            dailyEmissions.OrganicNitrogenInStoredManure = base.CalculateOrganicNitrogenInStoredManure(
                totalNitrogenExcretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            /*
             * Ammonia (NH3) from housing
             */

            // Equation 4.3.1-8
            dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = base.CalculateAmbientTemperatureAdjustment(
                averageMonthlyTemperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));

            var ammoniaEmissionFactorForHousingType = _beefDairyDefaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                housingType: managementPeriod.HousingDetails.HousingType);

            // Equation 4.3.1-9
            dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing = base.CalculateAdjustedEmissionFactorHousing(
                emissionFactor: ammoniaEmissionFactorForHousingType,
                temperatureAdjustment: dailyEmissions.AmbientAirTemperatureAdjustmentForHousing);

            // Equation 4.3.1-10
            dailyEmissions.AmmoniaEmissionRateFromHousing = base.CalculateAmmoniaEmissionRateFromHousing(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                adjustedEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing);

            // Equation 4.3.1-11
            dailyEmissions.AmmoniaConcentrationInHousing = base.CalculateAmmoniaConcentrationInHousing(
                emissionRate: dailyEmissions.AmmoniaEmissionRateFromHousing,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.1-12
            dailyEmissions.AmmoniaEmissionsFromHousingSystem = base.CalculateTotalAmmoniaEmissionsFromHousing(
                ammoniaConcentrationInHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            /*
             * Ammonia (NH3) from storage
             */

            // Equation 4.3.2-1
            dailyEmissions.TanEnteringStorageSystem = base.CalculateTanStorage(
                tanExcretion: dailyEmissions.TanExcretion,
                tanExcretionFromPreviousPeriod: previousDaysEmissions == null ? 0 : previousDaysEmissions.TanExcretion,
                ammoniaLostByHousingDuringPreviousPeriod: previousDaysEmissions == null ? 0 : previousDaysEmissions.AmmoniaConcentrationInHousing);

            // Equation 4.3.2-2
            dailyEmissions.AdjustedAmountOfTanInStoredManure = base.CalculateAdjustedAmountOfTanInStoredManure(
                tanEnteringStorageSystem: dailyEmissions.TanEnteringStorageSystem,
                fractionOfTanImmoblizedToOrganicNitrogen: managementPeriod.ManureDetails.FractionOfOrganicNitrogenImmobilized,
                fractionOfTanNitrifiedDuringManureStorage: managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified,
                nitrogenExretedThroughFeces: dailyEmissions.FecalNitrogenExcretion,
                fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage: managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            if (managementPeriod.ManureDetails.StateType.IsSolidManure())
            {
                // Equation 4.3.2-4
                dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = this.CalculateStorageTemperatureAdjustmentForSolidManure(
                    temperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));
            }
            else
            {
                // Equation 4.3.2-5
                dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = this.CalculateStorageTemperatureAdjustmentForLiquidManure(
                    temperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));
            }

            // Equation 4.3.2-6
            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = base.CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                ambientTemperatureAdjustmentStorage: dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                ammoniaEmissionFactorStorage: managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage);

            // Equation 4.3.2-7
            dailyEmissions.AmmoniaLostFromStorage = base.CalculateAmmoniaLossFromStoredManure(
                tanInStoredManure: dailyEmissions.AdjustedAmountOfTanInStoredManure,
                ammoniaEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            // Equation 4.3.2-8
            dailyEmissions.AmmoniaEmissionsFromStorageSystem = base.CalculateAmmoniaEmissionsFromStoredManure(
                ammoniaNitrogenLossFromStoredManure: dailyEmissions.AmmoniaLostFromStorage);

            /*
             * Volatilization
             */

            if (managementPeriod.ManureDetails.UseCustomVolatilizationFraction)
            {
                dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;
            }
            else
            {
                // Equation 4.3.3-1
                dailyEmissions.FractionOfManureVolatilized = this.CalculateFractionOfManureVolatilized(
                    ammoniaEmissionsFromHousing: dailyEmissions.AmmoniaConcentrationInHousing,
                    ammoniaEmissionsFromStorage: dailyEmissions.AmmoniaLostFromStorage,
                    amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                    amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);
            }

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
             * Leaching
             */

            // Equation 4.3.4-1
            dailyEmissions.ManureNitrogenLeachingRate = base.CalculateManureLeachingNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching, 
                amountOfNitrogenAddedFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            // Equation 4.3.4-2
            dailyEmissions.ManureN2ONLeachingEmission = base.CalculateManureLeachingNitrogenEmission(
                leachingNitrogenEmissionRate: dailyEmissions.ManureNitrogenLeachingRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);
        }

        /// <summary>
        ///  Equation 3.2.1-4
        /// </summary>
        /// <param name="milkProduction">Milk production (kg head^-1 day^-1)</param>
        /// <param name="fatContent">Fat content (%)</param>
        /// <returns>Net energy for lactation (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForLactation(double milkProduction, double fatContent)
        {
            return milkProduction * (1.47 + 0.40 * fatContent);
        }

        /// <summary>
        /// Equation 3.2.4-7
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfCattle">Number of cattle</param>
        /// <param name="numberOfDays">Number of days</param>
        /// <param name="volatilizationFraction">Volatilization fraction</param>
        /// <param name="leachingFraction">Leaching fraction</param>
        /// <returns>Manure available for land application (kg N)</returns>
        public double CalculateManureAvailableForLandApplication(double nitrogenExcretionRate,
                                                                 double numberOfCattle,
                                                                 double numberOfDays,
                                                                 double volatilizationFraction,
                                                                 double leachingFraction)
        {
            var a = nitrogenExcretionRate * numberOfCattle * numberOfDays;
            var b = 1 - volatilizationFraction - leachingFraction;

            return a * b;
        }

        /// <summary>
        /// Equation 3.2.5-1
        /// </summary>
        /// <returns>Enteric CH4 emission (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionForCalves()
        {
            return 0.0;
        }

        /// <summary>
        /// Equation 3.2.6-1
        /// </summary>
        /// <returns>Volatile solids (kg head^-1 day^-1)</returns>
        public double CalculateVolatileSolidsForCalves()
        {
            return 1.42;
        }

        /// <summary>
        /// Equation 4.2.1-15
        /// </summary>
        /// <returns>Nitrogen excretion rate (kg head^-1 day^-1)</returns>
        public double CalculateNitrogenExcretionRateForCalves(
            double proteinIntake, 
            double proteinRetained)
        {
            return (proteinIntake / 6.25) - (proteinRetained / 6.25);
        }

        /// <summary>
        /// Equation 3.2.1-13
        /// Equation 3.2.1-17
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg/d)</param>
        /// <param name="numberOfAnimals">Total number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionsUsingRaminHuhtanenMethod(double dryMatterIntake,
                                                                               double numberOfAnimals)
        {
            var emissionRate = ((20 + 35.8 * dryMatterIntake - 0.5 * Math.Pow(dryMatterIntake, 2)) * 0.714) * (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        /// Equation 3.2.1-14
        /// Equation 3.2.1-17
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg/d)</param>
        /// <param name="numberOfAnimals">Total number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionUsingMillsEtAl(double dryMatterIntake,
                                                                    double numberOfAnimals)
        {
            var emissionRate = (56.27 - (56.27 * Math.Exp(-0.028 * dryMatterIntake))) * (1.0 / 55.65);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        /// Equation 3.2.1-15
        /// Equation 3.2.1-17
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg/d)</param>
        /// <param name="acidDetergentFiberIntake">Acid detergent fiber intake (kg/d)</param>
        /// <param name="neutralDetergentFiberIntake">Neutral detergent fiber intake (kg/d)</param>
        /// <param name="numberOfAnimals">Total number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionUsingEllisEtAl(double dryMatterIntake,
                                                                    double acidDetergentFiberIntake,
                                                                    double neutralDetergentFiberIntake,
                                                                    double numberOfAnimals)
        {
            var emissionRate = (2.16 + 0.493 * dryMatterIntake - 1.36 * acidDetergentFiberIntake + 1.97 * neutralDetergentFiberIntake) * (1.0 / 55.65);

            var result = emissionRate *  numberOfAnimals;

            return result;
        }

        /// <summary>
        /// Equation 3.2.1-16
        /// Equation 3.2.1-17
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg/d)</param>
        /// <param name="etherExtract">Dietary fat/ether extract (% of DM)</param>
        /// <param name="dietaryNeutralDetergentFiber">Neutral detergent fiber intake (kg/d)</param>
        /// <param name="numberOfAnimals">Total number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionUsingNuiEtAl(double dryMatterIntake,
                                                                  double etherExtract,
                                                                  double dietaryNeutralDetergentFiber,
                                                                  double numberOfAnimals)
        {
            var emissionRate = (76 + 13.5 * dryMatterIntake - 9.55 * etherExtract + 2.24 * dietaryNeutralDetergentFiber) * (1.0 / 1000.0);

            return emissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 9.4-1
        /// </summary>
        public double CalculateMilkProductionPerMonthFromDairyCattle(double milkProductionPerDay, double numberOfAnimals, double numberOfDaysInMonth)
        {
            return milkProductionPerDay * numberOfDaysInMonth * numberOfAnimals;
        }

        /// <summary>
        /// Equation 9.4-2
        /// </summary>
        public double FatAndProteinCorrectedMilkProductionPerMonth(double milkProductionForMonth, double fatContent, double milkProtein)
        {
            return milkProductionForMonth * (0.1226 * fatContent) + (0.0776 * (milkProtein - 0.19)) + 0.2534;
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
        /// Equation 4.3.2-4
        /// </summary>
        /// <param name="temperature">Average daily temperature (degrees Celsius)</param>
        /// <returns>Temperature adjustment for solid manure</returns>
        public double CalculateStorageTemperatureAdjustmentForSolidManure(double temperature)
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

        /// <summary>
        /// Equation 6.2.1-1
        /// </summary>
        /// <param name="numberOfLactatingDairyCows">Number of dairy cows</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <param name="energyConversionFactor">Electricity conversion factor (kg CO2 kWh^-1)</param>
        /// <returns>Total CO2 emissions from dairy operations (kg CO2 year^-1) - for each lactating group</returns>
        public double CalculateTotalCarbonDioxideEmissionsFromDairyHousing(
            double numberOfLactatingDairyCows, 
            double numberOfDaysInMonth, 
            double energyConversionFactor)
        {
            const double DairyCowConversion = 968;

            return numberOfLactatingDairyCows * (DairyCowConversion /CoreConstants.DaysInYear) * energyConversionFactor * numberOfDaysInMonth;
        }

        #endregion
    }
}