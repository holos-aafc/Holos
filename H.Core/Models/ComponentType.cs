using H.Infrastructure;

namespace H.Core.Models
{
    public enum ComponentType
    {
        [LocalizedDescription("EnumRotation", typeof(Properties.Resources))]
        Rotation,

        [LocalizedDescription("EnumPasture", typeof(Properties.Resources))]
        Pasture,

        [LocalizedDescription("EnumRange", typeof(Properties.Resources))]
        Range,

        [LocalizedDescription("EnumShelterbelt", typeof(Properties.Resources))]
        Shelterbelt,

        [LocalizedDescription("EnumField", typeof(Properties.Resources))]
        Field,

        [LocalizedDescription("EnumCowCalf", typeof(Properties.Resources))]
        CowCalf,

        [LocalizedDescription("EnumBackgrounding", typeof(Properties.Resources))]
        Backgrounding,

        [LocalizedDescription("EnumFinishing", typeof(Properties.Resources))]
        Finishing,

        [LocalizedDescription("EnumGrassland", typeof(Properties.Resources))]
        Grassland,

        [LocalizedDescription("EnumDairy", typeof(Properties.Resources))]
        Dairy,

        [LocalizedDescription("EnumDairyLactating", typeof(Properties.Resources))]
        DairyLactating,

        [LocalizedDescription("EnumDairyHeifer", typeof(Properties.Resources))]
        DairyHeifer,

        [LocalizedDescription("EnumDairyDry", typeof(Properties.Resources))]
        DairyDry,

        [LocalizedDescription("EnumDairyCalf", typeof(Properties.Resources))]
        DairyCalf,

        [LocalizedDescription("EnumDairyBulls", typeof(Properties.Resources))]
        DairyBulls,

        [LocalizedDescription("EnumSwine", typeof(Properties.Resources))]
        Swine,

        [LocalizedDescription("EnumBoar", typeof(Properties.Resources))]
        Boar,

        [LocalizedDescription("EnumSwineFinishers", typeof(Properties.Resources))]
        SwineFinishers,

        [LocalizedDescription("EnumSwineStarters", typeof(Properties.Resources))]
        SwineStarters,

        [LocalizedDescription("EnumSwineLactatingSows", typeof(Properties.Resources))]
        SwineLactatingSows,

        [LocalizedDescription("EnumSwineDrySows", typeof(Properties.Resources))]
        SwineDrySows,

        [LocalizedDescription("EnumSwineGrowers", typeof(Properties.Resources))]
        SwineGrowers,

        [LocalizedDescription("EnumPoultry", typeof(Properties.Resources))]
        Poultry,

        [LocalizedDescription("EnumPoultryLayersWet", typeof(Properties.Resources))]
        PoultryLayersWet,

        [LocalizedDescription("EnumPoultryTurkeys", typeof(Properties.Resources))]
        PoultryTurkeys,

        [LocalizedDescription("EnumPoultryGeese", typeof(Properties.Resources))]
        PoultryGeese,

        [LocalizedDescription("EnumPoultryBroilers", typeof(Properties.Resources))]
        PoultryBroilers,

        [LocalizedDescription("EnumPoultryDucks", typeof(Properties.Resources))]
        PoultryDucks,

        [LocalizedDescription("EnumPoultryLayersDry", typeof(Properties.Resources))]
        PoultryLayersDry,

        [LocalizedDescription("EnumSheep", typeof(Properties.Resources))]
        Sheep,

        [LocalizedDescription("EnumSheepFeedlot", typeof(Properties.Resources))]
        SheepFeedlot,

        [LocalizedDescription("EnumRams", typeof(Properties.Resources))]
        Rams,

        [LocalizedDescription("EnumLambsAndEwes", typeof(Properties.Resources))]
        LambsAndEwes,

        [LocalizedDescription("EnumOtherLivestock", typeof(Properties.Resources))]
        OtherLivestock,

        [LocalizedDescription("EnumAlpaca", typeof(Properties.Resources))]
        Alpaca,

        [LocalizedDescription("EnumElk", typeof(Properties.Resources))]
        Elk,

        [LocalizedDescription("EnumGoats", typeof(Properties.Resources))]
        Goats,

        [LocalizedDescription("EnumDeer", typeof(Properties.Resources))]
        Deer,

        [LocalizedDescription("EnumHorses", typeof(Properties.Resources))]
        Horses,

        [LocalizedDescription("EnumMules", typeof(Properties.Resources))]
        Mules,

        [LocalizedDescription("EnumBison", typeof(Properties.Resources))]
        Bison,

        [LocalizedDescription("EnumLlamas", typeof(Properties.Resources))]
        Llamas,

        [LocalizedDescription("EnumFarrowToWean", typeof(Properties.Resources))]
        FarrowToWean,

        [LocalizedDescription("EnumSwineIsoWean", typeof(Properties.Resources))]
        IsoWean,

        [LocalizedDescription("EnumSwineSwineFarrowToFinish", typeof(Properties.Resources))]
        FarrowToFinish,

        [LocalizedDescription("EnumPulletFarm", typeof(Properties.Resources))]
        ChickenPulletFarm,

        [LocalizedDescription("EnumChickenMultiplierBreeder", typeof(Properties.Resources))]
        ChickenMultiplierBreeder,

        [LocalizedDescription("EnumChickenMeatProduction", typeof(Properties.Resources))]
        ChickenMeatProduction,

        [LocalizedDescription("EnumTurkeyMultiplierBreeder", typeof(Properties.Resources))]
        TurkeyMultiplierBreeder,

        [LocalizedDescription("EnumTurkeyMeatProduction", typeof(Properties.Resources))]
        TurkeyMeatProduction,

        [LocalizedDescription("EnumChickenEggProduction", typeof(Properties.Resources))]
        ChickenEggProduction,

        [LocalizedDescription("EnumChickenMultiplierHatchery", typeof(Properties.Resources))]
        ChickenMultiplierHatchery,

        [LocalizedDescription("EnumAnaerobicDigestion", typeof(Properties.Resources))]
        AnaerobicDigestion,
    }
}