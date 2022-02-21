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
    public class FeedEstimateResults
    {
        #region Fields

        private readonly UnitsOfMeasurementCalculator _calc = new UnitsOfMeasurementCalculator();

        #endregion

        #region Properties
        public double AllFarmsDryMatterIntake { get; set; }
    
        public double FarmDryMatterIntake { get; set; }
        
        public double ComponentDryMatterIntake { get; set; }

        public double AnimalGroupDryMatterIntake { get; set; }

        public double AnimalSubGroupDryMatterIntake { get; set; }

        public double TotalMonthlyDryMatterIntake { get; set; }

        public double MonthlyDryMatterIntake { get; set; }


        #endregion

        #region Public Methods

        /// <summary>
        /// Takes in a list of the emission results for ALL THE FARMS
        /// We filter out the relevant components for the estimates of production (some Components such as Sheep and Beef Production
        /// are not considered for Feed Estimates)
        /// Based on the relevant data, we calculate the Sum of the relevant data for ALL THE FARMS while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForAllFarms(List<KeyValuePair<string, List<AnimalComponentEmissionsResults>>> _emissionResultsForAllFarms)
        {
            var allFilteredFarms = _emissionResultsForAllFarms.SelectMany(x => x.Value.Where(y => y.Component.ComponentCategory == ComponentCategory.Sheep ||
                                                                                                  y.Component.ComponentCategory == ComponentCategory.BeefProduction ||
                                                                                                  y.Component.ComponentCategory == ComponentCategory.Dairy));

            AllFarmsDryMatterIntake = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.KilogramsNitrogen, allFilteredFarms.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(z => z.GroupEmissionsByMonths.Sum(w => w.DryMatterIntake))), false);


        }

        /// <summary>
        /// Takes in a list of the emission results for ONE FARM
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE FARM while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForAFarm(IEnumerable<AnimalComponentEmissionsResults> filteredFarmComponents)
        {
            FarmDryMatterIntake = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, filteredFarmComponents.Sum(w => w.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.DryMatterIntake))), false);
        }

        /// <summary>
        /// Takes in a grouping of the emission results for ONE COMPONENT IN A FARM based on Component Category
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE COMPONENT IN A FARM while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForOneComponent(IGrouping<ComponentCategory, AnimalComponentEmissionsResults> componentGroup)
        {
            ComponentDryMatterIntake = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, componentGroup.Sum(x => x.EmissionResultsForAllAnimalGroupsInComponent.Sum(y => y.GroupEmissionsByMonths.Sum(z => z.DryMatterIntake))), false);

        }

        /// <summary>
        /// Takes in a grouping of the emission results for ONE ANIMAL GROUP IN A COMPONENT (based on AnimalType)
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE ANIMAL GROUP IN A COMPONENT while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForOneAnimalGroup(IGrouping<AnimalType, AnimalGroupEmissionResults> animalGroup)
        {
            AnimalGroupDryMatterIntake = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalGroup.Sum(x => x.GroupEmissionsByMonths.Sum(y => y.DryMatterIntake)), false);


        }

        /// <summary>
        /// Takes in the emission results for ONE ANIMAL SUBGROUP (user defined animal groups... ie. SwineStartersGroup1). USED FOR YEARLY CALCULATIONS - we don't split up the animal sub group into months so
        /// we cannot use the CalculateMonthlyTotals method.
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE ANIMAL SUBGROUP while considering the units of measurement.
        /// </summary>
        public void CalculateTotalsForOneAnimalSubGroup(AnimalGroupEmissionResults animalSubGroup)
        {
            AnimalSubGroupDryMatterIntake = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, animalSubGroup.GroupEmissionsByMonths.Sum(x => x.DryMatterIntake), false);

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
            TotalMonthlyDryMatterIntake = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, allMonthlyEmissions.Sum(x => x.Sum(y => y.DryMatterIntake)), false);
        }

        /// <summary>
        /// Takes in ONE of the monthly emission results for ONE ANIMAL SUBGROUP (i.e month 4's emissions grouped by the integer value, 4 = April)
        /// The Components have already been filtered at this stage, so we do not need to keep filtering the Farm's Components
        /// Based on the relevant data, we calculate the Sum of the relevant data for ONE ANIMAL SUBGROUP while considering the units of measurement.
        /// </summary>
        public void CalculateMonthlyEmissionsForAnimalGroup(IGrouping<int, GroupEmissionsByMonth> monthlyEmissions)
        {
            MonthlyDryMatterIntake = _calc.GetUnitsOfMeasurementValue(CLIUnitsOfMeasurementConstants.measurementSystem, MetricUnitsOfMeasurement.Kilograms, monthlyEmissions.Sum(x => x.DryMatterIntake), false);
        }

        #endregion
    }
}

