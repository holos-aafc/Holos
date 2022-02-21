using H.Core.Enumerations;
using H.Core.Properties;
using System;

namespace H.Core.Converters
{
    public class DairyFeedClassTypeConverter : ConverterBase
    {
        public DairyFeedClassType Convert(string input)
        {
            input = this.GetLettersAsLowerCase(input);
            switch (input)
            {
                case "animal":
                    return DairyFeedClassType.Animal;
                case "conc":
                    return DairyFeedClassType.Conc;
                case "forage":
                    return DairyFeedClassType.Forage;
                case "fat":
                    return DairyFeedClassType.Fat;
                case "fatg":
                    return DairyFeedClassType.FatG;
                default:
                    throw new Exception(string.Format(Resources.UnknownDairyFeedClassType, input));
            }

        }
    }
}
