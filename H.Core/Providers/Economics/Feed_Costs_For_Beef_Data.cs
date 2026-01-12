using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Providers.Economics
{
    public class Feed_Costs_For_Beef_Data
    {
        /// <summary>
        /// Hay Quality for beef
        /// </summary>
        public DietType DietType { get; set; }

        /// <summary>
        /// $ / Kg
        /// </summary>
        [Units(MetricUnitsOfMeasurement.DollarsPerKilogram)]
        public double Cost { get; set; }
    }
}
