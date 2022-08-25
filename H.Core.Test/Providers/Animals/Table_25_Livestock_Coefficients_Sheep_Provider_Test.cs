using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_25_Livestock_Coefficients_Sheep_Provider_Test
    {
        private Table_25_Livestock_Coefficients_Sheep_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_25_Livestock_Coefficients_Sheep_Provider();
        }

        [TestMethod]
        public void GetDataByManureStorageType()
        {
            var result = _provider.GetCoefficientsByAnimalType(AnimalType.Ram) as Table_25_Livestock_Coefficients_Sheep_Data;
            Assert.AreEqual(AnimalType.Ram, result.AnimalType);
            Assert.AreEqual(0.250, result.BaselineMaintenanceCoefficient);
            Assert.AreEqual(2.5, result.CoefficientA);
            Assert.AreEqual(0.35, result.CoefficientB);
            Assert.AreEqual(125, result.DefaultInitialWeight);
            Assert.AreEqual(125, result.DefaultFinalWeight);
            Assert.AreEqual(4, result.WoolProduction);
        }
    }
}
