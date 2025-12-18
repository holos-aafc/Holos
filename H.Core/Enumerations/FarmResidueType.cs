using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum FarmResidueType
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("EnumBarleyStraw", typeof(Resources))]
        BarleyStraw,

        [LocalizedDescription("EnumCornSilage", typeof(Resources))]
        CornSilage,

        [LocalizedDescription("EnumCornStover", typeof(Resources))]
        CornStover,

        [LocalizedDescription("EnumGrassClippings", typeof(Resources))]
        GrassClippings,

        [LocalizedDescription("EnumGrassSilage", typeof(Resources))]
        GrassSilage,

        [LocalizedDescription("EnumMaizeGrain", typeof(Resources))]
        MaizeGrain,

        [LocalizedDescription("EnumMaizeSilage", typeof(Resources))]
        MaizeSilage,

        [LocalizedDescription("EnumOatStraw", typeof(Resources))]
        OatStraw,

        [LocalizedDescription("EnumOatSilage", typeof(Resources))]
        OatSilage,

        [LocalizedDescription("EnumPaddyStraw", typeof(Resources))]
        PaddyStraw,

        [LocalizedDescription("EnumRyeStraw", typeof(Resources))]
        RyeStraw,

        [LocalizedDescription("EnumStrawPellets", typeof(Resources))]
        StrawPellets,

        [LocalizedDescription("EnumStrawSample", typeof(Resources))]
        StrawSample,

        [LocalizedDescription("EnumSunflowerResidues", typeof(Resources))]
        SunflowerResidues,

        [LocalizedDescription("EnumTriticaleStraw", typeof(Resources))]
        TriticaleStraw,

        [LocalizedDescription("EnumWheatStraw", typeof(Resources))]
        WheatStraw,

        [LocalizedDescription("EnumSweageSludge", typeof(Resources))]
        SweageSludge,

        [LocalizedDescription("EnumFoodWaste", typeof(Resources))]
        FoodWaste,

        [LocalizedDescription("EnumVegetableOil", typeof(Resources))]
        VegetableOil,
    }
}
