using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class DefaultVolatileSolidExcretionData
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