using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_20_Dairy_Cattle_Feeding_Activity_Coefficient_Provider_Test
    {
        private Table_20_Dairy_Cattle_Feeding_Activity_Coefficient_Provider _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_20_Dairy_Cattle_Feeding_Activity_Coefficient_Provider();
        }

        [TestMethod]
        public void GetByAnimalType()
        {
            var result = _provider.GetByHousing(HousingType.EnclosedPasture);
            Assert.AreEqual(0.17, result.FeedingActivityCoefficient);
        }
    }
}
