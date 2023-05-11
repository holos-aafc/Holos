using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            var result = _sut.Data;

            Assert.AreEqual(16, result.Keys.Count);
        }

        [TestMethod]
        public void GetReturnsCorrectValue()
        {
            var result = _sut.Data;

            Assert.AreEqual(50, result[AnimalType.Bison][ManureStateType.Pasture]);
        }

        [TestMethod]
        public void GetReturnsCorrectValueForAnimalType()
        {
            //var result = _sut.GetManureExcretionRate()

            //Assert.AreEqual(50, result[AnimalType.Bison][ManureStateType.Pasture]);
        }

        #endregion
    }
}
