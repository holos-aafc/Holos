using H.Core.Enumerations;
using H.Core.Providers.Shelterbelt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Shelterbelt
{
    [TestClass]
    public class ShelterbeltEcodistrictToClusterLookupProviderTest
    {
        

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void GetClusterData()
        {
            var result = ShelterbeltEcodistrictToClusterLookupProvider.GetClusterData(647);
            Assert.AreEqual(result.EcodistrictId, 647);
            Assert.AreEqual(result.ClusterId, "GRAY_2");
            Assert.AreEqual(result.SoilZone, "GRAY");
        }
    }
}
