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
            var viewItem = new FarmResiduesSubstrateViewItem() {FarmResidueType = FarmResidueType.BarleyStraw};
            viewItem.FlowRate = 0.5;
            viewItem.TotalSolids = 0.5;
            viewItem.VolatileSolids = 0.5;
            viewItem.TotalNitrogen = 0.5;
            viewItem.TotalCarbon = 0.5;
            viewItem.BiomethanePotential = 0.5;
            viewItem.MethaneFraction = 0.5;

            var component = new AnaerobicDigestionComponent();
            component.HydraulicRetentionTimeInDays = 30;

            component.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems.Add(viewItem);

            var farm = new Farm();
            farm.Components.Add(component);

            var groupEmissionsByDay = new GroupEmissionsByDay();

            var managementPeriod = new ManagementPeriod();

            _calculator.CalculateResults(farm, groupEmissionsByDay, managementPeriod);
        } 

        #endregion
    }
}
