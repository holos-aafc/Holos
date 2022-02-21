using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum ForageActivities
    {
        [LocalizedDescription("EnumGrazed", typeof(Resources))]
        Grazed,

        [LocalizedDescription("EnumHayed", typeof(Resources))]
        Hayed,

        [LocalizedDescription("EnumSilage", typeof(Resources))]
        Silage,

        [LocalizedDescription("EnumSwath", typeof(Resources))]
        Swath
    }
}