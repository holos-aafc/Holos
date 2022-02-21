using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum FallowTypes
    {
        [LocalizedDescription("EnumMechanicalFallow", typeof(Resources))]
        Mechanical,

        [LocalizedDescription("EnumChemicalFallow", typeof(Resources))]
        Chemical,
    }
}