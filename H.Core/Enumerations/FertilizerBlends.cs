using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum FertilizerBlends
    {
        [LocalizedDescription("EnumUrea", typeof(Resources))]
        Urea,

        [LocalizedDescription("EnumAmmonia", typeof(Resources))]
        Ammonia,

        [LocalizedDescription("EnumUreaAmmoniumNitrate", typeof(Resources))]
        UreaAmmoniumNitrate,

        [LocalizedDescription("EnumAmmoniumNitrate", typeof(Resources))]
        AmmoniumNitrate,

        [LocalizedDescription("EnumCalciumAmmoniumNitrate", typeof(Resources))]
        CalciumAmmoniumNitrate,

        [LocalizedDescription("EnumAmmoniumSulphate", typeof(Resources))]
        AmmoniumSulphate,

        [LocalizedDescription("EnumMesS15", typeof(Resources))]
        MesS15,

        [LocalizedDescription("EnumMonoAmmoniumPhosphate", typeof(Resources))]
        MonoAmmoniumPhosphate,

        [LocalizedDescription("EnumDiAmmoniumPhosphate", typeof(Resources))]
        DiAmmoniumPhosphate,

        [LocalizedDescription("EnumTripleSuperPhosphate", typeof(Resources))]
        TripleSuperPhosphate,

        [LocalizedDescription("EnumPotash", typeof(Resources))]
        Potash,

        [LocalizedDescription("EnumNpk", typeof(Resources))]
        Npk,

        [LocalizedDescription("EnumCalciumNitrate", typeof(Resources))]
        CalciumNitrate,

        [LocalizedDescription("EnumAmmoniumNitroSulphate", typeof(Resources))]
        AmmoniumNitroSulphate,

        [LocalizedDescription("EnumCustom", typeof(Resources))]
        Custom,

        [LocalizedDescription("EnumLime", typeof(Resources))]
        Lime,
    }
}