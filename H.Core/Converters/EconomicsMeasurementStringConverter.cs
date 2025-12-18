using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using H.Core.Enumerations;

namespace H.Core.Converters
{
    public class EconomicsMeasurementStringConverter
    {
        public EconomicMeasurementUnits Convert(string measurementString)
        {
            var lower = measurementString.ToLower();

            switch (lower)
            {
                case "lb":
                    return EconomicMeasurementUnits.Pound;
                case "na":
                    return EconomicMeasurementUnits.None;
                case "t":
                    return EconomicMeasurementUnits.Tonne;
                case "bu":
                    return EconomicMeasurementUnits.Bushel;
                case "cwt":
                    return EconomicMeasurementUnits.HundredWeight;
                default:
                    Trace.TraceError($"{lower} is not a unit of measurement. Returning default 'none'");
                    return EconomicMeasurementUnits.None;
            }

        }
    }
}
