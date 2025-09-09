#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

#endregion

namespace H.Core.Services.Animals
{
    /// <summary>
    /// </summary>
    public class BeefCattleResultsService : BeefAndDairyResultsServiceBase, IBeefCattleResultsService
    {
        #region Constructors

        public BeefCattleResultsService()
        {
            _animalComponentCategory = ComponentCategory.BeefProduction;
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
                groupEmissionsByDay = CalculateDailyEmissionsForCalves(
                    managementPeriod,
                    dateTime,
                    animalComponentBase,
                    previousDaysEmissions,
                    animalGroup,
                    farm);
            else
                groupEmissionsByDay = CalculateDailyEmissionsForGroup(
                    animalComponentBase,
                    managementPeriod,
                    dateTime,
                    previousDaysEmissions,
                    animalGroup,
                    farm
                );

            return groupEmissionsByDay;
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

            InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            var temperature = farm.ClimateData.GetMeanTemperatureForDay(dateTime);

            /*
             * Enteric methane (CH4)
             */

            if (managementPeriod.PeriodDailyGain > 0)
                dailyEmissions.AverageDailyGain = managementPeriod.PeriodDailyGain;
            else
                dailyEmissions.AverageDailyGain = CalculateAverageDailyWeightGain(
                    managementPeriod.StartWeight,
                    managementPeriod.EndWeight,
                    managementPeriod.Duration.TotalDays);

            dailyEmissions.AnimalWeight = GetCurrentAnimalWeight(
                managementPeriod.StartWeight,
                dailyEmissions.AverageDailyGain,
                managementPeriod.Start,
                dailyEmissions.DateTime);

            var nemf = 0d;
            if (managementPeriod.SelectedDiet.DietaryNetEnergyConcentration == 0)
                // Default/system diets will have a predefined (lookup table) NEmf value
                nemf = managementPeriod.SelectedDiet.CalculateNemf();
            else
                // Custom diets will not have a predefined value and must therefore be calculated
                nemf = managementPeriod.SelectedDiet.DietaryNetEnergyConcentration;

            dailyEmissions.DryMatterIntake = CalculateDryMatterIntakeForCalves(
                nemf,
                dailyEmissions.AnimalWeight, managementPeriod.AnimalsAreMilkFedOnly);

            dailyEmissions.DryMatterIntakeForGroup = CalculateDryMatterIntakeForAnimalGroup(
                dailyEmissions.DryMatterIntake,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = CalculateDailyCarbonUptakeForGroup(
                dailyEmissions.DryMatterIntakeForGroup);

            dailyEmissions.GrossEnergyIntake = CalculateGrossEnergyIntakeForCalves(
                dailyEmissions.DryMatterIntake);

            dailyEmissions.AdditiveReductionFactor = AdditiveReductionFactorsProvider.GetAdditiveReductionFactor(
                managementPeriod.DietAdditive,
                managementPeriod.Duration.TotalDays,
                managementPeriod.SelectedDiet.Fat);

            dailyEmissions.EntericMethaneEmissionRate = CalculateEntericMethaneEmissionRate(
                dailyEmissions.GrossEnergyIntake,
                managementPeriod.SelectedDiet.MethaneConversionFactor,
                dailyEmissions.AdditiveReductionFactor);

            if (managementPeriod.AnimalsAreMilkFedOnly)
                dailyEmissions.EntericMethaneEmission = 0;
            else
                dailyEmissions.EntericMethaneEmission = CalculateEntericMethaneEmissions(
                    dailyEmissions.EntericMethaneEmissionRate,
                    managementPeriod.NumberOfAnimals);

            /*
             * Manure carbon (C) and methane (CH4)
             */

            dailyEmissions.FecalCarbonExcretionRate = CalculateFecalCarbonExcretionRate(
                dailyEmissions.GrossEnergyIntake);

            dailyEmissions.FecalCarbonExcretion = CalculateAmountOfFecalCarbonExcreted(
                dailyEmissions.FecalCarbonExcretionRate,
                managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-6
            dailyEmissions.CarbonAddedFromBeddingMaterial = 0;

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = CalculateAmountOfCarbonFromManureAndBedding(
                dailyEmissions.FecalCarbonExcretion,
                dailyEmissions.CarbonAddedFromBeddingMaterial);

            if (managementPeriod.AnimalsAreMilkFedOnly)
                dailyEmissions.VolatileSolids = 7.6 / 1000.0 * dailyEmissions.AnimalWeight;
            else
                // Equation 4.1.2-1
                dailyEmissions.VolatileSolids = CalculateVolatileSolids(
                    dailyEmissions.GrossEnergyIntake,
                    managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                    managementPeriod.SelectedDiet.Ash,
                    managementPeriod.SelectedDiet.Forage);

            // Equation 4.1.2-4
            dailyEmissions.ManureMethaneEmissionRate = CalculateManureMethaneEmissionRate(
                dailyEmissions.VolatileSolids,
                managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                managementPeriod.ManureDetails.MethaneConversionFactor);

            // Equation 4.1.2-5
            dailyEmissions.ManureMethaneEmission = CalculateManureMethane(
                dailyEmissions.ManureMethaneEmissionRate,
                managementPeriod.NumberOfAnimals);

            CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            dailyEmissions.ProteinIntakeFromSolidFood = CalculateCalfProteinIntakeFromSolidFood(
                dailyEmissions.DryMatterIntake,
                managementPeriod.SelectedDiet.CrudeProteinContent);

            var managementPeriodOfCows = cowCalfComponent.GetAssociatedManagementPeriodOfParentGroup(
                youngAnimalGroup: animalGroup,
                parentGroupType: AnimalType.BeefCowLactating,
                dateTime: dateTime);

            if (managementPeriodOfCows != null)
                dailyEmissions.ProteinIntakeFromMilk = CalculateCalfProteinIntakeFromMilk(
                    managementPeriodOfCows.MilkProduction,
                    managementPeriodOfCows.MilkProteinContent);

            dailyEmissions.ProteinIntake = CalculateCalfProteinIntake(
                dailyEmissions.ProteinIntakeFromMilk,
                dailyEmissions.ProteinIntakeFromSolidFood,
                managementPeriod.AnimalsAreMilkFedOnly);

            dailyEmissions.ProteinRetainedFromSolidFood = managementPeriod.AnimalsAreMilkFedOnly
                ? 0
                : CalculateCalfProteinRetainedFromSolidFeed(
                    dailyEmissions.ProteinIntakeFromSolidFood);

            dailyEmissions.ProteinRetainedFromMilk = CalculateCalfProteinRetainedFromMilk(
                dailyEmissions.ProteinIntakeFromMilk);

            dailyEmissions.ProteinRetained = CalculateCalfProteinRetained(
                dailyEmissions.ProteinRetainedFromMilk,
                dailyEmissions.ProteinRetainedFromSolidFood);

            dailyEmissions.NitrogenExcretionRate = CalculateCalfNitrogenExcretionRate(
                dailyEmissions.ProteinIntake,
                dailyEmissions.ProteinRetained);

            // Used in volatilization calculation
            dailyEmissions.AmountOfNitrogenExcreted = CalculateAmountOfNitrogenExcreted(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-31
            dailyEmissions.AmountOfNitrogenAddedFromBedding = 0;

            dailyEmissions.ManureDirectN2ONEmissionRate = CalculateManureDirectNitrogenEmissionRate(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            dailyEmissions.ManureDirectN2ONEmission = CalculateManureDirectNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmissionRate,
                managementPeriod.NumberOfAnimals);

            /*
             * Indirect manure N2O
             */

            CalculateIndirectManureNitrousOxide(
                dailyEmissions,
                managementPeriod,
                animalGroup,
                dateTime,
                previousDaysEmissions,
                temperature,
                farm);

            dailyEmissions.ManureIndirectN2ONEmission = CalculateManureIndirectNitrogenEmission(
                dailyEmissions.ManureVolatilizationN2ONEmission,
                dailyEmissions.ManureN2ONLeachingEmission);

            dailyEmissions.ManureN2ONEmission = CalculateManureNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmission,
                dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay =
                CalculateAccumulatedTanAvailableForLandApplication(
                    dailyEmissions.AccumulatedTanInStorageOnDay);

            CalculateOrganicNitrogen(dailyEmissions, managementPeriod, previousDaysEmissions);

            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay =
                CalculateTotalAvailableManureNitrogenInStoredManure(
                    dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay,
                    dailyEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.ManureCarbonNitrogenRatio = CalculateManureCarbonToNitrogenRatio(
                dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay =
                dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay + dailyEmissions.OrganicNitrogenCreatedOnDay;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication =
                CalculateTotalVolumeOfManureAvailableForLandApplication(
                    dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay,
                    managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null
                                                   ? 0
                                                   : previousDaysEmissions.AccumulatedVolume);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
                managementPeriod,
                temperature,
                dailyEmissions);

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

            InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            /*
             * Enteric methane (CH4)
             */

            if (managementPeriod.PeriodDailyGain > 0)
                dailyEmissions.AverageDailyGain = managementPeriod.PeriodDailyGain;
            else
                dailyEmissions.AverageDailyGain = CalculateAverageDailyWeightGain(
                    managementPeriod.StartWeight,
                    managementPeriod.EndWeight,
                    managementPeriod.Duration.TotalDays);

            dailyEmissions.AnimalWeight = GetCurrentAnimalWeight(
                managementPeriod.StartWeight,
                dailyEmissions.AverageDailyGain,
                managementPeriod.Start,
                dailyEmissions.DateTime);

            dailyEmissions.AdjustedMaintenanceCoefficient = CalculateTemperatureAdjustedMaintenanceCoefficient(
                managementPeriod.HousingDetails.BaselineMaintenanceCoefficient,
                dailyEmissions.Temperature);

            dailyEmissions.NetEnergyForMaintenance = CalculateNetEnergyForMaintenance(
                dailyEmissions.AdjustedMaintenanceCoefficient,
                dailyEmissions.AnimalWeight);

            dailyEmissions.NetEnergyForActivity = CalculateNetEnergyForActivity(
                managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation,
                dailyEmissions.NetEnergyForMaintenance);

            managementPeriod.NumberOfYoungAnimals = cowCalfComponent.GetTotalNumberOfYoungAnimalsByDate(
                dateTime,
                animalGroup,
                AnimalType.BeefCalf);

            var isLactatingAnimalGroup = managementPeriod.NumberOfYoungAnimals > 0;
            if (isLactatingAnimalGroup)
                // Beef cattle lactating cows stop lactating if there are no associated young animals - this differs from lactating dairy cows who are always lactating regardless of the fact
                // that there may or may not be any associated calf groups.
                dailyEmissions.NetEnergyForLactation = CalculateNetEnergyForLactation(
                    managementPeriod.MilkProduction,
                    managementPeriod.MilkFatContent,
                    managementPeriod.NumberOfYoungAnimals,
                    managementPeriod.NumberOfAnimals);

            if (animalGroup.GroupType.IsPregnantType())
                /*
                 * Cows are always pregnant during the entire year
                 */
                dailyEmissions.NetEnergyForPregnancy = CalculateNetEnergyForPregnancy(
                    dailyEmissions.NetEnergyForMaintenance);

            /*
             * Equation 3.1.1-7 at beginning of this method
             */

            dailyEmissions.NetEnergyForGain = CalculateNetEnergyForGain(
                dailyEmissions.AnimalWeight,
                managementPeriod.GainCoefficient,
                dailyEmissions.AverageDailyGain,
                managementPeriod.EndWeight);

            dailyEmissions.RatioOfEnergyAvailableForMaintenance =
                CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy(
                    managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            dailyEmissions.RatioOfEnergyAvailableForGain =
                CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed(
                    managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            dailyEmissions.GrossEnergyIntake = CalculateGrossEnergyIntake(
                dailyEmissions.NetEnergyForMaintenance,
                dailyEmissions.NetEnergyForActivity,
                dailyEmissions.NetEnergyForLactation,
                dailyEmissions.NetEnergyForPregnancy,
                dailyEmissions.NetEnergyForGain,
                dailyEmissions.RatioOfEnergyAvailableForMaintenance,
                dailyEmissions.RatioOfEnergyAvailableForGain,
                managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            dailyEmissions.AdditiveReductionFactor = AdditiveReductionFactorsProvider.GetAdditiveReductionFactor(
                managementPeriod.DietAdditive,
                managementPeriod.Duration.TotalDays,
                managementPeriod.SelectedDiet.Fat);

            dailyEmissions.EntericMethaneEmissionRate = CalculateEntericMethaneEmissionRate(
                dailyEmissions.GrossEnergyIntake,
                managementPeriod.SelectedDiet.MethaneConversionFactor,
                dailyEmissions.AdditiveReductionFactor);

            dailyEmissions.EntericMethaneEmission = CalculateEntericMethaneEmissions(
                dailyEmissions.EntericMethaneEmissionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.DryMatterIntake = CalculateDryMatterIntake(
                dailyEmissions.GrossEnergyIntake);

            dailyEmissions.DryMatterIntakeMax =
                CalculateDryMatterMax(managementPeriod.EndWeight, managementPeriod.AnimalType);

            if (IsOverDmiMax(dailyEmissions))
            {
                dailyEmissions.DryMatterIntake = dailyEmissions.DryMatterIntakeMax;

                dailyEmissions.OptimumTdn = CalculateRequiredTdnSoThatMaxDmiIsNotExceeded(
                    dailyEmissions.NetEnergyForMaintenance,
                    dailyEmissions.NetEnergyForActivity,
                    dailyEmissions.NetEnergyForLactation,
                    dailyEmissions.NetEnergyForPregnancy,
                    dailyEmissions.NetEnergyForGain,
                    dailyEmissions.RatioOfEnergyAvailableForMaintenance,
                    dailyEmissions.RatioOfEnergyAvailableForGain,
                    managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                    dailyEmissions.DryMatterIntakeMax);
            }

            dailyEmissions.DryMatterIntakeForGroup = CalculateDryMatterIntakeForAnimalGroup(
                dailyEmissions.DryMatterIntake,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = CalculateDailyCarbonUptakeForGroup(
                dailyEmissions.DryMatterIntakeForGroup);

            #region Additional enteric methane (CH4) calculations

            dailyEmissions.CrudeFatIntake = dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.FatContent +
                                            dailyEmissions.DryMatterIntake *
                                            managementPeriod.DietAdditive.GetFatFromAdditive();
            dailyEmissions.CrudeProteinIntake =
                dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.CrudeProteinContent;
            dailyEmissions.NeutralDetergentFiberIntake =
                dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.NdfContent;
            dailyEmissions.StarchIntake = dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.StarchContent;

            if (managementPeriod.SelectedDiet.Forage >= 25)
            {
                // Equation 3.1.1-14
                // Equation 3.1.1-18
                dailyEmissions.EntericMethaneEscobarEtAlAlOrBeef = CalculateEntericMethaneUsingEscobarAlOrEtAl(
                    dailyEmissions.AnimalWeight,
                    managementPeriod.SelectedDiet.Forage,
                    dailyEmissions.CrudeFatIntake,
                    dailyEmissions.GrossEnergyIntake,
                    managementPeriod.NumberOfAnimals);

                // Equation 3.1.1-15
                // Equation 3.1.1-18
                dailyEmissions.EntericMethaneLingenEtAlBeef = CalculateEntericMethaneEmissionUsingLingenEtAl(
                    dailyEmissions.DryMatterIntake,
                    managementPeriod.SelectedDiet.Forage,
                    dailyEmissions.AnimalWeight,
                    managementPeriod.NumberOfAnimals);
            }
            else
            {
                // Equation 3.1.1-16
                // Equation 3.1.1-18
                dailyEmissions.EntericMethaneEscobarEtAlLfMcBeef = CalculateEntericMethaneEmissionUsingEscobarLfMc(
                    dailyEmissions.AnimalWeight,
                    dailyEmissions.DryMatterIntake,
                    dailyEmissions.CrudeFatIntake,
                    dailyEmissions.CrudeProteinIntake,
                    dailyEmissions.NeutralDetergentFiberIntake,
                    dailyEmissions.StarchIntake,
                    managementPeriod.NumberOfAnimals);

                // Equation 3.1.1-17
                // Equation 3.1.1-18
                dailyEmissions.EntericMethaneEllisEtAlBeef = CalculateEntericMethaneEmissionUsingEllisEtAl(
                    dailyEmissions.DryMatterIntake,
                    dailyEmissions.StarchIntake,
                    dailyEmissions.NeutralDetergentFiberIntake,
                    managementPeriod.NumberOfAnimals);
            }

            #endregion

            /*
             * Manure carbon (C) and methane (CH4)
             */

            dailyEmissions.FecalCarbonExcretionRate = CalculateFecalCarbonExcretionRate(
                dailyEmissions.GrossEnergyIntake);

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

            dailyEmissions.VolatileSolids = CalculateVolatileSolids(
                dailyEmissions.GrossEnergyIntake,
                managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                managementPeriod.SelectedDiet.Ash,
                managementPeriod.SelectedDiet.Forage);

            dailyEmissions.ManureMethaneEmissionRate = CalculateManureMethaneEmissionRate(
                dailyEmissions.VolatileSolids,
                managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                managementPeriod.ManureDetails.MethaneConversionFactor);

            dailyEmissions.ManureMethaneEmission = CalculateManureMethane(
                dailyEmissions.ManureMethaneEmissionRate,
                managementPeriod.NumberOfAnimals);

            CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            CalculateDirectN2OFromBeefAndDairy(
                dailyEmissions,
                managementPeriod,
                animalGroup,
                isLactatingAnimalGroup,
                managementPeriod.NumberOfYoungAnimals);

            /*
             * Indirect manure N2O
             */

            CalculateIndirectManureNitrousOxide(
                dailyEmissions,
                managementPeriod,
                animalGroup,
                dateTime,
                previousDaysEmissions,
                dailyEmissions.Temperature,
                farm);

            dailyEmissions.ManureIndirectN2ONEmission = CalculateManureIndirectNitrogenEmission(
                dailyEmissions.ManureVolatilizationN2ONEmission,
                dailyEmissions.ManureN2ONLeachingEmission);

            dailyEmissions.ManureN2ONEmission = CalculateManureNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmission,
                dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay =
                CalculateAccumulatedTanAvailableForLandApplication(
                    dailyEmissions.AccumulatedTanInStorageOnDay);

            CalculateOrganicNitrogen(dailyEmissions, managementPeriod, previousDaysEmissions);

            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay =
                CalculateTotalAvailableManureNitrogenInStoredManure(
                    dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay,
                    dailyEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.ManureCarbonNitrogenRatio = CalculateManureCarbonToNitrogenRatio(
                dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay =
                dailyEmissions.AdjustedAmountOfTanInStoredManureOnDay + dailyEmissions.OrganicNitrogenCreatedOnDay;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication =
                CalculateTotalVolumeOfManureAvailableForLandApplication(
                    dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay,
                    managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null
                                                   ? 0
                                                   : previousDaysEmissions.AccumulatedVolume);

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
                managementPeriod,
                dailyEmissions.Temperature,
                dailyEmissions);

            return dailyEmissions;
        }

        protected override void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm, AnimalComponentBase animalComponentBase)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.HousingDetails.HousingType
                    .IsElectricalConsumingHousingType() == false) return;

            var energyConversionFactor =
                _energyConversionDefaultsProvider.GetElectricityConversionValue(
                    groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide =
                CalculateTotalCarbonDioxideEmissionsFromHousedBeefOperations(
                    groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                    groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth,
                    energyConversionFactor);
        }

        protected override void CalculateEstimatesOfProduction(
            GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm)
        {
            groupEmissionsByMonth.MonthlyBeefProduced = CalculateBeefProducedPerMonth(
                groupEmissionsByMonth.AverageDailyGain,
                groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth);
        }

        #endregion

        #region Equations

        /// <summary>
        ///     Equation 3.1.1-5
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
            if (Math.Abs(numberOfAnimals) < double.Epsilon) return 0;

            var v = 1.47 + 0.40 * fatContent;
            var netEnergyForLactation = milkProduction * v * (numberOfYoungAnimals / numberOfAnimals);
            return netEnergyForLactation;
        }

        /// <summary>
        ///     Equation 3.1.1-14
        ///     Equation 3.1.1-18
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
            var emissionRate =
                (-35 + 0.08 * bodyWeight + 1.2 * forageInDiet - 69.8 * Math.Pow(crudeFatIntake, 3) +
                 3.14 * (grossEnergyIntake / 4.184)) * (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        ///     Equation 3.1.1-15
        ///     Equation 3.1.1-18
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
            var emissionRate = (-6.41 + 11.3 * dryMatterIntake + 0.557 * dietaryForage + 0.0996 * bodyWeight) *
                               (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        ///     Equation 3.1.1-16
        ///     Equation 3.1.1-18
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
        ///     Equation 3.1.1-17
        ///     Equation 3.1.1-18
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
            var emissionRate = (48.2 + 14.4 * dryMatterIntake - 20.5 * (dietaryStarch / dietaryNeutralDetergentFiber)) *
                               (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        ///     Equation 11.4.2-1
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
        ///     Equation 3.1.10-1
        /// </summary>
        /// <param name="totalCarbonInStoredManure">Total amount of carbon in stored manure (kg C year-1)</param>
        /// <param name="totalVolumeOfManureProduced">
        ///     Total volume of manure produced from different housing and storage systems
        ///     (1000 kg wet weight for solid manure and 1000 litres for liquid manure)
        /// </param>
        /// <returns>Fraction of carbon in field applied manaure (kg C/1000 kg wet weight)</returns>
        public double CalculateFractionOfCarbonInFieldAppliedManure(
            double totalCarbonInStoredManure,
            double totalVolumeOfManureProduced)
        {
            if (Math.Abs(totalVolumeOfManureProduced) < double.Epsilon) return 0;

            return totalCarbonInStoredManure / totalVolumeOfManureProduced;
        }

        /// <summary>
        ///     Equation 3.1.10-2
        /// </summary>
        /// <param name="totalAvailableNitrogenInStoredManure">Total available manure nitrogen in stored manure (kg N year-1)</param>
        /// <param name="totalTotalAmmoniaEmissionsFromLandApplication">
        ///     Total ammonia emissions from land applied manure sourced
        ///     from different housing and storage systems (kg NH3 year-1)
        /// </param>
        /// <param name="totalVolumeOfManure">
        ///     Total volume of manure produced from different housing and storage systems (1000 kg
        ///     wet weight for solid manure and 1000 litres for liquid manure)
        /// </param>
        /// <returns>Fraction of nitrogen in field applied manaure (kg N/1000 kg wet weight)</returns>
        public double CalculateFractionOfNitrogenAppliedToSoil(
            double totalAvailableNitrogenInStoredManure,
            double totalTotalAmmoniaEmissionsFromLandApplication,
            double totalVolumeOfManure)
        {
            if (Math.Abs(totalVolumeOfManure) < double.Epsilon) return 0;

            var numerator = totalAvailableNitrogenInStoredManure -
                            totalTotalAmmoniaEmissionsFromLandApplication * (14.0 / 17.0);

            return numerator / totalVolumeOfManure;
        }

        /// <summary>
        ///     Equation 3.1.11-6
        /// </summary>
        /// <param name="ammoniaEmissionsFromAllPastures">Ammonia emission from beef cattle on pasture (kg NH3)</param>
        /// <returns>Total ammonia emissions from grazing animal on pasture (kg NH3 year-1)</returns>
        public double CalculateTotalAmmoniaEmissionsFromPasture(List<double> ammoniaEmissionsFromAllPastures)
        {
            return ammoniaEmissionsFromAllPastures.Sum();
        }

        /// <summary>
        ///     Equation 3.1.11-7
        /// </summary>
        /// <param name="carbonFromAnimalsOnAllPasture">Amount of C excreted (kg C)</param>
        /// <returns>Total amount of carbon from grazing animals on pasture (kg C year-1)</returns>
        public double CalculateTotalCarbonFromPastures(List<double> carbonFromAnimalsOnAllPasture)
        {
            return carbonFromAnimalsOnAllPasture.Sum();
        }

        /// <summary>
        ///     Equation 3.1.11-8
        /// </summary>
        /// <param name="nitrogenFromAnimalsOnAllPasture">Amount of N excreted (kg C)</param>
        /// <returns>Total amount of carbon from grazing animals on pasture (kg C year-1)</returns>
        public double CalculateTotalNitrogenFromPastures(List<double> nitrogenFromAnimalsOnAllPasture)
        {
            return nitrogenFromAnimalsOnAllPasture.Sum();
        }

        /// <summary>
        ///     Equation 3.1.12-1
        /// </summary>
        /// <param name="carbonStoredInManure">Total amount of carbon in stored manure (kg C year-1)</param>
        /// <param name="totalVolumeOfManure">
        ///     Total volume of manure produced from different housing and storage systems (1000 kg
        ///     wet weight for solid manure and 1000 litres for liquid manure)
        /// </param>
        /// <returns>Fraction of carbon in field applied manure (kg C/1000 kg wet weight)</returns>
        public double CalculateFractionOfCarbonInManureFromGrazingAnimals(
            double carbonStoredInManure,
            double totalVolumeOfManure)
        {
            if (Math.Abs(totalVolumeOfManure) < double.Epsilon) return 0;

            return carbonStoredInManure / totalVolumeOfManure;
        }

        /// <summary>
        ///     Equation 3.1.12-2
        /// </summary>
        /// <param name="availableNitrogenInManure">Total available manure nitrogen in stored manure (kg N year-1) </param>
        /// <param name="totalAmmoniaFromLandApplication">
        ///     Total ammonia emissions from land applied manure sourced from different
        ///     housing and storage systems (kg NH3 year-1)
        /// </param>
        /// <param name="volumeOfManure">
        ///     Total volume of manure produced from different housing and storage systems (1000 kg wet
        ///     weight for solid manure and 1000 litres for liquid manure)
        /// </param>
        /// <returns>Fraction of nitrogen added to pasture soil by grazing animals (kg N/1000 kg wet weight)</returns>
        public double CalculateFractionOfNitrogenAppliedByGrazingAnimals(
            double availableNitrogenInManure,
            double totalAmmoniaFromLandApplication,
            double volumeOfManure)
        {
            if (Math.Abs(volumeOfManure) < double.Epsilon) return 0;

            return (availableNitrogenInManure - totalAmmoniaFromLandApplication * (14.0 / 17.0)) / volumeOfManure;
        }

        /// <summary>
        ///     Equation 6.2.4-1
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

            return numberOfCattle * (housedBeefConversion / CoreConstants.DaysInYear) * energyConversionFactor *
                   numberOfDaysInMonth;
        }

        #endregion
    }
}