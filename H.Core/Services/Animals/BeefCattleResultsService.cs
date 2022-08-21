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

#endregion

namespace H.Core.Services.Animals
{
    /// <summary>
    /// </summary>
    public class BeefCattleResultsService : AnimalResultsServiceBase, IBeefCattleResultsService
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

            // Equation 3.1.2-1
            dailyEmissions.AnimalWeight = base.GetCurrentAnimalWeight(
                startWeight: managementPeriod.StartWeight,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                startDate: managementPeriod.Start,
                currentDate: dailyEmissions.DateTime);

            // Equation 3.1.2-2
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
                weight: dailyEmissions.AnimalWeight);

            dailyEmissions.DryMatterIntakeForGroup = base.CalculateDryMatterIntakeForAnimalGroup(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = base.CaclulateDailyCarbonUptakeForGroup(
                totalDailyDryMatterIntakeForGroup: dailyEmissions.DryMatterIntakeForGroup);

            // Equation 3.1.2-3
            dailyEmissions.GrossEnergyIntake = CalculateGrossEnergyIntakeForCalves(
                dryMatterIntake: dailyEmissions.DryMatterIntake);

            dailyEmissions.AdditiveReductionFactor = AdditiveReductionFactorsProvider.GetAdditiveReductionFactor(
                additiveType: managementPeriod.DietAdditive,
                numberOfDays: managementPeriod.Duration.TotalDays,
                fat: managementPeriod.SelectedDiet.Fat);

            // Equation 3.1.1-12
            dailyEmissions.EntericMethaneEmissionRate = base.CalculateEntericMethaneEmissionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                methaneConversionFactor: managementPeriod.SelectedDiet.MethaneConversionFactor,
                additiveReductionFactor: dailyEmissions.AdditiveReductionFactor);

            // Equation 3.1.1-13
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

            // BUG: When animals are milk fed the PI is set to 0, but this results in NaN calculations for indirect emissions. Need to fix this
            // Equation 4.2.1-9
            //dailyEmissions.ProteinIntakeFromSolidFood = managementPeriod.AnimalsAreMilkFedOnly ? 0 : this.CalculateCalfProteinIntakeFromSolidFood(
            //    dryMatterIntake: dailyEmissions.DryMatterIntake,
            //    crudeProteinContent: managementPeriod.SelectedDiet.CrudeProteinContent);

            dailyEmissions.ProteinIntakeFromSolidFood = this.CalculateCalfProteinIntakeFromSolidFood(
                dryMatterIntake: dailyEmissions.DryMatterIntake,
                crudeProteinContent: managementPeriod.SelectedDiet.CrudeProteinContent);

            if (managementPeriod.AnimalsAreMilkFedOnly)
            {
                var managementPeriodOfCows = cowCalfComponent.GetAssociatedManagementPeriodOfParentGroup(
                    youngAnimalGroup: animalGroup,
                    parentGroupType: AnimalType.BeefCowLactating,
                    dateTime: dateTime);

                if (managementPeriodOfCows != null)
                {
                    // Equation 4.2.1-10
                    dailyEmissions.ProteinIntakeFromMilk = base.CalculateCalfProteinIntakeFromMilk(
                        milkProduction: managementPeriodOfCows.MilkProduction,
                        proteinContentOfMilk: managementPeriodOfCows.MilkProteinContent);
                }
            }

            // Equation 4.2.1-11
            dailyEmissions.ProteinIntake = base.CalculateCalfProteinIntake(
                calfProteinIntakeFromMilk: dailyEmissions.ProteinIntakeFromMilk,
                calfProteinIntakeFromSolidFood: dailyEmissions.ProteinIntakeFromSolidFood);

            // Equation 4.2.1-12
            dailyEmissions.ProteinRetainedFromSolidFood = managementPeriod.AnimalsAreMilkFedOnly ? 0 : base.CalculateCalfProteinRetainedFromSolidFeed(
                calfProteinIntakeFromSolidFood: dailyEmissions.ProteinIntakeFromSolidFood);

            // Equation 4.2.1-13
            dailyEmissions.ProteinRetainedFromMilk = CalculateCalfProteinRetainedFromMilk(
                calfProteinIntakeFromMilk: dailyEmissions.ProteinIntakeFromMilk);

            // Equation 4.2.1-14
            dailyEmissions.ProteinRetained = CalculateCalfProteinRetained(
                calfProteinRetainedFromMilk: dailyEmissions.ProteinRetainedFromMilk,
                calfProteinRetainedFromSolidFeed: dailyEmissions.ProteinRetainedFromSolidFood);

            // Equation 4.2.1-15
            dailyEmissions.NitrogenExcretionRate = CalculateCalfNitrogenExcretionRate(
                calfProteinIntake: dailyEmissions.ProteinIntake,
                calfProteinRetained: dailyEmissions.ProteinRetained);

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

            // No equation for this when considering beef cattle as it is a lookup table in algorithm document
            dailyEmissions.FractionOfNitrogenExcretedInUrine = base.GetFractionOfNitrogenExcretedInUrine(
                crudeProteinInDiet: managementPeriod.SelectedDiet.CrudeProteinContent);

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

            if (managementPeriod.HousingDetails.HousingType.IsIndoorHousing())
            {
                // Equation 4.3.1-13
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustmentAddTwo(
                    averageMonthlyTemperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));
            }
            else
            {
                // Equation 4.3.1-8
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustment(
                    averageMonthlyTemperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));
            }

            var ammoniaEmissionFactorForHousingType = _beefDairyDefaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                housingType: managementPeriod.HousingDetails.HousingType);

            // Equation 4.3.1-9
            dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing = CalculateAdjustedEmissionFactorHousing(
                emissionFactor: ammoniaEmissionFactorForHousingType,
                temperatureAdjustment: dailyEmissions.AmbientAirTemperatureAdjustmentForHousing);

            // Equation 4.3.1-10
            dailyEmissions.AmmoniaEmissionRateFromHousing = CalculateAmmoniaEmissionRateFromHousing(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                adjustedEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing);

            // Equation 4.3.1-11
            dailyEmissions.AmmoniaConcentrationInHousing = CalculateAmmoniaConcentrationInHousing(
                emissionRate: dailyEmissions.AmmoniaEmissionRateFromHousing,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.1-12
            dailyEmissions.AmmoniaEmissionsFromHousingSystem = CalculateTotalAmmoniaEmissionsFromHousing(
                ammoniaConcentrationInHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            // Equation 4.3.2-1
            dailyEmissions.TanEnteringStorageSystem = CalculateTanStorage(
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

            // Equation 4.3.2-3
            dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = base.CalculateAmbientTemperatureAdjustmentForStoredManure(
                averageMonthlyTemperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));

            // Equation 4.3.2-6
            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                ambientTemperatureAdjustmentStorage: dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                ammoniaEmissionFactorStorage: managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage);

            // Equation 4.3.2-7
            dailyEmissions.AmmoniaLostFromStorage = CalculateAmmoniaLossFromStoredManure(
                tanInStoredManure: dailyEmissions.AdjustedAmountOfTanInStoredManure,
                ammoniaEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            // Equation 4.3.2-8
            dailyEmissions.AmmoniaEmissionsFromStorageSystem = CalculateAmmoniaEmissionsFromStoredManure(
                ammoniaNitrogenLossFromStoredManure: dailyEmissions.AmmoniaLostFromStorage);

            if (managementPeriod.ManureDetails.UseCustomVolatilizationFraction)
            {
                dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;
            }
            else
            {
                // Equation 4.3.4-1
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

            // Equation 4.3.3-4
            dailyEmissions.ManureVolatilizationN2ONEmission = this.CalculateManureVolatilizationNitrogenEmission(
                volatilizationRate: dailyEmissions.ManureVolatilizationRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.4-1
            dailyEmissions.ManureNitrogenLeachingRate = this.CalculateManureLeachingNitrogenEmissionRate(
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

            var manureCompositionData = farm.GetManureCompositionData(
                manureStateType: managementPeriod.ManureDetails.StateType,
                animalType: animalGroup.GroupType);

            // Equation 4.5.3-2
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = base.CalculateTotalVolumeOfManureAvailableForLandApplication(
                totalNitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                nitrogenFractionOfManure: manureCompositionData.NitrogenFraction);

            var temperature = farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month);

            // Equation 4.6.1-4
            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = base.CalculateTotalAmmoniaEmissionsFromLandAppliedManure(
                farm: farm,
                dateTime: dateTime,
                dailyEmissions: dailyEmissions,
                animalType: animalGroup.GroupType,
                temperature: temperature,
                managementPeriod: managementPeriod);

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            base.GetEmissionsFromBeefAndDairyGrazingAnimals(
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

            var temperature = farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month);
            if (temperature > 20 || managementPeriod.HousingDetails.HousingType == HousingType.HousedInBarn)
            {
                temperature = 20;
            }

            dailyEmissions.Temperature = temperature;

            // Equation 3.1.1-2
            dailyEmissions.AdjustedMaintenanceCoefficient = base.CalculateTemperatureAdjustedMaintenanceCoefficient(
                baselineMaintenanceCoefficient: managementPeriod.HousingDetails.BaselineMaintenanceCoefficient,
                averageMonthlyTemperature: temperature);

            // Equation 3.1.1-3
            dailyEmissions.NetEnergyForMaintenance = base.CalculateNetEnergyForMaintenance(
                maintenanceCoefficient: dailyEmissions.AdjustedMaintenanceCoefficient,
                weight: dailyEmissions.AnimalWeight);

            // Equation 3.1.1-4
            dailyEmissions.NetEnergyForActivity = base.CalculateNetEnergyForActivity(
                feedingActivityCoefficient: managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation,
                netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);

            var totalNumberOfYoungAnimalsOnDate = cowCalfComponent.GetTotalNumberOfYoungAnimalsByDate(
                dateTime: dateTime,
                parentGroup: animalGroup,
                childGroupType: AnimalType.BeefCalf);

            var isLactatingAnimalGroup = totalNumberOfYoungAnimalsOnDate > 0;
            if (isLactatingAnimalGroup)
            {
                // Equation 3.1.1-5
                dailyEmissions.NetEnergyForLactation = this.CalculateNetEnergyForLactation(
                    milkProduction: managementPeriod.MilkProduction,
                    fatContent: managementPeriod.MilkFatContent,
                    numberOfYoungAnimals: totalNumberOfYoungAnimalsOnDate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);
            }

            if (animalGroup.GroupType.IsPregnantType())
            {
                // Equation 3.1.1-6
                dailyEmissions.NetEnergyForPregnancy = base.CalculateNetEnergyForPregnancy(
                    netEnergyForMaintenance: dailyEmissions.NetEnergyForMaintenance);
            }        

            /*
             * Equation 3.1.1-7 at beginning of this method
             */

            // Equation 3.1.1-8
            dailyEmissions.NetEnergyForGain = base.CalculateNetEnergyForGain(
                weight: dailyEmissions.AnimalWeight,
                gainCoefficient: managementPeriod.GainCoefficient,
                averageDailyGain: dailyEmissions.AverageDailyGain,
                finalWeight: managementPeriod.EndWeight);

            // Equation 3.1.1-9
            dailyEmissions.RatioOfEnergyAvailableForMaintenance = base.CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy(
                totalDigestibleNutrient: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            // Equation 3.1.1-10
            dailyEmissions.RatioOfEnergyAvailableForGain = base.CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed(
                totalDigestibleNutrient: managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            // Equation 3.1.1-11
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

            // Equation 3.1.1-12
            dailyEmissions.EntericMethaneEmissionRate = base.CalculateEntericMethaneEmissionRate(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                methaneConversionFactor: managementPeriod.SelectedDiet.MethaneConversionFactor,
                additiveReductionFactor: dailyEmissions.AdditiveReductionFactor);

            // Equation 3.1.1-13
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

            // No equation for this when considering beef cattle as it is a lookup table in algorithm document
            dailyEmissions.FractionOfNitrogenExcretedInUrine = base.GetFractionOfNitrogenExcretedInUrine(
                crudeProteinInDiet: managementPeriod.SelectedDiet.CrudeProteinContent);

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

            if (managementPeriod.HousingDetails.HousingType.IsIndoorHousing())
            {
                // Equation 4.3.1-13
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustmentAddTwo(
                    averageMonthlyTemperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));
            }
            else
            {
                // Equation 4.3.1-8
                dailyEmissions.AmbientAirTemperatureAdjustmentForHousing = CalculateAmbientTemperatureAdjustment(
                    averageMonthlyTemperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));
            }

            var ammoniaEmissionFactorForHousingType = _beefDairyDefaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                housingType: managementPeriod.HousingDetails.HousingType);

            // Equation 4.3.1-9, 4.3.1-14
            dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing = CalculateAdjustedEmissionFactorHousing(
                emissionFactor: ammoniaEmissionFactorForHousingType,
                temperatureAdjustment: dailyEmissions.AmbientAirTemperatureAdjustmentForHousing);

            // Equation 4.3.1-10, 4.3.1-15
            dailyEmissions.AmmoniaEmissionRateFromHousing = CalculateAmmoniaEmissionRateFromHousing(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                adjustedEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForHousing);

            // Equation 4.3.1-11
            dailyEmissions.AmmoniaConcentrationInHousing = CalculateAmmoniaConcentrationInHousing(
                emissionRate: dailyEmissions.AmmoniaEmissionRateFromHousing,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.1-12
            dailyEmissions.AmmoniaEmissionsFromHousingSystem = CalculateTotalAmmoniaEmissionsFromHousing(
                ammoniaConcentrationInHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            // Equation 4.3.2-1
            dailyEmissions.TanEnteringStorageSystem = CalculateTanStorage(
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

            // Equation 4.3.2-3
            dailyEmissions.AmbientAirTemperatureAdjustmentForStorage = base.CalculateAmbientTemperatureAdjustmentForStoredManure(
                averageMonthlyTemperature: farm.ClimateData.TemperatureData.GetMeanTemperatureForMonth(dateTime.Month));

            // Equation 4.3.2-6
            dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage = CalculateAdjustedAmmoniaEmissionFactorStoredManure(
                ambientTemperatureAdjustmentStorage: dailyEmissions.AmbientAirTemperatureAdjustmentForStorage,
                ammoniaEmissionFactorStorage: managementPeriod.ManureDetails.AmmoniaEmissionFactorForManureStorage);

            // Equation 4.3.2-7
            dailyEmissions.AmmoniaLostFromStorage = CalculateAmmoniaLossFromStoredManure(
                tanInStoredManure: dailyEmissions.AdjustedAmountOfTanInStoredManure,
                ammoniaEmissionFactor: dailyEmissions.AdjustedAmmoniaEmissionFactorForStorage);

            // Equation 4.3.2-8
            dailyEmissions.AmmoniaEmissionsFromStorageSystem = CalculateAmmoniaEmissionsFromStoredManure(
                ammoniaNitrogenLossFromStoredManure: dailyEmissions.AmmoniaLostFromStorage);

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

            // Equation 4.3.3-2
            dailyEmissions.ManureVolatilizationRate = this.CalculateManureVolatilizationEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                beddingNitrogen: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized,
                volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            // Equation 4.3.3-4
            dailyEmissions.ManureVolatilizationN2ONEmission = this.CalculateManureVolatilizationNitrogenEmission(
                volatilizationRate: dailyEmissions.ManureVolatilizationRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.3.4-1
            dailyEmissions.ManureNitrogenLeachingRate = this.CalculateManureLeachingNitrogenEmissionRate(
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

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
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
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.HousingDetails.HousingType.IsElectricalConsumingHousingType() == false)
            {
                return;
            }

            var energyConversionFactor = _energyConversionDefaultsProvider.GetElectricityConversionValue(groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = this.CalculateTotalCarbonDioxideEmissionsFromHousedBeefOperations(
                numberOfCattle: groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                numberOfDaysInMonth: groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth,
                energyConversionFactor: energyConversionFactor);
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
        /// Equation 9.4-3
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
            double energyConversionFactor)
        {
            var housedBeefConversion = 65.7;

            return numberOfCattle * (housedBeefConversion / CoreConstants.DaysInYear) * energyConversionFactor * numberOfDaysInMonth;
        }

        #endregion

        #region Public Methods

        #endregion
    }
}