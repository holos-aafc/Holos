using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Core.Providers.Economics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Economics
{
    [TestClass]
    public class Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Provider_Test
    {
        #region Fields

        private static Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Provider _provider;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_63_Beef_Cattle_Pasture_Summer_Feed_Cost_Provider();

        }

        [ClassCleanup]
        public static void ClassCleanup() { }


        #endregion


        #region Test Methods

        [TestMethod]
        public void GetTableData()
        {
            var data = _provider.BeefCattlePastureSummerFeedCostData;

            Assert.AreEqual(9, data.Count);
        }

        [TestMethod]
        public void GetBeefCattleDataByType()
        {
            var data = _provider.GetBeefCattlePastureSummerFeedCostData(AnimalType.CowCalf, DietType.MediumEnergyAndProtein);
            Assert.AreEqual(0.24, data.FixedCosts);

            data = _provider.GetBeefCattlePastureSummerFeedCostData(AnimalType.Stockers, DietType.HighEnergyAndProtein);
            Assert.AreEqual(0.02, data.LabourCosts);

            data = _provider.GetBeefCattlePastureSummerFeedCostData(AnimalType.BeefBulls, DietType.LowEnergyAndProtein);
            Assert.AreEqual(1.19, data.VariableCostsNonFeed);

            data = _provider.GetBeefCattlePastureSummerFeedCostData(AnimalType.BeefBulls, DietType.LowEnergyAndProtein);
            Assert.AreEqual(0.99, data.VariableCostFeed);
        }

        [TestMethod]
        public void TestWrongInput()
        {
            var data = _provider.GetBeefCattlePastureSummerFeedCostData(AnimalType.Alpacas, DietType.HighEnergyAndProtein);
            Assert.IsNull(data);

            data = _provider.GetBeefCattlePastureSummerFeedCostData(AnimalType.BeefBulls, DietType.Corn);
            Assert.IsNull(data);
        }


        #endregion

    }
}
