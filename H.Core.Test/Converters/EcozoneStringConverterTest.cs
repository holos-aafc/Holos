using System;
using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for EcozoneStringConverterTest
    /// </summary>
    [TestClass]
    public class EcozoneStringConverterTest
    {
        private EcozoneStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new EcozoneStringConverter();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ConvertReturnsException()
        {
            converter.Convert("Alligator");
        }

        [TestMethod]
        public void ConvertReturnsAtlanticMaritimes()
        {
            Assert.AreEqual(Ecozone.AtlanticMaritimes, converter.Convert("Atlantic Maritimes"));
        }

        [TestMethod]
        public void ConvertReturnsBorealPlains()
        {
            Assert.AreEqual(Ecozone.BorealPlains, converter.Convert("Boreal Plains"));
        }

        [TestMethod]
        public void ConvertReturnsBorealShieldEast()
        {
            Assert.AreEqual(Ecozone.BorealShieldEast, converter.Convert("Boreal Shield East"));
        }

        [TestMethod]
        public void ConvertReturnsBorealShieldWest()
        {
            Assert.AreEqual(Ecozone.BorealShieldWest, converter.Convert("Boreal Shield West"));
        }

        [TestMethod]
        public void ConvertReturnsMixedwoodPlains()
        {
            Assert.AreEqual(Ecozone.MixedwoodPlains, converter.Convert("Mixedwood Plains"));
        }

        [TestMethod]
        public void ConvertReturnsMontaneCordillera()
        {
            Assert.AreEqual(Ecozone.MontaneCordillera, converter.Convert("Montane Cordillera"));
        }

        [TestMethod]
        public void ConvertReturnsPacificMaritime()
        {
            Assert.AreEqual(Ecozone.PacificMaritime, converter.Convert("Pacific Maritime"));
        }

        [TestMethod]
        public void ConvertReturnsSemiaridPrairies()
        {
            Assert.AreEqual(Ecozone.SemiaridPrairies, converter.Convert("Semiarid Prairies"));
        }

        [TestMethod]
        public void ConvertReturnsSubhumidPrairies()
        {
            Assert.AreEqual(Ecozone.SubhumidPrairies, converter.Convert("Subhumid Prairies"));
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