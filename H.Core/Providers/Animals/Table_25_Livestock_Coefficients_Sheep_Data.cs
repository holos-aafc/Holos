using H.Core.CustomAttributes;
using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public class Table_25_Livestock_Coefficients_Sheep_Data : AnimalCoefficientData
    {
        #region Properties

        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogram)]
        public double CoefficientA { get; set; }

        [Units(MetricUnitsOfMeasurement.MegaJoulesPerKilogramSquared)]
        public double CoefficientB { get; set; }

        [Units(MetricUnitsOfMeasurement.KilogramsPerYear)]
        public double WoolProduction { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(CoefficientA)}: {CoefficientA}, {nameof(CoefficientB)}: {CoefficientB}, {nameof(WoolProduction)}: {WoolProduction}";
        }

        #endregion
    }
}