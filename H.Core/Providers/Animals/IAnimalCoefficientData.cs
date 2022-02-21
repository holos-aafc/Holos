using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public interface IAnimalCoefficientData
    {
        AnimalType AnimalType { get; set; }

        /// <summary>
        /// C_f
        /// </summary>
        double BaselineMaintenanceCoefficient { get; set; }

        /// <summary>
        /// C_d
        /// </summary>
        double GainCoefficient { get; set; }

        /// <summary>
        /// (kg)
        /// </summary>
        double DefaultInitialWeight { get; set; }

        /// <summary>
        /// (kg)
        /// </summary>
        double DefaultFinalWeight { get; set; }

        /// <summary>
        /// (kg)
        /// </summary>
        double DefaultDailyGain { get; set; }
    }
}