#region Imports

using H.Core.CustomAttributes;
using H.Core.Enumerations;

#endregion

namespace H.Core.Providers.Animals
{
    public class AnimalCoefficientData : IAnimalCoefficientData
    {
        #region Properties

        public AnimalType AnimalType { get; set; }

        [Units(MetricUnitsOfMeasurement.MegaJoulesPerDayPerKilogram)]
        public double BaselineMaintenanceCoefficient { get; set; }

        public double GainCoefficient { get; set; }

        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double DefaultInitialWeight { get; set; }

        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double DefaultFinalWeight { get; set; }

        [Units(MetricUnitsOfMeasurement.Kilograms)]
        public double DefaultDailyGain { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{nameof(AnimalType)}: {AnimalType}, {nameof(BaselineMaintenanceCoefficient)}: {BaselineMaintenanceCoefficient}, {nameof(GainCoefficient)}: {GainCoefficient}, {nameof(DefaultInitialWeight)}: {DefaultInitialWeight}, {nameof(DefaultFinalWeight)}: {DefaultFinalWeight}";
        }

        #endregion
    }
}