using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum PerennialCroppingChangeType
    {
        [LocalizedDescription("IncreaseInPerennialCroppingArea", typeof(Resources))]
        IncreaseInPerennialCroppingArea,

        [LocalizedDescription("DecreaseInPerennialCroppingArea", typeof(Resources))]
        DecreaseInPerennialCroppingArea,
    }
}
