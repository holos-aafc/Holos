using H.Core.Enumerations;
using H.Core.Providers.Shelterbelt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Shelterbelt
{
    [TestClass]
    public class ShelterbeltRatioTableProviderTest
    {
        private ShelterbeltAgTRatioProvider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new ShelterbeltAgTRatioProvider();
        }

        [TestMethod]
        public void GetAboveGroundBiomassTotalTreeBiomassRatio()
        {
            var result = _provider.GetAboveGroundBiomassTotalTreeBiomassRatio(TreeSpecies.WhiteSpruce, 50);
            Assert.AreEqual(result, 0.876860909);
        }
    }
}
