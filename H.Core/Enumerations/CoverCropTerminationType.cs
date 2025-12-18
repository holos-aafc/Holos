using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Cover crop termination type depends on the type of tillage (reduced/no-till = chemical, intensive = mechanical)
    /// </summary>
    public enum CoverCropTerminationType
    {
        [LocalizedDescription("EnumOptionA", typeof(Resources))]
        OptionA,

        [LocalizedDescription("EnumOptionB", typeof(Resources))]
        OptionB,

        [LocalizedDescription("EnumOptionC", typeof(Resources))]
        OptionC,

        [LocalizedDescription("EnumNaturalTerminationType", typeof(Resources))]
        Natural,

        [LocalizedDescription("EnumChemicalTerminationType", typeof(Resources))]
        Chemical,

        [LocalizedDescription("EnumMechanicalTerminationType", typeof(Resources))]
        Mechanical,
    }
}