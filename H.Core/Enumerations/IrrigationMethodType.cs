using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum IrrigationMethodType
    {
        [LocalizedDescription("EnumFlooding", typeof(Resources))]
        Flooding,

        [LocalizedDescription("EnumPivot", typeof(Resources))]
        Pivot
    }
}