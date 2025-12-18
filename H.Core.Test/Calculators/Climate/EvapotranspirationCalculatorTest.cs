using System;
using H.Core.Calculators.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Calculators.Climate
{
    [TestClass]
    public class EvapotranspirationCalculatorTest
    {
        [TestMethod]
        public void CalculateReferenceEvapotranspirationCorrectValue()
        {
            var _sut = new EvapotranspirationCalculator();

            // Simulate getting nasa data where the solar radiation has not yet been converted to Holos units (x 3.6)
            var refEvapotransLowRH = _sut.CalculateReferenceEvapotranspiration(0.38, 0.82 * 3.6, 93);
            var refEvapotransHighRH = _sut.CalculateReferenceEvapotranspiration(5.08, 1.5 * 3.6, 61.32);

            Assert.AreEqual(0.038707, Math.Round(refEvapotransLowRH), 6);
            Assert.AreEqual(0.589, Math.Round(refEvapotransHighRH, 3));
        }
    }
}
