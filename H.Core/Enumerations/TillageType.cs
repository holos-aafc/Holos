using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum TillageType
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("ReducedTillage", typeof(Resources))]
        Reduced,

        /// <summary>
        /// This was called 'None' in version 3.
        /// </summary>
        [LocalizedDescription("NoTillTillage", typeof(Resources))]
        NoTill,

        /// <summary>
        /// This was called 'Intense' in version 3.
        /// </summary>
        [LocalizedDescription("IntensiveTillage", typeof(Resources))]
        Intensive,
    }
}