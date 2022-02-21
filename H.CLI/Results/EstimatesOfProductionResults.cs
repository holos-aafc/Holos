using H.CLI.UserInput;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Emissions.Results;
using H.Core.Models;

namespace H.CLI.Results
{
    public class EstimatesOfProductionResults
    {
        #region Properties

        //All Farms
        public double AllFarmsLandManure { get; set; }
        public double AllFarmsMilkProduced { get; set; }
        public double AllFarmsFatAndProteinCorrectedMilkProduction { get; set; }
        public double AllFarmsBeefProduced { get; set; }
        public double AllFarmsLambProduced { get; set; }

        //One Farm
        public double FarmLandManure { get; set; }
        public double FarmMilkProduced { get; set; }
        public double FarmFatAndProteinCorrectedMilkProduction { get; set; }
        public double FarmBeefProduced { get; set; }
        public double FarmLambProduced { get; set; }

        //One Component
        public double ComponentLandManure { get; set; }
        public double ComponentMilkProduced { get; set; }
        public double ComponentFatAndProteinCorrectedMilkProduction { get; set; }
        public double ComponentBeefProduced { get; set; }
        public double ComponentLambProduced { get; set; }

        //One Animal Group
        public double AnimalGroupLandManure { get; set; }
        public double AnimalGroupMilkProduced { get; set; }
        public double AnimalGroupFatAndProteinCorrectedMilkProduction { get; set; }
        public double AnimalGroupBeefProduced { get; set; }
        public double AnimalGroupLambProduced { get; set; }

        //One Animal SubGroup (user defined groups)
        public double AnimalSubGroupLandManure { get; set; }
        public double AnimalSubGroupMilkProduced { get; set; }
        public double AnimalSubGroupFatAndProteinCorrectedMilkProduction { get; set; }
        public double AnimalSubGroupBeefProduced { get; set; }
        public double AnimalSubGroupLambProduced { get; set; }

        //Total For All Months
        public double TotalMonthlyLandManure { get; set; }
        public double TotalMonthlyMilkProduced  { get; set; }
        public double TotalMonthlyFatAndProteinCorrectedMilkProduction  { get; set; }
        public double TotalMonthlyBeefProduced  { get; set; }
        public double TotalMonthlyLambProduced  { get; set; }

        //One Month
        public double MonthlyLandManure { get; set; }
        public double MonthlyMilkProduced { get; set; }
        public double MonthlyFatAndProteinCorrectedMilkProduction { get; set; }
        public double MonthlyBeefProduced { get; set; }
        public double MonthlyLambProduced { get; set; }

        #endregion

        #region Calculators
        private readonly UnitsOfMeasurementCalculator _calc = new UnitsOfMeasurementCalculator(); 
        #endregion

        #region Public Methods
        /// <summary>
        /// Takes in a list of the emission results for ALL THE FARMS
        /// We filter out the relevant components for the estimates of production (some Components such as OtherLiveStock and Dairy Animal Types that are not Dairy Lactating,
        /// are not considered for the Estimates Of Production)
        /// Based on the relevant data, we calculate the Sum of the relevant data for ALL THE FARMS while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForAllFarms(List<KeyValuePair<string, List<AnimalComponentEmissionsResults>>> _emissionResultsForAllFarms)
        {
            //Filters Out The Relevant Components In The Farm
            var allFilteredFarms = _emissionResultsForAllFarms.SelectMany(x => x.Value.Where(y => y.Component.ComponentType != ComponentType.Rams &&
                                                                                       y.Component.ComponentType != ComponentType.DairyBulls &&
                                                                                       y.Component.ComponentType != ComponentType.DairyDry &&
                                                                                       y.Component.ComponentType != ComponentType.DairyCalf &&
                                                                                       y.Component.ComponentType != ComponentType.DairyHeifer &&
                                                                                       y.Component.ComponentType != ComponentType.DairyBulls &&
                                                                                       y.Component.ComponentCategory != ComponentCategory.OtherLivestock));

            AllFarmsLandManure = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, allFilteredFarms.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.GroupEmissionsByMonths.Sum(w => w.MonthlyNitrogenAvailableForLandApplication))), false);
            AllFarmsMilkProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allFilteredFarms.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.GroupEmissionsByMonths.Sum(w => w.MonthlyMilkProduction))), false);
            AllFarmsFatAndProteinCorrectedMilkProduction = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allFilteredFarms.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.GroupEmissionsByMonths.Sum(w => w.MonthlyFatAndProteinCorrectedMilkProduction))), false);
            AllFarmsBeefProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allFilteredFarms.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.GroupEmissionsByMonths.Sum(w => w.MonthlyBeefProduced))), false);
            AllFarmsLambProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allFilteredFarms.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.GroupEmissionsByMonths.Sum(w => w.MonthlyLambProduced))), false);

        }

        /// <summary>
        /// Takes in a list of the emission results for ONE FARM
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE FARM while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForAFarm(IEnumerable<AnimalComponentEmissionsResults> filteredFarmComponents)
        {
            FarmLandManure = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, filteredFarmComponents.Sum(w => w.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyNitrogenAvailableForLandApplication))), false);
            FarmMilkProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, filteredFarmComponents.Sum(w => w.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyMilkProduction))), false);
            FarmFatAndProteinCorrectedMilkProduction = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, filteredFarmComponents.Sum(w => w.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyFatAndProteinCorrectedMilkProduction))), false);
            FarmBeefProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, filteredFarmComponents.Sum(w => w.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyBeefProduced))), false);
            FarmLambProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, filteredFarmComponents.Sum(w => w.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyLambProduced))), false);

        }

        /// <summary>
        /// Takes in a grouping of the emission results for ONE COMPONENT IN A FARM based on Component Category
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE COMPONENT IN A FARM while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForOneComponent(IGrouping<ComponentCategory, AnimalComponentEmissionsResults> componentGroup)
        {
            ComponentLandManure = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, componentGroup.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyNitrogenAvailableForLandApplication))), false);
            ComponentMilkProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, componentGroup.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyMilkProduction))), false);
            ComponentFatAndProteinCorrectedMilkProduction = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, componentGroup.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyFatAndProteinCorrectedMilkProduction))), false);
            ComponentBeefProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, componentGroup.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyBeefProduced))), false);
            ComponentLambProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, componentGroup.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.MonthlyLambProduced))), false);
        }

        /// <summary>
        /// Takes in a grouping of the emission results for ONE ANIMAL GROUP IN A COMPONENT (based on AnimalType)
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE ANIMAL GROUP IN A COMPONENT while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForOneAnimalGroup(IGrouping<AnimalType, AnimalGroupEmissionResults> animalGroup)
        {
            AnimalGroupLandManure = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyNitrogenAvailableForLandApplication)), false);
            AnimalGroupMilkProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyMilkProduction)), false);
            AnimalGroupFatAndProteinCorrectedMilkProduction = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyFatAndProteinCorrectedMilkProduction)), false);
            AnimalGroupBeefProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyBeefProduced)), false);
            AnimalGroupLambProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.MonthlyLambProduced)), false);

        }

        /// <summary>
        /// Takes in the emission results for ONE ANIMAL SUBGROUP (user defined animal groups... ie. SwineStartersGroup1). USED FOR YEARLY CALCULATIONS - we don't split up the animal sub group into months so
        /// we cannot use the CalculateMonthlyTotals method.
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE ANIMAL SUBGROUP while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForOneAnimalSubGroup(AnimalGroupEmissionResults animalSubGroup)
        {
            AnimalSubGroupLandManure = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyNitrogenAvailableForLandApplication), false);
            AnimalSubGroupMilkProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyMilkProduction), false);
            AnimalSubGroupFatAndProteinCorrectedMilkProduction = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyFatAndProteinCorrectedMilkProduction), false);
            AnimalSubGroupBeefProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyBeefProduced), false);
            AnimalSubGroupLambProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalSubGroup.GroupEmissionsByMonths.Sum(x => x.MonthlyLambProduced), false);
        }

        /// <summary>
        /// Takes in a grouped list of the monthly emission results for ONE ANIMAL SUBGROUP (month is represented by an integer and 
        /// are associated with their respective emission results for that month). USED FOR MONTHLY CALCULATIONS.
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE ANIMAL SUBGROUP while considering the units of measurement.
        /// NOTE: the sum of all the month's and their emissions is equivalent to the total for one animal subgroup. 
        /// </summary>
        public void CalculateMonthlyTotals(IEnumerable<IGrouping<int, GroupEmissionsByMonth>> allMonthlyEmissions)
        {
            TotalMonthlyLandManure = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allMonthlyEmissions.Sum(x => x.Sum(y => y.MonthlyNitrogenAvailableForLandApplication)), false);
            TotalMonthlyMilkProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allMonthlyEmissions.Sum(x => x.Sum(y => y.MonthlyMilkProduction)), false);
            TotalMonthlyFatAndProteinCorrectedMilkProduction = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allMonthlyEmissions.Sum(x => x.Sum(y => y.MonthlyFatAndProteinCorrectedMilkProduction)), false);
            TotalMonthlyBeefProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allMonthlyEmissions.Sum(x => x.Sum(y => y.MonthlyBeefProduced)), false);
            TotalMonthlyLambProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allMonthlyEmissions.Sum(x => x.Sum(y => y.MonthlyLambProduced)), false);
        }


        /// <summary>
        /// Takes in ONE of the monthly emission results for ONE ANIMAL SUBGROUP (i.e month 4's emissions grouped by the integer value, 4 = April)
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE ANIMAL SUBGROUP while considering the units of measurement.
        /// </summary>
        public void CalculateMonthlyEmissionsForAnimalGroup(IGrouping<int, GroupEmissionsByMonth> monthlyEmissions)
        {
            MonthlyLandManure = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, monthlyEmissions.Sum(x => x.MonthlyNitrogenAvailableForLandApplication), false);
            MonthlyMilkProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, monthlyEmissions.Sum(x => x.MonthlyMilkProduction), false);
            MonthlyFatAndProteinCorrectedMilkProduction = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, monthlyEmissions.Sum(x => x.MonthlyFatAndProteinCorrectedMilkProduction), false);
            MonthlyBeefProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, monthlyEmissions.Sum(x => x.MonthlyBeefProduced), false);
            MonthlyLambProduced = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, monthlyEmissions.Sum(x => x.MonthlyLambProduced), false);
        }

        #endregion

    }
}
