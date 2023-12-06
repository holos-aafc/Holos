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
        private Farm _farm;
        private List<AnimalComponentEmissionsResults> _animalComponentResults;
        private Mock<IADCalculator> _mockAdCalculator;
        private Mock<IAnimalService> _mockAnimalService;
        private List<DigestorDailyOutput> _dailyResults;

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
            _sut.AnimalService = _mockAnimalService.Object;
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
            var farm = base.GetTestFarm();

            var field = base.GetTestFieldComponent();
            farm.Components.Add(field);

            var crop = base.GetTestCropViewItem();
            field.CropViewItems.Add(crop);

            crop.DigestateApplicationViewItems.Add(base.GetTestDigestateApplicationViewItem());

            var digestor = new AnaerobicDigestionComponent();
            farm.Components.Add(digestor);

            digestor.IsLiquidSolidSeparated = false;

            var result = _sut.GetDailyTankStates(farm, _dailyResults);

            // 100 kg is produced on 1st day
            Assert.AreEqual(100, result.ElementAt(0).TotalRawDigestateAvailable);

            // another 100 kg is produced on second day totaling 200. Subtract 50 for the field application totaling 150 kg available on 2nd day
            Assert.AreEqual(150, result.ElementAt(1).TotalRawDigestateAvailable);
        }

        [TestMethod]
        public void GetTotalCarbonRemainingAtEndOfYear()
        {
            var farm = base.GetTestFarm();
            var adComponent = base.GetTestAnaerobicDigestionComponent();
            farm.Components.Add(adComponent);
            adComponent.IsLiquidSolidSeparated = false;

            var totalCarbon = _sut.GetTotalCarbonRemainingAtEndOfYear(DateTime.Now.Year,farm, adComponent);

            Assert.AreEqual(40, totalCarbon);
        }

        [TestMethod]
        public void GetTotalAmountOfDigestateAppliedOnDay()
        {
            var date = DateTime.Now;
            var state = DigestateState.LiquidPhase;

            var farm = base.GetTestFarm();
            var fieldComponent = base.GetTestFieldComponent();
            var cropViewItem = base.GetTestCropViewItem();
            var digestateApplication = base.GetTestDigestateApplicationViewItem();
            digestateApplication.DateCreated = date;
            digestateApplication.DigestateState = state;

            cropViewItem.DigestateApplicationViewItems.Add(digestateApplication);
            fieldComponent.CropViewItems.Add(cropViewItem);
            farm.Components.Add(fieldComponent);

            var result = _sut.GetTotalAmountOfDigestateAppliedOnDay(date, farm, state);

            Assert.AreEqual(50, result);
        }

        #endregion
    }
}