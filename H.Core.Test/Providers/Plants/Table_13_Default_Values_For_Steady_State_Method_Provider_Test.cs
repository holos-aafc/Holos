using H.Core.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Core.Providers.Plants;

namespace H.Core.Test.Providers.Plants
{
    [TestClass]
    public class Table_13_Default_Values_For_Steady_State_Method_Provider_Test
    {
        #region Fields

        private static Table_13_Default_Values_For_Steady_State_Method_Provider _provider;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_13_Default_Values_For_Steady_State_Method_Provider();
        }

        [ClassCleanup]
        public static void ClassCleanup() { }


        [TestInitialize]
        public void TestIntialize() { }

        [TestCleanup]
        public void TestCleanup() { }
        #endregion

        #region Tests

        [TestMethod] 
        public void ReadAllData()
        {
            var data = _provider.DefaultValuesData;

            Assert.AreEqual(7, data.Count);
        }

        [TestMethod]
        public void GetCTONRatioValue()
        {
            var data = _provider.GetSteadyStateMethodValueForAnimalGroup(AnimalType.Dairy);
            Assert.AreEqual(16, data.CarbonToNitrogenRatio);

            data = _provider.GetSteadyStateMethodValueForAnimalGroup(AnimalType.Poultry);
            Assert.AreEqual(10, data.CarbonToNitrogenRatio);

            data = _provider.GetSteadyStateMethodValueForAnimalGroup(AnimalType.Mules);
            Assert.AreEqual(20, data.CarbonToNitrogenRatio);

        }


        [TestMethod]
        public void GetLigninContentValues()
        {
            var data = _provider.GetSteadyStateMethodValueForAnimalGroup(AnimalType.Beef);
            Assert.AreEqual(9, data.LigninContentManure);

            data = _provider.GetSteadyStateMethodValueForAnimalGroup(AnimalType.Poultry);
            Assert.AreEqual(5, data.LigninContentManure);

            data = _provider.GetSteadyStateMethodValueForAnimalGroup(AnimalType.Sheep);
            Assert.AreEqual(13, data.LigninContentManure);
        }

        [TestMethod]
        public void CheckIncorrectValueResult()
        {
            var data = _provider.GetSteadyStateMethodValueForAnimalGroup(AnimalType.Alpacas);
            Assert.AreEqual(data, null);
        }

        #endregion
    }
}
