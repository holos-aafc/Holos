using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_39_Crude_Protein_Content_Swine_Feed_Provider_Test
    {
        private Table_39_Crude_Protein_Content_Swine_Feed_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_39_Crude_Protein_Content_Swine_Feed_Provider();
        }

        [TestMethod]
        public void GetByProvince()
        {
            var result = _provider.GetByProvince(Province.Ontario);
            Assert.AreEqual(14.28, result[DietType.Gestation]);
        }

        [TestMethod]
        public void GetCrudeProteinInFeedForPigGroupsByProvince()
        {
            var result = _provider.GetCrudeProteinInFeedForSwineGroupByProvince(Province.Alberta, DietType.Boars);
            Assert.AreEqual(20.1, result);
        }
    }
}
