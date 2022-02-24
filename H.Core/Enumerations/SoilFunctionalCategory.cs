using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Holos version 3 called this 'Soil Type'
    /// </summary>
    public enum SoilFunctionalCategory
    {
        [LocalizedDescription("NotApplicable", typeof(Resources))]
        NotApplicable,

        [LocalizedDescription("Brown", typeof(Resources))]
        Brown,

        [LocalizedDescription("BrownChernozem", typeof(Resources))]
        BrownChernozem,

        [LocalizedDescription("DarkBrown", typeof(Resources))]
        DarkBrown,

        [LocalizedDescription("DarkBrownChernozem", typeof(Resources))]
        DarkBrownChernozem,

        [LocalizedDescription("Black", typeof(Resources))]
        Black,

        [LocalizedDescription("BlackGrayChernozem", typeof(Resources))]
        BlackGrayChernozem,

        [LocalizedDescription("Organic", typeof(Resources))]
        Organic,

        [LocalizedDescription("EasternCanada", typeof(Resources))]
        EasternCanada,

        [LocalizedDescription("EnumSoilFunctionalAll", typeof(Resources))]
        All,

        Grey,
        DarkGrey
    }
}