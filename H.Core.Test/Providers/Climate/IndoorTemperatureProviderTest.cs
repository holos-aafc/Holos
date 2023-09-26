using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace H.Core.Test.Providers.Climate
{
    [TestClass]
    public class IndoorTemperatureProviderTest
    {
        #region Fields

        private IIndoorTemperatureProvider _provider;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new IndoorTemperatureProvider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void GetTemperatureReturnsCorrectValue()
        {
            var result = _provider.GetTemperature(new DateTime(DateTime.Now.Year, 2, 1));

            Assert.AreEqual(17, result);
        } 

        #endregion
    }
}
