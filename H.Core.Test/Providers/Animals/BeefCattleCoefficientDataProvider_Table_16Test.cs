using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class BeefCattleCoefficientDataProvider_Table_16Test
    {
        private BeefAndDairyCattleCoefficientDataProvider_Table_19 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new BeefAndDairyCattleCoefficientDataProvider_Table_19();
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
