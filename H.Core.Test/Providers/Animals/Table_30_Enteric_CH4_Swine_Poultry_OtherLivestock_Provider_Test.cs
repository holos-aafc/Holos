using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_30_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider_Test
    {
        private Table_30_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_30_Enteric_CH4_Swine_Poultry_OtherLivestock_Provider();
        }

        [TestMethod]
        public void GetAnnualEntericMethaneEmissionRateTest()
        {
            var result = _provider.GetAnnualEntericMethaneEmissionRate(AnimalType.SwineSows);
            Assert.AreEqual(1.5, result);
        }
    }
}
