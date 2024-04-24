
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals.Table_28;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services.Animals
{
    [TestClass]
    public class ScaleUpServiceTest
    {
        #region Fields

        private ITable_28_Production_Days_Provider _table28ProductionDaysProvider;
        private IScaleUpService _scaleUpService;

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
            _table28ProductionDaysProvider = new Table_28_Production_Days_Provider();
            _scaleUpService = new ScaleUpService(_table28ProductionDaysProvider);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void ShouldScaleUpReturnsTrue()
        {
            var result = _scaleUpService.ShouldScaleUp(true, AnimalType.BeefBackgrounderHeifer, ProductionStages.GrowingAndFinishing, ComponentType.Backgrounding, new Farm() { Defaults = new Defaults() { ScaleUpEmissionsEnabled = true } });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldScaleUpReturnsFalse()
        {
            var result = _scaleUpService.ShouldScaleUp(true, AnimalType.BeefCowLactating, ProductionStages.Lactating, ComponentType.CowCalf, new Farm() {Defaults = new Defaults() {ScaleUpEmissionsEnabled = true}});

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ScaleUpEmissionsReturnsCorrectValue()
        {
            var result = _scaleUpService.ScaleUpEmissions(100, 100, 50);
            Assert.AreEqual((CoreConstants.DaysInYear / (50 + 100) * 100), result, 2);
        }

        #endregion
    }
}
