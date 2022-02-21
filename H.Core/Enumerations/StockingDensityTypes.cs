using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum StockingDensityTypes
    {
        [LocalizedDescription("EnumStockingDensityNotSelected", typeof(Resources))]
        StockingDensityNotSelected,

        [LocalizedDescription("EnumLight", typeof(Resources))]
        Light,

        [LocalizedDescription("EnumMedium", typeof(Resources))]
        Medium,

        [LocalizedDescription("EnumHeavy", typeof(Resources))]
        Heavy,
    }
}