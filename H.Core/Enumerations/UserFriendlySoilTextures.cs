using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum UserFriendlySoilTextures
    {
        [LocalizedDescription("EnumClay", typeof(Resources))]
        Clay,

        [LocalizedDescription("EnumLoam", typeof(Resources))]
        Loam,

        [LocalizedDescription("EnumSand", typeof(Resources))]
        Sand,
    }
}