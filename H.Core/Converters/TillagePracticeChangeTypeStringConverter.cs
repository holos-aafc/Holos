using H.Core.Enumerations;
using H.Core.Properties;
using System;

namespace H.Core.Converters
{
    public class TillagePracticeChangeTypeStringConverter : ConverterBase
    {
        public TillagePracticeChangeType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "intensetoreduced":
                    return TillagePracticeChangeType.IntenseToReduced;
                case "intensetonone":
                    return TillagePracticeChangeType.IntenseToNone;
                case "reducedtonone":
                    return TillagePracticeChangeType.ReducedToNone;
                case "reducedtointense":
                    return TillagePracticeChangeType.ReducedToIntense;
                case "nonetoreduced":
                    return TillagePracticeChangeType.NoneToReduced;
                case "nonetointense":
                    return TillagePracticeChangeType.NoneToIntense;
                default:
                     throw new Exception(String.Format(Resources.NotAValidTillageChangeType, input));
            }
        }

    }
}
