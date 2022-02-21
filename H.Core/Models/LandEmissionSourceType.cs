using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models
{
    public enum LandEmissionSourceType
    {
        [LocalizedDescription("EnumNotApplicable", typeof(Resources))]
        NotApplicable,

        [LocalizedDescription("EnumCropResidueInputEmissionSourceType", typeof(Resources))]
        Crop,

        [LocalizedDescription("EnumLandAppliedManureInputsEmissionSourceType", typeof(Resources))]
        Manure,

        [LocalizedDescription("EnumMineralInputsEmissionSourceType", typeof(Resources))]
        Mineral
    }
}