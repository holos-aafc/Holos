using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider_Test
    {
        private Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_27_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider();
        }

        [TestMethod]
        public void GetAnnualEntericMethaneEmissionRateTest()
        {
            var result = _provider.GetAnnualEntericMethaneEmissionRate(new ManagementPeriod() {AnimalType = AnimalType.SwineSows});
            Assert.AreEqual(2.42, result);
        }
    }
}
