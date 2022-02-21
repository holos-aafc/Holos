using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations {
    public enum IngredientType {
        [LocalizedDescription("AlfalfaCubes", typeof(FeedNameResources))]
        AlfalfaCubes,

        [LocalizedDescription("AlfalfaDehy", typeof(FeedNameResources))]
        AlfalfaDehy,

        [LocalizedDescription("AlfalfaFresh", typeof(FeedNameResources))]
        AlfalfaFresh,

        [LocalizedDescription("AlfalfaGreenchop", typeof(FeedNameResources))]
        AlfalfaGreenchop,

        [LocalizedDescription("AlfalfaHay", typeof(FeedNameResources))]
        AlfalfaHay,

        [LocalizedDescription("AlfalfaMeal", typeof(FeedNameResources))]
        AlfalfaMeal,

        [LocalizedDescription("AlfalfaHaylage", typeof(FeedNameResources))]
        AlfalfaHaylage,

        [LocalizedDescription("AlfalfaMedicago", typeof(FeedNameResources))]
        AlfalfaMedicago,

        [LocalizedDescription("AlmondHulls", typeof(FeedNameResources))]
        AlmondHulls,

        [LocalizedDescription("ApplePomace", typeof(FeedNameResources))]
        ApplePomace,

        [LocalizedDescription("ApplePomaceWet", typeof(FeedNameResources))]
        ApplePomaceWet,

        [LocalizedDescription("BakeryMeal", typeof(FeedNameResources))]
        BakeryMeal,

        [LocalizedDescription("BakeryProducts", typeof(FeedNameResources))]
        BakeryProducts,

        [LocalizedDescription("BakeryByProductMeal", typeof(FeedNameResources))]
        BakeryProductMeal,

        [LocalizedDescription("BakeryBreadMeal", typeof(FeedNameResources))]
        BakeryBreadMeal,

        [LocalizedDescription("BakeryCerealByProduct", typeof(FeedNameResources))]
        BakeryCerealByProduct,

        [LocalizedDescription("BakeryCookieByProduct", typeof(FeedNameResources))]
        BakeryCookieByProduct,

        [LocalizedDescription("Barley", typeof(FeedNameResources))]
        Barley,

        [LocalizedDescription("BarleyDistillersDriedGrainsWithSolubles", typeof(FeedNameResources))]
        BarleyDistillerDriedGrainsWithSolubles,

        [LocalizedDescription("BarleyGrain", typeof(FeedNameResources))]
        BarleyGrain,

        [LocalizedDescription("BarleyGrainFlaked", typeof(FeedNameResources))]
        BarleyGrainFlaked,

        [LocalizedDescription("BarleyGrainRolled", typeof(FeedNameResources))]
        BarleyGrainRolled,

        [LocalizedDescription("BarleyMaltSprouts", typeof(FeedNameResources))]
        BarleyMaltSprouts,

        [LocalizedDescription("BarleyHay", typeof(FeedNameResources))]
        BarleyHay,

        [LocalizedDescription("BarleyHulless", typeof(FeedNameResources))]
        BarleyHulless,

        [LocalizedDescription("BarleySilage", typeof(FeedNameResources))]
        BarleySilage,


        [LocalizedDescription("BarleySilageHeaded", typeof(FeedNameResources))]
        BarleySilageHeaded,

        [LocalizedDescription("BarleyStraw", typeof(FeedNameResources))]
        BarleyStraw,

        [LocalizedDescription("BeansPhaseolusSPP", typeof(FeedNameResources))]
        BeansPhaseolusSPP,

        [LocalizedDescription("BeetPulpDry", typeof(FeedNameResources))]
        BeetPulpDry,

        [LocalizedDescription("BeetSugarPulpDried", typeof(FeedNameResources))]
        BeetSugarPulpDried,

        [LocalizedDescription("BeetPulpWet", typeof(FeedNameResources))]
        BeetPulpWet,

        [LocalizedDescription("BermudaGrassCoastalHayEarlyHead", typeof(FeedNameResources))]
        BermudaGrassCoastalHayEarlyHead,

        [LocalizedDescription("BermudaGrassTiftonHayThreeTo4WeekGrowth", typeof(FeedNameResources))]
        BermudaGrassTiftonHayThreeTo4WeekGrowth,

        [LocalizedDescription("BermudaGrassFresh", typeof(FeedNameResources))]
        BermudaGrassFresh,

        [LocalizedDescription("BermudaGrassHay", typeof(FeedNameResources))]
        BermudaGrassHay,

        [LocalizedDescription("BermudaGrassSilage", typeof(FeedNameResources))]
        BermudaGrassSilage,

        [LocalizedDescription("BloodCells", typeof(FeedNameResources))]
        BloodCells,

        [LocalizedDescription("BloodCellsSprayDried", typeof(FeedNameResources))]
        BloodCellsSprayDried,

        [LocalizedDescription("BloodMeal", typeof(FeedNameResources))]
        BloodMeal,

        [LocalizedDescription("BloodMealRingDried", typeof(FeedNameResources))]
        BloodMealRingDried,

        [LocalizedDescription("BloodMealBatchDried", typeof(FeedNameResources))]
        BloodMealBatchDried,

        [LocalizedDescription("BloodPlasma", typeof(FeedNameResources))]
        BloodPlasma,

        [LocalizedDescription("BloodPlasmaSprayDried", typeof(FeedNameResources))]
        BloodPlasmaSprayDried,

        [LocalizedDescription("BlueGrassHay", typeof(FeedNameResources))]
        BlueGrassHay,

        [LocalizedDescription("BlueStemFresh", typeof(FeedNameResources))]
        BlueStemFresh,

        [LocalizedDescription("BlueStemHay", typeof(FeedNameResources))]
        BlueStemHay,

        [LocalizedDescription("BrewersGrains", typeof(FeedNameResources))]
        BrewersGrains,

        [LocalizedDescription("BrewersDriedGrains", typeof(FeedNameResources))]
        BrewersDriedGrains,

        [LocalizedDescription("BrewersWetGrains", typeof(FeedNameResources))]
        BrewersWetGrains,

        [LocalizedDescription("BrewersYeast", typeof(FeedNameResources))]
        BrewersYeast,

        [LocalizedDescription("BrewersYeastWet", typeof(FeedNameResources))]
        BrewersYeastWet,

        [LocalizedDescription("BromeGrassHay", typeof(FeedNameResources))]
        BromeGrassHay,

        [LocalizedDescription("BromeGrassSilage", typeof(FeedNameResources))]
        BromeGrassSilage,

        [LocalizedDescription("BuffaloGrass", typeof(FeedNameResources))]
        BuffaloGrass,

        [LocalizedDescription("CamelinaMeal", typeof(FeedNameResources))]
        CamelinaMeal,

        [LocalizedDescription("CamelinaMealMech", typeof(FeedNameResources))]
        CamelinaMealMech,

        [LocalizedDescription("CaneFresh", typeof(FeedNameResources))]
        CaneFresh,

        [LocalizedDescription("CaneHay", typeof(FeedNameResources))]
        CaneHay,

        [LocalizedDescription("CaneSilage", typeof(FeedNameResources))]
        CaneSilage,

        [LocalizedDescription("CanolaFullFat", typeof(FeedNameResources))]
        CanolaFullFat,

        [LocalizedDescription("CanolaGrain", typeof(FeedNameResources))]
        CanolaGrain,

        [LocalizedDescription("CanolaMeal", typeof(FeedNameResources))]
        CanolaMeal,

        [LocalizedDescription("CanolaMealExpelled", typeof(FeedNameResources))]
        CanolaMealExpelled,

        [LocalizedDescription("CanolaMealMechExtracted", typeof(FeedNameResources))]
        CanolaMealMechExtracted,

        [LocalizedDescription("CanolaMealSolvent", typeof(FeedNameResources))]
        CanolaMealSolvent,

        [LocalizedDescription("CanolaSeed", typeof(FeedNameResources))]
        CanolaSeed,

        [LocalizedDescription("Casein", typeof(FeedNameResources))]
        Casein,

        [LocalizedDescription("CassavaMeal", typeof(FeedNameResources))]
        CassavaMeal,

        [LocalizedDescription("ChocolateByProduct", typeof(FeedNameResources))]
        ChocolateByProduct,

        [LocalizedDescription("CitrusPulp", typeof(FeedNameResources))]
        CitrusPulp,

        [LocalizedDescription("CitrusPulpDry", typeof(FeedNameResources))]
        CitrusPulpDry,

        [LocalizedDescription("CitrusPulpWet", typeof(FeedNameResources))]
        CitrusPulpWet,

        [LocalizedDescription("Chickpeas", typeof(FeedNameResources))]
        ChickPeas,

        [LocalizedDescription("CopraExpelled", typeof(FeedNameResources))]
        CopraExpelled,

        [LocalizedDescription("CopraMeal", typeof(FeedNameResources))]
        CopraMeal,

        [LocalizedDescription("CopraMealMech", typeof(FeedNameResources))]
        CopraMealMech,

        [LocalizedDescription("CopraMealSolvent", typeof(FeedNameResources))]
        CopraMealSolvent,

        [LocalizedDescription("CornBran", typeof(FeedNameResources))]
        CornBran,

        [LocalizedDescription("CornCobs", typeof(FeedNameResources))]
        CornCobs,

        [LocalizedDescription("CornDistillersDriedGrains", typeof(FeedNameResources))]
        CornDistillersDriedGrains,

        [LocalizedDescription("CornDistillesrDriedGrainsSolubles", typeof(FeedNameResources))]
        CornDistillersDriedGrainsSolubles,

        [LocalizedDescription("CornDistillersDriedGrainsSolublesGreaterThanTenPercentOil", typeof(FeedNameResources))]
        CornDistillersDriedGrainsSolublesGreaterThanTenPercentOil,

        [LocalizedDescription("CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil", typeof(FeedNameResources))]
        CornDistillersDriedGrainsSolublesGreaterThanSixAndLessThanNinePercentOil,

        [LocalizedDescription("CornDistillersDriedGrainsSolublesLessThanFourPercentOil", typeof(FeedNameResources))]
        CornDistillersDriedGrainsSolublesLessThanFourPercentOil,

        [LocalizedDescription("CornHpDistillersDriedGrains", typeof(FeedNameResources))]
        CornHpDistillersDriedGrains,

        [LocalizedDescription("CornDistillersSolubles", typeof(FeedNameResources))]
        CornDistillersSolubles,

        [LocalizedDescription("CornEarCorn", typeof(FeedNameResources))]
        CornEarCorn,

        [LocalizedDescription("CornEarlage", typeof(FeedNameResources))]
        CornEarlage,

        [LocalizedDescription("CornGerm", typeof(FeedNameResources))]
        CornGerm,

        [LocalizedDescription("CornGermMeal", typeof(FeedNameResources))]
        CornGermMeal,

        [LocalizedDescription("CornGlutenFeed", typeof(FeedNameResources))]
        CornGlutenFeed,

        [LocalizedDescription("CornGlutenFeedAndDistillersGoldenSynergy", typeof(FeedNameResources))]
        CornGlutenFeedAndDistillersGoldenSynergy,

        [LocalizedDescription("CornGlutenFeedWet", typeof(FeedNameResources))]
        CornGlutenFeedWet,

        [LocalizedDescription("CornGlutenFeedWetSweetBran", typeof(FeedNameResources))]
        CornGlutenFeedWetSweetBran,

        [LocalizedDescription("CornGlutenFeedDry", typeof(FeedNameResources))]
        CornGlutenFeedDry,

        [LocalizedDescription("CornGlutenMeal", typeof(FeedNameResources))]
        CornGlutenMeal,

        [LocalizedDescription("CornGrain", typeof(FeedNameResources))]
        CornGrain,

        [LocalizedDescription("CornGrainDry", typeof(FeedNameResources))]
        CornGrainDry,

        [LocalizedDescription("CornGrainHighMoisture", typeof(FeedNameResources))]
        CornGrainHighMoisture,

        [LocalizedDescription("CornGrainSteamFlaked", typeof(FeedNameResources))]
        CornGrainSteamFlaked,

        [LocalizedDescription("CornGreenchop", typeof(FeedNameResources))]
        CornGreenchop,

        [LocalizedDescription("CornGritsHominyFeed", typeof(FeedNameResources))]
        CornGritsHominyFeed,

        [LocalizedDescription("CornHominyFeed", typeof(FeedNameResources))]
        CornHominyFeed,

        [LocalizedDescription("CornNutridense", typeof(FeedNameResources))]
        CornNutridense,

        [LocalizedDescription("CornScreenings", typeof(FeedNameResources))]
        CornScreenings,

        [LocalizedDescription("CornSilage", typeof(FeedNameResources))]
        CornSilage,

        [LocalizedDescription("CornSnaplage", typeof(FeedNameResources))]
        CornSnaplage,

        [LocalizedDescription("CornStalklage", typeof(FeedNameResources))]
        CornStalklage,

        [LocalizedDescription("CornStalks", typeof(FeedNameResources))]
        CornStalks,

        [LocalizedDescription("CornSteep", typeof(FeedNameResources))]
        CornSteep,

        [LocalizedDescription("CornYellowCobs", typeof(FeedNameResources))]
        CornYellowCobs,

        [LocalizedDescription("CornYellowDent", typeof(FeedNameResources))]
        CornYellowDent,

        [LocalizedDescription("CornYellowDistillersGrainSolublesDried", typeof(FeedNameResources))]
        CornYellowDistillersGrainSolublesDried,

        [LocalizedDescription("CornYellowGlutenFeedDried", typeof(FeedNameResources))]
        CornYellowGlutenFeedDried,

        [LocalizedDescription("CornYellowGlutenMealDried", typeof(FeedNameResources))]
        CornYellowGlutenMealDried,

        [LocalizedDescription("CornYellowGrainCrackedDry", typeof(FeedNameResources))]
        CornYellowGrainCrackedDry,

        [LocalizedDescription("CornYellowGrainGraundDry", typeof(FeedNameResources))]
        CornYellowGrainGraundDry,

        [LocalizedDescription("CornYellowGrainSteamFlacked", typeof(FeedNameResources))]
        CornYellowGrainSteamFlacked,

        [LocalizedDescription("CornYellowGrainRolledHighMoisture", typeof(FeedNameResources))]
        CornYellowGrainRolledHighMoisture,

        [LocalizedDescription("CornYellowGrainGroundHighMoisture", typeof(FeedNameResources))]
        CornYellowGrainGroundHighMoisture,

        [LocalizedDescription("CornYellowGrainAndCobDryGround", typeof(FeedNameResources))]
        CornYellowGrainAndCobDryGround,

        [LocalizedDescription("CornYellowGrainAndCobHighMoistureGround", typeof(FeedNameResources))]
        CornYellowGrainAndCobHighMoistureGround,

        [LocalizedDescription("CornYellowHominy", typeof(FeedNameResources))]
        CornYellowHominy,

        [LocalizedDescription("CornYellowSilageImmature", typeof(FeedNameResources))]
        CornYellowSilageImmature,

        [LocalizedDescription("CornYellowSilageNormal", typeof(FeedNameResources))]
        CornYellowSilageNormal,

        [LocalizedDescription("CornYellowSilageMature", typeof(FeedNameResources))]
        CornYellowSilageMature,

        [LocalizedDescription("CottonBurrs", typeof(FeedNameResources))]
        CottonBurrs,

        [LocalizedDescription("CottonGinTrash", typeof(FeedNameResources))]
        CottonGinTrash,

        [LocalizedDescription("CottonSeed", typeof(FeedNameResources))]
        CottonSeed,

        [LocalizedDescription("CottonseedFullFat", typeof(FeedNameResources))]
        CottonseedFullFat,

        [LocalizedDescription("CottonseedHulls", typeof(FeedNameResources))]
        CottonseedHulls,

        [LocalizedDescription("CottonseedMeal", typeof(FeedNameResources))]
        CottonseedMeal,

        [LocalizedDescription("CottonSeedMealMech", typeof(FeedNameResources))]
        CottonSeedMealMech,

        [LocalizedDescription("CottonseedWhole", typeof(FeedNameResources))]
        CottonseedWhole,

        [LocalizedDescription("CottonSeedWholeWithLint", typeof(FeedNameResources))]
        CottonSeedWholeWithLint,

        [LocalizedDescription("CottonSeedMealSolvent", typeof(FeedNameResources))]
        CottonSeedMealSolvent,

        [LocalizedDescription("CowPeas", typeof(FeedNameResources))]
        CowPeas,

        [LocalizedDescription("DistillersGrainPlusSolubleDry", typeof(FeedNameResources))]
        DistillersGrainPlusSolubleDry,

        [LocalizedDescription("DistillersGrainPlusSolubleModified", typeof(FeedNameResources))]
        DistillersGrainPlusSolubleModified,

        [LocalizedDescription("DistillersGrainPlusSolubleWet", typeof(FeedNameResources))]
        DistillersGrainPlusSolubleWet,

        [LocalizedDescription("DistillersSolubles", typeof(FeedNameResources))]
        DistillersSolubles,

        [LocalizedDescription("DriedYeast", typeof(FeedNameResources))]
        DriedYeast,

        [LocalizedDescription("EggWholeSprayDried", typeof(FeedNameResources))]
        EggWholeSprayDried,

        [LocalizedDescription("FatAndOilsCalciumSoaps", typeof(FeedNameResources))]
        FatAndOilsCalciumSoaps,

        [LocalizedDescription("FabaBeans", typeof(FeedNameResources))]
        FabaBeans,

        [LocalizedDescription("FatAndOilsHydrolyzedTallowFattyAcids", typeof(FeedNameResources))]
        FatAndOilsHydrolyzedTallowFattyAcids,

        [LocalizedDescription("FatAndOilsPartiallyHydrogenatedTallow", typeof(FeedNameResources))]
        FatAndOilsPartiallyHydrogenatedTallow,

        [LocalizedDescription("FatAndOilsTallow", typeof(FeedNameResources))]
        FatAndOilsTallow,

        [LocalizedDescription("FatAndOilsVegitableOils", typeof(FeedNameResources))]
        FatAndOilsVegitableOils,

        [LocalizedDescription("FeatherMeal", typeof(FeedNameResources))]
        FeatherMeal,

        [LocalizedDescription("FeathersHydrolizedMeal", typeof(FeedNameResources))]
        FeathersHydrolizedMeal,

        [LocalizedDescription("FeathersHydrolizedMealSomeViscera", typeof(FeedNameResources))]
        FeathersHydrolizedMealSomeViscera,

        [LocalizedDescription("FescueHay", typeof(FeedNameResources))]
        FescueHay,

        [LocalizedDescription("FieldPeas", typeof(FeedNameResources))]
        FieldPeas,

        [LocalizedDescription("FieldPeaSplits", typeof(FeedNameResources))]
        FieldPeaSplits,

        [LocalizedDescription("FieldPeasHighCP", typeof(FeedNameResources))]
        FieldPeasHighCP,

        [LocalizedDescription("FieldPeasLowCP", typeof(FeedNameResources))]
        FieldPeasLowCP,

        [LocalizedDescription("FishMeal", typeof(FeedNameResources))]
        FishMeal,

        [LocalizedDescription("FishMealCombined", typeof(FeedNameResources))]
        FishMealCombined,

        [LocalizedDescription("FishMealMech", typeof(FeedNameResources))]
        FishMealMech,

        [LocalizedDescription("FishProteinHydrolized", typeof(FeedNameResources))]
        FishProteinHydrolized,

        [LocalizedDescription("FishByProductsAnchovyMealMech", typeof(FeedNameResources))]
        FishByProductsAnchovyMealMech,

        [LocalizedDescription("FishByProductsMenhadenMealMech", typeof(FeedNameResources))]
        FishByProductsMenhadenMealMech,

        [LocalizedDescription("Flaxseed", typeof(FeedNameResources))]
        Flaxseed,

        [LocalizedDescription("FlaxseedMeal", typeof(FeedNameResources))]
        FlaxseedMeal,

        [LocalizedDescription("FlaxseedMealMech", typeof(FeedNameResources))]
        FlaxseedMealMech,

        [LocalizedDescription("ForageSorghamSilage", typeof(FeedNameResources))]
        ForageSorghamSilage,

        [LocalizedDescription("ForageSorghamSudan", typeof(FeedNameResources))]
        ForageSorghamSudan,

        [LocalizedDescription("ForageSorghamSudanHay", typeof(FeedNameResources))]
        ForageSorghamSudanHay,

        [LocalizedDescription("ForageSorghamSudanSilage", typeof(FeedNameResources))]
        ForageSorghamSudanSilage,

        [LocalizedDescription("ForageSorghamFresh", typeof(FeedNameResources))]
        ForageSorghamFresh,

        [LocalizedDescription("ForageSorghamHay", typeof(FeedNameResources))]
        ForageSorghamHay,

        [LocalizedDescription("FoxtailMillet", typeof(FeedNameResources))]
        FoxtailMillet,

        [LocalizedDescription("FreshSoybeanForage", typeof(FeedNameResources))]
        FreshSoybeanForage,

        [LocalizedDescription("Gelatin", typeof(FeedNameResources))]
        Gelatin,

        [LocalizedDescription("Glycerin", typeof(FeedNameResources))]
        ForageGlycerin,

        [LocalizedDescription("GrainSorghumGrain", typeof(FeedNameResources))]
        ForageGrainSorghumGrain,

        [LocalizedDescription("GrainSorghumHay", typeof(FeedNameResources))]
        ForageGrainSorghumHay,

        [LocalizedDescription("GrainSorghumSilage", typeof(FeedNameResources))]
        ForageGrainSorghumSilage,

        [LocalizedDescription("GrainSorghumStalks", typeof(FeedNameResources))]
        ForageGrainSorghumStalks,

        [LocalizedDescription("GrainSorghumFlaked", typeof(FeedNameResources))]
        ForageGrainSorghumFlaked,

        [LocalizedDescription("GrainSorghumHighMoisture", typeof(FeedNameResources))]
        ForageGrainSorghumHighMoisture,

        [LocalizedDescription("GrapePomaceDry", typeof(FeedNameResources))]
        ForageGrapePomaceDry,

        [LocalizedDescription("GrapePomaceWet", typeof(FeedNameResources))]
        ForageGrapePomaceWet,

        [LocalizedDescription("GrassesCoolPastureIntense", typeof(FeedNameResources))]
        GrassesCoolPastureIntense,

        [LocalizedDescription("GrassesCoolHayAllSamples", typeof(FeedNameResources))]
        GrassesCoolHayAllSamples,

        [LocalizedDescription("GrassesCoolHayImmature", typeof(FeedNameResources))]
        GrassesCoolHayImmature,

        [LocalizedDescription("GrassesCoolHayMidMaturity", typeof(FeedNameResources))]
        GrassesCoolHayMidMaturity,

        [LocalizedDescription("GrassesCoolHayMature", typeof(FeedNameResources))]
        GrassesCoolHayMature,

        [LocalizedDescription("GrassesCoolSilageAllSamples", typeof(FeedNameResources))]
        GrassesCoolSilageAllSamples,

        [LocalizedDescription("GrassesCoolSilageImmature", typeof(FeedNameResources))]
        GrassesCoolSilageImmature,

        [LocalizedDescription("GrassesCoolSilageMature", typeof(FeedNameResources))]
        GrassesCoolSilageMature,

        [LocalizedDescription("GrassesCoolSilageMidMaturity", typeof(FeedNameResources))]
        GrassesCoolSilageMidMaturity,

        [LocalizedDescription("GrassLegumeMixturesPredomGrassHayImmature", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomGrassHayImmature,

        [LocalizedDescription("GrassLegumeMixturesPredomGrassHayMidMaturity", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomGrassHayMidMaturity,

        [LocalizedDescription("GrassLegumeMixturesPredomGrassHayMature", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomGrassHayMature,

        [LocalizedDescription("GrassLegumeMixturesPredomGrassSilageImmature", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomGrassSilageImmature,

        [LocalizedDescription("GrassLegumeMixturesPredomGrassSilageMidMaturity", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomGrassSilageMidMaturity,

        [LocalizedDescription("GrassLegumeMixturesPredomGrassSilageMature", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomGrassSilageMature,

        [LocalizedDescription("GrassLegumeMixturesMixedGrassAndLegumeHayImmature", typeof(FeedNameResources))]
        GrassLegumeMixturesMixedGrassAndLegumeHayImmature,

        [LocalizedDescription("GrassLegumeMixturesMixedGrassAndLegumeHayMidMaturity", typeof(FeedNameResources))]
        GrassLegumeMixturesMixedGrassAndLegumeHayMidMaturity,

        [LocalizedDescription("GrassLegumeMixturesMixedGrassAndLegumeHayMature", typeof(FeedNameResources))]
        GrassLegumeMixturesMixedGrassAndLegumeHayMature,

        [LocalizedDescription("GrassLegumeMixturesMixedGrassAndLegumeSilageImmature", typeof(FeedNameResources))]
        GrassLegumeMixturesMixedGrassAndLegumeSilageImmature,

        [LocalizedDescription("GrassLegumeMixturesMixedGrassAndLegumeSilageMidMaturity", typeof(FeedNameResources))]
        GrassLegumeMixturesMixedGrassAndLegumeSilageMidMaturity,

        [LocalizedDescription("GrassLegumeMixturesMixedGrassAndLegumeSilageMature", typeof(FeedNameResources))]
        GrassLegumeMixturesMixedGrassAndLegumeSilageMature,

        [LocalizedDescription("GrassLegumeMixturesPredomLegumesHayImmature", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomLegumesHayImmature,

        [LocalizedDescription("GrassLegumeMixturesPredomLegumesHayMidMaturity", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomLegumesHayMidMaturity,

        [LocalizedDescription("GrassLegumeMixturesPredomLegumesHayMature", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomLegumesHayMature,

        [LocalizedDescription("GrassLegumeMixturesPredomLegumesSilageImmature", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomLegumesSilageImmature,

        [LocalizedDescription("GrassLegumeMixturesPredomLegumesSilageMidMaturity", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomLegumesSilageMidMaturity,

        [LocalizedDescription("GrassLegumeMixturesPredomLegumesSilageMature", typeof(FeedNameResources))]
        GrassLegumeMixturesPredomLegumesSilageMature,


        [LocalizedDescription("Hominy", typeof(FeedNameResources))]
        Hominy,

        [LocalizedDescription("JohnsonGrass", typeof(FeedNameResources))]
        JoghnsonGrass,

        [LocalizedDescription("KidneyBeansExtruded", typeof(FeedNameResources))]
        KidneyBeansExtruded,

        [LocalizedDescription("KidneyBeansRaw", typeof(FeedNameResources))]
        KidneyBeansRaw,

        [LocalizedDescription("Kochia", typeof(FeedNameResources))]
        Kochia,

        [LocalizedDescription("KochiaSilage", typeof(FeedNameResources))]
        KochiaSilage,

        [LocalizedDescription("Lactose", typeof(FeedNameResources))]
        Lactose,

        [LocalizedDescription("LegumesForagePastureIntense", typeof(FeedNameResources))]
        LegumesForagePastureIntense,

        [LocalizedDescription("LegumesForageHayAllSamples", typeof(FeedNameResources))]
        LegumesForageHayAllSamples,

        [LocalizedDescription("LegumesForageHayImmature", typeof(FeedNameResources))]
        LegumesForageHayImmature,

        [LocalizedDescription("LegumesForageHayMidMaturity", typeof(FeedNameResources))]
        LegumesForageHayMidMaturity,

        [LocalizedDescription("LegumesForageHayMature", typeof(FeedNameResources))]
        LegumesForageHayMature,

        [LocalizedDescription("LegumesForageSilageAllSamples", typeof(FeedNameResources))]
        LegumesForageSilageAllSamples,

        [LocalizedDescription("LegumesForageSilageImmature", typeof(FeedNameResources))]
        LegumesForageSilageImmature,

        [LocalizedDescription("LegumesForageSilageMidMaturity", typeof(FeedNameResources))]
        LegumesForageSilageMidMaturity,

        [LocalizedDescription("LegumesForageSilageMature", typeof(FeedNameResources))]
        LegumesForageSilageMature,

        [LocalizedDescription("Lentils", typeof(FeedNameResources))]
        Lentils,

        [LocalizedDescription("LinseedMeal", typeof(FeedNameResources))]
        LinseedMeal,

        [LocalizedDescription("LinSeedFlaxMealSolvent", typeof(FeedNameResources))]
        LinSeedFlaxMealSolvent,

        [LocalizedDescription("Lupins", typeof(FeedNameResources))]
        Lupins,

        [LocalizedDescription("LupinSeed", typeof(FeedNameResources))]
        LupinSeed,

        [LocalizedDescription("MeadowHay", typeof(FeedNameResources))]
        MeadowHay,

        [LocalizedDescription("MeatAndBoneMealPhosphorusGreaterThanFourPercent", typeof(FeedNameResources))]
        MeatAndBoneMealPhosphorusGreaterThanFourPercent,

        [LocalizedDescription("MeatMeal", typeof(FeedNameResources))]
        MeatMeal,

        [LocalizedDescription("MeatBoneMeal", typeof(FeedNameResources))]
        MeatBoneMeal,

        [LocalizedDescription("MeatMealRendered", typeof(FeedNameResources))]
        MeatMealRendered,

        [LocalizedDescription("MeatMeatAndBoneRendered", typeof(FeedNameResources))]
        MeatMeatAndBoneRendered,

        [LocalizedDescription("Millet", typeof(FeedNameResources))]
        Millet,

        [LocalizedDescription("MilletForageFresh", typeof(FeedNameResources))]
        MilletForageFresh,

        [LocalizedDescription("MilletForageHay", typeof(FeedNameResources))]
        MilletForageHay,

        [LocalizedDescription("MilletGrain", typeof(FeedNameResources))]
        MilletGrain,

        [LocalizedDescription("MilletSilage", typeof(FeedNameResources))]
        MilletSilage,

        [LocalizedDescription("MolassesBeet", typeof(FeedNameResources))]
        MolassesBeet,

        [LocalizedDescription("MolassesCane", typeof(FeedNameResources))]
        MolassesCane,

        [LocalizedDescription("MolassesBeetSugar", typeof(FeedNameResources))]
        MolassesBeetSugar,

        [LocalizedDescription("MolassesSugarBeets", typeof(FeedNameResources))]
        MolassesSugarBeets,

        [LocalizedDescription("MolassesSugarCane", typeof(FeedNameResources))]
        MolassesSugarCane,

        [LocalizedDescription("NativePrairieHay", typeof(FeedNameResources))]
        NativePrairieHay,

        [LocalizedDescription("Oats", typeof(FeedNameResources))]
        Oats,

        [LocalizedDescription("OatForage", typeof(FeedNameResources))]
        OatForage,

        [LocalizedDescription("OatGrain", typeof(FeedNameResources))]
        OatGrain,

        [LocalizedDescription("OatsGrainRolled", typeof(FeedNameResources))]
        OatsGrainRolled,

        [LocalizedDescription("OatGroats", typeof(FeedNameResources))]
        OatGroats,

        [LocalizedDescription("OatHay", typeof(FeedNameResources))]
        OatHay,

        [LocalizedDescription("OatHayHeaded", typeof(FeedNameResources))]
        OatHayHeaded,

        [LocalizedDescription("OatHulls", typeof(FeedNameResources))]
        OatHulls,

        [LocalizedDescription("OatsNaked", typeof(FeedNameResources))]
        OatsNaked,

        [LocalizedDescription("OatSilageHeaded", typeof(FeedNameResources))]
        OatSilageHeaded,

        [LocalizedDescription("OatsRolledDehulled", typeof(FeedNameResources))]
        OatsRolledDehulled,

        [LocalizedDescription("OatStraw", typeof(FeedNameResources))]
        OatStraw,

        [LocalizedDescription("Oatlage", typeof(FeedNameResources))]
        Oatlage,

        [LocalizedDescription("Onions", typeof(FeedNameResources))]
        Onions,

        [LocalizedDescription("OrchardgrassHay", typeof(FeedNameResources))]
        OrchardgrassHay,

        [LocalizedDescription("PalmKernelExpelled", typeof(FeedNameResources))]
        PalmKernelExpelled,

        [LocalizedDescription("PalmKernelMeal", typeof(FeedNameResources))]
        PalmKernelMeal,

        [LocalizedDescription("PalmKernelMealMech", typeof(FeedNameResources))]
        PalmKernelMealMech,

        [LocalizedDescription("PhaselousBeans", typeof(FeedNameResources))]
        PhaselousBeans,

        [LocalizedDescription("PeaHulls", typeof(FeedNameResources))]
        PeaHulls,

        [LocalizedDescription("PeaProteinConcentrate", typeof(FeedNameResources))]
        PeaProteinConcentrate,

        [LocalizedDescription("PeaProteinIsolate", typeof(FeedNameResources))]
        PeaProteinIsolate,

        [LocalizedDescription("PeanutHay", typeof(FeedNameResources))]
        PeanutHay,

        [LocalizedDescription("PeanutHulls", typeof(FeedNameResources))]
        PeanutHulls,

        [LocalizedDescription("PeanutMeal", typeof(FeedNameResources))]
        PeanutMeal,

        [LocalizedDescription("PeanutMealExpelled", typeof(FeedNameResources))]
        PeanutMealExpelled,
            
        [LocalizedDescription("PeanutMealMech", typeof(FeedNameResources))]
        PeanutMealMech,

        [LocalizedDescription("PeanutMealSolvent", typeof(FeedNameResources))]
        PeanutMealSolvent,

        [LocalizedDescription("PeanutSilage", typeof(FeedNameResources))]
        PeanutSilage,

        [LocalizedDescription("Peas", typeof(FeedNameResources))]
        Peas,

        [LocalizedDescription("PeavineForage", typeof(FeedNameResources))]
        PeavineForage,

        [LocalizedDescription("PeavineHay", typeof(FeedNameResources))]
        PeavineHay,

        [LocalizedDescription("PeavineSilage", typeof(FeedNameResources))]
        PeavineSilage,

        [LocalizedDescription("PetFoodByProducts", typeof(FeedNameResources))]
        PetFoodByProducts,

        [LocalizedDescription("PorcineSolublesDried", typeof(FeedNameResources))]
        PorcineSolublesDried,

        [LocalizedDescription("PotatoProteinConcentrate", typeof(FeedNameResources))]
        PotatoProteinConcentrate,

        [LocalizedDescription("PotatoProtein", typeof(FeedNameResources))]
        PotatoProtein,

        [LocalizedDescription("PoultryByProduct", typeof(FeedNameResources))]
        PoultryByProduct,

        [LocalizedDescription("PoultryByProductMeal", typeof(FeedNameResources))]
        PoultryByProductMeal,

        [LocalizedDescription("PoultryMeal", typeof(FeedNameResources))]
        PoultryMeal,

        [LocalizedDescription("PopcornGrain", typeof(FeedNameResources))]
        PopcornGrain,

        [LocalizedDescription("PotatoByproductDry", typeof(FeedNameResources))]
        PotatoByproductDry,

        [LocalizedDescription("PotatoByProductMeal", typeof(FeedNameResources))]
        PotatoByProductMeal,

        [LocalizedDescription("PotatoByproductWet", typeof(FeedNameResources))]
        PotatoByproductWet,

        [LocalizedDescription("PotatoPeels", typeof(FeedNameResources))]
        PotatoPeels,

        [LocalizedDescription("Potatoes", typeof(FeedNameResources))]
        Potatoes,

        [LocalizedDescription("Rice", typeof(FeedNameResources))]
        Rice,

        [LocalizedDescription("RiceBran", typeof(FeedNameResources))]
        RiceBran,

        [LocalizedDescription("RiceBranDefatted", typeof(FeedNameResources))]
        RiceBranDefatted,

        [LocalizedDescription("RiceBroken", typeof(FeedNameResources))]
        RiceBroken,

        [LocalizedDescription("RiceGrain", typeof(FeedNameResources))]
        RiceGrain,

        [LocalizedDescription("RiceHay", typeof(FeedNameResources))]
        RiceHay,

        [LocalizedDescription("RiceHulls", typeof(FeedNameResources))]
        RiceHulls,

        [LocalizedDescription("RicePolished", typeof(FeedNameResources))]
        RicePolished,

        [LocalizedDescription("RiceProteinConcentrate", typeof(FeedNameResources))]
        RiceProteinConcentrate,

        [LocalizedDescription("RiceSilage", typeof(FeedNameResources))]
        RiceSilage,

        [LocalizedDescription("Rye", typeof(FeedNameResources))]
        Rye,

        [LocalizedDescription("RyeAnnualFresh", typeof(FeedNameResources))]
        RyeAnnualFresh,

        [LocalizedDescription("RyegrassHay", typeof(FeedNameResources))]
        RyegrassHay,

        [LocalizedDescription("RyegrassSilage", typeof(FeedNameResources))]
        RyegrassSilage,

        [LocalizedDescription("RyeAnnualSilageVegetative", typeof(FeedNameResources))]
        RyeAnnualSilageVegetative,

        [LocalizedDescription("RyeGrain", typeof(FeedNameResources))]
        RyeGrain,

        [LocalizedDescription("SafflowerMeal", typeof(FeedNameResources))]
        SafflowerMeal,

        [LocalizedDescription("SafflowerMealDehulled", typeof(FeedNameResources))]
        SafflowerMealDehulled,

        [LocalizedDescription("SafflowerMealSolvent", typeof(FeedNameResources))]
        SafflowerMealSolvent,

        [LocalizedDescription("SalmonProteinHydrolysate", typeof(FeedNameResources))]
        SalmonProteinHydrolysate,

        [LocalizedDescription("SesameMeal", typeof(FeedNameResources))]
        SesameMeal,

        [LocalizedDescription("SesameMealMech", typeof(FeedNameResources))]
        SesameMealMech,

        [LocalizedDescription("SkimmedMilkPowder", typeof(FeedNameResources))]
        SkimmedMilkPowder,

        [LocalizedDescription("Sorghum", typeof(FeedNameResources))]
        Sorghum,

        [LocalizedDescription("SorghumDistillersDriedGrainsWithSolubles", typeof(FeedNameResources))]
        SorghumDistillersDriedGrainsWithSolubles,

        [LocalizedDescription("SorghumGrainDryRolled", typeof(FeedNameResources))]
        SorghumGrainDryRolled,

        [LocalizedDescription("SorghumGrainSteamFlaked", typeof(FeedNameResources))]
        SorghumGrainSteamFlaked,

        [LocalizedDescription("SorghumSilage", typeof(FeedNameResources))]
        SorghumSilage,

        [LocalizedDescription("SorghumSudanTypeHay", typeof(FeedNameResources))]
        SorghumSudanTypeHay,

        [LocalizedDescription("SorghumSudanTypeSilage", typeof(FeedNameResources))]
        SorghumSudanTypeSilage,

        [LocalizedDescription("SoybeansFullFat", typeof(FeedNameResources))]
        SoybeansFullFat,

        [LocalizedDescription("SoybeanHay", typeof(FeedNameResources))]
        SoybeanHay,

        [LocalizedDescription("SoybeansHighProteinFullFat", typeof(FeedNameResources))]
        SoybeansHighProteinFullFat,

        [LocalizedDescription("SoybeanHulls", typeof(FeedNameResources))]
        SoybeanHulls,

        [LocalizedDescription("SoybeansLowOligosaccharideFullFat", typeof(FeedNameResources))]
        SoybeansLowOligosaccharideFullFat,

        [LocalizedDescription("SoybeanMealDehulledExpelled", typeof(FeedNameResources))]
        SoybeanMealDehulledExpelled,

        [LocalizedDescription("SoybeanMealDehulledSolventExtracted", typeof(FeedNameResources))]
        SoybeanMealDehulledSolventExtracted,

        [LocalizedDescription("SoybeanMealEnzymeTreated", typeof(FeedNameResources))]
        SoybeanMealEnzymeTreated,

        [LocalizedDescription("SoybeanMealExpelled", typeof(FeedNameResources))]
        SoybeanMealExpelled,

        [LocalizedDescription("SoybeanMealExpellers", typeof(FeedNameResources))]
        SoybeanMealExpellers,


        [LocalizedDescription("SoybeanMealHighProteinDehulledSolventExtracted", typeof(FeedNameResources))]
        SoybeanMealHighProteinDehulledSolventExtracted,

        [LocalizedDescription("SoybeanMealHighProteinExpelled", typeof(FeedNameResources))]
        SoybeanMealHighProteinExpelled,

        [LocalizedDescription("SoybeanMealLowOligosaccharideDehulledSolventExtracted", typeof(FeedNameResources))]
        SoybeanMealLowOligosaccharideDehulledSolventExtracted,

        [LocalizedDescription("SoybeanMealLowOligosaccharideExpelled", typeof(FeedNameResources))]
        SoybeanMealLowOligosaccharideExpelled,

        [LocalizedDescription("SoybeanMealNonEnzymaticallyBrowned", typeof(FeedNameResources))]
        SoybeanMealNonEnzymaticallyBrowned,

        [LocalizedDescription("SoybeanMealSolvent44", typeof(FeedNameResources))]
        SoybeanMealSolvent44,

        [LocalizedDescription("SoybeanMealSolvent48", typeof(FeedNameResources))]
        SoybeanMealSolvent48,

        [LocalizedDescription("SoybeanSeedsWhole", typeof(FeedNameResources))]
        SoybeanSeedsWhole,

        [LocalizedDescription("SoybeanSeedsWholeRoasted", typeof(FeedNameResources))]
        SoybeanSeedsWholeRoasted,

        [LocalizedDescription("SoybeanSilageEarlyMaturity", typeof(FeedNameResources))]
        SoybeanSilageEarlyMaturity,

        [LocalizedDescription("SoybeanMealHighCP", typeof(FeedNameResources))]
        SoybeanMealHighCP,

        [LocalizedDescription("SoybeanMealLowCP", typeof(FeedNameResources))]
        SoybeanMealLowCP,

        [LocalizedDescription("SoybeanMealHeated", typeof(FeedNameResources))]
        SoybeanMealHeated,

        [LocalizedDescription("SoybeanSilage", typeof(FeedNameResources))]
        SoybeanSilage,

        [LocalizedDescription("SoybeanStubble", typeof(FeedNameResources))]
        SoybeanStubble,

        [LocalizedDescription("SoybeansExpelled", typeof(FeedNameResources))]
        SoybeansExpelled,

        [LocalizedDescription("SoybeansExtruded", typeof(FeedNameResources))]
        SoybeansExptruded,

        [LocalizedDescription("SoybeansRoasted", typeof(FeedNameResources))]
        SoybeansRoasted,

        [LocalizedDescription("SoybeansWhole", typeof(FeedNameResources))]
        SoybeansWhole,

        [LocalizedDescription("SoybeanMealFermented", typeof(FeedNameResources))]
        SoybeanMealFermented,

        [LocalizedDescription("SoybeanMealMech", typeof(FeedNameResources))]
        SoybeanMealMech,

        [LocalizedDescription("SoybeanMealSolvent", typeof(FeedNameResources))]
        SoybeanMealSolvent,

        [LocalizedDescription("SoybeanMealSolventHighProtein", typeof(FeedNameResources))]
        SoybeanMealSolventHighProtein,

        [LocalizedDescription("SoybeanSeed", typeof(FeedNameResources))]
        SoybeanSeed,

        [LocalizedDescription("SoyProteinConcentrate", typeof(FeedNameResources))]
        SoyProteinConcentrate,

        [LocalizedDescription("SoyProteinIsolate", typeof(FeedNameResources))]
        SoyProteinIsolate,

        [LocalizedDescription("SudangrassFresh", typeof(FeedNameResources))]
        SudangrassFresh,

        [LocalizedDescription("SudangrassHay", typeof(FeedNameResources))]
        SudangrassHay,

        [LocalizedDescription("SudangrassSilage", typeof(FeedNameResources))]
        SudangrassSilage,

        [LocalizedDescription("SugarbeetMolasses", typeof(FeedNameResources))]
        SugarbeetMolasses,

        [LocalizedDescription("SugarBeetPulp", typeof(FeedNameResources))]
        SugarBeetPulp,

        [LocalizedDescription("SugarbeetPulpDry", typeof(FeedNameResources))]
        SugarbeetPulpDry,

        [LocalizedDescription("SugarcaneBagasseSilage", typeof(FeedNameResources))]
        SugarcaneBagrassSilage,

        [LocalizedDescription("SugarcaneHay", typeof(FeedNameResources))]
        SugarcaneHay,

        [LocalizedDescription("SugarcaneMolasses", typeof(FeedNameResources))]
        SugarcaneMolasses,

        [LocalizedDescription("SugarcaneSilage", typeof(FeedNameResources))]
        SugarcaneSilage,

        [LocalizedDescription("SunflowerFullFat", typeof(FeedNameResources))]
        SunflowerFullFat,

        [LocalizedDescription("SunflowerHulls", typeof(FeedNameResources))]
        SunflowerHulls,

        [LocalizedDescription("SunflowerMeal", typeof(FeedNameResources))]
        SunflowerMeal,

        [LocalizedDescription("SunflowerMealSolvent", typeof(FeedNameResources))]
        SunflowerMealSolvent,

        [LocalizedDescription("SunflowerMealDehulledSolventExtracted", typeof(FeedNameResources))]
        SunflowerMealDehulledSolventExtracted,


        [LocalizedDescription("SunflowerOilSeedsWhole", typeof(FeedNameResources))]
        SunflowerOilSeedsWhole,

        [LocalizedDescription("SunflowerScreenings", typeof(FeedNameResources))]
        SunflowerScreenings,

        [LocalizedDescription("SunflowerSeed", typeof(FeedNameResources))]
        SunflowerSeed,

        [LocalizedDescription("SunflowerSilage", typeof(FeedNameResources))]
        SunflowerSilage,

        [LocalizedDescription("Sunflowers", typeof(FeedNameResources))]
        Sunflowers,

        [LocalizedDescription("SwitchgrassHay", typeof(FeedNameResources))]
        SwitchgrassHay,

        [LocalizedDescription("TeffLovegrassHay", typeof(FeedNameResources))]
        TeffLovegrassHay,

        [LocalizedDescription("TimothyHay", typeof(FeedNameResources))]
        TimothyHay,

        [LocalizedDescription("TomatoPomace", typeof(FeedNameResources))]
        TomatoPomace,

        [LocalizedDescription("Triticale", typeof(FeedNameResources))]
        Triticale,

        [LocalizedDescription("TriticaleDistillersDriedGrainsWithSolubles", typeof(FeedNameResources))]
        TriticaleDistillersDriedGrainsWithSolubles,

        [LocalizedDescription("TriticaleForageFresh", typeof(FeedNameResources))]
        TriticaleForageFresh,

        [LocalizedDescription("TriticaleGrain", typeof(FeedNameResources))]
        TriticaleGrain,

        [LocalizedDescription("TriticaleHay", typeof(FeedNameResources))]
        TriticaleHay,

        [LocalizedDescription("TriticaleSilage", typeof(FeedNameResources))]
        TriticaleSilage,

        [LocalizedDescription("TriticaleSilageHeaded", typeof(FeedNameResources))]
        TriticaleSilageHeaded,

        [LocalizedDescription("WheatBran", typeof(FeedNameResources))]
        WheatBran,

        [LocalizedDescription("WheatDistillersDriedGrainsWithSolubles", typeof(FeedNameResources))]
        WheatDistillersDriedGrainsWithSolubles,

        [LocalizedDescription("WheatForageFresh", typeof(FeedNameResources))]
        WheatForageFresh,

        [LocalizedDescription("WheatGluten", typeof(FeedNameResources))]
        WheatGluten,

        [LocalizedDescription("WheatGrain", typeof(FeedNameResources))]
        WheatGrain,

        [LocalizedDescription("WheatGrainRolled", typeof(FeedNameResources))]
        WheatGrainRolled,

        [LocalizedDescription("WheatGrainFlaked", typeof(FeedNameResources))]
        WheatGrainFlaked,

        [LocalizedDescription("WheatHardRed", typeof(FeedNameResources))]
        WheatHardRed,

        [LocalizedDescription("WheatSoftRed", typeof(FeedNameResources))]
        WheatSoftRed,

        [LocalizedDescription("WheatHay", typeof(FeedNameResources))]
        WheatHay,

        [LocalizedDescription("WheatHayHeaded", typeof(FeedNameResources))]
        WheatHayHeaded,

        [LocalizedDescription("WheatMiddlings", typeof(FeedNameResources))]
        WheatMiddlings,

        [LocalizedDescription("WheatScreenings", typeof(FeedNameResources))]
        WheatScreenings,

        [LocalizedDescription("WheatShorts", typeof(FeedNameResources))]
        WheatShorts,

        [LocalizedDescription("WheyPermeateLactose", typeof(FeedNameResources))]
        WheyPermeateLactose,

        [LocalizedDescription("WheyPermeateLactose80", typeof(FeedNameResources))]
        WheyPermeateLactose80,

        [LocalizedDescription("WheyPermeateLactose85", typeof(FeedNameResources))]
        WheyPermeateLactose85,

        [LocalizedDescription("WheyPowder", typeof(FeedNameResources))]
        WheyPowder,

        [LocalizedDescription("WheyPowderProteinConcentrate", typeof(FeedNameResources))]
        WheyPowderProteinConcentrate,

        [LocalizedDescription("WheatSilage", typeof(FeedNameResources))]
        WheatSilage,

        [LocalizedDescription("WheatSilageEarlyHead", typeof(FeedNameResources))]
        WheatSilageEarlyHead,

        [LocalizedDescription("WheatStraw", typeof(FeedNameResources))]
        WheatStraw,

        [LocalizedDescription("WheyWet", typeof(FeedNameResources))]
        WheyWet,

        [LocalizedDescription("WheyWetCattle", typeof(FeedNameResources))]
        WheyWetCattle,

        [LocalizedDescription("WheyDry", typeof(FeedNameResources))]
        WheyDry,

        [LocalizedDescription("AmmoniumPhosphateDibasic", typeof(FeedNameResources))]
        AmmoniumPhosphateDibasic,

        [LocalizedDescription("AmmoniumPhosphateMonobasic", typeof(FeedNameResources))]
        AmmoniumPhosphateMonobasic,

        [LocalizedDescription("AmmoniumSulfate", typeof(FeedNameResources))]
        AmmoniumSulfate,

        [LocalizedDescription("BoneMeal", typeof(FeedNameResources))]
        BoneMeal,

        [LocalizedDescription("CalciumCarbonate", typeof(FeedNameResources))]
        CalciumCarbonate,

        [LocalizedDescription("CalciumChlorideAnhyrdrous", typeof(FeedNameResources))]
        CalciumChlorideAnhyrdrous,

        [LocalizedDescription("CalciumChlorideDihydrate", typeof(FeedNameResources))]
        CalciumChlorideDihydrate,

        [LocalizedDescription("CalciumHydroxide", typeof(FeedNameResources))]
        CalciumHydroxide,

        [LocalizedDescription("CalciumOxide", typeof(FeedNameResources))]
        CalciumOxide,

        [LocalizedDescription("CalciumPhosphateMonobasic", typeof(FeedNameResources))]
        CalciumPhosphateMonobasic,

        [LocalizedDescription("CalciumSulfateDihydrate", typeof(FeedNameResources))]
        CalciumSulfateDihydrate,

        [LocalizedDescription("CobaltCarbonate", typeof(FeedNameResources))]
        CobaltCarbonate,

        [LocalizedDescription("CopperSulfatePentahydrate", typeof(FeedNameResources))]
        CopperSulfatePentahydrate,

        [LocalizedDescription("DicalciumPhosphate", typeof(FeedNameResources))]
        DicalciumPhosphate,

        [LocalizedDescription("EDTA", typeof(FeedNameResources))]
        EDTA,

        [LocalizedDescription("IronSulfate", typeof(FeedNameResources))]
        IronSulfate,

        [LocalizedDescription("Limestone", typeof(FeedNameResources))]
        Limestone,

        [LocalizedDescription("LimestoneDilomiticMagnesium", typeof(FeedNameResources))]
        LimestoneDilomiticMagnesium,

        [LocalizedDescription("MagnesiumCarbonate", typeof(FeedNameResources))]
        MagnesiumCarbonate,

        [LocalizedDescription("MagnesiumChlorideHexahydrate", typeof(FeedNameResources))]
        MagnesiumChlorideHexahydrate,

        [LocalizedDescription("MagnesiumHydroxide", typeof(FeedNameResources))]
        MagnesiumHydroxide,

        [LocalizedDescription("MagnesiumOxide", typeof(FeedNameResources))]
        MagnesiumOxide,

        [LocalizedDescription("MagnesiumSulfateHeptahydrate", typeof(FeedNameResources))]
        MagnesiumSulfateHeptahydrate,

        [LocalizedDescription("ManganeseCarbonate", typeof(FeedNameResources))]
        ManganeseCarbonate,

        [LocalizedDescription("ManganeseOxide", typeof(FeedNameResources))]
        ManganeseOxide,

        [LocalizedDescription("ManganeseSulfateMonohydrate", typeof(FeedNameResources))]
        ManganeseSulfateMonohydrate,

        [LocalizedDescription("ManganeseSulfatePentahydrate", typeof(FeedNameResources))]
        ManganeseSulfatePentahydrate,

        [LocalizedDescription("OystershellGround", typeof(FeedNameResources))]
        OystershellGround,

        [LocalizedDescription("PhosphateDeflourinated", typeof(FeedNameResources))]
        PhosphateDeflourinated,

        [LocalizedDescription("PhosphateMonoMono", typeof(FeedNameResources))]
        PhosphateMonoMono,

        [LocalizedDescription("PhosphateRock", typeof(FeedNameResources))]
        PhosphateRock,

        [LocalizedDescription("PhosphateRockLowFlour", typeof(FeedNameResources))]
        PhosphateRockLowFlour,

        [LocalizedDescription("PhosphateRockSoft", typeof(FeedNameResources))]
        PhosphateRockSoft,

        [LocalizedDescription("PhosphoricAcid", typeof(FeedNameResources))]
        PhosphoricAcid,

        [LocalizedDescription("PotassiumBicarbonate", typeof(FeedNameResources))]
        PotassiumBicarbonate,

        [LocalizedDescription("PotassiumCarbonate", typeof(FeedNameResources))]
        PotassiumCarbonate,

        [LocalizedDescription("PotassiumChloride", typeof(FeedNameResources))]
        PotassiumChloride,

        [LocalizedDescription("PotassiumIodide", typeof(FeedNameResources))]
        PotassiumIodide,

        [LocalizedDescription("PotassiumSulfate", typeof(FeedNameResources))]
        PotassiumSulfate,

        [LocalizedDescription("SaltSodiumChloride", typeof(FeedNameResources))]
        SaltSodiumChloride,

        [LocalizedDescription("SodiumBicarbonate", typeof(FeedNameResources))]
        SodiumBicarbonate,

        [LocalizedDescription("SodiumPhosphateMonobasicMonohydrate", typeof(FeedNameResources))]
        SodiumPhosphateMonobasicMonohydrate,

        [LocalizedDescription("SodiumSelenite", typeof(FeedNameResources))]
        SodiumSelenite,

        [LocalizedDescription("SodiumSulfateDecahydrate", typeof(FeedNameResources))]
        SodiumSulfateDecahydrate,

        [LocalizedDescription("SodiumTripolyphosphate", typeof(FeedNameResources))]
        SodiumTripolyphosphate,

        [LocalizedDescription("YeastEthanol", typeof(FeedNameResources))]
        YeastEthanol,

        [LocalizedDescription("YeastSingleCellProtein", typeof(FeedNameResources))]
        YeastSingleCellProtein,

        [LocalizedDescription("YeastTorula", typeof(FeedNameResources))]
        YeastTorula,

        [LocalizedDescription("Urea", typeof(FeedNameResources))]
        Urea,

        [LocalizedDescription("ZincOxide", typeof(FeedNameResources))]
        ZincOxide,

        [LocalizedDescription("ZincSulfate", typeof(FeedNameResources))]
        ZincSulfate
    }
}