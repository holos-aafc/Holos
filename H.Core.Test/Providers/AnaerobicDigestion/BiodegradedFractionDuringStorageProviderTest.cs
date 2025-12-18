using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;

namespace H.Core.Test.Providers.AnaerobicDigestion
{
    [TestClass]
    public class BiodegradedFractionDuringStorageProviderTest
    {
        #region Fields

        BiodegradedFractionDuringStorageProvider _provider;

        #endregion

        #region Initialization
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) { }

        [ClassCleanup]
        public static void ClassCleanup() { }

        [TestInitialize]
        public void TestIntialize()
        {
            _provider = new BiodegradedFractionDuringStorageProvider();
        }

        [TestCleanup]
        public void TestCleanup() { }
        #endregion

        #region Tests

        [TestMethod]
        public void GetRawDigestateVolatileSolidsValues()
        {
            var data = _provider.GetBiodegradedFractionData(DigestateParameters.VolatileSolids, DigestateState.Raw);
            Assert.AreEqual(0.07, data.Cover);
            Assert.AreEqual(0.07, data.NoCover);
        }

        [TestMethod]
        public void GetLiquidPhaseTotalCarbonValues()
        {
            var data = _provider.GetBiodegradedFractionData(DigestateParameters.TotalCarbon, DigestateState.LiquidPhase);
            Assert.AreEqual(0.15, data.Cover);
            Assert.AreEqual(0.15, data.NoCover);

        }

        [TestMethod]
        public void GetSolidPhaseTotalOrganicNitrogenValues()
        {
            var data = _provider.GetBiodegradedFractionData(DigestateParameters.OrganicNitrogen, DigestateState.SolidPhase);
            Assert.AreEqual(0.09, data.Cover);
            Assert.AreEqual(0.09, data.NoCover);

        }

        [TestMethod]
        public void TestIncorrectDigestateParameter()
        {
            var data = _provider.GetBiodegradedFractionData(DigestateParameters.RawMaterial, DigestateState.SolidPhase);
            Assert.AreEqual(0, data.Cover);
            Assert.AreEqual(0, data.NoCover);
        }
        #endregion
    }
}
