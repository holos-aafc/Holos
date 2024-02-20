using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.Climate
{
    [TestClass]
    public class IndoorTemperatureProviderTest
    {
        #region Fields

        private Table_63_Indoor_Temperature_Provider _sut;

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
            _sut = new Table_63_Indoor_Temperature_Provider();
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
            var result = _sut.GetTemperature(new DateTime(DateTime.Now.Year, 2, 1));

            Assert.AreEqual(17, result);
        }

        [TestMethod]
        public void ReadFileTest()
        {
            var result = _sut.ReadFile();

            Assert.AreEqual(10, result.Count);
        }

        [TestMethod]
        public void GetDataTest()
        {
            var result = _sut.GetIndoorTemperature(Province.Quebec);

            Assert.AreEqual(18.17, result.September, 2);
        }

        #endregion
    }
}
