using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum MeasurementSystemType
    {
        [LocalizedDescription("Metric", typeof(Resources))]
        Metric,

        [LocalizedDescription("Imperial", typeof(Resources))]
        Imperial
    }
}