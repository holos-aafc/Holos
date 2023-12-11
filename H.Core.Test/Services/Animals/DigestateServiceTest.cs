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
        private DigestateApplicationViewItem _digestateApplication;
        private FieldSystemComponent _fieldSystemComponent;
        private CropViewItem _cropViewItem;

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

            var dailyOutput1 = new DigestorDailyOutput()
            {
                Date = DateTime.Now,
                FlowRateOfAllSubstratesInDigestate = 100,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplication = 10,
            };

            var dailyOutput2 = new DigestorDailyOutput()
            {
                Date = DateTime.Now.AddDays(1),
                FlowRateOfAllSubstratesInDigestate = 100,
                TotalAmountOfCarbonInRawDigestateAvailableForLandApplication = 30,
            };

            _dailyResults = new List<DigestorDailyOutput>() { dailyOutput1, dailyOutput2 };

            _mockAdCalculator.Setup(x => x.CalculateResults(It.IsAny<Farm>(), It.IsAny<List<AnimalComponentEmissionsResults>>())).Returns(new List<DigestorDailyOutput>(_dailyResults));

            _sut = new DigestateService();

            _sut.ADCalculator = _mockAdCalculator.Object;

            _date = DateTime.Now;
            _state = DigestateState.LiquidPhase;

            _farm = base.GetTestFarm();
            var fieldComponent = base.GetTestFieldComponent();
            _cropViewItem = base.GetTestCropViewItem();
            _digestateApplication = base.GetTestRawDigestateApplicationViewItem();
            _digestateApplication.DateCreated = _date;
            _digestateApplication.DigestateState = _state;

            _cropViewItem.DigestateApplicationViewItems.Add(_digestateApplication);
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
            _digestateApplication.DateCreated = _dailyResults[1].Date;
            _digestateApplication.DigestateState = DigestateState.Raw;

            var result = _sut.GetDailyTankStates(_farm, _dailyResults);

            // 100 kg is produced on 1st day
            Assert.AreEqual(100, result.ElementAt(0).TotalRawDigestateAvailable);

            // another 100 kg is produced on second day totaling 200. Subtract 50 for the field application totaling 150 kg available on 2nd day
            Assert.AreEqual(150, result.ElementAt(1).TotalRawDigestateAvailable);
        }

        [TestMethod]
        public void GetTotalCarbonRemainingAtEndOfYear()
        {
            _adComponent.IsLiquidSolidSeparated = false;

            var totalCarbon = _sut.GetTotalCarbonRemainingAtEndOfYear(DateTime.Now.Year,_farm, _adComponent);

            Assert.AreEqual(40, totalCarbon);
        }

        [TestMethod]
        public void CalculateAmountOfDigestateRemaining()
        {
            _adComponent.IsLiquidSolidSeparated = false;
            _digestateApplication.DigestateState = DigestateState.Raw;
            var year = DateTime.Now.Year;

            // Total carbon created is 40 over both days (10 kg on day 1, 30 on day 2)

            // Raw digestate applied on first day using 50% of available digestate. 0.5 * 10 kg on day 1 = 5
            // Total over 2 days is 5 + 30 = 35

            var result = _sut.GetTotalCarbonRemainingAtEndOfYear(year, _farm, _adComponent);

            Assert.AreEqual(35, result);
        }

        [TestMethod]
        public void GetTotalAmountOfDigestateAppliedOnDay()
        {
            var result = _sut.GetTotalAmountOfDigestateAppliedOnDay(_date, _farm, _state);

            Assert.AreEqual(50, result);
        }

        #endregion
    }
}