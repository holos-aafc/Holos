using H.Core.Calculators.Nitrogen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.Core.Calculators.Carbon;

namespace H.Core.Test.Calculators.Nitrogen
{
    [TestClass]
    public class Tier2NitrousOxideCalculatorTest : UnitTestBase
    {
        #region Fields

        private IPCCTier2SoilCarbonCalculator _sut;

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
            
            var ipcc = new IPCCTier2SoilCarbonCalculator(_climateProvider, n2oEmissionFactorCalculator);

            
            _sut = ipcc;
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void AdjustPoolsAfterCropDemandCalculationWhenMineralPoolGreaterThenMicrobialPool()
        {
            _sut.AvailabilityOfMineralN = 20;
            _sut.MicrobePool = 10;
            _sut.CropNitrogenDemand = 5;
            _sut.PoolRatio = 0.5;

            var expectedMineralPool = _sut.AvailabilityOfMineralN - (_sut.CropNitrogenDemand * (1 - _sut.PoolRatio));
            var expectedMicrobialPool = _sut.MicrobePool - (_sut.CropNitrogenDemand * _sut.PoolRatio);

            _sut.AdjustPoolsAfterDemandCalculation(_sut.CropNitrogenDemand);

            Assert.AreEqual(expectedMineralPool, _sut.AvailabilityOfMineralN);
            Assert.AreEqual(expectedMicrobialPool, _sut.MicrobePool);
        }

        [TestMethod]
        public void AdjustPoolsAfterCropDemandCalculationWhenMineralPoolIsZero()
        {
            _sut.AvailabilityOfMineralN = 0;
            _sut.MicrobePool = 10;
            _sut.CropNitrogenDemand = 5;
            _sut.PoolRatio = 0.5;

            var expectedMineralPool = _sut.AvailabilityOfMineralN - (_sut.CropNitrogenDemand * _sut.PoolRatio);
            var expectedMicrobialPool = _sut.MicrobePool - (_sut.CropNitrogenDemand * (1 - _sut.PoolRatio));

            _sut.AdjustPoolsAfterDemandCalculation(_sut.CropNitrogenDemand);

            Assert.AreEqual(expectedMineralPool, _sut.AvailabilityOfMineralN);
            Assert.AreEqual(expectedMicrobialPool, _sut.MicrobePool);
        }

        [TestMethod]
        public void AdjustPoolsAfterCropDemandCalculationWhenMicrobialPoolIsGreaterThanMineralPool()
        {
            _sut.AvailabilityOfMineralN = 20;
            _sut.MicrobePool = 100;
            _sut.CropNitrogenDemand = 5;
            _sut.PoolRatio = 0.5;

            var expectedMineralPool = _sut.AvailabilityOfMineralN - (_sut.CropNitrogenDemand * _sut.PoolRatio);
            var expectedMicrobialPool = _sut.MicrobePool - (_sut.CropNitrogenDemand * (1 - _sut.PoolRatio));

            _sut.AdjustPoolsAfterDemandCalculation(_sut.CropNitrogenDemand);

            Assert.AreEqual(expectedMineralPool, _sut.AvailabilityOfMineralN);
            Assert.AreEqual(expectedMicrobialPool, _sut.MicrobePool);
        }

        #endregion
    }
}
