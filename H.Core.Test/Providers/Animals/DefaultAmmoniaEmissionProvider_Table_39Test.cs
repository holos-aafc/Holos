using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class DefaultAmmoniaEmissionProvider_Table_39Test
    {
        private DefaultAmmoniaEmissionProvider_Table_39 _provider;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DefaultAmmoniaEmissionProvider_Table_39();
        }

        [TestMethod]
        public void GetByAnimalType()
        {
            var result = _provider.GetEmissionFactorByHousing(HousingType.ConfinedNoBarn);
            Assert.AreEqual(0.78, result);
        }

        [TestMethod]
        public void GetByManureStorageType()
        {
            var result = _provider.GetByManureStorageType(ManureStateType.CompostPassive);
            Assert.AreEqual(0.7, result);
        }

        [TestMethod]
        public void GetByLandApplicationType()
        {
            var result = _provider.GetAmmoniaEmissionFactorForSolidAppliedManure(TillageType.Intensive);
            Assert.AreEqual(0.69, result);
        }
    }
}
