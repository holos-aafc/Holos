using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum Languages
    {
        [LocalizedDescription("EnumEnglish", typeof(Resources))]
        English,

        [LocalizedDescription("EnumFrench", typeof(Resources))]
        French,
    }
}