using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Converters
{
    public class EmissionTypeStringConverter : ConverterBase
    {
        public EmissionTypes Convert(string input)
        {
            switch (this.GetLowerCase(input))
            {
                case "entericmethane":
                    return EmissionTypes.EntericMethane;

                case "manuremethane":
                    return EmissionTypes.ManureMethane;

                case "livestockindirectn2o":
                    return EmissionTypes.LivestockIndirectN20;

                case "livestockdirectn2o":
                    return EmissionTypes.LivestockDirectN20;

                case "cropsindirectn2o":
                    return EmissionTypes.CropsIndirectN20;

                case "cropsdirectn2o":
                    return EmissionTypes.CropsDirectN20;

                case "n2o":
                case "nitrousoxide":
                    return EmissionTypes.N2O;

                case "co2":
                    return EmissionTypes.CO2;

                case "co2e":
                    return EmissionTypes.CO2e;

                case "ch4":
                case "methane":
                    return EmissionTypes.CH4;

                case "ammonia":
                case "nh3":
                    return EmissionTypes.NH3;

                case "ch4nonfossil":
                    return EmissionTypes.NonFossilCH4;
                default:
                {
                    Trace.TraceError($"{nameof(EmissionTypeStringConverter)}.{nameof(EmissionTypeStringConverter.Convert)}: unknown emissions type: {input} returning {EmissionTypes.EntericMethane}");
                    return EmissionTypes.EntericMethane;
                }
            }
        }
    }
}
