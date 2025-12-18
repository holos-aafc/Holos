using System;
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

        /// <summary>
        ///     Also known as 'compost - intensive windrow'
        /// </summary>
        [LocalizedDescription("CompostIntensiveHandlingSystemName", typeof(Resources))]
        CompostIntensive,

        /// <summary>
        ///     Also known as 'compost - passive windrow'
        /// </summary>
        [LocalizedDescription("CompostPassiveHandlingSystemName", typeof(Resources))]
        CompostPassive,

        [LocalizedDescription("DailySpreadHandlingSystemName", typeof(Resources))]
        DailySpread,

        [LocalizedDescription("DeepBeddingHandlingSystemName", typeof(Resources))]
        DeepBedding,

        /// <summary>
        ///     Also known as 'Deep pit under barn'
        /// </summary>
        [LocalizedDescription("DeepPitHandlingSystemName", typeof(Resources))]
        DeepPit,

        [LocalizedDescription("EnumLiquid", typeof(Resources))]
        Liquid,

        [LocalizedDescription("LiquidCrustHandlingSystemName", typeof(Resources))] [Obsolete]
        LiquidCrust,

        [LocalizedDescription("EnumLiquidSeparated", typeof(Resources))] [Obsolete]
        LiquidSeparated,

        /// <summary>
        ///     Also known as 'Liquid/Slurry with no natural crust'
        /// </summary>
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

        [LocalizedDescription("EnumSlurry", typeof(Resources))] [Obsolete]
        Slurry,

        [LocalizedDescription("SlurryWithNaturalCrust", typeof(Resources))] [Obsolete]
        SlurryWithNaturalCrust,

        [LocalizedDescription("SlurryWithoutNaturalCrust", typeof(Resources))] [Obsolete]
        SlurryWithoutNaturalCrust,

        /// <summary>
        ///     Also known as 'Solid storage (stockpiled)'
        /// </summary>
        [LocalizedDescription("SolidStorageHandlingSystemName", typeof(Resources))]
        SolidStorage,

        [LocalizedDescription("EnumCustom", typeof(Resources))]
        Custom,

        [LocalizedDescription("EnumPitLagoonNoCover", typeof(Resources))] [Obsolete]
        PitLagoonNoCover,

        /// <summary>
        ///     Also known as 'Liquid/Slurry with natural crust'
        /// </summary>
        [LocalizedDescription("EnumLiquidWithNaturalCrust", typeof(Resources))]
        LiquidWithNaturalCrust,

        /// <summary>
        ///     Also known as Liquid/Slurry with solid cover
        /// </summary>
        [LocalizedDescription("EnumLiquidWithSolidCover", typeof(Resources))]
        LiquidWithSolidCover,

        /// <summary>
        ///     (Swine system)
        /// </summary>
        [LocalizedDescription("EnumCompostedInVessel", typeof(Resources))]
        CompostedInVessel,

        /// <summary>
        ///     (Poultry system) No different than 'Solid Storage' but poultry solid storage needs the term 'litter' which is
        ///     incorrect to use in the case of cattle 'Solid Storage' since
        ///     there is no 'litter' only 'bedding' when considering the cattle system
        /// </summary>
        [LocalizedDescription("EnumSolidStorageWithOrWithoutLitter", typeof(Resources))]
        SolidStorageWithOrWithoutLitter
    }
}