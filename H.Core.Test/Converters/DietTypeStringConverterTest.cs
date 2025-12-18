using System;
using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for DietTypeStringConverterTest
    /// </summary>
    [TestClass]
    public class DietTypeStringConverterTest
    {
        private DietTypeStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new DietTypeStringConverter();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ConvertReturnsException()
        {
            converter.Convert("m7");
        }

        [TestMethod]
        public void ConvertReturnsBarley()
        {
            Assert.AreEqual(DietType.Barley, converter.Convert("Barley"));
        }

        [TestMethod]
        public void ConvertReturnsCloseUp()
        {
            Assert.AreEqual(DietType.CloseUp, converter.Convert("Close Up"));
        }

        [TestMethod]
        public void ConvertReturnsCorn()
        {
            Assert.AreEqual(DietType.Corn, converter.Convert("Corn"));
        }

        [TestMethod]
        public void ConvertReturnsFarOffDry()
        {
            Assert.AreEqual(DietType.FarOffDry, converter.Convert("Far Off Dry"));
        }

        [TestMethod]
        public void ConvertReuturnsHighEnergy()
        {
            Assert.AreEqual(DietType.HighEnergy, converter.Convert("High Energy"));
        }

        [TestMethod]
        public void ConvertReturnsHighEnergyAndProtein()
        {
            Assert.AreEqual(DietType.HighEnergyAndProtein, converter.Convert("High Energy/ptn"));
        }

        [TestMethod]
        public void ConvertReturnsLowEnergy()
        {
            Assert.AreEqual(DietType.LowEnergy, converter.Convert("Low Energy"));
        }

        [TestMethod]
        public void ConvertReturnsLowEnergyAndProtein()
        {
            Assert.AreEqual(DietType.LowEnergyAndProtein, converter.Convert("Low Energy/ptn"));
        }

        [TestMethod]
        public void ConvertReturnsMediumEnergy()
        {
            Assert.AreEqual(DietType.MediumEnergy, converter.Convert("Medium Energy"));
        }

        [TestMethod]
        public void ConvertReturnsMediumEnergyAndProtein()
        {
            Assert.AreEqual(DietType.MediumEnergyAndProtein, converter.Convert("Medium Energy/ptn"));
        }

        [TestMethod]
        public void ConvertReturnsMediumGrowth()
        {
            Assert.AreEqual(DietType.MediumGrowth, converter.Convert("Medium Growth"));
        }

        [TestMethod]
        public void ConvertReturnsSlowGrowth()
        {
            Assert.AreEqual(DietType.SlowGrowth, converter.Convert("Slow Growth"));
        }

        [TestMethod]
        public void ConvertReturnsStandard()
        {
            Assert.AreEqual(DietType.Standard, converter.Convert("Standard"));
        }

        [TestMethod]
        public void ConvertReturnsReducedProtein()
        {
            Assert.AreEqual(DietType.ReducedProtein, converter.Convert("Reduced Protein"));
        }

        [TestMethod]
        public void ConvertReturnsHighlyDigestibleFeed()
        {
            Assert.AreEqual(DietType.HighlyDigestibleFeed, converter.Convert("Highly Digestible Feed"));
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