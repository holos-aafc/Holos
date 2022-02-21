using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    public enum ProductionStages
    {
        /// <summary>
        /// Animals that are pregnant.
        /// </summary>
        [LocalizedDescription("EnumGestating", typeof(Resources))]
        Gestating,

        /// <summary>
        /// Animals that are lactating. Also known as farrowing in swine systems.
        /// </summary>
        [LocalizedDescription("EnumLactating", typeof(Resources))]
        Lactating,

        /// <summary>
        /// Animals that are neither lactating or pregnant.
        /// </summary>
        [LocalizedDescription("EnumOpen", typeof(Resources))]
        Open,

        /// <summary>
        /// Animals that have not been weaned yet.
        /// </summary>
        [LocalizedDescription("EnumWeaning", typeof(Resources))]
        Weaning,

        /// <summary>
        /// Animals that have not been weaned yet.
        /// </summary>
        [LocalizedDescription("EnumGrowingAndFinishing", typeof(Resources))]
        GrowingAndFinishing,

        /// <summary>
        /// Animals that are used for breeding (boars, bulls, etc.)
        /// </summary>
        [LocalizedDescription("EnumBreedingStock", typeof(Resources))]
        BreedingStock,

        /// <summary>
        /// Animals that have been weaned and are no longer milk fed.
        /// </summary>
        [LocalizedDescription("EnumWeaned", typeof(Resources))]
        Weaned,
    }
}