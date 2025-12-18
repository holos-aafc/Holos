using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class SoilTextureStringConverter : ConverterBase
    {
        public SoilTexture Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "fine":
                    return SoilTexture.Fine;
                case "coarse":
                    return SoilTexture.Coarse;
                case "medium":
                    return SoilTexture.Medium;
                default:
                    throw new Exception(string.Format(Resources.ExceptionUnknownSoilTextureString, input));
            }
        }
    }
}