using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DairyCattleFeedingActivityCoefficientDataProvider_Table_17Test
    {
        private DairyCattleFeedingActivityCoefficientDataProvider_Table_17 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DairyCattleFeedingActivityCoefficientDataProvider_Table_17();
        }

        [TestMethod]
        public void GetByAnimalType()
        {
            var result = _provider.GetByHousing(HousingType.EnclosedPasture);
            Assert.AreEqual(0.17, result.FeedingActivityCoefficient);
        }
    }
}
