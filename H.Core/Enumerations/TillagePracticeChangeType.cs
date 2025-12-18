using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum TillagePracticeChangeType
    {
        [LocalizedDescription("IntenseToReduced", typeof(Resources))]
        IntenseToReduced,

        [LocalizedDescription("IntenseToNone", typeof(Resources))]
        IntenseToNone,

        [LocalizedDescription("ReducedToNone", typeof(Resources))]
        ReducedToNone,

        [LocalizedDescription("ReducedToIntense", typeof(Resources))]
        ReducedToIntense,

        [LocalizedDescription("NoneToReduced", typeof(Resources))]
        NoneToReduced,

        [LocalizedDescription("NoneToIntense", typeof(Resources))]
        NoneToIntense,
    }
}
