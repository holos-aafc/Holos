using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using H.Core.Providers.Fertilizer;

namespace H.Core.Test.Providers.Fertilizer
{
    [TestClass]
    public class FertilizerBlendProviderTest
    {
        #region Fields

        private FertilizerBlendProvider _provider;

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
            _provider = new FertilizerBlendProvider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetDataReturnsNonEmptyList()
        {
            var result = _provider.GetData();

            Assert.AreEqual(15, result.Count());
        }

        #endregion
    }
}
