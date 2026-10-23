using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Where the IPCC Tier 2 run-in period takes its yields from. The run-in period's average inputs set the starting
    /// soil carbon, so when its yields differ from the ones the simulation uses, soil carbon starts at one level and
    /// moves towards another from the first year.
    /// </summary>
    public enum RunInPeriodYieldSource
    {
        /// <summary>
        /// The run-in period repeats the first years of the field history, as many as it is long. Each run-in year
        /// takes the yield of the history year it repeats wherever that yield came from the user - a hay harvest,
        /// grazing, a typed (Custom) yield or an override. Other years take the yield assignment method's yield for the
        /// run-in year.
        /// </summary>
        [LocalizedDescription("EnumRunInPeriodYieldSourceFieldHistory", typeof(Resources))]
        FieldHistory,

        /// <summary>
        /// The run-in period continues the field's first rotation backwards from the start year, and every run-in year
        /// takes the yield assignment method's yield for that year (Small Area Data, for example), whatever the field
        /// history records - typical regional productivity before the user's records begin.
        /// </summary>
        [LocalizedDescription("EnumRunInPeriodYieldSourceYieldAssignmentMethod", typeof(Resources))]
        YieldAssignmentMethod,
    }
}
