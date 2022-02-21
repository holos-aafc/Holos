using System;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers;
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
        /// Equation 3.5.3-1
        /// </summary>
        [TestMethod]
        public void CalculateManureNitrogenReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 25.500;
            var numberOfPoultry = 190.625;
            var numberOfDays = 30;
            var result = _resultsService.CalculateManureNitrogen(nitrogenExcretionRate, numberOfPoultry, numberOfDays);
            Assert.AreEqual(13.317636986301369863013698630137 * numberOfDays, result);
        }


        /// <summary>
        /// Equation 3.5.3-2
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureNitrogen = 276.125;
            var emissionFactor = 176.875;
            var numberOfDays = 51.750;
            var result = _resultsService.CalculateManureDirectNitrogenEmission(manureNitrogen, emissionFactor, numberOfDays);
            Assert.AreEqual(2527449.78515625, result);
        }


        /// <summary>
        /// Equation 3.5.3-3
        /// </summary>
        [TestMethod]
        public void CalculateManureVolatilizationNitrogenEmissionReturnsCorrectValue()
        {
            var manureNitrogen = 283.750;
            var volatilizationFraction = 82.500;
            var emissionFactorForVolatilization = 136.250;
            var numberOfDaysInMonth = 0.875;
            var result = _resultsService.CalculateManureVolatilizationNitrogenEmission(manureNitrogen, volatilizationFraction,
                                                                            emissionFactorForVolatilization, numberOfDaysInMonth);
            Assert.AreEqual(2790836.42578125, result);
        }


        /// <summary>
        ///  Equation 3.5.3-4
        /// </summary>
        [TestMethod]
        public void CalculateManureLeachingNitrogenEmissionReturnsCorrectValue()
        {
            var manureNitrogen = 237.000;
            var leachingFraction = 315.250;
            var emissionFactorForLeaching = 143.500;
            var numberOfDaysInMonth = 178.875;
            var result = _resultsService.CalculateManureLeachingNitrogenEmission(manureNitrogen, leachingFraction,
                                                                      emissionFactorForLeaching, numberOfDaysInMonth);
            Assert.AreEqual(1917807395.765625, result);
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
        /// Equation  3.5.3-6
        /// </summary>
        [TestMethod]
        public void CalculateManureAvailableForLandApplicationReturnsCorrectValue()
        {
            var manureNitrogen = 215.125;
            var volitalizationFraction = 271.875;
            var leachingFraction = 190.750;
            var numberOfDays = 200.125;
            var result = _resultsService.CalculateManureAvailableForLandApplication(manureNitrogen, volitalizationFraction,
                                                                         leachingFraction, numberOfDays);
            Assert.AreEqual(-19873829.009765625, result);
        }

        /// <summary>
        /// Equation 4.2.3-2
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromPoultryOperationsReturnsCorrectValue()
        {
            var barnCapacityForPoultry = 70;

            var numberOfDaysInMonth = 30;
            var energyConversion = 2;
            var result = _resultsService.CalculateTotalEnergyCarbonDioxideEmissionsFromPoultryOperations(barnCapacityForPoultry,
                numberOfDaysInMonth, energyConversion);
            Assert.AreEqual(barnCapacityForPoultry * (2.88/365) * energyConversion * numberOfDaysInMonth, result);
        }

        #endregion

    }
}

