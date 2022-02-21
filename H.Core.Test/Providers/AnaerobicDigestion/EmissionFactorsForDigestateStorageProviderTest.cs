using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Providers.AnaerobicDigestion;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.AnaerobicDigestion
{
    [TestClass]
    public class EmissionFactorsForDigestateStorageProviderTest
    {
        #region Fields
        private static EmissionFactorsForDigestateStorageProvider _provider;
        #endregion

        #region Initialization
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) { }

        [ClassCleanup]
        public static void ClassCleanup() { }

        [TestInitialize]
        public void TestIntialize()
        {
            _provider = new EmissionFactorsForDigestateStorageProvider();
        }

        [TestCleanup]
        public void TestCleanup() { }
        #endregion

        #region Tests

        [TestMethod]
        public void GetEmissionFactorMethaneRaw()
        {
            var data = _provider.GetEmissionFactorInstance(EmissionTypes.CH4, DigestateState.Raw);
            Assert.AreEqual(0.59, data.EmissionFactor);
        }

        [TestMethod]
        public void GetEmissionFactorAmmoniaLiquid()
        {
            var data = _provider.GetEmissionFactorInstance(EmissionTypes.NH3, DigestateState.LiquidPhase);
            Assert.AreEqual(0.44, data.EmissionFactor);
        }

        [TestMethod]
        public void GetEmissionFactorNitrousOxideSolid()
        {
            var data = _provider.GetEmissionFactorInstance(EmissionTypes.N2O, DigestateState.SolidPhase);
            Assert.AreEqual(0.16, data.EmissionFactor);
        }

        [TestMethod]
        public void TestIncorrectEmissionType()
        {
            var data = _provider.GetEmissionFactorInstance(EmissionTypes.EntericMethane, DigestateState.Raw);
            Assert.AreEqual(0, data.EmissionFactor);
        }

        #endregion
    }
}
