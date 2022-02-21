using H.Core.Enumerations;
using H.Core.Properties;
using System;

namespace H.Core.Converters
{
    public class PerennialCroppingChangeTypeStringConverter : ConverterBase
    {
        public PerennialCroppingChangeType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "increaseinperennialcroppingarea":
                    return PerennialCroppingChangeType.IncreaseInPerennialCroppingArea;
                case "decreaseinperennialcroppingarea":
                    return PerennialCroppingChangeType.DecreaseInPerennialCroppingArea;
                default:
                    throw new Exception(String.Format(Resources.NotAValidPerennialCroppingChangeType, input));

            }
        }
    }
}
