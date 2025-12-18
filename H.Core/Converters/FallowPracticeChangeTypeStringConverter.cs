using H.Core.Enumerations;
using H.Core.Properties;
using System;

namespace H.Core.Converters
{
    public class FallowPracticeChangeTypeStringConverter : ConverterBase
    { 
        public FallowPracticeChangeType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "fallowcroppingtocontinous":
                    return FallowPracticeChangeType.FallowCroppingToContinous;
                case "continoustofallowcropping":
                    return FallowPracticeChangeType.ContinousToFallowCropping;
                default:
                    throw new Exception(String.Format(Resources.NotAValidFallowPracticeChangeType, input));
            }
        }

    }
}
