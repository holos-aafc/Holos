using H.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.Interfaces;

namespace H.CLI.ComponentKeys
{
    public class OtherLiveStockKeys : IComponentKeys
    {
        public Dictionary<string, ImperialUnitsOfMeasurement?> keys { get; set; } = new Dictionary<string, ImperialUnitsOfMeasurement?>
        {
            {Properties.Resources.Key_Name, null },
            {Properties.Resources.GroupName, null},
            {Properties.Resources.GroupType, null},
            {Properties.Resources.ManagementPeriodName, null},
            {Properties.Resources.ManagementPeriodStartDate, null},
            {Properties.Resources.ManagementPeriodDays, null},
            {Properties.Resources.NumberOfAnimals, null},
            {Properties.Resources.YearlyManureMethaneRate,  ImperialUnitsOfMeasurement.PoundPerHeadPerYear},
            {Properties.Resources.YearlyNitrogenExcretionRate,  ImperialUnitsOfMeasurement.PoundPerHeadPerYear},
            {Properties.Resources.YearlyEntericMethaneRate,  ImperialUnitsOfMeasurement.PoundPerHeadPerYear},
            {Properties.Resources.N2ODirectEmissionFactor, ImperialUnitsOfMeasurement.PoundsN2ONPerPoundN},
            {Properties.Resources.VolatilizationFraction, null },
        };

        public bool IsHeaderOptional(string s)
        {
            return false;
        }
        public Dictionary<string, bool> MissingHeaders { get; set; } = new Dictionary<string, bool>();
    }
}
