using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum AnimalType
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("EnumAlpacas", typeof(Resources))]
        Alpacas,

        [LocalizedDescription("EnumBeefBackgrounder", typeof(Resources))]
        BeefBackgrounder,

        [LocalizedDescription("EnumBeefBackgrounderSteer", typeof(Resources))]
        BeefBackgrounderSteer,

        [LocalizedDescription("EnumBeefBackgrounderHeifer", typeof(Resources))]
        BeefBackgrounderHeifer,

        [LocalizedDescription("EnumBeefFinishingSteer", typeof(Resources))]
        BeefFinishingSteer,

        [LocalizedDescription("EnumBeefFinishingHeifer", typeof(Resources))]
        BeefFinishingHeifer,

        [LocalizedDescription("EnumBeef", typeof(Resources))]
        Beef,

        [LocalizedDescription("EnumBeefBull", typeof(Resources))]
        BeefBulls,

        [LocalizedDescription("EnumBeefCalf", typeof(Resources))]
        BeefCalf,

        /// <summary>
        /// This also means 'regular' cows (i.e. non-lactating)
        /// </summary>
        [LocalizedDescription("EnumBeefCowLactating", typeof(Resources))]
        BeefCowLactating,

        [LocalizedDescription("EnumBeefCowDry", typeof(Resources))]
        BeefCowDry,

        [LocalizedDescription("EnumBeefFinisher", typeof(Resources))]
        BeefFinisher,

        /// <summary>
        /// Also known as buffalo
        /// </summary>
        [LocalizedDescription("EnumBison", typeof(Resources))]
        Bison,

        [LocalizedDescription("EnumBoar", typeof(Resources))]
        SwineBoar,

        [LocalizedDescription("EnumBroilers", typeof(Resources))]
        Broilers,

        [LocalizedDescription("EnumChicken", typeof(Resources))]
        Chicken,

        [LocalizedDescription("EnumCowCalf", typeof(Resources))]
        CowCalf,

        [LocalizedDescription("EnumBeefCow", typeof(Resources))]
        BeefCow,

        [LocalizedDescription("EnumCalf", typeof(Resources))]
        Calf,

        [LocalizedDescription("EnumDairy", typeof(Resources))]
        Dairy,

        [LocalizedDescription("EnumDairyBulls", typeof(Resources))]
        DairyBulls,

        [LocalizedDescription("EnumDairyDry", typeof(Resources))]
        DairyDryCow,

        [LocalizedDescription("EnumDairyCalves", typeof(Resources))]
        DairyCalves,

        [LocalizedDescription("EnumDairyHeifers", typeof(Resources))]
        DairyHeifers,

        [LocalizedDescription("EnumDairyLactating", typeof(Resources))]
        DairyLactatingCow,

        [LocalizedDescription("EnumDeer", typeof(Resources))]
        Deer,

        [LocalizedDescription("EnumDrySow", typeof(Resources))]
        SwineDrySow,

        [LocalizedDescription("EnumDucks", typeof(Resources))]
        Ducks,

        [LocalizedDescription("EnumElk", typeof(Resources))]
        Elk,

        /// <summary>
        /// Assumption is all ewes are pregnant
        /// </summary>
        [LocalizedDescription("EnumEwes", typeof(Resources))]
        Ewes,

        [LocalizedDescription("EnumGeese", typeof(Resources))]
        Geese,

        [LocalizedDescription("EnumGoats", typeof(Resources))]
        Goats,

        [LocalizedDescription("EnumGrower", typeof(Resources))]
        SwineGrower,

        [LocalizedDescription("EnumHorses", typeof(Resources))]
        Horses,

        [LocalizedDescription("EnumLambs", typeof(Resources))]
        Lambs,

        [LocalizedDescription("EnumLambsAndEwes", typeof(Resources))]
        LambsAndEwes,

        [LocalizedDescription("EnumLactatingSow", typeof(Resources))]
        SwineLactatingSow,

        [LocalizedDescription("EnumLayersDryPoultry", typeof(Resources))]
        LayersDryPoultry,

        [LocalizedDescription("EnumLayersWetPoultry", typeof(Resources))]
        LayersWetPoultry,

        [LocalizedDescription("EnumLlamas", typeof(Resources))]
        Llamas,

        [LocalizedDescription("EnumMules", typeof(Resources))]
        Mules,

        [LocalizedDescription("EnumOtherLivestock", typeof(Resources))]
        OtherLivestock,

        [LocalizedDescription("EnumPoultry", typeof(Resources))]
        Poultry,

        [LocalizedDescription("EnumReplacementHeifers", typeof(Resources))]
        BeefReplacementHeifers,

        [LocalizedDescription("EnumSheep", typeof(Resources))]
        Sheep,

        [LocalizedDescription("EnumRam", typeof(Resources))]
        Ram,

        [LocalizedDescription("EnumWeanedLamb", typeof(Resources))]
        WeanedLamb,

        [LocalizedDescription("EnumSheepFeedlot", typeof(Resources))]
        SheepFeedlot,

        [LocalizedDescription("Stockers", typeof(Resources))]
        Stockers,

        [LocalizedDescription("EnumStockerSteers", typeof(Resources))]
        StockerSteers,

        [LocalizedDescription("EnumStockerHeifers", typeof(Resources))]
        StockerHeifers,

        [LocalizedDescription("EnumSwine", typeof(Resources))]
        Swine,

        [LocalizedDescription("EnumSwineStarters", typeof(Resources))]
        SwineStarter,

        [LocalizedDescription("EnumSwineFinishers", typeof(Resources))]
        SwineFinisher,

        [LocalizedDescription("EnumTurkeys", typeof(Resources))]
        Turkeys,

        [LocalizedDescription("EnumYoungBulls", typeof(Resources))]
        YoungBulls,

        /// <summary>
        /// Female pigs that have not farrowed a litter. Also known as maiden gilts.
        /// </summary>
        [LocalizedDescription("EnumSwineGilts", typeof(Resources))]
        SwineGilts,

        [LocalizedDescription("EnumSwineSows", typeof(Resources))]
        SwineSows,

        [LocalizedDescription("EnumSwinePiglets", typeof(Resources))]
        SwinePiglets,

        /// <summary>
        /// Juvenile female
        /// </summary>
        [LocalizedDescription("EnumChickenPullets", typeof(Resources))]
        ChickenPullets,

        /// <summary>
        /// Juvenile male
        /// </summary>
        [LocalizedDescription("EnumChickenCockerel", typeof(Resources))]
        ChickenCockerels,

        /// <summary>
        /// Adult male
        /// </summary>
        [LocalizedDescription("EnumChickenRooster", typeof(Resources))]
        ChickenRoosters,

        /// <summary>
        /// Adult female
        /// </summary>
        [LocalizedDescription("EnumChickenHen", typeof(Resources))]
        ChickenHens,

        /// <summary>
        /// Juvenile male turkey
        /// </summary>
        [LocalizedDescription("EnumYoungTom", typeof(Resources))]
        YoungTom,

        /// <summary>
        /// Adult male turkey
        /// </summary>
        [LocalizedDescription("EnumTom", typeof(Resources))]
        Tom,

        /// <summary>
        /// Young female turkey
        /// </summary>
        [LocalizedDescription("EnumYoungTurkeyHen", typeof(Resources))]
        YoungTurkeyHen,

        /// <summary>
        /// Adult female turkey
        /// </summary>
        [LocalizedDescription("EnumTurkeyHen", typeof(Resources))]
        TurkeyHen,

        [LocalizedDescription("EnumChickenEggs", typeof(Resources))]
        ChickenEggs,

        [LocalizedDescription("EnumTurkeyEggs", typeof(Resources))]
        TurkeyEggs,

        /// <summary>
        /// Newly hatched chicken
        /// </summary>
        [LocalizedDescription("EnumChicks", typeof(Resources))]
        Chicks,

        /// <summary>
        /// Newly hatched turkey
        /// </summary>
        [LocalizedDescription("EnumPoults", typeof(Resources))]
        Poults,

        [LocalizedDescription("EnumCattle", typeof(Resources))]
        Cattle,

        [LocalizedDescription("EnumLayers", typeof(Resources))]
        Layers,
    }
}