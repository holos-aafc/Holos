using System;
using H.Core.Enumerations;
using H.Core.Properties;

namespace H.Core.Converters
{
    public class HousingStringConverter : ConverterBase
    {
        public HousingType Convert(string input)
        {
            switch (this.GetLettersAsLowerCase(input))
            {
                case "confinednobarn":
                    return HousingType.ConfinedNoBarn;
                case "housedinbarn":
                    return HousingType.HousedInBarn;
                case "enclosedpasture":
                    return HousingType.EnclosedPasture;
                case "openrangeorhills":
                    return HousingType.OpenRangeOrHills;
                case "tiestall":
                    return HousingType.TieStall;
                case "smallfreestall":
                    return HousingType.SmallFreeStall;
                case "largefreestall":
                    return HousingType.LargeFreeStall;
                case "drylot":
                    return HousingType.DryLot;
                case "grazingkmd":
                    if (input == "Grazing < 3km/d")
                    {
                        return HousingType.GrazingUnder3km;
                    }
                    else
                    {
                        return HousingType.GrazingOver3km;
                    }
                case "confined":
                    return HousingType.Confined;
                case "flatpasture":
                    return HousingType.FlatPasture;
                case "hillypastureopenrange":
                    return HousingType.HillyPastureOrOpenRange;
                default:
                    throw new Exception(string.Format(Resources.ExceptionUnknownHousing, input));
            }
        }
    }
}