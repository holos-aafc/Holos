using H.Core.Emissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace H.Core.Test.Calculators.Emissions
{
    [TestClass]
    public class LiquidManureMethaneEquationsCalculatorTest
    {
        #region Fields

        private LiquidManureMethaneEquationsCalculator calc;

        public TestContext TestContext { get; set; }

        #endregion

        #region Initialization

        [TestInitialize]
        public void testInitialize()
        {
            calc = new LiquidManureMethaneEquationsCalculator();
        }

        #endregion

        #region Tests

        /// <summary>
        /// Equation 9.1-1
        /// Converts from Celsius to Kelvin, Kelvin is used in the other equations
        /// </summary>
        [TestMethod]
        public void CalculateAirTemperature()
        {
            var result = calc.CalculateAirTemperature(23.1);
            Assert.AreEqual(296.25, result);
        }

        /// <summary>
        /// Equation 9.1-2
        /// </summary>
        [TestMethod]
        public void CalculateClimateFactor()
        {
            var result = calc.CalculateClimateFactor(.12, 310);
            Assert.AreEqual(1.0000043954854106367671754232903, result);
        }

        /// <summary>
        /// Equation 9.1-3
        /// </summary>
        [TestMethod]
        public void CalculateTotalVolatileSolidsProducedByAllAnimalsForTheMonth()
        {
            var result = calc.CalculateTotalVolatileSolidsProducedByAllAnimalsForTheMonth(33.43, 231.3, 30);
            Assert.AreEqual(231970.77, result, .01);
        }

        /// <summary>
        /// Equation 9.1-4
        /// </summary>
        [TestMethod]
        public void CalculateMonthlyVolatileSolidsLoadedIntoSystem()
        {
            var result = calc.CalculateMonthlyVolatileSolidsLoadedIntoSystem(0.678, 23);
            Assert.AreEqual(15.594, result, .001);
        }

        /// <summary>
        /// Equation 9.1-5
        /// </summary>
        [TestMethod]
        public void CalculateMonthlyVolatileSolidsAvailableForConversionToMethane()
        {
            var result = calc.CalculateMonthlyVolatileSolidsAvailableForConversionToMethane(0.432, 24.32, 3.35);
            Assert.AreEqual(21.402, result, .001);
        }


        /// <summary>
        /// Equation 9.1-6
        /// </summary>
        [TestMethod]
        public void CalculateMonthlyVolatileSolidsAvailableForConversionToMethanWhenLiquidManureIsEmptied()
        {
            var result = calc.CalculateMonthlyVolatileSolidsAvailableForConversionToMethanWhenLiquidManureIsEmptied(0.3252);
            Assert.AreEqual(0.3252, result);
        }

        /// <summary>
        /// Equation 9.1-7
        /// </summary>
        [TestMethod]
        public void CalculateMonthlyVolatileSolidsConsumed()
        {
            var result = calc.CalculateMonthlyVolatileSolidsConsumed(0.4, 31.34);
            Assert.AreEqual(12.536, result, 0.001);
        }

        /// <summary>
        /// Equation 9.1-8
        /// </summary>
        [TestMethod]
        public void CalculateMonthlyMethaneEmission()
        {
            var result = calc.CalculateMonthlyMethaneEmission(234.21, 0.453);
            Assert.AreEqual(71.0850771, result);
        }

        /// <summary>
        /// Equation 9.1-9
        /// </summary>
        [TestMethod]
        public void CalculateMonthlyMethaneEmissionForACoveredSystem()
        {
            var result = calc.CalculateMonthlyMethaneEmissionForACoveredSystem(23.241523);
            Assert.AreEqual(13.9449138, result);
        }

        /// <summary>
        /// Equation 9.1-10
        /// </summary>
        [TestMethod]
        public void CalculateYearlyManureMethaneEmission()
        {
            var result = calc.CalculateManureMethaneEmission(
                new List<double>
                {
                    12.4,
                    0.04,
                    7
                });
            Assert.AreEqual(19.44, result, .01);
        }

        /// <summary>
        /// Equation 9.1-11
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsProducedYearly()
        {
            var result = calc.CalculateVolatileSolidsProducedYearly(
                new List<double>
                {
                    .53,
                    34,
                    8.65
                });
            Assert.AreEqual(43.18, result);
        }

        /// <summary>
        /// Equation 9.1-12
        /// </summary>
        [TestMethod]
        public void CalculateMethaneConversionFactor()
        {
            var result = calc.CalculateMethaneConversionFactor(2.43, .765, 4.56);
            Assert.AreEqual(1.039693174991913497527840672797, result);
        }

        /// <summary>
        /// Equation 9.1-13
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionRate()
        {
            var result = calc.CalculateManureMethaneEmissionRate(5.82, 0.296, 9.52);
            Assert.AreEqual(10.988197248, result);
        }

        /// <summary>
        /// Equation 9.1-14
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmission()
        {
            var result = calc.CalculateManureMethaneEmission(0.456, 25, 30);
            Assert.AreEqual(342, result);
        }

        #endregion
    }
}
