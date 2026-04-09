using System;
using H.Core.Enumerations;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Collections;

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

        [TestMethod]
        public void GetFractionOfLandOccupiedByPortionsOfLandscape_CacheContainsKey_AndValueStableAcrossCalls()
        {
            // Access internal cache via reflection
            var cacheField = typeof(EcodistrictDefaultsProvider).GetField("_cacheByEcodistrictAndProvince", BindingFlags.Instance | BindingFlags.NonPublic);
            var cache = (IDictionary)cacheField.GetValue(_provider);
            Assert.IsNotNull(cache);
            Assert.IsTrue(cache.Count >0, "Expected provider cache to be pre-populated at construction.");

            var key = Tuple.Create(401, Province.Ontario);
            Assert.IsTrue(cache.Contains(key), "Expected cache to contain mapping for (401, Ontario).");

            // Repeated calls should return the same value and not mutate cache
            var beforeCount = cache.Count;
            var v1 = _provider.GetFractionOfLandOccupiedByPortionsOfLandscape(401, Province.Ontario);
            var v2 = _provider.GetFractionOfLandOccupiedByPortionsOfLandscape(401, Province.Ontario);

            Assert.AreEqual(v1, v2,0.0, "Expected stable cached value across repeated calls.");
            Assert.AreEqual(beforeCount, cache.Count, "Expected cache size to remain unchanged after repeated lookups.");
        }

        [TestMethod]
        public void GetFractionOfLandOccupiedByPortionsOfLandscape_UnknownKey_ReturnsDefault_AndDoesNotChangeCache()
        {
            var cacheField = typeof(EcodistrictDefaultsProvider).GetField("_cacheByEcodistrictAndProvince", BindingFlags.Instance | BindingFlags.NonPublic);
            var cache = (IDictionary)cacheField.GetValue(_provider);
            var beforeCount = cache.Count;

            var unknownId =999999;
            var value = _provider.GetFractionOfLandOccupiedByPortionsOfLandscape(unknownId, Province.Ontario);

            Assert.AreEqual(0.0, value,0.0, "Expected default value for unknown ecodistrict/province combination.");
            Assert.AreEqual(beforeCount, cache.Count, "Unknown key lookup should not mutate provider cache.");
        }

        [TestMethod]
        public void GetEcozone_CacheContainsKey_AndValueStableAcrossCalls()
        {
            // Access internal ecodistrict cache
            var cacheField = typeof(EcodistrictDefaultsProvider).GetField("_cacheByEcodistrict", BindingFlags.Instance | BindingFlags.NonPublic);
            var cache = (IDictionary)cacheField.GetValue(_provider);
            Assert.IsNotNull(cache);
            Assert.IsTrue(cache.Count >0, "Expected ecodistrict cache to be pre-populated at construction.");

            var key =497; // used in existing test
            Assert.IsTrue(cache.Contains(key), "Expected ecodistrict cache to contain key497.");

            var beforeCount = cache.Count;
            var z1 = _provider.GetEcozone(key);
            var z2 = _provider.GetEcozone(key);

            Assert.AreEqual(z1, z2, "Expected stable cached Ecozone across repeated calls.");
            Assert.AreEqual(beforeCount, cache.Count, "Expected ecodistrict cache size to remain unchanged after repeated lookups.");
        }

        [TestMethod]
        public void GetEcozone_UnknownKey_ReturnsDefault_AndDoesNotChangeCache()
        {
            var cacheField = typeof(EcodistrictDefaultsProvider).GetField("_cacheByEcodistrict", BindingFlags.Instance | BindingFlags.NonPublic);
            var cache = (IDictionary)cacheField.GetValue(_provider);
            var beforeCount = cache.Count;

            var unknownId =999999;
            var result = _provider.GetEcozone(unknownId);

            Assert.AreEqual(Ecozone.AtlanticMaritimes, result, "Expected default ecozone for unknown ecodistrict.");
            Assert.AreEqual(beforeCount, cache.Count, "Unknown ecozone lookup should not mutate ecodistrict cache.");
        }

        #endregion
    }
}
