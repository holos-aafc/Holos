using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Methods for applying fertilizer to a crop/field
    /// </summary>
    public enum FertilizerApplicationMethodologies
    {
        [LocalizedDescription("EnumBroadcast", typeof(Resources))]
        Broadcast,

        [LocalizedDescription("EnumIncorporatedOrPartiallyInjected", typeof(Resources))]
        IncorporatedOrPartiallyInjected,

        [LocalizedDescription("EnumFullyInjected", typeof(Resources))]
        FullyInjected,
    }
}