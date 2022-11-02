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
using H.Core.Providers.Feed;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Temperature;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services
{
    [TestClass]
    public class SheepResultsServiceTest
    {
        #region Fields
        
        private SheepResultsService _resultsService; 
        
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
            _resultsService = new SheepResultsService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }
        #endregion

        #region Tests

        /// <summary>
        /// Equation 3.4.1-2
        /// </summary>
        [TestMethod]
        public void CalculateLambRatioReturnsCorrectValue()
        {
            var numberOfLambs = 104.500;
            var numberOfEwes = 105.875;
            var result = _resultsService.CalculateLambRatio(numberOfLambs, numberOfEwes);
            Assert.AreEqual(0.987012987012987, result);
        }


        /// <summary>
        /// Equation 3.4.1-3
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForMaintenanceReturnsCorrectValue()
        {
            var maintenanceCoefficient = 93.375;
            var averageWeight = 173.375;
            var result = _resultsService.CalculateNetEnergyForMaintenance(maintenanceCoefficient, averageWeight);
            Assert.AreEqual(4461.3928855295277006211506085777, result);
        }


        /// <summary>
        /// Equation 3.4.1-4
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForActivityReturnsCorrectValue()
        {
            var feedingActivityCoefficient = 198.625;
            var averageWeight = 110.500;
            var result = _resultsService.CalculateNetEnergyForActivity(feedingActivityCoefficient, averageWeight);
            Assert.AreEqual(21948.0625, result);
        }


        /// <summary>
        ///  Equation 3.4.1-5
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForLactationReturnsCorrectValue()
        {
            var dailyWeightGainOfLambs = 225.375;
            var energyRequiredToProduceAKilogramOfMilk = 184.125;
            var result =
                _resultsService.CalculateNetEnergyForLactation(dailyWeightGainOfLambs, energyRequiredToProduceAKilogramOfMilk);
            Assert.AreEqual(207485.859375, result);
        }


        /// <summary>
        /// Equation 3.4.1-6
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForPregnancyReturnsCorrectValue()
        {
            var pregnancyConstant = 197.375;
            var netEnergyForMaintenance = 171.125;
            var result = _resultsService.CalculateNetEnergyForPregnancy(pregnancyConstant, netEnergyForMaintenance);
            Assert.AreEqual(33775.796875, result);
        }


        /// <summary>
        /// Equation 3.4.1-7
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForWoolProductionReturnsCorrectValue()
        {
            var energyValueOfAKilogramOfWool = 196.875;
            var woolProduction = 81.250;
            var result = _resultsService.CalculateNetEnergyForWoolProduction(energyValueOfAKilogramOfWool, woolProduction);
            Assert.AreEqual(43.824914383561643835616438356164, result);
        }


        /// <summary>
        ///    Equation 3.4.1-8
        /// </summary>
        [TestMethod]
        public void CalculateAverageDailyGainReturnsCorrectValue()
        {
            var finalWeight = 122.500;
            var initialWeight = 221.375;
            var numberOfDays = 124.750;
            var result = _resultsService.CalculateAverageDailyWeightGain(initialWeight: initialWeight, finalWeight: finalWeight, numberOfDays: numberOfDays);
            Assert.AreEqual((finalWeight - initialWeight)/numberOfDays, result);
        }


        /// <summary>
        ///  Equation 3.4.1-9
        ///  Equation 3.4.1-10
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForGainReturnsCorrectValue()
        {
            var averageDailyGain = 239.125;
            var averageWeight = 182.750;
            var coefficientA = 52.125;
            var coefficientB = 363.125;
            var result = _resultsService.CalculateNetEnergyForGain(averageDailyGain, averageWeight, coefficientA, coefficientB);
            Assert.AreEqual(15881060.93359375, result);
        }


        /// <summary>
        ///  Equation 3.4.1-11
        /// </summary>
        [TestMethod]
        public void
            CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumedReturnsCorrectValue()
        {
            var percentTotalDigestibleEnergyInFeed = 194.125;
            var result =
                _resultsService.CalculateRatioOfEnergyAvailableForMaintenance(
                                                                                                      percentTotalDigestibleEnergyInFeed);
            Assert.AreEqual(0.62212461728328228, result);
        }


        /// <summary>
        /// Equation 3.4.1-12
        /// </summary>
        [TestMethod]
        public void CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumedReturnsCorrectValue()
        {
            var percentTotalDigestibleEnergyInFeed = 50.875;
            var result =
                _resultsService.CalculateRatioEnergyAvailableForGain(
                                                                                               percentTotalDigestibleEnergyInFeed);
            Assert.AreEqual(0.20020437923986478, result);
        }


        /// <summary>
        /// Equation 3.4.1-13
        /// </summary>
        [TestMethod]
        public void CalculateGrossEnergyIntakeReturnsCorrectValue()
        {
            var netEnergyForMaintenance = 153.000;
            var netEnergyForActivity = 52.375;
            var netEnergyForLactation = 236.625;
            var netEnergyForPregnancy = 5.875;
            var netEnergyForGain = 7.125;
            var netEnergyForWoolProduction = 284.000;
            var ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed = 26.625;
            var ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed = 21.000;
            var percentTotalDigestibleEnergyInFeed = 75.875;
            var result = _resultsService.CalculateGrossEnergyIntake(netEnergyForMaintenance, netEnergyForActivity,
                                                         netEnergyForLactation, netEnergyForPregnancy, netEnergyForGain, netEnergyForWoolProduction,
                                                         ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed,
                                                         ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed, percentTotalDigestibleEnergyInFeed);
            Assert.AreEqual(40.441109037531064, result);
        }


        /// <summary>
        /// Equation 3.4.1-14
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionRateReturnsCorrectValue()
        {
            var grossEnergyIntake = 44.250;
            var methaneConversionFactor = 82.875;
            var result = _resultsService.CalculateEntericMethaneEmissionRateForSheep(grossEnergyIntake, methaneConversionFactor);
            Assert.AreEqual(65.897911051212938005390835579515, result);
        }


        /// <summary>
        /// Equation 3.4.1-15
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionReturnsCorrectValue()
        {
            var entericMethaneEmissionRate = 158.125;
            var numberOfSheep = 286.625;
            var result = _resultsService.CalculateEntericMethaneEmissions(entericMethaneEmissionRate, numberOfSheep);
            Assert.AreEqual(entericMethaneEmissionRate * numberOfSheep, result);
        }


        /// <summary>
        ///  Equation 3.4.2-1
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsReturnsCorrectValue()
        {
            var grossEnergyIntake = 125.625;
            var percentTotalDigestibleNutrientsInFeed = 200.500;
            var ashContentOfFeed = 15.375;
            var result = _resultsService.CalculateVolatileSolids(grossEnergyIntake, percentTotalDigestibleNutrientsInFeed,
                                                      ashContentOfFeed);
            Assert.AreEqual(-5.5603957063008123, result);
        }


        /// <summary>
        /// Equation 3.4.2-2
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionRateReturnsCorrectValue()
        {
            var volatileSolids = 221.375;
            var methaneProducingCapacity = 156.875;
            var methaneConversionFactor = 25.125;
            var result =
                _resultsService.CalculateManureMethaneEmissionRate(volatileSolids, methaneProducingCapacity,
                                                        methaneConversionFactor);
            Assert.AreEqual(584605.88935546875, result);
        }


        /// <summary>
        ///    Equation 3.4.2-3
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionReturnsCorrectValue()
        {
            var manureMethaneEmissionRate = 165.250;
            var numberOfSheep = 135.750;
            var numberOfDays = 269.125;
            var result = _resultsService.CalculateManureMethaneEmission(manureMethaneEmissionRate, numberOfSheep, numberOfDays);
            Assert.AreEqual(6037197.0234375, result);
        }


        /// <summary>
        ///   Equation 3.4.3-1
        /// </summary>
        [TestMethod]
        public void CalculateProteinIntakeReturnsCorrectValue()
        {
            var grossEnergyIntake = 14.375;
            var crudeProteinContent = 129.750;
            var result = _resultsService.CalculateProteinIntake(grossEnergyIntake, crudeProteinContent);
            Assert.AreEqual(101.0924, result, 4);
        }


        /// <summary>
        ///  Equation 3.4.3-2
        /// </summary>
        [TestMethod]
        public void CalculateProteinRetainedReturnsCorrectValue()
        {
            var result = _resultsService.CalculateProteinRetained();
            Assert.AreEqual(0.1, result);
        }


        /// <summary>
        ///    Equation 3.4.3-3
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenExcretionRateReturnsCorrectValue()
        {
            var proteinIntake = 66.750;
            var proteinRetained = 268.625;
            var result = _resultsService.CalculateNitrogenExcretionRate(proteinIntake, proteinRetained);
            Assert.AreEqual(-2858.235, result);
        }


        /// <summary>
        /// Equation 3.4.3-4
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrogenEmissionRateReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 38.500;
            var emissionFactor = 50.625;
            var result = _resultsService.CalculateManureDirectNitrogenEmissionRate(nitrogenExcretionRate, emissionFactor);
            Assert.AreEqual(1949.0625, result);
        }

        [TestMethod]
        public void CalculateManureDirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureDirectNitrogenEmissionRate = 103.625;
            var numberOfSheep = 75.750;
            var result =
                _resultsService.CalculateManureDirectNitrogenEmission(manureDirectNitrogenEmissionRate, numberOfSheep);
            Assert.AreEqual(numberOfSheep * manureDirectNitrogenEmissionRate, result);
        }


        /// <summary>
        /// Equation 3.4.3-6
        /// </summary>
        [TestMethod]
        public void CalculateManureVolatilizationNitrogenEmissionRateReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 185.625;
            var volatilizationFraction = 109.375;
            var emissionFactorForVolatilization = 20.750;
            var result = _resultsService.CalculateManureVolatilizationNitrogenEmissionRate(nitrogenExcretionRate,
                                                                                volatilizationFraction, emissionFactorForVolatilization);
            Assert.AreEqual(421281.73828125, result);
        }


        /// <summary>
        /// Equation 3.4.3-7
        /// </summary>
        [TestMethod]
        public void CalculateManureVolatilizationNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmissionRate = 149.875;
            var numberOfSheep = 353.625;
            var result = _resultsService.CalculateManureVolatilizationNitrogenEmission(manureVolatilizationNitrogenEmissionRate,
                                                                            numberOfSheep);
            Assert.AreEqual(numberOfSheep * manureVolatilizationNitrogenEmissionRate, result);
        }


        /// <summary>
        /// Equation 3.4.3-8
        /// </summary>
        [TestMethod]
        public void CalculateManureLeachingNitrogemEmissionRateReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 76.250;
            var leachingFraction = 29.000;
            var emissionFactorForLeaching = 106.750;
            var result = _resultsService.CalculateManureLeachingNitrogemEmissionRate(nitrogenExcretionRate, leachingFraction,
                                                                          emissionFactorForLeaching);
            Assert.AreEqual(236050.9375, result);
        }


        [TestMethod]
        public void CalculateManureLeachingNitrogenEmissionReturnsCorrectValue()
        {
            var manureLeachingNitrogemEmissionRate = 156.125;
            var numberOfSheep = 136.125;
            var result =
                _resultsService.CalculateManureLeachingNitrogenEmission(manureLeachingNitrogemEmissionRate, numberOfSheep);
            Assert.AreEqual(manureLeachingNitrogemEmissionRate * numberOfSheep, result);
        }


        /// <summary>
        /// Equation 3.4.3-10
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmission = 116.500;
            var manureLeachingNitrogenEmission = 91.750;
            var result = _resultsService.CalculateManureIndirectNitrogenEmission(manureVolatilizationNitrogenEmission,
                                                                      manureLeachingNitrogenEmission);
            Assert.AreEqual(208.25, result);
        }

        /// <summary>
        /// Equation 3.4.3-11
        /// </summary>
        [TestMethod]
        public void CalculateManureNitrogenEmissionReturnsCorrectValue()
        {
            var manureDirectNitrogenEmission = 116.500;
            var manureIndirectNitrogenEmission = 91.750;
            var result = _resultsService.CalculateManureNitrogenEmission(manureDirectNitrogenEmission,
                                                                      manureIndirectNitrogenEmission);
            Assert.AreEqual(208.25, result);
        }


        /// <summary>
        ///    Equation 3.4.3-12
        /// </summary>
        [TestMethod]
        public void CalculateManureAvailableForLandApplicationReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 47.125;
            var numberOfSheep = 30.500;
            var numberOfDays = 55.875;
            var volatilizationFraction = 103.375;
            var leachingFraction = 175.250;
            var result = _resultsService.CalculateManureAvailableForLandApplication(nitrogenExcretionRate, numberOfSheep,
                                                                         numberOfDays, volatilizationFraction, leachingFraction);
            Assert.AreEqual(-22296018.2021484375, result);
        }

        /// <summary>
        /// Equation 3.4.5-1
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrousOxideEmissionFromSheepReturnsCorrectValue()
        {
            var manureDirectNitrogenEmissionFromSheep = 324.875;
            var result = _resultsService.CalculateTotalManureDirectNitrousOxideEmissionFromSheep(manureDirectNitrogenEmissionFromSheep);
            Assert.AreEqual(510.51785714285714285714285714286, result);
        }


        /// <summary>
        ///   Equation 3.4.5-2
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrousOxideEmissionFromSheepReturnsCorrectValue()
        {
            var manureIndirectNitrogenEmissionFromSheep = 96.375;
            var result =
                _resultsService.CalculateTotalManureIndirectNitrousOxideEmissionFromSheep(manureIndirectNitrogenEmissionFromSheep);
            Assert.AreEqual(151.44642857142857142857142857143, result);
        }


        /// <summary>
        ///   Equation 3.4.5-3
        /// </summary>
        [TestMethod]
        public void CalculateTotalManureNitrousOxideEmissionFromSheepReturnsCorrectValue()
        {
            var result = _resultsService.CalculateTotalManureNitrousOxideEmissionFromSheep(3.456);
            Assert.AreEqual(5.4308571428571428571428571428571, result);
        }

        /// <summary>
        ///   Equation 9.4-3
        /// </summary>
        [TestMethod]
        public void CalculateLambProducedPerMonth()
        {
            var result = _resultsService.CalculateLambProducedPerMonth(123.43, 65.33, 0.65);
            Assert.AreEqual(5241.393235, result);
        }

        /// <summary>
        ///   Equation 9.3-1
        /// </summary>
        [TestMethod]
        public void CalculateDryMatterIntake()
        {
            var result = _resultsService.CalculateDryMatterIntake(54.74);
            Assert.AreEqual(2.9669376693766937669376693766938, result);
        }

       

        #endregion
    }
}
