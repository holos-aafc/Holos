using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Converters
{
    public class ModelParameterStringConverter : ConverterBase
    {
        public ModelParameters Convert(string input)
        {
            switch(this.GetLowerCase(input))
            {
                case "tillfac":
                    return ModelParameters.TillageModifier;

                case "ws":
                    return ModelParameters.SlopeParameter;

                case "kfaca":
                    return ModelParameters.DecayRateActive;

                case "kfacs":
                    return ModelParameters.DecayRateSlow;

                case "kfacp":
                    return ModelParameters.DecayRatePassive;

                case "f1":
                    return ModelParameters.FractionMetabolicDMActivePool;

                case "f2":
                    return ModelParameters.FractionStructuralDMActivePool;

                case "f3":
                    return ModelParameters.FractionStructuralDMSlowPool;

                case "f5":
                    return ModelParameters.FractionActiveDecayToPassive;

                case "f6":
                    return ModelParameters.FractionSlowDecayToPassive;

                case "f7":
                    return ModelParameters.FractionSlowDecayToActive;

                case "f8":
                    return ModelParameters.FractionPassiveDecayToActive;

                case "topt":
                    return ModelParameters.OptimumTemperature;

                case "tmax":
                    return ModelParameters.MaximumAvgTemperature;
    
                default:
                    Trace.TraceError($"{nameof(ModelParameterStringConverter)}.{nameof(ModelParameterStringConverter.Convert)}: unknown model parameter: {input}");
                    return ModelParameters.TillageModifier;
            }
        }
    }
}
