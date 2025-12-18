using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Converters
{
    public class DigestateStateStringConverter : ConverterBase
    {
        public DigestateState Convert(string input)
        {
            switch (GetLettersAsLowerCase(input))
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
                    Trace.TraceError($"{nameof(DigestateStateStringConverter)}.{nameof(Convert)} " +
                                     $"unknown DigestateState type: {input}. Returning {DigestateState.Raw}.");
                    return DigestateState.Raw;
                }
            }
        }
    }
}