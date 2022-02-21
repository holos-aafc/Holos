using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum SoilTexture
    {
        [LocalizedDescription("Fine", typeof(Resources))]
        Fine,

        [LocalizedDescription("Medium", typeof(Resources))]
        Medium,

        [LocalizedDescription("Coarse", typeof(Resources))]
        Coarse
    }
}