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
    public class Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Provider_Test
    {

        #region Fields

        private static Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Provider _provider;

        #endregion



        #region Initialization
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _provider = new Table_45_Poultry_OtherLivestock_Default_NExcretionRates_Provider();
        }

        #endregion


        #region Tests

        [TestMethod]
        public void GetPerHeadExcretionRateValue()
        {
            var value = _provider.GetNitrogenExcretionRateValue(AnimalType.TurkeyHen);
            Assert.AreEqual(0.0050, value);


            value = _provider.GetNitrogenExcretionRateValue(AnimalType.Geese);
            Assert.AreEqual(0.0033, value);

            value = _provider.GetNitrogenExcretionRateValue(AnimalType.Bison);
            Assert.AreEqual(0.2320, value);
        }

        [TestMethod]
        public void GetAverageAnimalWeight()
        {
            var data = _provider.GetNExcretionRateByAnimalType(AnimalType.Horses);

            Assert.AreEqual(450, data.AverageLiveAnimalWeight);

            data = _provider.GetNExcretionRateByAnimalType(AnimalType.Llamas);

            Assert.AreEqual(112, data.AverageLiveAnimalWeight);
        }



        [TestMethod]
        public void GetChickenAnimalValues()
        {
            var value = _provider.GetNitrogenExcretionRateValue(AnimalType.ChickenPullets);
            Assert.AreEqual(0.0009, value);

            value = _provider.GetNitrogenExcretionRateValue(AnimalType.ChickenCockerels);
            Assert.AreEqual(0.0009, value);

            value = _provider.GetNitrogenExcretionRateValue(AnimalType.ChickenHens);
            Assert.AreEqual(0.0017, value);

            value = _provider.GetNitrogenExcretionRateValue(AnimalType.ChickenRoosters);
            Assert.AreEqual(0.0022, value);
        }

        [TestMethod]
        public void GetChickenAnimalWeights()
        {
            var data = _provider.GetNExcretionRateByAnimalType(AnimalType.ChickenPullets);
            Assert.AreEqual(0.725, data.AverageLiveAnimalWeight);

            data = _provider.GetNExcretionRateByAnimalType(AnimalType.ChickenCockerels);
            Assert.AreEqual(0.725, data.AverageLiveAnimalWeight);

            data = _provider.GetNExcretionRateByAnimalType(AnimalType.ChickenHens);
            Assert.AreEqual(1.8, data.AverageLiveAnimalWeight);

            data = _provider.GetNExcretionRateByAnimalType(AnimalType.ChickenRoosters);
            Assert.AreEqual(0.9, data.AverageLiveAnimalWeight);
        }

        #endregion
    }
}
