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
        Standard
    }
}