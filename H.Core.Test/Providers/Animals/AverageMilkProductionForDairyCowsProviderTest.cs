using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Providers.Animals;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class AverageMilkProductionForDairyCowsProviderTest
    {
        #region Fields

        private Table_24_Average_Milk_Production_Dairy_Cows_Provider _provider;

        #endregion

        #region Initialization
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) { }

        [ClassCleanup]
        public static void ClassCleanup() { }

        [TestInitialize]
        public void TestIntialize()
        {
            _provider = new Table_24_Average_Milk_Production_Dairy_Cows_Provider();
        }

        [TestCleanup]
        public void TestCleanup() { }
        #endregion

        #region Tests

        [TestMethod]
        public void GetMilkProductionAlberta()
        {
            var avgMilkProduction = _provider.GetAverageMilkProductionForDairyCowsValue(2010, Province.Alberta);

            Assert.AreEqual(30.6, avgMilkProduction);
        }

        [TestMethod]
        public void GetMilkProductionPEI()
        {
            var avgMilkProduction = _provider.GetAverageMilkProductionForDairyCowsValue(2020, Province.PrinceEdwardIsland);

            Assert.AreEqual(32.8, avgMilkProduction);
        }

        [TestMethod]
        public void GetMilkProductionQuebec()
        {
            var avgMilkProduction = _provider.GetAverageMilkProductionForDairyCowsValue(2014, Province.Quebec);

            Assert.AreEqual(28.8, avgMilkProduction);
        }


        [TestMethod]
        public void CheckIncorrectYear()
        {
            var avgMilkProduction = _provider.GetAverageMilkProductionForDairyCowsValue(1989, Province.Ontario);

            // Should return value for last available year
            Assert.AreEqual(21.7, avgMilkProduction);
        }

        [TestMethod]
        public void CheckStartingYear()
        {
            var avgMilkProduction = _provider.GetAverageMilkProductionForDairyCowsValue(1990, Province.Ontario);

            Assert.AreEqual(21.7, avgMilkProduction);
        }

        [TestMethod]
        public void CheckIncorrectProvince()
        {
            var avgMilkProduction = _provider.GetAverageMilkProductionForDairyCowsValue(1990, Province.Yukon);

            Assert.AreEqual(0, avgMilkProduction);
        }

        #endregion
    }
}
