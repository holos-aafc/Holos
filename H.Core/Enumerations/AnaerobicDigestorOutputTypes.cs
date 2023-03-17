using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum AnaerobicDigestorOutputTypes
    {
        [LocalizedDescription("EnumElectricity", typeof(Resources))]
        Electricity,

        [LocalizedDescription("EnumHeat", typeof(Resources))]
        Heat,

        [LocalizedDescription("EnumMethaneToGrid", typeof(Resources))]
        MethaneInjectionToGasGrid
    }
}