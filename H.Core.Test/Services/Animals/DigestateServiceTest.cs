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

namespace H.Core.Test.Services.Animals
{
    [TestClass]
    public class DigestateServiceTest
    {
        #region Fields

        private DigestateService _sut;
        private Farm _farm;
        private List<AnimalComponentEmissionsResults> _animalComponentResults;

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
        public void ResetTankTest()
        {
            _sut.ResetTank(new DigestateTank(), _farm);
        }

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
            var adResults = new List<DigestorDailyOutput>() {new DigestorDailyOutput() {TotalNitrogenInDigestateAvailableForLandApplication = 100, TotalCarbonInDigestateAvailableForLandApplication = 200}};

            _sut.SetStartingStateOfTank(tank, adResults);

            Assert.AreEqual(200, tank.TotalAmountOfCarbonInStoredManure);
            Assert.AreEqual(100, tank.TotalAvailableManureNitrogenAvailableForLandApplication);
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
            digestateApplication.AmountAppliedPerHectare = 100;

            farm.Components.Add(field);
            field.CropViewItems.Add(crop);
            crop.DigestateApplicationViewItems.Add(digestateApplication);

            _sut.UpdateAmountsUsed(digestateTank, farm);

            Assert.AreEqual(digestateTank.VolumeSumOfAllManureApplicationsMade, 1000);
        }

        #endregion
    }
}
