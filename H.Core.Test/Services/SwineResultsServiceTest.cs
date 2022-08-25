using System;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers;
using H.Core.Providers.Feed;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services
{
    [TestClass]
    public class SwineResultsServiceTest
    {
        #region Fields
        
        private SwineResultsService _resultsService; 
        
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
            _resultsService = new SwineResultsService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
        #endregion

        #region Tests

        /// <summary>
        /// Equation 3.4.1-1
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionReturnsCorrectValue()
        {
            var entericMethaneEmissionRate = 713.4375;
            var numberOfPigs = 387.3125;
            var result =
                _resultsService.CalculateEntericMethaneEmissionForSwinePoultryAndOtherLivestock(entericMethaneEmissionRate, numberOfPigs);
            Assert.AreEqual((entericMethaneEmissionRate / 365)* numberOfPigs, result);
        }


        /// <summary>
        /// Equation 3.3.2-1
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidAdjustedReturnsCorrectValue()
        {
            var volatileSolidExcretion = 1229.8125;
            var volatileSolidAdjustmentFactor = 1366.9375;
            var result = _resultsService.CalculateVolatileSolidAdjusted(volatileSolidExcretion, volatileSolidAdjustmentFactor);
            Assert.AreEqual(1681076.82421875, result);
        }


        /// <summary>
        /// Equation 3.3.2-2
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsReturnsCorrectValue()
        {
            var volatileSolidAdjusted = 1479.3750;
            var feedIntake = 912.6250;
            var result = _resultsService.CalculateVolatileSolids(volatileSolidAdjusted, feedIntake);
            Assert.AreEqual(1350114.609375, result);
        }


        /// <summary>
        /// Equation 3.3.2-3
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionRateReturnsCorrectValue()
        {
            var volatileSolids = 7.4375;
            var methaneProducingCapacity = 11.4375;
            var methaneConversionFactor = 1821.9375;
            var result =
                _resultsService.CalculateManureMethaneEmissionRate(volatileSolids, methaneProducingCapacity,
                                                        methaneConversionFactor);
            Assert.AreEqual(103840.40260986329, result);
        }


        /// <summary>
        ///    Equation 3.3.2-4
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionReturnsCorrectValue()
        {
            var manureMethaneEmissionRate = 19.6875;
            var numberOfPigs = 285.3750;
            var numberOfDays = 85.4375;
            var result = _resultsService.CalculateManureMethaneEmission(manureMethaneEmissionRate, numberOfPigs, numberOfDays);
            Assert.AreEqual(480015.24169921875, result);
        }


        /// <summary>
        /// Equation 3.3.3-1
        /// </summary>
        [TestMethod]
        public void CalculateProteinIntakeReturnsCorrectValue()
        {
            var feedIntake = 40.6250;
            var crudeProteinIntake = 504.9375;
            var result = _resultsService.CalculateProteinIntake(feedIntake, crudeProteinIntake);
            Assert.AreEqual(20513.0859375, result);
        }

        /// <summary>
        ///  Equation 3.3.3-3
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenExcretionRateReturnsCorrectValue()
        {
            var proteinIntake = 856.0625;
            var proteinRetained = 319.4375;
            var nitrogenExcretedAdjustmentFactor = 93.0000;
            var result =
                _resultsService.CalculateNitrogenExcretionRate(proteinIntake, proteinRetained, nitrogenExcretedAdjustmentFactor);
            Assert.AreEqual(-4056323.746875, result);
        }


        /// <summary>
        /// Equation 3.3.3-4
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrogenEmissionRateReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 796.9375;
            var emissionFactor = 1705.3750;
            var result = _resultsService.CalculateManureDirectNitrogenEmissionRate(nitrogenExcretionRate, emissionFactor);
            Assert.AreEqual(1359077.2890625, result);
        }


        /// <summary>
        ///    Equation 3.3.3-5
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureDirectNitrogenEmissionRate = 363.5625;
            var numberOfPigs = 1100.3125;
            var result = _resultsService.CalculateManureDirectNitrogenEmission(manureDirectNitrogenEmissionRate, numberOfPigs);
            Assert.AreEqual(numberOfPigs * manureDirectNitrogenEmissionRate, result);
        }


        /// <summary>
        /// Equation 3.3.3-6
        /// </summary>
        [TestMethod]
        public void CalculateManureVolatilizationNitrogenEmissionRateReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 1047.1250;
            var volatilizationFraction = 410.2500;
            var emissionFactorForVolatilization = 135.2500;
            var result = _resultsService.CalculateManureVolatilizationNitrogenEmissionRate(nitrogenExcretionRate,
                                                                                volatilizationFraction, emissionFactorForVolatilization);
            Assert.AreEqual(58101104.9765625, result);
        }

        [TestMethod]
        public void CalculateManureVolatilizationNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmissionRate = 580.3125;
            var numberOfPigs = 1484.1250;
            var result = _resultsService.CalculateManureVolatilizationNitrogenEmission(manureVolatilizationNitrogenEmissionRate,
                                                                            numberOfPigs);
            Assert.AreEqual(numberOfPigs * manureVolatilizationNitrogenEmissionRate, result);
        }


        /// <summary>
        /// Equation 3.3.3-8
        /// </summary>
        [TestMethod]
        public void CalculateManureLeachingNitrogenEmissionRateReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 206.3125;
            var leachingFraction = 750.3125;
            var emissionFactorForLeaching = 824.1250;
            var result = _resultsService.CalculateManureLeachingNitrogenEmissionRate(nitrogenExcretionRate, leachingFraction,
                                                                          emissionFactorForLeaching, 0);
            Assert.AreEqual(127573600.32470703125, result);
        }



        [TestMethod]
        public void CalculateManureLeachingNitrogenEmissionReturnsCorrectValue()
        {
            var manureLeachingNitrogenEmissionRate = 940.7500;
            var numberOfPigs = 938.5625;
            var result =
                _resultsService.CalculateManureLeachingNitrogenEmission(manureLeachingNitrogenEmissionRate, numberOfPigs);
            Assert.AreEqual(manureLeachingNitrogenEmissionRate * numberOfPigs, result);
        }


        /// <summary>
        /// Equation 3.3.3-10
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmission = 303.8750;
            var manureLeachingNitrogenEmission = 930.4375;
            var result = _resultsService.CalculateManureIndirectNitrogenEmission(manureVolatilizationNitrogenEmission,
                                                                      manureLeachingNitrogenEmission);
            Assert.AreEqual(1234.3125, result);
        }

        /// <summary>
        /// Equation 3.3.3-11
        /// </summary>
        [TestMethod]
        public void CalculateManureNitrogenEmissionReturnsCorrectValue()
        {
            var manureDirectNitrogenEmission = 303.8750;
            var manureIndirectNitrogenEmission = 303.8750;
            var result = _resultsService.CalculateManureNitrogenEmission(manureDirectNitrogenEmission,
                                                                      manureIndirectNitrogenEmission);
            Assert.AreEqual(607.75, result);
        }


        /// <summary>
        /// Equation 3.3.3-12
        /// </summary>
        [TestMethod]
        public void CalculateManureAvailableForLandApplicationReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 840.8125;
            var numberOfPigs = 621.3125;
            var numberOfDays = 498.7500;
            var volatilizationFraction = 291.8125;
            var leachingFraction = 319.2500;
            var result = _resultsService.CalculateManureAvailableForLandApplication(nitrogenExcretionRate, numberOfPigs,
                                                                         numberOfDays, volatilizationFraction, leachingFraction);
            Assert.AreEqual(-158952180340.71258544921875, result);
        }


        /// <summary>
        /// Equation 3.3.5-1
        /// </summary>
        [TestMethod]
        public void CalculateTotalManureDirectN2OEmissionFromSwineReturnsCorrectValue()
        {
            var manureDirectNitrogenEmissionFromSwine = 606.2500;
            var result = _resultsService.CalculateTotalManureDirectN2OEmissionFromSwine(manureDirectNitrogenEmissionFromSwine);
            Assert.AreEqual(952.67857142857142857142857142857, result);
        }


        /// <summary>
        /// Equation 3.3.5-2
        /// </summary>
        [TestMethod]
        public void CalculateTotalManureIndirectN2OEmissionFromSwineReturnsCorrectValue()
        {
            var manureIndirectNitrogenEmissionFromSwine = 462.8750;
            var result =
                _resultsService.CalculateTotalManureIndirectN2OEmissionFromSwine(manureIndirectNitrogenEmissionFromSwine);
            Assert.AreEqual(727.375, result);
        }


        /// <summary>
        /// Equation 3.3.5-3
        /// </summary>
        [TestMethod]
        public void CalculateTotalManureN2OEmissionFromSwineReturnsCorrectValue()
        {
            var result = _resultsService.CalculateTotalManureN2OEmissionFromSwine(3.456);
            Assert.AreEqual(5.4308571428571428571428571428571, result);
        }

        /// <summary>
        /// Equation 4.2.2-1   # pigs = # sows
        /// Equation 4.2.2-2   # pigs = # boars
        /// Equation 4.2.2-3  # pigs = # finishers
        /// Equation 4.2.2-4   # pigs = # growers
        /// Equation 4.2.2-5   # pigs = # starters
        /// Equation 4.2.2-6
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromSwineOperationsReturnsCorrectValue()
        {
            var numberOfPigs = 60;
            var numberOfDaysInMonth = 29;
            var electricityConversion = 20;
            var result = _resultsService.CalculateTotalCarbonDioxideEmissionsFromSwineHousing(numberOfPigs,
                numberOfDaysInMonth,
                electricityConversion);
            Assert.AreEqual(numberOfPigs * numberOfDaysInMonth * (1.06 / 365) * electricityConversion, result, 2);
        }

        #endregion
    }
}

