using H.Core.Enumerations;

namespace H.Core.Providers.AnaerobicDigestion
{
    public class SolidLiquidSeparationCoefficientsData
    {
        /// <summary>
        /// Represents the fraction of the parameter in solid fraction following solid-liquid separation.
        /// </summary>
        public SeparationCoefficients SeparationCoefficient { get; set; }

        /// <summary>
        /// Represents a default value for separation coefficients (fraction in solid fraction) for solid-liquid separation of digestate.
        /// </summary>
        public double Centrifuge { get; set; }

        /// <summary>
        /// Represents a default value for separation coefficients (fraction in solid fraction) for solid-liquid separation of digestate.
        /// </summary>
        public double BeltPress { get; set; }
    }
}
