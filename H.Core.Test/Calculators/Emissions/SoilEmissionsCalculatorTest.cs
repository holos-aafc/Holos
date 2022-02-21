using H.Core.Emissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Calculators.Emissions
{
    /// <summary>
    /// Summary description for SoilEmissionsCalculatorTest
    /// </summary>
    [TestClass]
    public class SoilEmissionsCalculatorTest
    {
        private SoilEmissionsCalculator calc;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            calc = new SoilEmissionsCalculator();
        }

        /// <summary>
        /// Equation 2.1.1-1
        /// Equation 2.1.2-1
        /// Equation 2.1.3-1
        /// Equation 2.1.3-5
        /// Equation 2.1.4-1
        /// Equation 2.1.4-5
        /// For: Tillage, Fallow, Current Perennial, Past Perennial, Seeded Grassland, Broken Grassland
        /// </summary>
        [TestMethod]
        public void CalculateTimeSinceManagementChangeInYearsReturnsCorrectValue()
        {
            double currentYear = 1990;
            double yearOfTillageChange = 1898;
            var result = calc.CalculateTimeSinceManagementChangeInYears(currentYear, yearOfTillageChange);
            Assert.AreEqual(92, result);
        }

        /// <summary>
        /// Equation 2.1.1-2
        /// Equation 2.1.2-2
        /// Equation 2.1.3-2
        /// Equation 2.1.3-6
        /// Equation 2.1.4-2
        /// Equation 2.1.4-6   
        /// For: Tillage, Fallow, Current Perennial, Past Perennial, Seeded Grassland, Broken Grassland
        [TestMethod]
        public void CalculateCarbonChangeRateReturnsCorrectValue()
        {
            var maximumCarbonProducedByManagementChange = 221.225;
            var rateConstant = 1.75;
            var timeSinceManagementChangeInYears = 3.65;
            var result = calc.CalculateCarbonChangeRate(maximumCarbonProducedByManagementChange, rateConstant,
                                                        timeSinceManagementChangeInYears);
            Assert.AreEqual(1.7696705803356212, result);
        }

        /// <summary>
        /// Equation 2.1.1-3
        /// Equation 2.1.2-3
        /// Equation 2.1.3-3
        /// Equation 2.1.3-7
        /// Equation 2.1.4-3
        /// Equation 2.1.4-7
        /// For: Tillage, Fallow, Current Perennial, Past Perennial, Seeded Grassland, Broken Grassland
        /// </summary>
        [TestMethod]
        public void CalculateCarbonChangeReturnsCorrectValue()
        {
            var carbonChangeRate = 0.125;
            var areaOfManagementChange = 2004.675;
            var result = calc.CalculateCarbonChange(carbonChangeRate, areaOfManagementChange);
            Assert.AreEqual(2505.84375, result);
        }

        /// <summary>
        /// Equation 2.1.1-4
        /// Equation 2.1.2-4
        /// Equation 2.1.3-4
        /// Equation 2.1.3-8
        /// Equation 2.1.4-4
        /// Equation 2.1.4-8
        /// For: Tillage, Fallow, Current Perennial, Past Perennial, Seeded Grassland, Broken Grassland
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideChangeReturnsCorrectValue()
        {
            var carbonChange = 3.9325;
            var result = calc.CalculateCarbonDioxideChange(carbonChange);
            Assert.AreEqual(-14.419166666666666666666666666667, result);
        }

        /// <summary>
        /// Equation 2.1.5-1
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonChangeForSoilsReturnsCorrectValue()
        {
            var carbonChangeForTillage = 1.125;
            var carbonChangeForFallow = 10.0;
            var carbonChangeForPerennialSeeded = 0.5;
            var carbonChangeForPerennialPast = 12.25;
            var carbonChangeForGrasslandSeeded = 90.675;
            var carbonChangeForGrasslandBroken = 23.125;
            var result = calc.CalculateTotalCarbonChangeForSoils(carbonChangeForTillage, carbonChangeForFallow,
                                                                 carbonChangeForPerennialSeeded, carbonChangeForPerennialPast, carbonChangeForGrasslandSeeded,
                                                                 carbonChangeForGrasslandBroken);
            Assert.AreEqual(-137.675, result);
        }

        /// <summary>
        /// Equation 2.1.5-2
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideChangeForSoilsReturnsCorrectValue()
        {
            var CarbonDioxideChangeForTillage = 46.125;
            var CarbonDioxideChangeForFallow = 42.1875;
            var CarbonDioxideChangeForPerennial = 1432.4375;
            var CarbonDioxideChangeForGrassland = 28.25;
            var result = calc.CalculateTotalCarbonDioxideChangeForSoils(CarbonDioxideChangeForTillage,
                                                                        CarbonDioxideChangeForFallow, CarbonDioxideChangeForPerennial, CarbonDioxideChangeForGrassland);
            Assert.AreEqual(1549, result);
        }
        /// <summary>
        /// Equation 2.1.6-1
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideChangeForSoilsPerMonthReturnsCorrectValue()
        {
            var carbonChangeForSoilsPerYear =  39.555;
            var result = calc.CalculateCarbonDioxideChangeForSoilsPerMonth(carbonChangeForSoilsPerYear);
            Assert.AreEqual(3.2958, result, 4);
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