using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class FallowPracticeChangeTypeStringConverter : ConverterBase
    {
        public FallowPracticeChangeType Convert(string input)
        {
            switch (GetLettersAsLowerCase(input))
            {
                case "fallowcroppingtocontinous":
                    return FallowPracticeChangeType.FallowCroppingToContinous;
                case "continoustofallowcropping":
                    return FallowPracticeChangeType.ContinousToFallowCropping;
                default:
                    throw new Exception(string.Format(Resources.NotAValidFallowPracticeChangeType, input));
            }
        }
    }
}