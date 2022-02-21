using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum HousingType
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("ConfinedNoBarn", typeof(Resources))]
        ConfinedNoBarn,

        [LocalizedDescription("HousedInBarn", typeof(Resources))]
        HousedInBarn,

        [LocalizedDescription("HousedEwes", typeof(Resources))]
        HousedEwes,

        [LocalizedDescription("HousedInBarnSolid", typeof(Resources))]
        HousedInBarnSolid,

        [LocalizedDescription("HousedInBarnSlurry", typeof(Resources))]
        HousedInBarnSlurry,

        [LocalizedDescription("EnclosedPasture", typeof(Resources))]
        EnclosedPasture,

        [LocalizedDescription("OpenRangeOrHills", typeof(Resources))]
        OpenRangeOrHills,

        [LocalizedDescription("TieStall", typeof(Resources))]
        TieStall,

        [LocalizedDescription("SmallFreeStall", typeof(Resources))]
        SmallFreeStall,

        [LocalizedDescription("LargeFreeStall", typeof(Resources))]
        LargeFreeStall,

        [LocalizedDescription("GrazingUnder3kmd", typeof(Resources))]
        GrazingUnder3km,

        [LocalizedDescription("GrazingOver3kmd", typeof(Resources))]
        GrazingOver3km,

        [LocalizedDescription("Confined", typeof(Resources))]
        Confined,

        [LocalizedDescription("FlatPasture", typeof(Resources))]
        FlatPasture,

        [LocalizedDescription("HillyPastureOrOpenRange", typeof(Resources))]
        HillyPastureOrOpenRange,

        [LocalizedDescription("Pasture", typeof(Resources))]
        Pasture,

        /// <summary>
        /// Also known as 'Standing or exercise yard'
        /// </summary>
        [LocalizedDescription("Drylot", typeof(Resources))]
        DryLot,

        [LocalizedDescription("SwathGrazing", typeof(Resources))]
        SwathGrazing,

        [LocalizedDescription("EnumCustom", typeof(Resources))]
        Custom,

        [LocalizedDescription("EnumFreeStallBarnSolidLitter", typeof(Resources))]
        FreeStallBarnSolidLitter,

        [LocalizedDescription("EnumFreeStallBarnSlurryScraping", typeof(Resources))]
        FreeStallBarnSlurryScraping,

        [LocalizedDescription("EnumFreeStallBarnFlushing", typeof(Resources))]
        FreeStallBarnFlushing,

        [LocalizedDescription("EnumFreeStallBarnMilkParlourSlurryFlushing", typeof(Resources))]
        FreeStallBarnMilkParlourSlurryFlushing,

        [LocalizedDescription("EnumTieStallSolidLitter", typeof(Resources))]
        TieStallSolidLitter,

        [LocalizedDescription("EnumTieStallSlurry", typeof(Resources))]
        TieStallSlurry,
    }
}