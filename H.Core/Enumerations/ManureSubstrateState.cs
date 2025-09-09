using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum ManureSubstrateState
    {
        [LocalizedDescription("EnumFreshManure", typeof(Resources))]
        Fresh,

        [LocalizedDescription("EnumStoredManure", typeof(Resources))]
        Stored
    }
}