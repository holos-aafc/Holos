using H.Infrastructure;

namespace H.Core.Models
{
    public enum ComponentCategory
    {
        [LocalizedDescription("EnumLandManagement", typeof(Properties.Resources))]
        LandManagement,

        [LocalizedDescription("EnumBeefProduction", typeof(Properties.Resources))]
        BeefProduction,

        [LocalizedDescription("EnumDairy", typeof(Properties.Resources))]
        Dairy,

        [LocalizedDescription("EnumSwine", typeof(Properties.Resources))]
        Swine,

        [LocalizedDescription("EnumPoultry", typeof(Properties.Resources))]
        Poultry,

        [LocalizedDescription("EnumOtherLivestock", typeof(Properties.Resources))]
        OtherLivestock,

        [LocalizedDescription("EnumSheep", typeof(Properties.Resources))]
        Sheep,

        [LocalizedDescription("EnumInfrastructure", typeof(Properties.Resources))]
        Infrastructure,
    }
}