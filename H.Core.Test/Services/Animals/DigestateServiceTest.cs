using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Emissions.Results;
using System.Collections.Generic;
using System.Linq;
using H.Core.Calculators.Infrastructure;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
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

            _date = DateTime.Now.Date;
            _state = DigestateState.LiquidPhase;

            _farm = base.GetTestFarm();
            var fieldComponent = base.GetTestFieldComponent();
            _cropViewItem = base.GetTestCropViewItem();
            _livestockDigestateApplication = base.GetTestRawDigestateApplicationViewItem();
            _livestockDigestateApplication.ManureLocationSourceType = ManureLocationSourceType.Livestock;
            _livestockDigestateApplication.DateCreated = _dailyOutput1.Date.Date;
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

            // After Initialize(), RecalculateDigestateApplicationAmounts will have updated the stored
            // N/ha and C/ha values to be consistent with the tank state. For this Raw application on
            // Apr 1 with AmountAppliedPerHectare=50, Area=1, TotalRawDigestateProduced=100:
            //   fractionUsed = (50 * 1) / 100 = 0.5
            //   C/ha = (0.5 * 10) / 1 = 5   (daily raw C on Apr 1 = 10)
            //   N/ha = (0.5 * 20) / 1 = 10   (daily raw N on Apr 1 = 20)
            //
            // Tank state at Apr 3 (end of year):
            //   Day 1: CarbonFromRawDigestate = 10 - 5 = 5
            //   Day 2: 5 + 30 - 0 = 35
            //   Day 3: 35 + 80 - 0 = 115
            var result = _sut.GetTotalCarbonRemainingAtEndOfYear(year, _farm, _adComponent);

            Assert.AreEqual(115, result);
        }

        [TestMethod]
        public void GetTotalAmountOfDigestateAppliedOnDay()
        {
            var result = _sut.GetTotalAmountOfDigestateAppliedOnDay(_livestockDigestateApplication.DateCreated.Date, _farm, _state, ManureLocationSourceType.Livestock);

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
            // Amounts
            //
            // April 1, 2025
            // 
            // Total N created 113
            // Total N used on day 50 (Livestock digestate application)
            // Total N from previous day 0
            // Total N left over 63
            // 
            // April 2, 2025
            // 
            // Total N created 22
            // Total N used on day 0
            // Total N from previous day 63
            // Liquid N available 85 (63 + 25)
            // 
            // April 3, 2025
            // 
            // Total N created 10
            // Total N used on day 0
            // Total N from previous day 85
            // Liquid N available 95 (85 + 10)

            _adComponent.IsLiquidSolidSeparated = true;
            var result = _sut.GetTotalManureNitrogenRemainingForFarmAndYear(DateTime.Now.Year, _farm, _dailyResults, DigestateState.LiquidPhase);

            Assert.AreEqual(95, result);
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

        [Ignore("Imports not implemented yet")]
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

        #region RecalculateDigestateApplicationAmounts Tests (Fix 1 & Fix 2)

        /// <summary>
        /// Verifies that RecalculateDigestateApplicationAmounts correctly updates the N/ha value
        /// for a raw digestate application based on the tank state at the application date.
        ///
        /// For a Raw application on Apr 1 with:
        ///   AmountAppliedPerHectare = 50, Area = 1
        ///   TotalRawDigestateProduced on Apr 1 = 100 (FlowRateOfAllSubstratesInDigestate)
        ///   NitrogenFromRawDigestateNotConsideringFieldApplicationAmounts on Apr 1 = 20
        ///
        /// Expected: fractionUsed = (50 * 1) / 100 = 0.5
        ///           N/ha = (0.5 * 20) / 1 = 10
        /// </summary>
        [TestMethod]
        public void RecalculateDigestateApplicationAmountsUpdatesNitrogenPerHectareForRawApplication()
        {
            _adComponent.IsLiquidSolidSeparated = false;
            _livestockDigestateApplication.DigestateState = DigestateState.Raw;

            // Set an intentionally stale/incorrect value to verify the recalculation updates it
            _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare = 999;

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });

            // After Initialize() -> RecalculateDigestateApplicationAmounts(), the N/ha should be
            // recalculated from the actual tank state, not left at the stale value of 999
            Assert.AreEqual(10, _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare, 0.001,
                "RecalculateDigestateApplicationAmounts should update N/ha to the correct value derived from tank state");
        }

        /// <summary>
        /// Verifies that RecalculateDigestateApplicationAmounts correctly updates the C/ha value
        /// for a raw digestate application based on the tank state at the application date.
        ///
        /// For a Raw application on Apr 1 with:
        ///   AmountAppliedPerHectare = 50, Area = 1
        ///   TotalRawDigestateProduced on Apr 1 = 100
        ///   CarbonFromRawDigestateNotConsideringFieldApplicationAmounts on Apr 1 = 10
        ///
        /// Expected: fractionUsed = (50 * 1) / 100 = 0.5
        ///           C/ha = (0.5 * 10) / 1 = 5
        /// </summary>
        [TestMethod]
        public void RecalculateDigestateApplicationAmountsUpdatesCarbonPerHectareForRawApplication()
        {
            _adComponent.IsLiquidSolidSeparated = false;
            _livestockDigestateApplication.DigestateState = DigestateState.Raw;

            // Set an intentionally stale/incorrect value to verify the recalculation updates it
            _livestockDigestateApplication.AmountOfCarbonAppliedPerHectare = 999;

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });

            Assert.AreEqual(5, _livestockDigestateApplication.AmountOfCarbonAppliedPerHectare, 0.001,
                "RecalculateDigestateApplicationAmounts should update C/ha to the correct value derived from tank state");
        }

        /// <summary>
        /// Verifies that RecalculateDigestateApplicationAmounts correctly updates N/ha and C/ha values
        /// for a liquid-phase digestate application when liquid-solid separation is enabled.
        ///
        /// For a LiquidPhase application on Apr 1 with:
        ///   AmountAppliedPerHectare = 50, Area = 1
        ///   TotalLiquidDigestateProduced on Apr 1 = 2222 (FlowRateLiquidFraction)
        ///   NitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts on Apr 1 = 113
        ///   CarbonFromLiquidDigestateNotConsideringFieldApplicationAmounts on Apr 1 = 66
        ///
        /// Expected: fractionUsed = (50 * 1) / 2222 = 0.02250...
        ///           N/ha = (0.02250... * 113) / 1 = 2.5427...
        ///           C/ha = (0.02250... * 66) / 1 = 1.4851...
        /// </summary>
        [TestMethod]
        public void RecalculateDigestateApplicationAmountsUpdatesValuesForLiquidPhaseApplication()
        {
            _adComponent.IsLiquidSolidSeparated = true;
            _livestockDigestateApplication.DigestateState = DigestateState.LiquidPhase;
            _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare = 999;
            _livestockDigestateApplication.AmountOfCarbonAppliedPerHectare = 999;

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });

            var expectedFraction = (50.0 * 1.0) / 2222.0;
            var expectedNPerHa = (expectedFraction * 113.0) / 1.0;
            var expectedCPerHa = (expectedFraction * 66.0) / 1.0;

            Assert.AreEqual(expectedNPerHa, _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare, 0.001,
                "N/ha should be recalculated for liquid-phase application when separation is enabled");
            Assert.AreEqual(expectedCPerHa, _livestockDigestateApplication.AmountOfCarbonAppliedPerHectare, 0.001,
                "C/ha should be recalculated for liquid-phase application when separation is enabled");
        }

        /// <summary>
        /// Verifies that RecalculateDigestateApplicationAmounts handles the case where there are no
        /// digestate applications on any field (no-op scenario). Should not throw any exceptions.
        /// </summary>
        [TestMethod]
        public void RecalculateDigestateApplicationAmountsHandlesNoApplicationsGracefully()
        {
            _adComponent.IsLiquidSolidSeparated = false;

            // Remove all digestate applications from the crop view item
            _cropViewItem.DigestateApplicationViewItems.Clear();

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });

            // Should not throw - just a no-op
            var totalCarbon = _sut.GetTotalCarbonRemainingAtEndOfYear(DateTime.Now.Year, _farm, _adComponent);

            // With no field applications, carbon remaining = sum of all daily raw C = 10 + 30 + 80 = 120
            Assert.AreEqual(120, totalCarbon);
        }

        /// <summary>
        /// Verifies that RecalculateDigestateApplicationAmounts handles the case where the farm
        /// has no AD component. Should not throw any exceptions.
        /// </summary>
        [TestMethod]
        public void RecalculateDigestateApplicationAmountsHandlesNoADComponentGracefully()
        {
            // Remove the AD component from the farm
            _farm.Components.Remove(_adComponent);

            // Calling RecalculateDigestateApplicationAmounts directly (not through Initialize,
            // since Initialize would also fail without AD). This verifies the guard clause.
            _sut.RecalculateDigestateApplicationAmounts(_farm);

            // Should not throw - the N/ha should remain unchanged at the original test default of 50
            Assert.AreEqual(50, _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare,
                "N/ha should remain unchanged when there is no AD component");
        }

        /// <summary>
        /// Verifies that when a digestate application has a DigestateState that does not match the
        /// component's separation setting (e.g. LiquidPhase application but IsLiquidSolidSeparated = false),
        /// the recalculation correctly sets N/ha and C/ha to 0 since no digestate of that phase is produced.
        /// This simulates the scenario where a user has a misconfigured application state.
        /// </summary>
        [TestMethod]
        public void RecalculateDigestateApplicationAmountsSetsZeroWhenPhaseDoesNotMatchSeparationSetting()
        {
            // Component does NOT use liquid-solid separation, but application says LiquidPhase
            _adComponent.IsLiquidSolidSeparated = false;
            _livestockDigestateApplication.DigestateState = DigestateState.LiquidPhase;
            _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare = 999;
            _livestockDigestateApplication.AmountOfCarbonAppliedPerHectare = 999;

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });

            // Since IsLiquidSolidSeparated = false, the tank's liquid values (TotalLiquidDigestateProduced,
            // NitrogenFromLiquidDigestateNotConsideringFieldApplicationAmounts, etc.) will be 0.
            // GetFractionUsed will return 0 (guard: totalAmountCreated <= 0), so N/ha and C/ha = 0.
            Assert.AreEqual(0, _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare, 0.001,
                "N/ha should be 0 when application phase doesn't match component separation setting");
            Assert.AreEqual(0, _livestockDigestateApplication.AmountOfCarbonAppliedPerHectare, 0.001,
                "C/ha should be 0 when application phase doesn't match component separation setting");
        }

        /// <summary>
        /// Simulates the cross-ViewModel scenario: after Initialize() recalculates values, calling
        /// RecalculateDigestateApplicationAmounts again with the same data should produce the same
        /// results (idempotency). This verifies that the recalculation is stable and the "not
        /// considering field applications" values are truly independent of stored N/ha and C/ha.
        /// </summary>
        [TestMethod]
        public void RecalculateDigestateApplicationAmountsIsIdempotent()
        {
            _adComponent.IsLiquidSolidSeparated = false;
            _livestockDigestateApplication.DigestateState = DigestateState.Raw;

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });

            var nPerHaAfterFirstRecalc = _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare;
            var cPerHaAfterFirstRecalc = _livestockDigestateApplication.AmountOfCarbonAppliedPerHectare;

            // Call recalculate a second time (simulates what happens when the event fires)
            _sut.RecalculateDigestateApplicationAmounts(_farm);

            Assert.AreEqual(nPerHaAfterFirstRecalc, _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare, 0.001,
                "N/ha should be the same after a second recalculation (idempotent)");
            Assert.AreEqual(cPerHaAfterFirstRecalc, _livestockDigestateApplication.AmountOfCarbonAppliedPerHectare, 0.001,
                "C/ha should be the same after a second recalculation (idempotent)");
        }

        /// <summary>
        /// Verifies that the recalculation correctly reflects the proportional relationship between
        /// application rate and N/ha. Doubling the AmountAppliedPerHectare should double the N/ha
        /// (as long as the total application doesn't exceed total digestate produced).
        /// </summary>
        [TestMethod]
        public void RecalculateDigestateApplicationAmountsScalesLinearlyWithApplicationRate()
        {
            _adComponent.IsLiquidSolidSeparated = false;
            _livestockDigestateApplication.DigestateState = DigestateState.Raw;
            _livestockDigestateApplication.AmountAppliedPerHectare = 25; // Half the default

            _sut.Initialize(_farm, new List<AnimalComponentEmissionsResults>() { base.GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults() });

            var nPerHaAtHalfRate = _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare;

            // Now double the application rate and recalculate
            _livestockDigestateApplication.AmountAppliedPerHectare = 50;
            _sut.RecalculateDigestateApplicationAmounts(_farm);

            var nPerHaAtFullRate = _livestockDigestateApplication.AmountOfNitrogenAppliedPerHectare;

            Assert.AreEqual(nPerHaAtHalfRate * 2, nPerHaAtFullRate, 0.001,
                "N/ha should scale linearly with application rate");
        }

        #endregion
    }
}