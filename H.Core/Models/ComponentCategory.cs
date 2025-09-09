using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models
{
    public enum ComponentCategory
    {
        [LocalizedDescription("EnumLandManagement", typeof(Resources))]
        LandManagement,

        [LocalizedDescription("EnumBeefProduction", typeof(Resources))]
        BeefProduction,

        [LocalizedDescription("EnumDairy", typeof(Resources))]
        Dairy,

        [LocalizedDescription("EnumSwine", typeof(Resources))]
        Swine,

        [LocalizedDescription("EnumPoultry", typeof(Resources))]
        Poultry,

        [LocalizedDescription("EnumOtherLivestock", typeof(Resources))]
        OtherLivestock,

        [LocalizedDescription("EnumSheep", typeof(Resources))]
        Sheep,

        [LocalizedDescription("EnumInfrastructure", typeof(Resources))]
        Infrastructure
    }
}