using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum GrazingSystemTypes
    {
        [LocalizedDescription("EnumContinuousAndMobGrazing", typeof(Resources))]
        ContinuousAndMobGrazing,

        [LocalizedDescription("EnumRotationalGrazing", typeof(Resources))]
        RotationalGrazing,

        [LocalizedDescription("EnumSwitchbackGrazing", typeof(Resources))]
        SwitchbackGrazing,

        [LocalizedDescription("EnumStripGrazing", typeof(Resources))]
        StripGrazing
    }
}