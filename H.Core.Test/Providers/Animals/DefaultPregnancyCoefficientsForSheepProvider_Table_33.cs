using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultPregnancyCoefficientsForSheepProvider_Table_33Test
    {
        private DefaultPregnancyCoefficientsForSheepProvider_Table_33 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultPregnancyCoefficientsForSheepProvider_Table_33();
        }

        [TestMethod]
        public void GetByAnimalType()
        {
            var result = _provider.GetPregnancyCoefficient(1.5);
            Assert.AreEqual(0.1015, result);
        }
    }
}
