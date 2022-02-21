using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.Climate;

namespace H.Core.Test.Providers.Climate
{
    [TestClass]

    public class GlobalWarmingEmissionsPotentialProviderTest
    {

        #region Fields
        private static GlobalWarmingEmissionsPotentialProvider _provider;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new GlobalWarmingEmissionsPotentialProvider();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetGlobalWarmingPotentialValue()
        {
            GlobalWarmingEmissionsPotentialData data = _provider.GetGlobalWarmingEmissionsInstance(1990, EmissionTypes.CH4);
            Assert.AreEqual(21, data.GlobalWarmingPotentialValue);
        }

        [TestMethod]
        public void InstanceNotFoundWrongEmission()
        {
            GlobalWarmingEmissionsPotentialData data = _provider.GetGlobalWarmingEmissionsInstance(1995, EmissionTypes.CO2e);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void InstanceNotFoundWrongYear()
        {
            GlobalWarmingEmissionsPotentialData data = _provider.GetGlobalWarmingEmissionsInstance(2035, EmissionTypes.N2O);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void DataInstanceNotFoundAllWrongInput()
        {
            GlobalWarmingEmissionsPotentialData data = _provider.GetGlobalWarmingEmissionsInstance(2014, EmissionTypes.LivestockDirectN20);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void GetDataInstance()
        {
            GlobalWarmingEmissionsPotentialData data = _provider.GetGlobalWarmingEmissionsInstance(2013, EmissionTypes.N2O);
            Assert.AreNotEqual(null, data);
        }

        #endregion

    }
}
