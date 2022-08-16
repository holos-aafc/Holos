using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Soil
{
    [TestClass]
    public class Table_15_16_Soil_N2O_Emission_Factors_Provider_Test
    {
        private Table_15_16_Soil_N2O_Emission_Factors_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_15_16_Soil_N2O_Emission_Factors_Provider();
        }

        [TestMethod]
        public void GetRatioFactor()
        {
            var cropViewItem = new CropViewItem();
            cropViewItem.TillageType = TillageType.Intensive;
            var result = _provider.GetFactorForTillagePractice(Region.EasternCanada, cropViewItem);
            Assert.AreEqual(result, 1.0);
        }
    }
}
