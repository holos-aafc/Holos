using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_36_Daily_Feed_Intake_Swine_Provider_Test
    {
        private Table_36_Daily_Feed_Intake_Swine_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_36_Daily_Feed_Intake_Swine_Provider();
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
