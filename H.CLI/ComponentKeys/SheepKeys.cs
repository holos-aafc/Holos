using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;

namespace H.CLI.ComponentKeys
{
    public class SheepKeys : IComponentKeys
    {
        public Dictionary<string, ImperialUnitsOfMeasurement?> keys { get; set; } = new Dictionary<string, ImperialUnitsOfMeasurement?>
        {
            {Properties.Resources.Key_Name, null},
            {Properties.Resources.GroupName, null},
            {Properties.Resources.GroupType, null},
            {Properties.Resources.ManagementPeriodName, null},
            {Properties.Resources.ManagementPeriodStartDate, null},
            {Properties.Resources.ManagementPeriodDays, null},
            {Properties.Resources.NumberOfAnimals, null},
            {Properties.Resources.NumberOfLambs, null },
            {Properties.Resources.InitialWeight, ImperialUnitsOfMeasurement.Pounds},
            {Properties.Resources.FinalWeight, ImperialUnitsOfMeasurement.Pounds},
            {Properties.Resources.ADG, ImperialUnitsOfMeasurement.Pounds},

            {Properties.Resources.EnergyRequiredToProduceWool, ImperialUnitsOfMeasurement.BritishThermalUnitPerPound},
            {Properties.Resources.WoolProduction, ImperialUnitsOfMeasurement.PoundsPerYear},
            {Properties.Resources.EnergyRequiredToProduceMilk, ImperialUnitsOfMeasurement.BritishThermalUnitPerPound},

            {Properties.Resources.DietAdditiveType, null},
            {Properties.Resources.MethaneConversionFactorOfDiet, ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane},
            {Properties.Resources.MethaneConversionFactorAdjusted, ImperialUnitsOfMeasurement.Percentage},
            {Properties.Resources.FeedIntake, ImperialUnitsOfMeasurement.PoundPerHeadPerDay},
            {Properties.Resources.CrudeProtein, ImperialUnitsOfMeasurement.PoundsPerPound},
            {Properties.Resources.Forage, ImperialUnitsOfMeasurement.PercentageDryMatter},
            {Properties.Resources.TDN, ImperialUnitsOfMeasurement.PercentageDryMatter},
            {Properties.Resources.Starch, ImperialUnitsOfMeasurement.PercentageDryMatter},
            {Properties.Resources.Fat, ImperialUnitsOfMeasurement.PercentageDryMatter},
            {Properties.Resources.ME, ImperialUnitsOfMeasurement.BritishThermalUnitPerPound},
            {Properties.Resources.NDF, ImperialUnitsOfMeasurement.PercentageDryMatter},

          
            {Properties.Resources.PastureLocation, null},
            {Properties.Resources.GainCoefficientA, null},
            {Properties.Resources.GainCoefficientB, null},
            {Properties.Resources.ActivityCoefficient, ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound},
            {Properties.Resources.MaintenanceCoefficient, ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound},
         
            {Properties.Resources.MethaneConversionFactorOfManure, ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane},
            {Properties.Resources.N2ODirectEmissionFactor, ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN},
            {Properties.Resources.EmissionFactorVolatilization, null},
            {Properties.Resources.VolatilizationFraction, null},
            {Properties.Resources.EmissionFactorLeaching, null},
            {Properties.Resources.FractionLeaching, null},
            {Properties.Resources.AshContent, ImperialUnitsOfMeasurement.Percentage},
            {Properties.Resources.MethaneProducingCapacityOfManure, null},
        };

        public bool IsHeaderOptional(string s)
        {
            return false;
        }
        public Dictionary<string, bool> MissingHeaders { get; set; } = new Dictionary<string, bool>();
    }
}
