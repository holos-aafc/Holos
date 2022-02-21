using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Methods of applying manure to a field
    /// </summary>
    public enum ManureApplicationTypes
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("EnumOptionA", typeof(Resources))]
        OptionA,

        [LocalizedDescription("EnumOptionB", typeof(Resources))]
        OptionB,

        [LocalizedDescription("EnumOptionC", typeof(Resources))]
        OptionC,

        [LocalizedDescription("EnumTilledLandSolidSpread", typeof(Resources))]
        TilledLandSolidSpread,

        [LocalizedDescription("EnumUntilledLandSolidSpread", typeof(Resources))]
        UntilledLandSolidSpread,

        [LocalizedDescription("EnumSlurryBroadcasting", typeof(Resources))]
        SlurryBroadcasting,

        [LocalizedDescription("EnumDropHoseBanding", typeof(Resources))]
        DropHoseBanding,

        [LocalizedDescription("EnumShallowInjection", typeof(Resources))]
        ShallowInjection,

        [LocalizedDescription("EnumDeepInjection", typeof(Resources))]
        DeepInjection,
    }
}