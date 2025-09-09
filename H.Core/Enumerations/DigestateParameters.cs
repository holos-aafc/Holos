using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum DigestateParameters
    {
        [LocalizedDescription("EnumRawMaterial", typeof(Resources))]
        RawMaterial,

        [LocalizedDescription("EnumTotalSolids", typeof(Resources))]
        TotalSolids,

        [LocalizedDescription("EnumVolatileSolids", typeof(Resources))]
        VolatileSolids,

        [LocalizedDescription("EnumTotalAmmoniaNitrogen", typeof(Resources))]
        TotalAmmoniaNitrogen,

        [LocalizedDescription("EnumOrganicNitrogen", typeof(Resources))]
        OrganicNitrogen,

        [LocalizedDescription("EnumTotalCarbon", typeof(Resources))]
        TotalCarbon,

        [LocalizedDescription("EnumTotalNitrogen", typeof(Resources))]
        TotalNitrogen
    }
}