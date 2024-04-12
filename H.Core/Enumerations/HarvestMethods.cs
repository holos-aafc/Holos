using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum HarvestMethods
    {
        [LocalizedDescription("EnumSilage", typeof(Resources))]
        Silage,

        [LocalizedDescription("EnumSwathing", typeof(Resources))]
        Swathing,

        [LocalizedDescription("EnumGreenManure", typeof(Resources))]
        GreenManure,

        [LocalizedDescription("EnumCashCrop", typeof(Resources))]
        CashCrop,

        /// <summary>
        /// Used for fallow, etc.
        /// </summary>
        [LocalizedDescription("EnumNone", typeof(Resources))]
        None,

        [LocalizedDescription("EnumStubbleGrazing", typeof(Resources))]
        StubbleGrazing,
    }
}