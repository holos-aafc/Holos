using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// These describe the different pathways calves take to slaughter once they are weaned
    ///
    /// From Legesse et al. (2016 - Greenhouse gas emissions of Canadian beef production in 1981 as compared with 2011)
    /// </summary>
    public enum CalfToSlaughterPathways
    {
        [LocalizedDescription("EnumCalfFed", typeof(Resources))]
        CalfFed,

        [LocalizedDescription("EnumYearlingFed", typeof(Resources))]
        YearlingFed,

        [LocalizedDescription("EnumYearlingGrassFed", typeof(Resources))]
        YearlingGrassFed,
    }
}