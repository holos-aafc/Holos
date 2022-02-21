using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class HardinessZoneStringConverter : ConverterBase
    {
        public HardinessZone Convert(string input)
        {
            //departure from standard string cleaning pattern because hardiness zones cannot be identified by letters alone
            input = input.ToLowerInvariant();
            input = input.Trim();
            switch (input)
            {
                case "0a":
                    return HardinessZone.H0a;
                case "0b":
                    return HardinessZone.H0b;
                case "1a":
                    return HardinessZone.H1a;
                case "1b":
                    return HardinessZone.H1b;
                case "2a":
                    return HardinessZone.H2a;
                case "2b":
                    return HardinessZone.H2b;
                case "3a":
                    return HardinessZone.H3a;
                case "3b":
                    return HardinessZone.H3b;
                case "4a":
                    return HardinessZone.H4a;
                case "4b":
                    return HardinessZone.H4b;
                case "5a":
                    return HardinessZone.H5a;
                case "5b":
                    return HardinessZone.H5b;
                case "6a":
                    return HardinessZone.H6a;
                case "6b":
                    return HardinessZone.H6b;
                case "7a":
                    return HardinessZone.H7a;
                case "7b":
                    return HardinessZone.H7b;
                case "8a":
                    return HardinessZone.H8a;
                case "8b":
                    return HardinessZone.H8b;
                case "9a":
                    return HardinessZone.H9a;
                default:
                    throw new Exception(string.Format(Resources.ExceptionUnknownHardinessZoneString, input));
            }
        }
    }
}