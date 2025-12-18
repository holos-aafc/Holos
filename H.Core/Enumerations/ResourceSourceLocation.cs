using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum ResourceSourceLocation
    {
        /// <summary>
        /// For resources that originate from the farm
        /// </summary>
        [LocalizedDescription("EnumOnFarm", typeof(Resources))]
        OnFarm,

        /// <summary>
        /// For resources that need to be imported from off the the farm - or that originate from off the farm
        /// </summary>
        [LocalizedDescription("EnumOffFarm", typeof(Resources))]
        OffFarm,
    }
}