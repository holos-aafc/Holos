using System.Diagnostics;
using H.Core.Enumerations;

namespace H.Core.Converters
{
    public class BeddingMaterialTypeStringConverter : ConverterBase
    {
        public BeddingMaterialType Convert(string input)
        {
            switch (GetLettersAsLowerCase(input))
            {
                case "straw":
                    return BeddingMaterialType.Straw;
                case "woodchip":
                    return BeddingMaterialType.WoodChip;
                case "separatedmanuresolid":
                    return BeddingMaterialType.SeparatedManureSolid;
                case "sand":
                    return BeddingMaterialType.Sand;
                case "strawlong":
                    return BeddingMaterialType.StrawLong;
                case "strawchopped":
                    return BeddingMaterialType.StrawChopped;
                case "shavings":
                    return BeddingMaterialType.Shavings;
                case "sawdust":
                    return BeddingMaterialType.Sawdust;
                case "none":
                    return BeddingMaterialType.None;

                default:
                {
                    Trace.TraceError(
                        $"{nameof(BeddingMaterialTypeStringConverter)}.{nameof(Convert)} cannot find the given bedding material {input}. Returning {BeddingMaterialType.None}");
                    return BeddingMaterialType.None;
                }
            }
        }
    }
}