using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// http://sis.agr.gc.ca/cansis/nsdb/soil/v2/snt/drainage.html
    /// </summary>
    public enum SoilDrainageClasses
    {
        [LocalizedDescription("EnumVeryRapidlyDrained", typeof(Resources))]
        VeryRapidlyDrained,

        [LocalizedDescription("EnumRapidlyDrained", typeof(Resources))]
        RapidlyDrained,

        [LocalizedDescription("EnumWellDrained", typeof(Resources))]
        WellDrained,

        [LocalizedDescription("EnumModeratelyWellDrained", typeof(Resources))]
        ModeratelyWellDrained,

        [LocalizedDescription("EnumImperfectlyDrained", typeof(Resources))]
        ImperfectlyDrained,

        [LocalizedDescription("EnumPoorlyDrained", typeof(Resources))]
        PoorlyDrained,

        [LocalizedDescription("EnumVeryPoorlyDrained", typeof(Resources))]
        VeryPoorlyDrained,

        [LocalizedDescription("EnumNotApplicable", typeof(Resources))]
        NotApplicable,
    }
}