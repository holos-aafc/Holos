using H.Core.Enumerations;
using H.Core.Providers.Plants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Plants
{
    [TestClass]
    public class LumCMaxAndKValuesForTillagePracticeChangeProvider_Table_1Test
    {
        private LumCMaxAndKValuesForTillagePracticeChangeProvider_Table_1 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new LumCMaxAndKValuesForTillagePracticeChangeProvider_Table_1();
        }

        [TestMethod]
        public void GetLumCMax()
        {
            var result = _provider.GetLumCMax(Ecozone.AtlanticMaritimes, SoilTexture.Coarse, TillagePracticeChangeType.IntenseToReduced);
            Assert.AreEqual(result, 232);
        }

        [TestMethod]
        public void GetKValue()
        {
            var result = _provider.GetKValue(Ecozone.BorealShieldWest, SoilTexture.Medium, TillagePracticeChangeType.IntenseToNone);
            Assert.AreEqual(result, 0.0311);
        }
    }
}
