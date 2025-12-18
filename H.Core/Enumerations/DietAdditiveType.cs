using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum DietAdditiveType
    {
        [LocalizedDescription("EnumNone", typeof(Resources))]
        None,

        [LocalizedDescription("EnumTwoPercentFat", typeof(Resources))]
        TwoPercentFat,

        [LocalizedDescription("EnumFourPercentFat", typeof(Resources))]
        FourPercentFat,

        [LocalizedDescription("EnumIonophore", typeof(Resources))]
        Inonophore,

        [LocalizedDescription("EnumIonophorePlusTwoPercentFat", typeof(Resources))]
        InonophorePlusTwoPercentFat,

        [LocalizedDescription("EnumIonophorePlusFourPercentFat", typeof(Resources))]
        InonophorePlusFourPercentFat,

        [LocalizedDescription("EnumCustom", typeof(Resources))]
        Custom,

        [LocalizedDescription("EnumFivePercentFat", typeof(Resources))]
        FivePercentFat,

        [LocalizedDescription("EnumIonophorePlusFivePercentFat", typeof(Resources))]
        IonophorePlusFivePercentFat,
    }
}