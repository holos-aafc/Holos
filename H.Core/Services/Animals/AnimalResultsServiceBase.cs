using System;
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
using H.Core.Providers.AnaerobicDigestion;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Energy;
using H.Core.Tools;

namespace H.Core.Services.Animals
{
    public abstract class AnimalResultsServiceBase : IAnimalResultsService
    {
        #region Fields

        protected readonly Table_48_Parameter_Adjustments_For_Manure_Provider _parameterAdjustmentsForManureProvider = new Table_48_Parameter_Adjustments_For_Manure_Provider();
        protected readonly Table_52_Electricity_Conversion_Defaults_Provider _energyConversionDefaultsProvider = new Table_52_Electricity_Conversion_Defaults_Provider();
        protected readonly Table_46_Beef_Dairy_Default_Emission_Factors_Provider _beefDairyDefaultEmissionFactorsProvider = new Table_46_Beef_Dairy_Default_Emission_Factors_Provider();
        protected readonly Table_47_Fraction_OrganicN_Mineralized_As_Tan_Provider _fractionOrganicNMineralizedAsTanProvider = new Table_47_Fraction_OrganicN_Mineralized_As_Tan_Provider();
        protected IAdditiveReductionFactorsProvider AdditiveReductionFactorsProvider = new Table_22_Additive_Reduction_Factors_Provider();
        protected readonly ADCalculator _aDCalculator = new ADCalculator();
        protected readonly SingleYearNitrousOxideCalculator _singleYearNitrousOxideCalculator = new SingleYearNitrousOxideCalculator();
        protected readonly Table_49_Biogas_Methane_Production_Parameters_Provider _biogasMethaneProductionParametersProvider = new Table_49_Biogas_Methane_Production_Parameters_Provider();
        protected readonly Table_50_Solid_Liquid_Separation_Coefficients_Provider _solidLiquidSeparationCoefficientsProvider = new Table_50_Solid_Liquid_Separation_Coefficients_Provider();
        protected readonly Table_39_Livestock_Emission_Conversion_Factors_Provider _livestockEmissionConversionFactorsProvider = new Table_39_Livestock_Emission_Conversion_Factors_Provider();

        protected IAnimalComponentHelper AnimalComponentHelper = new AnimalComponentHelper();

        /// <summary>
        /// Calculations are expensive, cache results if they have already been calculated. Recalculate if component is 'dirty' i.e. has <see cref="ComponentBase.ResultsCalculated"/> flag set to false
        /// </summary>
        protected Dictionary<AnimalComponentBase, IList<AnimalGroupEmissionResults>> _cachedComponentListResults = new Dictionary<AnimalComponentBase, IList<AnimalGroupEmissionResults>>();

        protected ComponentCategory _animalComponentCategory;

        #endregion

        #region Constructors

        protected AnimalResultsServiceBase()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public List<AnimalComponentEmissionsResults> CalculateResultsForAnimalComponents(
            IEnumerable<AnimalComponentBase> components, Farm farm)
        {
            var animalComponentEmissionResults = new List<AnimalComponentEmissionsResults>();

            foreach (var component in components)
            {
                var result = this.CalculateComponentEmissionResults(component, farm);
                animalComponentEmissionResults.Add(result);
            }

            return animalComponentEmissionResults;
        }

        public virtual IList<AnimalGroupEmissionResults> CalculateResultsForComponent(
            AnimalComponentBase animalComponent,
            Farm farm)
        {
            // Don't calculate results for a component that has not been initialized yet since calculations are expensive and should only be done once all properties on a component have been set (i.e. has been initialized)
            if (animalComponent.IsInitialized == false)
            {
                return new List<AnimalGroupEmissionResults>();
            }

            // Results for this component have been calculated already, return cached results
            if (animalComponent.ResultsCalculated == true && _cachedComponentListResults.ContainsKey(animalComponent))
            {
                Trace.TraceInformation($"{nameof(BeefCattleResultsService)}.{nameof(AnimalResultsServiceBase.CalculateResultsForComponent)}: results already calculated for {animalComponent.Name}, returning cached results.");

                return _cachedComponentListResults[animalComponent];
            }

            Trace.TraceInformation($"{nameof(BeefCattleResultsService)}.{nameof(AnimalResultsServiceBase.CalculateResultsForComponent)}: calculating emissions for {animalComponent.Name}.");

            var animalGroupEmissionResults = new List<AnimalGroupEmissionResults>();

            // Loop over all the animal groups in this animal component
            foreach (var animalGroup in animalComponent.Groups)
            {
                var animalGroupEmissionResult = new AnimalGroupEmissionResults();
                animalGroupEmissionResult.AnimalGroup = animalGroup;

                foreach (var managementPeriod in animalGroup.ManagementPeriods)
                {
                    var monthlyBreakdownForManagementPeriod = this.AnimalComponentHelper.GetMonthlyBreakdownFromManagementPeriod(managementPeriod);
                    foreach (var month in monthlyBreakdownForManagementPeriod)
                    {
                        month.AnimalGroup = animalGroup;
                        month.ManagementPeriod = managementPeriod;

                        Trace.TraceInformation($"{nameof(AnimalResultsServiceBase)} calculating emissions for {month}.");

                        var dailyEmissionsForMonth = new List<GroupEmissionsByDay>();

                        var startDate = month.StartDate;
                        var endDate = month.EndDate;

                        for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
                        {
                            var previousDate = currentDate.AddDays(-1);
                            var groupEmissionsForPreviousDay = dailyEmissionsForMonth.SingleOrDefault(x => x.DateTime.Date.Equals(previousDate.Date));

                            var groupEmissionsForDay = this.CalculateDailyEmissions(
                                animalComponentBase: animalComponent,
                                managementPeriod: managementPeriod,
                                dateTime: currentDate,
                                previousDaysEmissions: groupEmissionsForPreviousDay,
                                animalGroup: animalGroup,
                                farm: farm);

                            dailyEmissionsForMonth.Add(groupEmissionsForDay);
                        }

                        var groupEmissionsByMonth = new GroupEmissionsByMonth(month, dailyEmissionsForMonth);

                        this.CalculateEnergyEmissions(groupEmissionsByMonth, farm);
                        this.CalculateEstimatesOfProduction(groupEmissionsByMonth, farm);

                        animalGroupEmissionResult.GroupEmissionsByMonths.Add(groupEmissionsByMonth);
                    }
                }

                animalGroupEmissionResults.Add(animalGroupEmissionResult);
            }

            // Cache the calculations for this animal component
            _cachedComponentListResults[animalComponent] = animalGroupEmissionResults;

            // Results for this component have been calculated and should be reused if there are no changes made to the component
            animalComponent.ResultsCalculated = true;

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

            var animalGroupEmissionResults = this.CalculateResultsForComponent(component, farm);
            foreach (var emissionsByGroup in animalGroupEmissionResults)
            {
                animalComponentEmissionResults.EmissionResultsForAllAnimalGroupsInComponent.Add(emissionsByGroup);
            }

            return animalComponentEmissionResults;
        }

        #endregion

        #region Protected Methods

        protected virtual void CalculateEnergyEmissions(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm) { }

        protected virtual void CalculateEstimatesOfProduction(GroupEmissionsByMonth groupEmissionsByMonth, Farm farm) { }

        protected abstract GroupEmissionsByDay CalculateDailyEmissions(
            AnimalComponentBase animalComponentBase,
            ManagementPeriod managementPeriod,
            DateTime dateTime,
            GroupEmissionsByDay previousDaysEmissions,
            AnimalGroup animalGroup,
            Farm farm);

        protected void CaclulateADResults(
            Farm farm,
            GroupEmissionsByDay groupEmissionsByDay,
            ManagementPeriod managementPeriod)
        {
            var component = farm.Components.OfType<AnaerobicDigestionComponent>().SingleOrDefault();
            if (component == null)
            {
                // This farm doesn't have an AD
                return;
            }
        }

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
        /// <returns>The dry matter intake of animals (kg head^-1 day^-1)</returns>
        public double CalculateDryMatterIntakeForCalves(
            double dietaryNetEnergyConcentration,
            double weight)
        {
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
        /// <param name="averageMonthlyTemperature">Average monthly temperature (upper limit = 20, temperatures above 20ºC or cattle are house in a barn, use 20ºC)</param>
        /// <returns>Maintenance coefficient – adjusted for temperature (MJ day^-1 kg^-1)</returns>
        public double CalculateTemperatureAdjustedMaintenanceCoefficient(
            double baselineMaintenanceCoefficient,
            double averageMonthlyTemperature)
        {
            var adjustedMaintenanceCoefficient =
                baselineMaintenanceCoefficient + 0.0048 * (20 - averageMonthlyTemperature);

            return adjustedMaintenanceCoefficient;
        }

        /// <summary>
        /// Equation 3.1.1-3
        /// Equation 3.2.1-2
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
        /// Equation 3.2.1-4
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
        public double CalculateEntericMethaneEmissionRate(double grossEnergyIntake,
                                                          double methaneConversionFactor,
                                                          double additiveReductionFactor)
        {
            return grossEnergyIntake * (methaneConversionFactor / 55.65) * (1.0 - additiveReductionFactor / 100.0);
        }

        /// <summary>
        /// Equation 3.1.1-13
        /// Equation 3.2.1-12
        /// Equation 3.3.1-14
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
        /// <param name="drymMatterIntakeForGroup"></param>
        /// <param name="numberOfDaysInManagementPeriod"></param>
        /// <returns></returns>
        public double CalculateDryMatterIntakeForManagementPeriod(
            double drymMatterIntakeForGroup,
            double numberOfDaysInManagementPeriod)
        {
            return drymMatterIntakeForGroup * numberOfDaysInManagementPeriod;
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
        /// </summary>
        /// <param name="manureExcretionRate">Volume of manure excreted daily (kg head^-1 day^-1)</param>
        /// <param name="carbonFractionOfManure">Fraction of carbon in the manure (% wet weight)</param>
        /// <returns>Rate of C excreted through feces (kg head^-1 day^-1)</returns>
        public double CalculateFecalCarbonExcretionRateForSheepPoultryAndOtherLivestock(
            double manureExcretionRate,
            double carbonFractionOfManure)
        {
            var result = manureExcretionRate * carbonFractionOfManure;

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
        /// Equation 4.1.1-5
        /// </summary>
        /// <param name="beddingRate">Rate of bedding material added (kg head^-1 day^-1)</param>
        /// <param name="carbonConcentrationOfBeddingMaterial">Carbon concentration of bedding material (kg C kg^-1 DM)</param>
        /// <param name="moistureContentOfBeddingMaterial">Moisture content of bedding material (%)</param>
        /// <returns>Rate of carbon added from bedding material (kg C head^-1 day^-1)</returns>
        public double CalculateRateOfCarbonAddedFromBeddingMaterial(double beddingRate,
            double carbonConcentrationOfBeddingMaterial,
            double moistureContentOfBeddingMaterial)
        {
            return beddingRate * carbonConcentrationOfBeddingMaterial * (1 - (moistureContentOfBeddingMaterial / 100.0));
        }

        /// <summary>
        /// Equation 4.1.1-6
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
        /// Equation 4.1.1-7
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
        /// <returns>Volatile solids (kg head^-1 day^-1)</returns>
        public double CalculateVolatileSolids(double grossEnergyIntake,
                                              double percentTotalDigestibleNutrientsInFeed,
                                              double ashContentOfFeed)
        {
            var a = grossEnergyIntake * (1.0 - percentTotalDigestibleNutrientsInFeed / 100.0) +
                    0.04 * grossEnergyIntake;
            var b = 1.0 - ashContentOfFeed / 100.0;
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
            return volatileSolidsProduced * 0.45;
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
        /// </summary>
        /// <param name="degreesCelsius">Air temperature (degrees Celsius)</param>
        /// <returns>Air temperature (degrees Kelvin)</returns>
        public double CalculateDegreeKelvin(
            double degreesCelsius)
        {
            return degreesCelsius + 273.15;
        }

        /// <summary>
        /// Equation 4.1.3-9
        /// </summary>
        /// <param name="kelvinAirTemperature">Air temperature (degrees Kelvin)</param>
        /// <returns>Air temperature (degrees Kelvin)</returns>
        public double CalculateClimateFactor(
            double kelvinAirTemperature)
        {
            const double t1 = 303.16;

            double numerator = 15175 * (kelvinAirTemperature - t1);
            double denominator = 1.987 * kelvinAirTemperature * t1;

            return Math.Exp(numerator / denominator);
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
        /// <param name="monthlyFecalCarbonExcretion">Amount of C excreted (kg C)</param>
        /// <param name="monthlyAmountOfCarbonFromBedding">Amount of carbon added from bedding materials (kg C)</param>
        /// <param name="monthlyAmountOfCarbonLostAsMethaneDuringManagement">Carbon lost as methane during manure management (kg C)</param>
        /// <returns>Amount of C in stored manure (kg C)</returns>
        public double CalculateAmountOfCarbonInStoredManure(
            double monthlyFecalCarbonExcretion,
            double monthlyAmountOfCarbonFromBedding,
            double monthlyAmountOfCarbonLostAsMethaneDuringManagement)
        {
            var result = (monthlyFecalCarbonExcretion + monthlyAmountOfCarbonFromBedding) -
                         monthlyAmountOfCarbonLostAsMethaneDuringManagement;

            return result;
        }

        /// <summary>
        /// Equation 4.2.1-30
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
            return beddingRate * nitrogenConcentrationOfBeddingMaterial * (1 - (moistureContentOfBeddingMaterial / 100.0));
        }

        /// <summary>
        /// Equation 4.2.1-31
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
        /// <returns>Protein retained for lactation (kg head^-1 day^-1)</returns>
        public double CalculateProteinRetainedForLactation(
            double milkProduction,
            double proteinContentOfMilk,
            double numberOfYoungAnimals,
            double numberOfAnimals)
        {
            if (Math.Abs(numberOfAnimals) < Double.Epsilon)
            {
                return 0;
            }

            return milkProduction * proteinContentOfMilk * numberOfYoungAnimals / numberOfAnimals;
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
        /// Equation 4.3.1-3, 4.3.4-1
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="fractionOfNitrogenExcretedInUrine">Fraction of N excreted in urine (urinary-N or urea-N fraction), varied with diet CP content (kg N kg^-1 total N excreted)</param>
        /// <returns>Total ammonical nitrogen (TAN) excretion rate (kg TAN head^-1 day^-1)</returns>
        public double CalculateTANExcretionRate(double nitrogenExcretionRate, double fractionOfNitrogenExcretedInUrine)
        {
            return nitrogenExcretionRate * fractionOfNitrogenExcretedInUrine;
        }

        /// <summary>
        /// Equation 4.3.1-4, 4.3.4-2
        /// </summary>
        /// <param name="tanExcretionRate">Total Ammonical nitrogen (TAN) excretion rate (kg TAN head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Total ammonical nitrogen (TAN) excretion (kg TAN animal^-1 day^-1)</returns>
        public double CalculateTANExcretion(double tanExcretionRate, double numberOfAnimals)
        {
            return tanExcretionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.1-5, 4.3.4-3
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="tanExcretionRate">Fraction of N excreted in urine (urinary-N or urea-N fraction), varied with diet CP content (kg N kg^-1 total N excreted)</param>
        /// <returns>Fecal N excretion rate (kg N head^-1 d^-1)</returns>
        public double CalculateFecalNitrogenExcretionRate(
            double nitrogenExcretionRate, 
            double tanExcretionRate)
        {
            return nitrogenExcretionRate * (1 - tanExcretionRate);
        }

        /// <summary>
        /// Equation 4.3.1-6, 4.3.4-4
        /// </summary>
        /// <param name="fecalNitrogenExcretionRate">Fecal N excretion rate (kg N head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns>Total nitrogen excreted through feces (kg N day^-1)</returns>
        public double CalculateFecalNitrogenExcretion(double fecalNitrogenExcretionRate, double numberOfAnimals)
        {
            return fecalNitrogenExcretionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.1-7, 4.3.4-5
        ///
        /// Organic N in stored manure
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
        /// <returns>Total manure N losses via NH3 volatilization during housing and storage for sheep, swine, and other livestock manure systems (kg N)</returns>
        public double CalculateTotalNitrogenLossFromHousingAndStorage(
            double ammoniaEmissionRate,
            double numberOfAnimals)
        {
            return ammoniaEmissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.4-8
        /// </summary>
        /// <param name="totalNitrogenLossFromHousingAndStorage">Total manure N losses via NH3 volatilization during housing and storage for sheep, swine, and other livestock manure systems (kg N)</param>
        /// <returns>Daily NH3 emission from sheep, swine and other livestock manure during the housing and storage stages (kg NH3)</returns>
        public double CalculateAmmoniaLossFromHousingAndStorage(
            double totalNitrogenLossFromHousingAndStorage)
        {
            return totalNitrogenLossFromHousingAndStorage * CoreConstants.ConvertNH3NToNH3;
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
        /// <param name="averageMonthlyTemperature">Average monthly temperature (°C)</param>
        /// <returns>Ambient temperature-based adjustments used to correct default NH3 emission for feedlot (confined no barn) housing, or pasture (unitless)</returns>
        public double CalculateAmbientTemperatureAdjustment(double averageMonthlyTemperature)
        {
            return Math.Pow(1.041, averageMonthlyTemperature) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 4.3.1-13
        ///
        /// For naturally ventilated enclosures (barn) - assumption is temperature is 2 degrees higher. Dairy enclosed housing types
        /// are climate controlled and so do not use this equation
        /// </summary>
        /// <param name="averageMonthlyTemperature">Average monthly temperature (°C)</param>
        /// <returns>Ambient temperature-based adjustments used to correct default NH3 emission factors for beef barn</returns>
        public double CalculateAmbientTemperatureAdjustmentAddTwo(double averageMonthlyTemperature)
        {
            return Math.Pow(1.041, averageMonthlyTemperature + 2) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 4.3.1-10, 4.3.1-15, 4.3.3-5
        ///
        /// Method is used for multiple equation numbers since only the parameter values differs between the equations
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
        /// Equation 4.3.1-11, 4.3.1-16, 4.3.3-6
        ///
        /// Method is used for multiple equation numbers since only the parameter values differs between the equations
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
        /// Equation 4.3.1-12, 4.3.1-17, 4.3.3-7
        ///
        /// Method is used for multiple equation numbers since only the parameter values differs between the equations
        /// </summary>
        /// <param name="ammoniaConcentrationInHousing">Total ammonia nitrogen production from cattle (kg NH3-N)</param>
        /// <returns>Ammonia emissions from animals (kg NH3)</returns>
        public double CalculateTotalAmmoniaEmissionsFromHousing(
            double ammoniaConcentrationInHousing)
        {
            return ammoniaConcentrationInHousing * CoreConstants.ConvertNH3NToNH3;
        }

        /// <summary>
        /// Equation 4.3.1-9, 4.3.1-14
        ///
        /// Method is used for both equation numbers since only the parameter values differs between the two equations
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
        /// Equation 4.3.3-8
        /// </summary>
        /// <param name="tanExcretion">TAN excreted by the animals for this period of time (kg TAN)</param>
        /// <param name="tanExcretionFromPreviousPeriod">TAN excreted by the animals in the previous period (kg TAN)</param>
        /// <param name="ammoniaLostByHousingDuringPreviousPeriod">NH3-N lost through NH3 emissions from housing systems (kg NH3-N) </param>
        /// <returns>Amount of TAN entering the storage system (kg TAN day^-1)</returns>
        public double CalculateTanStorage(
            double tanExcretion,
            double tanExcretionFromPreviousPeriod,
            double ammoniaLostByHousingDuringPreviousPeriod)
        {
            return tanExcretion + (tanExcretionFromPreviousPeriod - ammoniaLostByHousingDuringPreviousPeriod);
        }

        /// <summary>
        /// Equation 4.3.2-2
        /// </summary>
        /// <param name="tanEnteringStorageSystem">Adjusted amount of TAN in stored manure (kg TAN)</param>
        /// <param name="fractionOfTanImmoblizedToOrganicNitrogen">Fraction of TAN that is immobilized to organic N during manure storage, dimensionless</param>
        /// <param name="fractionOfTanNitrifiedDuringManureStorage">Fraction of TAN that is nitrified during storage</param>
        /// <param name="nitrogenExretedThroughFeces">Nitrogen excreted through feces (kg N)</param>
        /// <param name="fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage">Fraction of organic N that is mineralized as TAN during manure storage, dimensionless </param>
        /// <param name="beddingNitrogen">Bedding nitrogen (kg N)</param>
        /// <returns>Adjusted amount of TAN in stored manure (kg TAN day^-1)</returns>
        public double CalculateAdjustedAmountOfTanInStoredManure(
            double tanEnteringStorageSystem,
            double fractionOfTanImmoblizedToOrganicNitrogen,
            double fractionOfTanNitrifiedDuringManureStorage,
            double nitrogenExretedThroughFeces,
            double fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage,
            double beddingNitrogen)
        {
            return (tanEnteringStorageSystem * (1 - fractionOfTanImmoblizedToOrganicNitrogen) * (1 - fractionOfTanNitrifiedDuringManureStorage)) +
                   ((nitrogenExretedThroughFeces + beddingNitrogen) * fractionOfOrganicNitrogenMineralizedAsTanDuringManureStorage);
        }

        /// <summary>
        /// Equation 4.3.2-3
        /// </summary>
        /// <param name="averageMonthlyTemperature">Average monthly temperature (°C)</param>
        /// <returns>Ambient temperature-based adjustments used to correct default NH3 emission factors for manure storage (compost, stockpile/deep bedding)</returns>
        public double CalculateAmbientTemperatureAdjustmentForStoredManure(double averageMonthlyTemperature)
        {
            return Math.Pow(1.041, averageMonthlyTemperature + 2) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 4.3.2-6
        /// Equation 4.3.3-10
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
        /// Equation 4.3.3-11
        /// </summary>
        /// <param name="tanInStoredManure">Total ammonical nitrogen (TAN) excretion rate (kg TAN head^-1 day^-1)</param>
        /// <param name="ammoniaEmissionFactor">Adjusted ammonia emission factor for beef barn (0 ≤ EF≤ 1)</param>
        /// <returns>Ammonia nitrogen loss from stored manure (stockpile/deep bedding, compost) (kg NH3-N)</returns>
        public double CalculateAmmoniaLossFromStoredManure(
            double tanInStoredManure,
            double ammoniaEmissionFactor)
        {
            return tanInStoredManure * ammoniaEmissionFactor;
        }

        /// <summary>
        /// Equation 4.3.2-8
        /// Equation 4.3.3-12
        /// </summary>
        /// <param name="ammoniaNitrogenLossFromStoredManure">Monthly ammonia nitrogen loss from stored manure (kg NH3-N)</param>
        /// <returns>Ammonia emission from manure storage system (kg NH3)</returns>
        public double CalculateAmmoniaEmissionsFromStoredManure(
            double ammoniaNitrogenLossFromStoredManure)
        {
            return ammoniaNitrogenLossFromStoredManure * CoreConstants.ConvertNH3NToNH3;
        }

        /// <summary>
        /// Equation 4.3.4-1
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
        /// Equation 4.3.4-2
        /// </summary>
        /// <param name="nitrogenExcretionRate">Amount of N excreted (kg N)</param>
        /// <param name="beddingNitrogen">Bedding nitrogen (kg N)</param>
        /// <param name="volatilizationFraction">Fraction of manure N volatilized as NH3 and NOx from the specific manure management system</param>
        /// <param name="volatilizationEmissionFactor">Emission factor for volatilization [kg N2O-N (kg N)^-1]</param>
        /// <returns>Manure volatilization N emission rate (kg N2O-N head^-1 day^-1)</returns>
        public double CalculateManureVolatilizationEmissionRate(
            double nitrogenExcretionRate,
            double beddingNitrogen,
            double volatilizationFraction,
            double volatilizationEmissionFactor)
        {
            var result = (nitrogenExcretionRate + beddingNitrogen) * volatilizationFraction * volatilizationEmissionFactor;

            return result;
        }

        /// <summary>
        /// Equation 4.3.4-3
        /// Equation 4.3.4-3
        /// </summary>
        /// <param name="volatilizationRate">Manure volatilization N emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of animals</param>
        /// <returns></returns>
        public double CalculateManureVolatilizationNitrogenEmission(double volatilizationRate, double numberOfAnimals)
        {
            return volatilizationRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.5-1
        /// </summary>
        /// <param name="nitrogenExcretionRate">N excretion rate (kg head^-1 day^-1)</param>
        /// <param name="leachingFraction">Leaching fraction</param>
        /// <param name="emissionFactorForLeaching">Emission factor for leaching [kg N₂O-N (kg N)⁻¹]</param>
        /// <param name="amountOfNitrogenAddedFromBedding"></param>
        /// <returns>Manure leaching N emission rate (kg N₂O-N head^-1 day^-1)</returns>
        public double CalculateManureLeachingNitrogenEmissionRate(double nitrogenExcretionRate,
            double leachingFraction,
            double emissionFactorForLeaching, 
            double amountOfNitrogenAddedFromBedding)
        {
            return (nitrogenExcretionRate + amountOfNitrogenAddedFromBedding) * leachingFraction * emissionFactorForLeaching;
        }

        /// <summary>
        /// Equation 4.3.5-2
        /// </summary>
        /// <param name="leachingNitrogenEmissionRate">Manure leaching N emission rate (kg head^-1 day^-1)</param>
        /// <param name="numberOfAnimals">Number of cattle</param>
        /// <returns>Manure leaching N emission (kg N2O-N)</returns>
        public double CalculateManureLeachingNitrogenEmission(double leachingNitrogenEmissionRate,
                                                              double numberOfAnimals)
        {
            return leachingNitrogenEmissionRate * numberOfAnimals;
        }

        /// <summary>
        /// Equation 4.3.6-1
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
        /// Equation 4.3.7.1
        /// </summary>
        /// <param name="manureDirectNitrogenEmission">Manure direct N emission (kg N₂O-N)</param>
        /// <param name="manureIndirectNitrogenEmission">Manure indirect N emission (kg N₂O-N)</param>
        /// <returns>Manure N emission (kg N₂O-N)</returns>
        public double CalculateManureNitrogenEmission(double manureDirectNitrogenEmission,
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
        /// </summary>
        /// <param name="monthlyTanEnteringStorageSystem">Adjusted amount of TAN in stored manure (kg TAN)</param>
        /// <param name="ammoniaLostFromStoredManure">Ammonia nitrogen lost in manure storage system (compost, stockpile, deep bedding, kg NH3-N)</param>
        /// <returns>Monthly TAN available for land application (kg TAN)</returns>
        public double CalculateMonthlyTanAvailableForLandApplication(
            double monthlyTanEnteringStorageSystem,
            double ammoniaLostFromStoredManure)
        {
            return monthlyTanEnteringStorageSystem - ammoniaLostFromStoredManure;
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
        /// <returns>Organic nitrogen available for land application (kg N)</returns>
        public double CalculateMonthlyOrganicNitrogenAvailableForLandApplication(
            double fecalNitrogenExcretion,
            double beddingNitrogen,
            double fractionOfMineralizedNitrogen,
            double directManureEmissions,
            double leachingEmissions)
        {
            var result = (fecalNitrogenExcretion + beddingNitrogen) -
                         ((fecalNitrogenExcretion + beddingNitrogen) * (1 - fractionOfMineralizedNitrogen) +
                          directManureEmissions + leachingEmissions);

            return result;
        }

        /// <summary>
        /// Equation 4.5.2-5
        /// </summary>
        /// <param name="tanAvailableForLandApplication">Total ammonical nitrogen (TAN)/inorganic nitrogen in stored beef manure from different housing and storage systems (kg TAN)</param>
        /// <param name="organicNitrogenAvailableForLandApplication">Total organic nitrogen in stored beef manure from different housing and storage systems (kg N)</param>
        /// <returns>Total available manure nitrogen in stored manure (kg N year^-1)</returns>
        public double CalculateTotalAvailableManureNitrogenInStoredManure(
            double tanAvailableForLandApplication,
            double organicNitrogenAvailableForLandApplication)
        {
            return tanAvailableForLandApplication + organicNitrogenAvailableForLandApplication;
        }

        /// <summary>
        /// Equation 4.5.2-6
        /// </summary>
        /// <param name="nitrogenExcretion">Amount of total N excreted (kg N)</param>
        /// <param name="nitrogenFromBedding">Amount of N from bedding (kg N)</param>
        /// <param name="directN2ONEmission">Manure N loss as direct N2O-N (kg N)</param>
        /// <param name="indirectN2ONEmission">Manure N loss as indirect N2O-N (kg N)</param>
        /// <returns></returns>
        public double CalculateTotalAvailableManureNitrogenInStoredManureForSheepSwinePoultryAndOtherLivestock(
            double nitrogenExcretion,
            double nitrogenFromBedding,
            double directN2ONEmission,
            double indirectN2ONEmission)
        {
            return (nitrogenExcretion + nitrogenFromBedding) - (directN2ONEmission + indirectN2ONEmission);
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
        /// <param name="nitrogenFractionOfManure">Fraction of nitrogen in manure</param>
        /// <returns>Total volume of manure available for land application (1000 kg wet weight for solid manure, 1000 L for liquid manure)</returns>
        public double CalculateTotalVolumeOfManureAvailableForLandApplication(
            double totalNitrogenAvailableForLandApplication,
            double nitrogenFractionOfManure)
        {
            return (totalNitrogenAvailableForLandApplication / (nitrogenFractionOfManure * 10)) * 1000;
        }

        /// <summary>
        /// Equation 4.6.1-1
        /// </summary>
        /// <param name="temperature">The temperature (degrees C) when manure is applied.</param>
        /// <returns>Ambient temperature-based adjustment</returns>
        public double CalculateAmbientTemperatureAdjustmentForLandApplication(double temperature)
        {
            var result = 1 - (0.058 * (15 - temperature));

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
        /// Equation 4.6.1-2
        /// </summary>
        /// <param name="emissionFactorForLandApplication">Default NH3 emission factor for land application</param>
        /// <param name="ambientTemperatureAdjustment">Ambient temperature based adjustment</param>
        /// <returns></returns>
        public double CalculateAdjustedAmmoniaEmissionFactor(
            double emissionFactorForLandApplication,
            double ambientTemperatureAdjustment)
        {
            return emissionFactorForLandApplication * ambientTemperatureAdjustment;
        }

        /// <summary>
        /// Equation 4.6.1-3
        /// </summary>
        /// <param name="tanAvailableForLandApplication">Default NH3 emission factor for land application</param>
        /// <param name="adjustedEmissionFactor">Ambient temperature based adjustment</param>
        /// <param name="fractionOfManureUsed">Fraction of manure applied on land</param>
        /// <returns>The NH3-N loss from field applied solid or liquid manure (kg NH3-N)</returns>
        public double CalculateTotalAmmoniaLossFromLandApplicationForBeefAndDairy(
            double tanAvailableForLandApplication,
            double adjustedEmissionFactor,
            double fractionOfManureUsed)
        {
            return fractionOfManureUsed * tanAvailableForLandApplication * adjustedEmissionFactor;
        }

        /// <summary>
        /// Equation 4.6.3-1
        /// </summary>
        /// <param name="fractionOfManureUsed">Fraction of manure applied on land</param>
        /// <param name="nitrogenAvailableForLandApplication"></param>
        /// <param name="volatilizationFraction">Fraction of land applied manure N lost by volatilization (kg N (kg N)^-1)</param>
        /// <returns></returns>
        public double CalculateTotalAmmoniaLossFromLandApplicationForSheepSwineAndOtherLivestock(
            double fractionOfManureUsed,
            double nitrogenAvailableForLandApplication,
            double volatilizationFraction)
        {
            return fractionOfManureUsed * nitrogenAvailableForLandApplication * volatilizationFraction;
        }

        /// <summary>
        /// Equation 4.6.2-1
        /// </summary>
        public double CalculateTotalAmmoniaLossFromLandApplicationForPoultry(
            double tanExcreted,
            double emissionFactorForLandApplication)
        {
            return tanExcreted * emissionFactorForLandApplication;
        }

        /// <summary>
        /// Equation 4.6.1-4
        /// </summary>
        /// <param name="ammoniaLossFromLandApplication">NH3-N loss from field applied manure (kg NH3-N)</param>
        /// <returns>The NH3 emissions from field applied solid or liquid manure (kg NH3)</returns>
        public double CalculateAmmoniaEmissionFromLandApplication(
            double ammoniaLossFromLandApplication)
        {
            return ammoniaLossFromLandApplication * CoreConstants.ConvertNH3NToNH3;
        }

        /// <summary>
        /// There may be multiple manure applications made on a specific day, calculate total ammonia emissions from these land applications.
        /// </summary>
        public double CalculateTotalAmmoniaEmissionsFromLandAppliedManure(Farm farm,
                                                                          DateTime dateTime,
                                                                          GroupEmissionsByDay dailyEmissions,
                                                                          AnimalType animalType,
                                                                          double temperature,
                                                                          ManagementPeriod managementPeriod)
        {
            var result = 0.0;

            // Iterate over each field and see if manure applications were made
            foreach (var fieldSystemComponent in farm.FieldSystemComponents)
            {
                var singleYearItem = fieldSystemComponent.GetSingleYearViewItem();

                var manureApplicationsByAnimalType = singleYearItem.GetManureApplicationsFromLivestock(
                    animalType: animalType,
                    dateOfManureApplication: dateTime);

                // There could be multiple applications on same day (unlikely but must consider this possibility)
                foreach (var manureApplicationViewItem in manureApplicationsByAnimalType)
                {
                    var totalVolumeOfManure = manureApplicationViewItem.AmountOfManureAppliedPerHectare * singleYearItem.Area;
                    var fractionOfManureUsed = totalVolumeOfManure / dailyEmissions.TotalVolumeOfManureAvailableForLandApplication;

                    var emissionFactorForApplicationType = 0.0;
                    if (manureApplicationViewItem.ManureStateType.IsLiquidManure())
                    {
                        emissionFactorForApplicationType = _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForLiquidAppliedManure(
                            manureApplicationType: manureApplicationViewItem.ManureApplicationMethod);
                    }
                    else
                    {
                        emissionFactorForApplicationType = _beefDairyDefaultEmissionFactorsProvider.GetAmmoniaEmissionFactorForSolidAppliedManure(
                            tillageType: singleYearItem.TillageType);
                    }

                    // Equation 4.6.1-1
                    var ambientTemperatureAdjustmentForLandApplication = this.CalculateAmbientTemperatureAdjustmentForLandApplication(
                        temperature: temperature);

                    // Equation 4.6.1-2
                    var adjustedEmissionFactor = this.CalculateAdjustedAmmoniaEmissionFactor(
                        emissionFactorForLandApplication: emissionFactorForApplicationType,
                        ambientTemperatureAdjustment: ambientTemperatureAdjustmentForLandApplication);

                    var ammoniaLossFromLandApplication = 0.0;
                    if (animalType.IsBeefCattleType() || animalType.IsDairyCattleType())
                    {
                        // Equation 4.6.1-3
                        ammoniaLossFromLandApplication = this.CalculateTotalAmmoniaLossFromLandApplicationForBeefAndDairy(
                            tanAvailableForLandApplication: dailyEmissions.TanAvailableForLandApplication,
                            adjustedEmissionFactor: adjustedEmissionFactor,
                            fractionOfManureUsed: fractionOfManureUsed);
                    }
                    else if (animalType.IsSheepType() || animalType.IsSwineType() || animalType.IsOtherAnimalType())
                    {
                        // Equation 4.6.3-1
                        ammoniaLossFromLandApplication = this.CalculateTotalAmmoniaLossFromLandApplicationForSheepSwineAndOtherLivestock(
                            fractionOfManureUsed: fractionOfManureUsed,
                            nitrogenAvailableForLandApplication: dailyEmissions.NitrogenAvailableForLandApplication,
                            volatilizationFraction: dailyEmissions.FractionOfManureVolatilized);
                    }
                    else if (animalType.IsPoultryType())
                    {
                        // Equation 4.6.2-1
                        ammoniaLossFromLandApplication = this.CalculateTotalAmmoniaLossFromLandApplicationForPoultry(
                            tanExcreted: dailyEmissions.TanExcretion,
                            emissionFactorForLandApplication: managementPeriod.ManureDetails.AmmoniaEmissionFactorForLandApplication);
                    }

                    // Equation 4.6.1-4
                    var ammoniaEmissionsFromLandApplication = this.CalculateAmmoniaEmissionFromLandApplication(
                        ammoniaLossFromLandApplication: ammoniaLossFromLandApplication);

                    result += ammoniaEmissionsFromLandApplication;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if dairy or beef cattle animals are grazing during the <see cref="ManagementPeriod"/> and calculates manure direct/indirect emissions.
        /// </summary>
        public void GetEmissionsFromBeefAndDairyGrazingAnimals(
            ManagementPeriod managementPeriod,
            double temperature,
            GroupEmissionsByDay groupEmissionsByDay)
        {
            if (managementPeriod.HousingDetails.HousingType == HousingType.Pasture && managementPeriod.HousingDetails.PastureLocation != null)
            {
                // Equation 5.3.1-1
                groupEmissionsByDay.ManureDirectN2ONEmissionRate = groupEmissionsByDay.TanExcretionRate * managementPeriod.ManureDetails.N2ODirectEmissionFactor;

                // Equation 5.3.1-5
                groupEmissionsByDay.ManureDirectN2ONEmission = groupEmissionsByDay.ManureDirectN2ONEmissionRate * managementPeriod.NumberOfAnimals;

                // Equation 5.3.3-1
                var temperatureAdjustmentForGrazing = this.GetAmbientTemperatureAdjustmentForGrazing(
                    temperature: temperature);

                var emissionFactorForGrazing = _beefDairyDefaultEmissionFactorsProvider.GetEmissionFactorByHousing(
                    housingType: managementPeriod.HousingDetails.HousingType);

                // Equation 5.3.3-2
                var adjustedAmmoniaEmissionFactor = this.GetAdjustedEmissionFactorForGrazing(
                    emissionFactor: emissionFactorForGrazing,
                    temperatureAdjustment: temperatureAdjustmentForGrazing);

                // Equation 5.3.3-3
                var ammoniaEmissionRateFromGrazingAnimals = this.GetAmmoniaEmissionRateForGrazingAnimals(
                    tanExcretionRate: groupEmissionsByDay.TanExcretionRate,
                    adjustedEmissionFactor: adjustedAmmoniaEmissionFactor);

                // Equation 5.3.3-4
                var totalAmmoniaProductionFromGrazingAnimals = this.GetTotalAmmoniaProductionFromGrazingAnimals(
                    ammoniaEmissionRate: ammoniaEmissionRateFromGrazingAnimals,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                // Equation 5.3.3-5
                var ammoniaEmissionsFromAnimalsOnPasture = this.GetAmmoniaEmissionsFromAnimalsOnPasture(
                    totalAmmoniaProduction: totalAmmoniaProductionFromGrazingAnimals);

                // Equation 5.3.5-1
                var volatilizationFraction = totalAmmoniaProductionFromGrazingAnimals / groupEmissionsByDay.AmountOfNitrogenExcreted;

                // Equation 5.3.5-2
                groupEmissionsByDay.ManureVolatilizationRate = groupEmissionsByDay.NitrogenExcretionRate * volatilizationFraction * managementPeriod.ManureDetails.EmissionFactorVolatilization;

                // Equation 5.3.5-3
                groupEmissionsByDay.ManureVolatilizationN2ONEmission = groupEmissionsByDay.ManureVolatilizationRate * managementPeriod.NumberOfAnimals;

                // Equation 5.3.6-1
                groupEmissionsByDay.ManureNitrogenLeachingRate = groupEmissionsByDay.NitrogenExcretionRate * managementPeriod.ManureDetails.LeachingFraction * 0.011;

                // Equation 5.3.6-2
                groupEmissionsByDay.ManureN2ONLeachingEmission = groupEmissionsByDay.ManureNitrogenLeachingRate * managementPeriod.NumberOfAnimals;

                // Equation 5.3.7-1
                groupEmissionsByDay.ManureIndirectN2ONEmission = groupEmissionsByDay.ManureN2ONLeachingEmission + groupEmissionsByDay.ManureVolatilizationN2ONEmission;

                // Not in document but needed so monthly emissions can be calculated
                groupEmissionsByDay.ManureN2ONEmission = groupEmissionsByDay.ManureDirectN2ONEmission + groupEmissionsByDay.ManureIndirectN2ONEmission;
            }
        }

        /// <summary>
        /// Checks if sheep, swine, poultry, or other livestock animals are grazing during the <see cref="ManagementPeriod"/> and calculates ammonia emissions from grazing animals as required.
        /// </summary>
        public double GetAmmoniaEmissionsFromGrazingSheepSwinePoultryAndOtherLiveStock(
            ManagementPeriod managementPeriod,
            GroupEmissionsByDay groupEmissionsByDay)
        {
            var result = 0.0;

            if (managementPeriod.HousingDetails.HousingType == HousingType.Pasture && managementPeriod.HousingDetails.PastureLocation != null)
            {
                // Equation 5.2.5-6
                var ammoniaEmissionRateForAnimalsOnPasture = this.GetAmmoniaEmissionRateFromAnimalsOnPasture(
                    nitrogenExcretionRate: groupEmissionsByDay.NitrogenExcretionRate,
                    volatilizationFraction: groupEmissionsByDay.FractionOfManureVolatilized);

                // Equation 5.2.5-7
                var ammoniaEmissions = this.GetTotalAmmoniaProductionFromGrazingAnimals(
                    ammoniaEmissionRate: ammoniaEmissionRateForAnimalsOnPasture,
                    numberOfAnimals: managementPeriod.NumberOfAnimals);

                // Equation 5.2.5-8
                result = this.GetAmmoniaEmissionsFromAnimalsOnPasture(
                    totalAmmoniaProduction: ammoniaEmissions);
            }

            return result;
        }

        /// <summary>
        /// Equation 5.2.5-6
        /// </summary>
        /// <param name="nitrogenExcretionRate">Nitrogen excretion rate of animals (kg N head^-1 day^-1)</param>
        /// <param name="volatilizationFraction">Fraction of land applied manure N lost by volatilization (kg N (kg N)^-1)</param>
        /// <returns>Ammonia N emission rate for animals grazing on pasture (kg NH3 head^-1 day^-1) </returns>
        public double GetAmmoniaEmissionRateFromAnimalsOnPasture(
            double nitrogenExcretionRate,
            double volatilizationFraction)
        {
            return nitrogenExcretionRate * volatilizationFraction;
        }

        /// <summary>
        /// Equation 5.3.3-1
        /// </summary>
        /// <param name="temperature">Temperature (degrees C)</param>
        /// <returns>Ambient temperature adjustment for grazing ammonia emission factor</returns>
        public double GetAmbientTemperatureAdjustmentForGrazing(double temperature)
        {
            return Math.Pow(1.0141, temperature) / Math.Pow(1.041, 15);
        }

        /// <summary>
        /// Equation 5.3.3-2
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
        /// Equation 5.3.3-3
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
        /// Equation 5.3.3-4
        /// Equation 5.2.5-7
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
        /// Equation 5.2.5-5
        /// Equation 5.2.5-8
        /// </summary>
        /// <param name="totalAmmoniaProduction">Total ammonia production from animals grazing on pasture (kg NH3-N)</param>
        /// <returns>Total ammonia emissions from animals on pasture (kg NH3)</returns>
        public double GetAmmoniaEmissionsFromAnimalsOnPasture(
            double totalAmmoniaProduction)
        {
            return totalAmmoniaProduction * CoreConstants.ConvertNH3NToNH3;
        }

        /// <summary>
        /// Equation 3.1.8-8
        /// Equation 4.3.1-1
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
        /// <returns>Calf protein intake (kg head^-1 day^-1)</returns>
        public double CalculateCalfProteinIntake(
            double calfProteinIntakeFromMilk,
            double calfProteinIntakeFromSolidFood)
        {
            return calfProteinIntakeFromMilk + calfProteinIntakeFromSolidFood;
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
            const double DieselConversion = 70;         // Same for both liquid & solid manure
            const double ManureConversion = 0.0248;     // Same for both liquid & solid manure

            return volumeOfManure * ManureConversion * DieselConversion;
        }

        /// <summary>
        /// Equation 3.4.4-1
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

        /// <summary>
        /// Equation 12.3.1-5
        /// </summary>
        /// <param name="animalType">The animal group type</param>
        /// <param name="finalWeightOfAnimal">Final weight of animal (kg animal^-1)</param>
        /// <returns></returns>
        public double CalculateDryMatterMax(
            AnimalType animalType,
            double finalWeightOfAnimal)
        {
            var intakeLimit = 2.25;

            var weightLimit = finalWeightOfAnimal * (intakeLimit / 100.0);

            // Add in additional 15%
            var result = weightLimit * 1.15;

            return result;
        }

        /// <summary>
        /// Equation 3.2.1-1
        /// Equation 3.1.2-1
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
        public double CaclulateDailyCarbonUptakeForGroup(
            double totalDailyDryMatterIntakeForGroup)
        {
            var result = totalDailyDryMatterIntakeForGroup * CoreConstants.CarbonConcentration;

            return result;
        }

        public double CalculateAdjustedAmmoniaFromHousing(GroupEmissionsByDay dailyEmissions, ManagementPeriod managementPeriod)
        {
            var volatilizationFractionForHousing = this.CalculateVolatalizationFractionForHousing(
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                dailyAmmoniaEmissionsFromHousing: dailyEmissions.AmmoniaConcentrationInHousing);

            var volatilizationEmissionsFromHousing = this.CalculateAmmoniaVolatilizationFromHousing(
                amountOfNitrogenExcreted: dailyEmissions.AmountOfNitrogenExcreted,
                amountOfNitrogenFromBedding: dailyEmissions.AmountOfNitrogenAddedFromBedding,
                volatilizationFractionFromHousing: volatilizationFractionForHousing,
                emissionFactorForVolatilization: managementPeriod.ManureDetails.EmissionFactorVolatilization);

            var ammoniaHousingAdjustment = this.CalculateAmmoniaHousingAdjustment(
                ammoniaFromHousing: dailyEmissions.AmmoniaConcentrationInHousing,
                ammoniaVolatilizedDuringHousing: volatilizationEmissionsFromHousing);

            var result = CalculateAmmoniaEmissionsFromHousing(
                ammoniaFromHousingAdjustment: ammoniaHousingAdjustment);

            return result;
        }

        /// <summary>
        /// Equation 4.3.4-4
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
        /// Equation 4.3.4-5
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
            return (amountOfNitrogenExcreted + amountOfNitrogenFromBedding) * volatilizationFractionFromHousing * emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 4.3.4-6
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
        /// Equation 4.3.4-7
        /// </summary>
        /// <param name="ammoniaFromHousingAdjustment">	Adjusted daily NH3 emissions from beef and dairy cattle and broiler, layer and turkey manure during the housing stage (kg NH3)</param>
        /// <returns>Adjusted daily NH3-N emissions from housing (kg NH3-N day-1)</returns>
        public double CalculateAmmoniaEmissionsFromHousing(
            double ammoniaFromHousingAdjustment)
        {
            return ammoniaFromHousingAdjustment * CoreConstants.ConvertNH3NToNH3;
        }

        /// <summary>
        /// Equation 4.3.4-8
        /// </summary>
        /// <param name="dailyAmmoniaEmissionsFromStorage">Daily NH3-N emissions during storage of manure (kg NH3-N day^-1)</param>
        /// <param name="amountOfNitrogenExcreted">Total amount of N excreted by beef or dairy cattle or broilers, layers or turkeys (kg N day^-1)</param>
        /// <param name="amountOfNitrogenFromBedding">Total amount of N added from bedding materials (kg N day^-1)</param>
        /// <returns>Fraction of manure N volatilized as NH3 and NOx during the manure storage stage</returns>
        public double CalculateVolatalizationFractionForStorage(
            double dailyAmmoniaEmissionsFromStorage,
            double amountOfNitrogenExcreted,
            double amountOfNitrogenFromBedding)
        {
            return dailyAmmoniaEmissionsFromStorage / (amountOfNitrogenExcreted + amountOfNitrogenFromBedding);
        }

        /// <summary>
        /// Equation 4.3.4-9
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
            return (amountOfNitrogenExcreted + amountOfNitrogenFromBedding) * volatilizationFractionFromStorage * emissionFactorForVolatilization;
        }

        /// <summary>
        /// Equation 4.3.4-10
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

        /// <summary>
        /// Equation 4.3.4-11
        /// </summary>
        /// <param name="ammoniaFromStorageAdjustment">	Adjusted daily NH3 emissions from beef and dairy cattle and broiler, layer and turkey manure during the housing stage (kg NH3)</param>
        /// <returns>Adjusted daily NH3-N emissions from housing (kg NH3-N day-1)</returns>
        public double CalculateAmmoniaEmissionsFromStorage(
            double ammoniaFromStorageAdjustment)
        {
            return ammoniaFromStorageAdjustment * CoreConstants.ConvertNH3NToNH3;
        }

        #endregion

        /// <summary>
        /// -- there will be one of these values for each day, in the view model, get the largest value and show this to user so that the optimal TDN value is for all days over management period...
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
            for (double tdn = currentTdn; tdn <= 100 && targetTdnFound == false; tdn += tdnStep)
            {
                // Keep calculating a new GEI until the DMI is brought down to a level that is lower than the DMI_max
                var grossEnergyIntake = this.CalculateGrossEnergyIntake(
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
    }
}