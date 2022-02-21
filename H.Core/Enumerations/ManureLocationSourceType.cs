using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum ManureLocationSourceType
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("EnumLivestock", typeof(Resources))]
        Livestock,

        [LocalizedDescription("EnumImported", typeof(Resources))]
        Imported
    }
}