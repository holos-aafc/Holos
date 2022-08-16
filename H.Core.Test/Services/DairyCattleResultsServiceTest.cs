using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Dairy;
using H.Core.Providers.Feed;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services
{
    [TestClass]
    public class DairyCattleResultsServiceTest
    {
        #region Fields
        
        private DairyCattleResultsService _dairyCattleResultsService;

        #endregion

        #region Tests
        
        [TestInitialize]
        public void TestInitialize()
        {
            _dairyCattleResultsService = new DairyCattleResultsService();
        }

        [TestMethod]
        public void TestCalculateMonthlyEmissionsForGroup()
        {

            var animalGroup = new AnimalGroup()
            {
                GroupType = AnimalType.DairyHeifers,
            };

            var managementPeriod = new ManagementPeriod()
            {
                //AnimalGroupGuid = tempSwineStarterGroup.Guid,
                AnimalType = AnimalType.DairyHeifers,
                Guid = Guid.NewGuid(),
                NumberOfAnimals = 50,
                Name = "ExampleDairyComponentName",
                Start = Convert.ToDateTime("04/25/1996"),
                StartWeight = 20,
                EndWeight = 30,
                MilkFatContent = 1,
                MilkProduction = 1,
                MilkProteinContentAsPercentage = 1,

                SelectedDiet = new Diet()
                {
                    CrudeProtein = 1,
                    Forage = 1,
                    TotalDigestibleNutrient = 1,
                    Starch = 1,
                    Fat = 1,
                    MetabolizableEnergy = 1,
                    Ndf = 1,
                    MethaneConversionFactor = 1,
                    VolatileSolidsAdjustmentFactorForDiet = 1,
                    NitrogenExcretionAdjustFactorForDiet = 1,
                },

                FeedIntakeAmount = 1,
                GainCoefficient = 1,
                GainCoefficientA = 1,
                GainCoefficientB = 1,
                PeriodDailyGain = 1,

                HousingDetails = new HousingDetails()
                {
                    Guid = Guid.NewGuid(),
                    HousingType = HousingType.Confined,
                    BaselineMaintenanceCoefficient = 1,
                    ActivityCeofficientOfFeedingSituation = 1,
                },

                ManureDetails = new ManureDetails()
                {
                    Guid = Guid.NewGuid(),
                    N2ODirectEmissionFactor = 1,
                    VolatilizationFraction = 1,
                    AshContentOfManure = 1,
                    MethaneConversionFactor = 1
                }

            };

            managementPeriod.End = managementPeriod.Start.AddDays(1);

            animalGroup.ManagementPeriods.Add(managementPeriod);

            var swineComponent = new DairyComponent()
            {
                IsInitialized = true,
                
                Groups = new ObservableCollection<AnimalGroup>()
                {
                   animalGroup
                },
            };

            var animalList = new List<AnimalComponentBase>();
            animalList.Add(swineComponent);

            var roundingDigits = 2;
            //var results = _dairyCattleResultsService.CalculateResultsForAnimalComponents(animalList, new Core.Models.Farm());


            ///First Component Total Emissions/////////
            //Assert.AreEqual(Math.Round(results[0].TotalEntericMethaneEmission, roundingDigits), -153.51);

            //////First Components First Animal Group////////
            //var firstAnimalGroup = results[0].EmissionResultsForAllAnimalGroupsInComponent[0];
            //Assert.AreEqual(results.Count(), 1);
            //Assert.AreEqual(firstAnimalGroup.AnimalGroup.Guid, animalGroup.Guid);
            //Period is only 1 month
            //Assert.AreEqual(firstAnimalGroup.GroupEmissionsByMonths.Count(), 1);


            ///First Animal Groups First Monthly Emission//////
            //var firstMonthlyEmission = firstAnimalGroup.GroupEmissionsByMonths[0];
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.MonthlyEntericMethaneEmission, roundingDigits), -153.51);
        }
       
        /// <summary>
        /// Equation 3.2.1-2
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForMaintenanceReturnsCorrectValue()
        {
            var maintenanceCoefficient = 215.5000;
            var averageWeight = 646.6875;
            var result = _dairyCattleResultsService.CalculateNetEnergyForMaintenance(maintenanceCoefficient, averageWeight);
            Assert.AreEqual(27635.550765520045, result);
        }


        /// <summary>
        /// Equation 3.2.1-3
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForActivityReturnsCorrectValue()
        {
            var feedingActivityCoefficient = 1133.2500;
            var netEnergyForMaintenance = 1280.7500;
            var result = _dairyCattleResultsService.CalculateNetEnergyForActivity(feedingActivityCoefficient, netEnergyForMaintenance);
            Assert.AreEqual(1451409.9375, result);
        }


        /// <summary>
        ///  Equation 3.2.1-4
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForLactationReturnsCorrectValue()
        {
            var milkProduction = 947.6875;
            var fatContent = 300.6250;
            var result = _dairyCattleResultsService.CalculateNetEnergyForLactation(milkProduction, fatContent);
            Assert.AreEqual(115352.52249999999, result);
        }


        /// <summary>
        ///    Equation 3.2.1-5
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForPregnancyReturnsCorrectValue()
        {
            var netEnergyForMaintenance = 617.1875;
            var result = _dairyCattleResultsService.CalculateNetEnergyForPregnancy(netEnergyForMaintenance);
            Assert.AreEqual(61.71875, result);
        }

        /// <summary>
        /// Equation 3.2.1-7
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForGainReturnsCorrectValue()
        {
            var averageWeight = 605.8125;
            var gainCoefficient = 232.0625;
            var finalWeightOfMilkCow = 683.3750;
            var averageDailyGain = 389.5000;
            var result = _dairyCattleResultsService.CalculateNetEnergyForGain(averageWeight, gainCoefficient, finalWeightOfMilkCow,
                                                        averageDailyGain);
            Assert.AreEqual(663.9076, result, 4);
        }


        /// <summary>
        ///    Equation 3.2.1-8
        /// </summary>
        [TestMethod]
        public void CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyReturnsCorrectValue()
        {
            var percentTotalDigestibleNutrientsInFeed = 918.0000;
            var result =
                _dairyCattleResultsService.CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy(
                                                                                              percentTotalDigestibleNutrientsInFeed);
            Assert.AreEqual(6.8279473946840969, result);
        }


        /// <summary>
        ///    Equation 3.2.1-9
        /// </summary>
        [TestMethod]
        public void CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumedReturnsCorrectValue()
        {
            var percentTotalDigestibleNutrientsInFeed = 100.0;
            var result =
                _dairyCattleResultsService.CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed(
                                                                                               percentTotalDigestibleNutrientsInFeed);
            Assert.AreEqual(0.4044, result);
        }


        /// <summary>
        ///  Equation 3.2.1-10
        /// </summary>
        [TestMethod]
        public void CalculateGrossEnergyIntakeReturnsCorrectValue()
        {
            var netEnergyForMaintenance = 1174.4375;
            var netEnergyForActivity = 83.0000;
            var netEnergyForLactation = 1701.7500;
            var netEnergyForPregnancy = 669.1875;
            var netEnergyForGain = 748.8750;
            var ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy = 475.0000;
            var ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed = 1290.0625;
            var percentTotalDigestibleNutrientsInFeed = 417.0625;
            var result = _dairyCattleResultsService.CalculateGrossEnergyIntake(netEnergyForMaintenance, netEnergyForActivity,
                                                         netEnergyForLactation, netEnergyForPregnancy, netEnergyForGain,
                                                         ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy,
                                                         ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed,
                                                         percentTotalDigestibleNutrientsInFeed);
            Assert.AreEqual(1.970730847672991, result);
        }


        /// <summary>
        ///  Equation 3.2.1-11
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionRateReturnsCorrectValue()
        {
            var grossEnergyIntake = 1000;
            var methaneConversionFactor = 200;
            var additiveReductionFactor = 50;
            var result = _dairyCattleResultsService.CalculateEntericMethaneEmissionRate(grossEnergyIntake, methaneConversionFactor,
                                                                  additiveReductionFactor);
            Assert.AreEqual(1796.945193171608265947888589398, result);
        }

        /// <summary>
        ///    Equation 3.2.2-1
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsReturnsCorrectValue()
        {
            var grossEnergyIntake = 1072.8125;
            var percentDigestibleEnergyInFeed = 2000.0625;
            var ashContentOfFeed = 349.0625;
            var result =
                _dairyCattleResultsService.CalculateVolatileSolids(grossEnergyIntake, percentDigestibleEnergyInFeed, ashContentOfFeed);
            Assert.AreEqual(2745.9235805769927, result);
        }


        /// <summary>
        /// Equation 3.2.2-2
        /// </summary>
        [TestMethod]
        public void CalculateManureMethaneEmissionRateReturnsCorrectValue()
        {
            var volatileSolids = 325.0625;
            var methaneProducingCapacity = 426.1875;
            var methaneConversionFactor = 122.3125;
            var result =
                _dairyCattleResultsService.CalculateManureMethaneEmissionRate(volatileSolids, methaneProducingCapacity,
                                                        methaneConversionFactor);
            Assert.AreEqual(11353067.62124267578125, result);
        }

        /// <summary>
        ///    Equation 3.2.3-1
        /// </summary>
        [TestMethod]
        public void CalculateProteinIntakeReturnsCorrectValue()
        {
            var grossEnergyIntake = 513.1250;
            var conversionFactorForGrossEnergyIntake = 681.1250;
            var result = _dairyCattleResultsService.CalculateProteinIntake(grossEnergyIntake, conversionFactorForGrossEnergyIntake);
            Assert.AreEqual(18943.21222899728997289972899729, result);
        }


        /// <summary>
        /// Equation 3.2.3-2
        /// </summary>
        [TestMethod]
        public void CalculateProteinRetainedForPregnancyReturnsCorrectValue()
        {
            var result = _dairyCattleResultsService.CalculateProteinRetainedForPregnancy();
            Assert.AreEqual(0.01369863013698630136986301369863, result);
        }


        /// <summary>
        /// Equation 3.2.3-4
        /// </summary>
        [TestMethod]
        public void CalculateEmptyBodyWeightReturnsCorrectValue()
        {
            var averageWeight = 653.4375;
            var result = _dairyCattleResultsService.CalculateEmptyBodyWeight(averageWeight);
            Assert.AreEqual(582.2128125, result);
        }


        /// <summary>
        /// Equation 3.2.3-5
        /// </summary>
        [TestMethod]
        public void CalculateEmptyBodyGainReturnsCorrectValue()
        {
            var averageDailyGain = 955.5625;
            var result = _dairyCattleResultsService.CalculateEmptyBodyGain(averageDailyGain);
            Assert.AreEqual(913.51775, result);
        }


        /// <summary>
        /// Equation 3.2.3-6
        /// </summary>
        [TestMethod]
        public void CalculateRetainedEnergyReturnsCorrectValue()
        {
            var emptyBodyWeight = 99.5000;
            var emptyBodyGain = 1081.7500;
            var result = _dairyCattleResultsService.CalculateRetainedEnergy(emptyBodyWeight, emptyBodyGain);
            Assert.AreEqual(4261.6546140976652, result);
        }


        /// <summary>
        ///  Equation 3.2.3-7
        /// </summary>
        [TestMethod]
        public void CalculateProteinRetainedForGainReturnsCorrectValue()
        {
            var averageDailyGain = 823.9375;
            var retainedEnergy = 1632.9375;
            var result = _dairyCattleResultsService.CalculateProteinRetainedForGain(averageDailyGain, retainedEnergy);
            Assert.AreEqual(172.80688750000002, result);
        }


        /// <summary>
        /// Equation 3.2.3-8
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenExcretionRateReturnsCorrectValue()
        {
            var proteinIntake = 597.0625;
            var proteinRetainedForPregnancy = 700.3750;
            var proteinRetainedForLactation = 266.5625;
            var proteinRetainedForGain = 132.6250;
            var result = _dairyCattleResultsService.CalculateNitrogenExcretionRate(proteinIntake, proteinRetainedForPregnancy,
                                                             proteinRetainedForLactation, proteinRetainedForGain);
            Assert.AreEqual(-79.530956112852664, result);
        }


        /// <summary>
        /// Equation 3.2.3-9
        /// </summary>
        [TestMethod]
        public void CalculateManureDirectNitrogenEmissionRateReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 183.8125;
            var emissionFactor = 415.6875;
            var result = _dairyCattleResultsService.CalculateManureDirectNitrogenEmissionRate(nitrogenExcretionRate, emissionFactor);
            Assert.AreEqual(76408.55859375, result);
        }

        [TestMethod]
        public void CalculateManureDirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureDirectNitrogenEmissionRate = 149.7500;
            var numberOfCattle = 471.5625;
            var result = _dairyCattleResultsService.CalculateManureDirectNitrogenEmission(manureDirectNitrogenEmissionRate, numberOfCattle);
            Assert.AreEqual(numberOfCattle * manureDirectNitrogenEmissionRate, result);
        }

        [TestMethod]
        public void CalculateManureVolatilizationNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmissionRate = 1221.5625;
            var numberOfCattle = 852.3750;
            var result = _dairyCattleResultsService.CalculateManureVolatilizationNitrogenEmission(manureVolatilizationNitrogenEmissionRate,
                                                                            numberOfCattle);
            Assert.AreEqual(manureVolatilizationNitrogenEmissionRate * numberOfCattle, result);
        }

        /// <summary>
        ///  Equation 3.2.4-3
        /// </summary>
        [TestMethod]
        public void CalculateManureLeachingNitrogenEmissionRateReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 723.3125;
            var leachingFraction = 210.6875;
            var emissionFactorForLeaching = 993.6250;
            var result = _dairyCattleResultsService.CalculateManureLeachingNitrogenEmissionRate(nitrogenExcretionRate, leachingFraction,
                                                                          emissionFactorForLeaching, 0);
            Assert.AreEqual(151421397.59130859375, result);
        }


        [TestMethod]
        public void CalculateManureLeachingNitrogenEmissionReturnsCorrectValue()
        {
            var manureLeachingNitrogenEmissionRate = 893.8750;
            var numberOfCattle = 619.2500;
            var result =
                _dairyCattleResultsService.CalculateManureLeachingNitrogenEmission(manureLeachingNitrogenEmissionRate, numberOfCattle);
            Assert.AreEqual(numberOfCattle * manureLeachingNitrogenEmissionRate, result);
        }


        /// <summary>
        ///    Equation 3.2.4-5
        /// </summary>
        [TestMethod]
        public void CalculateManureIndirectNitrogenEmissionReturnsCorrectValue()
        {
            var manureVolatilizationNitrogenEmission = 826.0625;
            var manureLeachingNitrogenEmission = 1288.5000;
            var result = _dairyCattleResultsService.CalculateManureIndirectNitrogenEmission(manureVolatilizationNitrogenEmission,
                                                                      manureLeachingNitrogenEmission);
            Assert.AreEqual(2114.5625, result);
        }

        /// <summary>
        ///    Equation 3.2.4-6
        /// </summary>
        [TestMethod]
        public void CalculateManureNitrogenEmissionReturnsCorrectValue()
        {
            var manureDirectNitrogenEmission = 1563.163;
            var manureIndirectNitrogenEmission = 2114.5625;
            var result = _dairyCattleResultsService.CalculateManureNitrogenEmission(manureDirectNitrogenEmission,
                                                                      manureIndirectNitrogenEmission);
            Assert.AreEqual(3677.7255, result);
        }


        /// <summary>
        /// Equation 3.2.4-7
        /// </summary>
        [TestMethod]
        public void CalculateManureAvailableForLandApplicationReturnsCorrectValue()
        {
            var nitrogenExcretionRate = 101.2500;
            var numberOfCattle = 1017.5625;
            var numberOfDays = 112.0625;
            var volatilizationFraction = 456.6250;
            var leachingFraction = 230.8750;
            var result = _dairyCattleResultsService.CalculateManureAvailableForLandApplication(nitrogenExcretionRate, numberOfCattle,
                                                                         numberOfDays, volatilizationFraction, leachingFraction);
            Assert.AreEqual(-7926053035.71533203125, result);
        }


        /// <summary>
        /// Equation 3.2.5-1
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionForCalvesReturnsCorrectValue()
        {
            var result = _dairyCattleResultsService.CalculateEntericMethaneEmissionForCalves();
            Assert.AreEqual(0.0, result);
        }


        /// <summary>
        ///    Equation 3.2.6-1
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsForCalvesReturnsCorrectValue()
        {
            var result = _dairyCattleResultsService.CalculateVolatileSolidsForCalves();
            Assert.AreEqual(1.42, result);
        }


        /// <summary>
        ///    Equation 3.2.7-1
        /// </summary>
        [TestMethod]
        public void CalculateNitrogenExcretionRateForCalvesReturnsCorrectValue()
        {
            var result = _dairyCattleResultsService.CalculateNitrogenExcretionRateForCalves(20, 10);
            Assert.AreEqual((20/6.25) - (10/6.25), result);
        }

        /// <summary>
        /// Equation 9.4-1
        /// </summary>
        [TestMethod]
        public void CalculateMilkProductionPerMonthFromDairyCattle()
        {
            var result = _dairyCattleResultsService.CalculateMilkProductionPerMonthFromDairyCattle(123.43, 65.33, 0.65);
            Assert.AreEqual(5241.393235, result);
        }

        /// <summary>
        /// Equation 9.4-2
        /// </summary>
        [TestMethod]
        public void FatAndProteinCorrectedMilkProductionPerMonth()
        {
            var result = _dairyCattleResultsService.FatAndProteinCorrectedMilkProductionPerMonth(23.45, 0.567, 43);
            Assert.AreEqual(5.20556399, result, 0.000000001);
        }

        /// <summary>
        /// Equation 9.3-1
        /// </summary>
        [TestMethod]
        public void CalculateDryMatterIntake()
        {
            var result = _dairyCattleResultsService.CalculateDryMatterIntake(54.74);
            Assert.AreEqual(2.9669376693766937669376693766938, result);
        }

        /// <summary>
        ///  Equation 4.2.1-1
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromDairyOperationsReturnsCorrectValue()
        {
            var numberOfLactatingDairyCows = 14;
            var dairyCowConversion = 968;
            var electricityConversion = 160.875;
            var numberOfDaysInMonth = 30;
            var result = _dairyCattleResultsService.CalculateTotalCarbonDioxideEmissionsFromDairyHousing(numberOfLactatingDairyCows,
                numberOfDaysInMonth, electricityConversion);
            Assert.AreEqual(numberOfLactatingDairyCows * (dairyCowConversion / CoreConstants.DaysInYear) * numberOfDaysInMonth * electricityConversion, result);
        }

        #endregion
    }
}
