using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum DigestateState
    {
        /// <summary>
        ///     Represents digestate in a raw state.
        /// </summary>
        [LocalizedDescription("EnumRaw", typeof(Resources))]
        Raw,

        /// <summary>
        ///     Represents the liquid fraction of digestate.
        /// </summary>
        [LocalizedDescription("EnumLiquidPhase", typeof(Resources))]
        LiquidPhase,

        /// <summary>
        ///     Represents the solid fraction of digestate.
        /// </summary>
        [LocalizedDescription("EnumSolidPhase", typeof(Resources))]
        SolidPhase
    }
}