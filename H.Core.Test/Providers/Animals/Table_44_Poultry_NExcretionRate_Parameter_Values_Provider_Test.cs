using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Core.Providers.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Animals
{
    [TestClass]
    public class Table_44_Poultry_NExcretionRate_Parameter_Values_Provider_Test
    {
        #region Fields

        private static Table_44_Poultry_NExcretionRate_Parameter_Values_Provider _provider;


        #endregion


        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_44_Poultry_NExcretionRate_Parameter_Values_Provider();
        }


        #endregion

        #region Tests

        [TestMethod]
        public void GetPulletsInformation()
        {
            var data = _provider.GetParameterValues(AnimalType.ChickenPullets);

            Assert.AreEqual(0.0447, data.DailyMeanIntake);
            Assert.AreEqual(18.2, data.CrudeProtein);
            Assert.AreEqual(133, data.ProductionPeriod);
        }

        [TestMethod]
        public void GetBroilersInformation()
        {
            var data = _provider.GetParameterValues(AnimalType.Broilers);

            Assert.AreEqual(0.095, data.DailyMeanIntake);
            Assert.AreEqual(2, data.FinalWeight);
            Assert.AreEqual(0.043, data.InitialWeight);
        }


        [TestMethod]
        public void GetWetDryLayersInformation()
        {
            var data = _provider.GetParameterValues(AnimalType.LayersDryPoultry);

            Assert.AreEqual(0.175, data.ProteinLiveWeight);
            Assert.AreEqual(0.0015, data.WeightGain);
            Assert.AreEqual(0.12, data.ProteinContentEgg);

            data = _provider.GetParameterValues(AnimalType.LayersWetPoultry);
            Assert.AreEqual(48.50, data.EggProduction);

        }

        [TestMethod]
        public void GetLayersInformation()
        {
            var data = _provider.GetParameterValues(AnimalType.Layers);

            Assert.AreEqual(0.175, data.ProteinLiveWeight);
            Assert.AreEqual(0.0015, data.WeightGain);
            Assert.AreEqual(0.12, data.ProteinContentEgg);

            data = _provider.GetParameterValues(AnimalType.LayersWetPoultry);
            Assert.AreEqual(48.50, data.EggProduction);

        }

        [TestMethod]
        public void GetHensInformation()
        {
            var data = _provider.GetParameterValues(AnimalType.ChickenHens);

            Assert.AreEqual(0.175, data.ProteinLiveWeight);
            Assert.AreEqual(0.0015, data.WeightGain);
            Assert.AreEqual(0.12, data.ProteinContentEgg);
            Assert.AreEqual(48.50, data.EggProduction);

        }


        [TestMethod]
        public void GetAllData()
        {
            var data = _provider.PoultryParameterValueData;

            Assert.AreEqual(8, data.Count);
        }

        [TestMethod]
        public void TestIncorrectAnimalType()
        {
            var data = _provider.GetParameterValues(AnimalType.CowCalf);
            Assert.AreEqual(0, data.CrudeProtein);
        }


        #endregion

    }
}
