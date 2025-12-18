using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Soil
{
    [TestClass]
    public class CanadianAgriculturalRegionIdToSlcIdProviderTest
    {
        private CanadianAgriculturalRegionIdToSlcIdProvider _idProvider;

        [TestInitialize]
        public void TestInitialize()
        {

            _idProvider = new CanadianAgriculturalRegionIdToSlcIdProvider();
        }

        [TestMethod]
        public void GetCarId()
        {
            var result = _idProvider.GetCarId(652005, 0);
            Assert.AreEqual(result, 4790);
        }

        [TestMethod]
        public void GetCarIdReturnsDefaultValueWhenNonExistantPolygonIdIsEntered()
        {
            var result = _idProvider.GetCarId(-1, 0);
            Assert.AreEqual(CanadianAgriculturalRegionIdToSlcIdProvider.DefaultValueForBadPolygonInput, result);
        }
    }
}
