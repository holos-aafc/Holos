using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace H.Core.Enumerations
{
    public static class HousingTypeExtensions
    {
        public static bool IsFreeStall(this HousingType housingType)
        {
            switch (housingType)
            {
                case HousingType.SmallFreeStall:
                case HousingType.LargeFreeStall:
                case HousingType.FreeStallBarnFlushing:
                case HousingType.FreeStallBarnMilkParlourSlurryFlushing:
                case HousingType.FreeStallBarnSlurryScraping:
                case HousingType.FreeStallBarnSolidLitter:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsTieStall(this HousingType housingType)
        {
            switch (housingType)
            {
                case HousingType.TieStall:
                case HousingType.TieStallSlurry:
                case HousingType.TieStallSolidLitter:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsElectricalConsumingHousingType(this HousingType housingType)
        {
            if (housingType.IsFreeStall() || 
                housingType.IsBarn() ||
                housingType.IsTieStall() ||
                housingType.IsFeedlot())
            {
                return true;
            }

            switch (housingType)
            {
                case HousingType.DryLot:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsIndoorHousing(this HousingType housingType)
        {
            switch (housingType)
            {
                case HousingType.HousedInBarn:
                case HousingType.HousedInBarnSlurry:
                case HousingType.HousedInBarnSolid:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsFeedlot(this HousingType housingType)
        {
            switch (housingType)
            {
                case HousingType.Confined:
                case HousingType.ConfinedNoBarn:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsBarn(this HousingType housingType)
        {
            switch (housingType)
            {
                case HousingType.HousedInBarn:
                case HousingType.HousedInBarnSlurry:
                case HousingType.HousedInBarnSolid:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsPasture(this HousingType housingType)
        {
            switch (housingType)
            {
                case HousingType.Pasture:
                case HousingType.EnclosedPasture:
                case HousingType.FlatPasture:
                case HousingType.GrazingOver3km:
                case HousingType.GrazingUnder3km:
                case HousingType.HillyPastureOrOpenRange:
                case HousingType.OpenRangeOrHills:
                case HousingType.SwathGrazing:
                    return true;

                default:
                    return false;
            }
        }
    }
}
