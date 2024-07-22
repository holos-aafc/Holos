using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;

namespace H.Core.Test.Calculators.Carbon
{
    [TestClass]
    public class ICBMCarbonInputCalculatorTest
    {
        #region Fields

        private ICBMCarbonInputCalculator _sut;

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
            _sut = new ICBMCarbonInputCalculator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void CalculateInputsFromSupplementalHayFedToGrazingAnimals()
        {
            var farm = new Farm()
            {
                Defaults = new Defaults()
                {
                    DefaultSupplementalFeedingLossPercentage = 20,
                },
            };

            var currentYearViewItem = new CropViewItem()
            {
                CarbonConcentration = 0.45,

                // This is a supplemental feeding to grazing animals and the extra carbon left over once animals are finished must be accounted for in total above ground inputs
                HayImportViewItems = new ObservableCollection<HayImportViewItem>()
                {
                    new HayImportViewItem()
                    {
                        NumberOfBales = 10,
                        BaleWeight = 500,
                        MoistureContentAsPercentage = 12,
                    }
                }
            };

            var result = _sut.CalculateInputsFromSupplementalHayFedToGrazingAnimals(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItems: null,
                farm: farm);

            // = [(10 * 500) * (12/100) * (1 - 20/100)] * 0.45
            // = (5000 * 0.88 * 0.8) * 0.45
            // = 1584

            Assert.AreEqual(396, result);
        }

        /// <summary>
        /// Equation 2.2.5-3
        /// </summary>
        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProduct()
        {
            var currentYearViewItem = new CropViewItem()
            {
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 22,
                CropType = CropType.Barley,
                MoistureContentOfCrop = 0.12,
            };

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, currentYearViewItem, new Farm());

            // = [(1000 + 1000 * .22) * (1 - 0.12)] * 0.45
            // = 1220 * 0.88 * 0.45

            Assert.AreEqual(483.12, result, delta: 2);
        }

        /// <summary>
        /// Equation 2.2.5-3
        /// </summary>
        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductGreenManure()
        {
            var currentYearViewItem = new CropViewItem()
            {
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 100, // Indicates green manure
                CropType = CropType.Barley,
                MoistureContentOfCrop = 0.12,
            };

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, currentYearViewItem, new Farm());

            // = [(1000) * (1 - 0.12)] * 0.45
            // = 1000 * 0.88 * 0.45

            Assert.AreEqual(396, result, delta: 2);
        }

        [TestMethod]
        public void CalculateProductivity()
        {
            const double annualPrecipitation = 806;
            const double annualPotentialEvapotranspiration = 618;
            const double proportionOfPrecipitationMayToSeptember = 0.04;
            const double moistureContent = 13;
            const double carbonConcentration = 0.45;

            var result = _sut.CalculateProductivity(
                annualPrecipitation: annualPrecipitation,
                annualPotentialEvapotranspiration: annualPotentialEvapotranspiration,
                proportionOfPrecipitationMayThroughSeptember: proportionOfPrecipitationMayToSeptember,
                carbonConcentration: carbonConcentration);

            Assert.AreEqual(87.578217100325958, result, 2);
        }

        /// <summary>
        /// Equation 2.2.2-6
        /// Equation 2.2.2-13
        /// Equation 2.2.2-17
        /// Equation 2.2.2-20
        /// Equation 2.2.2-23
        /// </summary>
        [TestMethod]
        public void CalculateCarbonInputFromProduct()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                PlantCarbonInAgriculturalProduct = 200,
                PercentageOfProductYieldReturnedToSoil = 10,
                Yield = 1000,
                MoistureContentOfCrop = 0.12,
                CarbonConcentration = 0.45,
            };

            var result = _sut.CalculateCarbonInputFromProduct(null, currentYearViewItem, null, new Farm());

            // C_ptoSoil = C_p * (S_p / 100)
            // = 200 * (10 / 100)
            // = 20

            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void CalculateCarbonInputFromRootsForPerennialsWhenCurrentYearInputsIsGreaterThanPreviousYearInputs()
        {
            var previousYearViewItem = new CropViewItem()
            {
                CarbonInputFromRoots = 5000,
            };

            var currentYearViewItem = new CropViewItem()
            {
                PlantCarbonInAgriculturalProduct = 100,
                BiomassCoefficientProduct = 0.15,
                BiomassCoefficientRoots = 0.04,
                PercentageOfRootsReturnedToSoil = 100,
            };

            var farm = new Farm();

            var result = _sut.CalculateCarbonInputFromRootsForPerennials(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                farm);

            Assert.AreEqual(5967.5, result, 1);
        }

        [TestMethod]
        public void CalculateCarbonInputFromRootsForPerennialsWhenCurrentYearInputsIsLessThanPreviousYearInputs()
        {
            var previousYearViewItem = new CropViewItem()
            {
                CarbonInputFromRoots = 50,
            };

            var currentYearViewItem = new CropViewItem()
            {
                PlantCarbonInAgriculturalProduct = 100,
                BiomassCoefficientProduct = 0.15,
                BiomassCoefficientRoots = 0.04,
                PercentageOfRootsReturnedToSoil = 100,
            };

            var farm = new Farm();

            var result = _sut.CalculateCarbonInputFromRootsForPerennials(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                farm);

            // For perennials, the C input from roots in the current year cannot be less than the carbon input from roots in the previous year
            Assert.AreEqual(59.675, result);
        }

        [TestMethod]
        public void CalculateCarbonInputFromExtrarootsForPerennialsWhenCurrentYearInputsIsGreaterThanPreviousYearInputs()
        {
            var previousYearViewItem = new CropViewItem()
            {
                CarbonInputFromExtraroots = 10,
            };

            var currentYearViewItem = new CropViewItem()
            {
                PlantCarbonInAgriculturalProduct = 100,
                BiomassCoefficientProduct = 0.15,
                BiomassCoefficientExtraroot = 0.04,
            };

            var farm = new Farm();

            var result = _sut.CalculateCarbonInputFromExtrarootsForPerennials(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                farm);

            Assert.AreEqual(26.7, result, 1);
        }

        [TestMethod]
        public void CalculateCarbonInputFromExtrarootsForPerennialsWhenCurrentYearInputsIsLessThanPreviousYearInputs()
        {
            var previousYearViewItem = new CropViewItem()
            {
                CarbonInputFromExtraroots = 50,
            };

            var currentYearViewItem = new CropViewItem()
            {
                PlantCarbonInAgriculturalProduct = 100,
                BiomassCoefficientProduct = 0.15,
                BiomassCoefficientExtraroot = 0.04,
            };

            var farm = new Farm();

            var result = _sut.CalculateCarbonInputFromExtrarootsForPerennials(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                farm);

            // For perennials, the C input from extraroots in the current year cannot be less than the carbon input from extraroots in the previous year
            Assert.AreEqual(26.6, result, 0.1);
        }

        #endregion
    }
}
