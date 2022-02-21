using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultVolatileSolidExcretionForPigGroupByProvinceProvider_Table_25Test
    {
        private DefaultVolatileSolidExcretionForPigGroupByProvinceProvider_Table_30 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultVolatileSolidExcretionForPigGroupByProvinceProvider_Table_30();
        }

        [TestMethod]
        public void GetByProvince()
        {
            var result = _provider.GetByProvince(Province.Alberta);
            Assert.AreEqual(0.1504, result[AnimalType.SwineStarter]);
            Assert.AreEqual(0.1389, result[AnimalType.SwineGrower]);
            Assert.AreEqual(0.1389, result[AnimalType.SwineFinisher]);
            Assert.AreEqual(0.1228, result[AnimalType.SwineDrySow]);
            Assert.AreEqual(0.1228, result[AnimalType.SwineLactatingSow]);
        }
    }
}
