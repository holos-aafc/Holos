using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum EquilibriumCalculationStrategies
    {
        /// <summary>
        /// Used to indicate that the user defined yield for equilibrium year should be used
        /// </summary>
        [LocalizedDescription("EnumDefault", typeof(Resources))]
        Default,

        /// <summary>
        /// Used to indicate that the CAR region yield for the equilibrium year should be used
        /// </summary>
        [LocalizedDescription("EnumCarSingleYear", typeof(Resources))]
        CarSingleYear,

        /// <summary>
        /// Used to indicate that the average of N years of the CAR region yields (centered around the equilibrium year) should be used
        /// </summary>
        [LocalizedDescription("EnumCarMultipleYearAverage", typeof(Resources))]
        CarMultipleYearAverage,
    }
}