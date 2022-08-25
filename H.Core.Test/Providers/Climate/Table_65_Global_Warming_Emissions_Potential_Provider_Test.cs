using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.Climate;

namespace H.Core.Test.Providers.Climate
{
    [TestClass]

    public class Table_65_Global_Warming_Emissions_Potential_Provider_Test
    {

        #region Fields
        private static Table_65_Global_Warming_Emissions_Potential_Provider _provider;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_65_Global_Warming_Emissions_Potential_Provider();
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
        public void GetGlobalWarmingPotentialValue()
        {
            Table_65_Global_Warming_Emissions_Potential_Data data = _provider.GetGlobalWarmingEmissionsInstance(1990, EmissionTypes.CH4);
            Assert.AreEqual(21, data.GlobalWarmingPotentialValue);
        }

        [TestMethod]
        public void InstanceNotFoundWrongEmission()
        {
            Table_65_Global_Warming_Emissions_Potential_Data data = _provider.GetGlobalWarmingEmissionsInstance(1995, EmissionTypes.CO2e);
            Assert.AreEqual(0, data.GlobalWarmingPotentialValue);
        }

        [TestMethod]
        public void InstanceNotFoundWrongYear()
        {
            Table_65_Global_Warming_Emissions_Potential_Data data = _provider.GetGlobalWarmingEmissionsInstance(2035, EmissionTypes.N2O);
            Assert.AreEqual(0, data.GlobalWarmingPotentialValue);
        }

        [TestMethod]
        public void DataInstanceNotFoundAllWrongInput()
        {
            Table_65_Global_Warming_Emissions_Potential_Data data = _provider.GetGlobalWarmingEmissionsInstance(2014, EmissionTypes.LivestockDirectN20);
            Assert.AreEqual(0, data.GlobalWarmingPotentialValue);
        }

        [TestMethod]
        public void GetCH4NonFossilValue()
        {
            Table_65_Global_Warming_Emissions_Potential_Data data = _provider.GetGlobalWarmingEmissionsInstance(2021, EmissionTypes.NonFossilCH4);
            Assert.AreEqual(30, data.GlobalWarmingPotentialValue);
        }

        [TestMethod]
        public void GetDataInstance()
        {
            Table_65_Global_Warming_Emissions_Potential_Data data = _provider.GetGlobalWarmingEmissionsInstance(2013, EmissionTypes.N2O);
            Assert.AreNotEqual(null, data);
        }

        #endregion

    }
}
