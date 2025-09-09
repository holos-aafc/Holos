using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class TillageTypeStringConverter : ConverterBase
    {
        public TillageType Convert(string input)
        {
            switch (GetLettersAsLowerCase(input))
            {
                case "notill":
                case "nt":
                    return TillageType.NoTill;
                case "reduced":
                case "rt":
                    return TillageType.Reduced;
                case "intensive":
                case "it":
                case "conventional":
                    return TillageType.Intensive;
            }

            throw new Exception(string.Format(Resources.ExceptionUnknownTillageType, "input"));
        }
    }
}