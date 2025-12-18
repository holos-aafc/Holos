using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum FallowPracticeChangeType
    {
        [LocalizedDescription("FallowCroppingToContinous", typeof(Resources))]
        FallowCroppingToContinous,

        [LocalizedDescription("ContinousToFallowCropping", typeof(Resources))]
        ContinousToFallowCropping,
    }
}
