using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum Months
    {
        [LocalizedDescription("EnumJanuary", typeof(Resources))]
        January = 1,

        [LocalizedDescription("EnumFebruary", typeof(Resources))]
        February,

        [LocalizedDescription("EnumMarch", typeof(Resources))]
        March,

        [LocalizedDescription("EnumApril", typeof(Resources))]
        April,

        [LocalizedDescription("EnumMay", typeof(Resources))]
        May,

        [LocalizedDescription("EnumJune", typeof(Resources))]
        June,

        [LocalizedDescription("EnumJuly", typeof(Resources))]
        July,

        [LocalizedDescription("EnumAugust", typeof(Resources))]
        August,

        [LocalizedDescription("EnumSeptember", typeof(Resources))]
        September,

        [LocalizedDescription("EnumOctober", typeof(Resources))]
        October,

        [LocalizedDescription("EnumNovember", typeof(Resources))]
        November,

        [LocalizedDescription("EnumDecember", typeof(Resources))]
        December
    }
}