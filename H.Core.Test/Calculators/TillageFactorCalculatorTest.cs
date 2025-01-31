#region Imports

using H.Core.Calculators.Tillage;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace H.Core.Test.Calculators
{
    [TestClass]
    public class TillageFactorCalculatorTest
    {
        #region Fields

        private TillageFactorCalculator _calculator;

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
            _calculator = new TillageFactorCalculator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void CalculateTillageFactorReturnsConstantForNonPrarieProvinces()
        {
            Assert.AreEqual(1,
                            _calculator.CalculateTillageFactor(Province.Ontario, SoilFunctionalCategory.Black, TillageType.NoTill,
                                                               CropType.Barley));
            Assert.AreEqual(1,
                            _calculator.CalculateTillageFactor(Province.Quebec, SoilFunctionalCategory.Black, TillageType.NoTill,
                                                               CropType.Barley));
            Assert.AreEqual(1,
                            _calculator.CalculateTillageFactor(Province.BritishColumbia, SoilFunctionalCategory.Black,
                                                               TillageType.NoTill, CropType.Barley));
            Assert.AreEqual(1,
                            _calculator.CalculateTillageFactor(Province.Quebec, SoilFunctionalCategory.Black, TillageType.NoTill,
                                                               CropType.Barley));
            Assert.AreEqual(1,
                            _calculator.CalculateTillageFactor(Province.PrinceEdwardIsland, SoilFunctionalCategory.Black,
                                                               TillageType.NoTill, CropType.Barley));
            Assert.AreEqual(1,
                            _calculator.CalculateTillageFactor(Province.Newfoundland, SoilFunctionalCategory.Black,
                                                               TillageType.NoTill, CropType.Barley));
            Assert.AreEqual(1,
                            _calculator.CalculateTillageFactor(Province.NovaScotia, SoilFunctionalCategory.Black,
                                                               TillageType.NoTill, CropType.Barley));
        }

        [TestMethod]
        public void CalculateTillageFactorReturnsConstantForFirstYearOfPerennial()
        {
            Assert.AreEqual(0.6,
                            _calculator.CalculateTillageFactor(Province.Manitoba, SoilFunctionalCategory.Black, TillageType.NoTill,
                                                               CropType.TameLegume));
        }

        [TestMethod]
        public void CalculateTillageFactorReturnsConstantForSubsequentYearsOfPerennial()
        {
            Assert.AreEqual(0.6,
                            _calculator.CalculateTillageFactor(Province.Manitoba, SoilFunctionalCategory.Black, TillageType.NoTill,
                                                               CropType.TameLegume));
        }

        #endregion
    }
}