using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultCrudeProteinInFeedForPigGroupsByProvinceProvider_Table_38Test
    {
        private DefaultCrudeProteinInFeedForSwineProvider_Table_38 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultCrudeProteinInFeedForSwineProvider_Table_38();
        }

        [TestMethod]
        public void GetByProvince()
        {
            var result = _provider.GetByProvince(Province.Ontario);
            Assert.AreEqual(0.210, result[AnimalType.SwineStarter]);
            Assert.AreEqual(0.175, result[AnimalType.SwineGrower]);
            Assert.AreEqual(0.135, result[AnimalType.SwineFinisher]);
            Assert.AreEqual(0.135, result[AnimalType.SwineDrySow]);
            Assert.AreEqual(0.135, result[AnimalType.SwineBoar]);
            Assert.AreEqual(0.185, result[AnimalType.SwineLactatingSow]);
        }

        [TestMethod]
        public void GetCrudeProteinInFeedForPigGroupsByProvince()
        {
            var result = _provider.GetCrudeProteinInFeedForSwineGroupByProvince(Province.Alberta, AnimalType.SwineBoar);
            Assert.AreEqual(0.135, result);
        }
    }
}
