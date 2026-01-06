using H.Core.Enumerations;
using System.Collections.Generic;
using System.Diagnostics;

namespace H.Core.Converters
{
    public class SoilFunctionalCategoryStringConverter : ConverterBase
    {
        #region Properties

        public static Dictionary<string, SoilFunctionalCategory> Cache { get; set; } = new Dictionary<string, SoilFunctionalCategory>();

        #endregion

        public SoilFunctionalCategory Convert(string input)
        {
            SoilFunctionalCategory result;

            if (Cache.ContainsKey(input))
            {
                return Cache[input];
            }

            switch (this.GetLettersAsLowerCase(input))
            {
                case "brownchernozem":
                    result =  SoilFunctionalCategory.BrownChernozem;
                    break;

                case "darkbrownchernozem":
                    result =  SoilFunctionalCategory.DarkBrownChernozem;
                    break;

                case "blackgraychernozem":
                    result =  SoilFunctionalCategory.BlackGrayChernozem;
                    break;

                case "all":
                    result =  SoilFunctionalCategory.All;
                    break;

                case "brown":
                    result =  SoilFunctionalCategory.Brown;
                    break;

                case "darkbrown":
                    result =  SoilFunctionalCategory.DarkBrown;
                    break;

                case "black":
                    result =  SoilFunctionalCategory.Black;
                    break;

                case "organic":
                    result =  SoilFunctionalCategory.Organic;
                    break;

                case "easterncanada":
                case "east":
                    result =  SoilFunctionalCategory.EasternCanada;
                    break;

                default:
                    {
                        Trace.TraceError($"{nameof(SoilFunctionalCategoryStringConverter)}: Soil functional category '{input}' not mapped, result = ing default value.");

                        result =  SoilFunctionalCategory.NotApplicable;

                        break;
                    }
            }

            Cache.Add(input, result);

            return result;

        }
    }
}
