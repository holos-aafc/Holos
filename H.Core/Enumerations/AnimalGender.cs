using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum AnimalGender
    {
        [LocalizedDescription("EnumSteer", typeof(Resources))]
        Steer,

        [LocalizedDescription("EnumHeifer", typeof(Resources))]
        Heifer
    }
}