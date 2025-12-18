using System;
using H.Core.Converters;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Converters
{
    /// <summary>
    /// Summary description for ProvinceStringConverterTest
    /// </summary>
    [TestClass]
    public class ProvinceStringConverterTest
    {
        private ProvinceStringConverter converter;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            converter = new ProvinceStringConverter();
        }

        [TestMethod]
        public void ConvertReturnsManitoba()
        {
            var input = "Manitoba";
            var result = converter.Convert(input);
            Assert.AreEqual(Province.Manitoba, result);
        }

        [TestMethod]
        public void ConvertReturnsSaskatchewan()
        {
            Assert.AreEqual(converter.Convert("Saskatchewan"), Province.Saskatchewan);
        }

        [TestMethod]
        public void ConvertReturnsDefaultValueForUnknownProvince()
        {
            Assert.AreEqual(Province.Alberta, converter.Convert("?"));
        }

        [TestMethod]
        public void ConvertReturnsQuebec()
        {
            Assert.AreEqual(converter.Convert("Québec"), Province.Quebec);
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