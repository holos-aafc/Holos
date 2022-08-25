using System.Collections.Generic;
using H.Core.Calculators.Nitrogen;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Calculators.Emissions
{
    /// <summary>
    /// Summary description for EmissionsCalculatorTest
    /// </summary>
    [TestClass]
    public class NitrogenEmissionsCalculatorTest
    {
        private SingleYearNitrousOxideCalculator calc;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void testInitialize()
        {
            calc = new SingleYearNitrousOxideCalculator();
        }

        /// <summary>
        /// Equation 2.5.1-1
        /// </summary>
        [TestMethod]
        public void CalculatedEmissionFactorReturnsCorrectValue()
        {
            var result = calc.CalculateEcodistrictEmissionFactor(500.0, 600.0);
            Assert.AreEqual(0.012881024751743584, result);
        }

        /// <summary>
        /// Equation 2.5.2-1
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenInputsFromFertilizerAppliedReturnsCorrectValue()
        {          
        }    

        /// <summary>
        /// Equation 2.5.2-6
        /// </summary>
        [TestMethod]
        public void CalculateTotalResidueNitrogenReturnsCorrectValue()
        {
            var aboveGroundResidueNitrogen = 2.541;
            var belowGroundResidueNitrogen = 7.516;
            var area = 4.126;
            var result =
                calc.CalculateInputsFromResidueReturned(aboveGroundResidueNitrogen, belowGroundResidueNitrogen, area);
            Assert.AreEqual(41.495182000000007, result);
        }       

        /// <summary>
        /// Equation 2.5.2-8
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenMineralizationReturnsCorrectValue()
        {
            var result = calc.CalculateNitrogenInputsFromMineralizationOfNativeSoilOrganicMatter(1115);
            Assert.AreEqual(111.5, result);
        }

        /// <summary>
        ///    Equation 2.5.3-1
        /// </summary>
        [TestMethod]
        public void CalculateFractionOfNitrogenLostByLeachingAndRunoffReturnsCorrectValue()
        {
            var growingSeasonPrecipitation = 34.2;
            var growingSeasonEvapotranspiration = 0.65;
            var result =
                calc.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                    growingSeasonPrecipitation, 
                    growingSeasonEvapotranspiration);

            Assert.AreEqual(0.3, result);
        }
        
        /// <summary>
        /// Equation 2.5.3-3
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenEmissionsDueToLeachingAndRunoffFromMineralizedNitrogenReturnsCorrectValue()
        {
            var nitrogenInputsFromMineralizationOfNativeSoilOrganicMatter = 4.5;
            var fractionOfNitrogenLostByLeachingAndRunoff = 8.2;
            var emissionsFactorForLeachingAndRunoff = 4.1;
            var result = calc.CalculateNitrogenEmissionsDueToLeachingAndRunoffFromMineralizedNitrogen(
                                                                                                      nitrogenInputsFromMineralizationOfNativeSoilOrganicMatter, fractionOfNitrogenLostByLeachingAndRunoff,
                                                                                                      emissionsFactorForLeachingAndRunoff);
            Assert.AreEqual(151.29, result);
        }

        /// <summary>
        /// Equation 2.5.3-4
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenEmissionsDueToLeachingAndRunoffFromAllLandAppliedManureReturnsCorrectValue()
        {
            var totalNitrogenInputsFromAllLandAppliedManureNitrogen = 8.9;
            var fractionOfNitrogenLostByLeachingAndRunoff = 4.1;
            var emissionsFactorForLeachingAndRunoff = 2.2;
            var result = calc.CalculateNitrogenEmissionsDueToLeachingAndRunoffFromAllLandAppliedManure(
                                                                                                       totalNitrogenInputsFromAllLandAppliedManureNitrogen, fractionOfNitrogenLostByLeachingAndRunoff,
                                                                                                       emissionsFactorForLeachingAndRunoff);
            Assert.AreEqual(80.277999999999992, result);
        }

        /// <summary>
        ///  Equation 2.5.3-5
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenEmissionsDueToVolatizationFromCroplandReturnsCorrectValue()
        {
            var nitrogenInputsFromSyntheticFertilizer = 2.9;
            var fractionOfNitrogenLostByVolatilization = 3.9;
            var emissionFactorForVolatilization = 6.6;
            var result = calc.CalculateNitrogenEmissionsDueToVolatizationFromCropland(
                                                                                      nitrogenInputsFromSyntheticFertilizer, fractionOfNitrogenLostByVolatilization,
                                                                                      emissionFactorForVolatilization);
            Assert.AreEqual(74.645999999999987, result);
        }

        /// <summary>
        /// Equation 2.5.3-6
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenEmissionsDueToVolatilizationFromAllLandAppliedManureNitrogenReturnsCorrectValue()
        {
            var totalNitrogenInputsFromAllLandAppliedManureNitrogen = 7.2;
            var fractionOfNitrogenLostByVolatilization = 9.9;
            var emissionFactorForVolatilization = 2.5;
            var result = calc.CalculateNitrogenEmissionsDueToVolatilizationOfAllLandAppliedManure(
                                                                                                            totalNitrogenInputsFromAllLandAppliedManureNitrogen, fractionOfNitrogenLostByVolatilization,
                                                                                                            emissionFactorForVolatilization);
            Assert.AreEqual(178.2, result);
        }     
        
        /// <summary>
        /// Equation 2.5.4-4
        /// </summary>
        [TestMethod]
        public void CalculateTotalIndirectNitrogenEmissionsReturnsCorrectValue()
        {
            double nitrogenEmissionsDueToLeachingAndRunoff = 72;
            var nitrogenEmissionsDueToVolatilization = 80.75;
            var result = calc.CalculateTotalIndirectNitrogenEmissions(nitrogenEmissionsDueToLeachingAndRunoff,
                                                                      nitrogenEmissionsDueToVolatilization);
            Assert.AreEqual(152.75, result);
        }

        /// <summary>
        /// Equation 2.5.4-5
        /// </summary>
        [TestMethod]
        public void CalculateTotalNitrogenEmissionsReturnsCorrectValue()
        {
            double totalDirectNitrogenEmissions = 72;
            double totalIndirectNitrogenEmissions = 50;
            var result = calc.CalculateTotalNitrogenEmissions(totalDirectNitrogenEmissions,
                                                                      totalIndirectNitrogenEmissions);
            Assert.AreEqual(122, result);
        }        

        /// <summary>
        ///    Equation 2.5.6-1
        /// </summary>
        [TestMethod]
        public void CalculateDirectNitrousOxideEmissionsFromSoilsByMonthReturnsCorrectValue()
        {
            var directNitrousOxideEmissionsFromSoils = 4078.935;
            var percentageOfAnnualEmissionsAllocatedToMonth = 0.125;
            var result = calc.CalculateDirectNitrousOxideEmissionsFromSoilsByMonth(directNitrousOxideEmissionsFromSoils,
                                                                                   percentageOfAnnualEmissionsAllocatedToMonth);
            Assert.AreEqual(5.09866875, result);
        }

        /// <summary>
        /// Equation 2.5.6-2
        /// </summary>
        [TestMethod]
        public void CalculateIndirectNitrousOxideEmissionsFromSoilsByMonthReturnsCorrectValue()
        {
            var indirectNitrousOxideEmissionsFromSoils = 6878.5;
            var percentageOfAnnualEmissionsAllocatedToMonth = 2.625;
            var result = calc.CalculateIndirectNitrousOxideEmissionsFromSoilsByMonth(
                                                                                     indirectNitrousOxideEmissionsFromSoils, percentageOfAnnualEmissionsAllocatedToMonth);
            Assert.AreEqual(180.560625, result);
        }
        /// <summary>
        /// Equation 2.5.6-3
        /// </summary>
        [TestMethod]
        public void CalculateTotalNitrousOxideEmissionsFromSoilsByMonthReturnsCorrectValue()
        {
            var totalNitrousOxideEmissionsFromSoils = 5555.512;
            var percentageOfAnnualEmissionsAllocatedToMonth = 0.125;
            var result = calc.CalculateTotalNitrousOxideEmissionsFromSoilsByMonth(totalNitrousOxideEmissionsFromSoils,
                                                                                   percentageOfAnnualEmissionsAllocatedToMonth);
            Assert.AreEqual(6.94439, result, 5);
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