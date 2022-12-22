using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Tillage;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Infrastructure;

namespace H.Core.Test.Calculators.Infrastructure
{
    [TestClass]
    public class ADCalculatorTest
    {
        #region Fields

        private ADCalculator _sut;
        private Farm _farm;
        private List<AnimalComponentEmissionsResults> _animalComponentResults;
        private GroupEmissionsByDay _day1Emissions;
        private GroupEmissionsByDay _day2Emissions;
        private AnaerobicDigestionComponent _component;
        private ManagementPeriod _managementPeriod1;
        private ManagementPeriod _managementPeriod2;

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
            _sut = new ADCalculator();

            _farm = new Farm();

            var residuesSubstrateViewItem = new FarmResiduesSubstrateViewItem() { FarmResidueType = FarmResidueType.BarleyStraw };
            residuesSubstrateViewItem.FlowRate = 0.5;
            residuesSubstrateViewItem.TotalSolids = 0.5;
            residuesSubstrateViewItem.VolatileSolids = 0.1;
            residuesSubstrateViewItem.TotalNitrogen = 0.5;
            residuesSubstrateViewItem.TotalCarbon = 0.5;
            residuesSubstrateViewItem.BiomethanePotential = 0.5;
            residuesSubstrateViewItem.MethaneFraction = 0.5;

            var manureSubstrateViewItem = new ManureSubstrateViewItem() { AnimalType = AnimalType.Beef, IsFreshManure = true };
            manureSubstrateViewItem.FlowRate = 1.25;
            manureSubstrateViewItem.TotalSolids = 0.75;
            manureSubstrateViewItem.VolatileSolids = 0.1;
            manureSubstrateViewItem.TotalNitrogen = 0.33;
            manureSubstrateViewItem.TotalCarbon = 0.11;
            manureSubstrateViewItem.BiomethanePotential = 0.22;
            manureSubstrateViewItem.MethaneFraction = 2;

            _component = new AnaerobicDigestionComponent();
            _component.HydraulicRetentionTimeInDays = 30;
            _component.ProportionAsPercentageTotalManureAddedToAd = 0.5;
            _component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(residuesSubstrateViewItem);
            _component.AnaerobicDigestionViewItem.ManureSubstrateViewItems.Add(manureSubstrateViewItem);

            _farm.Components.Add(_component);

            _managementPeriod1 = new ManagementPeriod();
            _managementPeriod1.AnimalType = AnimalType.Beef;
            _managementPeriod1.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

            _managementPeriod2 = new ManagementPeriod();
            _managementPeriod2.AnimalType = AnimalType.Dairy;
            _managementPeriod2.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

            var dailyEmissions = new List<GroupEmissionsByDay>()
            {
            };

            _day1Emissions = new GroupEmissionsByDay();
            _day1Emissions.TotalVolumeOfManureAvailableForLandApplication = 100;
            _day1Emissions.VolatileSolids = 0.01;
            _day1Emissions.AmountOfNitrogenExcreted = 10;
            _day1Emissions.AmountOfNitrogenAddedFromBedding = 10;
            _day1Emissions.OrganicNitrogenInStoredManure = 20;
            _day1Emissions.TanExcretion = 10;
            _day1Emissions.CarbonFromManureAndBedding = 12;

            _day2Emissions = new GroupEmissionsByDay();
            _day2Emissions.TotalVolumeOfManureAvailableForLandApplication = 50;
            _day2Emissions.VolatileSolids = 0.02;
            _day2Emissions.AmountOfNitrogenExcreted = 20;
            _day2Emissions.AmountOfNitrogenAddedFromBedding = 20;
            _day2Emissions.OrganicNitrogenInStoredManure = 30;
            _day2Emissions.TanExcretion = 20;
            _day2Emissions.CarbonFromManureAndBedding = 16;

            dailyEmissions.Add(_day1Emissions); 
            dailyEmissions.Add(_day2Emissions);

            _animalComponentResults = new List<AnimalComponentEmissionsResults>();

            var beefComponentEmissionResults = new AnimalComponentEmissionsResults()
            {
                EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>()
                {
                    new AnimalGroupEmissionResults()
                    {
                        GroupEmissionsByMonths = new List<GroupEmissionsByMonth>()
                        {
                            new GroupEmissionsByMonth(new MonthsAndDaysData() {ManagementPeriod = _managementPeriod1}, dailyEmissions)
                        }
                    }
                }
            };

            var dairyComponentEmissionResults = new AnimalComponentEmissionsResults()
            {
                EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>()
                {
                    new AnimalGroupEmissionResults()
                    {
                        GroupEmissionsByMonths = new List<GroupEmissionsByMonth>()
                        {
                            new GroupEmissionsByMonth(new MonthsAndDaysData() {ManagementPeriod = _managementPeriod2}, dailyEmissions)
                        }
                    }
                }
            };

            _animalComponentResults.Add(beefComponentEmissionResults);
            _animalComponentResults.Add(dairyComponentEmissionResults);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void TestMethod1()
        {
            var farm = new Farm();
            farm.Components.Add(_component);

            var groupEmissionsByDay = new GroupEmissionsByDay();
            groupEmissionsByDay.TotalVolumeOfManureAvailableForLandApplication = 100;
            groupEmissionsByDay.VolatileSolids = 0.01;
            groupEmissionsByDay.AmountOfNitrogenExcreted = 10;
            groupEmissionsByDay.AmountOfNitrogenAddedFromBedding = 10;
            groupEmissionsByDay.OrganicNitrogenInStoredManure = 20;
            groupEmissionsByDay.TanExcretion = 10;
            groupEmissionsByDay.CarbonFromManureAndBedding = 12;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.ManureDetails.DailyTanExcretion = 0.25;

            _sut.CalculateResults(farm, groupEmissionsByDay, managementPeriod);
        }

        [TestMethod]
        public void CalculateResultsDoesNotCalculateResultsWhenManureHandlingSystemIsNotSetToAD()
        {
            var farm = new Farm();
            farm.Components.Add(_component);

            var groupEmissionsByDay = new GroupEmissionsByDay();
            groupEmissionsByDay.TotalVolumeOfManureAvailableForLandApplication = 100;
            groupEmissionsByDay.VolatileSolids = 0.01;
            groupEmissionsByDay.AmountOfNitrogenExcreted = 10;
            groupEmissionsByDay.AmountOfNitrogenAddedFromBedding = 10;
            groupEmissionsByDay.OrganicNitrogenInStoredManure = 20;
            groupEmissionsByDay.TanExcretion = 10;
            groupEmissionsByDay.CarbonFromManureAndBedding = 12;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.ManureDetails.DailyTanExcretion = 0.25;
            managementPeriod.ManureDetails.StateType = ManureStateType.DeepPit;

            var result = _sut.CalculateResults(farm, groupEmissionsByDay, managementPeriod);

            Assert.AreEqual(0, result.FlowRateOfAllSubstrates);
        }

        [TestMethod]
        public void CalculateResultsWhenManureHandlingSystemIsADCalculatesFlow()
        {
            var farm = new Farm();
            farm.Components.Add(_component);

            var groupEmissionsByDay = new GroupEmissionsByDay();
            groupEmissionsByDay.TotalVolumeOfManureAvailableForLandApplication = 100;
            groupEmissionsByDay.VolatileSolids = 0.01;
            groupEmissionsByDay.AmountOfNitrogenExcreted = 10;
            groupEmissionsByDay.AmountOfNitrogenAddedFromBedding = 10;
            groupEmissionsByDay.OrganicNitrogenInStoredManure = 20;
            groupEmissionsByDay.TanExcretion = 10;
            groupEmissionsByDay.CarbonFromManureAndBedding = 12;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.ManureDetails.DailyTanExcretion = 0.25;
            managementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

            var result = _sut.CalculateResults(farm, groupEmissionsByDay, managementPeriod);

            Assert.IsTrue(result.FlowRateOfAllSubstrates > 0);
        }

        [TestMethod]
        public void CalculateResultsTestTotalsOutputsForAllDays()
        {
            var results = _sut.CalculateResults(_farm, _animalComponentResults);
        }

        [TestMethod]
        public void TestCalculateResults()
        {
            var results = _sut.CalculateResults(_farm, _animalComponentResults);

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void CombineSubstrateFlowsOfSameTypeOnSameDayReturnsCorectNumberOfItems()
        {
            var substrateFlows = new List<SubstrateFlowInformation>()
            {
                new SubstrateFlowInformation() {DateCreated = DateTime.Now},
                new SubstrateFlowInformation() {DateCreated = DateTime.Now},
                new SubstrateFlowInformation() {DateCreated = DateTime.Now.AddDays(23)},
            };

            var result = _sut.CombineSubstrateFlowsOfSameTypeOnSameDay(substrateFlows);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetFreshManureFlowRatesFromAnimals()
        {
            var results = _sut.GetFreshManureFlowRateFromAnimals(_component, _day1Emissions, _managementPeriod1);

            Assert.AreEqual(0.033734693877551, results.CarbonFlowInDigestate, 0.00001);
        }

        [TestMethod]
        public void GetStoredManureFlowRateFromAnimalsTest()
        {
            // here 
            var results = _sut.GetStoredManureFlowRateFromAnimals(_component, _day1Emissions, _managementPeriod1);

            Assert.AreEqual(0.0437755102040816, results.DegradedVolatileSolids,0.00001);
        }

        [TestMethod]
        public void TestCalculateResultsFromTwoSeparateMangementPeriodsCombinesFlows()
        {
            var results = _sut.CalculateResults_NEW(_farm, _animalComponentResults);
        }

        [TestMethod]
        public void GetFlowsFromDailyResultsReturnsCorrectNumberOfItems()
        {
            var results = _sut.GetFlowsFromDailyResults(_farm, _animalComponentResults, _component);

            // 2 daily emissions from the beef results, 2 from the dairy results
            Assert.AreEqual(4, results.Count);
        }

        #endregion
    }
}
