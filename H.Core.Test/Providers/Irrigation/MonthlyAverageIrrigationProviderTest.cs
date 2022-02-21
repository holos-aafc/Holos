using H.Core.Providers.Irrigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.Irrigation
{
    [TestClass]
    public class MonthlyAverageIrrigationProviderTest
    {
        #region Fields
        private static MonthlyAverageIrrigationProvider _provider;
        #endregion

        #region Initalization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new MonthlyAverageIrrigationProvider();
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
            MonthlyAverageIrrigationData data = _provider.GetMonthlyAverageIrrigationDataInstance(Months.April, Province.BritishColumbia);

            Assert.AreEqual(8, data.IrrigationVolume);
        }

        [TestMethod]
        public void CheckNullReturnWrongMonth()
        {
            MonthlyAverageIrrigationData data = _provider.GetMonthlyAverageIrrigationDataInstance(Months.January, Province.NewBrunswick);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void CheckNullReturnWrongProvince()
        {
            MonthlyAverageIrrigationData data = _provider.GetMonthlyAverageIrrigationDataInstance(Months.April, Province.Yukon);
            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void CheckAllWrongInput()
        {
            MonthlyAverageIrrigationData data = _provider.GetMonthlyAverageIrrigationDataInstance(Months.February, Province.Yukon);
            Assert.AreEqual(null, data);
        }
        #endregion



    }
}
