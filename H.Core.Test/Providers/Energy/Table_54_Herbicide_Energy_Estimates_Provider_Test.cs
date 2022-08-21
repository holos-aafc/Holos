using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.Energy;

namespace H.Core.Test.Providers.Energy
{
    [TestClass]
    public class Table_54_Herbicide_Energy_Estimates_Provider_Test
    {
        #region Fields
        private static Table_54_Herbicide_Energy_Estimates_Provider _provider;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_54_Herbicide_Energy_Estimates_Provider();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
        #endregion

        #region Tests

        [TestMethod]
        public void GetHerbicideEnergyEstimateInstanceOntario()
        {
            Table_54_Herbicide_Energy_Estimates_Data data = _provider.GetHerbicideEnergyDataInstance(Province.Ontario, SoilFunctionalCategory.EasternCanada, TillageType.Reduced, CropType.BeansDryField);
            Assert.AreEqual(0.12, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyEstimateInstanceBritishColumbia()
        {
            Table_54_Herbicide_Energy_Estimates_Data data = _provider.GetHerbicideEnergyDataInstance(Province.BritishColumbia, SoilFunctionalCategory.Brown, TillageType.NoTill, CropType.ForageForSeed);
            Assert.AreEqual(0.46, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyEstimateInstanceNewfoundLand()
        {
            Table_54_Herbicide_Energy_Estimates_Data data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.EasternCanada, TillageType.Intensive, CropType.Chickpeas);
            Assert.AreEqual(0.08, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongProvince()
        {
            Table_54_Herbicide_Energy_Estimates_Data data = _provider.GetHerbicideEnergyDataInstance(Province.Yukon, SoilFunctionalCategory.Brown, TillageType.NoTill, CropType.ForageForSeed);
            Assert.AreEqual(0, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongSoil()
        {
            Table_54_Herbicide_Energy_Estimates_Data data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.Organic, TillageType.NoTill, CropType.Durum);
            Assert.AreEqual(0, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongCrop()
        {
            Table_54_Herbicide_Energy_Estimates_Data data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.EasternCanada, TillageType.NoTill, CropType.LargeKabuliChickpea);
            Assert.AreEqual(0, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyValue()
        {
            Table_54_Herbicide_Energy_Estimates_Data data = _provider.GetHerbicideEnergyDataInstance(Province.NewBrunswick, SoilFunctionalCategory.EasternCanada, TillageType.NoTill, CropType.Potatoes);
            Assert.AreEqual(0.12, data.HerbicideEstimate);

            data = _provider.GetHerbicideEnergyDataInstance(Province.Quebec, SoilFunctionalCategory.EasternCanada, TillageType.Reduced, CropType.Tobacco);
            Assert.AreEqual(0.24, data.HerbicideEstimate);

            data = _provider.GetHerbicideEnergyDataInstance(Province.Saskatchewan, SoilFunctionalCategory.Brown, TillageType.Intensive, CropType.BerseemCloverTrifoliumAlexandriumL);
            Assert.AreEqual(0.16, data.HerbicideEstimate);

            data = _provider.GetHerbicideEnergyDataInstance(Province.Alberta, SoilFunctionalCategory.Black, TillageType.NoTill, CropType.ForageForSeed);
            Assert.AreEqual(0.46, data.HerbicideEstimate);
        }
        #endregion
    }
}
