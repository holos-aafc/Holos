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
    public class DairyCattleResultsService : BeefAndDairyResultsServiceBase, IDairyResultsService
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

            this.InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            /*
             * Enteric methane (CH4)
             */

            if (managementPeriod.PeriodDailyGain > 0)
            {
                dailyEmissions.AverageDailyGain = managementPeriod.PeriodDailyGain;
            }
            else
            {
                dailyEmissions.AverageDailyGain = base.CalculateAverageDailyWeightGain(
                    initialWeight: managementPeriod.StartWeight,
                    finalWeight: managementPeriod.EndWeight,
                    numberOfDays: managementPeriod.Duration.TotalDays);
            }

            dailyEmissions.AnimalWeight = base.GetCurrentAnimalWeight(
                startWeight: managementPeriod.StartWeight,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                startDate: managementPeriod.Start,
                currentDate: dailyEmissions.DateTime);

            dailyEmissions.DryMatterIntake = base.CalculateDryMatterIntakeForCalves( 0, dailyEmissions.AnimalWeight, true);

            dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = base.CalculateDailyCarbonUptakeForGroup(
                totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

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
            dailyEmissions.VolatileSolids = (9.3 / 1000.0) * dailyEmissions.AnimalWeight;

            /*
             * Manure methane calculations differ depending if the manure is stored as a liquid or as a solid
             */

            var temperature = farm.ClimateData.GetMeanTemperatureForDay(dateTime);

            if (managementPeriod.ManureDetails.StateType.IsSolidManure())
            {
                dailyEmissions.ManureMethaneEmissionRate = base.CalculateManureMethaneEmissionRate(
                    volatileSolids: dailyEmissions.VolatileSolids,
                    methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);

                dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                    emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }
            else
            {
                base.CalculateManureMethaneFromLiquidSystems(
                    dailyEmissions,
                    previousDaysEmissions,
                    managementPeriod,
                    temperature);
            }

            base.CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            dailyEmissions.NitrogenExcretionRate = 0.078;

            dailyEmissions.AmountOfNitrogenExcreted = base.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-31
            dailyEmissions.AmountOfNitrogenAddedFromBedding = 0;

            dailyEmissions.ManureDirectN2ONEmissionRate = base.CalculateManureDirectNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                emissionFactor: managementPeriod.ManureDetails.N2ODirectEmissionFactor);

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
                dateTime: dateTime,
                previousDaysEmissions: previousDaysEmissions, 
                temperature: temperature);

            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay = base.CalculateAccumulatedTanAvailableForLandApplication(
                accumulatedTANEnteringStorageSystemOnDay: dailyEmissions.AccumulatedTanInStorageOnDay);

            base.CalculateOrganicNitrogen(dailyEmissions, managementPeriod, previousDaysEmissions);

            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay = base.CalculateTotalAvailableManureNitrogenInStoredManure(
                tanAvailableForLandApplication: dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay,
                organicNitrogenAvailableForLandApplication: dailyEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay,
                nitrogenContentOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            base.GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
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

            this.InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            var temperature = farm.ClimateData.GetMeanTemperatureForDay(dateTime);

            /*
             * Enteric methane (CH4)
             */

            if (managementPeriod.PeriodDailyGain > 0)
            {
                dailyEmissions.AverageDailyGain = managementPeriod.PeriodDailyGain;
            }
            else
            {
                dailyEmissions.AverageDailyGain = base.CalculateAverageDailyWeightGain(
                    initialWeight: managementPeriod.StartWeight,
                    finalWeight: managementPeriod.EndWeight,
                    numberOfDays: managementPeriod.Duration.TotalDays);
            }

            dailyEmissions.AnimalWeight = base.GetCurrentAnimalWeight(
                startWeight: managementPeriod.StartWeight,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                startDate: managementPeriod.Start,
                currentDate: dailyEmissions.DateTime);

            dailyEmissions.NetEnergyForMaintenance = base.CalculateNetEnergyForMaintenance(
                maintenanceCoefficient: managementPeriod.HousingDetails.BaselineMaintenanceCoefficient,
                weight: dailyEmissions.AnimalWeight);

            dailyEmissions.NetEnergyForActivity = base.CalculateNetEnergyForActivity(
                feedingActivityCoefficient: managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation,
                netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);

            managementPeriod.NumberOfYoungAnimals = dairyComponent.GetTotalNumberOfYoungAnimalsByDate(
                dateTime: dateTime,
                parentGroup: animalGroup,
                childGroupType: AnimalType.DairyCalves);

            if (managementPeriod.AnimalType.IsLactatingType())
            {
                // Lactating dairy cows are always lactating - even if they are separated from the calves. This means the lactation calculations are always used regardless if any
                // associated groups of calves. This differs from beef cattle cows/calves where if the calves are removed then the lactation stops.

                dailyEmissions.NetEnergyForLactation = this.CalculateNetEnergyForLactation(
                    milkProduction: managementPeriod.MilkProduction,
                    fatContent: managementPeriod.MilkFatContent);
            }

            if (animalGroup.GroupType.IsPregnantType())
            {
                dailyEmissions.NetEnergyForPregnancy = base.CalculateNetEnergyForPregnancy(
                    netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);
            }

            dailyEmissions.NetEnergyForGain = base.CalculateNetEnergyForGain(
                weight: dailyEmissions.AnimalWeight,
                gainCoefficient: managementPeriod.GainCoefficient,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                finalWeight: managementPeriod.EndWeight);

            dailyEmissions.RatioOfEnergyAvailableForMaintenance = base.CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy(
                totalDigestibleNutrient: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            dailyEmissions.RatioOfEnergyAvailableForGain = base.CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed(
                totalDigestibleNutrient: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

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

            dailyEmissions.EntericMethaneEmissionRate = base.CalculateEntericMethaneEmissionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                methaneConversionFactor: managementPeriod.SelectedDiet.MethaneConversionFactor,
                additiveReductionFactor: dailyEmissions.AdditiveReductionFactor);

            dailyEmissions.EntericMethaneEmission = base.CalculateEntericMethaneEmissions(
                entericMethaneEmissionRate: dailyEmissions.EntericMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 12.3.1-1
            dailyEmissions.DryMatterIntake = base.CalculateDryMatterIntake(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake);

            dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = base.CalculateDailyCarbonUptakeForGroup(
                totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

            // Equation 12.3.1-5
            dailyEmissions.DryMatterIntakeMax = base.CalculateDryMatterMax(finalWeightOfAnimal: managementPeriod.EndWeight);

            if (base.IsOverDmiMax(dailyEmissions))
            {
                dailyEmissions.DryMatterIntake = dailyEmissions.DryMatterIntakeMax;

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

            dailyEmissions.EntericMethaneRaminHuhtanenDairy = this.CalculateEntericMethaneEmissionsUsingRaminHuhtanenMethod(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.EntericMethaneMillsEtAlDairy = this.CalculateEntericMethaneEmissionUsingMillsEtAl(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.EntericMethaneEllisEtAlDairy = this.CalculateEntericMethaneEmissionUsingEllisEtAl(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                acidDetergentFiberIntake: dailyEmissions.AcidDetergentFiberIntake,
                neutralDetergentFiberIntake: dailyEmissions.NeutralDetergentFiberIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.EntericMethaneNuiEtAlDairy = this.CalculateEntericMethaneEmissionUsingNuiEtAl(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                etherExtract: managementPeriod.SelectedDiet.Ee,
                dietaryNeutralDetergentFiber: managementPeriod.SelectedDiet.Ndf,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            #endregion

            /*
             * Manure carbon (C) and methane (CH4)
             */

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

            dailyEmissions.VolatileSolids = base.CalculateVolatileSolids(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                percentTotalDigestibleNutrientsInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                ashContentOfFeed: managementPeriod.SelectedDiet.Ash,
                percentageForageInDiet: managementPeriod.SelectedDiet.Forage);

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
                base.CalculateManureMethaneFromLiquidSystems(
                    dailyEmissions,
                    previousDaysEmissions,
                    managementPeriod,
                    temperature);
            }

            base.CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            var isLactatingGroup = animalGroup.GroupType == AnimalType.DairyLactatingCow;
            base.CalculateDirectN2OFromBeefAndDairy(
                dailyEmissions,
                managementPeriod,
                animalGroup,
                isLactatingGroup,
                managementPeriod.NumberOfYoungAnimals);

            /*
             * Indirect manure N2O
             */

            this.CalculateIndirectManureNitrousOxide(
                dailyEmissions: dailyEmissions,
                managementPeriod: managementPeriod,
                animalGroup: animalGroup,
                dateTime: dateTime,
                previousDaysEmissions: previousDaysEmissions, 
                temperature: temperature);

            dailyEmissions.ManureIndirectN2ONEmission = base.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);

            dailyEmissions.ManureN2ONEmission = base.CalculateManureNitrogenEmission(
                manureDirectNitrogenEmission: dailyEmissions.ManureDirectN2ONEmission,
                manureIndirectNitrogenEmission: dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay = base.CalculateAccumulatedTanAvailableForLandApplication(
                accumulatedTANEnteringStorageSystemOnDay: dailyEmissions.AccumulatedTanInStorageOnDay);

            base.CalculateOrganicNitrogen(dailyEmissions, managementPeriod, previousDaysEmissions);

            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay = base.CalculateTotalAvailableManureNitrogenInStoredManure(
                tanAvailableForLandApplication: dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay,
                organicNitrogenAvailableForLandApplication: dailyEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.ManureCarbonNitrogenRatio = base.CalculateManureCarbonToNitrogenRatio(
                carbonFromStorage: dailyEmissions.AmountOfCarbonInStoredManure,
                nitrogenFromManure: dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay,
                nitrogenContentOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            base.GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
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
        /// Equation 4.2.1-3
        /// 
        /// Overriden since diary lactating cows always lactate even if there are no associated calves.
        /// </summary>
        /// <param name="milkProduction">Milk production (kg head^-1 day^-1)</param>
        /// <param name="proteinContentOfMilk">Protein content of milk (kg kg⁻¹)</param>
        /// <param name="numberOfYoungAnimals">Number of calves</param>
        /// <param name="numberOfAnimals">Number of cows</param>
        /// <param name="animalsAreAlwaysLactating">Indicates if the animal is always lactating regardless of the number of young animals present</param>
        /// <returns>Protein retained for lactation (kg head^-1 day^-1)</returns>
        public override double CalculateProteinRetainedForLactation(double milkProduction,
            double proteinContentOfMilk,
            double numberOfYoungAnimals,
            double numberOfAnimals, 
            bool animalsAreAlwaysLactating)
        {
            return milkProduction * proteinContentOfMilk;
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