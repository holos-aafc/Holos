using H.Core.Providers.Irrigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.Irrigation
{
    [TestClass]
    public class Table_7_Monthly_Irrigation_Water_Application_Provider_Test
    {
        #region Fields
        private static Table_7_Monthly_Irrigation_Water_Application_Provider _waterApplicationProvider;
        #endregion

        #region Initalization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _waterApplicationProvider = new Table_7_Monthly_Irrigation_Water_Application_Provider();
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
        public void GetMonthlyAverageIrrigationDataTest()
        {
            Table_7_Monthly_Irrigation_Water_Application_Data irrigationWaterApplicationData = _waterApplicationProvider.GetMonthlyAverageIrrigationDataInstance(Months.April, Province.BritishColumbia);

            Assert.AreEqual(8, irrigationWaterApplicationData.IrrigationVolume);
        }

        [TestMethod]
        public void CheckNullReturnWrongMonth()
        {
            Table_7_Monthly_Irrigation_Water_Application_Data irrigationWaterApplicationData = _waterApplicationProvider.GetMonthlyAverageIrrigationDataInstance(Months.January, Province.NewBrunswick);
            Assert.AreEqual(null, irrigationWaterApplicationData);
        }

        [TestMethod]
        public void CheckNullReturnWrongProvince()
        {
            Table_7_Monthly_Irrigation_Water_Application_Data irrigationWaterApplicationData = _waterApplicationProvider.GetMonthlyAverageIrrigationDataInstance(Months.April, Province.Yukon);
            Assert.AreEqual(null, irrigationWaterApplicationData);
        }

        [TestMethod]
        public void CheckAllWrongInput()
        {
            Table_7_Monthly_Irrigation_Water_Application_Data irrigationWaterApplicationData = _waterApplicationProvider.GetMonthlyAverageIrrigationDataInstance(Months.February, Province.Yukon);
            Assert.AreEqual(null, irrigationWaterApplicationData);
        }
        #endregion



    }
}
