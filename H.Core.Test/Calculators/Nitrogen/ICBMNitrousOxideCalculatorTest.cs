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
        /// Equation 2.6.6-9
        /// </summary>
        [TestMethod]
        public void CalculateCropNitrogenDemand()
        {
            var result = _calculator.CalculateCropNitrogenDemand(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0.45);
            Assert.AreEqual(-711.111111111111, result, 4);
        }

        #endregion
    }
}