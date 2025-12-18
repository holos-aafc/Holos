using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models
{
    public enum ComponentType
    {
        [LocalizedDescription("EnumRotation", typeof(Resources))]
        Rotation,

        [LocalizedDescription("EnumPasture", typeof(Resources))]
        Pasture,

        [LocalizedDescription("EnumRange", typeof(Resources))]
        Range,

        [LocalizedDescription("EnumShelterbelt", typeof(Resources))]
        Shelterbelt,

        [LocalizedDescription("EnumField", typeof(Resources))]
        Field,

        [LocalizedDescription("EnumCowCalf", typeof(Resources))]
        CowCalf,

        [LocalizedDescription("EnumBackgrounding", typeof(Resources))]
        Backgrounding,

        [LocalizedDescription("EnumFinishing", typeof(Resources))]
        Finishing,

        [LocalizedDescription("EnumGrassland", typeof(Resources))]
        Grassland,

        [LocalizedDescription("EnumDairy", typeof(Resources))]
        Dairy,

        [LocalizedDescription("EnumDairyLactating", typeof(Resources))]
        DairyLactating,

        [LocalizedDescription("EnumDairyHeifer", typeof(Resources))]
        DairyHeifer,

        [LocalizedDescription("EnumDairyDry", typeof(Resources))]
        DairyDry,

        [LocalizedDescription("EnumDairyCalf", typeof(Resources))]
        DairyCalf,

        [LocalizedDescription("EnumDairyBulls", typeof(Resources))]
        DairyBulls,

        [LocalizedDescription("EnumSwine", typeof(Resources))]
        Swine,

        [LocalizedDescription("EnumBoar", typeof(Resources))]
        Boar,

        [LocalizedDescription("EnumSwineFinishers", typeof(Resources))]
        SwineFinishers,

        [LocalizedDescription("EnumSwineStarters", typeof(Resources))]
        SwineStarters,

        [LocalizedDescription("EnumSwineLactatingSows", typeof(Resources))]
        SwineLactatingSows,

        [LocalizedDescription("EnumSwineDrySows", typeof(Resources))]
        SwineDrySows,

        [LocalizedDescription("EnumSwineGrowers", typeof(Resources))]
        SwineGrowers,

        [LocalizedDescription("EnumPoultry", typeof(Resources))]
        Poultry,

        [LocalizedDescription("EnumPoultryLayersWet", typeof(Resources))]
        PoultryLayersWet,

        [LocalizedDescription("EnumPoultryTurkeys", typeof(Resources))]
        PoultryTurkeys,

        [LocalizedDescription("EnumPoultryGeese", typeof(Resources))]
        PoultryGeese,

        [LocalizedDescription("EnumPoultryBroilers", typeof(Resources))]
        PoultryBroilers,

        [LocalizedDescription("EnumPoultryDucks", typeof(Resources))]
        PoultryDucks,

        [LocalizedDescription("EnumPoultryLayersDry", typeof(Resources))]
        PoultryLayersDry,

        [LocalizedDescription("EnumSheep", typeof(Resources))]
        Sheep,

        [LocalizedDescription("EnumSheepFeedlot", typeof(Resources))]
        SheepFeedlot,

        [LocalizedDescription("EnumRams", typeof(Resources))]
        Rams,

        [LocalizedDescription("EnumLambsAndEwes", typeof(Resources))]
        LambsAndEwes,

        [LocalizedDescription("EnumOtherLivestock", typeof(Resources))]
        OtherLivestock,

        [LocalizedDescription("EnumAlpaca", typeof(Resources))]
        Alpaca,

        [LocalizedDescription("EnumElk", typeof(Resources))]
        Elk,

        [LocalizedDescription("EnumGoats", typeof(Resources))]
        Goats,

        [LocalizedDescription("EnumDeer", typeof(Resources))]
        Deer,

        [LocalizedDescription("EnumHorses", typeof(Resources))]
        Horses,

        [LocalizedDescription("EnumMules", typeof(Resources))]
        Mules,

        [LocalizedDescription("EnumBison", typeof(Resources))]
        Bison,

        [LocalizedDescription("EnumLlamas", typeof(Resources))]
        Llamas,

        [LocalizedDescription("EnumFarrowToWean", typeof(Resources))]
        FarrowToWean,

        [LocalizedDescription("EnumSwineIsoWean", typeof(Resources))]
        IsoWean,

        [LocalizedDescription("EnumSwineSwineFarrowToFinish", typeof(Resources))]
        FarrowToFinish,

        [LocalizedDescription("EnumPulletFarm", typeof(Resources))]
        ChickenPulletFarm,

        [LocalizedDescription("EnumChickenMultiplierBreeder", typeof(Resources))]
        ChickenMultiplierBreeder,

        [LocalizedDescription("EnumChickenMeatProduction", typeof(Resources))]
        ChickenMeatProduction,

        [LocalizedDescription("EnumTurkeyMultiplierBreeder", typeof(Resources))]
        TurkeyMultiplierBreeder,

        [LocalizedDescription("EnumTurkeyMeatProduction", typeof(Resources))]
        TurkeyMeatProduction,

        [LocalizedDescription("EnumChickenEggProduction", typeof(Resources))]
        ChickenEggProduction,

        [LocalizedDescription("EnumChickenMultiplierHatchery", typeof(Resources))]
        ChickenMultiplierHatchery,

        [LocalizedDescription("EnumAnaerobicDigestion", typeof(Resources))]
        AnaerobicDigestion
    }
}