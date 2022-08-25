using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_42_Crude_Protein_Content_Swine_Feed_Provider_Test
    {
        private Table_42_Crude_Protein_Content_Swine_Feed_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_42_Crude_Protein_Content_Swine_Feed_Provider();
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
