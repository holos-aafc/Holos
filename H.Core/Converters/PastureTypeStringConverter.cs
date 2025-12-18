using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Converters
{
    public class PastureTypeStringConverter : ConverterBase
    {
        public PastureType Convert (string input)
        {
            switch (GetLettersAsLowerCase(input))
            {
                case "pasturegrasshighquality":
                    return PastureType.PastureGrassHigh;

                case "pasturegrassmediumquality":
                    return PastureType.PastureGrassMedium;

                case "pasturegrasslowquality":
                    return PastureType.PastureGrassLow;

                default:
                    Trace.TraceError($"{nameof(PastureTypeStringConverter)}.{nameof(Convert)}: could not parse " +
                                     $"string input: {input}. Returning {nameof(PastureType)}.{nameof(PastureType.None)}");
                    return PastureType.None;
            }
        }
    }
}
