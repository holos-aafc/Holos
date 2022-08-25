using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum BeddingMaterialType
    {
        [LocalizedDescription("EnumNone", typeof(Resources))]
        None,

        [LocalizedDescription("Straw", typeof(Resources))]
        Straw,

        [LocalizedDescription("WoodChip", typeof(Resources))]
        WoodChip,

        [LocalizedDescription("SeparatedManureSolid", typeof(Resources))]
        SeparatedManureSolid,

        [LocalizedDescription("Sand", typeof(Resources))]
        Sand,

        [LocalizedDescription("StrawLong", typeof(Resources))]
        StrawLong,

        [LocalizedDescription("StrawChopped", typeof(Resources))]
        StrawChopped,

        [LocalizedDescription("Shavings", typeof(Resources))]
        Shavings,

        [LocalizedDescription("Sawdust", typeof(Resources))]
        Sawdust,

        [LocalizedDescription("PaperProducts", typeof(Resources))]
        PaperProducts,

        [LocalizedDescription("Peat", typeof(Resources))]
        Peat,

        [LocalizedDescription("Hemp", typeof(Resources))]
        Hemp,
    }
}
