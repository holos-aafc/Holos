using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for TreeSpeciesTest
    /// </summary>
    [TestClass]
    public class TreeSpeciesStringConverterTest
    {
        private TreeSpeciesStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new TreeSpeciesStringConverter();
        }

        [TestMethod]
        public void ConvertReturnsCaragana()
        {
            Assert.AreEqual(TreeSpecies.Caragana, converter.Convert("Caragana"));
        }

        [TestMethod]
        public void ConvertReturnsGreenAsh()
        {
            Assert.AreEqual(TreeSpecies.GreenAsh, converter.Convert("GreenAsh"));
        }

        [TestMethod]
        public void ConvertReturnsHybridPoplar()
        {
            Assert.AreEqual(TreeSpecies.HybridPoplar, converter.Convert("HybridPoplar"));
        }

        [TestMethod]
        public void ConvertReturnsManitobaMaple()
        {
            Assert.AreEqual(TreeSpecies.ManitobaMaple, converter.Convert("ManitobaMaple"));
        }

        [TestMethod]
        public void ConvertReturnsScotsPine()
        {
            Assert.AreEqual(TreeSpecies.ScotsPine, converter.Convert("ScotsPine"));
        }

        [TestMethod]
        public void ConvertReturnsWhiteSpruce()
        {
            Assert.AreEqual(TreeSpecies.WhiteSpruce, converter.Convert("WhiteSpruce"));
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