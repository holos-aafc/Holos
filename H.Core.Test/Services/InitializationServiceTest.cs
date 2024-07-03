using H.Core.Emissions.Results;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Services;

namespace H.Core.Test.Services
{
    [TestClass]
    public class InitializationServiceTest
    {
        #region Fields

        private IInitializationService _initializationService;
        private List<Farm> _farms;
        private Farm _farm1;
        private Farm _farm2;

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
            _farms = new List<Farm>();

            _farm1 = new Farm();
            _farm2 = new Farm();

            _farms.Add(_farm1);
            _farms.Add(_farm2);

            _initializationService = new InitializationService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void ReInitializeFarmsTest()
        {
            _farm1.ClimateData.BarnTemperatureData.January = 30;

            _initializationService.ReInitializeFarms(_farms);

            Assert.AreEqual(_farm1.ClimateData.BarnTemperatureData.January, 2.767742149);
        } 

        #endregion
    }
}
