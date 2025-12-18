using H.Core.Calculators.Carbon;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace H.Core.Test.Calculators.Nitrogen
{
    [TestClass]
    public class ICBMNitrogenInputCalculatorTest : UnitTestBase
    {
        #region Fields

        private IICBMNitrogenInputCalculator _nitrogenInputCalculator;

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
            _nitrogenInputCalculator = new ICBMNitrogenInputCalculator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void CalculateFractionOfNitrogenLostByLeachingAndRunoffReturnsCorrectValue()
        {
            var growingSeasonPrecipitation = 34.2;
            var growingSeasonEvapotranspiration = 0.65;
            var result =
                _nitrogenInputCalculator.CalculateFractionOfNitrogenLostByLeachingAndRunoff(
                    growingSeasonPrecipitation,
                    growingSeasonEvapotranspiration);

            Assert.AreEqual(0.3, result);
        }

        [TestMethod]
        public void CalculateSyntheticFertilizerApplied()
        {
            var firstRate = _nitrogenInputCalculator.CalculateSyntheticFertilizerApplied(
                nitrogenContentOfGrainReturnedToSoil: 100,
                fertilizerEfficiencyFraction: 0.5,
                soilTestN: 10,
                isNitrogenFixingCrop: false,
                nitrogenFixationAmount: 0,
                atmosphericNitrogenDeposition: 0);

            Assert.AreEqual(180, firstRate);

            // Increasing efficiency should reduce the required amount of fertilizer
            var secondRate = _nitrogenInputCalculator.CalculateSyntheticFertilizerApplied(
                nitrogenContentOfGrainReturnedToSoil: 100,
                fertilizerEfficiencyFraction: 0.75,
                soilTestN: 10,
                isNitrogenFixingCrop: false,
                nitrogenFixationAmount: 0,
                atmosphericNitrogenDeposition: 0);

            Assert.IsTrue(secondRate < firstRate);
        }

        #endregion
    }
}
