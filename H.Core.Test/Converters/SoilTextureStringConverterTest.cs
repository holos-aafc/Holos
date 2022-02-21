using System;
using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for SoilTextureStringConverterTest
    /// </summary>
    [TestClass]
    public class SoilTextureStringConverterTest
    {
        private SoilTextureStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new SoilTextureStringConverter();
        }

        [TestMethod]
        public void ConvertReturnsFine()
        {
            Assert.AreEqual(SoilTexture.Fine, converter.Convert("Fine"));
        }

        [TestMethod]
        public void ConvertReturnsMedium()
        {
            Assert.AreEqual(SoilTexture.Medium, converter.Convert("Medium"));
        }

        [TestMethod]
        public void ConvertReturnsCoarse()
        {
            Assert.AreEqual(SoilTexture.Coarse, converter.Convert("Coarse"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ConvertReturnsException()
        {
            converter.Convert("beer");
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