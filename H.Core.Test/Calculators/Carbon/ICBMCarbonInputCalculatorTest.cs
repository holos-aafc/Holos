using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
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
                farm: farm);

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
                farm: farm);

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
                Yield = 1000,
                PercentageOfProductYieldReturnedToSoil = 10,
                PercentageOfStrawReturnedToSoil = 0, // This will be set to zero for the user when they have selected a perennial
                CarbonConcentration = 0.45,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                PlantCarbonInAgriculturalProduct = 435.6,
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
                farm: farm);

            // Plant C in agricultural product: Cp = 435.6

            // Cag = Cptosoil
            // = (Cp * %yieldReturned / 100) 
            // = (435.6 * 0.1) 
            // = 43.56 

            Assert.AreEqual(43.56, currentYearViewItem.AboveGroundCarbonInput, 2);
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
                farm: farm);

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
                farm: farm);

            Assert.AreEqual(10.293, currentYearViewItem.AboveGroundCarbonInput, 3);
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
                farm: farm);

            // Plant C in agricultural product: Cp = 396

            // Cbg = Cr + Ce
            // = [Cp * (Rr/Rp) * (Sr/100) * 1/standLength] + [Cp * (Re/Rp)]
            // = [396 * (0.4/0.5) * (100/100) * 1/5] + [396 * (0.3 / 0.5)]
            // = [396 * 0.8 * 1 * 0.2] + [396 * (0.6)]
            // = 63.36 + 237.6
            // = 300.96

            // * NOTE: the stand length was removed from this calculation (not considered anymore). New calculation should be:
            // Cbg = Cr + Ce
            // = [Cp * (Rr/Rp) * (Sr/100)] + [Cp * (Re/Rp)]
            // = [396 * (0.4/0.5) * (100/100)] + [396 * (0.3 / 0.5)]
            // = [396 * 0.8 * 1] + [396 * (0.6)]
            // = 316.8 + 237.6
            // = 554.4

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
                farm: farm);

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
                farm: farm);

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

        /// <summary>
        /// Equation 2.2.5-3
        /// </summary>
        [TestMethod]
        public void CalculatePlantCarbonInAgriculturalProductGrazedFieldAndCustomYieldAssignmentMethod()
        {
            var currentYearViewItem = new CropViewItem()
            {
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 10,
                CropType = CropType.Barley,
                MoistureContentOfCrop = 0.12,
            };

            currentYearViewItem.GrazingViewItems.Add(new GrazingViewItem());
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
            };

            var grazingViewItem = new GrazingViewItem();
            grazingViewItem.Utilization = 55;

            currentYearViewItem.GrazingViewItems.Add(grazingViewItem);

            var farm = new Farm();
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            var result = _sut.CalculateAboveGroundCarbonInputFromPerennials(null, currentYearViewItem, null, farm);

            Assert.AreEqual(9, result);
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
                farm: farm);

            // Plant C in agricultural product: Cp = 534.6

            // Cag = Cptosoil + Cs 
            // = [Cp * (Sp/100)] + 0 (since straw inputs are not calculated for silage crops)
            // = 534.6 * 0.35
            // = 187.11

            Assert.AreEqual(187.11, currentYearViewItem.AboveGroundCarbonInput, 2);
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
                farm: farm);

            // Plant C in agricultural product: Cp = 534.6

            // Cbg = Cr + Ce
            // = [Cp * (Rr/Rp) * (Sr/100)] + [Cp * (Re/Rp)]
            // = [534.6 * (0.4/0.2) * 100/100] + [534.6 * (0.3/0.2)]
            // = [534.6 * 2 * 1] + [534.6 * 1.5]
            // = 1069.2 + 801.9
            // = 1871.1

            Assert.AreEqual(1871, currentYearViewItem.BelowGroundCarbonInput, 1);
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
                farm: farm);

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
                farm: farm);

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
                farm: farm);

            // Plant C in agricultural product: Cp = 594

            // Cag = CptoSoil + Cs
            // = [Cp * (Sp / 100)] + [Cp * (Rs / Rp) * (Ss / 100)]
            // = [594 * (50 / 100)] + [594 * (0.340 / 0.451) * (10 / 100)]
            // = [594 * 0.5] + [594 * 0.753 * 0.1] 
            // = 297 + 44.7282
            // = 341.7282

            Assert.AreEqual(341.7282, currentYearViewItem.AboveGroundCarbonInput, 4);
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
                farm: farm);

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
                farm: farm);

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
                farm: farm);


            Assert.AreEqual(104.131479140329, currentYearViewItem.BelowGroundCarbonInput, 3);
        }



        #endregion


    }
}
