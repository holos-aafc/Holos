using System;
using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for HandlingSystemNameStringConverterTest
    /// </summary>
    [TestClass]
    public class HandlingSystemNameStringConverterTest
    {
        private HandlingSystemNameStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new HandlingSystemNameStringConverter();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ConvertReturnsException()
        {
            converter.Convert("sheep");
        }

        [TestMethod]
        public void ConvertReturnsAnaerobicDigester()
        {
            Assert.AreEqual(ManureStateType.AnaerobicDigester, converter.Convert("Anaerobic Digester"));
        }

        [TestMethod]
        public void ConvertReturnsCompostIntensive()
        {
            Assert.AreEqual(ManureStateType.CompostIntensive, converter.Convert("Compost Intensive"));
        }

        [TestMethod]
        public void ConvertReturnsCompostPassive()
        {
            Assert.AreEqual(ManureStateType.CompostPassive, converter.Convert("Compost Passive"));
        }

        [TestMethod]
        public void ConvertReturnsDailySpread()
        {
            Assert.AreEqual(ManureStateType.DailySpread, converter.Convert("Daily Spread"));
        }

        [TestMethod]
        public void ConvertReturnsDeepBedding()
        {
            Assert.AreEqual(ManureStateType.DeepBedding, converter.Convert("Deep Bedding"));
        }

        [TestMethod]
        public void ConvertReturnsDeepPit()
        {
            Assert.AreEqual(ManureStateType.DeepPit, converter.Convert("Deep Pit"));
        }

        [TestMethod]
        public void ConvertReturnsLiquidCrust()
        {
            Assert.AreEqual(ManureStateType.LiquidCrust, converter.Convert("Liquid Crust"));
        }

        [TestMethod]
        public void ConvertReturnsLiquidNoCrust()
        {
            Assert.AreEqual(ManureStateType.LiquidNoCrust, converter.Convert("Liquid No Crust"));
        }

        [TestMethod]
        public void ConvertReturnsPasture()
        {
            Assert.AreEqual(ManureStateType.Pasture, converter.Convert("Pasture"));
        }

        [TestMethod]
        public void ConvertReturnsSolidStorage()
        {
            Assert.AreEqual(ManureStateType.SolidStorage, converter.Convert("Solid Storage"));
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion
    }
}