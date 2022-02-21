using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Used in combo boxes to allow user to show results in different units
    /// </summary>
    public enum EmissionDisplayUnits
    {
        [LocalizedDescription("EnumMgGhg", typeof(Resources))]
        MegagramsGhgs,

        [LocalizedDescription("EnumMgC02e", typeof(Resources))]
        MegagramsCO2e,

        [LocalizedDescription("EnumKgGhg", typeof(Resources))]
        KilogramsGhgs,

        [LocalizedDescription("EnumKgC02e", typeof(Resources))]
        KilogramsC02e,

        [LocalizedDescription("EnumKgC02", typeof(Resources))]
        KilogramsC02,

        [LocalizedDescription("EnumKgCH4", typeof(Resources))]
        KilogramsCH4,

        [LocalizedDescription("EnumKgN2O", typeof(Resources))]
        KilogramsN2O,

        [LocalizedDescription("EnumLbsGhg", typeof(Resources))]
        PoundsGhgs,

        [LocalizedDescription("EnumLbsCO2e", typeof(Resources))]
        PoundsCO2e,

        [LocalizedDescription("EnumLbsCO2", typeof(Resources))]
        PoundsCO2,

        [LocalizedDescription("EnumLbsCH4", typeof(Resources))]
        PoundsCH4,

        [LocalizedDescription("EnumLbsN2O", typeof(Resources))]
        PoundsN2O,
    }
}