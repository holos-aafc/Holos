using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Services.LandManagement;

namespace H.Core.Test.Services.LandManagement
{
    [TestClass]
    public class IrrigationServiceTest
    {
        #region Fields

        private IrrigationService _sut;

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
            _sut = new IrrigationService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
