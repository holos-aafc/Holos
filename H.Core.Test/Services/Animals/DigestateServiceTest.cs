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

        #endregion
    }
}