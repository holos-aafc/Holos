using H.Core.Calculators.Infrastructure;
using H.Core.Enumerations;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using H.Core.Services.Initialization.Animals;

namespace H.Core.Test.Services.Animals
{
    [TestClass]
    public class AnimalInitializationServiceTest : UnitTestBase
    {
        #region Fields

        private AnimalInitializationService _service;

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
            _service = new AnimalInitializationService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void InitializeFarmTest()
        {
            var farm = new Farm();

            _service.InitializeFarm(farm);
        } 

        #endregion
    }
}
