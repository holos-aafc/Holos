using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum TreeSpecies
    {
        [LocalizedDescription("Caragana", typeof(Resources))]
        Caragana,

        [LocalizedDescription("GreenAsh", typeof(Resources))]
        GreenAsh,

        [LocalizedDescription("HybridPoplar", typeof(Resources))]
        HybridPoplar,

        [LocalizedDescription("ManitobaMaple", typeof(Resources))]
        ManitobaMaple,

        [LocalizedDescription("ScotsPine", typeof(Resources))]
        ScotsPine,

        [LocalizedDescription("WhiteSpruce", typeof(Resources))]
        WhiteSpruce,

        [LocalizedDescription("AverageDeciduous", typeof(Resources))]
        AverageDeciduous,

        [LocalizedDescription("AverageConifer", typeof(Resources))]
        AverageConifer,
    }
}