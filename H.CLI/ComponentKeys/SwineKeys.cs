using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;
using H.Core.Enumerations;

namespace H.CLI.ComponentKeys
{
    /// <summary>
    /// The dictionary below takes in a string - the header and a nullable ImperialUnitsOfMeasurement?. We do not include
    /// the MetricUnitsOfMeasurement because the calculations performed later demand that the values be in Metric.
    /// Therefore, in our parser, we need to convert all Imperial units to Metric units and to do that, we only need
    /// to know what the Imperial units are (because if its metric, we don't need to do anything because the data is already
    /// in Metric). The Imperial Units will be used in our ConvertToMetricFromImperial method using a switch statement based
    /// on the ImperialUnitsOfMeasurement here and convert the data to Metric for the calculations.
    /// </summary>
    public class SwineKeys : IComponentKeys
    {  
       /// <summary>
       /// When you modify the key, remember to add a new property corresponding to the new key that you have added 
       /// below to the ShelterBeltTemporaryInput class in the format: "Example Format",
       /// In this case, please add a new property in the format: ExampleFormat to the concrete SwineStarterTemporaryInput class.
       /// The order of the keys below is the order in which they will be written when creating the template files for a Shelterbelt
       /// </summary>
        public Dictionary<string, ImperialUnitsOfMeasurement?> keys { get; set; } = new Dictionary<string, ImperialUnitsOfMeasurement?>()
        {
             {Properties.Resources.Key_Name, null },
             {Properties.Resources.GroupName, null },
             {Properties.Resources.GroupType, null },
             {Properties.Resources.ManagementPeriodName, null },
             {Properties.Resources.ManagementPeriodStartDate, null },
             {Properties.Resources.ManagementPeriodDays, null },
             {Properties.Resources.NumberOfAnimals, null },

             {Properties.Resources.DietAdditiveType, null},
             {Properties.Resources.FeedIntake, ImperialUnitsOfMeasurement.PoundPerHeadPerDay},
             {Properties.Resources.CrudeProtein, ImperialUnitsOfMeasurement.PercentageDryMatter},
             {Properties.Resources.Forage,  ImperialUnitsOfMeasurement.Percentage},
             {Properties.Resources.TDN, ImperialUnitsOfMeasurement.PercentageDryMatter},
             {Properties.Resources.Starch, ImperialUnitsOfMeasurement.PercentageDryMatter},
             {Properties.Resources.Fat, ImperialUnitsOfMeasurement.PercentageDryMatter},
             {Properties.Resources.ME, ImperialUnitsOfMeasurement.BritishThermalUnitPerPound},
             {Properties.Resources.NDF, ImperialUnitsOfMeasurement.PercentageDryMatter},
             {Properties.Resources.VolatileSolidAdjusted, ImperialUnitsOfMeasurement.PoundsPerPound},
             {Properties.Resources.NitrogenExcretionAdjusted, ImperialUnitsOfMeasurement.PoundsPerPound},
           

        
             {Properties.Resources.PastureLocation, null },
             {Properties.Resources.CA, ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound},
             {Properties.Resources.CFTemp, ImperialUnitsOfMeasurement.BritishThermalUnitPerDayPerPound},

        
             {Properties.Resources.MethaneConversionFactor, ImperialUnitsOfMeasurement.PoundsMethanePerPoundMethane},
             {Properties.Resources.MethaneConversionFactorAdjusted, ImperialUnitsOfMeasurement.Percentage},
             {Properties.Resources.MethaneProducingCapacityOfManure, null},
             {Properties.Resources.N2ODirectEmissionFactor, ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN},
             {Properties.Resources.EmissionFactorVolatilization, null},
             {Properties.Resources.VolatilizationFraction, null},
             {Properties.Resources.EmissionFactorLeaching, null},
             {Properties.Resources.FractionLeaching, null},
             {Properties.Resources.AshContent, ImperialUnitsOfMeasurement.Percentage},
             {Properties.Resources.VolatileSolidsExcretion, ImperialUnitsOfMeasurement.PoundsPerPound},
             {Properties.Resources.YearlyEntericMethaneRate,  ImperialUnitsOfMeasurement.PoundPerHeadPerYear},
        };

        public bool IsHeaderOptional(string s)
        {
            return false;
        }
        public Dictionary<string, bool> MissingHeaders { get; set; } = new Dictionary<string, bool>();
    }
}
