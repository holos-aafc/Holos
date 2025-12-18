using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum Seasons
    {
        [LocalizedDescription("EnumSpring", typeof(Resources))]
        Spring,

        [LocalizedDescription("EnumSummer", typeof(Resources))]
        Summer,

        [LocalizedDescription("EnumFall", typeof(Resources))]
        Fall,

        [LocalizedDescription("EnumWinter", typeof(Resources))]
        Winter,
    }
}