using System;
using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for AnimalTypeStringConverterTest
    /// </summary>
    [TestClass]
    public class AnimalTypeStringConverterTest
    {
        private AnimalTypeStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new AnimalTypeStringConverter();
        }

        [TestMethod]
        public void ConvertReturnsDefaultValueWhenUnknowAnimalTypeUsed()
        {
            Assert.AreEqual(AnimalType.BeefBackgrounder, converter.Convert("Tree"));
        }

        [TestMethod]
        public void ConvertReturnsBackgrounder()
        {
            Assert.AreEqual(AnimalType.BeefBackgrounder, converter.Convert("Backgrounder"));
        }

        [TestMethod]
        public void ConvertReturnsBeef()
        {
            Assert.AreEqual(AnimalType.Beef, converter.Convert("Beef"));
        }

        [TestMethod]
        public void ConvertReturnsBeefFinisher()
        {
            Assert.AreEqual(AnimalType.BeefFinisher, converter.Convert("Beef Finisher"));
        }

        [TestMethod]
        public void ConvertReturnsBoar()
        {
            Assert.AreEqual(AnimalType.SwineBoar, converter.Convert("SwineBoar"));
        }

        [TestMethod]
        public void ConvertReturnsChicken()
        {
            Assert.AreEqual(AnimalType.Chicken, converter.Convert("Chicken"));
        }

        [TestMethod]
        public void ConvertReturnsCowCalf()
        {
            Assert.AreEqual(AnimalType.CowCalf, converter.Convert("CowCalf"));
        }

        [TestMethod]
        public void ConvertReturnsDairy()
        {
            Assert.AreEqual(AnimalType.Dairy, converter.Convert("Dairy"));
        }

        [TestMethod]
        public void ConvertReturnsDairyBulls()
        {
            Assert.AreEqual(AnimalType.DairyBulls, converter.Convert("DairyBulls"));
        }

        [TestMethod]
        public void convertReturnsDairyDry()
        {
            Assert.AreEqual(AnimalType.DairyDryCow, converter.Convert("DairyDryCow"));
        }

        [TestMethod]
        public void ConvertReturnsDairyHeifers()
        {
            Assert.AreEqual(AnimalType.DairyHeifers, converter.Convert("DairyHeifers"));
        }

        [TestMethod]
        public void ConvertReturnsDairyLactating()
        {
            Assert.AreEqual(AnimalType.DairyLactatingCow, converter.Convert("DairyLactating"));
        }

        [TestMethod]
        public void ConvertReturnsDrySow()
        {
            Assert.AreEqual(AnimalType.SwineDrySow, converter.Convert("Dry Sow"));
        }

        [TestMethod]
        public void ConvertReturnsGrower()
        {
            Assert.AreEqual(AnimalType.SwineGrower, converter.Convert("SwineGrower"));
        }

        [TestMethod]
        public void ConvertReturnsLactatingSow()
        {
            Assert.AreEqual(AnimalType.SwineLactatingSow, converter.Convert("Lactating Sow"));
        }

        [TestMethod]
        public void ConvertReturnsSheep()
        {
            Assert.AreEqual(AnimalType.Sheep, converter.Convert("Sheep"));
        }

        [TestMethod]
        public void ConvertReturnsStockers()
        {
            Assert.AreEqual(AnimalType.Stockers, converter.Convert("Stockers"));
        }

        [TestMethod]
        public void ConvertReturnsSwine()
        {
            Assert.AreEqual(AnimalType.Swine, converter.Convert("Swine"));
        }

        [TestMethod]
        public void ConvertReturnsSwineFinisher()
        {
            Assert.AreEqual(AnimalType.SwineFinisher, converter.Convert("Swine Finisher"));
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