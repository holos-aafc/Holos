using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultsFeedIntakeForEachPigGroupByProvinceProvider_Table_32Test
    {
        private DefaultFeedIntakeForSwineProviderTable_Table_32 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultFeedIntakeForSwineProviderTable_Table_32();
        }

        [TestMethod]
        public void GetByProvince()
        {
            var result = _provider.GetByProvince(Province.Ontario);

            Assert.AreEqual(0.65, result[AnimalType.SwineStarter]);
            Assert.AreEqual(2.0, result[AnimalType.SwineGrower]);
            Assert.AreEqual(2.80, result[AnimalType.SwineFinisher]);
            Assert.AreEqual(2.45, result[AnimalType.SwineDrySow]);
            Assert.AreEqual(2.45, result[AnimalType.SwineBoar]);
            Assert.AreEqual(5.85, result[AnimalType.SwineLactatingSow]);
        }

        [TestMethod]
        public void GetDataByManureStorageType()
        {
            var result = _provider.GetFeedIntakeAmount(AnimalType.SwineStarter, Province.Alberta);

            Assert.AreEqual(0.7, result);
        }
    }
}
