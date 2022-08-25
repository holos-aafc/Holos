using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_28_Pregnancy_Coefficients_For_Sheep_Provider_Test
    {
        private Table_28_Pregnancy_Coefficients_For_Sheep_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_28_Pregnancy_Coefficients_For_Sheep_Provider();
        }

        [TestMethod]
        public void GetByAnimalType()
        {
            var result = _provider.GetPregnancyCoefficient(1.5);
            Assert.AreEqual(0.1015, result);
        }
    }
}
