using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum ManureStateType
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("AnaerobicDigesterHandlingSystemName", typeof(Resources))]
        AnaerobicDigester,

        [LocalizedDescription("EnumComposted", typeof(Resources))]
        Composted,

        [LocalizedDescription("CompostIntensiveHandlingSystemName", typeof(Resources))]
        CompostIntensive,

        [LocalizedDescription("CompostPassiveHandlingSystemName", typeof(Resources))]
        CompostPassive,

        [LocalizedDescription("DailySpreadHandlingSystemName", typeof(Resources))]
        DailySpread,

        [LocalizedDescription("DeepBeddingHandlingSystemName", typeof(Resources))]
        DeepBedding,

        [LocalizedDescription("DeepPitHandlingSystemName", typeof(Resources))]
        DeepPit,

        [LocalizedDescription("EnumLiquid", typeof(Resources))]
        Liquid,

        [LocalizedDescription("LiquidCrustHandlingSystemName", typeof(Resources))]
        LiquidCrust,

        [LocalizedDescription("EnumLiquidSeparated", typeof(Resources))]
        LiquidSeparated,

        [LocalizedDescription("LiquidNoCrustHandlingSystemName", typeof(Resources))]
        LiquidNoCrust,

        [LocalizedDescription("PastureHandlingSystemName", typeof(Resources))]
        Pasture,

        [LocalizedDescription("EnumRange", typeof(Resources))]
        Range,

        [LocalizedDescription("EnumPaddock", typeof(Resources))]
        Paddock,

        [LocalizedDescription("EnumSolid", typeof(Resources))]
        Solid,
        
        [LocalizedDescription("EnumSlurry", typeof(Resources))]
        Slurry,

        [LocalizedDescription("SlurryWithNaturalCrust", typeof(Resources))]
        SlurryWithNaturalCrust,

        [LocalizedDescription("SlurryWithoutNaturalCrust", typeof(Resources))]
        SlurryWithoutNaturalCrust,

        /// <summary>
        /// Also known as 'stockpiled'
        /// </summary>
        [LocalizedDescription("SolidStorageHandlingSystemName", typeof(Resources))]
        SolidStorage,

        [LocalizedDescription("EnumCustom", typeof(Resources))]
        Custom        ,

        [LocalizedDescription("EnumPitLagoonNoCover", typeof(Resources))]
        PitLagoonNoCover,

        [LocalizedDescription("EnumLiquidWithNaturalCrust", typeof(Resources))]
        LiquidWithNaturalCrust,

        [LocalizedDescription("EnumLiquidWithSolidCover", typeof(Resources))]
        LiquidWithSolidCover,
    }
}