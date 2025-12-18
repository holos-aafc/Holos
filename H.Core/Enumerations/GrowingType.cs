using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum GrowingType
    {

        Annual,

        Perennial,

        [LocalizedDescription("CoverCropGrowingType", typeof(Resources))]
        CoverCrop,

        Grassland,

        [LocalizedDescription("RootCropGrowingType", typeof(Resources))]
        RootCrop,

        Fallow,

        [LocalizedDescription("EnumSilage", typeof(Resources))]
        Silage,

        [LocalizedDescription("SmallGrainCereals", typeof(Resources))]
        SmallGrains,

        [LocalizedDescription("Oilseeds", typeof(Resources))]
        OilSeeds,

        [LocalizedDescription("EnumOtherFieldCrops", typeof(Resources))]
        OtherFieldCrops,

        [LocalizedDescription("PulseCrops", typeof(Resources))]
        PulseCrops,
    }
}
