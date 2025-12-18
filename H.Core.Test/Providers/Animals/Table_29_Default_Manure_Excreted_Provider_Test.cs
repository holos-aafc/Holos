using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_29_Default_Manure_Excreted_Provider_Test
    {
        #region Fields
        
        private static Table_29_Default_Manure_Excreted_Provider _sut; 

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _sut = new Table_29_Default_Manure_Excreted_Provider();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        #endregion

        #region Tests
        
        [TestMethod]
        public void GetNumberOfLinesReturnsCorrectCount()
        {
            var result = _sut.PercentageTotalManureProducedInSystem;

            Assert.AreEqual(19, result.Keys.Count);
        }

        [TestMethod]
        public void GetReturnsCorrectValue()
        {
            var result = _sut.PercentageTotalManureProducedInSystem;

            Assert.AreEqual(50, result[AnimalType.Bison][ManureStateType.Pasture]);
        }

        [TestMethod]
        public void GetReturnsCorrectValueForAnimalType()
        {
            var result = _sut.GetPercentageOfManureProducedInSystem(AnimalType.BeefBackgrounder, ManureStateType.SolidStorageWithOrWithoutLitter);

            Assert.AreEqual(45, result);
        }

        [TestMethod]
        public void GetReturnsCorrectValueForAnimalType2()
        {
            var result = _sut.GetPercentageOfManureProducedInSystem(AnimalType.Sheep, ManureStateType.SolidStorageWithOrWithoutLitter);

            Assert.AreEqual(34, result);
        }

        /// <summary>
        /// Verify all animal type and manure state types are considered
        /// </summary>
        [TestMethod]
        public void GetReturnsCorrectValueForAll()
        {
            var results = new List<double>();

            foreach (var value in Enum.GetValues(typeof(AnimalType)).Cast<AnimalType>())
            {
                foreach (var manureStateType in Enum.GetValues(typeof(ManureStateType)).Cast<ManureStateType>())
                {
                    var result = _sut.GetPercentageOfManureProducedInSystem(value, manureStateType);

                    results.Add(result);
                }
            }

            Assert.IsTrue(results.All(x => x >= 0));
        }

        [TestMethod]
        public void GetManureExcretionRateReturnsCorrectValueForAnimalType()
        {
            var result = _sut.GetManureExcretionRate(AnimalType.BeefBackgrounder);

            Assert.AreEqual(-9, result);
        }

        [TestMethod]
        public void GetManureExcretionRateReturnsCorrectValueForAnimalType2()
        {
            var result = _sut.GetManureExcretionRate(AnimalType.Layers);

            Assert.AreEqual(0.12, result);
        }

        #endregion
    }
}
