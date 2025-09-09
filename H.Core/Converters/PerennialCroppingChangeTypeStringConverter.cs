using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class PerennialCroppingChangeTypeStringConverter : ConverterBase
    {
        public PerennialCroppingChangeType Convert(string input)
        {
            switch (GetLettersAsLowerCase(input))
            {
                case "increaseinperennialcroppingarea":
                    return PerennialCroppingChangeType.IncreaseInPerennialCroppingArea;
                case "decreaseinperennialcroppingarea":
                    return PerennialCroppingChangeType.DecreaseInPerennialCroppingArea;
                default:
                    throw new Exception(string.Format(Resources.NotAValidPerennialCroppingChangeType, input));
            }
        }
    }
}