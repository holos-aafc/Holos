using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class Table_37_Livestock_Daily_Volatile_Excretion_Factors_Data
    {
        /// <summary>
        /// (kg VS (1,000 kg animal mass)^-1 day^-1)
        /// </summary>
        public double VolatileSolidExcretionRate { get; set; }

        /// <summary>
        /// (kg head^-1 day^-1)
        /// </summary>
        public double VolatileSolids { get; set; }
    }
}