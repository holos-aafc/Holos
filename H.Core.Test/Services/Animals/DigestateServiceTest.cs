using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Sheep;
using H.Core.Emissions.Results;
using System.Collections.Generic;
using H.Core.Calculators.Infrastructure;
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

            _sut = new DigestateService(_mockAdCalculator.Object, _mockAnimalService.Object);

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
        public void GetDigestateTankInternalTest()
        {
            var result = _sut.GetDigestateTankInternal(2012, DigestateState.LiquidPhase);

            Assert.IsNotNull(result);
            Assert.AreEqual(2012, result.Year);
        }

        [TestMethod]
        public void SetStartingStateOfTankTest()
        {
            var tank = new DigestateTank();
            tank.Year = DateTime.Now.Year;

            var adResults = new List<DigestorDailyOutput>()
            {
                new DigestorDailyOutput()
                {
                    Date = DateTime.Now.AddDays(-1), FlowRateOfAllSubstratesInDigestate = 200
                },

                // This item should not be included in the totals since it won't have the same year as the target date
                new DigestorDailyOutput()
                {
                    Date = DateTime.Now.AddYears(-1), FlowRateOfAllSubstratesInDigestate = 600
                },

                new DigestorDailyOutput()
                {
                    Date = DateTime.Now.AddDays(5), FlowRateOfAllSubstratesInDigestate = 100
                },
            };

            _sut.SetStartingStateOfTank(tank, adResults, new Farm());

            Assert.AreEqual(300, tank.TotalDigestateAfterAllApplications);
        }

        [TestMethod]
        public void UpdateAmountsUsed()
        {
            var farm = new Farm();
            var field = new FieldSystemComponent();
            var crop = new CropViewItem();
            var digestateApplication = new DigestateApplicationViewItem();
            var digestateTank = new DigestateTank();

            crop.Area = 10;
            digestateApplication.MaximumAmountOfDigestateAvailablePerHectare = 1000;
            digestateApplication.AmountAppliedPerHectare = 100;

            farm.Components.Add(field);
            field.CropViewItems.Add(crop);
            crop.DigestateApplicationViewItems.Add(digestateApplication);

            _sut.UpdateAmountsUsed(digestateTank, farm);

            Assert.AreEqual(digestateTank.VolumeSumOfAllManureApplicationsMade, 1000);
        }

        [TestMethod]
        public void GetTankTest()
        {
            var farm = new Farm();
            var fieldComponent = new FieldSystemComponent();

            var crop = new CropViewItem();
            fieldComponent.CropViewItems.Add(crop);

            var digestateApplication = new DigestateApplicationViewItem();
            crop.DigestateApplicationViewItems.Add(digestateApplication);

            digestateApplication.AmountOfNitrogenAppliedPerHectare = 100;

            farm.Components.Add(fieldComponent);
            
            var year = 2022;

            var targetDate = new DateTime(year, 3, 1);

            _mockAdCalculator
                .Setup(x => x.CalculateResults(It.IsAny<Farm>(), It.IsAny<List<AnimalComponentEmissionsResults>>()))
                .Returns(new List<DigestorDailyOutput>());

            var tank = _sut.GetTank(farm, targetDate.Year, DigestateState.Raw, new List<DigestorDailyOutput>());

            Assert.IsNotNull(tank);
            Assert.AreEqual(2022, tank.Year);
        }

        [TestMethod]
        public void ReduceTankByDigestateApplicationsTest()
        {
            var farm = new Farm();
            var fieldComponent = new FieldSystemComponent();

            var crop = new CropViewItem();
            fieldComponent.CropViewItems.Add(crop);

            var digestateApplication = new DigestateApplicationViewItem();
            crop.DigestateApplicationViewItems.Add(digestateApplication);

            digestateApplication.DigestateState = DigestateState.SolidPhase;
            digestateApplication.MaximumAmountOfDigestateAvailablePerHectare = 100;
            digestateApplication.AmountAppliedPerHectare = 75;

            farm.Components.Add(fieldComponent);

            var digestateTank = new DigestateTank();
            digestateTank.DigestateState = DigestateState.SolidPhase;
            digestateTank.TotalDigestateAfterAllApplications = 100;

            _sut.ReduceTankByDigestateApplications(farm, digestateTank);

            Assert.AreEqual(25, digestateTank.TotalDigestateAfterAllApplications);
        }

        [TestMethod]
        public void InitializeTest()
        {
            var farm = new Farm();

            _sut.Initialize(farm);
        }

        #endregion
    }
}