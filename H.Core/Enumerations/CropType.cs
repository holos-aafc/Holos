using System.Windows.Media.TextFormatting;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// When adding new crops to this enumeration, add the new crops to the end of the list. Do not alphabetize the crops in this list since farm files store a numerical
    /// value for crop view items which is the expected numerical position within this list.
    /// </summary>
    public enum CropType
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("Barley", typeof(Resources))]
        Barley,

        [LocalizedDescription("BarleySilage", typeof(Resources))]
        BarleySilage,

        [LocalizedDescription("EnumBeansDryField", typeof(Resources))]
        BeansDryField,

        [LocalizedDescription("EnumBerriesAndGraps", typeof(Resources))]
        BerriesAndGrapes,

        [LocalizedDescription("Buckwheat", typeof(Resources))]
        Buckwheat,

        [LocalizedDescription("CanarySeed", typeof(Resources))]
        CanarySeed,

        [LocalizedDescription("Canola", typeof(Resources))]
        Canola,

        [LocalizedDescription("Chickpeas", typeof(Resources))]
        Chickpeas,

        [LocalizedDescription("EnumCorn", typeof(Resources))]
        Corn,

        [LocalizedDescription("ColouredWhiteFabaBeans", typeof(Resources))]
        ColouredWhiteFabaBeans,

        [LocalizedDescription("DryPeas", typeof(Resources))]
        DryPeas,

        [LocalizedDescription("Durum", typeof(Resources))]
        Durum,

        [LocalizedDescription("Fallow", typeof(Resources))]
        Fallow,

        [LocalizedDescription("EnumFieldPeas", typeof(Resources))]
        FieldPeas,

        [LocalizedDescription("Flax", typeof(Resources))]
        Flax,

        [LocalizedDescription("FlaxSeed", typeof(Resources))]
        FlaxSeed,

        [LocalizedDescription("FodderCorn", typeof(Resources))]
        FodderCorn,

        [LocalizedDescription("Forage", typeof(Resources))]
        Forage,

        [LocalizedDescription("EnumForageForSeed", typeof(Resources))]
        ForageForSeed,

        [LocalizedDescription("EnumFallRye", typeof(Resources))]
        FallRye,

        [LocalizedDescription("GrainSorghum", typeof(Resources))]
        GrainSorghum,

        [LocalizedDescription("GrainCorn", typeof(Resources))]
        GrainCorn,

        [LocalizedDescription("EnumRangelandNative", typeof(Resources))]
        RangelandNative,

        [LocalizedDescription("EnumHairyVetch", typeof(Resources))]
        HairyVetch,

        [LocalizedDescription("EnumHairyVetchAndRye", typeof(Resources))]
        HairyVetchAndRye,

        [LocalizedDescription("EnumTameGrass", typeof(Resources))]
        TameGrass,

        [LocalizedDescription("EnumTameLegume", typeof(Resources))]
        TameLegume,

        [LocalizedDescription("EnumTameMixed", typeof(Resources))]
        TameMixed,

        [LocalizedDescription("HayAndForageSeed", typeof(Resources))]
        HayAndForageSeed,

        [LocalizedDescription("Lentils", typeof(Resources))]
        Lentils,

        [LocalizedDescription("MixedGrains", typeof(Resources))]
        MixedGrains,

        [LocalizedDescription("Mustard", typeof(Resources))]
        Mustard,

        [LocalizedDescription("MustardSeed", typeof(Resources))]
        MustardSeed,

        [LocalizedDescription("Oats", typeof(Resources))]
        Oats,

        [LocalizedDescription("Oilseeds", typeof(Resources))]
        Oilseeds,

        [LocalizedDescription("EnumOtherFieldCrops", typeof(Resources))]
        OtherFieldCrops,

        [LocalizedDescription("Peas", typeof(Resources))]
        Peas,

        [LocalizedDescription("EnumSeededGrassland", typeof(Resources))]
        SeededGrassland,

        [LocalizedDescription("EnumPerennialForages", typeof(Resources))]
        PerennialForages,

        [LocalizedDescription("Potatoes", typeof(Resources))]
        Potatoes,

        [LocalizedDescription("PulseCrops", typeof(Resources))]
        PulseCrops,

        [LocalizedDescription("Rye", typeof(Resources))]
        Rye,

        [LocalizedDescription("Safflower", typeof(Resources))]
        Safflower,

        [LocalizedDescription("EnumSilageCorn", typeof(Resources))]
        SilageCorn,

        [LocalizedDescription("SmallGrainCereals", typeof(Resources))]
        SmallGrainCereals,

        [LocalizedDescription("Soybeans", typeof(Resources))]
        Soybeans,

        [LocalizedDescription("EnumSorghum", typeof(Resources))]
        Sorghum,

        [LocalizedDescription("SpringWheat", typeof(Resources))]
        SpringWheat,

        [LocalizedDescription("SpringRye", typeof(Resources))]
        SpringRye,

        [LocalizedDescription("EnumSugarBeets", typeof(Resources))]
        SugarBeets,

        [LocalizedDescription("SunflowerSeed", typeof(Resources))]
        SunflowerSeed,

        [LocalizedDescription("EnumSummerFallow", typeof(Resources))]
        SummerFallow,

        [LocalizedDescription("EnumTobacco", typeof(Resources))]
        Tobacco,

        [LocalizedDescription("EnumTreeFruitAndNuts", typeof(Resources))]
        TreeFruitAndNuts,

        [LocalizedDescription("Triticale", typeof(Resources))]
        Triticale,

        [LocalizedDescription("UndersownBarley", typeof(Resources))]
        UndersownBarley,

        [LocalizedDescription("EnumVegetables", typeof(Resources))]
        Vegetables,

        [LocalizedDescription("WheatBolinder", typeof(Resources))]
        WheatBolinder,

        [LocalizedDescription("WheatGan", typeof(Resources))]
        WheatGan,

        [LocalizedDescription("WinterWheat", typeof(Resources))]
        WinterWheat,

        [LocalizedDescription("EnumWinterWeeds", typeof(Resources))]
        WinterWeeds,

        [LocalizedDescription("EnumWheat", typeof(Resources))]
        Wheat,

        [LocalizedDescription("EnumBrokenGrassland", typeof(Resources))]
        BrokenGrassland,

        [LocalizedDescription("BromeHay", typeof(Resources))]
        BromeHay,

        [LocalizedDescription("Caraway", typeof(Resources))]
        Caraway,

        [LocalizedDescription("Carrot", typeof(Resources))]
        Carrot,

        [LocalizedDescription("CornSilage", typeof(Resources))]
        CornSilage,

        [LocalizedDescription("CPSWheat", typeof(Resources))]
        CPSWheat,

        [LocalizedDescription("Dill", typeof(Resources))]
        Dill,

        [LocalizedDescription("DryBean", typeof(Resources))]
        DryBean,

        [LocalizedDescription("FabaBeans", typeof(Resources))]
        FabaBeans,

        [LocalizedDescription("FreshCornSweet", typeof(Resources))]
        FreshCornSweet,

        [LocalizedDescription("FreshPeas", typeof(Resources))]
        FreshPeas,

        [LocalizedDescription("GrassSeed", typeof(Resources))]
        GrassSeed,

        [LocalizedDescription("GreenFeed", typeof(Resources))]
        GreenFeed,

        [LocalizedDescription("HardRedSpringWheat", typeof(Resources))]
        HardRedSpringWheat,

        [LocalizedDescription("Hyola", typeof(Resources))]
        Hyola,

        [LocalizedDescription("Linola", typeof(Resources))]
        Linola,

        [LocalizedDescription("Maltbarley", typeof(Resources))]
        MaltBarley,

        [LocalizedDescription("MarketGarden", typeof(Resources))]
        MarketGarden,

        [LocalizedDescription("MilkVetch", typeof(Resources))]
        MilkVetch,

        [LocalizedDescription("Mint", typeof(Resources))]
        Mint,

        [LocalizedDescription("Monarda", typeof(Resources))]
        Monarda,

        [LocalizedDescription("NativePasture", typeof(Resources))]
        NativePasture,

        [LocalizedDescription("OatSilage", typeof(Resources))]
        OatSilage,

        [LocalizedDescription("Onion", typeof(Resources))]
        Onion,

        [LocalizedDescription("SeedPotato", typeof(Resources))]
        SeedPotato,

        [LocalizedDescription("SmallFruit", typeof(Resources))]
        SmallFruit,

        [LocalizedDescription("SoftWheat", typeof(Resources))]
        SoftWheat,

        [LocalizedDescription("SorghumSudanGrass", typeof(Resources))]
        SorghumSudanGrass,

        [LocalizedDescription("TamePasture", typeof(Resources))]
        TamePasture,

        [LocalizedDescription("TimothyHay", typeof(Resources))]
        TimothyHay,

        [LocalizedDescription("TurfSod", typeof(Resources))]
        TurfSod,

        [LocalizedDescription("Millet", typeof(Resources))]
        Millet,

        [LocalizedDescription("AlfalfaSeed", typeof(Resources))]
        AlfalfaSeed,

        [LocalizedDescription("BarleySilageUnderSeed", typeof(Resources))]
        BarleySilageUnderSeed,

        [LocalizedDescription("Sunflower", typeof(Resources))]
        Sunflower,

        [LocalizedDescription("GrassHay", typeof(Resources))]
        GrassHay,

        [LocalizedDescription("EnumGrasslandSeeded", typeof(Resources))]
        GrasslandSeeded,

        [LocalizedDescription("EnumTriticaleSilage", typeof(Resources))]
        TriticaleSilage,

        [LocalizedDescription("EnumWheatSilage", typeof(Resources))]
        WheatSilage,

        [LocalizedDescription("EnumRedCloverTrifoliumPratenseL", typeof(Resources))]
        RedCloverTrifoliumPratenseL,

        [LocalizedDescription("EnumBerseemCloverTrifoliumAlexandriumL", typeof(Resources))]
        BerseemCloverTrifoliumAlexandriumL,

        [LocalizedDescription("EnumSweetCloverMelilotusOfficianalis", typeof(Resources))]
        SweetCloverMelilotusOfficinalis,

        [LocalizedDescription("EnumCrimsonCloverTrifoliumIncarnatum", typeof(Resources))]
        CrimsonCloverTrifoliumIncarnatum,

        [LocalizedDescription("EnumHairyVetchViciaVillosaRoth", typeof(Resources))]
        HairyVetchViciaVillosaRoth,

        [LocalizedDescription("EnumAlfalfaMedicagoSativaL", typeof(Resources))]
        AlfalfaMedicagoSativaL,

        [LocalizedDescription("EnumFabaBeanBroadBeanViciaFaba", typeof(Resources))]
        FabaBeanBroadBeanViciaFaba,

        [LocalizedDescription("EnumCowpeaVignaUnguiculata", typeof(Resources))]
        CowpeaVignaUnguiculata,

        [LocalizedDescription("EnumAustrianWinterPea", typeof(Resources))]
        AustrianWinterPea,

        [LocalizedDescription("EnumRapeseedBrassicaNapusL", typeof(Resources))]
        RapeseedBrassicaNapusL,

        [LocalizedDescription("EnumWinterTurnipRapeBrassicaRapaSppOleiferaLCVLargo", typeof(Resources))]
        WinterTurnipRapeBrassicaRapaSppOleiferaLCVLargo,

        [LocalizedDescription("EnumPhaceliaPhaceliaTanacetifoliaCVPhaci", typeof(Resources))]
        PhaceliaPhaceliaTanacetifoliaCVPhaci,

        [LocalizedDescription("EnumForageRadishRaphanusSativusL", typeof(Resources))]
        ForageRadishRaphanusSativusL,

        [LocalizedDescription("EnumMustardSinapusAlbaLSubspMaireiHLindbMaire", typeof(Resources))]
        MustardSinapusAlbaLSubspMaireiHLindbMaire,

        [LocalizedDescription("EnumBarleyHordeumVulgare", typeof(Resources))]
        BarleyHordeumVulgare,

        [LocalizedDescription("EnumOatAvenaSativa", typeof(Resources))]
        OatAvenaSativa,

        [LocalizedDescription("EnumRyeSecaleCerealeWinterRyeCerealRye", typeof(Resources))]
        RyeSecaleCerealeWinterRyeCerealRye,

        [LocalizedDescription("EnumSesameSesamumIndicum", typeof(Resources))]
        SesameSesamumIndicum,

        [LocalizedDescription("EnumFlaxLinumUsitatissimum", typeof(Resources))]
        FlaxLinumUsitatissimum,

        [LocalizedDescription("EnumRyegrassLoliumPerenneL", typeof(Resources))]
        RyeGrassLoliumPerenneL,

        [LocalizedDescription("EnumAnnualRyegrassLoliumMultiflorum", typeof(Resources))]
        AnnualRyeGrassLoliumMultiflorum,

        [LocalizedDescription("EnumSorghumSorghumBicolour", typeof(Resources))]
        SorghumSorghumBicolour,

        [LocalizedDescription("EnumPigeonBean", typeof(Resources))]
        PigeonBean,

        [LocalizedDescription("EnumShepherdsPurse", typeof(Resources))]
        ShepherdsPurse,

        [LocalizedDescription("EnumWinterWheatTriticumAestivum", typeof(Resources))]
        WinterWheatTriticumAestivum,

        [LocalizedDescription("EnumNone", typeof(Resources))]
        None,

        #region Economic Crops

        [LocalizedDescription("EnumFeedBarley", typeof(Resources))]
        FeedBarley,

        [LocalizedDescription("EnumRedLentils", typeof(Resources))]
        RedLentils,

        [LocalizedDescription("EnumMillingOats", typeof(Resources))]
        MillingOats,

        [LocalizedDescription("EnumPolishCanola", typeof(Resources))]
        PolishCanola,

        [LocalizedDescription("EnumArgentineHTCanola", typeof(Resources))]
        ArgentineHTCanola,

        [LocalizedDescription("EnumKabuliChickpea", typeof(Resources))]
        KabuliChickpea,

        [LocalizedDescription("EnumYellowMustard", typeof(Resources))]
        YellowMustard,

        [LocalizedDescription("EnumCerealSilage", typeof(Resources))]
        CerealSilage,

        [LocalizedDescription("EnumAlfalfaHay", typeof(Resources))]
        AlfalfaHay,

        [LocalizedDescription("EnumEdibleGreenPeas", typeof(Resources))]
        EdibleGreenPeas,

        [LocalizedDescription("EnumEdibleYellowPeas", typeof(Resources))]
        EdibleYellowPeas,

        [LocalizedDescription("EnumBlackBean", typeof(Resources))]
        BlackBean,

        [LocalizedDescription("EnumHybridFallRye", typeof(Resources))]
        HybridFallRye,

        [LocalizedDescription("EnumBrownMustard", typeof(Resources))]
        BrownMustard,

        [LocalizedDescription("EnumOrientalMustard", typeof(Resources))]
        OrientalMustard,

        [LocalizedDescription("EnumSunflowerOilseedEMSS", typeof(Resources))]
        SunflowerOilseedEMSS,

        [LocalizedDescription("EnumDesiChickpeas", typeof(Resources))]
        DesiChickpeas,

        [LocalizedDescription("EnumCamelina", typeof(Resources))]
        Camelina,

        [LocalizedDescription("EnumCarawayFirstSeason", typeof(Resources))]
        CarawayFirstSeason,

        [LocalizedDescription("EnumCarawaySecondSeason", typeof(Resources))]
        CarawaySecondSeason,

        [LocalizedDescription("EnumCoriander", typeof(Resources))]
        Coriander,

        [LocalizedDescription("EnumFenugreek", typeof(Resources))]
        Fenugreek,

        [LocalizedDescription("EnumQuinoa", typeof(Resources))]
        Quinoa,

        [LocalizedDescription("EnumWheatHardRedSpring", typeof(Resources))]
        WheatHardRedSpring,

        [LocalizedDescription("EnumWheatPrairieSpring", typeof(Resources))]
        WheatPrairieSpring,

        [LocalizedDescription("EnumWheatOtherSpring", typeof(Resources))]
        WheatOtherSpring,

        [LocalizedDescription("EnumBeansPinto", typeof(Resources))]
        BeansPinto,

        [LocalizedDescription("EnumSunflowerConfection", typeof(Resources))]
        SunflowerConfection,

        [LocalizedDescription("EnumLargeGreenLentils", typeof(Resources))]
        LargeGreenLentils,

        [LocalizedDescription("EnumWheatNorthernHardRed", typeof(Resources))]
        WheatNorthernHardRed,

        [LocalizedDescription("EnumSunflowerOil", typeof(Resources))]
        SunflowerOil,

        [LocalizedDescription("EnumBeansWhite", typeof(Resources))]
        BeansWhite,

        [LocalizedDescription("EnumLargeKabuliChickpea", typeof(Resources))]
        LargeKabuliChickpea,

        [LocalizedDescription("EnumSmallKabuliChickpea", typeof(Resources))]
        SmallKabuliChickpea,

        [LocalizedDescription("EnumUserDefined", typeof(Resources))]
        UserDefined,

        [LocalizedDescription("EnumColouredBeans", typeof(Resources))]
        ColouredBeans,

        [LocalizedDescription("EnumHardRedWinterWheat", typeof(Resources))]
        HardRedWinterWheat,

        [LocalizedDescription("EnumHardRedWinterWheatNoTill", typeof(Resources))]
        HardRedWinterWheatNoTill,

        [LocalizedDescription("EnumNorthernOntarioBarley", typeof(Resources))]
        NorthernOntarioBarley,

        [LocalizedDescription("EnumSoutherOntarioBarley", typeof(Resources))]
        SouthernOntarioBarley,

        [LocalizedDescription("EnumNorthernOntarioOats", typeof(Resources))]
        NorthernOntarioOats,

        [LocalizedDescription("EnumSoutherOntarioOats", typeof(Resources))]
        SouthernOntarioOats,

        [LocalizedDescription("EnumSpringCanolaHt", typeof(Resources))]
        SpringCanolaHt,

        [LocalizedDescription("EnumSoybeanNoTill", typeof(Resources))]
        SoybeanNoTill,

        [LocalizedDescription("EnumSoybeanRoundupReady", typeof(Resources))]
        SoybeansRoundUpReady,

        [LocalizedDescription("EnumSwitchgrassDirect", typeof(Resources))]
        SwitchgrassDirect,

        [LocalizedDescription("EnumSwitchgrassDirectNoTill", typeof(Resources))]
        SwitchgrassDirectNoTill,

        [LocalizedDescription("EnumSwitchgrassUnderseeded", typeof(Resources))]
        SwitchgrassUnderseeded,

        [LocalizedDescription("EnumSwitchgrassUnderseededNoTill", typeof(Resources))]
        SwitchgrassUnderseededNoTill,

        [LocalizedDescription("EnumSoftWinterWheat", typeof(Resources))]
        SoftWinterWheat,

        [LocalizedDescription("EnumSoftWinterWheatNoTill", typeof(Resources))]
        SoftWinterWheatNoTill,

        [LocalizedDescription("EnumWhiteBlackBeans", typeof(Resources))]
        WhiteBlackBeans,

        [LocalizedDescription("EnumWinterCanolaHybrid", typeof(Resources))]
        WinterCanolaHybrid,
        #endregion


        /// <summary>
        /// The new crops below are from the "Default values for nitrogen and lignin contents in crops for steady state methods.
        /// </summary>
        #region Nitrogen and lignin contents table
        [LocalizedDescription("EnumGrains", typeof(Resources))]
        Grains,

        [LocalizedDescription("EnumDryFieldPeas", typeof(Resources))]
        DryFieldPeas,

        [LocalizedDescription("EnumOtherDryFieldBeans", typeof(Resources))]
        OtherDryFieldBeans,

        [LocalizedDescription("EnumPulses", typeof(Resources))]
        Pulses,

        [LocalizedDescription("EnumWhiteBeans", typeof(Resources))]
        WhiteBeans,

        [LocalizedDescription("EnumWheatRye", typeof(Resources))]
        WheatRye,

        [LocalizedDescription("EnumGenericGrains", typeof(Resources))]
        GenericGrains,

        [LocalizedDescription("EnumRice", typeof(Resources))]
        Rice,

        [LocalizedDescription("EnumBeans", typeof(Resources))]
        Beans,

        [LocalizedDescription("Tubers", typeof(Resources))]
        Tubers,

        [LocalizedDescription("Peanuts", typeof(Resources))]
        Peanuts,

        [LocalizedDescription("EnumNFixingForages", typeof(Resources))]
        NFixingForages,

        [LocalizedDescription("EnumNonNFixingForages", typeof(Resources))]
        NonNFixingForages,

        [LocalizedDescription("EnumPerennialGrasses", typeof(Resources))]
        PerennialGrasses,

        [LocalizedDescription("EnumGrassCloverMixtures", typeof(Resources))]
        GrassCloverMixtures,

        [LocalizedDescription("EnumNonLegumeHay", typeof(Resources))]
        NonLegumeHay,

        [LocalizedDescription("EnumCereals", typeof(Resources))]
        Cereals,

        #endregion
    }
}