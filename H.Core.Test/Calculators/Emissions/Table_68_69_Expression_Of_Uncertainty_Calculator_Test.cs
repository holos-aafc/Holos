using System;
using System.Collections.Generic;
using H.Core.Emissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Calculators.Emissions
{
    /// <summary>
    /// Summary description for Table_68_69_Expression_Of_Uncertainty_Calculator_Test
    /// </summary>
    [TestClass]
    public class Table_68_69_Expression_Of_Uncertainty_Calculator_Test
    {
        private Table_68_69_Expression_Of_Uncertainty_Calculator calc;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void initializeTests()
        {
            calc = new Table_68_69_Expression_Of_Uncertainty_Calculator();
        }

        /// <summary>
        ///  Equation 7.2.1-1
        /// </summary>
        [TestMethod]
        public void CalculateUncertaintyAssociatedWithNetFarmEmissionEstimateReturnsCorrectValue()
        {
            double[] emissions = {1.0, 2.5, 3.625};
            double[] uncertainty = {0.25, 1.125, 0.75};
            var argument = new List<Tuple<double, double>>();
            for (var i = 0; i < 3; ++i)
            {
                argument.Add(new Tuple<double, double>(emissions[i], uncertainty[i]));
            }

            var result = calc.CalculateUncertaintyAssociatedWithNetFarmEmissionEstimate(argument);
            Assert.AreEqual(0.86804157076857535, result);
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