using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.Energy;

namespace H.Core.Test.Providers.Energy
{
    [TestClass]
    public class FuelEnergyEstimatesProviderTest
    {
        #region Fields
        private static FuelEnergyEstimatesProvider_Table_50 _provider;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new FuelEnergyEstimatesProvider_Table_50();
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
        public void GetFuelEnergyEstimateInstanceAlberta()
        {
            FuelEnergyEstimatesData data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Alberta, SoilFunctionalCategory.Black, TillageType.NoTill, CropType.CanarySeed);

            Assert.AreEqual(1.43, data.FuelEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyEstimateInstanceOntario()
        {
            FuelEnergyEstimatesData data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Ontario, SoilFunctionalCategory.EasternCanada, TillageType.Reduced, CropType.WinterTurnipRapeBrassicaRapaSppOleiferaLCVLargo);
            Assert.AreEqual(1.8, data.FuelEstimate);
        }
        
        [TestMethod]
        public void GetFuelEnergyEstimateInstanceBritishColumbia()
        {
            FuelEnergyEstimatesData data = _provider.GetFuelEnergyEstimatesDataInstance(Province.BritishColumbia, SoilFunctionalCategory.Brown, TillageType.NoTill, CropType.ForageForSeed);
            Assert.AreEqual(1.42, data.FuelEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyEstimateInstanceNewfoundLand()
        {
            FuelEnergyEstimatesData data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Newfoundland, SoilFunctionalCategory.EasternCanada, TillageType.Intensive, CropType.Chickpeas);
            Assert.AreEqual(3.29, data.FuelEstimate);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongProvince()
        {
            FuelEnergyEstimatesData data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Yukon, SoilFunctionalCategory.Brown, TillageType.NoTill, CropType.ForageForSeed);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongSoil()
        {
            FuelEnergyEstimatesData data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Newfoundland, SoilFunctionalCategory.Organic, TillageType.NoTill, CropType.Durum);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void GetFuelEnergyInstanceWrongCrop()
        {
            FuelEnergyEstimatesData data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Newfoundland, SoilFunctionalCategory.EasternCanada, TillageType.NoTill, CropType.LargeKabuliChickpea);
            Assert.AreEqual(null, data);
        }
        #endregion

        [TestMethod]
        public void GetFuelEnergyValue()
        {
            FuelEnergyEstimatesData data = _provider.GetFuelEnergyEstimatesDataInstance(Province.NewBrunswick, SoilFunctionalCategory.EasternCanada, TillageType.NoTill, CropType.Potatoes);
            Assert.AreEqual(1.9, data.FuelEstimate);
        }
    }
}
