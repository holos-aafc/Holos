using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Indicates the animal category source of the manure (beef, dairy, etc.)
    /// </summary>
    public enum ManureAnimalSourceTypes
    {
        [LocalizedDescription("EnumNotSelected", typeof(Resources))]
        NotSelected,

        [LocalizedDescription("EnumBeefManure", typeof(Resources))]
        BeefManure,

        [LocalizedDescription("EnumDairyManure", typeof(Resources))]
        DairyManure,

        [LocalizedDescription("EnumSwineManure", typeof(Resources))]
        SwineManure,

        [LocalizedDescription("EnumPoultryManure", typeof(Resources))]
        PoultryManure,

        [LocalizedDescription("EnumSheepManure", typeof(Resources))]
        SheepManure,

        [LocalizedDescription("EnumOtherLivestockManure", typeof(Resources))]
        OtherLivestockManure,
    }
}