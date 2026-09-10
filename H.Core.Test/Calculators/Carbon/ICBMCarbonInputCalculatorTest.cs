using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using H.Core.Emissions.Results;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using H.Core.Providers;

namespace H.Core.Test.Calculators.Carbon
{
    [TestClass]
    public class ICBMCarbonInputCalculatorTest : UnitTestBase
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
        public void CalculateAboveGroundCarbonInputForAnnuals()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 10,
                PercentageOfStrawReturnedToSoil = 20,
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.451,
                BiomassCoefficientStraw = 0.4,
                PlantCarbonInAgriculturalProduct = 435.6
            };

            var nextYearViewItem = new CropViewItem();

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };



            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 435.6

            // Cag = Cptosoil + Cs
            // = (Cp * %yieldReturned / 100) + (Cpotsoil * (Rs / Rp) * %strawReturned / 100);
            // = (435.6 * 0.1) * (435.6 * (0.4/0.451) * 0.2)
            // = 43.56 * 77.27

            Assert.AreEqual(120.83, currentYearViewItem.AboveGroundCarbonInput, 2);
        }

        [TestMethod]
        public void CalculateBelowGroundCarbonInputForAnnuals()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                Yield = 1000,
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 2,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                PerennialStandLength = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.5,
                BiomassCoefficientStraw = 0,
                BiomassCoefficientRoots = 0.4,
                BiomassCoefficientExtraroot = 0.3,
                PlantCarbonInAgriculturalProduct = 403.92
            };

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 403.92

            // Cbg = Cr + Ce
            // = [Cp * (Rr / Rp) * (Sr / 100)] + [Cp * (Re / Rp)]
            // = [403.92 * (0.4 / 0.5) * (100 / 100)] + [403.92 * (0.3 / 0.5)]
            // = [403.92 * 0.8 * 1] + [403.92 * 0.6]
            // = 323.136 + 242.352
            // = 565.488

            Assert.AreEqual(565.49, currentYearViewItem.BelowGroundCarbonInput, 2);
        }

        #endregion

        #region Perennials

        [TestMethod]
        public void CalculateAboveGroundCarbonInputForPerennials()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameMixed,
                Yield = 5000,
                PercentageOfProductYieldReturnedToSoil = 35,
                PercentageOfStrawReturnedToSoil = 0, // This will be set to zero for the user when they have selected a perennial
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.13,
                HarvestMethod = HarvestMethods.CashCrop,
            };

            var nextYearViewItem = new CropViewItem();

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 3011.538

            // Cag = Cptosoil
            // = (Cp * %yieldReturned / 100) 
            // = (3011.538 * 0.35) 
            // = 1054.03 

            Assert.AreEqual(1053.04, currentYearViewItem.AboveGroundCarbonInput, 2);
        }

        [TestMethod]
        public void CalculateAboveGroundCarbonInputForPerennialsWhenPlantCarbonForAgriculturalProductIsUnknownInCurrentYearButKnownInNextYear()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameMixed,
                Yield = 0,      // Set to 0 to set condition
                PercentageOfProductYieldReturnedToSoil = 10,
                PercentageOfStrawReturnedToSoil = 0, // This will be set to zero for the user when they have selected a perennial
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                YearInPerennialStand = 1,
            };

            var nextYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameMixed,
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 10,
                PercentageOfStrawReturnedToSoil = 0, // This will be set to zero for the user when they have selected a perennial
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                MoistureContentOfCropPercentage = 12,
                HarvestMethod = HarvestMethods.CashCrop,
                PlantCarbonInAgriculturalProduct = 435.6,
                YearInPerennialStand = 2,
            };

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                ClimateData = new ClimateData()
                {
                    PrecipitationData = new PrecipitationData()
                    {
                        January = 100,
                        May = 200,
                    },

                    EvapotranspirationData = new EvapotranspirationData()
                    {
                        January = 300,
                        May = 600,
                    }
                },

                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            //  Plant C in agricultural product for next year: 435.6

            // Cag = Cptosoil
            // = (Cp * %yieldReturned / 100) 
            // = (435.6 * 0.1) 
            // = 43.56 

            Assert.AreEqual(21.78, currentYearViewItem.AboveGroundCarbonInput);
        }

        [TestMethod]
        public void CalculateAboveGroundCarbonInputForPerennialsWhenPlantCarbonForAgriculturalProductIsUnknownInCurrentYearAndIsUnknownInNextYear()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameMixed,
                Yield = 0,      // Set to 0 to set condition
                PercentageOfProductYieldReturnedToSoil = 10,
                PercentageOfStrawReturnedToSoil = 0, // This will be set to zero for the user when they have selected a perennial
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                YearInPerennialStand = 1,
            };

            var nextYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameMixed,
                Yield = 0,      // Set to zero to set condition
                PercentageOfProductYieldReturnedToSoil = 10,
                PercentageOfStrawReturnedToSoil = 0, // This will be set to zero for the user when they have selected a perennial
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                MoistureContentOfCropPercentage = 12,
                HarvestMethod = HarvestMethods.CashCrop,
                YearInPerennialStand = 2,
            };

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                Defaults = new Defaults()
                {
                    EstablishmentGrowthFactorPercentageForPerennials = 50,
                },

                ClimateData = new ClimateData()
                {
                    PrecipitationData = new PrecipitationData()
                    {
                        January = 100,
                        May = 200,
                    },

                    EvapotranspirationData = new EvapotranspirationData()
                    {
                        January = 300,
                        May = 600,
                    }
                },

                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // See issue https://github.com/holos-aafc/Holos/issues/405
            var moistureCorrection = (1.0 - 0.13) / (1.0 - 0.8);
            var expected = 10.293 * (moistureCorrection);

            Assert.AreEqual(expected, currentYearViewItem.AboveGroundCarbonInput, 3);
        }

        [TestMethod]
        public void CalculateBelowGroundCarbonInputForPerennialsInFirstYear()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameMixed,
                Yield = 1000,
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 0,
                PercentageOfExtraRootsReturnedToSoil = 100,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.5,
                BiomassCoefficientRoots = 0.4,
                BiomassCoefficientExtraroot = 0.3,
                PlantCarbonInAgriculturalProduct = 396,

            };

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 391.5

            // Cbg = Cr + Ce
            // = [Cp * (Rr/Rp) * (Sr/100) * 1/standLength] + [Cp * (Re/Rp) * (Se/100)]
            // = [396 * (0.4/0.5) * (100/100) * 1/5] + [396 * (0.3 / 0.5) * 1]
            // = [396 * 0.8 * 1 * 0.2] + [396 * (0.6)]
            // = 63.36 + 237.6
            // = 300.96

            // * NOTE: the stand length was removed from this calculation (not considered anymore). New calculation should be:
            // Cbg = Cr + Ce
            // = [Cp * (Rr/Rp) * (Sr/100)] + [Cp * (Re/Rp)  * (Se/100)]
            // = [396 * (0.3/0.5) * (100/100)] + [396 * (0.3 / 0.5) * 1]
            // = [396 * 0.8 * 1] + [396 * (0.6)]
            // = [root input less than 450 so = 450] 
            // = 237.6 + 450
            // = 684.9

            Assert.AreEqual(687.6, currentYearViewItem.BelowGroundCarbonInput, 1);
        }

        [TestMethod]
        public void CalculateAboveGroundInputsForGreenManureHarvestMethod()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.Lentils,
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 2,
                PercentageOfStrawReturnedToSoil = 0,
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.GreenManure,
                BiomassCoefficientProduct = 0.5,
                BiomassCoefficientStraw = 0.2,
                PlantCarbonInAgriculturalProduct = 403.92
            };

            var nextYearViewItem = new CropViewItem();

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 403.92

            Assert.AreEqual(7.92, currentYearViewItem.AboveGroundCarbonInput);
        }

        [TestMethod]
        public void CalculateBelowGroundInputsForGreenManureHarvestMethod()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.Lentils,
                Yield = 1000,
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 2,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                PerennialStandLength = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.GreenManure,
                BiomassCoefficientProduct = 0.5,
                BiomassCoefficientStraw = 0.2,
                BiomassCoefficientRoots = 0.4,
                BiomassCoefficientExtraroot = 0.3,
                PlantCarbonInAgriculturalProduct = 403.92
            };

            var farm = new Farm()
            {
                Province = Province.Alberta,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(396, currentYearViewItem.BelowGroundCarbonInput, 3);
        }

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

        [TestMethod]
        public void GetSupplementalLossesReturnsEatenPortionOfBales()
        {
            // Equation 11.3.2-2 (b): the subtraction term that removes the "eaten" portion of
            // supplemental hay from total grazing carbon losses. Pins the regression that previously
            // over-subtracted this value by a factor of 1/Loss_feeding (~5x at the 20% default),
            // which under-counted grazing carbon losses for any field configured with both grazing
            // animals and supplemental hay. Complementary to the test above, which pins the
            // "wasted" portion that returns to soil (396 kg C); the two together cover the full
            // bale carbon (1980 kg C at these inputs).
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

            var result = _sut.GetSupplementalLosses(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItems: null,
                farm: farm);

            // bale_dry      = 10 * 500 * (1 - 12/100) = 4400 kg
            // total_bale_C  = 4400 * 0.45             = 1980 kg C
            // eaten portion = total_bale_C * (1 - 20/100) = 1980 * 0.8 = 1584 kg C
            Assert.AreEqual(1584, result);
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

            // = [(1000 / (1 - 0.22)) * (1 - 0.12)] * 0.45
            // = [(1000 / 0.78) * (0.88)] * 0.45

            Assert.AreEqual(507.69, result, delta: 2);
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

        /// <summary>
        /// Equation 2.2.5-3
        /// </summary>
        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductGrazedFieldAndCustomYieldAssignmentMethod()
        {
            var date = new DateTime(DateTime.Now.Year, 1, 1);

            var currentYearViewItem = new CropViewItem()
            {
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 10,
                CropType = CropType.Barley,
                MoistureContentOfCrop = 0.12,
                Year = date.Year,
            };

            currentYearViewItem.GrazingViewItems.Add(new GrazingViewItem() {Start = date});
            var farm = new Farm();
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, currentYearViewItem, farm);

            // = [(1000) * (1 - 0.80)] * 0.45
            // = 1000 * 0.88 * 0.45

            Assert.AreEqual(450, result, delta: 2);
        }

        /// <summary>
        /// Equation 2.2.5-3
        /// </summary>
        [TestMethod]
        public void CalculateAboveGroundCarbonInputFromPerennialsGrazedFieldAndCustomYieldAssignmentMethod()
        {
            var currentYearViewItem = new CropViewItem()
            {
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 10,
                CropType = CropType.Barley,
                MoistureContentOfCrop = 0.12,
                YearInPerennialStand = 1,
                PlantCarbonInAgriculturalProduct = 20,

                // The grazed branch is scoped to the item's own year, so the year and the grazing period have to agree.
                Year = 1985,
            };

            var grazingViewItem = new GrazingViewItem();
            grazingViewItem.Utilization = 55;
            grazingViewItem.Start = new DateTime(1985, 6, 1);

            currentYearViewItem.GrazingViewItems.Add(grazingViewItem);

            var farm = new Farm();
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            var result = _sut.CalculateAboveGroundCarbonInputFromPerennials(null, currentYearViewItem, null, farm);

            Assert.AreEqual(9, result);
        }

        /// <summary>
        /// When all product is returned to soil (percentage = 100, as the pipeline sets for a no-harvest / no-grazing
        /// perennial), plant C is the yield used directly - with no gross-up - for a non-Custom yield assignment method.
        /// </summary>
        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductPerennialAllProductReturnedUsesYieldDirectlyForNonCustom()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameGrass,
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 100,
                MoistureContentOfCrop = 0.12,
                Year = 1985,
            };

            var farm = new Farm(); // non-Custom (default) yield assignment method

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, currentYearViewItem, farm);

            // Yield used directly: 1000 * (1 - 0.12) * 0.45 = 396
            Assert.AreEqual(396, result, delta: 1);
        }

        /// <summary>
        /// The all-product-returned result must not depend on the yield assignment method - this guards the removal of
        /// the Custom-only "no harvest and no grazing" branch (both Custom and non-Custom now use the yield directly).
        /// </summary>
        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductPerennialAllProductReturnedIsMethodIndependent()
        {
            var nonCustomItem = new CropViewItem()
            {
                CropType = CropType.TameGrass,
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 100,
                MoistureContentOfCrop = 0.12,
                Year = 1985,
            };
            var customItem = new CropViewItem()
            {
                CropType = CropType.TameGrass,
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 100,
                MoistureContentOfCrop = 0.12,
                Year = 1985,
            };

            var nonCustomResult = _sut.CalculatePlantCarbonInAgriculturalProduct(null, nonCustomItem, new Farm());
            var customResult = _sut.CalculatePlantCarbonInAgriculturalProduct(null, customItem,
                new Farm() {YieldAssignmentMethod = YieldAssignmentMethod.Custom});

            Assert.AreEqual(nonCustomResult, customResult, delta: 0.0001);
            Assert.AreEqual(396, nonCustomResult, delta: 1);
        }

        /// <summary>
        /// Regression: a harvested perennial (product removed, percentage returned &lt; 100) still grosses the yield up
        /// by the fraction returned to soil. Removing the Custom no-harvest/no-grazing branch must not affect this path.
        /// </summary>
        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductHarvestedPerennialGrossesUpYield()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameGrass,
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 35,
                MoistureContentOfCrop = 0.12,
                Year = 1985,
            };
            currentYearViewItem.HarvestViewItems.Add(new HarvestViewItem()
                {Start = new DateTime(1985, 8, 1), ForageActivity = ForageActivities.Hayed});

            var farm = new Farm();

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, currentYearViewItem, farm);

            // Grossed up: [1000 / (1 - 0.35)] * (1 - 0.12) * 0.45 = 1538.46 * 0.88 * 0.45 = 609.2
            Assert.AreEqual(609.2, result, delta: 2);
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

            // See issue https://github.com/holos-aafc/Holos/issues/405
            var moistureCorrection = (1.0 - 0.13) / (1.0 - 0.8);
            var expected = 87.578217100325958 * moistureCorrection;

            Assert.AreEqual(expected, result, 2);
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
        public void CalculateCarbonInputFromRootsForPerennialsWhenCurrentYearInput()
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

        /// <summary>
        /// The stand is terminated, so the whole accumulated root mass is returned. The algorithm document gives this
        /// as Cr(final year of stand) = (Cr(t-1) / 3) * 10 - built from the previous year's root input, not from this
        /// year's above ground carbon.
        /// </summary>
        [TestMethod]
        public void CalculateCarbonInputFromRootsForPerennialsInFinalYearBuildsOnPreviousYear()
        {
            var standId = Guid.NewGuid();

            var previousYearViewItem = new CropViewItem()
            {
                PerennialStandGroupId = standId,
                CarbonInputFromRoots = 784.6,
            };

            var currentYearViewItem = new CropViewItem()
            {
                PerennialStandGroupId = standId,
                YearInPerennialStand = 13,
                PerennialStandLength = 13,
                PercentageOfRootsReturnedToSoil = 100,
                PlantCarbonInAgriculturalProduct = 1676.7,
                BiomassCoefficientRoots = 0.339,
                BiomassCoefficientProduct = 0.441,
            };

            var result = _sut.CalculateCarbonInputFromRootsForPerennials(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                new Farm());

            Assert.AreEqual((784.6 / 3.0) * 10.0, result, 0.01);

            // Guards the defect this replaced: the final year used to be recomputed from this year's above ground
            // carbon, which discarded every year of root accumulation and reported the establishment year's value.
            var aboveGroundDerivedValue = 1676.7 * (0.339 / 0.441);
            Assert.AreNotEqual(aboveGroundDerivedValue, result, 0.01);
        }

        /// <summary>
        /// The first year of a stand has no root mass carried in, so it is the one year that is built from the above
        /// ground carbon.
        /// </summary>
        [TestMethod]
        public void CalculateCarbonInputFromRootsForPerennialsInFirstYearUsesAboveGroundCarbon()
        {
            var currentYearViewItem = new CropViewItem()
            {
                YearInPerennialStand = 1,
                PerennialStandLength = 13,
                PercentageOfRootsReturnedToSoil = 30,
                PlantCarbonInAgriculturalProduct = 1676.7,
                BiomassCoefficientRoots = 0.339,
                BiomassCoefficientProduct = 0.441,
            };

            var result = _sut.CalculateCarbonInputFromRootsForPerennials(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                new Farm());

            Assert.AreEqual(1676.7 * (0.339 / 0.441) * 0.30, result, 0.01);
        }

        /// <summary>
        /// A stand that runs to the end of the simulation, or a native rangeland, is never terminated - it keeps the
        /// 30% annual turnover in its last year, so that year continues the chain rather than returning the whole
        /// root mass.
        /// </summary>
        [TestMethod]
        public void CalculateCarbonInputFromRootsForPerennialsInFinalYearWithoutTerminationContinuesTheChain()
        {
            var standId = Guid.NewGuid();

            var previousYearViewItem = new CropViewItem()
            {
                PerennialStandGroupId = standId,
                CarbonInputFromRoots = 784.6,
            };

            var currentYearViewItem = new CropViewItem()
            {
                PerennialStandGroupId = standId,
                YearInPerennialStand = 13,
                PerennialStandLength = 13,
                PercentageOfRootsReturnedToSoil = 30,
                PlantCarbonInAgriculturalProduct = 1676.7,
                BiomassCoefficientRoots = 0.339,
                BiomassCoefficientProduct = 0.441,
            };

            var result = _sut.CalculateCarbonInputFromRootsForPerennials(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                new Farm());

            // Past year 5 the chain is flat, so the last year matches the one before it
            Assert.AreEqual(784.6, result, 0.01);
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
                PercentageOfExtraRootsReturnedToSoil = 100,
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
                PercentageOfExtraRootsReturnedToSoil = 100,
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

        #region Silage

        [TestMethod]
        public void CalculateAboveGroundCarbonInputForSilage()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.BarleySilage,
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 35, // v4 default
                PercentageOfStrawReturnedToSoil = 0, // This will be set to zero for the user when they have selected a silage
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0,
                BiomassCoefficientStraw = 0,
                PlantCarbonInAgriculturalProduct = 534.6
            };

            var nextYearViewItem = new CropViewItem();

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 609.23

            // Cag = Cptosoil + Cs 
            // = [Cp * (Sp/100)] + 0 (since straw inputs are not calculated for silage crops)
            // = 609.23 * 0.35
            // = 187.11

            Assert.AreEqual(213.23, currentYearViewItem.AboveGroundCarbonInput, 2);
        }

        [TestMethod]
        public void CalculateBelowGroundCarbonInputForSilage()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.BarleySilage,
                Yield = 1000,
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 35,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                PerennialStandLength = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.2,
                BiomassCoefficientStraw = 0.5,
                BiomassCoefficientRoots = 0.4,
                BiomassCoefficientExtraroot = 0.3,
                PlantCarbonInAgriculturalProduct = 534.6
            };

            var farm = new Farm()
            {
                Province = Province.Alberta,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 609.23

            // Cbg = Cr + Ce
            // = [Cp * (Rr/Rp) * (Sr/100)] + [Cp * (Re/Rp)]
            // = [609.23 * (0.4/0.2) * 100/100] + [609.23 * (0.3/0.2)]
            // = [609.23 * 2 * 1] + [609.23 * 1.5]
            // = 1218.46 + 913.845
            // = 2132.305

            Assert.AreEqual(2132.305, currentYearViewItem.BelowGroundCarbonInput, 1);
        }

        #endregion

        #region Root Crops

        [TestMethod]
        public void CalculateAboveGroundCarbonInputForRootCrops()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.Potatoes,
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 0,
                PercentageOfStrawReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.736,
                BiomassCoefficientStraw = 0.239,
                PlantCarbonInAgriculturalProduct = 396
            };

            var nextYearViewItem = new CropViewItem();

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 396

            // Cag = Cs
            // = Cp * (Rs/Rp) * (Ss / 100)
            // = 396 * (0.239 / 0.736) * (100 / 100)
            // = 396 * 0.324 * 1
            // = 128.59

            Assert.AreEqual(128.59, currentYearViewItem.AboveGroundCarbonInput, 2);
        }

        [TestMethod]
        public void CalculateBelowGroundCarbonInputForRootCrops()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.Potatoes,
                Yield = 1000,
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 0,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                PerennialStandLength = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.736,
                BiomassCoefficientStraw = 0.239,
                BiomassCoefficientRoots = 0.015,
                BiomassCoefficientExtraroot = 0.01,
                PlantCarbonInAgriculturalProduct = 396
            };

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 396

            // Cbg = CptoSoil + Ce
            // = [Cp * (Sp / 100)] + [Cp * (Re / Rp)]
            // = [396 * (0 / 100)] + [396 * (0.01 / 0.736)]
            // = 0 + [396 * 0.0135]
            // = 5.38

            Assert.AreEqual(5.38, currentYearViewItem.BelowGroundCarbonInput, 2);
        }

        #endregion

        #region Cover Crops

        /// <summary>
        /// Test above ground carbon input calculation when the user enters the 'main' crop as a cover crop i.e. 'Winter Wheat' (instead of specifying fallow for that crop).
        /// </summary>
        [TestMethod]
        public void CalculateAboveGroundCarbonInputFromCoverCropsWhenCoverCropIsUsedAsMainCrop()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.WinterWheat, // Under 'Small Grain Cereals' in residue table
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 50,
                PercentageOfStrawReturnedToSoil = 10,
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.451,
                BiomassCoefficientStraw = 0.340,
            };

            var nextYearViewItem = new CropViewItem();

            var farm = new Farm()
            {
                Province = Province.Alberta,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 792

            // Cag = CptoSoil + Cs
            // = [Cp * (Sp / 100)] + [Cp * (Rs / Rp) * (Ss / 100)]
            // = [792 * (50 / 100)] + [792 * (0.340 / 0.451) * (10 / 100)]
            // = [792 * 0.5] + [792 * 0.753 * 0.1] 
            // = 396 + 59.6376
            // = 455.6376

            Assert.AreEqual(455.6376, currentYearViewItem.AboveGroundCarbonInput, 4);
        }

        /// <summary>
        /// Test below ground carbon input calculation when the user enters the 'main' crop as a cover crop i.e. 'Winter Wheat' (instead of specifying fallow for that crop).
        /// </summary>
        [TestMethod]
        public void CalculateBelowGroundCarbonInputFromCoverCropsWhenCoverCropIsUsedAsMainCrop()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.WinterWheat, // Under 'Small Grain Cereals' in residue table
                Yield = 1000,
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 100,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                PerennialStandLength = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.451,
                BiomassCoefficientStraw = 0.340,
                BiomassCoefficientRoots = 0.126,
                BiomassCoefficientExtraroot = 0.082,
                PlantCarbonInAgriculturalProduct = 396
            };

            var farm = new Farm()
            {
                Province = Province.Alberta,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            // Plant C in agricultural product: Cp = 396           

            // Cbg = Cr + Ce
            // = [Cp * (Rr / Rp) * (Sr / 100)] + [Cp * (Re / Rp)]
            // = [396 * (0.126 / 0.451) * (100 / 100)] + [396 * (0.082 / 0.451)]
            // = [396 * 0.2793 * 1] + [396 * 0.181]
            // = 110.6 + 71.676
            // = 182.28

            Assert.AreEqual(182.28, currentYearViewItem.BelowGroundCarbonInput, 1);
        }

        /// <summary>
        /// Test above ground carbon input calculation when the user enters the 'main' crop (i.e. 'Winter Wheat') as a cover crop and also specifies 
        /// green manure harvest type.
        /// </summary>
        [TestMethod]
        public void CalculateAboveGroundCarbonInputFromCoverCropsWhenCoverCropIsUsedAsGreenManure()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.WinterWheat, // Under 'Small Grain Cereals' in residue table
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 50,
                PercentageOfStrawReturnedToSoil = 10,
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.GreenManure,
                BiomassCoefficientProduct = 0.451,
                BiomassCoefficientStraw = 0.340,
                PlantCarbonInAgriculturalProduct = 594
            };

            var nextYearViewItem = new CropViewItem();

            var farm = new Farm()
            {
                Province = Province.Alberta,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(198, currentYearViewItem.AboveGroundCarbonInput, 1);
        }

        /// <summary>
        /// Test below ground carbon input calculation when the user enters the 'main' crop (i.e. 'Winter Wheat') as a cover crop and also specifies 
        /// green manure harvest type.
        /// </summary>
        [TestMethod]
        public void CalculateBelowGroundCarbonInputFromCoverCropsWhenCoverCropIsUsedAsGreenManure()
        {
            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.WinterWheat, // Under 'Small Grain Cereals' in residue table
                Yield = 1000,
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 50,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                PerennialStandLength = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.GreenManure,
                BiomassCoefficientProduct = 0.451,
                BiomassCoefficientStraw = 0.340,
                BiomassCoefficientRoots = 0.126,
                BiomassCoefficientExtraroot = 0.082,
                PlantCarbonInAgriculturalProduct = 594
            };

            var farm = new Farm()
            {
                Province = Province.BritishColumbia,
                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            _sut.AssignInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm, animalResults: new List<AnimalComponentEmissionsResults>());


            Assert.AreEqual(104.131479140329, currentYearViewItem.BelowGroundCarbonInput, 3);
        }

        /// <summary>
        /// A grazed perennial whose only variable is the yield assignment method. Grazing moisture is zero so the two
        /// expected values differ only by the harvest-loss gross-up: custom gives 1000 * 0.45 = 450, while a modelled
        /// method gives 1000 / (1 - 0.35) * 0.45 = 692.31.
        /// </summary>
        /// <summary>
        /// A field that is both grazed and hayed in the same year, with round numbers chosen so the expected values can
        /// be checked by hand. The animals ate 1000 kg C at a utilization that implies 1667 kg C grew, and 500 kg C was
        /// baled off at a 35% harvest loss, implying a further 500 / 0.65 = 769.23 kg C grew.
        ///
        ///     plant carbon  = 1667 + 769.23                = 2436.23 kg C ha^-1
        ///     returned      = 2436.23 - 1000 (eaten) - 500 (baled) =  936.23 kg C ha^-1
        ///
        /// Before the hay term existed these were 1667 and 667: the hay was modelled as though it had never been cut,
        /// and the 269 kg C of residue the cut left behind went uncounted.
        /// </summary>
        private Farm CreateGrazedAndHayedFarm(out CropViewItem viewItem)
        {
            var farm = CreateGrazedFieldFarm(out viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;
            farm.Defaults.CarbonConcentration = 0.45;

            viewItem.Area = 1;
            viewItem.TotalCarbonLossesByGrazingAnimals = 1667;
            viewItem.TotalCarbonUptakeByAnimals = 1000;
            viewItem.TotalCarbonLossFromBaleExports = 500;

            viewItem.HarvestViewItems.Add(new HarvestViewItem()
            {
                Start = new DateTime(1985, 8, 1),
                ForageActivity = ForageActivities.Hayed,
                HarvestLossPercentage = 35,
                AboveGroundBiomassDryWeight = 1000,
            });

            return farm;
        }

        [TestMethod]
        public void CalculateCarbonInputFromProductSubtractsBaledHayUnderACustomYieldWithGrazing()
        {
            // Under Custom the entered yield is already the total aboveground biomass produced, so C_p needs no
            // gross-up - but the hay carted off still has to come off what the animals left. C_p 2000 kg C/ha at 60%
            // utilization leaves 800, and baling 500 kg C off one hectare leaves 300.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            viewItem.Area = 1;
            viewItem.PlantCarbonInAgriculturalProduct = 2000;
            viewItem.DoNotRecalculatePlantCarbonInAgriculturalProduct = true;
            viewItem.GrazingViewItems[0].Utilization = 60;
            viewItem.TotalCarbonLossFromBaleExports = 500;

            var result = _sut.CalculateCarbonInputFromProduct(null, viewItem, null, farm);

            Assert.AreEqual(300, result, 0.001);
        }

        [TestMethod]
        public void CalculateCarbonInputFromProductUnderACustomYieldWithGrazingIsUnchangedWithoutHay()
        {
            // Regression guard: with nothing baled off, the utilization-only result stands as before.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            viewItem.Area = 1;
            viewItem.PlantCarbonInAgriculturalProduct = 2000;
            viewItem.DoNotRecalculatePlantCarbonInAgriculturalProduct = true;
            viewItem.GrazingViewItems[0].Utilization = 60;
            viewItem.TotalCarbonLossFromBaleExports = 0;

            var result = _sut.CalculateCarbonInputFromProduct(null, viewItem, null, farm);

            Assert.AreEqual(800, result, 0.001);
        }

        [TestMethod]
        public void CalculateCarbonInputFromProductNeverReturnsNegativeCarbonWhenMoreWasBaledThanRemained()
        {
            // A user can enter a harvest larger than the yield implies. That is their data to fix, but the carbon input
            // must not go negative and pull soil carbon down with it.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            viewItem.Area = 1;
            viewItem.PlantCarbonInAgriculturalProduct = 2000;
            viewItem.DoNotRecalculatePlantCarbonInAgriculturalProduct = true;
            viewItem.GrazingViewItems[0].Utilization = 60;
            viewItem.TotalCarbonLossFromBaleExports = 5000;

            var result = _sut.CalculateCarbonInputFromProduct(null, viewItem, null, farm);

            Assert.AreEqual(0, result, 0.001);
        }

        [TestMethod]
        public void CalculateCarbonInputFromProductIgnoresGrazingFromAnotherYearUnderACustomYield()
        {
            // The grazing branch is scoped to the item's own year. A field grazed in 1984 must not have its 1985 return
            // cut by the utilization rate: with no animals on it that year, the ordinary percentage returned applies.
            // C_p 2000 at the 35% default gives 700, not the 800 that a utilization of 60% would have produced.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            viewItem.Area = 1;
            viewItem.PlantCarbonInAgriculturalProduct = 2000;
            viewItem.DoNotRecalculatePlantCarbonInAgriculturalProduct = true;
            viewItem.PercentageOfProductYieldReturnedToSoil = 35;
            viewItem.GrazingViewItems[0].Start = new DateTime(1984, 6, 1);
            viewItem.GrazingViewItems[0].Utilization = 60;

            var result = _sut.CalculateCarbonInputFromProduct(null, viewItem, null, farm);

            Assert.AreEqual(700, result, 0.001);
        }

        [TestMethod]
        public void AssignInputsAddsTheBaledHayToPlantCarbonOnAGrazedField()
        {
            // Equation 11.3.2-5 / -7: what grew is the grazed portion recovered from utilization PLUS the baled portion
            // recovered from its harvest loss. Only the grazing half was counted before.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(2436.2308, viewItem.PlantCarbonInAgriculturalProduct, 0.001);
        }

        [TestMethod]
        public void AssignInputsSubtractsBothRemovalsFromWhatIsReturnedToSoil()
        {
            // Equation 11.3.2-8, with the sign corrected: subtract what the animals ate AND what was baled off. The
            // published equation brackets these as (uptake - export), which would give 2436.23 - (1000 - 500) = 1936.23,
            // leaving more carbon on the field than was removed from it.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(936.2308, viewItem.CarbonInputFromProduct, 0.001);
        }

        [TestMethod]
        public void AssignInputsLeavesAGrazedFieldWithoutHayUnchanged()
        {
            // Regression guard: with nothing baled off, the hay term is zero and the grazing-only result stands.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);
            viewItem.TotalCarbonLossFromBaleExports = 0;
            viewItem.HarvestViewItems.Clear();

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(1667, viewItem.PlantCarbonInAgriculturalProduct, 0.001);
            Assert.AreEqual(667, viewItem.CarbonInputFromProduct, 0.001);
        }

        [TestMethod]
        public void AssignInputsReportsThePercentageThatActuallyStayedOnAGrazedAndHayedField()
        {
            // 936.23 of the 2436.23 kg C that grew stayed on the field, so 38.4295% was returned - not the 40% that
            // 100 - utilization would suggest, which counts only what the animals left and ignores the hay cut.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);
            viewItem.PercentageOfProductYieldReturnedToSoil = 40;

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(38.4295, viewItem.PercentageOfProductYieldReturnedToSoil, 0.001);
        }

        [TestMethod]
        public void AssignInputsDoesNotReportOverAManuallyOverriddenPercentageReturned()
        {
            // A value the user pinned through advanced input editing is theirs to keep.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);
            viewItem.PercentageOfProductYieldReturnedToSoil = 40;
            viewItem.DoNotRecalculatePercentageReturnedToSoil = true;

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(40, viewItem.PercentageOfProductYieldReturnedToSoil, 0.001);
        }

        [TestMethod]
        public void AssignInputsIncludesTheBaledHayInTheBackCalculatedYield()
        {
            // Equation 11.3.2-9 works from the same plant carbon, so the hay reaches the reported yield too:
            // 2436.2308 / 0.45 = 5413.85 kg ha^-1 at zero moisture.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(5413.8462, viewItem.Yield, 0.001);
        }

        [TestMethod]
        public void AssignInputsReportsThePercentageFromTheBalanceUnderACustomYieldToo()
        {
            // M4. The Custom path used to leave this at 100 - utilization, which describes only what the animals left
            // and ignores the hay taken from the same standing crop - so the figure on screen disagreed with the carbon
            // credited whenever a cut was entered. Both methods now report from the balance.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            viewItem.PercentageOfProductYieldReturnedToSoil = 40;

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            var expected = 100.0 * viewItem.CarbonInputFromProduct / viewItem.PlantCarbonInAgriculturalProduct;
            Assert.AreEqual(expected, viewItem.PercentageOfProductYieldReturnedToSoil, 0.001,
                "the reported percentage must be what actually stayed on the field");
            Assert.AreNotEqual(40, viewItem.PercentageOfProductYieldReturnedToSoil, 0.001);
        }

        [TestMethod]
        public void AssignInputsKeepsAPinnedPercentageUnderACustomYieldWithGrazing()
        {
            var farm = CreateGrazedAndHayedFarm(out var viewItem);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            viewItem.PercentageOfProductYieldReturnedToSoil = 40;
            viewItem.DoNotRecalculatePercentageReturnedToSoil = true;

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(40, viewItem.PercentageOfProductYieldReturnedToSoil, 0.001);
        }

        [TestMethod]
        public void AssignInputsRecordsWhenTheCutRemovesMoreThanTheYieldAccountsFor()
        {
            // M5. Only a yield typed independently of the removals can contradict them, so this is the one path where
            // it can happen. The floor at zero stays - nothing more than grew can be returned - but the year is marked
            // so the interface can say so instead of reporting a plausible-looking zero.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            viewItem.TotalCarbonLossFromBaleExports = 500000;

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.AreEqual(0, viewItem.CarbonInputFromProduct, 0.001, "floored, not negative");
            Assert.IsTrue(viewItem.HayCutExceedsWhatTheYieldAccountsFor);
        }

        [TestMethod]
        public void AssignInputsDoesNotRecordAContradictionWhenTheYieldCoversTheCut()
        {
            // The counterpart. The shared fixture's yield of 1000 is deliberately small next to its bale exports, so it
            // trips the contradiction under a custom yield - which is the case above. Give the field a yield that can
            // account for the cut and the flag must stay clear.
            var farm = CreateGrazedAndHayedFarm(out var viewItem);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            viewItem.Yield = 100000;

            _sut.AssignInputs(null, viewItem, null, farm, new List<AnimalComponentEmissionsResults>());

            Assert.IsTrue(viewItem.CarbonInputFromProduct > 0, "test setup: the yield must cover the cut");
            Assert.IsFalse(viewItem.HayCutExceedsWhatTheYieldAccountsFor);
        }

        private static Farm CreateGrazedFieldFarm(out CropViewItem viewItem, out FieldSystemComponent field)
        {
            var farm = new Farm();
            field = new FieldSystemComponent();

            viewItem = new CropViewItem()
            {
                CropType = CropType.TameGrass,
                Year = 1985,
                FieldSystemComponentGuid = field.Guid,
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 35,
                CarbonConcentration = 0.45,
                MoistureContentOfCrop = 0,
            };
            viewItem.GrazingViewItems.Add(new GrazingViewItem()
                {Start = new DateTime(1985, 6, 1), MoistureContentAsPercentage = 0});

            field.CropViewItems.Add(viewItem);
            farm.Components.Add(field);

            return farm;
        }

        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductUsesFieldLevelCustomMethodWhenEnabled()
        {
            // Field-level assignment on and the field is Custom: the entered yield is already the total aboveground
            // biomass, so the harvest-loss gross-up must be suppressed even though the farm is set to a modelled method.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;
            farm.UseFieldLevelYieldAssignement = true;
            field.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, viewItem, farm);

            Assert.AreEqual(450, result, 0.0001);
        }

        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductUsesFieldLevelModelledMethodWhenEnabled()
        {
            // The mirror case: the farm is Custom but this field is modelled, so the gross-up applies.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            farm.UseFieldLevelYieldAssignement = true;
            field.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, viewItem, farm);

            Assert.AreEqual(692.3077, result, 0.0001);
        }

        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductFallsBackToFarmMethodWhenFieldLevelDisabled()
        {
            // Regression guard: with field-level assignment off the farm's method decides, so a field marked modelled
            // is ignored and the custom (no gross-up) result is returned.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            farm.UseFieldLevelYieldAssignement = false;
            field.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, viewItem, farm);

            Assert.AreEqual(450, result, 0.0001);
        }

        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductUsesFarmMethodWhenViewItemHasNoField()
        {
            // An orphan view item (no matching field component) must still resolve to the farm-level method rather
            // than throwing or silently changing branch - this is the shape most unit tests construct.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            farm.UseFieldLevelYieldAssignement = true;
            viewItem.FieldSystemComponentGuid = Guid.NewGuid();

            var result = _sut.CalculatePlantCarbonInAgriculturalProduct(null, viewItem, farm);

            Assert.AreEqual(450, result, 0.0001);
        }



        #endregion


    }
}
