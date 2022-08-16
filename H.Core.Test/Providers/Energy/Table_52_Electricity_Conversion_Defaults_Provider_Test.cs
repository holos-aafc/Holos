using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Providers.Energy;

namespace H.Core.Test.Providers.Energy
{
    [TestClass]
    public class Table_52_Electricity_Conversion_Defaults_Provider_Test
    {
        #region Fields
        private static Table_52_Electricity_Conversion_Defaults_Provider _provider;
        private const int FirstYear = 1990;
        private const int LastYear = 2018;
        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_52_Electricity_Conversion_Defaults_Provider();
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
        public void GetElectricityConversionDataInstance()
        {
            Table_52_Electricity_Conversion_Defaults_Data data = _provider.GetElectricityConversionData(1994, Province.Alberta);
            Assert.AreEqual(0.98, data.ElectricityValue);
        }

        [TestMethod]
        public void TestWrongYearElectricityDataInstance()
        {
            Table_52_Electricity_Conversion_Defaults_Data data = _provider.GetElectricityConversionData(2075, Province.PrinceEdwardIsland);
            Assert.AreEqual(0, data.ElectricityValue);
        }

        [TestMethod]
        public void TestWrongProvinceElectricityDataInstance()
        {
            Table_52_Electricity_Conversion_Defaults_Data data = _provider.GetElectricityConversionData(1994, Province.Yukon);
            Assert.AreEqual(0, data.ElectricityValue);
        }

        [TestMethod]
        public void TestAllWrongInputElectricityDataInstance()
        {
            Table_52_Electricity_Conversion_Defaults_Data data = _provider.GetElectricityConversionData(2075, Province.Yukon);
            Assert.AreEqual(0, data.ElectricityValue);
        }


        [TestMethod]
        public void CheckStartYearLessThanData()
        {
            double averageElectricityValue = _provider.GetElectricityConversionValue(1985, Province.Ontario);
            Assert.AreEqual(0.166, averageElectricityValue);
        }

        [TestMethod]
        public void CheckEndYearGreaterThanData()
        {
            double averageElectricityValue = _provider.GetElectricityConversionValue(2019, Province.NovaScotia);
            Assert.AreEqual(0.694, averageElectricityValue);
        }

        [TestMethod]
        public void GetValueOfYearWithinRange()
        {
            double electricityValue = _provider.GetElectricityConversionValue(2005, Province.Saskatchewan);
            Assert.AreEqual(0.78, electricityValue);
        }

        [TestMethod]
        public void GetValueAtUpperBoundary()
        {
            double electricityValue = _provider.GetElectricityConversionValue(FirstYear, Province.Quebec);
            Assert.AreEqual(0.013, electricityValue);
        }

        [TestMethod]
        public void GetValueAtLowerBoundary()
        {
            double electricityValue = _provider.GetElectricityConversionValue(LastYear, Province.Alberta);
            Assert.AreEqual(0.63, electricityValue);
        }


        #endregion

    }
}