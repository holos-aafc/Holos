using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum AnaerobicDigestorSeparatorType
    {
        [LocalizedDescription("EnumCentrifuge", typeof(Resources))]
        Centrifuge,

        [LocalizedDescription("EnumBeltPress", typeof(Resources))]
        BeltPress
    }
}