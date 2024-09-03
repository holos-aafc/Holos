using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Sheep;
using H.Core.Emissions.Results;
using System.Collections.Generic;
using System.Linq;
using H.Core.Calculators.Infrastructure;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Climate;
using Moq;

namespace H.Core.Test.Services.Animals
{
    [TestClass]
    public class DigestateServiceTest : UnitTestBase
    {
        #region Fields

        private DigestateService _sut;
        private Mock<IADCalculator> _mockAdCalculator;
        private Mock<IAnimalService> _mockAnimalService;
        private List<DigestorDailyOutput> _dailyResults;
        private DigestateState _state;
        private DateTime _date;
        private Farm _farm;
        private AnaerobicDigestionComponent _adComponent;
        private DigestateApplicationViewItem _livestockDigestateApplication;
        private DigestateApplicationViewItem _importedDigestateApplication;
        private FieldSystemComponent _fieldSystemComponent;
        private CropViewItem _cropViewItem;
        private DigestorDailyOutput _dailyOutput1;
        private DigestorDailyOutput _dailyOutput2;
        private DigestorDailyOutput _dailyOutput3;
        private DigestorDailyOutput _dailyOutput4;

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
            _mockAdCalculator = new Mock<IADCalculator>();
            _mockAnimalService = new Mock<IAnimalService>();

            _dailyOutput1 = new DigestorDailyOutput()
            {
                Date = new DateTime(DateTime.Now.Year, 4, 1),
                FlowRateOfAllSubstratesInDigestate = 100,
                FlowRateSolidFraction = 200,
                FlowRateLiquidFraction = 2222,

                TotalAmountOfCarbonInRawDigestateAvailableForLandApplication = 10,
                TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication = 20,

                TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction = 33,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction = 66,

                TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = 103,
                TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction = 113,
            };

            _dailyOutput2 = new DigestorDailyOutput()
            {
                Date = new DateTime(DateTime.Now.Year, 4, 2),
                FlowRateOfAllSubstratesInDigestate = 100,
                FlowRateSolidFraction = 100,
                FlowRateLiquidFraction = 3333,

                TotalAmountOfCarbonInRawDigestateAvailableForLandApplication = 30,
                TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication = 50,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction = 21,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction = 55,

                TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = 11,
                TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction = 22,
            };

            _dailyOutput3 = new DigestorDailyOutput()
            {
                Date = new DateTime(DateTime.Now.Year - 1, 4, 1),
                FlowRateOfAllSubstratesInDigestate = 100,
                FlowRateSolidFraction = 600,
                FlowRateLiquidFraction = 1001,

                TotalAmountOfCarbonInRawDigestateAvailableForLandApplication = 30,
                TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication = 50,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction = 27,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction = 96,

                TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = 2,
                TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction = 3,
            };

            _dailyOutput4 = new DigestorDailyOutput()
            {
                Date = new DateTime(DateTime.Now.Year, 4, 3),
                FlowRateOfAllSubstratesInDigestate = 300,
                FlowRateSolidFraction = 222,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplication = 80,
                TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication = 20,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction = 26,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction = 77,

                TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = 55,
                TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction = 10,
            };

            _dailyResults = new List<DigestorDailyOutput>() { _dailyOutput1, _dailyOutput2, _dailyOutput3, _dailyOutput4 };

            _mockAdCalculator.Setup(x => x.CalculateResults(It.IsAny<Farm>(), It.IsAny<List<AnimalComponentEmissionsResults>>())).Returns(new List<DigestorDailyOutput>(_dailyResults));

            _sut = new DigestateService();

            _sut.ADCalculator = _mockAdCalculator.Object;

            _date = DateTime.Now;
            _state = DigestateState.LiquidPhase;

            _farm = base.GetTestFarm();
            var fieldComponent = base.GetTestFieldComponent();
            _cropViewItem = base.GetTestCropViewItem();
            _livestockDigestateApplication = base.GetTestRawDigestateApplicationViewItem();
            _livestockDigestateApplication.ManureLocationSourceType = ManureLocationSourceType.Livestock;
            _livestockDigestateApplication.DateCreated = _date;
            _livestockDigestateApplication.DigestateState = _state;

            _importedDigestateApplication = base.GetTestRawDigestateApplicationViewItem();
            _importedDigestateApplication.ManureLocationSourceType = ManureLocationSourceType.Imported;
            _importedDigestateApplication.DateCreated = _date;
            _importedDigestateApplication.DigestateState = _state;

            _cropViewItem.DigestateApplicationViewItems.Add(_livestockDigestateApplication);
            _fieldSystemComponent = fieldComponent;
            _fieldSystemComponent.CropViewItems.Add(_cropViewItem);

            _adComponent = base.GetTestAnaerobicDigestionComponent();
            _farm.Components.Add(_adComponent);
            _farm.Components.Add(_fieldSystemComponent);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetTankStatesReturnsReducedAmountsConsideringFieldApplications()
        {
            _adComponent.IsLiquidSolidSeparated = false;
            _livestockDigestateApplication.DateCreated = _dailyResults[1].Date;
            _livestockDigestateApplication.DigestateState = DigestateState.Raw;

            var result = _sut.GetDailyTankStates(_farm, _dailyResults, _dailyResults[1].Date.Year);

            
            Assert.AreEqual(100, result.ElementAt(0).TotalRawDigestateAvailable);

            Assert.AreEqual(150, result.ElementAt(1).TotalRawDigestateAvailable);
        }

        [TestMethod]
        public void GetTotalCarbonRemainingAtEndOfYear()
        {
            _adComponent.IsLiquidSolidSeparated = false;

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() {base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults()});

            var totalCarbon = _sut.GetTotalCarbonRemainingAtEndOfYear(DateTime.Now.Year,_farm, _adComponent);

            Assert.AreEqual(120, totalCarbon);
        }

        [TestMethod]
        public void CalculateAmountOfDigestateRemaining()
        {
            _adComponent.IsLiquidSolidSeparated = false;
            _livestockDigestateApplication.DigestateState = DigestateState.Raw;
            var year = DateTime.Now.Year;

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });

            var result = _sut.GetTotalCarbonRemainingAtEndOfYear(year, _farm, _adComponent);

            Assert.AreEqual(120, result);
        }

        [TestMethod]
        public void GetTotalAmountOfDigestateAppliedOnDay()
        {
            var result = _sut.GetTotalAmountOfDigestateAppliedOnDay(_date, _farm, _state, ManureLocationSourceType.Livestock);

            Assert.AreEqual(50, result);
        }

        [TestMethod]
        public void GetTotalNitrogenRemainingAtEndOfYearReturnsNonZero()
        {
            _adComponent.IsLiquidSolidSeparated = false;

            var year = DateTime.Now.Year;

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });
            var totalNitrogen = _sut.GetTotalNitrogenRemainingAtEndOfYearAfterFieldApplications(year, _farm);

            Assert.AreEqual(90, totalNitrogen);
        }

        [TestMethod]
        public void CalculateSolidAmountsAvailable()
        {
            var fieldSystemComponent = new FieldSystemComponent();
            var digestateApplication = new DigestateApplicationViewItem();
            digestateApplication.DigestateState = DigestateState.SolidPhase;
            digestateApplication.AmountAppliedPerHectare = 20;

            var cropViewItem = new CropViewItem();
            cropViewItem.Area = 50;
            cropViewItem.DigestateApplicationViewItems.Add(digestateApplication);
            fieldSystemComponent.CropViewItems.Add(cropViewItem);

            _farm.Components.Clear();
            _farm.Components.Add(fieldSystemComponent);

            var digestorOutput = new DigestorDailyOutput();
            var outputDate = DateTime.Now;
            var outputNumber = 1;
            var tanks = new List<DigestateTank>();
            tanks.Add(new DigestateTank()
            {
                TotalSolidDigestateAvailable = 2000,
                NitrogenFromSolidDigestate = 900,
                CarbonFromSolidDigestate = 777,
            });

            var tank = new DigestateTank();
            
            var component = new AnaerobicDigestionComponent();

            digestorOutput.FlowRateSolidFraction = 10;
            digestorOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = 2000;
            digestorOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction = 8999;

            _sut.CalculateSolidAmountsAvailable(
                digestorOutput, 
                outputDate, 
                _farm, 
                outputNumber,
                tanks, 
                tank, 
                component);

            Assert.AreEqual(9776, tank.CarbonFromSolidDigestate);
            Assert.AreEqual(2900, tank.NitrogenFromSolidDigestate);
        }

        [TestMethod]
        public void CalculateLiquidAmountsAvailable()
        {
            var fieldSystemComponent = new FieldSystemComponent();
            var digestateApplication = new DigestateApplicationViewItem();
            digestateApplication.DigestateState = DigestateState.LiquidPhase;
            digestateApplication.AmountAppliedPerHectare = 20;

            var cropViewItem = new CropViewItem();
            cropViewItem.Area = 50;
            cropViewItem.DigestateApplicationViewItems.Add(digestateApplication);
            fieldSystemComponent.CropViewItems.Add(cropViewItem);

            _farm.Components.Clear();
            _farm.Components.Add(fieldSystemComponent);

            var digestorOutput = new DigestorDailyOutput();
            var outputDate = DateTime.Now;
            var outputNumber = 1;
            var tanks = new List<DigestateTank>();
            tanks.Add(new DigestateTank()
            {
                TotalSolidDigestateAvailable = 0,
                NitrogenFromSolidDigestate = 0,
                CarbonFromSolidDigestate = 0,

                TotalLiquidDigestateAvailable = 6000,
                NitrogenFromLiquidDigestate = 1800,
                CarbonFromLiquidDigestate = 555,
            });

            var tank = new DigestateTank();

            var component = new AnaerobicDigestionComponent();

            digestorOutput.FlowRateLiquidFraction = 30;
            digestorOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = 0;
            digestorOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction = 0;
            digestorOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction = 220;
            digestorOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction = 330;

            _sut.CalculateLiquidAmountsAvailable(
                digestorOutput,
                outputDate,
                _farm,
                outputNumber,
                tanks,
                tank,
                component);

            Assert.AreEqual(885, tank.CarbonFromLiquidDigestate, 0.0001);
            Assert.AreEqual(2020, tank.NitrogenFromLiquidDigestate, 0.0001);
        }

        [TestMethod]
        public void CalculateRawAmountsAvailable()
        {
            var fieldSystemComponent = new FieldSystemComponent();
            var digestateApplication = new DigestateApplicationViewItem();
            digestateApplication.DigestateState = DigestateState.Raw;
            digestateApplication.AmountAppliedPerHectare = 20;

            var cropViewItem = new CropViewItem();
            cropViewItem.Area = 50;
            cropViewItem.DigestateApplicationViewItems.Add(digestateApplication);
            fieldSystemComponent.CropViewItems.Add(cropViewItem);

            _farm.Components.Clear();
            _farm.Components.Add(fieldSystemComponent);

            var digestorOutput = new DigestorDailyOutput();
            var outputDate = DateTime.Now;
            var outputNumber = 1;
            var tanks = new List<DigestateTank>();
            tanks.Add(new DigestateTank()
            {
                TotalSolidDigestateAvailable = 0,
                NitrogenFromSolidDigestate = 0,
                CarbonFromSolidDigestate = 0,

                TotalLiquidDigestateAvailable = 0,
                NitrogenFromLiquidDigestate = 0,
                CarbonFromLiquidDigestate = 0,

                TotalRawDigestateAvailable = 10,
                NitrogenFromRawDigestate = 20,
                CarbonFromRawDigestate = 30,
            });

            var tank = new DigestateTank();

            var component = new AnaerobicDigestionComponent();
            component.IsLiquidSolidSeparated = false;

            digestorOutput.FlowRateOfAllSubstratesInDigestate = 10099;

            digestorOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromSolidFraction = 0;
            digestorOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromSolidFraction = 0;

            digestorOutput.TotalAmountOfNitrogenInRawDigestateAvailableForLandApplicationFromLiquidFraction = 0;
            digestorOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplicationFromLiquidFraction = 0;

            digestorOutput.TotalAmountOfNitrogenFromRawDigestateAvailableForLandApplication = 11;
            digestorOutput.TotalAmountOfCarbonInRawDigestateAvailableForLandApplication = 33;

            _sut.CalculateRawAmountsAvailable(
                digestorOutput,
                outputDate,
                _farm,
                outputNumber,
                tanks,
                tank,
                component);

            Assert.AreEqual(63, tank.CarbonFromRawDigestate, 0.0001);
            Assert.AreEqual(31, tank.NitrogenFromRawDigestate, 0.0001);
        }

        [TestMethod]
        public void GetDateOfMaximumAvailableDigestateTest()
        {
            var year = DateTime.Now.Year;

            var result = _sut.GetDateOfMaximumAvailableDigestate(_farm, DigestateState.Raw, year, _dailyResults, false);

            Assert.AreEqual(new DateTime(DateTime.Now.Year, 4, 3), result);
        }

        [TestMethod]
        public void GetDateOfMaximumAvailableRawDigestateProducedTest()
        {
            var year = DateTime.Now.Year;

            // Since we are interested in the amount of raw digestate, we don't want to separate into liquid/solid fractions
            _adComponent.IsLiquidSolidSeparated = false;

            _livestockDigestateApplication.DigestateState = DigestateState.Raw;
            _livestockDigestateApplication.DateCreated = new DateTime(DateTime.Now.Year, 4, 3);
            _livestockDigestateApplication.AmountAppliedPerHectare = 6000;

            _dailyOutput2.FlowRateOfAllSubstratesInDigestate = 20000;

            var result = _sut.GetDateOfMaximumAvailableDigestate(_farm, DigestateState.Raw, year, _dailyResults, false);

            Assert.AreEqual(new DateTime(DateTime.Now.Year, 4, 3), result);
        }

        [TestMethod]
        public void GetTotalManureNitrogenRemainingForFarmAndYearReturnsRawAmountsNotConsideringLandAppliedAmountsTest()
        {
            _adComponent.IsLiquidSolidSeparated = false;
            var result = _sut.GetTotalManureNitrogenRemainingForFarmAndYear(DateTime.Now.Year, _farm, _dailyResults, DigestateState.Raw);

            Assert.AreEqual(90, result);
        }

        [TestMethod]
        public void GetTotalManureNitrogenRemainingForFarmAndYearReturnsRawAmountsConsideringLandAppliedAmountsTest()
        {
            _livestockDigestateApplication.DigestateState = DigestateState.Raw;
            _livestockDigestateApplication.DateCreated = new DateTime(DateTime.Now.Year, 4, 3);
            _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare = 10;

            _adComponent.IsLiquidSolidSeparated = false;
            var result = _sut.GetTotalManureNitrogenRemainingForFarmAndYear(DateTime.Now.Year, _farm, _dailyResults, DigestateState.Raw);

            Assert.AreEqual(80, result);
        }

        [TestMethod]
        public void GetTotalManureNitrogenRemainingForFarmAndYearReturnsSolidAmountsNotConsideringLandAppliedAmountsTest()
        {
            _adComponent.IsLiquidSolidSeparated = true;
            var result = _sut.GetTotalManureNitrogenRemainingForFarmAndYear(DateTime.Now.Year, _farm, _dailyResults, DigestateState.SolidPhase);

            Assert.AreEqual(169, result);
        }

        [TestMethod]
        public void GetTotalManureNitrogenRemainingForFarmAndYearReturnsSolidAmountsConsideringLandAppliedAmountsTest()
        {
            _livestockDigestateApplication.DigestateState = DigestateState.SolidPhase;
            _livestockDigestateApplication.DateCreated = new DateTime(DateTime.Now.Year, 4, 3);
            _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare = 50;

            _adComponent.IsLiquidSolidSeparated =true;
            var result = _sut.GetTotalManureNitrogenRemainingForFarmAndYear(DateTime.Now.Year, _farm, _dailyResults, DigestateState.SolidPhase);

            Assert.AreEqual(119, result, 1);
        }

        [TestMethod]
        public void GetTotalManureNitrogenRemainingForFarmAndYearReturnsLiquidAmountsNotConsideringLandAppliedAmountsTest()
        {
            _adComponent.IsLiquidSolidSeparated = true;
            var result = _sut.GetTotalManureNitrogenRemainingForFarmAndYear(DateTime.Now.Year, _farm, _dailyResults, DigestateState.LiquidPhase);

            Assert.AreEqual(145, result);
        }

        [TestMethod]
        public void GetTotalManureNitrogenRemainingForFarmAndYearReturnsLiquidAmountsConsideringLandAppliedAmountsTest()
        {
            _livestockDigestateApplication.DigestateState = DigestateState.LiquidPhase;
            _livestockDigestateApplication.DateCreated = new DateTime(DateTime.Now.Year, 4, 3);
            _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare = 50;

            _adComponent.IsLiquidSolidSeparated = true;
            var result = _sut.GetTotalManureNitrogenRemainingForFarmAndYear(DateTime.Now.Year, _farm, _dailyResults, DigestateState.LiquidPhase);

            Assert.AreEqual(95, result, 1);
        }

        [TestMethod]
        public void GetTotalManureNitrogenRemainingForFarmAndYearReturnsLiquidAmountsConsideringImportedLandAppliedAmountsTest()
        {
            _sut.SubtractAmountsFromImportedDigestateLandApplications = true;
            _cropViewItem.DigestateApplicationViewItems.Add(_importedDigestateApplication);

            _adComponent.IsLiquidSolidSeparated = true;
            var result = _sut.GetTotalManureNitrogenRemainingForFarmAndYear(DateTime.Now.Year, _farm, _dailyResults, DigestateState.LiquidPhase);

            Assert.AreEqual(145, result, 1);
        }

        #endregion
    }
}