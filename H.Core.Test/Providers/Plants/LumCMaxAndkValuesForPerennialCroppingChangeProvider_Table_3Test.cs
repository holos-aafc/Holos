using H.Core.Enumerations;
using H.Core.Providers.Plants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Plants
{
    [TestClass]
    public class LumCMaxAndkValuesForPerennialCroppingChangeProvider_Table_3Test
    {
        private LumCMaxAndkValueForPerennialsAndGrasslandProvider_Table_3 _forPerennialsAndGrasslandProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _forPerennialsAndGrasslandProvider = new LumCMaxAndkValueForPerennialsAndGrasslandProvider_Table_3();
        }

        [TestMethod]
        public void GetLumCMax()
        {
            var result = _forPerennialsAndGrasslandProvider.GetLumCMax(Ecozone.AtlanticMaritimes, SoilTexture.Coarse, PerennialCroppingChangeType.DecreaseInPerennialCroppingArea);
            Assert.AreEqual(result, -3769);
        }

        [TestMethod]
        public void GetKValue()
        {
            var result = _forPerennialsAndGrasslandProvider.GetKValue(Ecozone.BorealShieldWest, SoilTexture.Medium, PerennialCroppingChangeType.IncreaseInPerennialCroppingArea);
            Assert.AreEqual(result, 0.0253);
        }
    }
}
