using System;
using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for HousingStringConverterTest
    /// </summary>
    [TestClass]
    public class HousingStringConverterTest
    {
        private HousingStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new HousingStringConverter();
        }

        [TestMethod]
        public void ConvertReturnsConfinedNoBarn()
        {
            Assert.AreEqual(HousingType.ConfinedNoBarn, converter.Convert("Confined No Barn"));
        }

        [TestMethod]
        public void ConvertReturnsHousedInBarn()
        {
            Assert.AreEqual(HousingType.HousedInBarn, converter.Convert("Housed In Barn"));
        }

        [TestMethod]
        public void ConvertReturnsEnclosedPasture()
        {
            Assert.AreEqual(HousingType.EnclosedPasture, converter.Convert("Enclosed Pasture"));
        }

        [TestMethod]
        public void ConvertReturnsOpenRangeOrHills()
        {
            Assert.AreEqual(HousingType.OpenRangeOrHills, converter.Convert("Open Range Or Hills"));
        }

        [TestMethod]
        public void ConvertReturnsTieStall()
        {
            Assert.AreEqual(HousingType.TieStall, converter.Convert("Tie-Stall"));
        }

        [TestMethod]
        public void ConvertReturnsSmallFreeStall()
        {
            Assert.AreEqual(HousingType.SmallFreeStall, converter.Convert("Small Free-Stall"));
        }

        [TestMethod]
        public void ConvertReturnsLargeFreeStall()
        {
            Assert.AreEqual(HousingType.LargeFreeStall, converter.Convert("Large Free-Stall"));
        }

        [TestMethod]
        public void ConvertReturnsDrylot()
        {
            Assert.AreEqual(HousingType.DryLot, converter.Convert("Drylot"));
        }

        [TestMethod]
        public void ConvertReturnsGrazingUnder3km()
        {
            Assert.AreEqual(HousingType.GrazingUnder3km, converter.Convert("Grazing < 3km/d"));
        }

        [TestMethod]
        public void ConvertReturnsGrazingOver3km()
        {
            Assert.AreEqual(HousingType.GrazingOver3km, converter.Convert("Grazing > 3km/d"));
        }

        [TestMethod]
        public void ConvertReturnsConfined()
        {
            Assert.AreEqual(HousingType.Confined, converter.Convert("Confined"));
        }

        [TestMethod]
        public void ConvertReturnsFlatPasture()
        {
            Assert.AreEqual(HousingType.FlatPasture, converter.Convert("Flat Pasture"));
        }

        [TestMethod]
        public void ConvertReturnsHillyPastureOrOpenRange()
        {
            Assert.AreEqual(HousingType.HillyPastureOrOpenRange, converter.Convert("Hilly Pasture / Open Range"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ConvertReturnsException()
        {
            converter.Convert("Beef");
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