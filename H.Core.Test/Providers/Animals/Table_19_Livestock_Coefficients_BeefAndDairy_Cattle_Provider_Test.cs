using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_19_Livestock_Coefficients_BeefAndDairy_Cattle_Provider_Test
    {
        private Table_19_Livestock_Coefficients_BeefAndDairy_Cattle_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_19_Livestock_Coefficients_BeefAndDairy_Cattle_Provider();
        }

        [TestMethod]
        public void GetByBeefAnimalType()
        {
            var result = _provider.GetCoefficientsByAnimalType(AnimalType.BeefFinishingHeifer);
            Assert.AreEqual(AnimalType.BeefFinishingHeifer, result.AnimalType);
            Assert.AreEqual(0.322, result.BaselineMaintenanceCoefficient);
            Assert.AreEqual(0.8, result.GainCoefficient);
            Assert.AreEqual(300, result.DefaultInitialWeight);
            Assert.AreEqual(580, result.DefaultFinalWeight);            
        }

        [TestMethod]
        public void GetByDairyAnimalType()
        {
            var result = _provider.GetCoefficientsByAnimalType(AnimalType.DairyLactatingCow);
            Assert.AreEqual(AnimalType.DairyLactatingCow, result.AnimalType);
            Assert.AreEqual(0.386, result.BaselineMaintenanceCoefficient);
            Assert.AreEqual(0.8, result.GainCoefficient);
            Assert.AreEqual(687, result.DefaultInitialWeight);
            Assert.AreEqual(687, result.DefaultFinalWeight);
        }
    }
}
