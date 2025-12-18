using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum EmissionTypes
    {
        [LocalizedDescription("EnumEntericMethane", typeof(Resources))]
        EntericMethane,

        [LocalizedDescription("EnumManureMethane", typeof(Resources))]
        ManureMethane,

        [LocalizedDescription("EnumLivestockIndirectN2O", typeof(Resources))]
        LivestockIndirectN20,

        [LocalizedDescription("EnumLivestockDirectN20", typeof(Resources))]
        LivestockDirectN20,

        [LocalizedDescription("EnumCropsIndirectN2O", typeof(Resources))]
        CropsIndirectN20,

        [LocalizedDescription("EnumCropsDirectN2O", typeof(Resources))]
        CropsDirectN20,

        [LocalizedDescription("EnumN2O", typeof(Resources))]
        N2O,

        [LocalizedDescription("EnumCO2", typeof(Resources))]
        CO2,

        [LocalizedDescription("EnumCO2e", typeof(Resources))]
        CO2e,

        [LocalizedDescription("EnumCH4", typeof(Resources))]
        CH4,

        [LocalizedDescription("EnumNH3", typeof(Resources))]
        NH3,

        [LocalizedDescription("EnumNonFossilCH4", typeof(Resources))]
        NonFossilCH4,

    }
}