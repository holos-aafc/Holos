using System;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace H.Core.Test.Services
{
    [TestClass]
    public class ManureServiceTest
    {
        #region Fields

        private ManureService _sut;

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
            _sut = new ManureService(new Mock<IAnimalService>().Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void InitializeTankTest()
        {
            var managementPeriod = new ManagementPeriod() {Start = DateTime.Now};

            managementPeriod.NumberOfDays = 365 * 12;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            var group = new AnimalGroup();
            group.ManagementPeriods.Add(managementPeriod);

            var component = new FinishingComponent();
            component.Groups.Add(group);

            var farm = new Farm();
            farm.Components.Add(component);
        }

        #endregion
    }
}