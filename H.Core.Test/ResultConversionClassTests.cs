using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.Core.Enumerations;

namespace H.Core.Test
{
    [TestClass()]
    public class ResultConversionClassTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod()]
        public void ConvertTest()
        {
            const double energyCO2 = 14;

            var metricResult = energyCO2.Convert(
                MeasurementSystemType.Metric, MetricUnitsOfMeasurement.Kilograms,
                EmissionDisplayUnits.KilogramsC02e);
            Assert.AreEqual(14, metricResult);

            //should be the same result as above since we want to see in kg CO2e
            var imperialResult = energyCO2.Convert(
                MeasurementSystemType.Imperial, MetricUnitsOfMeasurement.Kilograms,
                EmissionDisplayUnits.KilogramsC02e);
            Assert.AreEqual(14, imperialResult);

            //should be different b/c now we are interested in lb GHGs
            //14 kg * 2.205lb/kg
            var imperialPoundsResult = energyCO2.Convert(
                MeasurementSystemType.Imperial, MetricUnitsOfMeasurement.Kilograms,
                EmissionDisplayUnits.PoundsGhgs);
            var expected = Math.Round(14 * 2.205, 2);
            Assert.AreEqual(expected, Math.Round(imperialPoundsResult, 2));

            var originalTemperature = 23.2;

            var celsius = originalTemperature.Convert(MeasurementSystemType.Metric, MetricUnitsOfMeasurement.DegreesCelsius,
                EmissionDisplayUnits.KilogramsC02e);
            Assert.AreEqual(originalTemperature, celsius);

            var fahrenheit = originalTemperature.Convert(MeasurementSystemType.Imperial,
                MetricUnitsOfMeasurement.DegreesCelsius, EmissionDisplayUnits.KilogramsC02e);
            var fahrenheitExpected = 73.76;
            Assert.AreEqual(fahrenheitExpected, Math.Round(fahrenheit, 2));
        }
    }
}