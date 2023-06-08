using H.Core.Providers.Animals.Table_69;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals.Table_28;

namespace H.Core.Test.Providers.Animals.Table_28
{
    [TestClass]
    public class Table_28_Production_Days_Provider_Test
    {
        #region Fields

        private ITable_28_Production_Days_Provider _sut;

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
            _sut = new Table_28_Production_Days_Provider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetBackgroundingDataTest()
        {
            var result = _sut.GetBackgroundingData();

            Assert.AreEqual(109, result.NumberOfDaysInProductionCycle);
        }

        [TestMethod]
        public void GetSwineDataTest()
        {
            var result = _sut.GetSwineData(AnimalType.SwineSows, ComponentType.FarrowToFinish);

            Assert.AreEqual(140, result.NumberOfDaysInProductionCycle);
        }

        [TestMethod]
        public void GetSwineWeanerDataTest()
        {
            var result = _sut.GetSwineData(AnimalType.SwinePiglets, ComponentType.IsoWean, ProductionStages.Weaning);

            Assert.AreEqual(35, result.NumberOfDaysInProductionCycle);
        }

        [TestMethod]
        public void GetPoultryDataTest()
        {
            var result = _sut.GetPoultryData(AnimalType.ChickenPullets);

            Assert.AreEqual(133, result.NumberOfDaysInProductionCycle);
        }

        #endregion
    }
}
