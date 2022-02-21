using System;
using H.Core.Enumerations;
using H.Core.Providers.Polygon;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Soil
{
    [TestClass]
    public class EcodistrictToEcozoneProviderTest
    {
        #region Fields

        private static EcodistrictDefaultsProvider _provider; 

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new EcodistrictDefaultsProvider();
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
        public void GetEcozone()
        {
            var result = _provider.GetEcozone(497);

            Assert.AreEqual(Ecozone.AtlanticMaritimes, result);
        }

        [TestMethod]
        public void GetFractionOfLandOccupiedByPortionsOfLandscape()
        {
            var result = _provider.GetFractionOfLandOccupiedByPortionsOfLandscape(401, Province.Ontario);

            Assert.AreEqual(0.008, result);
        }

        #endregion
    }
}
