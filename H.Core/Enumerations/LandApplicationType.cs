using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum LandApplicationType
    {
        [LocalizedDescription("TilledLand", typeof(Resources))]
        TilledLand,

        [LocalizedDescription("UntilledLand", typeof(Resources))]
        UntilledLand,
    }
}
