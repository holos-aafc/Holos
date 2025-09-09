using System;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Animals
{
    public class DairyCattleResultsService : BeefAndDairyResultsServiceBase, IDairyResultsService
    {
        #region Constructors

        public DairyCattleResultsService()
        {
            _animalComponentCategory = ComponentCategory.Dairy;
        }

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
                    farm);

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

            dailyEmissions.DryMatterIntake = CalculateDryMatterIntakeForCalves(0, dailyEmissions.AnimalWeight, true);

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
                managementPeriod.SelectedDiet.Ee);

            // Equation 3.2.2-1
            dailyEmissions.EntericMethaneEmission = 0;

            /*
             * Manure carbon (C) and methane (CH4)
             */

            dailyEmissions.FecalCarbonExcretionRate = CalculateFecalCarbonExcretionRate(
                dailyEmissions.GrossEnergyIntake);

            // Equation 4.1.1-4
            dailyEmissions.FecalCarbonExcretion = CalculateAmountOfFecalCarbonExcreted(
                dailyEmissions.FecalCarbonExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = CalculateRateOfCarbonAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.1.1-6
            dailyEmissions.CarbonAddedFromBeddingMaterial = CalculateAmountOfCarbonAddedFromBeddingMaterial(
                dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = CalculateAmountOfCarbonFromManureAndBedding(
                dailyEmissions.FecalCarbonExcretion,
                dailyEmissions.CarbonAddedFromBeddingMaterial);

            // Equation 4.1.2-1
            dailyEmissions.VolatileSolids = 9.3 / 1000.0 * dailyEmissions.AnimalWeight;

            /*
             * Manure methane calculations differ depending if the manure is stored as a liquid or as a solid
             */

            var temperature = farm.ClimateData.GetMeanTemperatureForDay(dateTime);

            if (managementPeriod.ManureDetails.StateType.IsSolidManure())
            {
                dailyEmissions.ManureMethaneEmissionRate = CalculateManureMethaneEmissionRate(
                    dailyEmissions.VolatileSolids,
                    managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    managementPeriod.ManureDetails.MethaneConversionFactor);

                dailyEmissions.ManureMethaneEmission = CalculateManureMethane(
                    dailyEmissions.ManureMethaneEmissionRate,
                    managementPeriod.NumberOfAnimals);
            }
            else
            {
                CalculateManureMethaneFromLiquidSystems(
                    dailyEmissions,
                    previousDaysEmissions,
                    managementPeriod,
                    temperature,
                    farm);
            }

            CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            dailyEmissions.NitrogenExcretionRate = 0.078;

            dailyEmissions.AmountOfNitrogenExcreted = CalculateAmountOfNitrogenExcreted(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-31
            dailyEmissions.AmountOfNitrogenAddedFromBedding = CalculateRateOfNitrogenAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

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

            GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
                managementPeriod,
                temperature,
                dailyEmissions);

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

            dailyEmissions.NetEnergyForMaintenance = CalculateNetEnergyForMaintenance(
                managementPeriod.HousingDetails.BaselineMaintenanceCoefficient,
                dailyEmissions.AnimalWeight);

            dailyEmissions.NetEnergyForActivity = CalculateNetEnergyForActivity(
                managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation,
                dailyEmissions.NetEnergyForMaintenance);

            managementPeriod.NumberOfYoungAnimals = dairyComponent.GetTotalNumberOfYoungAnimalsByDate(
                dateTime,
                animalGroup,
                AnimalType.DairyCalves);

            if (managementPeriod.AnimalType.IsLactatingType())
                // Lactating dairy cows are always lactating - even if they are separated from the calves. This means the lactation calculations are always used regardless if any
                // associated groups of calves. This differs from beef cattle cows/calves where if the calves are removed then the lactation stops.
                dailyEmissions.NetEnergyForLactation = CalculateNetEnergyForLactation(
                    managementPeriod.MilkProduction,
                    managementPeriod.MilkFatContent);

            if (animalGroup.GroupType.IsPregnantType())
                dailyEmissions.NetEnergyForPregnancy = CalculateNetEnergyForPregnancy(
                    dailyEmissions.NetEnergyForMaintenance);

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
                managementPeriod.SelectedDiet.Ee);

            dailyEmissions.EntericMethaneEmissionRate = CalculateEntericMethaneEmissionRate(
                dailyEmissions.GrossEnergyIntake,
                managementPeriod.SelectedDiet.MethaneConversionFactor,
                dailyEmissions.AdditiveReductionFactor);

            dailyEmissions.EntericMethaneEmission = CalculateEntericMethaneEmissions(
                dailyEmissions.EntericMethaneEmissionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.DryMatterIntake = CalculateDryMatterIntake(
                dailyEmissions.GrossEnergyIntake);

            dailyEmissions.DryMatterIntakeForGroup = CalculateDryMatterIntakeForAnimalGroup(
                dailyEmissions.DryMatterIntake,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.TotalCarbonUptakeForGroup = CalculateDailyCarbonUptakeForGroup(
                dailyEmissions.DryMatterIntakeForGroup);

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

            #region Additional enteric methane (CH4) calculations

            dailyEmissions.NeutralDetergentFiberIntake =
                dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.NdfContent;
            dailyEmissions.AcidDetergentFiberIntake =
                dailyEmissions.DryMatterIntake * managementPeriod.SelectedDiet.AdfContent;

            dailyEmissions.EntericMethaneRaminHuhtanenDairy = CalculateEntericMethaneEmissionsUsingRaminHuhtanenMethod(
                dailyEmissions.DryMatterIntake,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.EntericMethaneMillsEtAlDairy = CalculateEntericMethaneEmissionUsingMillsEtAl(
                dailyEmissions.DryMatterIntake,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.EntericMethaneEllisEtAlDairy = CalculateEntericMethaneEmissionUsingEllisEtAl(
                dailyEmissions.DryMatterIntake,
                dailyEmissions.AcidDetergentFiberIntake,
                dailyEmissions.NeutralDetergentFiberIntake,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.EntericMethaneNuiEtAlDairy = CalculateEntericMethaneEmissionUsingNuiEtAl(
                dailyEmissions.DryMatterIntake,
                managementPeriod.SelectedDiet.Ee,
                managementPeriod.SelectedDiet.Ndf,
                managementPeriod.NumberOfAnimals);

            #endregion

            /*
             * Manure carbon (C) and methane (CH4)
             */

            dailyEmissions.FecalCarbonExcretionRate = CalculateFecalCarbonExcretionRate(
                dailyEmissions.GrossEnergyIntake);

            // Equation 4.1.1-4
            dailyEmissions.FecalCarbonExcretion = CalculateAmountOfFecalCarbonExcreted(
                dailyEmissions.FecalCarbonExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = CalculateRateOfCarbonAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.1.1-6
            dailyEmissions.CarbonAddedFromBeddingMaterial = CalculateAmountOfCarbonAddedFromBeddingMaterial(
                dailyEmissions.RateOfCarbonAddedFromBeddingMaterial,
                managementPeriod.NumberOfAnimals);

            // Equation 4.1.1-7
            dailyEmissions.CarbonFromManureAndBedding = CalculateAmountOfCarbonFromManureAndBedding(
                dailyEmissions.FecalCarbonExcretion,
                dailyEmissions.CarbonAddedFromBeddingMaterial);

            dailyEmissions.VolatileSolids = CalculateVolatileSolids(
                dailyEmissions.GrossEnergyIntake,
                managementPeriod.SelectedDiet.TotalDigestibleNutrient,
                managementPeriod.SelectedDiet.Ash,
                managementPeriod.SelectedDiet.Forage);

            /*
             * Manure methane calculations differ depending if the manure is stored as a liquid or as a solid
             *
             * If user specifies custom a custom methane conversion factor, then skip liquid calculations (even if system is liquid, calculate manure methane using 2-4 and 2-5.)
             */

            if (managementPeriod.ManureDetails.StateType.IsSolidManure() ||
                managementPeriod.ManureDetails.UseCustomMethaneConversionFactor)
            {
                // Equation 4.1.2-4
                dailyEmissions.ManureMethaneEmissionRate = CalculateManureMethaneEmissionRate(
                    dailyEmissions.VolatileSolids,
                    managementPeriod.ManureDetails.MethaneProducingCapacityOfManure,
                    managementPeriod.ManureDetails.MethaneConversionFactor);

                // Equation 4.1.2-5
                dailyEmissions.ManureMethaneEmission = CalculateManureMethane(
                    dailyEmissions.ManureMethaneEmissionRate,
                    managementPeriod.NumberOfAnimals);
            }
            else
            {
                CalculateManureMethaneFromLiquidSystems(
                    dailyEmissions,
                    previousDaysEmissions,
                    managementPeriod,
                    temperature,
                    farm);
            }

            CalculateCarbonInStorage(dailyEmissions, previousDaysEmissions, managementPeriod);

            /*
             * Direct manure N2O
             */

            var isLactatingGroup = animalGroup.GroupType == AnimalType.DairyLactatingCow;
            CalculateDirectN2OFromBeefAndDairy(
                dailyEmissions,
                managementPeriod,
                animalGroup,
                isLactatingGroup,
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

            GetEmissionsFromGrazingBeefPoultryAndDairyAnimals(
                managementPeriod,
                temperature,
                dailyEmissions);

            return dailyEmissions;
        }

        protected override void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm, AnimalComponentBase animalComponentBase)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType !=
                AnimalType.DairyLactatingCow) return;

            var energyConversionFactor =
                _energyConversionDefaultsProvider.GetElectricityConversionValue(
                    groupEmissionsByMonth.MonthsAndDaysData.Year, farm.DefaultSoilData.Province);
            groupEmissionsByMonth.MonthlyEnergyCarbonDioxide = CalculateTotalCarbonDioxideEmissionsFromDairyHousing(
                groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth,
                energyConversionFactor);
        }

        protected override void CalculateEstimatesOfProduction(
            GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType.IsLactatingType())
            {
                groupEmissionsByMonth.MonthlyMilkProduction = CalculateMilkProductionPerMonthFromDairyCattle(
                    groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.MilkProduction,
                    groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                    groupEmissionsByMonth.MonthsAndDaysData.DaysInMonth);

                groupEmissionsByMonth.MonthlyFatAndProteinCorrectedMilkProduction =
                    FatAndProteinCorrectedMilkProductionPerMonth(
                        groupEmissionsByMonth.MonthlyMilkProduction,
                        groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.MilkFatContent,
                        groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.MilkFatContent);
            }
        }

        #endregion

        #region Equations

        /// <summary>
        ///     Equation 3.2.1-4
        /// </summary>
        /// <param name="milkProduction">Milk production (kg head^-1 day^-1)</param>
        /// <param name="fatContent">Fat content (%)</param>
        /// <returns>Net energy for lactation (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForLactation(double milkProduction, double fatContent)
        {
            return milkProduction * (1.47 + 0.40 * fatContent);
        }

        /// <summary>
        ///     Equation 3.2.4-7
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
        ///     Equation 3.2.5-1
        /// </summary>
        /// <returns>Enteric CH4 emission (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionForCalves()
        {
            return 0.0;
        }

        /// <summary>
        ///     Equation 3.2.6-1
        /// </summary>
        /// <returns>Volatile solids (kg head^-1 day^-1)</returns>
        public double CalculateVolatileSolidsForCalves()
        {
            return 1.42;
        }

        /// <summary>
        ///     Equation 4.2.1-15
        /// </summary>
        /// <returns>Nitrogen excretion rate (kg head^-1 day^-1)</returns>
        public double CalculateNitrogenExcretionRateForCalves(
            double proteinIntake,
            double proteinRetained)
        {
            return proteinIntake / 6.25 - proteinRetained / 6.25;
        }

        /// <summary>
        ///     Equation 3.2.1-13
        ///     Equation 3.2.1-17
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg/d)</param>
        /// <param name="numberOfAnimals">Total number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionsUsingRaminHuhtanenMethod(double dryMatterIntake,
            double numberOfAnimals)
        {
            var emissionRate = (20 + 35.8 * dryMatterIntake - 0.5 * Math.Pow(dryMatterIntake, 2)) * 0.714 *
                               (1.0 / 1000.0);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        ///     Equation 3.2.1-14
        ///     Equation 3.2.1-17
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg/d)</param>
        /// <param name="numberOfAnimals">Total number of animals</param>
        /// <returns>Enteric methane emissions (kg CH4)</returns>
        public double CalculateEntericMethaneEmissionUsingMillsEtAl(double dryMatterIntake,
            double numberOfAnimals)
        {
            var emissionRate = (56.27 - 56.27 * Math.Exp(-0.028 * dryMatterIntake)) * (1.0 / 55.65);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        ///     Equation 3.2.1-15
        ///     Equation 3.2.1-17
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
            var emissionRate =
                (2.16 + 0.493 * dryMatterIntake - 1.36 * acidDetergentFiberIntake +
                 1.97 * neutralDetergentFiberIntake) * (1.0 / 55.65);

            var result = emissionRate * numberOfAnimals;

            return result;
        }

        /// <summary>
        ///     Equation 3.2.1-16
        ///     Equation 3.2.1-17
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
            var emissionRate =
                (76 + 13.5 * dryMatterIntake - 9.55 * etherExtract + 2.24 * dietaryNeutralDetergentFiber) *
                (1.0 / 1000.0);

            return emissionRate * numberOfAnimals;
        }

        /// <summary>
        ///     Equation 4.2.1-3
        ///     Overriden since diary lactating cows always lactate even if there are no associated calves.
        /// </summary>
        /// <param name="milkProduction">Milk production (kg head^-1 day^-1)</param>
        /// <param name="proteinContentOfMilk">Protein content of milk (kg kg⁻¹)</param>
        /// <param name="numberOfYoungAnimals">Number of calves</param>
        /// <param name="numberOfAnimals">Number of cows</param>
        /// <param name="animalsAreAlwaysLactating">
        ///     Indicates if the animal is always lactating regardless of the number of young
        ///     animals present
        /// </param>
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
        ///     Equation 9.4-1
        /// </summary>
        public double CalculateMilkProductionPerMonthFromDairyCattle(double milkProductionPerDay,
            double numberOfAnimals, double numberOfDaysInMonth)
        {
            return milkProductionPerDay * numberOfDaysInMonth * numberOfAnimals;
        }

        /// <summary>
        ///     Equation 9.4-2
        /// </summary>
        public double FatAndProteinCorrectedMilkProductionPerMonth(double milkProductionForMonth, double fatContent,
            double milkProtein)
        {
            return milkProductionForMonth * (0.1226 * fatContent) + 0.0776 * (milkProtein - 0.19) + 0.2534;
        }


        /// <summary>
        ///     Equation 6.2.1-1
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

            return numberOfLactatingDairyCows * (DairyCowConversion / CoreConstants.DaysInYear) *
                   energyConversionFactor * numberOfDaysInMonth;
        }

        #endregion
    }
}