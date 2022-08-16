using H.Content.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum PastureType
    {
        [LocalizedDescription("PastureGrassHighQuality", typeof(Resource))]
        PastureGrassHigh,

        [LocalizedDescription("PastureGrassMediumQuality", typeof(Resource))]
        PastureGrassMedium,

        [LocalizedDescription("PastureGrassLowQuality", typeof(Resource))]
        PastureGrassLow,

        [LocalizedDescription("None", typeof(Resource))]
        None,
    }
}
