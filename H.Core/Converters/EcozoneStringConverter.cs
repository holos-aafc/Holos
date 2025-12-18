using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class EcozoneStringConverter : ConverterBase
    {
        public Ecozone Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "atlanticmaritimes":
                    return Ecozone.AtlanticMaritimes;
                case "borealplains":
                    return Ecozone.BorealPlains;
                case "borealshieldeast":
                    return Ecozone.BorealShieldEast;
                case "borealshieldwest":
                    return Ecozone.BorealShieldWest;
                case "mixedwoodplains":
                    return Ecozone.MixedwoodPlains;
                case "montanecordillera":
                    return Ecozone.MontaneCordillera;
                case "pacificmaritime":
                case "pacificmaritimes":
                    return Ecozone.PacificMaritime;
                case "semiaridprairies":
                    return Ecozone.SemiaridPrairies;
                case "subhumidprairies":
                    return Ecozone.SubhumidPrairies;
                default:
                    throw new Exception(string.Format(Resources.ExceptionUnknownEcozoneString, input));
            }
        }
    }
}