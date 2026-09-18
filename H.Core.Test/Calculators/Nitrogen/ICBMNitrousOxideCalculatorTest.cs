#region Imports

using H.Core.Calculators.Nitrogen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using H.Core.Calculators.Carbon;
using H.Core.Providers.Climate;
using H.Core.Services.LandManagement;

#endregion

namespace H.Core.Test.Calculators.Nitrogen
{
    [TestClass]
    public class ICBMNitrousOxideCalculatorTest : UnitTestBase
    {
        #region Fields

        private ICBMSoilCarbonCalculator _calculator;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            
            var n2oEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            var iCBMSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, n2oEmissionFactorCalculator);
            

            
            _calculator = iCBMSoilCarbonCalculator;
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        /// <summary>
        /// Equation 2.6.4-1
        /// </summary>
        [TestMethod]
        public void CalculateAvailablityOfNitrogenInTheMicrobialPoolAtStartingPoint()
        {
            var result = _calculator.CalculateCropResiduesAtStartingPoint(2, 3, 7, 8);

            Assert.AreEqual(5, result);
        }

        /// <summary>
        /// Equation 2.6.8-12. The expected value here has always been the one the equation gives; the tolerance used
        /// to be 4, wide enough to hide that the code was subtracting the fixation term rather than multiplying by it.
        /// </summary>
        [TestMethod]
        public void CalculateCropNitrogenDemand()
        {
            var result = _calculator.CalculateCropNitrogenDemand(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0.45);
            Assert.AreEqual(-711.111111111111, result, 0.0001);
        }

        /// <summary>
        /// A crop that fixes none of its own nitrogen demands the full amount from the soil, so the fixation term
        /// leaves the summed pools alone.
        /// </summary>
        [TestMethod]
        public void CalculateCropNitrogenDemandWithNoFixationReturnsTheSummedPools()
        {
            // One pool only, so the expected value can be read off the equation: 100 / 0.45 * (1 - 0.12) * 0.02
            var expected = 100 / 0.45 * (1 - 0.12) * 0.02;

            var result = _calculator.CalculateCropNitrogenDemand(100, 0, 0, 0, 0.12, 0.02, 0, 0, 0, 0, 0.45);

            Assert.AreEqual(expected, result, 0.0001);
        }

        /// <summary>
        /// The fixation term scales the demand rather than being taken off it. A crop fixing 55% of its own nitrogen
        /// asks the soil for 45% of what it would otherwise need - not 0.45 kg N ha^-1 less.
        /// </summary>
        [TestMethod]
        public void CalculateCropNitrogenDemandScalesByTheFixationFraction()
        {
            var withoutFixation = _calculator.CalculateCropNitrogenDemand(100, 0, 0, 0, 0.12, 0.02, 0, 0, 0, 0, 0.45);
            var withFixation = _calculator.CalculateCropNitrogenDemand(100, 0, 0, 0, 0.12, 0.02, 0, 0, 0, 0.55, 0.45);

            Assert.AreEqual(withoutFixation * 0.45, withFixation, 0.0001);
        }

        /// <summary>
        /// A crop fixing all of its own nitrogen asks the soil for none. Subtracting the term instead would leave
        /// the whole demand standing.
        /// </summary>
        [TestMethod]
        public void CalculateCropNitrogenDemandWithCompleteFixationIsZero()
        {
            var result = _calculator.CalculateCropNitrogenDemand(100, 50, 25, 10, 0.12, 0.02, 0.01, 0.01, 0.01, 1.0, 0.45);

            Assert.AreEqual(0, result, 0.0001);
        }




        #endregion
    }
}