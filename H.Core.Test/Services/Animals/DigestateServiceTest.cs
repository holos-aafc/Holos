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
    public class DigestateServiceTest
    {
        #region Fields

        private DigestateService _sut;
        private Farm _farm;
        private List<AnimalComponentEmissionsResults> _animalComponentResults;
        private Mock<IADCalculator> _mockAdCalculator;
        private Mock<IAnimalService> _mockAnimalService;

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

            _sut = new DigestateService();

            var managementPeriod = new ManagementPeriod()
            {
                Start = DateTime.Now,
                End = DateTime.Now.AddYears(1),
                ManureDetails = new ManureDetails()
                {
                    StateType = ManureStateType.AnaerobicDigester,
                }
            };

            _farm = new Farm();

            var animalGroup = new AnimalGroup();
            animalGroup.ManagementPeriods.Add(managementPeriod);

            var component = new SheepFeedlotComponent();
            component.Groups.Add(animalGroup);

            _farm.Components.Add(component);

            _animalComponentResults = new List<AnimalComponentEmissionsResults>();
            _animalComponentResults.Add(new AnimalComponentEmissionsResults()
            {
                EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>()
                {
                    new AnimalGroupEmissionResults()
                    {
                        GroupEmissionsByMonths = new List<GroupEmissionsByMonth>()
                        {
                            new GroupEmissionsByMonth(new MonthsAndDaysData() {ManagementPeriod = managementPeriod}, new List<GroupEmissionsByDay>()
                            {
                                new GroupEmissionsByDay(),
                            })
                        }
                    }
                }
            });
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
            var farm = new Farm();

            var field = new FieldSystemComponent();
            farm.Components.Add(field);

            var crop = new CropViewItem();
            field.CropViewItems.Add(crop);

            crop.Area = 1;

            var digestateApplication = new DigestateApplicationViewItem();
            crop.DigestateApplicationViewItems.Add(digestateApplication);

            digestateApplication.DateCreated = DateTime.Now.AddDays(1);
            digestateApplication.DigestateState = DigestateState.Raw;
            digestateApplication.MaximumAmountOfDigestateAvailablePerHectare = 100;
            digestateApplication.AmountAppliedPerHectare = 50;

            var digestor = new AnaerobicDigestionComponent();
            farm.Components.Add(digestor);

            digestor.IsLiquidSolidSeparated = false;

            var dailyOutput1 = new DigestorDailyOutput()
            {
                Date = DateTime.Now,
                FlowRateOfAllSubstratesInDigestate = 100,
            };

            var dailyOutput2 = new DigestorDailyOutput()
            {
                Date = DateTime.Now.AddDays(1),
                FlowRateOfAllSubstratesInDigestate = 100,
            };

            var dailyResults = new List<DigestorDailyOutput>() {dailyOutput1, dailyOutput2};

            var result = _sut.GetDailyTankStates(farm, dailyResults);

            // 100 kg is produced on 1st day
            Assert.AreEqual(100, result.ElementAt(0).TotalRawDigestateAvailable);

            // another 100 kg is produced on second day totaling 200. Subtract 50 for the field application totaling 150 kg available on 2nd day
            Assert.AreEqual(150, result.ElementAt(1).TotalRawDigestateAvailable);
        }

        #endregion
    }
}