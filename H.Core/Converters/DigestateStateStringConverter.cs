using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using System.Diagnostics;

namespace H.Core.Converters
{
    public class DigestateStateStringConverter : ConverterBase
    {
        public DigestateState Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "raw":
                case "rawdigestate":
                    return DigestateState.Raw;

                case "liquid":
                case "liquidfraction":
                case "liquidphase":
                    return DigestateState.LiquidPhase;

                case "solid":
                case "solidfraction":
                case "solidphase":
                    return DigestateState.SolidPhase;

                default:
                    {
                        Trace.TraceError($"{nameof(DigestateStateStringConverter)}.{nameof(DigestateStateStringConverter.Convert)} " +
                            $"unknown DigestateState type: {input}. Returning {DigestateState.Raw}.");
                        return DigestateState.Raw;
                    }
            }
        }
    }
}
