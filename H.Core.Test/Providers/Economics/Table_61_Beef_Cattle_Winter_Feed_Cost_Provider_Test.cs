using H.Core.Enumerations;
using H.Core.Providers.Economics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Economics
{
    [TestClass]
    public class Table_61_Beef_Cattle_Winter_Feed_Cost_Provider_Test
    {
        #region Fields

        private Table_61_Beef_Cattle_Winter_Feed_Cost_Provider _provider;

        #endregion


        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) {}

        [ClassCleanup]
        public static void ClassCleanup() {}

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new Table_61_Beef_Cattle_Winter_Feed_Cost_Provider();
        }

        [TestCleanup]
        public void TestCleanup() {}

        #endregion


        #region Tests

        [TestMethod]
        public void GetBeefCattleData()
        {
            var data = _provider.BeefCattleWinterFeedData;

            Assert.AreEqual(data.Count, 6);
        }

        [TestMethod]
        public void GetBeefCattleDataByType()
        {
            var data = _provider.GetBeefCattleWinterFeedCost(AnimalType.CowCalf, DietType.All);
            Assert.AreEqual(0.24, data.FixedCosts);

            data = _provider.GetBeefCattleWinterFeedCost(AnimalType.BeefFinisher, DietType.Corn);
            Assert.AreEqual(0.16, data.LabourCosts);

            data = _provider.GetBeefCattleWinterFeedCost(AnimalType.BeefBackgrounder, DietType.SlowGrowth);
            Assert.AreEqual(4.05, data.VariableCostsNonFeed);
        }

        [TestMethod]
        public void TestWrongInput()
        {
            var data = _provider.GetBeefCattleWinterFeedCost(AnimalType.Alpacas, DietType.All);
            Assert.IsNull(data);

            data = _provider.GetBeefCattleWinterFeedCost(AnimalType.BeefBulls, DietType.Corn);
            Assert.IsNull(data);
        }
        #endregion
    }
}
