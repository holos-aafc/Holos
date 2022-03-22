#region Imports

using System;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Providers;
using H.Core.Providers.Feed;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace H.Core.Test.Services
{
    [TestClass]
    public class BeefCattleResultsServiceTest
    {
        #region Fields

        private BeefCattleResultsService _resultsService;

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
            _resultsService = new BeefCattleResultsService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        #region CalculateResultsForComponentTest

        [TestMethod]
        public void CalculateResultsForComponentReturnsNonEmptyListOfAnimalGroupEmissionResults()
        {
            var managementPeriod = new ManagementPeriod();
            managementPeriod.Start = new DateTime(2019, 1, 1);
            managementPeriod.End = managementPeriod.Start.AddDays(90);
            managementPeriod.SelectedDiet = new Diet();

            var cowGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCow,
                ManagementPeriods = new ObservableCollection<ManagementPeriod>() {managementPeriod},
            };

            var cowCalfComponent = new CowCalfComponent()
            {
                IsInitialized = true,
                ResultsCalculated = false,
                Groups = new ObservableCollection<AnimalGroup>() {cowGroup}
            };

            var farm = new Farm();

            var result = _resultsService.CalculateResultsForComponent(
                animalComponent: cowCalfComponent, 
                farm: farm);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void CalculateResultsForComponentReturnsNonEmptyListOfGroupEmissionsByMonth()
        {
            var managementPeriod = new ManagementPeriod();
            managementPeriod.Start = new DateTime(2019, 1, 1);
            managementPeriod.End = managementPeriod.Start.AddDays(89);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.SelectedDiet = new Diet();

            var cowGroup = new AnimalGroup()
            {
                ManagementPeriods = new ObservableCollection<ManagementPeriod>() { managementPeriod },
            };

            var cowCalfComponent = new CowCalfComponent()
            {
                IsInitialized = true,
                ResultsCalculated = false,
                Groups = new ObservableCollection<AnimalGroup>() { cowGroup }
            };

            var farm = new Farm();

            var componentEmissionResults = _resultsService.CalculateResultsForComponent(
                animalComponent: cowCalfComponent,
                farm: farm);

            var componentEmissionResult = componentEmissionResults.Single();
            var results = componentEmissionResult.GroupEmissionsByMonths;

            Assert.AreEqual(3, results.Count);
        }

        #endregion

        #region CalculateEntericMethaneForBeefCattleGroup Tests

        [TestMethod]
        public void CalculateEntericMethaneForBeefCattleGroup()
        {
            var managementPeriod = new ManagementPeriod
            {
                DietAdditive = DietAdditiveType.FourPercentFat,
                AnimalType = AnimalType.BeefFinishingHeifer,
                NumberOfAnimals = 100,
                Name = "BeefFinishingHeiferGroup1Period1",
                Start = new DateTime(1996, 4, 25),
                FeedIntakeAmount = 1,
                StartWeight = 20,
                EndWeight = 50,
                GainCoefficient = 1,

                SelectedDiet = new Diet()
                {
                    CrudeProtein = 1,
                    Forage = 1,
                    TotalDigestibleNutrient = 1,
                    Starch = 1,
                    VolatileSolidsAdjustmentFactorForDiet = 1,
                    Fat = 1,
                    MetabolizableEnergy = 1,
                    Ndf = 1,
                    NitrogenExcretionAdjustFactorForDiet = 1,
                    MethaneConversionFactor = 1,
                },

                HousingDetails = new HousingDetails()
                {
                    HousingType = HousingType.ConfinedNoBarn,
                    BaselineMaintenanceCoefficient = 1,
                    ActivityCeofficientOfFeedingSituation = 1,
                },

                ManureDetails = new ManureDetails()
                {
                    StateType = ManureStateType.CompostIntensive,
                    N2ODirectEmissionFactor = 1,
                    VolatilizationFraction = 1,
                    AshContentOfManure = 1,
                    MethaneConversionFactor = 1,
                    VolatileSolidExcretion = 1,
                    YearlyEntericMethaneRate = 1,
                    MethaneProducingCapacityOfManure = 1,
                }
            };

            managementPeriod.End = managementPeriod.Start.AddDays(60);

            var beefGroup = new AnimalGroup()
            {
                Name = "BeefFinishingHeiferGroup1",
                GroupType = AnimalType.BeefFinishingHeifer,
                ManagementPeriods = new ObservableCollection<ManagementPeriod>() { managementPeriod}
            };

            var storage = new Storage()
            {
                ApplicationData = new ApplicationData(),
            };
            var geographicData = new GeographicData();            
            storage.ApplicationData.GlobalSettings.ActiveFarm = new Farm();
            storage.ApplicationData.GlobalSettings.ActiveFarm.Defaults = new Defaults();
            storage.ApplicationData.GlobalSettings.ActiveFarm.GeographicData = geographicData;
            
            var beefHelper = new BeefCattleResultsService(); 
            //var result = beefHelper.GetEmissionsForGroup(beefGroup, storage.ApplicationData.GlobalSettings.ActiveFarm);
          //  Assert.AreEqual(result.AnimalGroup.Guid, beefGroup.Guid);
            //var groupMonthlyEmissions = result.GroupEmissionsByMonths;

            //Duration is three months => 3 monthly emissions for the beef group
            //Assert.AreEqual(groupMonthlyEmissions.Count(), 3);

            //Get first monthly emission
           // var firstMonthlyEmission = groupMonthlyEmissions[0];

            // First Monthly Emission Calculations
            var roundingDigits = 2;
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.EntericMethaneEmission, roundingDigits), -956.88);
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.ManureDirectN2ONEmission, roundingDigits), -4.68);
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.ManureIndirectN2ONEmission, roundingDigits), -4.68);
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.ManureNitrogenEmission, roundingDigits), -9.36);
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.ManureMethaneEmission, roundingDigits), -1991.76);
            ////Leaching Fraction is 0 for a Beef Heifer therefore, the calculated value will be multiplied by 0
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.ManureLeachingNitrogenEmission, roundingDigits), 0);
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.ManureVolatilizationNitrogenEmission, roundingDigits), -4.68);
       
            ////Leaching Fraction is 0 for a Beef Heifer therefore, the calculated value will be multiplied by 0
            //Assert.AreEqual(Math.Round(firstMonthlyEmission.ManureAvailableForLandApplication, roundingDigits), 0);
        }

        #endregion

        #region Equations

        /// <summary>
        /// Equation 3.1.1-2
        /// </summary>
        [TestMethod]
        public void CalculateMaintenanceCoefficientReturnsCorrectValue()
        {
            var baselineMaintenanceCoefficient = 630.5625;
            var averageMonthlyTemperature = 899.5000;
            var result =
                _resultsService.CalculateTemperatureAdjustedMaintenanceCoefficient(baselineMaintenanceCoefficient, averageMonthlyTemperature);
            Assert.AreEqual(626.3409, result);
        }


        /// <summary>
        /// Equation 3.1.1-3
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForMaintenanceReturnsCorrectValue()
        {
            var maintenanceCoefficient = 683.2500;
            var averageWeight = 508.8125;
            var result = _resultsService.CalculateNetEnergyForMaintenance(maintenanceCoefficient, averageWeight);
            Assert.AreEqual(73197.79007846931954717544435093071236136869444175760877411, result);
        }


        /// <summary>
        /// Equation 3.1.1-4
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForActivityReturnsCorrectValue()
        {
            var feedingActivityCoefficient = 827.4375;
            var netEnergyForMaintenance = 1022.8750;
            var result = _resultsService.CalculateNetEnergyForActivity(feedingActivityCoefficient, netEnergyForMaintenance);
            Assert.AreEqual(846365.1328125, result);
        }


        /// <summary>
        /// Equation 3.1.1-5
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForLactationReturnsCorrectValue()
        {
            var milkProduction = 399.7500;
            var fatContent = 908.1250;
            var numberOfCalves = -748.9375;
            var numberOfCows = -682.5000;
            var result = _resultsService.CalculateNetEnergyForLactation(milkProduction, fatContent, numberOfCalves, numberOfCows);
            Assert.AreEqual(159989.31264285717, result);
        }


        /// <summary>
        /// Equation 3.1.1-6
        /// </summary>
        [TestMethod]
        public void CalculateNetEnergyForPregnancyReturnsCorrectValue()
        {
            var netEnergyForMaintenance = 235.8125;
            var result = _resultsService.CalculateNetEnergyForPregnancy(netEnergyForMaintenance);
            Assert.AreEqual(23.58125, result);
        }


        /// <summary>
        /// Equation 3.1.1-7
        /// </summary>
        [TestMethod]
        public void CalculateAverageDailyWeightGainReturnsCorrectValue()
        {
            var initialWeight = 1051.2500;
            var finalWeight = -123.6875;
            var numberOfDays = -891.5625;
            var result = _resultsService.CalculateAverageDailyWeightGain(initialWeight, finalWeight, numberOfDays);
            Assert.AreEqual(1.3178408692604276200490711531721, result);
        }


        /// <summary>
        /// Equation 3.1.1-9
        /// </summary>
        [TestMethod]
        public void CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyReturnsCorrectValue()
        {
            var percentTotalDigestibleNutrientsInFeed = 1144.9375;
            var result =
                _resultsService.CalculateRatioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergy(
                                                                                              percentTotalDigestibleNutrientsInFeed);
            var duplicate = 1.123 - 0.004092 * percentTotalDigestibleNutrientsInFeed +
                            0.00001126 * percentTotalDigestibleNutrientsInFeed * percentTotalDigestibleNutrientsInFeed -
                            25.4 / percentTotalDigestibleNutrientsInFeed;
            Assert.AreEqual(duplicate, result);
        }


        /// <summary>
        /// Equation 3.1.1-10
        /// </summary>
        [TestMethod]
        public void CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumedReturnsCorrectValue()
        {
            var percentTotalDigestibleNutrientsInFeed = 2.0;
            var result =
                _resultsService.CalculateRatioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed(
                                                                                               percentTotalDigestibleNutrientsInFeed);
            Assert.AreEqual(-17.56626768, result);
        }


        /// <summary>
        /// Equation 3.1.1-11
        /// </summary>
        [TestMethod]
        public void CalculateGrossEnergyIntakeReturnsCorrectValue()
        {
            var netEnergyForMaintainance = 583.6875;
            var netEnergyForActivity = 1568.4375;
            var netEnergyForLactation = -244.8125;
            var netEnergyForPregnancy = 921.9375;
            var netEnergyForGain = 1024.4375;
            var ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed = -87.4375;
            var ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed = -1107.6875;
            var percentTotalDigestibleNutrientsInFeed = 863.0000;
            var result = _resultsService.CalculateGrossEnergyIntake(netEnergyForMaintainance, netEnergyForActivity,
                                                         netEnergyForLactation, netEnergyForPregnancy, netEnergyForGain,
                                                         ratioOfNetEnergyAvailableInDietForMaintenanceToDigestibleEnergyConsumed,
                                                         ratioOfNetEnergyAvailableInDietForGainToDigestibleEnergyConsumed,
                                                         percentTotalDigestibleNutrientsInFeed);
            Assert.AreEqual(-3.8565749206638751, result);
        }


        /// <summary>
        /// Equation 3.1.1-12
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionRateReturnsCorrectValue()
        {
            var grossEnergyIntake = -672.1875;
            var methaneConversionFactor = -1152.9375;
            var additiveReductionFactor = -494.5625;
            var result = _resultsService.CalculateEntericMethaneEmissionRate(grossEnergyIntake, methaneConversionFactor,
                                                                  additiveReductionFactor);
            Assert.AreEqual(82799.6579, result, 4);
        }


        /// <summary>
        /// Equation 3.1.1-13
        /// </summary>
        [TestMethod]
        public void CalculateEntericMethaneEmissionsReturnsCorrectValue()
        {
            var entericMethaneEmissionRate = 10;
            var numberOfCattle = 30;
            
            var result =
                _resultsService.CalculateEntericMethaneEmissions(entericMethaneEmissionRate, numberOfCattle);
            Assert.AreEqual(300, result);
        }

        /// <summary>
        /// Equation 3.1.5-3
        /// </summary>
        [TestMethod]
        public void CalculateCalfGrossEnergyIntakeReturnsCorrectValue()
        {
            var dryMatterIntake = -629.6875;
            var result = _resultsService.CalculateCalfGrossEnergyIntake(dryMatterIntake);
            Assert.AreEqual(-11617.734375, result);
        }


        /// <summary>
        /// Equation 3.1.7-1
        /// </summary>
        [TestMethod]
        public void CalculateCalfProteinIntakeFromSolidFoodReturnsCorrectValue()
        {
            var dryMatterIntake = -635.5000;
            var crudeProteinContent = -1019.3125;
            var result = _resultsService.CalculateCalfProteinIntakeFromSolidFood(dryMatterIntake, crudeProteinContent);
            Assert.AreEqual(647773.09375, result);
        }


        /// <summary>
        /// Equation 3.1.7-2
        /// </summary>
        [TestMethod]
        public void CalculateCalfProteinIntakeFromMilkReturnsCorrectValue()
        {
            var milkProduction = 1661.3125;
            var proteinContentOfMilk = -763.8750;
            var result = _resultsService.CalculateCalfProteinIntakeFromMilk(milkProduction, proteinContentOfMilk);
            Assert.AreEqual(-1269035.0859375, result);
        }

        /// <summary>
        /// Equation 3.1.7-3
        /// </summary>
        [TestMethod]
        public void CalculateCalfProteinIntakeReturnsCorrectValue()
        {
            var calfProteinIntakeFromMilk = -8.5625;
            var calfProteinIntakeFromSolidFood = 1799.3125;
            var result = _resultsService.CalculateCalfProteinIntake(calfProteinIntakeFromMilk, calfProteinIntakeFromSolidFood);
            Assert.AreEqual(1790.75, result);
        }


        /// <summary>
        /// Equation 3.1.7-4
        /// </summary>
        [TestMethod]
        public void CalculateCalfProteinRetainedFromSolidFeedReturnsCorrectValue()
        {
            var calfProteinIntakeFromSolidFood = 1743.5625;
            var result = _resultsService.CalculateCalfProteinRetainedFromSolidFeed(calfProteinIntakeFromSolidFood);
            Assert.AreEqual(348.71250000000003, result);
        }

        /// <summary>
        /// Equation 3.1.7-5
        /// </summary>
        [TestMethod]
        public void CalculateCalfProteinRetainedFromMilkReturnsCorrectValue()
        {
            var calfProteinIntakeFromMilk = 889.3125;
            var result = _resultsService.CalculateCalfProteinRetainedFromMilk(calfProteinIntakeFromMilk);
            Assert.AreEqual(355.725, result);
        }


        /// <summary>
        ///    Equation 3.1.7-6
        /// </summary>
        [TestMethod]
        public void CalculateCalfProteinRetainedReturnsCorrectValue()
        {
            var calfProteinRetainedFromMilk = 689.3125;
            var calfProteinRetainedFromSolidFeed = 394.3750;
            var result =
                _resultsService.CalculateCalfProteinRetained(calfProteinRetainedFromMilk, calfProteinRetainedFromSolidFeed);
            Assert.AreEqual(1083.6875, result);
        }


        /// <summary>
        ///  Equation 3.1.7-7
        /// </summary>
        [TestMethod]
        public void CalculateCalfNitrogenExcretionRateReturnsCorrectValue()
        {
            var calfProteinIntake = -71.9375;
            var calfProteinRetained = -806.6250;
            var result = _resultsService.CalculateCalfNitrogenExcretionRate(calfProteinIntake, calfProteinRetained);
            Assert.AreEqual(117.55, result);
        }

        /// <summary>
        ///  Equation 9.3-1
        /// </summary>
        [TestMethod]
        public void CalculateDryMatterIntake()
        {
            var result = _resultsService.CalculateDryMatterIntake(54.74);
            Assert.AreEqual(2.9669376693766937669376693766938, result);
        }

        /// <summary>
        /// Equation 4.2.4-1
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromHousedBeefOperationsReturnsCorrectValue()
        {
            var numberOfCattle = 40;
            var housedBeefConversion = 65.7;
            var electricityConversion = 219.875;
            var numberOfDaysInMonth = 30;
            var result = _resultsService.CalculateTotalCarbonDioxideEmissionsFromHousedBeefOperations(numberOfCattle,
                numberOfDaysInMonth, electricityConversion);
            Assert.AreEqual(numberOfCattle * (housedBeefConversion / CoreConstants.DaysInYear) * numberOfDaysInMonth * electricityConversion, result);
        }

        /// <summary>
        /// Equation 4.3.1-4
        /// Equation 4.3.1-5
        /// </summary>
        [TestMethod]
        public void CalculateVolumeOfSolidManureReturnsCorrectValue()
        {
            var totalNitrogenFromSolidAppliedLandManure = 273.625;
            var nitrogenConcentrationOfSolidManureBasedOnAnimalType = 79.125;
            var result = _resultsService.CalculateTotalVolumeManure(totalNitrogenFromSolidAppliedLandManure,
                nitrogenConcentrationOfSolidManureBasedOnAnimalType);
            Assert.AreEqual(3.4581358609794628751974723538705, result);
        }

        [TestMethod]
        public void CalculateDryMatterMax()
        {
            var result = _resultsService.CalculateDryMatterMax(
                AnimalType.BeefCowLactating, 100);

            Assert.AreEqual(2.5875, result);
        }

        [TestMethod]
        public void GetCurrentAnimalWeight()
        {
            var startWeight = 300;
            var averageDailyGain = 1;
            var startDate = new DateTime(2022, 1, 1);
            var currentDate = new DateTime(2022, 1, 10);

            var result = _resultsService.GetCurrentAnimalWeight(
                startWeight: startWeight,
                averageDailyGain: averageDailyGain,
                startDate: startDate,
                currentDate: currentDate);

            Assert.AreEqual(310, result);
        }

        [TestMethod]
        public void CalculateRequiredTdnSoThatMaxDmiIsNotExceeded()
        {

        }

        #endregion

        #endregion
    }
}