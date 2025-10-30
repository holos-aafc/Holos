#region Imports

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
using H.Core.Models.LandManagement.Fields;

#endregion

namespace H.Core.Services.Animals
{
    /// <summary>
    /// </summary>
    public class BeefCattleResultsService : BeefAndDairyResultsServiceBase, IBeefCattleResultsService
    {
        #region Fields

        #endregion

        #region Constructors

        public BeefCattleResultsService() : base()
        {
            _animalComponentCategory = ComponentCategory.BeefProduction;
        }

        #endregion

        #region Private Methods

        protected GroupEmissionsByDay CalculateDailyEmissionsForCalves(
            ManagementPeriod managementPeriod,
            DateTime dateTime,
            AnimalComponentBase cowCalfComponent,
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

            var nemf = 0d;
            if (managementPeriod.SelectedDiet.DietaryNetEnergyConcentration == 0)
            {
                // Default/system diets will have a predefined (lookup table) NEmf value
                nemf = managementPeriod.SelectedDiet.CalculateNemf();
            }
            else
            {
                // Custom diets will not have a predefined value and must therefore be calculated
                nemf = managementPeriod.SelectedDiet.DietaryNetEnergyConcentration;
            }

            dailyEmissions.DryMatterIntake = base.CalculateDryMatterIntakeForCalves(
                dietaryNetEnergyConcentration: nemf,
                weight: dailyEmissions.AnimalWeight, areMilkFedOnly: 
                managementPeriod.AnimalsAreMilkFedOnly);

            dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = base.CalculateDailyCarbonUptakeForGroup(
                totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

            dailyEmissions.GrossEnergyIntake = CalculateGrossEnergyIntakeForCalves(
                dryMatterIntake: dailyEmissions.DryMatterIntake);

            dailyEmissions.AdditiveReductionFactor = AdditiveReductionFactorsProvider.GetAdditiveReductionFactor(
                additiveType: managementPeriod.DietAdditive,
                numberOfDays: managementPeriod.Duration.TotalDays,
                fat: managementPeriod.SelectedDiet.Fat);

            dailyEmissions.EntericMethaneEmissionRate = base.CalculateEntericMethaneEmissionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                methaneConversionFactor: managementPeriod.SelectedDiet.MethaneConversionFactor,
                additiveReductionFactor: dailyEmissions.AdditiveReductionFactor);

            if (managementPeriod.AnimalsAreMilkFedOnly)
            {
                dailyEmissions.EntericMethaneEmission = 0;
            }
            else
            {
                dailyEmissions.EntericMethaneEmission = base.CalculateEntericMethaneEmissions(
                    entericMethaneEmissionRate: dailyEmissions.EntericMethaneEmissionRate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

            /*
             * Manure carbon (C) and methane (CH4)
             */

            dailyEmissions.FecalCarbonExcretionRate = base.CalculateFecalCarbonExcretionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake);

            dailyEmissions.FecalCarbonExcretion = base.CalculateAmountOfFecalCarbonExcreted(
                excretionRate: dailyEmissions.FecalCarbonExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-6
            dailyEmissions.CarbonAddedFromBeddingMaterial = 0;

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = base.CalculateAmountOfCarbonFromManureAndBedding(
                carbonExcreted: dailyEmissions.FecalCarbonExcretion,
                carbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial);

            if (managementPeriod.AnimalsAreMilkFedOnly)
            {
                dailyEmissions.VolatileSolids = (7.6 / 1000.0) * dailyEmissions.AnimalWeight;
            }
            else
            {
                // Equation 4.1.2-1
                dailyEmissions.VolatileSolids = base.CalculateVolatileSolids(
                    grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                    percentTotalDigestibleNutrientsInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                    ashContentOfFeed: managementPeriod.SelectedDiet.Ash,
                    percentageForageInDiet: managementPeriod.SelectedDiet.Forage);
            }

            // Equation 4.1.2-4
            dailyEmissions.ManureMethaneEmissionRate = base.CalculateManureMethaneEmissionRate(
                volatileSolids: dailyEmissions.VolatileSolids,
                methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);

            // Equation 4.1.2-5
            dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            base.CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            dailyEmissions.ProteinIntakeFromSolidFood = this.CalculateCalfProteinIntakeFromSolidFood(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                crudeProteinContent: managementPeriod.SelectedDiet.CrudeProteinContent);

            var managementPeriodOfCows = cowCalfComponent.GetAssociatedManagementPeriodOfParentGroup(
                youngAnimalGroup: animalGroup,
                parentGroupType: AnimalType.BeefCowLactating,
                dateTime: dateTime);

            if (managementPeriodOfCows != null)
            {
                dailyEmissions.ProteinIntakeFromMilk = base.CalculateCalfProteinIntakeFromMilk(
                    milkProduction: managementPeriodOfCows.MilkProduction,
                    proteinContentOfMilk: managementPeriodOfCows.MilkProteinContent);
            }

            dailyEmissions.ProteinIntake = base.CalculateCalfProteinIntake(
                calfProteinIntakeFromMilk: dailyEmissions.ProteinIntakeFromMilk,
                calfProteinIntakeFromSolidFood: dailyEmissions.ProteinIntakeFromSolidFood, 
                areMilkFedOnly: managementPeriod.AnimalsAreMilkFedOnly);

            dailyEmissions.ProteinRetainedFromSolidFood = managementPeriod.AnimalsAreMilkFedOnly ? 0 : base.CalculateCalfProteinRetainedFromSolidFeed(
                calfProteinIntakeFromSolidFood: dailyEmissions.ProteinIntakeFromSolidFood);

            dailyEmissions.ProteinRetainedFromMilk = CalculateCalfProteinRetainedFromMilk(
                calfProteinIntakeFromMilk: dailyEmissions.ProteinIntakeFromMilk);

            dailyEmissions.ProteinRetained = CalculateCalfProteinRetained(
                calfProteinRetainedFromMilk: dailyEmissions.ProteinRetainedFromMilk,
                calfProteinRetainedFromSolidFeed: dailyEmissions.ProteinRetainedFromSolidFood);

            dailyEmissions.NitrogenExcretionRate = CalculateCalfNitrogenExcretionRate(
                calfProteinIntake: dailyEmissions.ProteinIntake,
                calfProteinRetained: dailyEmissions.ProteinRetained);

            // Used in volatilization calculation
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
                temperature: temperature, 
                farm: farm);

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
                carbonFromStorage: dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                nitrogenFromManure: dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay = dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay + dailyEmissions.OrganicNitrogenCreatedOnDay;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay,
                nitrogenContentOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedVolume);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            base.GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
                managementPeriod: managementPeriod,
                temperature: temperature,
                groupEmissionsByDay: dailyEmissions);

            return dailyEmissions;
        }

        protected GroupEmissionsByDay CalculateDailyEmissionsForGroup(
            AnimalComponentBase cowCalfComponent,
            ManagementPeriod managementPeriod,
            DateTime dateTime,
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

            dailyEmissions.AdjustedMaintenanceCoefficient = base.CalculateTemperatureAdjustedMaintenanceCoefficient(
                baselineMaintenanceCoefficient: managementPeriod.HousingDetails.BaselineMaintenanceCoefficient,
                dailyTemperature: dailyEmissions.Temperature);

            dailyEmissions.NetEnergyForMaintenance = base.CalculateNetEnergyForMaintenance(
                maintenanceCoefficient: dailyEmissions.AdjustedMaintenanceCoefficient,
                weight: dailyEmissions.AnimalWeight);

            dailyEmissions.NetEnergyForActivity = base.CalculateNetEnergyForActivity(
                feedingActivityCoefficient: managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation,
                netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);

            managementPeriod.NumberOfYoungAnimals = cowCalfComponent.GetTotalNumberOfYoungAnimalsByDate(
                dateTime: dateTime,
                parentGroup: animalGroup,
                childGroupType: AnimalType.BeefCalf);

            var isLactatingAnimalGroup = managementPeriod.NumberOfYoungAnimals > 0;
            if (isLactatingAnimalGroup)
            {
                // Beef cattle lactating cows stop lactating if there are no associated young animals - this differs from lactating dairy cows who are always lactating regardless of the fact
                // that there may or may not be any associated calf groups.

                dailyEmissions.NetEnergyForLactation = this.CalculateNetEnergyForLactation(
                    milkProduction: managementPeriod.MilkProduction,
                    fatContent: managementPeriod.MilkFatContent,
                    numberOfYoungAnimals: managementPeriod.NumberOfYoungAnimals,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

            if (animalGroup.GroupType.IsPregnantType())
            {
                /*
                 * Cows are always pregnant during the entire year
                 */

                dailyEmissions.NetEnergyForPregnancy = base.CalculateNetEnergyForPregnancy(
                    netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);
            }

            /*
             * Equation 3.1.1-7 at beginning of this method
             */

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

            dailyEmissions.DryMatterIntake = base.CalculateDryMatterIntake(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake);

            dailyEmissions.DryMatterIntakeMax = base.CalculateDryMatterMax(finalWeightOfAnimal: managementPeriod.EndWeight, animalType: managementPeriod.AnimalType);

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

            dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = base.CalculateDailyCarbonUptakeForGroup(
                totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

            #region Additional enteric methane (CH4) calculations

            dailyEmissions.CrudeFatIntake = (dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.FatContent) + (dailyEmissions.DryMatterIntake * managementPeriod.DietAdditive.GetFatFromAdditive());
            dailyEmissions.CrudeProteinIntake = dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.CrudeProteinContent;
            dailyEmissions.NeutralDetergentFiberIntake = dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.NdfContent;
            dailyEmissions.StarchIntake = dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.StarchContent;

            if (managementPeriod.SelectedDiet.Forage >= 25)
            {
                // Equation 3.1.1-14
                // Equation 3.1.1-18
                dailyEmissions.EntericMethaneEscobarEtAlAlOrBeef = this.CalculateEntericMethaneUsingEscobarAlOrEtAl(
                    bodyWeight: dailyEmissions.AnimalWeight,
                    forageInDiet: managementPeriod.SelectedDiet.Forage,
                    crudeFatIntake: dailyEmissions.CrudeFatIntake,
                    grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                // Equation 3.1.1-15
                // Equation 3.1.1-18
                dailyEmissions.EntericMethaneLingenEtAlBeef = this.CalculateEntericMethaneEmissionUsingLingenEtAl(
                    dryMatterIntake: dailyEmissions.DryMatterIntake,
                    dietaryForage: managementPeriod.SelectedDiet.Forage,
                    bodyWeight: dailyEmissions.AnimalWeight,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }
            else
            {
                // Equation 3.1.1-16
                // Equation 3.1.1-18
                dailyEmissions.EntericMethaneEscobarEtAlLfMcBeef = this.CalculateEntericMethaneEmissionUsingEscobarLfMc(
                    bodyWeight: dailyEmissions.AnimalWeight,
                    dryMatterIntake: dailyEmissions.DryMatterIntake,
                    crudeFatIntake: dailyEmissions.CrudeFatIntake,
                    crudeProteinIntake: dailyEmissions.CrudeProteinIntake,
                    neutralDetergentFiberIntake: dailyEmissions.NeutralDetergentFiberIntake,
                    starchIntake: dailyEmissions.StarchIntake,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                // Equation 3.1.1-17
                // Equation 3.1.1-18
                dailyEmissions.EntericMethaneEllisEtAlBeef = this.CalculateEntericMethaneEmissionUsingEllisEtAl(
                    dryMatterIntake: dailyEmissions.DryMatterIntake,
                    dietaryStarch: dailyEmissions.StarchIntake,
                    dietaryNeutralDetergentFiber: dailyEmissions.NeutralDetergentFiberIntake,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

            #endregion

            /*
             * Manure carbon (C) and methane (CH4)
             */

            dailyEmissions.FecalCarbonExcretionRate = base.CalculateFecalCarbonExcretionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake);

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

            dailyEmissions.VolatileSolids = base.CalculateVolatileSolids(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                percentTotalDigestibleNutrientsInFeed: managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                ashContentOfFeed: managementPeriod.SelectedDiet.Ash,
                percentageForageInDiet: managementPeriod.SelectedDiet.Forage);

            dailyEmissions.ManureMethaneEmissionRate = base.CalculateManureMethaneEmissionRate(
                volatileSolids: dailyEmissions.VolatileSolids,
                methaneProducingCapacity: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                methaneConversionFactor: managementPeriod.ManureDetails.MethaneConversionFactor);

            dailyEmissions.ManureMethaneEmission = base.CalculateManureMethane(
                emissionRate: dailyEmissions.ManureMethaneEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            base.CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            base.CalculateDirectN2OFromBeefAndDairy(
                dailyEmissions,
                managementPeriod,
                animalGroup,
                isLactatingAnimalGroup,
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
                temperature: dailyEmissions.Temperature, 
                farm: farm);

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
                carbonFromStorage: dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                nitrogenFromManure: dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay = dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay + dailyEmissions.OrganicNitrogenCreatedOnDay;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay,
                nitrogenContentOfManure: managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedVolume);

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            base.GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
                managementPeriod: managementPeriod,
                temperature: dailyEmissions.Temperature,
                groupEmissionsByDay: dailyEmissions);

            return dailyEmissions;
        }

        protected override void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm, AnimalComponentBase animalComponentBase)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.HousingDetails.HousingType.IsElectricalConsumingHousingType() == false)
            {
                return;
            }

            var energyConversionFactor = 0d;
            if (farm.Defaults.UseCustomElectricityConversionFactorForBeef)
            {
                energyConversionFactor = farm.Defaults.ConversionOfElectricityToCo2;
            }
            else
            {
                energyConversionFactor = _energyConversionDefaultsProvider.GetElectricityConversionValue(groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
            }

            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = this.CalculateTotalCarbonDioxideEmissionsFromHousedBeefOperations(
                numberOfCattle: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                numberOfDaysInMonth: groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth,
                energyConversionFactor: energyConversionFactor, 
                housedBeefConversionFactor: farm.Defaults.ElectricityHousedBeef);
        }

        protected override void CalculateEstimatesOfProduction(
            GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm)
        {
            groupEmissionsByMonth.MonthlyBeefProduced = this.CalculateBeefProducedPerMonth(
                averageDailyGain: groupEmissionsByMonth.AverageDailyGain,
                numberOfAnimals: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                numberOfDaysInMonth: groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth);
        }

        #endregion

        #region Overrides

        protected override GroupEmissionsByDay CalculateDailyEmissions(
            AnimalComponentBase animalComponentBase,
            ManagementPeriod managementPeriod,
            DateTime dateTime,
            GroupEmissionsByDay previousDaysEmissions,
            AnimalGroup animalGroup,
            Farm farm)
        {
            GroupEmissionsByDay groupEmissionsByDay;

            if (animalGroup.GroupType == AnimalType.BeefCalf)
            {
                groupEmissionsByDay = this.CalculateDailyEmissionsForCalves(
                    managementPeriod: managementPeriod,
                    dateTime: dateTime,
                    cowCalfComponent: animalComponentBase,
                    previousDaysEmissions: previousDaysEmissions,
                    animalGroup: animalGroup,
                    farm: farm);
            }
            else
            {
                groupEmissionsByDay = this.CalculateDailyEmissionsForGroup(
                    cowCalfComponent: animalComponentBase,
                    managementPeriod: managementPeriod,
                    dateTime: dateTime,
                    previousDaysEmissions: previousDaysEmissions,
                    animalGroup: animalGroup,
                    farm: farm
                );
            }

            return groupEmissionsByDay;
        }

        #endregion

        #region Equations

        /// <summary>
        /// Equation 3.1.1-5
        /// </summary>
        /// <param name="milkProduction">Milk production (kg head⁻¹ day⁻¹)</param>
        /// <param name="fatContent">Fat content (%)</param>
        /// <param name="numberOfYoungAnimals">Number of young animals</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Net energy for lactation (MJ head⁻¹ day⁻¹)</returns>
        public double CalculateNetEnergyForLactation(double milkProduction,
                                                     double fatContent,
                                                     double numberOfYoungAnimals,
                                                     double numberOfAnimals)
        {
            if (Math.Abs(numberOfAnimals) < double.Epsilon)
            {
                return 0;
            }

            var v = 1.47 + 0.40 * fatContent;
            var netEnergyForLactation = milkProduction * v * (numberOfYoungAnimals / numberOfAnimals);
            return netEnergyForLactation;
        }

        /// <summary>
        /// Equation 3.1.1-14
        /// Equation 3.1.1-18
        /// </summary>
        /// <param name="bodyWeight">Body weight (kg)</param>
        /// <param name="forageInDiet">% forage in diet (% DM)</param>
        /// <param name="crudeFatIntake">Ether extract/crude fat intake (kg head⁻¹ day⁻¹)</param>
        /// <param name="grossEnergyIntake">Gross energy intake (MJ head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneUsingEscobarAlOrEtAl(double bodyWeight,
                                                                  double forageInDiet,
                                                                  double crudeFatIntake,
                                                                  double grossEnergyIntake,
                                                                  double numberOfAnimals)
        {
            var emissionRate = (-35 + 0.08 * bodyWeight + 1.2 * forageInDiet - 69.8 * Math.Pow(crudeFatIntake, 3) + (3.14 * (grossEnergyIntake / 4.184))) * (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        /// Equation 3.1.1-15
        /// Equation 3.1.1-18
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg head^-1 day^-1)</param>
        /// <param name="dietaryForage">Dietary forage (% DM)</param>
        /// <param name="bodyWeight">Body weight (kg)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionUsingLingenEtAl(double dryMatterIntake,
                                                                     double dietaryForage,
                                                                     double bodyWeight,
                                                                     double numberOfAnimals)
        {
            var emissionRate = (-6.41 + 11.3 * dryMatterIntake + 0.557 * dietaryForage + 0.0996 * bodyWeight) * (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        /// Equation 3.1.1-16
        /// Equation 3.1.1-18
        /// </summary>
        /// <param name="bodyWeight">Body weight (kg)</param>
        /// <param name="dryMatterIntake">Dry matter intake (kg head^-1 day^-1)</param>
        /// <param name="crudeFatIntake">Ether extract/ crude fat intake (kg head^-1 day^-1)</param>
        /// <param name="crudeProteinIntake">Crude protein intake (kg head^-1 day^-1)</param>
        /// <param name="neutralDetergentFiberIntake">Neutral detergent fiber intake (kg head^-1 day^-1)</param>
        /// <param name="starchIntake">Starch intake (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionUsingEscobarLfMc(double bodyWeight,
                                                                      double dryMatterIntake,
                                                                      double crudeFatIntake,
                                                                      double crudeProteinIntake,
                                                                      double neutralDetergentFiberIntake,
                                                                      double starchIntake,
                                                                      double numberOfAnimals)
        {
            var emissionRate = (-10.1 + 0.21 * bodyWeight + 0.36 * Math.Pow(dryMatterIntake, 2) -
                                69.2 * Math.Pow(crudeFatIntake, 3) +
                                13 * (crudeProteinIntake / neutralDetergentFiberIntake) -
                                4.9 * (starchIntake / neutralDetergentFiberIntake)) * (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        /// Equation 3.1.1-17
        /// Equation 3.1.1-18
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg head^-1 day^-1)</param>
        /// <param name="dietaryStarch">Dietary starch (% of DM)</param>
        /// <param name="dietaryNeutralDetergentFiber">Dietary neutral detergent fiber (% of DM)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionUsingEllisEtAl(double dryMatterIntake,
                                                                    double dietaryStarch,
                                                                    double dietaryNeutralDetergentFiber,
                                                                    double numberOfAnimals)
        {
            var emissionRate = (48.2 + 14.4 * dryMatterIntake - 20.5 * (dietaryStarch / dietaryNeutralDetergentFiber)) * (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        /// Equation 11.4.2-1
        /// </summary>
        /// <param name="averageDailyGain"></param>
        /// <param name="numberOfAnimals"></param>
        /// <param name="numberOfDaysInMonth"></param>
        /// <returns></returns>
        public double CalculateBeefProducedPerMonth(double averageDailyGain,
                                                    double numberOfAnimals,
                                                    double numberOfDaysInMonth)
        {
            return averageDailyGain * numberOfDaysInMonth * numberOfAnimals;
        }


        /// <summary>
        /// Equation 3.1.10-1
        /// </summary>
        /// <param name="totalCarbonInStoredManure">Total amount of carbon in stored manure (kg C year-1)</param>
        /// <param name="totalVolumeOfManureProduced">Total volume of manure produced from different housing and storage systems (1000 kg wet weight for solid manure and 1000 litres for liquid manure)</param>
        /// <returns>Fraction of carbon in field applied manaure (kg C/1000 kg wet weight)</returns>
        public double CalculateFractionOfCarbonInFieldAppliedManure(
            double totalCarbonInStoredManure,
            double totalVolumeOfManureProduced)
        {
            if (Math.Abs(totalVolumeOfManureProduced) < double.Epsilon)
            {
                return 0;
            }

            return totalCarbonInStoredManure / totalVolumeOfManureProduced;
        }

        /// <summary>
        /// Equation 3.1.10-2
        /// </summary>
        /// <param name="totalAvailableNitrogenInStoredManure">Total available manure nitrogen in stored manure (kg N year-1)</param>
        /// <param name="totalTotalAmmoniaEmissionsFromLandApplication">Total ammonia emissions from land applied manure sourced from different housing and storage systems (kg NH3 year-1)</param>
        /// <param name="totalVolumeOfManure">Total volume of manure produced from different housing and storage systems (1000 kg wet weight for solid manure and 1000 litres for liquid manure)</param>
        /// <returns>Fraction of nitrogen in field applied manaure (kg N/1000 kg wet weight)</returns>
        public double CalculateFractionOfNitrogenAppliedToSoil(
            double totalAvailableNitrogenInStoredManure,
            double totalTotalAmmoniaEmissionsFromLandApplication,
            double totalVolumeOfManure)
        {
            if (Math.Abs(totalVolumeOfManure) < Double.Epsilon)
            {
                return 0;
            }

            var numerator = totalAvailableNitrogenInStoredManure - (totalTotalAmmoniaEmissionsFromLandApplication * (14.0 / 17.0));

            return numerator / totalVolumeOfManure;
        }

        /// <summary>
        /// Equation 3.1.11-6
        /// </summary>
        /// <param name="ammoniaEmissionsFromAllPastures">Ammonia emission from beef cattle on pasture (kg NH3)</param>
        /// <returns>Total ammonia emissions from grazing animal on pasture (kg NH3 year-1)</returns>
        public double CalculateTotalAmmoniaEmissionsFromPasture(List<double> ammoniaEmissionsFromAllPastures)
        {
            return ammoniaEmissionsFromAllPastures.Sum();
        }

        /// <summary>
        /// Equation 3.1.11-7
        /// </summary>
        /// <param name="carbonFromAnimalsOnAllPasture">Amount of C excreted (kg C)</param>
        /// <returns>Total amount of carbon from grazing animals on pasture (kg C year-1)</returns>
        public double CalculateTotalCarbonFromPastures(List<double> carbonFromAnimalsOnAllPasture)
        {
            return carbonFromAnimalsOnAllPasture.Sum();
        }

        /// <summary>
        /// Equation 3.1.11-8
        /// </summary>
        /// <param name="nitrogenFromAnimalsOnAllPasture">Amount of N excreted (kg C)</param>
        /// <returns>Total amount of carbon from grazing animals on pasture (kg C year-1)</returns>
        public double CalculateTotalNitrogenFromPastures(List<double> nitrogenFromAnimalsOnAllPasture)
        {
            return nitrogenFromAnimalsOnAllPasture.Sum();
        }

        /// <summary>
        /// Equation 3.1.12-1
        /// </summary>
        /// <param name="carbonStoredInManure">Total amount of carbon in stored manure (kg C year-1)</param>
        /// <param name="totalVolumeOfManure">Total volume of manure produced from different housing and storage systems (1000 kg wet weight for solid manure and 1000 litres for liquid manure)</param>
        /// <returns>Fraction of carbon in field applied manure (kg C/1000 kg wet weight)</returns>
        public double CalculateFractionOfCarbonInManureFromGrazingAnimals(
            double carbonStoredInManure,
            double totalVolumeOfManure)
        {
            if (Math.Abs(totalVolumeOfManure) < Double.Epsilon)
            {
                return 0;
            }

            return carbonStoredInManure / totalVolumeOfManure;
        }

        /// <summary>
        /// Equation 3.1.12-2
        /// </summary>
        /// <param name="availableNitrogenInManure">Total available manure nitrogen in stored manure (kg N year-1) </param>
        /// <param name="totalAmmoniaFromLandApplication">Total ammonia emissions from land applied manure sourced from different housing and storage systems (kg NH3 year-1)</param>
        /// <param name="volumeOfManure">Total volume of manure produced from different housing and storage systems (1000 kg wet weight for solid manure and 1000 litres for liquid manure)</param>
        /// <returns>Fraction of nitrogen added to pasture soil by grazing animals (kg N/1000 kg wet weight)</returns>
        public double CalculateFractionOfNitrogenAppliedByGrazingAnimals(
            double availableNitrogenInManure,
            double totalAmmoniaFromLandApplication,
            double volumeOfManure)
        {
            if (Math.Abs(volumeOfManure) < Double.Epsilon)
            {
                return 0;
            }

            return ((availableNitrogenInManure - (totalAmmoniaFromLandApplication * (14.0 / 17.0)))) / volumeOfManure;
        }

        /// <summary>
        /// Equation 6.2.4-1
        /// </summary>
        /// <param name="numberOfCattle">Number of cattle</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <param name="energyConversionFactor">Electricity conversion factor (kg CO2 kWh^-1)</param>
        /// <returns>Total CO2 emissions from operations (kg CO2 year^-1)</returns>
        public double CalculateTotalCarbonDioxideEmissionsFromHousedBeefOperations(
            double numberOfCattle,
            double numberOfDaysInMonth,
            double energyConversionFactor, 
            double housedBeefConversionFactor)
        {
            return numberOfCattle * (housedBeefConversionFactor / CoreConstants.DaysInYear) * energyConversionFactor * numberOfDaysInMonth;
        }

        #endregion

        #region Public Methods

        #endregion
    }
}