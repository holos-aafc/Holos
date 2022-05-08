using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.Energy;

namespace H.Core.Test.Providers.Energy
{
    [TestClass]
    public class HerbicideEnergyDefaultsProviderTest
    {
        #region Fields
        private static HerbicideEnergyEstimatesProvider _provider;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new HerbicideEnergyEstimatesProvider();
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
            HerbicideEnergyEstimatesData data = _provider.GetHerbicideEnergyDataInstance(Province.Ontario, SoilFunctionalCategory.EasternCanada, TillageType.Reduced, CropType.BeansDryField);
            Assert.AreEqual(0.12, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyEstimateInstanceBritishColumbia()
        {
            HerbicideEnergyEstimatesData data = _provider.GetHerbicideEnergyDataInstance(Province.BritishColumbia, SoilFunctionalCategory.Brown, TillageType.NoTill, CropType.ForageForSeed);
            Assert.AreEqual(0.46, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyEstimateInstanceNewfoundLand()
        {
            HerbicideEnergyEstimatesData data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.EasternCanada, TillageType.Intensive, CropType.Chickpeas);
            Assert.AreEqual(0.08, data.HerbicideEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongProvince()
        {
            HerbicideEnergyEstimatesData data = _provider.GetHerbicideEnergyDataInstance(Province.Yukon, SoilFunctionalCategory.Brown, TillageType.NoTill, CropType.ForageForSeed);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongSoil()
        {
            HerbicideEnergyEstimatesData data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.Organic, TillageType.NoTill, CropType.Durum);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongCrop()
        {
            HerbicideEnergyEstimatesData data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.EasternCanada, TillageType.NoTill, CropType.LargeKabuliChickpea);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void GetFuelEnergyValue()
        {
            HerbicideEnergyEstimatesData data = _provider.GetHerbicideEnergyDataInstance(Province.NewBrunswick, SoilFunctionalCategory.EasternCanada, TillageType.NoTill, CropType.Potatoes);
            Assert.AreEqual(0.12, data.HerbicideEstimate);
        }
        #endregion
    }
}
