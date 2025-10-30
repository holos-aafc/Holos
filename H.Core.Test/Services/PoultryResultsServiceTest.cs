using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Temperature;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services
{
    [TestClass]
    public class PoultryResultsServiceTest
    {
        #region Fields
        
        private PoultryResultsService _resultsService; 
        
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
            _resultsService = new PoultryResultsService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
        #endregion

        #region Tests

        /// <summary>
        ///  Equation 3.5.1-1
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionReturnsCorrectValue()
        {
            var entericMethaneEmissionRate = 150.125;
            var numberOfPoultry = 59.125;
            var numberOfDaysInMonth = 70.750;
            var result =
                _resultsService.CalculateEntericMethaneEmission(entericMethaneEmissionRate, numberOfPoultry, numberOfDaysInMonth);
            Assert.AreEqual(1720.5121896404109589041095890411, result);
        }


        /// <summary>
        ///    Equation 3.5.2-1
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionReturnsCorrectValue()
        {
            var manureMethaneEmissionRate = 88.250;
            var numberOfPoultry = 126.500;
            var numberOfDaysInMonth = 215.500;
            var result =
                _resultsService.CalculateManureMethaneEmission(manureMethaneEmissionRate, numberOfPoultry, numberOfDaysInMonth);
            Assert.AreEqual(6591.126541095890410958904109589, result);
        }

        /// <summary>
        /// Equation 3.5.3-5
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmission = 18.750;
            var manureLeachingNitrogenEmission = 231.000;
            var result = _resultsService.CalculateManureIndirectNitrogenEmission(manureVolatilizationNitrogenEmission,
                                                                      manureLeachingNitrogenEmission);
            Assert.AreEqual(249.75, result);
        }

        /// <summary>
        /// Equation 4.2.3-2
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromPoultryOperationsReturnsCorrectValue()
        {
            var barnCapacityForPoultry = 70;
            var housingFactor = 2.88;
            var numberOfDaysInMonth = 30;
            var energyConversion = 2;
            var result = _resultsService.CalculateTotalEnergyCarbonDioxideEmissionsFromPoultryOperations(barnCapacityForPoultry,
                numberOfDaysInMonth, energyConversion, housingFactor);
            Assert.AreEqual(barnCapacityForPoultry * (housingFactor/365) * energyConversion * numberOfDaysInMonth, result);
        }

        #endregion
    }
}

