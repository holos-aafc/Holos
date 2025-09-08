﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Navigation;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.AnaerobicDigestion;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Energy;
using H.Core.Providers.Feed;
using H.Core.Tools;

namespace H.Core.Services.Animals
{
    public abstract partial class AnimalResultsServiceBase : IAnimalResultsService
    {
        #region Fields


        private static readonly DietProvider _dietProvider;

        protected readonly Table_49_Electricity_Conversion_Defaults_Provider _energyConversionDefaultsProvider = new Table_49_Electricity_Conversion_Defaults_Provider();
        protected readonly Table_43_Beef_Dairy_Default_Emission_Factors_Provider _defaultEmissionFactorsProvider = new Table_43_Beef_Dairy_Default_Emission_Factors_Provider();
        protected IAdditiveReductionFactorsProvider AdditiveReductionFactorsProvider = new Table_19_Additive_Reduction_Factors_Provider();
        protected IAnimalComponentHelper AnimalComponentHelper = new AnimalComponentHelper();

        protected ComponentCategory _animalComponentCategory;
        private static List<Diet> _diets;

        #endregion

        #region Constructors

        static AnimalResultsServiceBase()
        {
            _dietProvider = new DietProvider();
            _diets = _dietProvider.GetDiets();
        }

        protected AnimalResultsServiceBase()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public List<AnimalComponentEmissionsResults> CalculateResultsForAnimalComponents(
            IEnumerable<AnimalComponentBase> components,
            Farm farm)
        {
            var animalComponentEmissionResults = new List<AnimalComponentEmissionsResults>();

            foreach (var component in components)
            {
                var result = CalculateComponentEmissionResults(component, farm);
                animalComponentEmissionResults.Add(result);
            }

            return animalComponentEmissionResults;
        }

        public virtual AnimalGroupEmissionResults GetResultsForManagementPeriod(AnimalGroup animalGroup, ManagementPeriod managementPeriod, AnimalComponentBase animalComponent, Farm farm)
        {
            GroupEmissionsByDay previousDaysEmissions = null;
            var animalGroupEmissionResult = new AnimalGroupEmissionResults();
            animalGroupEmissionResult.AnimalGroup = animalGroup;

            if (managementPeriod.SelectedDiet == null)
            {
                managementPeriod.SelectedDiet = _diets.FirstOrDefault(x =>
                    x.AnimalType.GetCategory() == managementPeriod.DietGroupType.GetCategory());
            }

            var monthlyBreakdownForManagementPeriod = AnimalComponentHelper.GetMonthlyBreakdownFromManagementPeriod(managementPeriod);
            foreach (var month in monthlyBreakdownForManagementPeriod)
            {
                month.AnimalGroup = animalGroup;
                month.ManagementPeriod = managementPeriod;

                Trace.TraceInformation($"{nameof(AnimalResultsServiceBase)} calculating emissions for {month}.");

                // Daily emissions for the current month only
                var dailyEmissionsForMonth = new List<GroupEmissionsByDay>();

                var startDate = month.StartDate;
                var endDate = month.EndDate;

                for (var currentDate = startDate;
                     currentDate <= endDate;
                     currentDate = currentDate.AddDays(1))
                {
                    if (this.HasMovedToNewYear(currentDate))
                    {
                        previousDaysEmissions = null;
                    }

                    var groupEmissionsForDay = CalculateDailyEmissions(
                        animalComponentBase: animalComponent,
                        managementPeriod: managementPeriod,
                        dateTime: currentDate,
                        previousDaysEmissions: previousDaysEmissions,
                        animalGroup: animalGroup,
                        farm: farm);

                    previousDaysEmissions = groupEmissionsForDay;

                    dailyEmissionsForMonth.Add(groupEmissionsForDay);
                }

                var groupEmissionsByMonth = new GroupEmissionsByMonth(month, dailyEmissionsForMonth);

                CalculateEnergyEmissions(groupEmissionsByMonth, farm, animalComponent);
                CalculateEstimatesOfProduction(groupEmissionsByMonth, farm);

                animalGroupEmissionResult.GroupEmissionsByMonths.Add(groupEmissionsByMonth);
            }

            return animalGroupEmissionResult;
        }

        public virtual AnimalGroupEmissionResults GetResultsForGroup(AnimalGroup animalGroup, Farm farm, AnimalComponentBase animalComponent)
        {
            GroupEmissionsByDay previousDaysEmissions = null;
            var animalGroupEmissionResult = new AnimalGroupEmissionResults();
            animalGroupEmissionResult.AnimalGroup = animalGroup;

            foreach (var managementPeriod in animalGroup.ManagementPeriods)
            {
                if (managementPeriod.SelectedDiet == null)
                {
                    managementPeriod.SelectedDiet = _diets.FirstOrDefault(x =>
                        x.AnimalType.GetCategory() == managementPeriod.DietGroupType.GetCategory());
                }

                var monthlyBreakdownForManagementPeriod = AnimalComponentHelper.GetMonthlyBreakdownFromManagementPeriod(managementPeriod);
                foreach (var month in monthlyBreakdownForManagementPeriod)
                {
                    month.AnimalGroup = animalGroup;
                    month.ManagementPeriod = managementPeriod;

                    Trace.TraceInformation($"{nameof(AnimalResultsServiceBase)} calculating emissions for {month}.");

                    // Daily emissions for the current month only
                    var dailyEmissionsForMonth = new List<GroupEmissionsByDay>();

                    var startDate = month.StartDate;
                    var endDate = month.EndDate;

                    for (var currentDate = startDate;
                         currentDate <= endDate;
                         currentDate = currentDate.AddDays(1))
                    {
                        if (this.HasMovedToNewYear(currentDate)) 
                        {
                            previousDaysEmissions = null;
                        }

                        var groupEmissionsForDay = CalculateDailyEmissions(
                            animalComponentBase: animalComponent,
                            managementPeriod: managementPeriod,
                            dateTime: currentDate,
                            previousDaysEmissions: previousDaysEmissions,
                            animalGroup: animalGroup,
                            farm: farm);

                        previousDaysEmissions = groupEmissionsForDay;

                        dailyEmissionsForMonth.Add(groupEmissionsForDay);
                    }

                    var groupEmissionsByMonth = new GroupEmissionsByMonth(month, dailyEmissionsForMonth);

                    CalculateEnergyEmissions(groupEmissionsByMonth, farm, animalComponent);
                    CalculateEstimatesOfProduction(groupEmissionsByMonth, farm);

                    animalGroupEmissionResult.GroupEmissionsByMonths.Add(groupEmissionsByMonth);
                }
            }

            return animalGroupEmissionResult;
        }

        public virtual IList<AnimalGroupEmissionResults> CalculateResultsForComponent(
            AnimalComponentBase animalComponent,
            Farm farm)
        {
            Trace.TraceInformation($"{nameof(BeefCattleResultsService)}.{nameof(CalculateResultsForComponent)}: calculating emissions for {animalComponent.Name}.");

            var animalGroupEmissionResults = new List<AnimalGroupEmissionResults>();

            // Loop over all the animal groups in this animal component
            foreach (var animalGroup in animalComponent.Groups)
            {

                var animalGroupEmissionResult = this.GetResultsForGroup(animalGroup, farm, animalComponent);

                animalGroupEmissionResults.Add(animalGroupEmissionResult);
            }

            return animalGroupEmissionResults;
        }

        #endregion

        #region Private Methods

        private AnimalComponentEmissionsResults CalculateComponentEmissionResults(
            AnimalComponentBase component, Farm farm)
        {
            var animalComponentEmissionResults = new AnimalComponentEmissionsResults();
            animalComponentEmissionResults.Component = component;
            animalComponentEmissionResults.Farm = farm;

            var animalGroupEmissionResults = CalculateResultsForComponent(component, farm);
            foreach (var emissionsByGroup in animalGroupEmissionResults)
            {
                animalComponentEmissionResults.EmissionResultsForAllAnimalGroupsInComponent.Add(emissionsByGroup);
            }

            return animalComponentEmissionResults;
        }

        #endregion

        #region Protected Methods

        protected virtual void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm,
            AnimalComponentBase animalComponentBase)
        {
        }

        protected virtual void CalculateEstimatesOfProduction(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm)
        {
        }

        protected abstract GroupEmissionsByDay CalculateDailyEmissions(
            AnimalComponentBase animalComponentBase,
            ManagementPeriod managementPeriod,
            DateTime dateTime,
            GroupEmissionsByDay previousDaysEmissions,
            AnimalGroup animalGroup,
            Farm farm);

        #endregion

        #region Equations

        /// <summary>
        /// Equation 3.1.2-3
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake of calves (kg head^-1 day^-1)</param>
        /// <returns>Gross energy intake of calves (MJ head^-1 day^-1)</returns>
        public double CalculateGrossEnergyIntakeForCalves(
            double dryMatterIntake)
        {
            return dryMatterIntake * 18.45;
        }

        /*
         * Equations that are common to multiple types of animal components
         */

        /// <summary>
        /// Equation 3.1.2-2
        /// </summary>
        /// <param name="dietaryNetEnergyConcentration">Dietary net energy concentration of diet (MJ kg^-1)</param>
        /// <param name="weight">Average weight of animals (kg)</param>
        /// <param name="areMilkFedOnly"></param>
        /// <returns>The dry matter intake of animals (kg head^-1 day^-1)</returns>
        public double CalculateDryMatterIntakeForCalves(
            double dietaryNetEnergyConcentration,
            double weight,
            bool areMilkFedOnly)
        {
            if (areMilkFedOnly)
            {
                return weight * 0.01;
            }

            var term1 = Math.Pow(weight, 0.75);
            var term2 = (0.05822 * dietaryNetEnergyConcentration -
                         0.00266 * Math.Pow(dietaryNetEnergyConcentration, 2) - 0.1128);
            var term3 = 0.239 * dietaryNetEnergyConcentration;

            if (term3 == 0)
            {
                return 0;
            }
            else
            {
                return term1 * (term2 / term3);
            }
        }

        /// <summary>
        /// Equation 3.1.1-2
        /// </summary>
        /// <param name="baselineMaintenanceCoefficient">Baseline maintenance coefficient (MJ day^-1 kg^-1)</param>
        /// <param name="dailyTemperature">Daily temperature (upper limit = 20, temperatures above 20ºC or cattle are house in a barn, use 20ºC)</param>
        /// <returns>Maintenance coefficient – adjusted for temperature (MJ day^-1 kg^-1)</returns>
        public double CalculateTemperatureAdjustedMaintenanceCoefficient(
            double baselineMaintenanceCoefficient,
            double dailyTemperature)
        {
            if (dailyTemperature > 20)
            {
                dailyTemperature = 20;
            }

            var adjustedMaintenanceCoefficient =
                baselineMaintenanceCoefficient + 0.0048 * (20 - dailyTemperature);

            return adjustedMaintenanceCoefficient;
        }

        /// <summary>
        /// Equation 3.1.1-3
        /// Equation 3.2.1-2
        /// Equation 3.3.1-3
        /// </summary>
        /// <param name="maintenanceCoefficient">Maintenance coefficient – adjusted for temperature (MJ day^-1 kg⁻¹)</param>
        /// <param name="weight">Average weight (kg head^-1)</param>
        /// <returns>Net energy for maintenance (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForMaintenance(double maintenanceCoefficient, double weight)
        {
            if (Math.Abs(weight) < double.Epsilon)
            {
                return 0;
            }

            return maintenanceCoefficient * Math.Pow(weight, 0.75);
        }

        /// <summary>
        /// Equation 3.2.1-3
        /// Equation 3.1.1-4
        /// Equation 3.1.1-3
        /// </summary>
        /// <param name="feedingActivityCoefficient">Feeding activity coefficient</param>
        /// <param name="netEnergyForMaintenance">Net energy for maintenance (MJ head^-1 day^-1)</param>
        /// <returns>Net energy for activity (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForActivity(double feedingActivityCoefficient, double netEnergyForMaintenance)
        {
            return feedingActivityCoefficient * netEnergyForMaintenance;
        }

        /// <summary>
        /// Equation 3.1.1-6
        /// Equation 3.2.1-5
        /// </summary>
        /// <param name="netEnergyForMaintenance">Net energy for maintenance (MJ head^-1 day^-1)</param>
        /// <returns>Net energy for pregnancy (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForPregnancy(double netEnergyForMaintenance)
        {
            return 0.10 * netEnergyForMaintenance;
        }

        /// <summary>
        /// Equation 3.1.1-7
        /// Equation 3.2.1-6
        /// Equation 3.3.1-9
        /// </summary>
        /// <param name="initialWeight">Initial weight (kg head^-1)</param>
        /// <param name="finalWeight">Final weight (kg head^-1)</param>
        /// <param name="numberOfDays">Number of days</param>
        /// <returns>Average daily weight gain of animals (kg head^-1 day^-1)</returns>
        public double CalculateAverageDailyWeightGain(double initialWeight,
            double finalWeight,
            double numberOfDays)
        {
            if (Math.Abs(numberOfDays) < double.Epsilon)
            {
                return 0;
            }

            return (finalWeight - initialWeight) / numberOfDays;
        }

        /// <summary>
        /// Equation 3.1.1-8
        /// Equation 3.2.1-7
        /// </summary>
        /// <param name="weight">Average weight (kg head^-1)</param>
        /// <param name="gainCoefficient">Gain coefficient</param>
        /// <param name="averageDailyGain">Average daily gain (kg head^-1 day^-1)</param>
        /// <param name="finalWeight">The final weight of the animal</param>
        /// <returns>Net energy for gain (MJ head^-1 day^-1)</returns>
        public double CalculateNetEnergyForGain(double weight,
            double gainCoefficient,
            double averageDailyGain,
            double finalWeight)
        {
            if (Math.Abs(averageDailyGain) < Double.Epsilon ||
                Math.Abs(weight) < Double.Epsilon ||
                Math.Abs(gainCoefficient) < Double.Epsilon)
            {
                return 0;
            }

            var a = 22.02;
            var b = Math.Pow(weight / (gainCoefficient * finalWeight), 0.75);
            var c = Math.Pow(averageDailyGain, 1.097);

            return a * b * c;
        }

        /// <summary>
        /// Equation 3.1.1-9
        /// Equation 3.2.1-8
        /// </summary>
        /// <param name="totalDigestibleNutrient">Percent total digestible nutrients in feed (%)</param>
        /// <returns>Ratio of net energy available in diet for maintenance to digestible energy consumed</returns>
        public double CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy(
            double totalDigestibleNutrient)
        {
            if (Math.Abs(totalDigestibleNutrient) < double.Epsilon)
            {
                return 0;
            }

            var a = 1.123;
            var b = 0.004092 * totalDigestibleNutrient;
            var c = 0.00001126 * totalDigestibleNutrient * totalDigestibleNutrient;
            var d = 25.4 / totalDigestibleNutrient;

            return a - b + c - d;
        }

        /// <summary>
        /// Equation 3.1.1-10
        /// Equation 3.2.1-9
        /// </summary>
        /// <param name="totalDigestibleNutrient">Percent total digestible nutrients in feed (%)</param>
        /// <returns>Ratio of net energy available in diet for gain to digestible energy consumed</returns>
        public double CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed(
            double totalDigestibleNutrient)
        {
            if (Math.Abs(totalDigestibleNutrient) < double.Epsilon)
            {
                return 0;
            }

            var a = 1.164;
            var b = 0.005160 * totalDigestibleNutrient;
            var c = 0.00001308 * totalDigestibleNutrient * totalDigestibleNutrient;
            var d = 37.44 / totalDigestibleNutrient;

            return a - b + c - d;
        }

        /// <summary>
        /// Equation 3.1.1-11
        /// Equation 3.2.1-10
        /// </summary>
        /// <param name="netEnergyForMaintenance">Net energy for maintenance (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForActivity">Net energy for activity (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForLactation">Net energy for lactation (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForPregnancy">Net energy for pregnancy (MJ head^-1 day^-1)</param>
        /// <param name="netEnergyForGain">Net energy for gain (MJ head^-1 day^-1)</param>
        /// <param name="ratioOfEnergyAvailableForMaintenance">Ratio of net energy available in diet for maintenance to digestible energy consumed</param>
        /// <param name="ratioOfEnergyAvailableForGain">Ratio of net energy available in diet for gain to digestible energy consumed</param>
        /// <param name="percentTotalDigestibleNutrientsInFeed">Percent total digestible nutrients in feed (%)</param>
        /// <returns>Gross energy intake (MJ head^-1 day^-1)</returns>
        public double CalculateGrossEnergyIntake(
            double netEnergyForMaintenance,
            double netEnergyForActivity,
            double netEnergyForLactation,
            double netEnergyForPregnancy,
            double netEnergyForGain,
            double ratioOfEnergyAvailableForMaintenance,
            double ratioOfEnergyAvailableForGain,
            double percentTotalDigestibleNutrientsInFeed)
        {
            // Check for division by zero
            if (Math.Abs(ratioOfEnergyAvailableForMaintenance) < double.Epsilon ||
                Math.Abs(ratioOfEnergyAvailableForGain) < double.Epsilon ||
                Math.Abs(percentTotalDigestibleNutrientsInFeed) < double.Epsilon)
            {
                return 0;
            }

            var left = netEnergyForMaintenance + netEnergyForActivity + netEnergyForLactation + netEnergyForPregnancy;
            left /= ratioOfEnergyAvailableForMaintenance;
            var right = netEnergyForGain / ratioOfEnergyAvailableForGain;
            var denominator = percentTotalDigestibleNutrientsInFeed / 100.0;
            return (left + right) / denominator;
        }

        /// <summary>
        /// Equation 3.1.1-12
        /// Equation 3.2.1-11
        /// </summary>
        /// <param name="grossEnergyIntake">Gross energy intake (MJ head^-1 day^-1)</param>
        /// <param name="methaneConversionFactor">Methane conversion factor</param>
        /// <param name="additiveReductionFactor">Additive reduction factor</param>
        /// <returns>Enteric CH4 emission rate (kg CH4 head^-1 day^-1)</returns>
        public double CalculateEntericMethaneEmissionRate(
            double grossEnergyIntake,
            double methaneConversionFactor,
            double additiveReductionFactor)
        {
            return grossEnergyIntake * (methaneConversionFactor / 55.65) * (1.0 - additiveReductionFactor / 100.0);
        }

        /// <summary>
        /// Equation 3.1.1-13
        /// Equation 3.2.1-12
        /// Equation 3.3.1-15
        /// </summary>
        /// <param name="entericMethaneEmissionRate">Enteric CH4 emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of cattle</param>
        /// <returns>Enteric CH4 emission (kg CH4)</returns>
        public double CalculateEntericMethaneEmissions(
            double entericMethaneEmissionRate,
            double numberOfAnimals)
        {
            return entericMethaneEmissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 12.3.1-1
        /// </summary>
        /// <param name="grossEnergyIntake">Gross energy intake (MJ head^-1 day^-1)</param>
        /// <returns>Dry matter intake (kg head^-1 day^-1)</returns>
        public double CalculateDryMatterIntake(double grossEnergyIntake)
        {
            var conversionFactor = 18.45;

            return grossEnergyIntake / conversionFactor;
        }

        /// <summary>
        /// Equation 12.3.1-2
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake per animal and day (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals"></param>
        /// <returns>Dry matter intake per animal group during the entire management period (kg group^-1 period^-1)</returns>
        public double CalculateDryMatterIntakeForAnimalGroup(
            double dryMatterIntake,
            double numberOfAnimals)
        {
            return dryMatterIntake * numberOfAnimals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dryMatterIntakeForGroup"></param>
        /// <param name="numberOfDaysInManagementPeriod"></param>
        /// <returns></returns>
        public double CalculateDryMatterIntakeForManagementPeriod(
            double dryMatterIntakeForGroup,
            double numberOfDaysInManagementPeriod)
        {
            return dryMatterIntakeForGroup * numberOfDaysInManagementPeriod;
        }

        /// <summary>
        /// Equation 4.1.1-1
        /// </summary>
        /// <param name="grossEnergyIntake">Gross energy intake (MJ head^-1 day^-1)</param>
        /// <returns>Rate of C excreted through feces (kg head^-1 day^-1)</returns>
        public double CalculateFecalCarbonExcretionRate(
            double grossEnergyIntake)
        {
            var result = (grossEnergyIntake / 18.45) * 0.45 * (1 - 0.61);

            return result;
        }

        /// <summary>
        /// Equation 4.1.1-3
        ///
        /// Note: C content values for pasture/range/paddock are always used (regardless of manure handling system)
        /// </summary>
        /// <param name="manureExcretionRate">Volume of manure excreted daily (kg head^-1 day^-1)</param>
        /// <param name="carbonFractionOfManure">Fraction of carbon in the manure (% wet weight)</param>
        /// <returns>Rate of C excreted through feces (kg head^-1 day^-1)</returns>
        public double CalculateFecalCarbonExcretionRateForSheepPoultryAndOtherLivestock(
            double manureExcretionRate,
            double carbonFractionOfManure)
        {
            var result = (manureExcretionRate * carbonFractionOfManure) / 100.0;

            return result;
        }

        /// <summary>
        /// Equation 4.1.1-4
        /// </summary>
        /// <param name="excretionRate">Rate of C excreted through feces (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of cattle</param>
        /// <returns>Amount of C excreted (kg C)</returns>
        public double CalculateAmountOfFecalCarbonExcreted(
            double excretionRate,
            double numberOfAnimals)
        {
            return excretionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.1.1-6
        /// </summary>
        /// <param name="beddingRate">Rate of bedding material added (kg head^-1 day^-1)</param>
        /// <param name="carbonConcentrationOfBeddingMaterial">Carbon concentration of bedding material (kg C kg^-1 DM)</param>
        /// <param name="moistureContentOfBeddingMaterial">Moisture content of bedding material (%)</param>
        /// <returns>Rate of carbon added from bedding material (kg C head^-1 day^-1)</returns>
        public double CalculateRateOfCarbonAddedFromBeddingMaterial(
            double beddingRate,
            double carbonConcentrationOfBeddingMaterial,
            double moistureContentOfBeddingMaterial)
        {
            return beddingRate * carbonConcentrationOfBeddingMaterial *
                   (1 - (moistureContentOfBeddingMaterial / 100.0));
        }

        /// <summary>
        /// Equation 4.1.1-7
        /// </summary>
        /// <param name="rateOfCarbonAddedFromBedding">Rate of carbon added from bedding material (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Amount of carbon added from bedding materials (kg C)</returns>
        public double CalculateAmountOfCarbonAddedFromBeddingMaterial(
            double rateOfCarbonAddedFromBedding,
            double numberOfAnimals)
        {
            return rateOfCarbonAddedFromBedding * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.1.1-8
        /// </summary>
        /// <param name="carbonExcreted">Carbon excreted by animals (kg C)</param>
        /// <param name="carbonFromBedding">Carbon added from bedding (kg C)</param>
        /// <returns>Amount of carbon added from bedding materials (kg C)</returns>
        public double CalculateAmountOfCarbonFromManureAndBedding(
            double carbonExcreted,
            double carbonFromBedding)
        {
            return carbonExcreted + carbonFromBedding;
        }

        /// <summary>
        /// Equation 4.1.2-1
        /// </summary>
        /// <param name="grossEnergyIntake">Gross energy intake (MJ head^-1 day^-1)</param>
        /// <param name="percentTotalDigestibleNutrientsInFeed">Percent total digestible nutrients in feed (%)</param>
        /// <param name="ashContentOfFeed">Ash content of feed (%)</param>
        /// <param name="percentageForageInDiet">Percentage of forage in diet (% DM). Will be used to determine the grain content of the diet.</param>
        /// <returns>Volatile solids (kg head^-1 day^-1)</returns>
        public double CalculateVolatileSolids(
            double grossEnergyIntake,
            double percentTotalDigestibleNutrientsInFeed,
            double ashContentOfFeed,
            double percentageForageInDiet)
        {
            double totalGrainInDiet = 100 - percentageForageInDiet;
            if (totalGrainInDiet < 0)
            {
                totalGrainInDiet = 0;
            }

            double urinaryEnergy = 0.04;
            if (totalGrainInDiet >= 85)
            {
                urinaryEnergy = 0.02;
            }

            var a = grossEnergyIntake * (1.0 - percentTotalDigestibleNutrientsInFeed / 100.0) +
                    urinaryEnergy * grossEnergyIntake;
            var b = 1.0 - (ashContentOfFeed / 100.0);
            var c = 1.0 / 18.45;

            return a * b * c;
        }

        /// <summary>
        /// Equation 4.1.2-4
        /// </summary>
        /// <param name="volatileSolids">Volatile solids (kg head^-1 day^-1)</param>
        /// <param name="methaneProducingCapacity">Methane producing capacity (m³ CH4/kg VS)</param>
        /// <param name="methaneConversionFactor">Methane conversion factor</param>
        /// <returns>Manure CH4 emission rate (kg CH4 head^-1 day^-1)</returns>
        public double CalculateManureMethaneEmissionRate(double volatileSolids,
            double methaneProducingCapacity,
            double methaneConversionFactor)
        {
            return volatileSolids * methaneProducingCapacity * methaneConversionFactor * 0.67;
        }

        /// <summary>
        /// Equation 4.1.2-5
        /// </summary>
        /// <param name="emissionRate">Manure CH4 emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of cattle</param>
        /// <returns>Manure CH4 emission (kg CH4)</returns>
        public double CalculateManureMethane(
            double emissionRate,
            double numberOfAnimals)
        {
            return emissionRate * numberOfAnimals;
        }

        public void CalculateManureMethaneFromLiquidSystems(
            GroupEmissionsByDay dailyEmissions,
            GroupEmissionsByDay previousDaysEmissions,
            ManagementPeriod managementPeriod,
            double temperature, Farm farm)
        {
            var averageTemperatureOverLast30Days = this.CalculateDegreesKelvinOverLast30Days(farm, dailyEmissions);
            dailyEmissions.AverageTemperatureOverLast30Days = averageTemperatureOverLast30Days;

            dailyEmissions.ClimateFactor = this.CalculateClimateFactor(
                averageKelvinAirTemperature: averageTemperatureOverLast30Days);

            dailyEmissions.VolatileSolidsProduced = this.CalculateVolatileSolidsProduced(
                volatileSolids: dailyEmissions.VolatileSolids,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.VolatileSolidsLoaded = this.CalculateVolatileSolidsLoaded(
                volatileSolidsProduced: dailyEmissions.VolatileSolidsProduced);

            // (On days that the liquid manure is emptied, there is no carryover so VolatileSolidsAvailable = VolatileSolidsLoaded)
            dailyEmissions.VolatileSolidsAvailable = this.CalculateVolatileSolidsAvailable(
                volatileSolidsLoaded: dailyEmissions.VolatileSolidsLoaded,
                volatileSolidsAvailableFromPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsAvailable,
                volatileSolidsConsumedFromPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.VolatileSolidsConsumed);

            dailyEmissions.VolatileSolidsConsumed = this.CalculateVolatileSolidsConsumed(
                climateFactor: dailyEmissions.ClimateFactor,
                volatileSolidsAvailable: dailyEmissions.VolatileSolidsAvailable);

            dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethane(
                volatileSolidsConsumed: dailyEmissions.VolatileSolidsConsumed,
                methaneProducingCapacityOfManure: managementPeriod.ManureDetails.MethaneProducingCapacityOfManure);

            if (managementPeriod.ManureDetails.StateType.IsCoveredSystem())
            {
                var reductionFactor = 0d;
                if (managementPeriod.ManureDetails.StateType == ManureStateType.LiquidWithNaturalCrust)
                {
                    reductionFactor = 0.4;
                }
                else
                {
                    reductionFactor = 0.25;
                }

                dailyEmissions.ManureMethaneEmission = this.CalculateLiquidManureMethaneForCoveredSystem(
                    manureMethane: dailyEmissions.ManureMethaneEmission,
                    emissionReductionFactor: reductionFactor);
            }
        }

        /// <summary>
        /// Equation 4.1.3-1
        /// </summary>
        /// <param name="farm"></param>
        /// <param name="groupEmissionsByDay"></param>
        /// <returns>Average air temperature over last 30 days (degrees Kelvin)</returns>
        public double CalculateDegreesKelvinOverLast30Days(Farm farm, GroupEmissionsByDay groupEmissionsByDay)
        {
            var degreesKelvinList = new List<double>();

            var dateNow = groupEmissionsByDay.DateTime;
            var dateStart = dateNow.Subtract(TimeSpan.FromDays(30)).Date;
            var temperatures = farm.ClimateData.GetTemperatureByDateRange(dateStart.Date, dateNow.Date);

            foreach (var temperature in temperatures)
            {
                var degreesKelvin = 0d;
                var degreesCelsius = temperature;
                if (degreesCelsius <= 1)
                {
                    degreesCelsius = 1;
                }

                degreesKelvin = degreesCelsius + 273.15;
                degreesKelvinList.Add(degreesKelvin);
            }

            var result = degreesKelvinList.Average();

            return result;
        }

        /// <summary>
        /// Equation 4.1.3-2
        /// </summary>
        /// <param name="averageKelvinAirTemperature">Air temperature (degrees Kelvin)</param>
        /// <returns>Average air temperature over last 30 days(degrees Kelvin)</returns>
        public double CalculateClimateFactor(double averageKelvinAirTemperature)
        {
            const double t1 = 308.16;
            var t2 = averageKelvinAirTemperature;

            const double activationEnergy = 19347.0;
            const double idealGasConstant = 1.987;

            var numerator = activationEnergy * (t2 - t1);
            var denominator = idealGasConstant * t1 * t2;

            var exponentResult = Math.Exp(numerator / denominator);
            var innerTerm = 1 - exponentResult;

            var power = 1.0 / 30.0;
            var powerTerm = Math.Pow(innerTerm, power);

            var result = 1 - powerTerm;

            return result;
        }

        /// <summary>
        /// Equation 4.1.3-3
        /// </summary>
        /// <param name="volatileSolids">Volatile solids (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Total volatile solids produced by all animals (kg day^-1)</returns>
        public double CalculateVolatileSolidsProduced(
            double volatileSolids,
            double numberOfAnimals)
        {
            return volatileSolids * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.1.3-4
        /// </summary>
        /// <param name="volatileSolidsProduced">Volatile solids (kg day^-1)</param>
        /// <returns>Total volatile solids loaded into system (kg day^-1)</returns>
        public double CalculateVolatileSolidsLoaded(
            double volatileSolidsProduced)
        {
            var managementAndDesignPracticeFactor = 1.0;

            return volatileSolidsProduced * managementAndDesignPracticeFactor;
        }

        /// <summary>
        /// Equation 4.1.3-5
        /// </summary>
        /// <param name="volatileSolidsLoaded">Volatile solids loaded into system (kg day^-1)</param>
        /// <param name="volatileSolidsAvailableFromPreviousDay">Volatile solids available from the previous day (kg day^-1)</param>
        /// <param name="volatileSolidsConsumedFromPreviousDay">Volatile solids consumed from the previous day (kg day^-1)</param>
        /// <returns>Volatile solids available for conversion to CH4 (kg day^-1)</returns>
        public double CalculateVolatileSolidsAvailable(
            double volatileSolidsLoaded,
            double volatileSolidsAvailableFromPreviousDay,
            double volatileSolidsConsumedFromPreviousDay)
        {
            return volatileSolidsLoaded +
                   (volatileSolidsAvailableFromPreviousDay - volatileSolidsConsumedFromPreviousDay);
        }

        /// <summary>
        /// Equation 4.1.3-7
        /// </summary>
        /// <param name="climateFactor">Climate factor (degree Kelvin)</param>
        /// <param name="volatileSolidsAvailable">Volatile solids available (kg)</param>
        /// <returns>Daily volatile solids consumed (kg day^-1)</returns>
        public double CalculateVolatileSolidsConsumed(
            double climateFactor,
            double volatileSolidsAvailable)
        {
            return climateFactor * volatileSolidsAvailable;
        }

        /// <summary>
        /// Equation 4.1.3-8
        ///
        /// Calculates the manure methane produced from non-covered manure handling systems.
        /// </summary>
        /// <param name="volatileSolidsConsumed">Daily volatile solids consumed (kg day^-1)(</param>
        /// <param name="methaneProducingCapacityOfManure">Methane producing capacity of manure (unitless)</param>
        /// <returns>Daily methane emission (kg CH4)</returns>
        public double CalculateLiquidManureMethane(
            double volatileSolidsConsumed,
            double methaneProducingCapacityOfManure)
        {
            return volatileSolidsConsumed * methaneProducingCapacityOfManure * 0.67;
        }

        /// <summary>
        /// Equation 4.1.3-9
        ///
        /// Calculates the manure methane produced from covered manure handling systems.
        /// </summary>
        /// <param name="manureMethane">Daily manure methane emission (kg CH4)</param>
        /// <param name="emissionReductionFactor">Reduction in CH4 emissions from liquid systems due to cover.</param>
        /// <returns></returns>
        public double CalculateLiquidManureMethaneForCoveredSystem(
            double manureMethane,
            double emissionReductionFactor)
        {
            return manureMethane * (1 - emissionReductionFactor);
        }

        public void CalculateCarbonInStorage(
            GroupEmissionsByDay dailyEmissions,
            GroupEmissionsByDay previousDaysEmissions,
            ManagementPeriod managementPeriod)
        {
            dailyEmissions.AmountOfCarbonLostAsMethaneDuringManagement = this.CalculateCarbonLostAsMethaneDuringManagement(
                monthlyManureMethaneEmission: dailyEmissions.ManureMethaneEmission);

            dailyEmissions.AmountOfCarbonInStoredManure = this.CalculateAmountOfCarbonInStoredManure(
                dailyFecalCarbonExcretion: dailyEmissions.FecalCarbonExcretion,
                dailyAmountOfCarbonFromBedding: dailyEmissions.CarbonAddedFromBeddingMaterial,
                dailyAmountOfCarbonLostAsMethaneDuringManagement: dailyEmissions.AmountOfCarbonLostAsMethaneDuringManagement);

            dailyEmissions.NonAccumulatedCarbonCreatedOnDay = dailyEmissions.AmountOfCarbonInStoredManure;

            dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay = this.CalculateAmountOfCarbonInStorageOnCurrentDay(
                amountOfCarbonInStorageInPreviousDay: previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay,
                amountOfCarbonFlowingIntoStorage: dailyEmissions.AmountOfCarbonInStoredManure);

            if (managementPeriod.HousingDetails.HousingType.IsPasture())
            {
                dailyEmissions.AmountOfCarbonLostAsMethaneDuringManagement = 0;
                dailyEmissions.AmountOfCarbonInStoredManure = 0;
                dailyEmissions.NonAccumulatedCarbonCreatedOnDay = 0;
                dailyEmissions.AccumulatedAmountOfCarbonInStoredManureOnDay = 0;
            }
        }

        /// <summary>
        /// Equation 4.1.3-13
        /// </summary>
        /// <param name="monthlyManureMethaneEmission">Manure CH4 emission (kg CH4)</param>
        /// <returns>Carbon lost as methane during manure management (kg C)</returns>
        public double CalculateCarbonLostAsMethaneDuringManagement(
            double monthlyManureMethaneEmission)
        {
            return monthlyManureMethaneEmission * (12.0 / 16.0);
        }

        /// <summary>
        /// Equation 4.1.3-14
        /// </summary>
        /// <param name="dailyFecalCarbonExcretion">Amount of C excreted (kg C day^-1)</param>
        /// <param name="dailyAmountOfCarbonFromBedding">Amount of carbon added from bedding materials (kg C day^-1)</param>
        /// <param name="dailyAmountOfCarbonLostAsMethaneDuringManagement">Carbon lost as methane during manure management (kg C day^-1)</param>
        /// <returns>Total amount of C flowing into storage each day (minus housing emissions) (kg C day^-1)</returns>
        public double CalculateAmountOfCarbonInStoredManure(
            double dailyFecalCarbonExcretion,
            double dailyAmountOfCarbonFromBedding,
            double dailyAmountOfCarbonLostAsMethaneDuringManagement)
        {
            var result = (dailyFecalCarbonExcretion + dailyAmountOfCarbonFromBedding) - dailyAmountOfCarbonLostAsMethaneDuringManagement;

            return result;
        }

        /// <summary>
        /// Equation 4.1.3-15
        /// </summary>
        /// <param name="amountOfCarbonInStorageInPreviousDay">Amount of C in stored manure on the previous day for each animal group and manure management system (kg C)</param>
        /// <param name="amountOfCarbonFlowingIntoStorage">Amount of C flowing into storage from manure on the current day for each animal group and manure management system (kg C day^-1)</param>
        /// <returns>Amount of C in stored manure on the current day for each animal group and manure management system (kg C)</returns>
        public double CalculateAmountOfCarbonInStorageOnCurrentDay(
            double amountOfCarbonInStorageInPreviousDay,
            double amountOfCarbonFlowingIntoStorage)
        {
            return amountOfCarbonFlowingIntoStorage + amountOfCarbonInStorageInPreviousDay;
        }

        /// <summary>
        /// Equation 4.2.1-31
        /// </summary>
        /// <param name="beddingRate">Rate of bedding material added (kg head^-1 day^-1)</param>
        /// <param name="nitrogenConcentrationOfBeddingMaterial">Nitrogen concentration of bedding material (kg N kg^-1 DM)</param>
        /// <param name="moistureContentOfBeddingMaterial">Moisture content of bedding material (%)</param>
        /// <returns>Rate of nitrogen added from bedding material (kg N head^-1 day^-1)</returns>
        public double CalculateRateOfNitrogenAddedFromBeddingMaterial(
            double beddingRate,
            double nitrogenConcentrationOfBeddingMaterial,
            double moistureContentOfBeddingMaterial)
        {
            return beddingRate * nitrogenConcentrationOfBeddingMaterial *
                   (1 - (moistureContentOfBeddingMaterial / 100.0));
        }

        /// <summary>
        /// Equation 4.2.1-32
        /// </summary>
        /// <param name="rateOfNitrogenAddedFromBedding">Rate of nitrogen added from bedding material (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of cattle</param>
        /// <returns>Total amount of nitrogen added from bedding materials (kg N day^-1)</returns>
        public double CalculateAmountOfNitrogenAddedFromBeddingMaterial(
            double rateOfNitrogenAddedFromBedding,
            double numberOfAnimals)
        {
            return rateOfNitrogenAddedFromBedding * numberOfAnimals;
        }

        public void CalculateDirectN2OFromBeefAndDairy(
            GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod,
            AnimalGroup animalGroup,
            bool isLactatingAnimalGroup,
            double totalNumberOfYoungAnimalsOnDate)
        {
            // Equation 4.2.1-1
            dailyEmissions.ProteinIntake = this.CalculateProteinIntake(
                grossEnergyIntake: dailyEmissions.GrossEnergyIntake,
                crudeProtein: managementPeriod.SelectedDiet.CrudeProteinContent);

            if (animalGroup.GroupType.IsPregnantType())
            {
                // Equation 4.2.1-2
                dailyEmissions.ProteinRetainedForPregnancy = this.CalculateProteinRetainedForPregnancy();
            }

            if (isLactatingAnimalGroup)
            {
                // Dairy lactating cows are always lactating - beef lactating cows only lactate when calves are present
                var animalsAreAlwaysLactating = animalGroup.GroupType == AnimalType.DairyLactatingCow;

                // Equation 4.2.1-3
                dailyEmissions.ProteinRetainedForLactation = this.CalculateProteinRetainedForLactation(
                    milkProduction: managementPeriod.MilkProduction,
                    proteinContentOfMilk: managementPeriod.MilkProteinContent,
                    numberOfYoungAnimals: totalNumberOfYoungAnimalsOnDate,
                    numberOfAnimals: managementPeriod.NumberOfAnimals,
                    animalsAreAlwaysLactating: animalsAreAlwaysLactating);
            }

            if (managementPeriod.HasGrowingAnimals)
            {
                // Equation 4.2.1-4
                dailyEmissions.EmptyBodyWeight = this.CalculateEmptyBodyWeight(
                    weight: dailyEmissions.AnimalWeight);

                // Equation 4.2.1-5
                dailyEmissions.EmptyBodyGain = this.CalculateEmptyBodyGain(
                    averageDailyGain: dailyEmissions.AverageDailyGain);

                // Equation 4.2.1-6
                dailyEmissions.RetainedEnergy = this.CalculateRetainedEnergy(
                    emptyBodyWeight: dailyEmissions.EmptyBodyWeight,
                    emptyBodyGain: dailyEmissions.EmptyBodyGain);

                // Equation 4.2.1-7
                dailyEmissions.ProteinRetainedForGain = this.CalculateProteinRetainedForGain(
                    averageDailyGain: dailyEmissions.AverageDailyGain,
                    retainedEnergy: dailyEmissions.RetainedEnergy);
            }

            // Equation 4.2.1-8
            dailyEmissions.NitrogenExcretionRate = this.CalculateNitrogenExcretionRate(
                proteinIntake: dailyEmissions.ProteinIntake,
                proteinRetainedForPregnancy: dailyEmissions.ProteinRetainedForPregnancy,
                proteinRetainedForLactation: dailyEmissions.ProteinRetainedForLactation,
                proteinRetainedForGain: dailyEmissions.ProteinRetainedForGain);

            // Equation 4.2.1-29 (used in volatilization calculation)
            dailyEmissions.AmountOfNitrogenExcreted = this.CalculateAmountOfNitrogenExcreted(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.1-31
            dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial = this.CalculateRateOfNitrogenAddedFromBeddingMaterial(
                beddingRate: managementPeriod.HousingDetails.UserDefinedBeddingRate,
                nitrogenConcentrationOfBeddingMaterial: managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding,
                moistureContentOfBeddingMaterial: managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial);

            // Equation 4.2.1-32
            dailyEmissions.AmountOfNitrogenAddedFromBedding = this.CalculateAmountOfNitrogenAddedFromBeddingMaterial(
                rateOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            // Equation 4.2.2-1
            dailyEmissions.ManureDirectN2ONEmissionRate = this.CalculateManureDirectNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                emissionFactor: managementPeriod.ManureDetails.N2ODirectEmissionFactor);

            // Equation 4.2.2-2
            dailyEmissions.ManureDirectN2ONEmission = this.CalculateManureDirectNitrogenEmission(
                manureDirectNitrogenEmissionRate: dailyEmissions.ManureDirectN2ONEmissionRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);
        }

        /// <summary>
        /// Equation 4.2.1-1
        /// Equation 4.2.1-16
        /// </summary>
        /// <param name="grossEnergyIntake">Gross energy intake (MJ head^-1 day^-1)</param>
        /// <param name="crudeProtein">Crude protein content (kg kg⁻¹)</param>
        /// <returns>Protein intake (kg head^-1 day^-1)</returns>
        public double CalculateProteinIntake(double grossEnergyIntake, double crudeProtein)
        {
            return (grossEnergyIntake / 18.45) * crudeProtein;
        }

        /// <summary>
        /// Equation 4.2.1-2
        /// </summary>
        /// <returns>Protein retained for pregnancy (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForPregnancy()
        {
            return 5.0 / CoreConstants.DaysInYear;
        }

        /// <summary>
        /// Equation 4.2.1-3
        /// </summary>
        /// <param name="milkProduction">Milk production (kg head^-1 day^-1)</param>
        /// <param name="proteinContentOfMilk">Protein content of milk (kg kg⁻¹)</param>
        /// <param name="numberOfYoungAnimals">Number of calves</param>
        /// <param name="numberOfAnimals">Number of cows</param>
        /// <param name="animalsAreAlwaysLactating">Indicates if the animal is always lactating regardless of the number of young animals present</param>
        /// <returns>Protein retained for lactation (kg head^-1 day^-1)</returns>
        public virtual double CalculateProteinRetainedForLactation(
            double milkProduction,
            double proteinContentOfMilk,
            double numberOfYoungAnimals,
            double numberOfAnimals,
            bool animalsAreAlwaysLactating)
        {
            if (Math.Abs(numberOfAnimals) < Double.Epsilon)
            {
                return 0;
            }

            if (animalsAreAlwaysLactating)
            {
                return milkProduction * proteinContentOfMilk;
            }
            else
            {
                return milkProduction * proteinContentOfMilk * numberOfYoungAnimals / numberOfAnimals;
            }
        }

        /// <summary>
        /// Equation 3.2.3-3
        /// </summary>
        /// <param name="milkProduction">Milk production (kg head^-1 day^-1)</param>
        /// <param name="milkProtein">Protein content of milk(kg kg^-1)</param>
        /// <returns>Protein retained for lactation (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForLactation(double milkProduction, double milkProtein)
        {
            return milkProduction * milkProtein;
        }

        /// <summary>
        /// Equation 4.2.1-4
        /// </summary>
        /// <param name="weight">Average weight (kg head^-1)</param>
        /// <returns>Empty body weight (kg head^-1)</returns>
        public double CalculateEmptyBodyWeight(double weight)
        {
            return 0.891 * weight;
        }


        /// <summary>
        /// Equation 4.2.1-5
        /// </summary>
        /// <param name="averageDailyGain">Average daily gain (kg head^-1 day^-1)</param>
        /// <returns>Empty body gain (kg head^-1 day^-1)</returns>
        public double CalculateEmptyBodyGain(double averageDailyGain)
        {
            return 0.956 * averageDailyGain;
        }

        /// <summary>
        /// Equation 4.2.1-6
        /// </summary>
        /// <param name="emptyBodyWeight">Empty body weight (kg head^-1)</param>
        /// <param name="emptyBodyGain">Empty body gain (kg head^-1 day^-1)</param>
        /// <returns>Retained energy (Mcal head^-1 day^-1)</returns>
        public double CalculateRetainedEnergy(double emptyBodyWeight, double emptyBodyGain)
        {
            if (Math.Abs(emptyBodyGain) < double.Epsilon ||
                Math.Abs(emptyBodyWeight) < double.Epsilon)
            {
                return 0;
            }

            return 0.0635 * Math.Pow(emptyBodyWeight, 0.75) * Math.Pow(emptyBodyGain, 1.097);
        }

        /// <summary>
        /// Equation 4.2.1-7
        /// </summary>
        /// <param name="averageDailyGain">Average daily gain (kg head^-1 day^-1)</param>
        /// <param name="retainedEnergy">Retained energy (Mcal head^-1 day^-1)</param>
        /// <returns>Protein retained for gain (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForGain(double averageDailyGain, double retainedEnergy)
        {
            if (Math.Abs(averageDailyGain) < double.Epsilon)
            {
                return 0;
            }

            return averageDailyGain * ((268.0 - (29.4 * (retainedEnergy / averageDailyGain))) / 1000.0);
        }

        /// <summary>
        /// Equation 4.2.1-8
        /// Equation 3.1.2-19
        /// </summary>
        /// <param name="proteinIntake">Protein intake (kg head^-1 day^-1)</param>
        /// <param name="proteinRetainedForPregnancy">Protein retained for pregnancy (kg head^-1 day^-1)</param>
        /// <param name="proteinRetainedForLactation">Protein retained for lactation (kg head^-1 day^-1)</param>
        /// <param name="proteinRetainedForGain">Protein retained for gain (kg head^-1 day^-1)</param>
        /// <returns>N excretion rate (kg head^-1 day^-1)</returns>
        public double CalculateNitrogenExcretionRate(
            double proteinIntake,
            double proteinRetainedForPregnancy,
            double proteinRetainedForLactation,
            double proteinRetainedForGain)
        {
            return (proteinIntake / 6.25) - (proteinRetainedForPregnancy / 6.25 + proteinRetainedForLactation / 6.38 +
                                             proteinRetainedForGain / 6.25);
        }

        /// <summary>
        /// Equation 4.2.1-29
        ///
        /// Used in volatilization calculation
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of cattle</param>
        /// <returns>Amount of N excreted (kg N)</returns>
        public double CalculateAmountOfNitrogenExcreted(
            double nitrogenExcretionRate,
            double numberOfAnimals)
        {
            return nitrogenExcretionRate * numberOfAnimals;
        }

        /// <summary>
        /// Used for beef cattle and sheep. Dairy has different methodology for calculating fraction of N excreted in urine
        /// </summary>
        /// <returns>Fraction of nitrogen excreted in urine [kg TAN (kg manure-N)^-1]</returns>
        public double GetFractionOfNitrogenExcretedInUrine(double crudeProteinInDiet)
        {
            if (crudeProteinInDiet < 0.09)
            {
                return 0.4;
            }
            else if (crudeProteinInDiet >= 0.09 && crudeProteinInDiet < 0.15)
            {
                return 0.57;
            }
            else
            {
                return 0.61;
            }
        }

        /// <summary>
        /// Equation 4.3.1-3
        /// Equation 4.3.4-1
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="fractionOfNitrogenExcretedInUrine">Fraction of N excreted in urine (urinary-N or urea-N fraction), varied with diet CP content (kg N kg^-1 total N excreted)</param>
        /// <returns>Total ammoniacal nitrogen (TAN) excretion rate (kg TAN head^-1 day^-1)</returns>
        public double CalculateTANExcretionRate(double nitrogenExcretionRate, double fractionOfNitrogenExcretedInUrine)
        {
            return nitrogenExcretionRate * fractionOfNitrogenExcretedInUrine;
        }

        /// <summary>
        /// Equation 4.3.1-4
        /// Equation 4.3.3-1
        /// Equation 4.3.4-2
        /// Equation 4.3.3-1
        /// </summary>
        /// <param name="tanExcretionRate">Total Ammonical nitrogen (TAN) excretion rate (kg TAN head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Total ammonical nitrogen (TAN) excretion (kg TAN animal^-1 day^-1)</returns>
        public double CalculateTANExcretion(double tanExcretionRate, double numberOfAnimals)
        {
            return tanExcretionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.1-5
        /// Equation 4.3.3-2
        /// Equation 4.3.4-3
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="tanExcretionRate">Fraction of N excreted in urine (urinary-N or urea-N fraction), varied with diet CP content (kg N kg^-1 total N excreted)</param>
        /// <returns>Fecal N excretion rate (kg N head^-1 d^-1)</returns>
        public double CalculateFecalNitrogenExcretionRate(
            double nitrogenExcretionRate,
            double tanExcretionRate)
        {
            return nitrogenExcretionRate - tanExcretionRate;
        }

        /// <summary>
        /// Equation 4.3.1-6
        /// Equation 4.3.3-3
        /// Equation 4.3.4-4
        /// </summary>
        /// <param name="fecalNitrogenExcretionRate">Fecal N excretion rate (kg N head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Total nitrogen excreted through feces (kg N day^-1)</returns>
        public double CalculateFecalNitrogenExcretion(double fecalNitrogenExcretionRate, double numberOfAnimals)
        {
            return fecalNitrogenExcretionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.1-7
        /// Equation 4.3.3-4
        /// Equation 4.3.4-5
        /// Total amount of N added from feces, urine, and bedding material
        /// </summary>
        /// <param name="totalNitrogenExcretedThroughFeces">Total nitrogen excreted through feces (kg N animal^-1 day^-1)</param>
        /// <param name="amountOfNitrogenAddedFromBedding">Total amount of nitrogen added from bedding materials (kg N animal^-1 day^-1)</param>
        /// <returns>Organic nitrogen in stored manure (kg N)</returns>
        public double CalculateOrganicNitrogenInStoredManure(
            double totalNitrogenExcretedThroughFeces,
            double amountOfNitrogenAddedFromBedding)
        {
            return totalNitrogenExcretedThroughFeces + amountOfNitrogenAddedFromBedding;
        }

        /// <summary>
        /// Equation 4.3.4-6
        /// </summary>
        /// <param name="nitrogenExcretionRate">Fecal N excretion rate (kg N head^-1 day^-1)</param>
        /// <param name="rateOfNitrogenAddedFromBedding">Total amount of nitrogen added from bedding materials (kg N animal^-1 day^-1)</param>
        /// <param name="volatilizationFraction">Volatilization fraction for sheep, swine and other livestock groups</param>
        /// <returns>Manure N losses via NH3 volatilization during housing and storage for sheep, swine, and other livestock manure systems (kg NH3-N)</returns>
        public double CalculateAmmoniaEmissionRateFromHousingAndStorage(
            double nitrogenExcretionRate,
            double rateOfNitrogenAddedFromBedding,
            double volatilizationFraction)
        {
            return (nitrogenExcretionRate + rateOfNitrogenAddedFromBedding) * volatilizationFraction;
        }

        /// <summary>
        /// Equation 4.3.4-7
        /// </summary>
        /// <param name="ammoniaEmissionRate">Manure N losses via NH3 volatilization during housing and storage for sheep, swine, and other livestock manure systems (kg NH3-N)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Total manure N losses via NH3 volatilization during housing and storage for sheep, swine, and other livestock manure systems (kg NH3-N)</returns>
        public double CalculateTotalNitrogenLossFromHousingAndStorage(
            double ammoniaEmissionRate,
            double numberOfAnimals)
        {
            return ammoniaEmissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.5.3-10
        /// </summary>
        /// <param name="totalAmmoniaLossFromHousingAndStorage">Total manure N losses via NH3 volatilization during housing and storage for sheep, swine, and other livestock manure systems (kg N)</param>
        /// <param name="manureVolatilizationEmissions">Manure volatilization N emissions during the housing and manure storage stages (kg N2O-N day-1)</param>
        /// <returns></returns>
        public double CalculateAmmoniaAdjustmentFromHousingAndStorage(
            double totalAmmoniaLossFromHousingAndStorage,
            double manureVolatilizationEmissions)
        {
            return totalAmmoniaLossFromHousingAndStorage - manureVolatilizationEmissions;
        }

        /// <summary>
        /// Equation 4.2.2-1
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="emissionFactor">Emission factor [kg N2O-N (kg N)^-1]</param>
        /// <returns>Manure direct N emission rate (kg N2O-N head^-1 day^-1)</returns>
        public double CalculateManureDirectNitrogenEmissionRate(double nitrogenExcretionRate, double emissionFactor)
        {
            return nitrogenExcretionRate * emissionFactor;
        }

        /// <summary>
        /// Equation 4.2.2-2
        /// Equation 5.3.1-3
        /// </summary>
        /// <param name="manureDirectNitrogenEmissionRate">Manure direct N emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Manure direct N emission (kg N2O-N)</returns>
        public double CalculateManureDirectNitrogenEmission(
            double manureDirectNitrogenEmissionRate,
            double numberOfAnimals)
        {
            return manureDirectNitrogenEmissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.1-8
        /// </summary>
        /// <param name="temperature">Average monthly temperature (°C)</param>
        /// <returns>Ambient temperature-based adjustments used to correct default NH3 emission for feedlot (confined no barn) housing, or pasture (unitless)</returns>
        public double CalculateAmbientTemperatureAdjustmentNoBarn(double temperature)
        {
            return Math.Pow(1.041, temperature) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 4.3.1-12
        ///
        /// Indoor dairy housing systems do not get an additional +2 degrees - just beef housing systems
        /// </summary>
        /// <param name="dailyTemperature">Average daily outdoor temperature (°C)</param>
        /// <param name="isDairyIndoorHousing"></param>
        /// <returns>Temperature-based adjustments used to correct default NH3 emission factors for indoor housing</returns>
        public double CalculateAmbientTemperatureAdjustmentForIndoorHousing(double dailyTemperature,
            bool isDairyIndoorHousing = false)
        {
            var temperatureAdjustment = dailyTemperature;
            if (isDairyIndoorHousing == false)
            {
                temperatureAdjustment += 2;
            }

            return Math.Pow(1.041, temperatureAdjustment) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 4.3.1-10
        /// Equation 4.3.1-14
        /// Equation 4.3.3-5
        /// </summary>
        /// <param name="tanExcretionRate">Total Ammonical Nitrogen (TAN) excretion rate (kg TAN head^-1 day^-1)</param>
        /// <param name="adjustedEmissionFactor">Adjusted ammonia emission factor</param>
        /// <returns>Ammonia nitrogen emission rate from animals (kg NH3 head^-1 d^-1)</returns>
        public double CalculateAmmoniaEmissionRateFromHousing(
            double tanExcretionRate,
            double adjustedEmissionFactor)
        {
            return tanExcretionRate * adjustedEmissionFactor;
        }

        /// <summary>
        /// Equation 4.3.1-11
        /// Equation 4.3.1-15
        /// Equation 4.3.3-6
        /// </summary>
        /// <param name="emissionRate">Ammonia nitrogen emission rate from housing of animals (kg NH3 head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Total ammonia nitrogen production animals (kg NH3-N)</returns>
        public double CalculateAmmoniaConcentrationInHousing(
            double emissionRate,
            double numberOfAnimals)
        {
            return emissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.1-9
        /// Equation 4.3.1-13
        /// </summary>
        /// <param name="emissionFactor">Default ammonia emission factor</param>
        /// <param name="temperatureAdjustment">Ambient temperature-based adjustments used to correct default NH3 emission factor (Table 17)</param>
        /// <returns>Adjusted ammonia emission factor for housing (kg NH3-N (kg TAN)^-1)</returns>
        public double CalculateAdjustedEmissionFactorHousing(
            double emissionFactor,
            double temperatureAdjustment)
        {
            var result = emissionFactor * temperatureAdjustment;
            if (result < 0)
            {
                return 0;
            }
            else if (result > 1)
            {
                return 1;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Equation 4.3.2-1
        /// Equation 4.3.3-7
        /// Equation 4.3.3-11
        /// </summary>
        /// <param name="tanExcretion">TAN excreted by the animals for this period of time (kg TAN)</param>
        /// <param name="ammoniaLostFromHousing">NH3-N lost through NH3 emissions from housing systems (kg NH3-N) </param>
        /// <returns>Amount of TAN entering the storage system (kg TAN day^-1)</returns>
        public double CalculateTanFlowingIntoStorageEachDay(double tanExcretion,
            double ammoniaLostFromHousing)
        {
            return tanExcretion - ammoniaLostFromHousing;
        }

        /// <summary>
        /// Equation 4.3.2-2
        /// </summary>
        /// <param name="tanEnteringStorageSystem">Amount of TAN entering the storage system each day (kg TAN day^-1)</param>
        /// <param name="fractionOfTanImmobilizedToOrganicNitrogen">Fraction of TAN that is immobilized to organic N during manure storage, dimensionless</param>
        /// <param name="fractionOfTanNitrifiedDuringManureStorage">Fraction of TAN that is nitrified during storage</param>
        /// <param name="nitrogenExcretedThroughFeces">Nitrogen excreted through feces (kg N)</param>
        /// <param name="fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage">Fraction of organic N that is mineralized as TAN during manure storage, dimensionless </param>
        /// <param name="beddingNitrogen">Bedding nitrogen (kg N)</param>
        /// <returns>Adjusted amount of TAN in stored manure (kg TAN day^-1)</returns>
        public double CalculateAdjustedAmountOfTanFlowingIntoStorageEachDay(
            double tanEnteringStorageSystem,
            double fractionOfTanImmobilizedToOrganicNitrogen,
            double fractionOfTanNitrifiedDuringManureStorage,
            double nitrogenExcretedThroughFeces,
            double fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage,
            double beddingNitrogen)
        {
            return (tanEnteringStorageSystem * (1 - fractionOfTanImmobilizedToOrganicNitrogen) *
                    (1 - fractionOfTanNitrifiedDuringManureStorage)) +
                   ((nitrogenExcretedThroughFeces + beddingNitrogen) *
                    fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage);
        }

        /// <summary>
        /// Equation 4.3.2-9
        /// Equation 4.3.3-12
        /// </summary>
        /// <param name="tanInStorageOnPreviousDay">TAN in storage on the previous day for each animal group and manure management system (kg TAN) </param>
        /// <param name="flowOfTanEnteringStorage">Adjusted amount of TAN in stored manure (kg TAN day^-1)</param>
        /// <returns>TAN in storage on the current day for each animal group and manure management system (kg TAN)</returns>
        public double CalculateAmountOfTanInStorageOnDay(double tanInStorageOnPreviousDay, double flowOfTanEnteringStorage)
        {
            return tanInStorageOnPreviousDay + flowOfTanEnteringStorage;
        }

        /// <summary>
        /// Equation 4.3.2-6
        /// Equation 4.3.3-9
        /// </summary>
        /// <param name="ambientTemperatureAdjustmentStorage">Ambient temperature-based adjustments used to correct default NH3 emission factors for manure storage (compost, stockpile/deep bedding)</param>
        /// <param name="ammoniaEmissionFactorStorage">Default ammonia emission factor for manure stores (deep bedding, solid storage/stockpile, compost (passive, active))</param>
        /// <returns>Adjusted ammonia emission factor for stored manure [kg NH3-N (kg TAN)^-1]; 0 ≤ EF ≤ 1</returns>
        public double CalculateAdjustedAmmoniaEmissionFactorStoredManure(
            double ambientTemperatureAdjustmentStorage,
            double ammoniaEmissionFactorStorage)
        {
            var result = ambientTemperatureAdjustmentStorage * ammoniaEmissionFactorStorage;
            if (result < 0)
            {
                return 0;
            }
            else if (result > 1)
            {
                return 1;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Equation 4.3.2-7
        /// Equation 4.3.3-10
        /// </summary>
        /// <param name="amountOfTANEnteringStorageDaily">Amount of TAN entering the storage system each day (kg TAN day^-1)</param>
        /// <param name="adjustedAmmoniaEmissionFactor">Adjusted ammonia emission factor for beef barn (0 ≤ EF≤ 1)</param>
        /// <returns>Ammonia nitrogen loss from stored manure (stockpile/deep bedding, compost) (kg NH3-N)</returns>
        public double CalculateAmmoniaLossFromStoredManure(
            double amountOfTANEnteringStorageDaily,
            double adjustedAmmoniaEmissionFactor)
        {
            return amountOfTANEnteringStorageDaily * adjustedAmmoniaEmissionFactor;
        }


        /// <summary>
        /// Equation 4.3.5-1
        /// </summary>
        /// <param name="ammoniaEmissionsFromHousing">Ammonia emission from housing (kg NH3-N)</param>
        /// <param name="ammoniaEmissionsFromStorage">Ammonia nitrogen loss from stored manure (kg NH3-N)</param>
        /// <param name="amountOfNitrogenExcreted">Amount of N excreted (kg N)</param>
        /// <param name="amountOfNitrogenFromBedding">Bedding nitrogen (kg N)</param>
        /// <returns>Fraction of manure N volatilized as NH3 and NOx from the specific manure management system</returns>
        public double CalculateFractionOfManureVolatilized(
            double ammoniaEmissionsFromHousing,
            double ammoniaEmissionsFromStorage,
            double amountOfNitrogenExcreted,
            double amountOfNitrogenFromBedding)
        {
            var denominator = amountOfNitrogenExcreted + amountOfNitrogenFromBedding;
            if (denominator == 0)
            {
                return 0;
            }

            return (ammoniaEmissionsFromHousing + ammoniaEmissionsFromStorage) / denominator;
        }

        /// <summary>
        /// Equation 4.3.5-2
        /// Equation 5.4.3-2
        /// </summary>
        /// <param name="nitrogenExcretionRate">Amount of N excreted (kg N)</param>
        /// <param name="beddingNitrogenRate">Bedding nitrogen (kg N)</param>
        /// <param name="volatilizationFraction">Fraction of manure N volatilized as NH3 and NOx from the specific manure management system</param>
        /// <param name="volatilizationEmissionFactor">Emission factor for volatilization [kg N2O-N (kg N)^-1]</param>
        /// <returns>Manure volatilization N emission rate (kg N2O-N head^-1 day^-1)</returns>
        public double CalculateManureVolatilizationEmissionRate(
            double nitrogenExcretionRate,
            double beddingNitrogenRate,
            double volatilizationFraction,
            double volatilizationEmissionFactor)
        {
            var result = (nitrogenExcretionRate + beddingNitrogenRate) * volatilizationFraction *
                         volatilizationEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.3.4-3
        /// Equation 4.3.4-3
        /// Equation 4.3.5-3
        /// Equation 5.4.3-3
        /// </summary>
        /// <param name="volatilizationRate">Manure volatilization N emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>(kg N2O-N)</returns>
        public double CalculateManureVolatilizationNitrogenEmission(double volatilizationRate, double numberOfAnimals)
        {
            return volatilizationRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.6-1
        /// Equation 5.4.4-4
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg N2O-N head^-1 day^-1)</param>
        /// <param name="leachingFraction">Leaching fraction</param>
        /// <param name="emissionFactorForLeaching">Emission factor for leaching [kg N₂O-N (kg N)⁻¹]</param>
        /// <param name="nitrogenBeddingRate"></param>
        /// <returns>Manure leaching N emission rate (kg N₂O-N head^-1 day^-1)</returns>
        public double CalculateManureLeachingNitrogenEmissionRate(
            double nitrogenExcretionRate,
            double leachingFraction,
            double emissionFactorForLeaching,
            double nitrogenBeddingRate)
        {
            return (nitrogenExcretionRate + nitrogenBeddingRate) * leachingFraction *
                   emissionFactorForLeaching;
        }

        /// <summary>
        /// Equation 4.3.6-2
        /// Equation 5.4.4-2
        /// </summary>
        /// <param name="leachingNitrogenEmissionRate">Manure leaching N emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of cattle</param>
        /// <returns>Manure leaching N emission (kg N2O-N day^-1)</returns>
        public double CalculateManureLeachingNitrogenEmission(
            double leachingNitrogenEmissionRate,
            double numberOfAnimals)
        {
            return leachingNitrogenEmissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.6-3
        /// Equation 5.4.4-4
        /// </summary>
        /// <returns>Nitrate leached N from livestock manure during storage (kg N03-N)</returns>
        public double CalculateNitrateLeaching(
            double nitrogenExcretionRate,
            double nitrogenBeddingRate,
            double leachingFraction,
            double emissionFactorForLeaching)
        {
            return (nitrogenExcretionRate + nitrogenBeddingRate) * leachingFraction * (1 - emissionFactorForLeaching);
        }

        /// <summary>
        /// Equation 4.3.7-1
        /// Equation 5.4.5-1
        /// Equation 5.5.2-6
        /// </summary>
        /// <param name="manureVolatilizationNitrogenEmission">Manure volatilization N emission (kg N2O-N)</param>
        /// <param name="manureLeachingNitrogenEmission">Manure leaching N emission (kg N2O-N)</param>
        /// <returns>Manure indirect N2O-N emission (kg N2O-N)</returns>
        public double CalculateManureIndirectNitrogenEmission(
            double manureVolatilizationNitrogenEmission,
            double manureLeachingNitrogenEmission)
        {
            return manureVolatilizationNitrogenEmission + manureLeachingNitrogenEmission;
        }

        /// <summary>
        /// Equation 4.3.8.1
        /// Equation 5.4.6-1
        /// Equation 5.5.2-7
        /// </summary>
        /// <param name="manureDirectNitrogenEmission">Manure direct N emission (kg N₂O-N)</param>
        /// <param name="manureIndirectNitrogenEmission">Manure indirect N emission (kg N₂O-N)</param>
        /// <returns>Manure N emission (kg N₂O-N)</returns>
        public double CalculateManureNitrogenEmission(
            double manureDirectNitrogenEmission,
            double manureIndirectNitrogenEmission)
        {
            return manureDirectNitrogenEmission + manureIndirectNitrogenEmission;
        }

        /// <summary>
        /// Equation 3.1.6-10
        /// </summary>
        /// <param name="emissionsFromHousing">Total ammonia emission from beef cattle housing systems (kg NH3 year^-1)</param>
        /// <param name="emissionsFromStorage">Total ammonia emission from beef cattle manure storage systems (kg NH3 year^-1)</param>
        /// <returns>Total ammonia emission from beef cattle (kg NH3 year^-1)</returns>
        public double CalculateTotalAmmoniaEmissionsFromAnimals(
            double emissionsFromHousing,
            double emissionsFromStorage)
        {
            return emissionsFromHousing + emissionsFromStorage;
        }

        /// <summary>
        /// Equation 4.5.2-1
        /// Equation 4.5.2-11
        /// </summary>
        /// <param name="accumulatedTANEnteringStorageSystemOnDay">Adjusted amount of TAN in stored manure (kg TAN)</param>
        /// <returns>TAN available for land application (kg TAN)</returns>
        public double CalculateAccumulatedTanAvailableForLandApplication(double accumulatedTANEnteringStorageSystemOnDay)
        {
            return accumulatedTANEnteringStorageSystemOnDay;
        }

        /// <summary>
        /// Equation 4.5.2-3
        /// 
        /// Total organic N available for land application considers fecal N and bedding N as an input and the losses due to mineralization during NH3 emission and N loss as direct N2O.   
        /// </summary>
        /// <param name="fecalNitrogenExcretion">Total nitrogen excreted through feces (kg N animal^-1 day^-1)</param>
        /// <param name="beddingNitrogen">Bedding nitrogen (kg N)</param>
        /// <param name="fractionOfMineralizedNitrogen">Fraction of organic N (fecal and bedding N) that is mineralized as TAN during manure storage, dimensionless</param>
        /// <param name="directManureEmissions">Manure  N loss as direct N2O-N (kg N)</param>
        /// <param name="leachingEmissions">Manure  N loss as direct N2O-N (kg N)</param>
        /// <param name="nO3NLeachingEmissions"></param>
        /// <returns>Amount of organic N entering the pool of available manure organic N each day for each animal group and management system (kg N day^-1)</returns>
        public double CalculateOrganicNitrogenCreatedOnDay(
            double fecalNitrogenExcretion,
            double beddingNitrogen,
            double fractionOfMineralizedNitrogen,
            double directManureEmissions,
            double leachingEmissions,
            double nO3NLeachingEmissions)
        {
            var result =
                (fecalNitrogenExcretion + beddingNitrogen) - (((fecalNitrogenExcretion + beddingNitrogen) * fractionOfMineralizedNitrogen) + directManureEmissions + leachingEmissions + (nO3NLeachingEmissions));

            return result;
        }

        /// <summary>
        /// Equation 4.5.2-4 
        /// </summary>
        public double CalculateOrganicNitrogenAvailableForLandApplicationOnDay(
            double organicNitrogenAvailableOnCurrentDay,
            double organicNitrogenAvailableFromPreviousDay)
        {
            return organicNitrogenAvailableFromPreviousDay + organicNitrogenAvailableOnCurrentDay;
        }

        public void CalculateOrganicNitrogen(
            GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod,
            GroupEmissionsByDay previousDaysEmissions)
        {
            dailyEmissions.OrganicNitrogenCreatedOnDay = this.CalculateOrganicNitrogenCreatedOnDay(
                fecalNitrogenExcretion: dailyEmissions.FecalNitrogenExcretion,
                beddingNitrogen: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                fractionOfMineralizedNitrogen: managementPeriod.ManureDetails.FractionOfOrganicNitrogenMineralized,
                directManureEmissions: dailyEmissions.ManureDirectN2ONEmission,
                leachingEmissions: dailyEmissions.ManureN2ONLeachingEmission,
                nO3NLeachingEmissions: dailyEmissions.ManureNitrateLeachingEmission);

            dailyEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay = this.CalculateOrganicNitrogenAvailableForLandApplicationOnDay(
                organicNitrogenAvailableOnCurrentDay: dailyEmissions.OrganicNitrogenCreatedOnDay,
                organicNitrogenAvailableFromPreviousDay: (previousDaysEmissions == null ? 0 : previousDaysEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay));

            if (managementPeriod.HousingDetails.HousingType.IsPasture())
            {
                dailyEmissions.OrganicNitrogenCreatedOnDay = 0;
                dailyEmissions.AccumulatedOrganicNitrogenAvailableForLandApplicationOnDay = 0;
            }
        }

        /// <summary>
        /// Equation 4.5.2-6
        /// </summary>
        /// <param name="tanAvailableForLandApplication">Total ammonical nitrogen (TAN)/inorganic nitrogen in stored beef manure from different housing and storage systems (kg TAN day^-1)</param>
        /// <param name="organicNitrogenAvailableForLandApplication">Total organic nitrogen in stored beef manure from different housing and storage systems (kg N day^-1)</param>
        /// <returns>Total available manure nitrogen in stored manure (kg N day^-1)</returns>
        public double CalculateTotalAvailableManureNitrogenInStoredManure(
            double tanAvailableForLandApplication,
            double organicNitrogenAvailableForLandApplication)
        {
            return tanAvailableForLandApplication + organicNitrogenAvailableForLandApplication;
        }

        /// <summary>
        /// Equation 4.5.2-21
        /// </summary>
        /// <param name="nitrogenExcretion">Amount of total N excreted (kg N)</param>
        /// <param name="nitrogenFromBedding">Amount of N from bedding (kg N)</param>
        /// <param name="directN2ONEmission">Manure N loss as direct N2O-N (kg N)</param>
        /// <param name="ammoniaLostFromHousingAndStorage">Manure N loss as NH3-N (kg N) (volatilization and leaching/runoff)</param>
        /// <param name="leachingN2ONEmission">Manure N loss via leaching (kg N2O-N day^-1)</param>
        /// <param name="leachingNO3NEmission">Manure NO3-N loss as leaching during manure storage (kg NO3-N day^-1)</param>
        /// <returns></returns>
        public double CalculateNitrogenAvailableForLandApplicationFromSheepSwineAndOtherLivestock(
            double nitrogenExcretion,
            double nitrogenFromBedding,
            double directN2ONEmission,
            double ammoniaLostFromHousingAndStorage,
            double leachingN2ONEmission,
            double leachingNO3NEmission)
        {
            return (nitrogenExcretion + nitrogenFromBedding) -
                   (directN2ONEmission + ammoniaLostFromHousingAndStorage + leachingN2ONEmission + leachingNO3NEmission);
        }

        /// <summary>
        /// Equation 4.5.3-1
        /// </summary>
        /// <param name="carbonFromStorage">Total amount of carbon in stored manure (kg C year^-1)</param>
        /// <param name="nitrogenFromManure">Total available manure nitrogen in stored manure (kg N year^-1)</param>
        /// <returns>Carbon to nitrogen ratio of stored manure (fraction)</returns>
        public double CalculateManureCarbonToNitrogenRatio(
            double carbonFromStorage,
            double nitrogenFromManure)
        {
            if (nitrogenFromManure == 0)
            {
                return 0;
            }

            return carbonFromStorage / nitrogenFromManure;
        }

        /// <summary>
        /// Equation 4.5.3-2
        /// </summary>
        /// <param name="totalNitrogenAvailableForLandApplication">Total nitrogen available for land application (kg N)</param>
        /// <param name="nitrogenContentOfManure">Nitrogen content of manure (%)</param>
        /// <returns>Total volume of manure available for land application (1000 kg wet weight for solid manure, 1000 L for liquid manure)</returns>
        public double CalculateTotalVolumeOfManureAvailableForLandApplication(
            double totalNitrogenAvailableForLandApplication,
            double nitrogenContentOfManure)
        {
            // Prevent divide by zero
            if (nitrogenContentOfManure <= 0)
            {
                return 0;
            }

            var result = ((totalNitrogenAvailableForLandApplication * 100) / nitrogenContentOfManure) / 1000.0;

            return result;
        }

        /// <summary>
        /// Equation 5.4.3-6
        /// </summary>
        public double CalculateAdjustedAmmoniaEmissionFromGrazingAnimals(
            double nH3NFromGrazingAnimals,
            double volatilizationN2ON)
        {
            return nH3NFromGrazingAnimals - volatilizationN2ON;
        }

        /// <summary>
        /// Equation 5.4.2-1
        /// </summary>
        /// <param name="nitrogenExcretionRate">Nitrogen excretion rate of animals (kg N head^-1 day^-1)</param>
        /// <param name="volatilizationFraction">Fraction of land applied manure N lost by volatilization (kg N (kg N)^-1)</param>
        /// <returns>Ammonia N emission rate for animals grazing on pasture (kg NH3-N head^-1 day^-1) </returns>
        public double GetAmmoniaEmissionRateFromAnimalsOnPasture(
            double nitrogenExcretionRate,
            double volatilizationFraction)
        {
            return nitrogenExcretionRate * volatilizationFraction;
        }

        /// <summary>
        /// Equation 5.4.1-1
        /// </summary>
        /// <param name="temperature">Temperature (degrees C)</param>
        /// <returns>Ambient temperature adjustment for grazing ammonia emission factor</returns>
        public double GetAmbientTemperatureAdjustmentForGrazing(double temperature)
        {
            return Math.Pow(1.041, temperature) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 5.4.1-2
        /// </summary>
        /// <param name="emissionFactor">Ammonia emission factor for grazing animals</param>
        /// <param name="temperatureAdjustment">Ambient temperature adjustment</param>
        /// <returns>Adjusted NH3 emission factor for pasture grazing</returns>
        public double GetAdjustedEmissionFactorForGrazing(
            double emissionFactor,
            double temperatureAdjustment)
        {
            var result = emissionFactor * temperatureAdjustment;
            if (result > 1)
            {
                return 1;
            }
            else if (result < 0)
            {
                return 0;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Equation 5.4.1-3
        /// </summary>
        /// <param name="tanExcretionRate">TAN excretion rate of animals</param>
        /// <param name="adjustedEmissionFactor">Adjusted ammonia emission factor</param>
        /// <returns>Ammonia emission rate for animals managed on pasture</returns>
        public double GetAmmoniaEmissionRateForGrazingAnimals(
            double tanExcretionRate,
            double adjustedEmissionFactor)
        {
            return tanExcretionRate * adjustedEmissionFactor;
        }

        /// <summary>
        /// Equation 5.4.1-4
        /// Equation 5.4.2-2
        /// </summary>
        /// <param name="ammoniaEmissionRate">Ammonia emission rate for animals managed on pasture</param>
        /// <param name="numberOfAnimals">Number of animals grazing</param>
        /// <returns>Total ammonia production from animals grazing on pasture (kg NH3-N)</returns>
        public double GetTotalAmmoniaProductionFromGrazingAnimals(
            double ammoniaEmissionRate,
            double numberOfAnimals)
        {
            return ammoniaEmissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 3.1.8-8
        ///
        /// Note: volume calculation for both solid and liquid manure is the same
        /// </summary>
        /// <param name="nitrogenFromManure">Total available manure nitrogen in stored manure (kg N year^-1) </param>
        /// <param name="nitrogenConcentration">Nitrogen concentration of manure (default values for different management system; kg N per 1000 kg wet weight for solid manure and kg N per 1000 litres for liquid manure</param>
        /// <returns>Total volume of manure produced from different housing and storage systems (1000 kg wet weight for solid manure and 1000 litres for liquid manure)</returns>
        public double CalculateTotalVolumeManure(
            double nitrogenFromManure,
            double nitrogenConcentration)
        {
            if (nitrogenConcentration == 0)
            {
                return 0;
            }

            return nitrogenFromManure / nitrogenConcentration;
        }

        /// <summary>
        /// Equation 3.1.2-3
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg head^-1 day^-1)</param>
        /// <returns>Gross energy intake (MJ head^-1 day^-1)</returns>
        public double CalculateCalfGrossEnergyIntake(double dryMatterIntake)
        {
            return dryMatterIntake * 18.45;
        }

        /// <summary>
        /// Equation 4.2.1-9
        /// </summary>
        /// <param name="dryMatterIntake">Dry matter intake (kg head^-1 day^-1)</param>
        /// <param name="crudeProteinContent">Crude protein content (kg kg⁻¹) </param>
        /// <returns>Calf protein intake from solid food (kg head^-1 day^-1)</returns>
        public double CalculateCalfProteinIntakeFromSolidFood(double dryMatterIntake, double crudeProteinContent)
        {
            return dryMatterIntake * crudeProteinContent;
        }

        /// <summary>
        /// Equation 4.2.1-10
        /// </summary>
        /// <param name="milkProduction">Milk production (kg head^-1 day^-1)</param>
        /// <param name="proteinContentOfMilk">Protein content of milk (kg kg⁻¹)</param>
        /// <returns>Calf protein intake from milk (kg head^-1 day^-1)</returns>
        public double CalculateCalfProteinIntakeFromMilk(double milkProduction, double proteinContentOfMilk)
        {
            return milkProduction * proteinContentOfMilk;
        }

        /// <summary>
        /// Equation 4.2.1-13
        /// </summary>
        /// <param name="calfProteinIntakeFromMilk">Calf protein intake from milk (kg head^-1 day^-1)</param>
        /// <returns>Calf protein retained from milk (kg head^-1 day^-1)</returns>
        public double CalculateCalfProteinRetainedFromMilk(double calfProteinIntakeFromMilk)
        {
            return 0.40 * calfProteinIntakeFromMilk;
        }

        /// <summary>
        /// Equation 4.2.1-11
        /// </summary>
        /// <param name="calfProteinIntakeFromMilk">Calf protein intake from milk (kg head^-1 day^-1)</param>
        /// <param name="calfProteinIntakeFromSolidFood">Calf protein intake from solid food (kg head^-1 day^-1)</param>
        /// <param name="areMilkFedOnly"></param>
        /// <returns>Calf protein intake (kg head^-1 day^-1)</returns>
        public double CalculateCalfProteinIntake(double calfProteinIntakeFromMilk,
            double calfProteinIntakeFromSolidFood, bool areMilkFedOnly)
        {
            if (areMilkFedOnly)
            {
                return calfProteinIntakeFromMilk;
            }
            else
            {
                return calfProteinIntakeFromMilk + calfProteinIntakeFromSolidFood;
            }
        }

        /// <summary>
        /// Equation 4.2.1-15
        /// </summary>
        /// <param name="calfProteinIntake">Calf protein intake (kg head^-1 day^-1)</param>
        /// <param name="calfProteinRetained">Protein retained (kg head^-1 day^-1)</param>
        /// <returns>N excretion rate (kg head^-1 day^-1)</returns>
        public double CalculateCalfNitrogenExcretionRate(double calfProteinIntake, double calfProteinRetained)
        {
            var a = calfProteinIntake / 6.25;
            var b = calfProteinRetained / 6.25;

            return a - b;
        }

        /// <summary>
        /// Equation 4.2.1-14
        /// </summary>
        /// <param name="calfProteinRetainedFromMilk">Calf protein retained from milk (kg head^-1 day^-1)</param>
        /// <param name="calfProteinRetainedFromSolidFeed">Calf protein retained from solid feed (kg head^-1 day^-1)</param>
        /// <returns>Protein retained (kg head^-1 day^-1)</returns>
        public double CalculateCalfProteinRetained(
            double calfProteinRetainedFromMilk,
            double calfProteinRetainedFromSolidFeed)
        {
            return calfProteinRetainedFromMilk + calfProteinRetainedFromSolidFeed;
        }

        /// <summary>
        /// Equation 4.2.1-12
        /// </summary>
        /// <param name="calfProteinIntakeFromSolidFood">Calf protein intake from solid food (kg head^-1 day^-1)</param>
        /// <returns>Calf protein retained from solid feed (kg head^-1 day^-1)</returns>
        public double CalculateCalfProteinRetainedFromSolidFeed(double calfProteinIntakeFromSolidFood)
        {
            return 0.20 * calfProteinIntakeFromSolidFood;
        }

        /// <summary>
        /// Note: calculation is the same for both liquid and solid manure
        ///
        /// In v3 the emissions from manure application were separated into emissions from solid manure and emissions from liquid manure. In v4 Aklilu updated
        /// the manure composition information so that specific emissions can be calculated from each different type of manure handling system.
        ///
        /// Assumed that all available manure created in the month is applied to a field - must therefore always calculate CO2 spreading emissions
        /// </summary>
        /// <param name="volumeOfManure">Volume of manure (1000 litres)</param>
        /// <returns>CO2 emissions from liquid manure spreading</returns>
        public double CalculateCarbonDioxideEmissionsFromManureSpreading(double volumeOfManure)
        {
            const double DieselConversion = 70; // Same for both liquid & solid manure
            const double ManureConversion = 0.0248; // Same for both liquid & solid manure

            return volumeOfManure * ManureConversion * DieselConversion;
        }

        /// <summary>
        /// Equation 3.4.1-1
        /// </summary>
        /// <param name="entericMethaneEmissionRate">Enteric CH4 emission (kg CH4 year^-1)</param>
        /// <param name="numberOfAnimals"></param>
        /// <returns>Enteric CH4 Emissions (kg CH4 day^-1) </returns>
        public double CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(
            double entericMethaneEmissionRate,
            double numberOfAnimals)
        {
            return (entericMethaneEmissionRate / 365) * numberOfAnimals;
        }

        /// <summary>
        /// Equation 11.3.1-5
        /// </summary>
        /// <param name="finalWeightOfAnimal">Final weight of animal (kg animal^-1)</param>
        /// <param name="animalType">The type of animal</param>
        /// <returns>The maximum dry matter intake (kg head day^-1)</returns>
        public double CalculateDryMatterMax(double finalWeightOfAnimal, AnimalType animalType)
        {
            var intakeLimit = 0d;

            if (animalType.IsBeefCattleType())
            {
                intakeLimit = 0.0225;
            }
            else if (animalType.IsDairyCattleType())
            {
                intakeLimit = 0.04;
            }
            else
            {
                intakeLimit = 1.0;
            }

            var weightLimit = finalWeightOfAnimal * intakeLimit;

            return weightLimit;
        }

        public bool IsOverDmiMax(GroupEmissionsByDay groupEmissionsByDay)
        {
            // Add in additional 15%
            return groupEmissionsByDay.DryMatterIntake > (groupEmissionsByDay.DryMatterIntakeMax);
        }

        /// <summary>
        /// Equation 3.1.1.1
        /// Equation 3.1.2-1
        /// Equation 3.2.1-1
        /// Equation 3.3.1-1
        /// </summary>
        public double GetCurrentAnimalWeight(
            double startWeight,
            double averageDailyGain,
            DateTime startDate,
            DateTime currentDate)
        {
            var totalDaysElapsed = currentDate.Subtract(startDate).TotalDays + 1;

            var result = totalDaysElapsed * averageDailyGain + startWeight;

            return result;
        }

        /// <summary>
        /// Calculate the total daily carbon uptake.
        /// </summary>
        /// <param name="totalDailyDryMatterIntakeForGroup">Total dry matter intake for all animals in the group.</param>
        /// <returns>Total carbon uptake by the group of animals (kg C day^-1)</returns>
        public double CalculateDailyCarbonUptakeForGroup(
            double totalDailyDryMatterIntakeForGroup)
        {
            var result = totalDailyDryMatterIntakeForGroup * CoreConstants.CarbonConcentration;

            return result;
        }

        public double CalculateAdjustedAmmoniaFromHousing(
            GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod)
        {

            dailyEmissions.VolatilizationFractionForHousing = CalculateVolatalizationFractionForHousing(
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                dailyAmmoniaEmissionsFromHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            dailyEmissions.VolatilizationEmissionsFromHousing = CalculateAmmoniaVolatilizationFromHousing(
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFractionFromHousing: dailyEmissions.VolatilizationFractionForHousing,
                emissionFactorForVolatilization: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            var ammoniaHousingAdjustment = CalculateAmmoniaHousingAdjustment(
                ammoniaFromHousing: dailyEmissions.AmmoniaConcentrationInHousing,
                ammoniaVolatilizedDuringHousing: dailyEmissions.VolatilizationEmissionsFromHousing);

            var result = ammoniaHousingAdjustment;

            return result;
        }

        /// <summary>
        /// Equation 4.3.5-4
        /// </summary>
        /// <param name="dailyAmmoniaEmissionsFromHousing">Daily NH3-N emissions during housing of beef cattle (confined-no barn (feedlot), barn), dairy cattle (tie-stall barns, free-stall barns, milking parlours, yards/exercise lots and pasture), and broilers, layers and turkeys (kg NH3-N day^-1)</param>
        /// <param name="amountOfNitrogenExcreted">Total amount of N excreted by beef or dairy cattle or broilers, layers or turkeys (kg N day^-1)</param>
        /// <param name="amountOfNitrogenFromBedding">Total amount of N added from bedding materials (kg N day^-1)</param>
        /// <returns>Fraction of manure N volatilized as NH3 and NOx during the housing stage</returns>
        public double CalculateVolatalizationFractionForHousing(
            double dailyAmmoniaEmissionsFromHousing,
            double amountOfNitrogenExcreted,
            double amountOfNitrogenFromBedding)
        {
            return dailyAmmoniaEmissionsFromHousing / (amountOfNitrogenExcreted + amountOfNitrogenFromBedding);
        }

        /// <summary>
        /// Equation 4.3.5-5
        /// </summary>
        /// <param name="amountOfNitrogenExcreted">Total amount of N excreted by beef or dairy cattle or broilers, layers or turkeys (kg N day^-1)</param>
        /// <param name="amountOfNitrogenFromBedding">Total amount of N added from bedding materials (kg N day^-1)</param>
        /// <param name="volatilizationFractionFromHousing">Fraction of manure N volatilized as NH3 and NOx during the housing stage</param>
        /// <param name="emissionFactorForVolatilization">Emission factor for volatilization [kg N2O-N (kg N)^-1]</param>
        /// <returns>Manure volatilization N emissions during the housing stage (kg N2O-N day^-1)</returns>
        public double CalculateAmmoniaVolatilizationFromHousing(
            double amountOfNitrogenExcreted,
            double amountOfNitrogenFromBedding,
            double volatilizationFractionFromHousing,
            double emissionFactorForVolatilization)
        {
            return (amountOfNitrogenExcreted + amountOfNitrogenFromBedding) * volatilizationFractionFromHousing *
                   emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 4.3.5-6
        /// </summary>
        /// <param name="ammoniaFromHousing">Daily NH3-N emissions during housing of beef cattle (confined-no barn (feedlot), barn), dairy cattle (tie-stall barns, free-stall barns, milking parlours, yards/exercise lots and pasture), and broilers, layers and turkeys (kg NH3-N day^-1)</param>
        /// <param name="ammoniaVolatilizedDuringHousing">Manure volatilization N emissions during the housing stage (kg N2O-N day^-1)</param>
        /// <returns>Adjusted daily NH3-N emissions from housing (kg NH3-N day-1)</returns>
        public double CalculateAmmoniaHousingAdjustment(
            double ammoniaFromHousing,
            double ammoniaVolatilizedDuringHousing)
        {
            return ammoniaFromHousing - ammoniaVolatilizedDuringHousing;
        }

        /// <summary>
        /// Equation 4.3.5-7
        /// </summary>
        /// <param name="dailyAmmoniaEmissionsFromStorage">Daily NH3-N emissions during storage of manure (kg NH3-N day^-1)</param>
        /// <param name="amountOfNitrogenExcreted">Total amount of N excreted by beef or dairy cattle or broilers, layers or turkeys (kg N day^-1)</param>
        /// <param name="amountOfNitrogenFromBedding">Total amount of N added from bedding materials (kg N day^-1)</param>
        /// <returns>Fraction of manure N volatilized as NH3 and NOx during the manure storage stage</returns>
        public double CalculateVolatilizationFractionForStorage(
            double dailyAmmoniaEmissionsFromStorage,
            double amountOfNitrogenExcreted,
            double amountOfNitrogenFromBedding)
        {
            return dailyAmmoniaEmissionsFromStorage / (amountOfNitrogenExcreted + amountOfNitrogenFromBedding);
        }

        /// <summary>
        /// Equation 4.3.5-8
        /// </summary>
        /// <param name="amountOfNitrogenExcreted">Total amount of N excreted by beef or dairy cattle or broilers, layers or turkeys (kg N day^-1)</param>
        /// <param name="amountOfNitrogenFromBedding">Total amount of N added from bedding materials (kg N day^-1)</param>
        /// <param name="volatilizationFractionFromStorage">Fraction of manure N volatilized as NH3 and NOx during the housing stage</param>
        /// <param name="emissionFactorForVolatilization">Emission factor for volatilization [kg N2O-N (kg N)^-1]</param>
        /// <returns>Manure volatilization N emissions during the housing stage (kg N2O-N day^-1)</returns>
        public double CalculateAmmoniaVolatilizationFromStorage(
            double amountOfNitrogenExcreted,
            double amountOfNitrogenFromBedding,
            double volatilizationFractionFromStorage,
            double emissionFactorForVolatilization)
        {
            return (amountOfNitrogenExcreted + amountOfNitrogenFromBedding) * volatilizationFractionFromStorage *
                   emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 4.3.5-9
        /// </summary>
        /// <param name="ammoniaFromStorage">Daily NH3-N emissions during housing of beef cattle (confined-no barn (feedlot), barn), dairy cattle (tie-stall barns, free-stall barns, milking parlours, yards/exercise lots and pasture), and broilers, layers and turkeys (kg NH3-N day^-1)</param>
        /// <param name="ammoniaVolatilizedDuringStorage">Manure volatilization N emissions during the housing stage (kg N2O-N day^-1)</param>
        /// <returns>Adjusted daily NH3-N emissions from housing (kg NH3-N day-1)</returns>
        public double CalculateAmmoniaStorageAdjustment(
            double ammoniaFromStorage,
            double ammoniaVolatilizedDuringStorage)
        {
            return ammoniaFromStorage - ammoniaVolatilizedDuringStorage;
        }

        #endregion

        /// <summary>
        /// There will be one of these values for each day, in the view model, get the largest value and show this to user so that the optimal TDN value is for all days over management period...
        /// </summary>
        public double CalculateRequiredTdnSoThatMaxDmiIsNotExceeded(
            double netEnergyForMaintenance,
            double netEnergyForActivity,
            double netEnergyForLactation,
            double netEnergyForPregnancy,
            double netEnergyForGain,
            double ratioOfEnergyForMaintenance,
            double ratioOfEnergyForGain,
            double currentTdn,
            double currentDmiMax)
        {
            var result = 0.0;

            var tdnStep = 0.01;
            var targetTdnFound = false;
            for (var tdn = currentTdn; tdn <= 100 && targetTdnFound == false; tdn += tdnStep)
            {
                // Keep calculating a new GEI until the DMI is brought down to a level that is lower than the DMI_max
                var grossEnergyIntake = CalculateGrossEnergyIntake(
                    netEnergyForMaintenance: netEnergyForMaintenance,
                    netEnergyForActivity: netEnergyForActivity,
                    netEnergyForLactation: netEnergyForLactation,
                    netEnergyForPregnancy: netEnergyForPregnancy,
                    netEnergyForGain: netEnergyForGain,
                    ratioOfEnergyAvailableForMaintenance: ratioOfEnergyForMaintenance,
                    ratioOfEnergyAvailableForGain: ratioOfEnergyForGain,
                    percentTotalDigestibleNutrientsInFeed: tdn);

                var dmiAtThisTdn = CalculateDryMatterIntake(
                    grossEnergyIntake: grossEnergyIntake);

                // Check if this new dmi is less than dmi max
                var isLessThanDmiMax = dmiAtThisTdn < currentDmiMax;
                if (isLessThanDmiMax)
                {
                    targetTdnFound = true;
                    result = tdn;
                }
            }

            return result;
        }

        public double CalculateAdjustedAmmoniaFromStorage(
            GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod)
        {
            dailyEmissions.VolatilizationForStorage = CalculateVolatilizationFractionForStorage(
                dailyAmmoniaEmissionsFromStorage: dailyEmissions.AmmoniaLostFromStorage,
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding);

            dailyEmissions.AmmoniaLossFromStorage = CalculateAmmoniaVolatilizationFromStorage(
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFractionFromStorage: dailyEmissions.VolatilizationForStorage,
                emissionFactorForVolatilization: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            var adjustedAmmoniaLossFromStorage = CalculateAmmoniaStorageAdjustment(
                ammoniaFromStorage: dailyEmissions.AmmoniaLostFromStorage,
                ammoniaVolatilizedDuringStorage: dailyEmissions.AmmoniaLossFromStorage);

            var totalAdjustedAmmoniaLossesFromStorage = CoreConstants.ConvertToNH3(
                amountOfNH3N: adjustedAmmoniaLossFromStorage);

            return adjustedAmmoniaLossFromStorage;
        }

        /// <summary>
        /// Calculates ammonia in housing for beef, dairy, and poultry.
        /// </summary>
        public void CalculateAmmoniaInHousing(
            GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod,
            double adjustedEmissionFactorForHousing)
        {
            dailyEmissions.AmmoniaEmissionRateFromHousing = CalculateAmmoniaEmissionRateFromHousing(
                tanExcretionRate: dailyEmissions.TanExcretionRate,
                adjustedEmissionFactor: adjustedEmissionFactorForHousing);

            dailyEmissions.AmmoniaConcentrationInHousing = CalculateAmmoniaConcentrationInHousing(
                emissionRate: dailyEmissions.AmmoniaEmissionRateFromHousing,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.AmmoniaEmissionsFromHousingSystem = CoreConstants.ConvertToNH3(dailyEmissions.AmmoniaConcentrationInHousing);

            dailyEmissions.AdjustedNH3NFromHousing = this.CalculateAdjustedAmmoniaFromHousing(dailyEmissions, managementPeriod);
        }

        /// <summary>
        /// Calculate ammonia emissions from housing/storage for sheep, swine, and other livestock.
        /// </summary>
        public void CalculateIndirectEmissionsFromHousingAndStorage(GroupEmissionsByDay dailyEmissions, ManagementPeriod managementPeriod)
        {
            dailyEmissions.FractionOfManureVolatilized = managementPeriod.ManureDetails.VolatilizationFraction;

            /*
             * Ammonia emissions
             */

            dailyEmissions.AmmoniaEmissionRateFromHousingAndStorage = this.CalculateAmmoniaEmissionRateFromHousingAndStorage(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                rateOfNitrogenAddedFromBedding: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized);

            dailyEmissions.TotalNitrogenLossesFromHousingAndStorage = this.CalculateTotalNitrogenLossFromHousingAndStorage(
                ammoniaEmissionRate: dailyEmissions.AmmoniaEmissionRateFromHousingAndStorage,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.AmmoniaEmissionsFromHousingAndStorage = dailyEmissions.TotalNitrogenLossesFromHousingAndStorage;

            /*
             * Volatilization
             */

            this.CalculateVolatilizationEmissions(dailyEmissions, managementPeriod);

            /*
             * Ammonia adjustments
             */

            dailyEmissions.AdjustedTotalNitrogenEmissionsFromHousingAndStorage = this.CalculateAmmoniaAdjustmentFromHousingAndStorage(
                totalAmmoniaLossFromHousingAndStorage: dailyEmissions.TotalNitrogenLossesFromHousingAndStorage,
                manureVolatilizationEmissions: dailyEmissions.ManureVolatilizationN2ONEmission);

            dailyEmissions.AdjustedAmmoniaEmissionsFromHousingAndStorage = CoreConstants.ConvertToNH3(
                amountOfNH3N: dailyEmissions.AdjustedTotalNitrogenEmissionsFromHousingAndStorage);

            /*
             * Leaching
             */

            this.CalculateLeachingEmissions(dailyEmissions, managementPeriod);

            /*
             * Indirect total
             */

            dailyEmissions.ManureIndirectN2ONEmission = this.CalculateManureIndirectNitrogenEmission(
                manureVolatilizationNitrogenEmission: dailyEmissions.ManureVolatilizationN2ONEmission,
                manureLeachingNitrogenEmission: dailyEmissions.ManureN2ONLeachingEmission);
        }

        protected void CalculateVolatilizationEmissions(
            GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod)
        {
            dailyEmissions.ManureVolatilizationRate = CalculateManureVolatilizationEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                beddingNitrogenRate: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial,
                volatilizationFraction: dailyEmissions.FractionOfManureVolatilized,
                volatilizationEmissionFactor: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            dailyEmissions.ManureVolatilizationN2ONEmission = CalculateManureVolatilizationNitrogenEmission(
                volatilizationRate: dailyEmissions.ManureVolatilizationRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);
        }

        protected void CalculateLeachingEmissions(
            GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod)
        {
            dailyEmissions.ManureNitrogenLeachingRate = CalculateManureLeachingNitrogenEmissionRate(
                nitrogenExcretionRate: dailyEmissions.NitrogenExcretionRate,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching,
                nitrogenBeddingRate: dailyEmissions.RateOfNitrogenAddedFromBeddingMaterial);

            dailyEmissions.ManureN2ONLeachingEmission = CalculateManureLeachingNitrogenEmission(
                leachingNitrogenEmissionRate: dailyEmissions.ManureNitrogenLeachingRate,
                numberOfAnimals: managementPeriod.NumberOfAnimals);

            dailyEmissions.ManureNitrateLeachingEmission = CalculateNitrateLeaching(
                nitrogenExcretionRate: dailyEmissions.AmountOfNitrogenExcreted,
                nitrogenBeddingRate: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                leachingFraction: managementPeriod.ManureDetails.LeachingFraction,
                emissionFactorForLeaching: managementPeriod.ManureDetails.EmissionFactorLeaching);
        }

        protected void InitializeDailyEmissions(
            GroupEmissionsByDay dailyEmissions,
            ManagementPeriod managementPeriod,
            Farm farm,
            DateTime dateTime)
        {
            dailyEmissions.DateTime = dateTime;
            var temperature = farm.ClimateData.GetMeanTemperatureForDay(dateTime);
            dailyEmissions.Temperature = temperature;
            dailyEmissions.NumberOfAnimals = managementPeriod.NumberOfAnimals;
            dailyEmissions.EmissionFactorForVolatilization =
                managementPeriod.ManureDetails.EmissionFactorVolatilization;
            dailyEmissions.EmissionFactorForLeaching = managementPeriod.ManureDetails.EmissionFactorLeaching;
            dailyEmissions.LeachingFraction = managementPeriod.ManureDetails.LeachingFraction;
        }

        /// <summary>
        /// Equation 4.3.2-8
        /// Equation 4.3.3-11
        /// </summary>
        /// <param name="amountOfTANFlowingIntoStorageEachDay"></param>
        /// <param name="adjustedAmmoniaLossFromStorage"></param>
        /// <returns>Amount of TAN entering the storage system each day (minus housing and storage NH3-N emissions) in broiler, layer and turkey manure (kg TAN day-1), by animal group and manure management system</returns>
        public double CalculateAdjustedAmountOfTANEnteringStorage(
            double amountOfTANFlowingIntoStorageEachDay,
            double adjustedAmmoniaLossFromStorage)
        {
            return amountOfTANFlowingIntoStorageEachDay - adjustedAmmoniaLossFromStorage;
        }

        /// <summary>
        /// Checks if daily calculations are from the same year.
        /// </summary>
        private bool HasMovedToNewYear(DateTime currentDate)
        {
            var previousDate = currentDate.Subtract(TimeSpan.FromDays(1));
            if (previousDate.Year != currentDate.Year)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}