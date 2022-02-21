using System.Collections.Generic;
using H.Core.Emissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Calculators.Emissions
{
    /// <summary>
    /// Summary description for EnergyCarbonDioxideEmissionsCalculatorTest
    /// </summary>
    [TestClass]
    public class EnergyCarbonDioxideEmissionsCalculatorTest
    {
        private EnergyCarbonDioxideEmissionsCalculator calc;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void initializeTests()
        {
            calc = new EnergyCarbonDioxideEmissionsCalculator();
        }

        /// <summary>
        /// Equation 4.3.1-3
        /// Sum from all animal types
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromLiquidManureSpreadingReturnsCorrectValue()
        {
            var carbonDioxideEmissionsFromLiquidManureSpreading =
                new List<double> {128.125, 23.000, 92.250, 89.125, 24.500, 243.875, 22.750, 106.87};
            var result =
                calc.CalculateTotalCarbonDioxideEmissionsFromLiquidManureSpreading(
                                                                                   carbonDioxideEmissionsFromLiquidManureSpreading);
            Assert.AreEqual(730.495, result);
        }






        /// <summary>
        /// Equation 4.3.1-6
        /// Sum from all animal types
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromSolidManureSpreadingReturnsCorrectValue()
        {
            var carbonDioxideEmissionsFromSolidManureSpreading =
                new List<double> {110.250, 314.000, 40.500, 251.875, 116.000, 272.000, 73.87};
            var result =
                calc.CalculateTotalCarbonDioxideEmissionsFromSolidManureSpreading(
                                                                                  carbonDioxideEmissionsFromSolidManureSpreading);
            Assert.AreEqual(1178.495, result);
        }


        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion
    }
}