using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Providers.AnaerobicDigestion;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.AnaerobicDigestion
{
    [TestClass]
    public class Table_48_Parameter_Adjustments_For_Manure_Provider_Test
    {
        #region Fields

        private Table_48_Parameter_Adjustments_For_Manure_Provider _provider;

        #endregion

        #region Initialization
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) { }

        [ClassCleanup]
        public static void ClassCleanup() { }

        [TestInitialize]
        public void TestIntialize()
        {
            _provider = new Table_48_Parameter_Adjustments_For_Manure_Provider();
        }

        [TestCleanup]
        public void TestCleanup() { }
        #endregion

        #region Tests

        [TestMethod]
        public void GetManureStateDeepBeddingVSReductionFactor()
        {
            var data = _provider.GetParametersAdjustmentInstance(ManureStateType.DeepBedding);

            Assert.AreEqual(0.65, data.VolatileSolidsReductionFactor);
        }

        [TestMethod]
        public void GetManureStateDeepBeddingHydrolysisRate()
        {
            var data = _provider.GetParametersAdjustmentInstance(ManureStateType.DeepBedding);

            Assert.AreEqual(0.06, data.HydrolysisRateOfSubstrate);
        }

        [TestMethod]
        public void GetManureStateStockpilingVSReductionFactor()
        {
            var data = _provider.GetParametersAdjustmentInstance(ManureStateType.SolidStorage);

            Assert.AreEqual(0.9, data.VolatileSolidsReductionFactor);
        }

        [TestMethod]
        public void GetManureStateStockpilingHydrolysisRate()
        {
            var data = _provider.GetParametersAdjustmentInstance(ManureStateType.SolidStorage);

            Assert.AreEqual(0.05, data.HydrolysisRateOfSubstrate);
        }

        [TestMethod]
        public void CheckIncorrectManureStateType()
        {
            var data = _provider.GetParametersAdjustmentInstance(ManureStateType.Liquid);

            Assert.AreEqual(0, data.HydrolysisRateOfSubstrate);
        }

        #endregion
    }
}
