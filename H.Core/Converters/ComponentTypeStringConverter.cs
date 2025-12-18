using System.Diagnostics;
using System.Runtime.InteropServices;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Converters
{
    public class ComponentTypeStringConverter : ConverterBase
    {
        public ComponentType Convert(string input)
        {
            var cleanedInput = base.GetLettersAsLowerCase(input);
            switch (cleanedInput)
            {
                case "backgrounding":
                    return ComponentType.Backgrounding;

                case "growertofinish":
                    return ComponentType.SwineGrowers;

                case "isowean":
                    return ComponentType.IsoWean;

                case "farrowtowean":
                    return ComponentType.FarrowToWean;

                case "farrowtofinish":
                    return ComponentType.FarrowToFinish;

                default:
                {
                    Trace.TraceError($"{nameof(ComponentTypeStringConverter)}.{nameof(ComponentTypeStringConverter.Convert)}: unknown component type {input}. Returning {ComponentType.Backgrounding}");

                    return ComponentType.Backgrounding;
                }
            }
        }
    }
}