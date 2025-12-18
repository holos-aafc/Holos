#region Imports

using System.Linq;
using H.Core.Enumerations;
using H.Core.Providers.Feed;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace H.Core.Test.Providers.Feed
{
    [TestClass]
    public class DietProviderTest
    {
        #region Fields

        private DietProvider _provider;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new DietProvider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetDietsReturnsCorrectValuesForDiet()
        {
            var diets = _provider.GetDiets();

            var diet = diets.Single(x => x.AnimalType == AnimalType.BeefBackgrounder && x.Name == Properties.Resources.SlowGrowthDietType);

            Assert.AreEqual(70.05, diet.TotalDigestibleNutrient);
        }

        [TestMethod]
        public void GetSwineDiet()
        {
            var diets = _provider.GetDiets();

            var dietA = diets.Single(x => x.AnimalType == AnimalType.Swine && x.Name == "Gestation");

            Assert.AreEqual(14.4, dietA.CrudeProtein, 1);
            Assert.AreEqual(2172.9, dietA.NetEnergy, 1);
            Assert.AreEqual(0.45, dietA.P, 1);
        }

        #endregion
    }
}