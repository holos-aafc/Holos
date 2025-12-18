using System;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;

namespace H.Core.Services.Animals
{
    public class SheepResultsService : AnimalResultsServiceBase, ISheepResultsService
    {
        #region Constructor

        public SheepResultsService()
        {
            _pregnancyCoefficientProvider = new Table_25_Pregnancy_Coefficients_For_Sheep_Provider();
            _lambDailyWeightGainProvider = new Table_24_Lamb_Daily_Weight_Gain_Provider();

            _animalComponentCategory = ComponentCategory.Sheep;
        }

        #endregion

        #region Fields

        private readonly Table_24_Lamb_Daily_Weight_Gain_Provider _lambDailyWeightGainProvider;
        private readonly Table_25_Pregnancy_Coefficients_For_Sheep_Provider _pregnancyCoefficientProvider;

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

            InitializeDailyEmissions(dailyEmissions, managementPeriod, farm, dateTime);

            // Weaning lambs don't produce any emissions.
            if (managementPeriod.ProductionStage == ProductionStages.Weaning) return dailyEmissions;

            dailyEmissions.DateTime = dateTime;

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

            managementPeriod.NumberOfYoungAnimals = animalComponentBase.GetTotalNumberOfYoungAnimalsByDate(
                dateTime,
                animalGroup,
                AnimalType.Lambs);

            if (animalGroup.GroupType == AnimalType.Ewes)
                dailyEmissions.LambEweRatio = CalculateLambRatio(
                    managementPeriod.NumberOfYoungAnimals,
                    managementPeriod.NumberOfAnimals);

            dailyEmissions.NetEnergyForMaintenance = CalculateNetEnergyForMaintenance(
                managementPeriod.HousingDetails.BaselineMaintenanceCoefficient,
                dailyEmissions.AnimalWeight);

            dailyEmissions.NetEnergyForActivity = CalculateNetEnergyForActivity(
                managementPeriod.HousingDetails.ActivityCeofficientOfFeedingSituation,
                dailyEmissions.AnimalWeight);

            var isLactatingAnimalGroup = managementPeriod.NumberOfYoungAnimals > 0;
            if (isLactatingAnimalGroup)
            {
                var dailyGainForLambs = _lambDailyWeightGainProvider.GetDailyWeightGain(
                    dailyEmissions.LambEweRatio);

                if (managementPeriod.UseCustomMilkProductionValue == false)
                    dailyEmissions.NetEnergyForLactation = CalculateNetEnergyForLactation(
                        dailyGainForLambs,
                        managementPeriod.EnergyRequiredForMilk);
                else
                    dailyEmissions.NetEnergyForLactation = CalculateNetEnergyForLactationUsingMilkProduction(
                        managementPeriod.MilkProduction,
                        managementPeriod.EnergyRequiredForMilk);
            }

            if (animalGroup.GroupType.IsPregnantType() &&
                (managementPeriod.ProductionStage == ProductionStages.Gestating ||
                 managementPeriod.ProductionStage == ProductionStages.Lactating))
            {
                var pregnancyCoefficient = _pregnancyCoefficientProvider.GetPregnancyCoefficient(
                    dailyEmissions.LambEweRatio);

                dailyEmissions.PregnancyCoefficient = pregnancyCoefficient;

                dailyEmissions.NetEnergyForPregnancy = CalculateNetEnergyForPregnancy(
                    pregnancyCoefficient,
                    dailyEmissions.NetEnergyForMaintenance);
            }

            if (managementPeriod.AnimalType == AnimalType.Ewes || managementPeriod.AnimalType == AnimalType.Ram ||
                managementPeriod.AnimalType == AnimalType.SheepFeedlot)
                dailyEmissions.NetEnergyForWoolProduction = CalculateNetEnergyForWoolProduction(
                    managementPeriod.EnergyRequiredForWool,
                    managementPeriod.WoolProduction);

            if (animalGroup.GroupType == AnimalType.SheepFeedlot)
                dailyEmissions.AverageDailyGain = managementPeriod.PeriodDailyGain;
            else
                dailyEmissions.AverageDailyGain = CalculateAverageDailyWeightGain(
                    managementPeriod.StartWeight,
                    managementPeriod.EndWeight,
                    managementPeriod.Duration.TotalDays);

            dailyEmissions.NetEnergyForGain = CalculateNetEnergyForGain(
                dailyEmissions.AverageDailyGain,
                dailyEmissions.AnimalWeight,
                managementPeriod.GainCoefficientA,
                managementPeriod.GainCoefficientB);

            dailyEmissions.RatioOfEnergyAvailableForMaintenance = CalculateRatioOfEnergyAvailableForMaintenance(
                managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            dailyEmissions.RatioOfEnergyAvailableForGain = CalculateRatioEnergyAvailableForGain(
                managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            dailyEmissions.GrossEnergyIntake = CalculateGrossEnergyIntake(
                dailyEmissions.NetEnergyForMaintenance,
                dailyEmissions.NetEnergyForActivity,
                dailyEmissions.NetEnergyForLactation,
                dailyEmissions.NetEnergyForPregnancy,
                dailyEmissions.NetEnergyForGain,
                dailyEmissions.NetEnergyForWoolProduction,
                dailyEmissions.RatioOfEnergyAvailableForMaintenance,
                dailyEmissions.RatioOfEnergyAvailableForGain,
                managementPeriod.SelectedDiet.TotalDigestibleNutrient);

            dailyEmissions.EntericMethaneEmissionRate = CalculateEntericMethaneEmissionRateForSheep(
                dailyEmissions.GrossEnergyIntake,
                managementPeriod.SelectedDiet.MethaneConversionFactor);

            dailyEmissions.EntericMethaneEmission = CalculateEntericMethaneEmissions(
                dailyEmissions.EntericMethaneEmissionRate,
                managementPeriod.NumberOfAnimals);

            /*
             * Manure carbon (C) and methane (CH4)
             */

            var manureCompositionData = farm.GetManureCompositionData(ManureStateType.Pasture, AnimalType.Sheep);

            dailyEmissions.FecalCarbonExcretionRate = CalculateFecalCarbonExcretionRateForSheepPoultryAndOtherLivestock(
                managementPeriod.ManureDetails.ManureExcretionRate,
                manureCompositionData.CarbonFraction);

            dailyEmissions.FecalCarbonExcretion = CalculateAmountOfFecalCarbonExcreted(
                dailyEmissions.FecalCarbonExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfCarbonAddedFromBeddingMaterial = CalculateRateOfCarbonAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.1.1-6
            if (animalGroup.GroupType == AnimalType.Lambs)
                dailyEmissions.CarbonAddedFromBeddingMaterial = 0;
            else
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

            dailyEmissions.ProteinIntake = CalculateProteinIntake(
                dailyEmissions.GrossEnergyIntake,
                managementPeriod.SelectedDiet.CrudeProteinContent);

            // Equation 4.2.1-17
            dailyEmissions.ProteinRetained = 0.1;

            dailyEmissions.NitrogenExcretionRate = CalculateNitrogenExcretionRate(
                dailyEmissions.ProteinIntake,
                dailyEmissions.ProteinRetained);

            dailyEmissions.AmountOfNitrogenExcreted = CalculateAmountOfNitrogenExcreted(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial = CalculateRateOfNitrogenAddedFromBeddingMaterial(
                managementPeriod.HousingDetails.UserDefinedBeddingRate,
                managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.2.1-31
            if (animalGroup.GroupType == AnimalType.Lambs)
                dailyEmissions.AmountOfNitrogenAddedFromBedding = 0;
            else
                dailyEmissions.AmountOfNitrogenAddedFromBedding = CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                    dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                    managementPeriod.NumberOfAnimals);

            dailyEmissions.ManureDirectN2ONEmissionRate = CalculateManureDirectNitrogenEmissionRate(
                dailyEmissions.NitrogenExcretionRate,
                managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            dailyEmissions.ManureDirectN2ONEmission = CalculateManureDirectNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmissionRate,
                managementPeriod.NumberOfAnimals);

            /*
             * Ammonia emissions
             */

            // No equation for this when considering sheep (or beef) as it is a lookup table in algorithm document
            dailyEmissions.FractionOfNitrogenExcretedInUrine = GetFractionOfNitrogenExcretedInUrine(
                managementPeriod.SelectedDiet.CrudeProteinContent);

            dailyEmissions.TanExcretionRate = CalculateTANExcretionRate(
                dailyEmissions.NitrogenExcretionRate,
                dailyEmissions.FractionOfNitrogenExcretedInUrine);

            dailyEmissions.TanExcretion = CalculateTANExcretion(
                dailyEmissions.TanExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.FecalNitrogenExcretionRate = CalculateFecalNitrogenExcretionRate(
                dailyEmissions.NitrogenExcretionRate,
                dailyEmissions.TanExcretionRate);

            dailyEmissions.FecalNitrogenExcretion = CalculateFecalNitrogenExcretion(
                dailyEmissions.FecalNitrogenExcretionRate,
                managementPeriod.NumberOfAnimals);

            dailyEmissions.OrganicNitrogenInStoredManure = CalculateOrganicNitrogenInStoredManure(
                dailyEmissions.FecalNitrogenExcretion,
                dailyEmissions.AmountOfNitrogenAddedFromBedding);

            /*
             * Indirect manure N2O
             */

            CalculateIndirectEmissionsFromHousingAndStorage(dailyEmissions, managementPeriod);

            dailyEmissions.ManureN2ONEmission = CalculateManureNitrogenEmission(
                dailyEmissions.ManureDirectN2ONEmission,
                dailyEmissions.ManureIndirectN2ONEmission);

            dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage =
                CalculateNitrogenAvailableForLandApplicationFromSheepSwineAndOtherLivestock(
                    dailyEmissions.AmountOfNitrogenExcreted,
                    dailyEmissions.AmountOfNitrogenAddedFromBedding,
                    dailyEmissions.ManureDirectN2ONEmission,
                    dailyEmissions.TotalNitrogenLossesFromHousingAndStorage,
                    dailyEmissions.ManureN2ONLeachingEmission,
                    dailyEmissions.ManureNitrateLeachingEmission);

            // Equation 4.5.2-22
            dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay =
                dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage +
                (previousDaysEmissions == null
                    ? 0
                    : previousDaysEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.ManureCarbonNitrogenRatio = CalculateManureCarbonToNitrogenRatio(
                dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                dailyEmissions.AccumulatedNitrogenAvailableForLandApplicationOnDay);

            dailyEmissions.TotalAmountOfNitrogenInStoredManureAvailableForDay =
                dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage;

            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication =
                CalculateTotalVolumeOfManureAvailableForLandApplication(
                    dailyEmissions.NonAccumulatedNitrogenEnteringPoolAvailableInStorage,
                    managementPeriod.ManureDetails.FractionOfNitrogenInManure);

            dailyEmissions.AccumulatedVolume = dailyEmissions.TotalVolumeOfManureAvailableForLandApplication +
                                               (previousDaysEmissions == null
                                                   ? 0
                                                   : previousDaysEmissions.AccumulatedVolume);

            dailyEmissions.AmmoniaEmissionsFromLandAppliedManure = 0;

            // If animals are housed on pasture, overwrite direct/indirect N2O emissions from manure
            GetEmissionsFromGrazingSheepSwineAndOtherLiveStock(
                managementPeriod,
                dailyEmissions);

            return dailyEmissions;
        }

        protected override void CalculateEstimatesOfProduction(
            GroupEmissionsByMonth groupEmissionsByMonth,
            Farm farm)
        {
            if (groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.AnimalType == AnimalType.SheepFeedlot)
                groupEmissionsByMonth.MonthlyLambProduced = CalculateLambProducedPerMonth(
                    groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.PeriodDailyGain,
                    groupEmissionsByMonth.MonthsAndDaysData.ManagementPeriod.NumberOfAnimals,
                    groupEmissionsByMonth.DaysInMonth);
        }

        #endregion

        #region Equations

        /// <summary>
        ///     Equation 3.3.1-2
        /// </summary>
        /// <param name="numberOfLambs">Number of lambs</param>
        /// <param name="numberOfEwes">Number of ewes</param>
        /// <returns>Lamb:ewe ratio</returns>
        public double CalculateLambRatio(double numberOfLambs, double numberOfEwes)
        {
            if (numberOfEwes == 0) return 0;

            return numberOfLambs / numberOfEwes;
        }

        /// <summary>
        ///     Equation 3.3.1-4
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
        ///     Equation 3.3.1-5
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
        ///     Equation 3.3.1-6
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
        ///     Equation 3.3.1-7
        /// </summary>
        /// <param name="pregnancyCoefficient">Pregnancy constant</param>
        /// <param name="netEnergyForMaintenance">Net energy for maintenance (MJ head^-1 day^-1)</param>
        /// <returns>Net energy for pregnancy (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForPregnancy(double pregnancyCoefficient, double netEnergyForMaintenance)
        {
            return netEnergyForMaintenance * pregnancyCoefficient;
        }

        /// <summary>
        ///     Equation 3.3.1-8
        /// </summary>
        /// <param name="energyValueOfAKilogramOfWool">Energy value of 1 kg of wool (MJ kg^-1)</param>
        /// <param name="woolProduction">Wool production (kg year^-1)</param>
        /// <returns>Net energy for wool production (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForWoolProduction(double energyValueOfAKilogramOfWool, double woolProduction)
        {
            return energyValueOfAKilogramOfWool * woolProduction / CoreConstants.DaysInYear;
        }

        /// <summary>
        ///     Equation 3.3.1-10
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
        ///     Equation 3.3.1-11
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
        ///     Equation 3.3.1-12
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
        ///     Equation 3.3.1-13
        /// </summary>
        /// <param name="netEnergyForMaintenance">Net energy for maintenance (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForActivity">Net energy for activity (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForLactation">Net energy for lactation (MJ head -1 day^-1)</param>
        /// <param name="netEnergyForPregnancy">Net energy for pregnancy (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForGain">Net energy for gain (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForWoolProduction">Net energy for wool production (MJ head^-1 day^-1)</param>
        /// <param name="ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed">
        ///     Ratio of net energy available in
        ///     diet for maintenance to digestible energy consumed
        /// </param>
        /// <param name="ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed">
        ///     Ratio of net energy available in diet
        ///     for gain to digestible energy consumed
        /// </param>
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
        ///     Equation 3.3.1-14
        /// </summary>
        /// <param name="grossEnergyIntake">Gross energy intake (MJ head^-1 day^-1)</param>
        /// <param name="methaneConversionFactor">Methane conversion factor</param>
        /// <returns>Enteric CH4 emission rate (kg head^-1 day^-1)</returns>
        public double CalculateEntericMethaneEmissionRateForSheep(double grossEnergyIntake,
            double methaneConversionFactor)
        {
            return grossEnergyIntake * methaneConversionFactor / 55.65;
        }

        /// <summary>
        ///     Equation 3.4.2-3
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
        ///     Equation 3.4.3-2
        /// </summary>
        /// <returns>Protein retained</returns>
        public double CalculateProteinRetained()
        {
            return 0.10;
        }

        /// <summary>
        ///     Equation 4.2.1-18
        /// </summary>
        /// <param name="proteinIntake">Protein intake (kg head^-1 day^-1)</param>
        /// <param name="proteinRetained">Protein Retained</param>
        /// <returns>N excretion rate (kg head^-1 day^-1)</returns>
        public double CalculateNitrogenExcretionRate(double proteinIntake, double proteinRetained)
        {
            return (1.0 - proteinRetained) * proteinIntake / 6.25;
        }

        /// <summary>
        ///     Equation 3.4.3-6
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
        ///     Equation 3.4.3-8
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
        ///     Equation 3.4.3-12
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
        ///     Equation 3.4.5-1
        /// </summary>
        /// <param name="manureTotalDirectNitrogenEmissionFromSheep">Total manure direct N emission from sheep (kg N2O-N year^-1)</param>
        /// <returns>Total manure direct N2O emission from sheep (kg N2O year^-1)</returns>
        public double CalculateTotalManureDirectNitrousOxideEmissionFromSheep(
            double manureTotalDirectNitrogenEmissionFromSheep)
        {
            return manureTotalDirectNitrogenEmissionFromSheep * 44.0 / 28.0;
        }

        /// <summary>
        ///     Equation 3.4.5-2
        /// </summary>
        /// <param name="manureTotalIndirectNitrogenEmissionFromSheep">
        ///     Total manure indirect N emission from sheep (kg N2O-N
        ///     year^-1)
        /// </param>
        /// <returns>Total manure indirect N2O emission from sheep (kg N2O year^-1)</returns>
        public double CalculateTotalManureIndirectNitrousOxideEmissionFromSheep(
            double manureTotalIndirectNitrogenEmissionFromSheep)
        {
            return manureTotalIndirectNitrogenEmissionFromSheep * 44.0 / 28.0;
        }

        /// <summary>
        ///     Equation 3.4.5-3
        /// </summary>
        /// <param name="totalManureNitrogenEmission">Total manure N emission from sheep (kg N2O-N year^-1)</param>
        /// <returns>Total manure N2O emission from sheep (kg N2O year^-1)</returns>
        public double CalculateTotalManureNitrousOxideEmissionFromSheep(
            double totalManureNitrogenEmission)
        {
            return totalManureNitrogenEmission * 44 / 28;
        }

        /// <summary>
        ///     Equation 9.4-3
        /// </summary>
        /// <param name="averageDailyGain">Total manure N2O emission from sheep (kg N2O year^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <param name="numberOfDaysInMonth">Number of days in month</param>
        /// <returns></returns>
        public double CalculateLambProducedPerMonth(double averageDailyGain, double numberOfAnimals,
            double numberOfDaysInMonth)
        {
            return averageDailyGain * numberOfDaysInMonth * numberOfAnimals;
        }

        #endregion
    }
}