using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum DietType
    {
        [LocalizedDescription("BarleyDietType", typeof(Resources))]
        Barley,

        [LocalizedDescription("CloseUpDietType", typeof(Resources))]
        CloseUp,

        [LocalizedDescription("CornDietType", typeof(Resources))]
        Corn,

        [LocalizedDescription("FarOffDryDietType", typeof(Resources))]
        FarOffDry,

        [LocalizedDescription("HighEnergyDietType", typeof(Resources))]
        HighEnergy,

        [LocalizedDescription("HighEnergyAndProteinDietType", typeof(Resources))]
        HighEnergyAndProtein,

        [LocalizedDescription("LowEnergyDietType", typeof(Resources))]
        LowEnergy,

        [LocalizedDescription("LowEnergyAndProteinDietType", typeof(Resources))]
        LowEnergyAndProtein,

        [LocalizedDescription("MediumEnergyDietType", typeof(Resources))]
        MediumEnergy,

        [LocalizedDescription("MediumEnergyAndProteinDietType", typeof(Resources))]
        MediumEnergyAndProtein,

        [LocalizedDescription("MediumGrowthDietType", typeof(Resources))]
        MediumGrowth,

        [LocalizedDescription("SlowGrowthDietType", typeof(Resources))]
        SlowGrowth,

        [LocalizedDescription("HighlyDigestibleFeed", typeof(Resources))]
        HighlyDigestibleFeed,

        [LocalizedDescription("ReducedProtein", typeof(Resources))]
        ReducedProtein,

        [LocalizedDescription("Standard", typeof(Resources))]
        Standard,

        [LocalizedDescription("HighFiberDietType", typeof(Resources))]
        HighFiber,

        [LocalizedDescription("LowFiberDietType", typeof(Resources))]
        LowFiber,

        [LocalizedDescription("FarOffDietType", typeof(Resources))]
        FarOff,

        [LocalizedDescription("LegumeForageBasedDietType", typeof(Resources))]
        LegumeForageBased,

        [LocalizedDescription("BarleySilageBasedDiet", typeof(Resources))]
        BarleySilageBased,

        [LocalizedDescription("CornSilageBasedDietType", typeof(Resources))]
        CornSilageBased,

        [LocalizedDescription("BarleyGrainBasedDietType", typeof(Resources))]
        BarleyGrainBased,

        [LocalizedDescription("CornGrainBasedDietType", typeof(Resources))]
        CornGrainBased,
        
        [LocalizedDescription("GestationDietType", typeof(Resources))]
        Gestation,

        [LocalizedDescription("LactationDietType", typeof(Resources))]
        Lactation,

        [LocalizedDescription("NurseryWeanersStarterDiet1Type", typeof(Resources))]
        NurseryWeanersStarter1,

        [LocalizedDescription("NurseryWeanersStarterDiet2Type", typeof(Resources))]
        NurseryWeanersStarter2,

        [LocalizedDescription("GrowerFinisherDiet1Type", typeof(Resources))]
        GrowerFinisherDiet1,

        [LocalizedDescription("GrowerFinisherDiet2Type", typeof(Resources))]
        GrowerFinisherDiet2,

        [LocalizedDescription("GrowerFinisherDiet3Type", typeof(Resources))]
        GrowerFinisherDiet3,

        [LocalizedDescription("GrowerFinisherDiet4Type", typeof(Resources))]
        GrowerFinisherDiet4,

        [LocalizedDescription("GoodQualityForageDietType", typeof(Resources))]
        GoodQualityForage,

        [LocalizedDescription("AverageQualityForageDietType", typeof(Resources))]
        AverageQualityForage,

        [LocalizedDescription("PoorQualityForageDietType", typeof(Resources))]
        PoorQualityForage,

        [LocalizedDescription("None", typeof(Resources))]
        None,

        [LocalizedDescription("All", typeof(Resources))]
        All,

        [LocalizedDescription("Forage", typeof(Resources))]
        Forage,
    }
}