using System;
using System.Globalization;

namespace H.Infrastructure
{
    public class InfrastructureConstants
    {
        public static readonly CultureInfo EnglishCultureInfo = CreateCultureOrFallback("en-CA");
        public static readonly CultureInfo FrenchCultureInfo = CreateCultureOrFallback("fr-CA");

        private static CultureInfo CreateCultureOrFallback(string name)
        {
            try
            {
                return new CultureInfo(name);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.InvariantCulture;
            }
        }
    }
}