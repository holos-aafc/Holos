using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Infrastructure.Test
{
    [TestClass]
    public class ListExtensionsTest
    {
        #region Fields

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
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void WeightedAverageTest()
        {
            var records = new[]
            {
                new {Area = 100, EmissionFactor = 2},
                new {Area = 500, EmissionFactor = 10}
            };

            var weightedAverage = records.WeightedAverage(record => record.EmissionFactor, record => record.Area);

            Assert.IsTrue(weightedAverage > 0);
        }

        #endregion   
    }
}
