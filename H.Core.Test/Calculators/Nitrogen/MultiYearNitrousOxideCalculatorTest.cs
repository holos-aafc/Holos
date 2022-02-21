#region Imports

using H.Core.Calculators.Nitrogen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

#endregion

namespace H.Core.Test.Calculators.Nitrogen
{
    [TestClass]
    public class MultiYearNitrousOxideCalculatorTest
    {
        #region Fields

        private MultiYearNitrousOxideCalculator _calculator;

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
            _calculator = new MultiYearNitrousOxideCalculator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        /// <summary>
        /// Equation 2.6.1-1
        /// </summary>
        [TestMethod]
        public void CalculateSyntheticNitrogenFromFertilizerApplication()
        {
            var result = _calculator.CalculateSyntheticNitrogenFromFertilizerApplication(2.4, 2.0);
            Assert.AreEqual(4.8, result);
        }

        /// <summary>
        /// Equation 2.6.1-2
        /// </summary>
        [TestMethod]
        public void CalculateSyntheticNitrogenFromDeposition()
        {
            var result = _calculator.CalculateSyntheticNitrogenFromDeposition(10.0024, 10.0);
            Assert.AreEqual(100.024, result);
        }

        /// <summary>
        /// Equation 2.6.2-1
        /// </summary>
        [TestMethod]
        public void CalculateAboveGroundResidueNAtEquilibrium()
        {
            var result = _calculator.CalculateAboveGroundResidueNitrogenAtEquilibrium(10, 5, 2, 7, 8, 3);

            Assert.AreEqual(5.3690802409551844E-09, result);
        }

        /// <summary>
        /// Equation 2.6.2-4
        /// </summary>
        [TestMethod]
        public void CalculateBelowGroundResidueNAtEquilibrium()
        {
            var result = _calculator.CalculateBelowGroundResidueNitrogenAtEquilibrium(10, 5, 2, 7, 8, 3);

            Assert.AreEqual(5.3690802409551844E-09, result);
        }     

        /// <summary>
        /// Equation 2.6.3-1
        /// </summary>
        [TestMethod]
        public void CalculateAvailablityOfNitrogenInTheMicrobialPoolAtStartingPoint()
        {
            var result = _calculator.CalculateCropResiduesAtStartingPoint(2, 3, 7, 8);

            Assert.AreEqual(5, result);
        }    

        /// <summary>
        /// Equation 2.6.5-3
        /// </summary>
        [TestMethod]
        public void CalculateNEmissionsFromLeachingAndRunoffFromCropLand()
        {
            var result = _calculator.CalculateIndirectNitrousOxideEmissionsFromLeachingAndRunoffFromSyntheticFertilizer(10, 2, 2);
            Assert.AreEqual(40, result);
        }

        /// <summary>
        /// Equation 2.6.5-4
        /// </summary>
        [TestMethod]
        public void CalculateLeachedNFromField()
        {
            var result = _calculator.CalculateIndirectNitrousOxideEmissionsFromLeachingAndRunoffOfCropResidues(10, 2, 2);
            Assert.AreEqual(40, result);
        }

        /// <summary>
        /// Equation 2.6.5-5
        /// </summary>
        [TestMethod]
        public void CalculateNEmissionsDueToVolatilizationFromCropLand()
        {
            var result = _calculator.CalculateNitrousOxideEmissionsDueToLeachingAndRunoffOfMineralizedNitrogen(10, 3, 3);
            Assert.AreEqual(90, result);
        }

        /// <summary>
        /// Equation 2.6.5-6
        /// </summary>
        [TestMethod]
        public void CalculateAmmoniaVolatilizedFromField()
        {
            var result = _calculator.CalculateNitrousOxideEmissionsDueToOrganicNitrogen(10, 3, 3);
            Assert.AreEqual(90, result);
        }

        /// <summary>
        /// Equation 2.6.6-5
        /// </summary>
        [TestMethod]
        public void CalculateRatioBetweenMineralAndMicrobialN()
        {
            var result = _calculator.CalculateRatioBetweenMineralAndMicrobialNitrogen(10, 5);
            Assert.AreEqual(0.5, result);
        }

        /// <summary>
        /// Equation 2.6.6-6
        /// </summary>
        [TestMethod]
        public void CalculateNRequirementForCarbonTransitionFromYoungToOldPool()
        {
            var result = _calculator.CalculateNitrogenRequirementForCarbonTransitionFromYoungToOldPool(1, 2, 8, 9, 3, 4, 5, 6, 7, 10, 11, 12, 13);                                                                    
            Assert.AreEqual(-4.9067E-48, result, 4);
        }

        /// <summary>
        /// Equation 2.6.6-9
        /// </summary>
        [TestMethod]
        public void CalculateCropNitrogenDemand()
        {
            var result = _calculator.CalculateCropNitrogenDemand(1, 2, 3, 4, 5, 6, 7, 8, 9, 0);
            Assert.AreEqual(-711.111111111111, result, 4);
        }

        /// <summary>
        /// Equation 2.6.6-12
        /// </summary>
        [TestMethod]
        public void CalculateFourthReductionInAvailabilityOfMicrobialNOnField()
        {
            var result = _calculator.CalculateAmountOfMicrobeDeath(10, 0.5);
            Assert.AreEqual(5, result);
        }

        /// <summary>
        /// Equation 2.6.6.13
        /// </summary>
        [TestMethod]
        public void CalculateIncreaseInAvailabilityOfMineralNOnField()
        {
            var result = _calculator.CalculateIncreaseInAvailabilityOfMineralNOnField(5, 10);
            Assert.AreEqual(55, result);
        }

        /// <summary>
        /// Equation 2.6.6-14
        /// </summary>
        [TestMethod]
        public void CalculateDenitrificationOfMineralN()
        {
            var result = _calculator.CalculateAmountOfDenitrification(5, 15);
            Assert.AreEqual(75, result);
        }

        /// <summary>
        /// Equation 2.6.6-15
        /// </summary>
        [TestMethod]
        public void CalculateFifthReductionInAvailabilityOfMineralNOnField()
        {
            var result = _calculator.CalculateFifthReductionInAvailabilityOfMineralNOnField(5, 15);
            Assert.AreEqual(-70, result);
        }


        /// <summary>
        /// Equation 2.6.7-1
        /// </summary>
        [TestMethod]
        public void CalculateN2OEmissionsDueToCropNInputs()
        {
            var result = _calculator.CalculateDirectNitrousOxideEmissions(5, 10, 15, 20);
            Assert.AreEqual(50, result);
        }

        /// <summary>
        /// Equation 2.6.7-2
        /// </summary>
        [TestMethod]
        public void CalculateSumOfDirectN2OEmissions()
        {
            var result = _calculator.CalculateSumOfDirectN2OEmissions(5, 10);
            Assert.AreEqual(15, result);
        }

        #endregion
    }
}