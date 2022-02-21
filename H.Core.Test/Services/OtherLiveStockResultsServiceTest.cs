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
    public class OtherLiveStockResultsServiceTest
    {
        #region Fields
        
        private OtherLivestockResultsService _service; 
        
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
            _service = new OtherLivestockResultsService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
        #endregion

        #region Tests

        /// <summary>
        /// Equation 3.6.1-1
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionReturnsCorrectValue()
        {
            var entericMethaneEmissionRate = 150.125;
            var numberOfAnimals = 59.125;
            var numberOfDaysInMonth = 70.750;
            var result =
                _service.CalculateEntericMethaneEmission(entericMethaneEmissionRate, numberOfAnimals, numberOfDaysInMonth);
            Assert.AreEqual(1720.5121896404109589041095890411, result);
        }


        /// <summary>
        ///  Equation 3.6.2-1
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionReturnsCorrectValue()
        {
            var manureMethaneEmissionRate = 88.250;
            var numberOfAnimals = 126.500;
            var numberOfDaysInMonth = 215.500;
            var result =
                _service.CalculateManureMethaneEmission(manureMethaneEmissionRate, numberOfAnimals, numberOfDaysInMonth);
            Assert.AreEqual(6591.126541095890410958904109589, result);
        }


        /// <summary>
        ///    Equation 3.6.3-1
        /// </summary>
        [TestMethod]
        public void CalculateManureNitrogenReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 25.500;
            var numberOfAnimals = 190.625;
            var result = _service.CalculateManureNitrogen(nitrogenExcretionRate, numberOfAnimals);
            Assert.AreEqual(13.317636986301369863013698630137, result);
        }


        /// <summary>
        /// Equation 3.6.3-2
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureNitrogen = 276.125;
            var emissionFactor = 176.875;
            var numberOfDays = 51.750;
            var result = _service.CalculateManureDirectNitrogenEmission(manureNitrogen, emissionFactor, numberOfDays);
            Assert.AreEqual(2527449.78515625, result);
        }


        /// <summary>
        /// Equation 3.6.3-3
        /// </summary>
        [TestMethod]
        public void CalculateManureVolatilizationNitrogenEmissionReturnsCorrectValue()
        {
            var manureNitrogen = 283.750;
            var volatilizationFraction = 82.500;
            var emissionFactorForVolatilization = 136.250;
            var numberOfDaysInMonth = 0.875;
            var result = _service.CalculateManureVolatilizationNitrogenEmission(manureNitrogen, volatilizationFraction,
                                                                            emissionFactorForVolatilization, numberOfDaysInMonth);
            Assert.AreEqual(2790836.42578125, result);
        }


        /// <summary>
        /// Equation 3.6.3-4
        /// </summary>
        [TestMethod]
        public void CalculateManureLeachingNitrogenEmissionReturnsCorrectValue()
        {
            var manureNitrogen = 237.000;
            var leachingFraction = 315.250;
            var emissionFactorForLeaching = 143.500;
            var numberOfDaysInMonth = 178.875;
            var result = _service.CalculateManureLeachingNitrogenEmission(manureNitrogen, leachingFraction,
                                                                      emissionFactorForLeaching, numberOfDaysInMonth);
            Assert.AreEqual(1917807395.765625, result);
        }


        /// <summary>
        ///    Equation 3.6.3-5
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmission = 18.750;
            var manureLeachingNitrogenEmission = 231.000;
            var result = _service.CalculateManureIndirectNitrogenEmission(manureVolatilizationNitrogenEmission,
                                                                      manureLeachingNitrogenEmission);
            Assert.AreEqual(249.75, result);
        }


        /// <summary>
        ///    Equation 3.6.3-6
        /// </summary>
        [TestMethod]
        public void CalculateManureAvailableForLandApplicationReturnsCorrectValue()
        {
            var result = _service.CalculateManureAvailableForLandApplication();
            Assert.AreEqual(0.0, result);
        }


        /// <summary>
        ///  Equation 3.6.4-1
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrousEmissionFromAnimalsReturnsCorrectValue()
        {
            var manureDirectNitrogenEmissionFromAnimals = 138.750;
            var result = _service.CalculateManureDirectNitrousEmissionFromAnimals(manureDirectNitrogenEmissionFromAnimals);
            Assert.AreEqual(218.03571428571428571428571428571, result);
        }


        /// <summary>
        ///    Equation 3.6.4-2
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrousEmissionFromAnimalsReturnsCorrectValue()
        {
            var manureIndirectNitrogenEmissionFromAnimals = 279.250;
            var result =
                _service.CalculateManureIndirectNitrousEmissionFromAnimals(manureIndirectNitrogenEmissionFromAnimals);
            Assert.AreEqual(438.82142857142857142857142857143, result);
        }

        #endregion
    }
}
