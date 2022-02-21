using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public interface IEmissionData
    {
        AnimalType AnimalType { get; set; }

        ManureStateType HandlingSystem { get; set; }

        /// <summary>
        /// Methane conversion factor of manure
        ///
        /// (kg kg^-1)
        /// </summary>
        double MethaneConversionFactor { get; set; }

        /// <summary>
        /// Ef_direct
        ///
        /// [kg N2O-N (kg N)^-1]
        /// </summary>
        double N20DirectEmissionFactor { get; set; }

        /// <summary>
        /// Frac_volatilization
        /// </summary>
        double VolatilizationFraction { get; set; }

        /// <summary>
        /// EF_volatilization
        ///
        /// [kg N2O-N (kg N)^-1]
        /// </summary>
        double EmissionFactorVolatilization { get; set; }

        /// <summary>
        /// Frac_leach
        ///
        /// (unitless)
        /// </summary>
        double LeachingFraction { get; set; }

        /// <summary>
        /// EF_leach
        ///
        /// [kg N2O-N (kg N)^-1] 
        /// </summary>
        double EmissionFactorLeach { get; set; }

        /// <summary>
        /// CH4 Enteric Rate
        /// </summary>
        double MethaneEntericRate { get; set; }

        /// <summary>
        /// CH4 Manure Rate
        /// </summary>
        double MethaneManureRate { get; set; }

        /// <summary>
        /// N Excretion Rate
        /// </summary>
        double NitrogenExcretionRate { get; set; }
    }
}