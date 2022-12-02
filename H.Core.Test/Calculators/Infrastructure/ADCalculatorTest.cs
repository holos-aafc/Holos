using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

        private ADCalculator _calculator;

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
            _calculator = new ADCalculator();
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
            var residuesSubstrateViewItem = new FarmResiduesSubstrateViewItem() {FarmResidueType = FarmResidueType.BarleyStraw};
            residuesSubstrateViewItem.FlowRate = 0.5;
            residuesSubstrateViewItem.TotalSolids = 0.5;
            residuesSubstrateViewItem.VolatileSolids = 0.5;
            residuesSubstrateViewItem.TotalNitrogen = 0.5;
            residuesSubstrateViewItem.TotalCarbon = 0.5;
            residuesSubstrateViewItem.BiomethanePotential = 0.5;
            residuesSubstrateViewItem.MethaneFraction = 0.5;

            var manureSubstrateViewItem = new ManureSubstrateViewItem() {AnimalType = AnimalType.Beef, IsFreshManure = true};
            manureSubstrateViewItem.FlowRate = 1.25;
            manureSubstrateViewItem.TotalSolids = 0.75;
            manureSubstrateViewItem.VolatileSolids = 0.99;
            manureSubstrateViewItem.TotalNitrogen = 0.33;
            manureSubstrateViewItem.TotalCarbon = 0.11;
            manureSubstrateViewItem.BiomethanePotential = 0.22;
            manureSubstrateViewItem.MethaneFraction = 2;

            var component = new AnaerobicDigestionComponent();
            component.HydraulicRetentionTimeInDays = 30;
            component.ProportionTotalManureAddedToAD = 0.5;

            component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(residuesSubstrateViewItem);
            component.AnaerobicDigestionViewItem.ManureSubstrateViewItems.Add(manureSubstrateViewItem);

            var farm = new Farm();
            farm.Components.Add(component);

            var groupEmissionsByDay = new GroupEmissionsByDay();
            groupEmissionsByDay.TotalVolumeOfManureAvailableForLandApplication = 100;
            groupEmissionsByDay.VolatileSolids = 50;
            groupEmissionsByDay.AmountOfNitrogenExcreted = 10;
            groupEmissionsByDay.AmountOfNitrogenAddedFromBedding = 10;
            groupEmissionsByDay.OrganicNitrogenInStoredManure = 20;
            groupEmissionsByDay.TanExcretion = 10;
            groupEmissionsByDay.CarbonFromManureAndBedding = 12;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.ManureDetails.DailyTanExcretion = 0.25;

            _calculator.CalculateResults(farm, groupEmissionsByDay, managementPeriod);
        } 

        #endregion
    }
}
