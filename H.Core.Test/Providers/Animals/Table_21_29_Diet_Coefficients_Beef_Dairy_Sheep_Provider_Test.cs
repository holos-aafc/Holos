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
    public class Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Provider_Test
    {
        #region Fields

        private static Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Provider _dietCoefficientsProvider;
        private Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Data _dietCoefficientsData;

        #endregion


        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _dietCoefficientsProvider = new Table_21_29_Diet_Coefficients_Beef_Dairy_Sheep_Provider();
        }

        [ClassCleanup]
        public static void ClassCleanup() { }

        #endregion


        #region Tests

        [TestMethod]
        public void GetBeefCattleDietCoefficients()
        {
            _dietCoefficientsData = _dietCoefficientsProvider.GetDietCoefficientsDataInstance(AnimalType.CowCalf, DietType.LowEnergyAndProtein);

            Assert.AreEqual(_dietCoefficientsData.TotalDigestibleNutrients, 48);
            Assert.AreEqual(_dietCoefficientsData.CrudeProteinContent, 0.057);

            _dietCoefficientsData = _dietCoefficientsProvider.GetDietCoefficientsDataInstance(AnimalType.BeefBackgrounder, DietType.MediumGrowth);

            Assert.AreEqual(_dietCoefficientsData.MethaneConversionFactor, 0.063);

            _dietCoefficientsData = _dietCoefficientsProvider.GetDietCoefficientsDataInstance(AnimalType.BeefFinisher, DietType.Corn);
            Assert.AreEqual(_dietCoefficientsData.TotalDigestibleNutrients, 84);
            Assert.AreEqual(_dietCoefficientsData.MethaneConversionFactor, 0.03);

        }

        [TestMethod]
        public void GetDairyCattleDietCoefficients()
        {
            _dietCoefficientsData = _dietCoefficientsProvider.GetDietCoefficientsDataInstance(AnimalType.DairyDryCow, DietType.CloseUp);
            Assert.AreEqual(_dietCoefficientsData.TotalDigestibleNutrients, 69);
            Assert.AreEqual(_dietCoefficientsData.CrudeProteinContent, 18.32);

            _dietCoefficientsData = _dietCoefficientsProvider.GetDietCoefficientsDataInstance(AnimalType.DairyBulls, DietType.MediumEnergy);
            Assert.AreEqual(_dietCoefficientsData.TotalDigestibleNutrients, 68);
            Assert.AreEqual(_dietCoefficientsData.MethaneConversionFactor, 0.065);

        }


        [TestMethod]
        public void IncorrectParameterTest()
        {
            _dietCoefficientsData = _dietCoefficientsProvider.GetDietCoefficientsDataInstance(AnimalType.Swine, DietType.CloseUp);
            Assert.AreEqual(null, _dietCoefficientsData);

            _dietCoefficientsData = _dietCoefficientsProvider.GetDietCoefficientsDataInstance(AnimalType.DairyHeifers, DietType.BarleySilageBased);
            Assert.AreEqual(null, _dietCoefficientsData);
        }

        #endregion
    }
}
