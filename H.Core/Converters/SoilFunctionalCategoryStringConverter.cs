using H.Core.Enumerations;
using H.Core.Properties;
using System;
using System.Diagnostics;

namespace H.Core.Converters
{
    public class SoilFunctionalCategoryStringConverter : ConverterBase
    {
        public SoilFunctionalCategory Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "brownchernozem":
                    return SoilFunctionalCategory.BrownChernozem;
                case "darkbrownchernozem":
                    return SoilFunctionalCategory.DarkBrownChernozem;
                case "blackgraychernozem":
                    return SoilFunctionalCategory.BlackGrayChernozem;
                case "all":
                    return SoilFunctionalCategory.All;
                case "brown":
                    return SoilFunctionalCategory.Brown;
                case "darkbrown":
                    return SoilFunctionalCategory.DarkBrown;
                case "black":
                    return SoilFunctionalCategory.Black;
                case "organic":
                    return SoilFunctionalCategory.Organic;
                case "easterncanada":
                case "east":
                    return SoilFunctionalCategory.EasternCanada;
                default:
                    {
                        Trace.TraceError($"{nameof(SoilFunctionalCategoryStringConverter)}: Soil functional category '{input}' not mapped, returning default value.");

                        return SoilFunctionalCategory.NotApplicable;
                    }
            }

        }
    }
}
