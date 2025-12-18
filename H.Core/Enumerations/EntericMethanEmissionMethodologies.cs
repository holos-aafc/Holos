using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum EntericMethanEmissionMethodologies
    {
        // The old/normal way to calculate enteric methane
        [LocalizedDescription("EnumDefault", typeof(Resources))]
        Default,

        [LocalizedDescription("EnumEscobarEcobarBahamondes2017a", typeof(Resources))]
        EscobarBahamondes2017a,

        [LocalizedDescription("EnumLingen2019eq17", typeof(Resources))]
        Lingen2019eq17,

        [LocalizedDescription("EnumEcobarBahamondes2017aLFMC", typeof(Resources))]
        EcobarBahamondes2017aLFMC,

        [LocalizedDescription("EnumEllisEqN", typeof(Resources))]
        EllisEqN,


        [LocalizedDescription("EnumRaminAndHuhtanen", typeof(Resources))]
        RaminAndHuhtanen,

        [LocalizedDescription("EnumMillsEtAl", typeof(Resources))]
        MillsEtAl,

        [LocalizedDescription("EnumNuiEtAl", typeof(Resources))]
        NuiEtAl,

        //[LocalizedDescription("EnumEscobarBahamondesEtAlHighForage", typeof(Resources))]
        //EscobarBahamondesEtAlHighForage,

        //[LocalizedDescription("EnumEllisEtAlEquationG", typeof(Resources))]
        //EllisEtAlEquationG,

        //[LocalizedDescription("EnumEllisEtAlEquationN", typeof(Resources))]
        //EllisEtAlEquationN,

        //[LocalizedDescription("EnumMoraesEtAlDietaryLevel1", typeof(Resources))]
        //MoraesEtAlDietaryLevel1,

        //[LocalizedDescription("EnumMoraesEtAlDietaryLevel2", typeof(Resources))]
        //MoraesEtAlDietaryLevel2,

        //[LocalizedDescription("EnumMoraesEtAlDietaryAnimalLevel1", typeof(Resources))]
        //MoraesEtAlDietaryAnimalLevel1,

        //[LocalizedDescription("EnumMoraesEtAlDietaryAnimalLevel2", typeof(Resources))]
        //MoraesEtAlDietaryAnimalLevel2,



        //[LocalizedDescription("EnumEscobarBahamondesEtAlMediumForage", typeof(Resources))]
        //EscobarBahamondesEtAlMediumForage,

        //[LocalizedDescription("EnumEllisEtAlEquation12B", typeof(Resources))]
        //EllisEtAlEquation12B,

        //[LocalizedDescription("EnumEscobarBahamondesEtAlLowForage", typeof(Resources))]
        //EscobarBahamondesEtAlLowForage,

        //[LocalizedDescription("EnumEllisEtAlEquation10B", typeof(Resources))]
        //EllisEtAlEquation10B,

        //[LocalizedDescription("EnumEllisEtAlEquation9B", typeof(Resources))]
        //EllisEtAlEquation9B,        
    }
}