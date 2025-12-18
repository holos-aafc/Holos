using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;

namespace H.Core
{
    public static class ResultConversionClass
    {
        private static readonly UnitsOfMeasurementCalculator _calculator;
        static ResultConversionClass()
        {
            _calculator = new UnitsOfMeasurementCalculator();
        }

        /// <summary>
        /// This method is used in results view models to make the conversion from metric to imperial
        /// </summary>
        /// <param name="self">the double we are working on</param>
        /// <param name="currentMeasurementType">the current farm units of measurement type</param>
        /// <param name="sourceType">the type of units the original value uses</param>
        /// <param name="displayUnits">User's preferred units to display</param>
        /// <returns>a double that is in the correct units of measurement</returns>
        public static double Convert(this double self, MeasurementSystemType currentMeasurementType,
            MetricUnitsOfMeasurement sourceType, EmissionDisplayUnits displayUnits)
        {
            if (currentMeasurementType == MeasurementSystemType.Imperial)
            {
                if (sourceType == MetricUnitsOfMeasurement.DegreesCelsius)
                {
                    return _calculator.ConvertValueToImperialFromMetric(sourceType, self);
                }

                if (displayUnits == EmissionDisplayUnits.PoundsGhgs)
                {
                    return _calculator.ConvertValueToImperialFromMetric(sourceType, self);
                }
            }
            return self;
        }
    }
}
