using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum NitrogenFertilizerType
    {
        [LocalizedDescription("EnumUrea", typeof(Resources))]
        Urea,

        [LocalizedDescription("EnumUreaAmmoniumNitrate", typeof(Resources))]
        UreaAmmoniumNitrate,

        [LocalizedDescription("EnumAnhydrousAmmonia", typeof(Resources))]
        AnhydrousAmmonia,

        [LocalizedDescription("EnumOther", typeof(Resources))]
        Other,
    }
}