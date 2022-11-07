#region Imports

using System;
using System.Collections.ObjectModel;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

#endregion

namespace H.Core.Test.Calculators
{
    [TestClass]
    public class ICBMSoilCarbonCalculatorTest
    {
        #region Fields

        private ICBMSoilCarbonCalculator _sut;

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
            _sut = new ICBMSoilCarbonCalculator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        #region CalculateManureCarbonInput

        [TestMethod]
        public void CalculateManureCarbonInputReturnsZeroWhenThereAreNoManureApplicationViewItems()
        {
            var cropViewItem = new CropViewItem()
            {
                ManureApplicationViewItems = new ObservableCollection<ManureApplicationViewItem>()
                {

                }
            };

            var result = _sut.CalculateManureCarbonInputPerHectare(cropViewItem, new Farm());

            Assert.AreEqual(0, result);
        }


        [TestMethod]
        public void CalculateManureCarbonInputReturnsNonZeroWhenThereAreManureApplicationViewItems()
        {
            var manureComposition = new DefaultManureCompositionData()
            {
                ManureStateType = ManureStateType.Composted,
                CarbonFraction = 25,
                AnimalType = AnimalType.Beef
            };

            var cropViewItem = new CropViewItem()
            {
                Year = DateTime.Now.Year,
                ManureApplicationViewItems = new ObservableCollection<ManureApplicationViewItem>()
                {
                    new ManureApplicationViewItem()
                    {
                        ManureStateType = ManureStateType.Composted,
                        ManureAnimalSourceType = ManureAnimalSourceTypes.BeefManure,
                        ManureLocationSourceType = ManureLocationSourceType.Livestock,
                        AnimalType = AnimalType.Beef,
                        DateOfApplication = DateTime.Now,
                        DefaultManureCompositionData = manureComposition,
                        AmountOfManureAppliedPerHectare = 100,
                    }
                }
            };

            var farm = new Farm()
            {
                DefaultManureCompositionData = new ObservableCollection<DefaultManureCompositionData>()
                {
                   manureComposition,
                }
            };

            var result = _sut.CalculateManureCarbonInputPerHectare(cropViewItem, farm);

            Assert.AreEqual(25, result);
        }

        #endregion

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
        /// Equation 2.2.2-27
        /// </summary>
        [TestMethod]
        public void CalculateAmountOfNitrogenAppliedFromManure()
        {
            var result = _sut.CalculateAmountOfNitrogenAppliedFromManure(0.234, 0.121);
            Assert.AreEqual(0.028314, result, 0.000001);
        }

        /// <summary>
        /// Equation 2.2.2-28
        /// </summary>
        [TestMethod]
        public void CalculateAmountOfPhosphorusAppliedFromManure()
        {
            var result = _sut.CalculateAmountOfPhosphorusAppliedFromManure(1.234, 123.231);
            Assert.AreEqual(152.067054, result, 0.00001);
        }

        /// <summary>
        /// Equation 2.2.2-29
        /// </summary>
        [TestMethod]
        public void CalculateMoistureOfManure()
        {
            var result = _sut.CalculateMoistureOfManure(1.234, 123.231);
            Assert.AreEqual(0.0152067054, result);
        }

        /// <summary>
        /// Equation 2.2.3-1
        /// </summary>
        [TestMethod]
        public void CalculateAverageAboveGroundResidueCarbonInput()
        {
            var result = _sut.CalculateAverageAboveGroundResidueCarbonInput(0.12, 0.442);
            Assert.AreEqual(0.562, result);
        }

        /// <summary>
        /// Equation 2.2.3-2
        /// </summary>
        [TestMethod]
        public void CalculateAverageBelowGroundResidueCarbonInput()
        {
            var result = _sut.CalculateAverageBelowGroundResidueCarbonInput(0.234, 3.4234);
            Assert.AreEqual(3.6574, result);
        }

        /// <summary>
        /// Equation 2.2.3-3
        /// </summary>
        [TestMethod]
        public void CalculateAverageManureCarbonInput()
        {
            var result = _sut.CalculateAverageManureCarbonInput(1243.35434532);
            Assert.AreEqual(1243.35434532, result);
        }

        /// <summary>
        /// Equation 2.2.3-4
        /// </summary>
        [TestMethod]
        public void CalculateAboveGroundSteadyState()
        {
            var result = _sut.CalculateYoungPoolSteadyStateAboveGround(1, 2, 1);
            Assert.AreEqual(0.15651764274966565181808062346542, result, 0.00000001);
        }

        /// <summary>
        /// Equation 2.2.3-5
        /// </summary>
        [TestMethod]
        public void CalculateBelowGroundSteadyState()
        {
            var result = _sut.CalculateYoungPoolSteadyStateBelowGround(1, 3, 3);
            Assert.AreEqual(1.2342503594618505957689934913731e-4, result, 0.00000001);
        }

        /// <summary>
        /// Equation 2.2.3-6
        /// </summary>
        [TestMethod]
        public void CalculateYoungPoolSteadyStateManure()
        {
            var result = _sut.CalculateYoungPoolSteadyStateManure(1, 2, 2);
            Assert.AreEqual(0.01865736036377404793890488238391, result);
        }

        /// <summary>
        /// Equation 2.2.3-7
        /// </summary>
        [TestMethod]
        public void CalculateOldPoolSteadyState()
        {
            var youngPoolDecompostionRate = 1.0;
            var oldPoolDecompositionRate = 2.0;
            var climateParamter = 3.0;
            var aboveGroundHumification = 1.0;
            var belowGroundHumification = 2.0;
            var aboveGroundCarbonInput = 1.0;
            var belowGroundCarbonInput = 2.0;
            var manureSteadyState = 1.0;
            var manureCarbonInput = 2.0;
            var manureHumification = 3.0;

            var youngPoolSteadyStateAboveGround = 1.0; //_sut.CalculateYoungPoolSteadyStateAboveGround(aboveGroundCarbonInput, youngPoolDecompostionRate, climateParamter);
            var youngPoolSteadyStateBelowGround = 2.0;//_sut.CalculateYoungPoolSteadyStateBelowGround(belowGroundCarbonInput, youngPoolDecompostionRate, climateParamter);

            var result = _sut.CalculateOldPoolSteadyState(youngPoolDecompostionRate,
                                                          oldPoolDecompositionRate,
                                                          climateParamter,
                                                          aboveGroundHumification,
                                                          belowGroundHumification,
                                                          aboveGroundCarbonInput,
                                                          belowGroundCarbonInput,
                                                          youngPoolSteadyStateAboveGround,
                                                          youngPoolSteadyStateBelowGround,
                                                          manureSteadyState,
                                                          manureCarbonInput,
                                                          manureHumification);
            Assert.AreEqual(0.90109159037376883669811488366329, result, 0.00000000001);
        }

        /// <summary>
        /// Equation 2.2.4-1
        /// </summary>
        [TestMethod]
        public void CalculateYoungPoolAboveGroundCarbonAtInterval()
        {
            var result = _sut.CalculateYoungPoolAboveGroundCarbonAtInterval(0.232, 3.432, 0.532, 5.767);
            Assert.AreEqual(0.17042012737096903330645093846963, result, 0.0000001);
        }

        /// <summary>
        /// Equation 2.2.4-2
        /// </summary>
        [TestMethod]
        public void CalculateYoungPoolBelowGroundCarbonAtInterval()
        {
            var result = _sut.CalculateYoungPoolBelowGroundCarbonAtInterval(2.546, 2.7543, 0.435, 32.3);
            Assert.AreEqual(4.1903069151643476599675250028938e-6, result, 0.0000001);
        }

        /// <summary>
        /// Equation 2.2.4-3
        /// </summary>
        [TestMethod]
        public void CalculateYoungPoolManureCarbonAtInterval()
        {
            var result = _sut.CalculateYoungPoolManureCarbonAtInterval(0.32, 0.2342, 45.53, 0.65);
            Assert.AreEqual(7.7792634024786830648485547714321e-14, result, 0.0000001);
        }
        /// <summary>
        /// Equation 2.2.4-4
        /// </summary>
        [TestMethod]
        public void CalculateOldPoolSoilCarbonAtInterval()
        {
            var result = _sut.CalculateOldPoolSoilCarbonAtInterval(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            Assert.AreEqual(6.1853337707258386287097434158479e-15, result, 0.0000000001);
        }
        /// <summary>
        /// Equation 2.2.4-5
        /// </summary>
        [TestMethod]
        public void CalculateSoilCarbonAtInterval()
        {
            var result = _sut.CalculateSoilCarbonAtInterval(0.32, 21.03, 42.34, 0.321);
            Assert.AreEqual(64.011, result, 0.001);
        }

        /// <summary>
        /// Equation 2.2.4-6
        /// </summary>
        [TestMethod]
        public void CalculateChangeInSoilCarbonAtInterval()
        {
            var result = _sut.CalculateChangeInSoilCarbonAtInterval(0.446, 0.853);
            Assert.AreEqual(-0.407, result);
        }

        /// <summary>
        /// Equation 2.2.4-7
        /// </summary>
        [TestMethod]
        public void CalculateChangeInSoilOrganicCarbonForFieldAtInterval()
        {
            var result = _sut.CalculateChangeInSoilOrganicCarbonForFieldAtInterval(123.34, 0.5679);
            Assert.AreEqual(70.044786, result);
        }

        /// <summary>
        /// Equation 2.2.4-8
        /// </summary>
        [TestMethod]
        public void CalculateChangeInSoilOrganicCarbonForFarmAtInterval()
        {
            var result = _sut.CalculateChangeInSoilOrganicCarbonForFarmAtInterval(
                new System.Collections.Generic.List<double>
                {
                    0.24,
                    12.42,
                    0.56
                });
            Assert.AreEqual(13.22, result);
        }

        /// <summary>
        /// Equation 2.2.5-1
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideEquivalentsForSoil()
        {
            var result = _sut.CalculateCarbonDioxideEquivalentsForSoil(47.654564);
            Assert.AreEqual(174.73340133333333333333333333333, result, 0.00000001);
        }

        /// <summary>
        /// Equation 2.2.5-2
        /// </summary>
        [TestMethod]
        public void CalculateChangeInCarbonDioxideEquivalentsForSoil()
        {
            var result = _sut.CalculateChangeInCarbonDioxideEquivalentsForSoil(0.332542);
            Assert.AreEqual(1.2193206666666666666666666666667, result);
        }

        /// <summary>
        /// Equation 2.2.5-3
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideChangeForSoilsByMonth()
        {
            var result = _sut.CalculateCarbonDioxideChangeForSoilsByMonth(0.0032);
            Assert.AreEqual(2.6666666666666666666666666666667e-4, result);
        }

        #region Carbon Input From Product (C_p)

        /// <summary>
        /// Equation 2.2.2-6
        /// Equation 2.2.2-10
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

        #endregion

        #region CalculateInputsFromSupplementalHayFedToGrazingAnimals

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

            // = [(10 * 500) * (1 - 12/100) * (1 - 20/100)] * 0.45
            // = (5000 * 0.88 * 0.8) * 0.45
            // = 1584

            Assert.AreEqual(1584, result);
        }

        #endregion

        #region Plant Carbon In Agricultural Product

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

        #endregion

        #region Green Manure Harvest Method

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

            _sut.SetCarbonInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm);

            // Plant C in agricultural product: Cp = 403.92

            // Cag = CptoSoil
            // = Cp * (Sp / 100)
            // = 403.92 * (2 / 100)
            // = 8.0784

            Assert.AreEqual(8.0784, currentYearViewItem.AboveGroundCarbonInput);
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

            _sut.SetCarbonInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm);

            // In the case when an annual crop is used and green manure is specified as harvest method Rp is the sum of the Rp and Rs values.

            // Plant C in agricultural product: Cp = 403.92

            // Cbg = Cr + Ce
            // = [Cp * (Rr / Rp) * (Sr / 100)] + [Cp * (Re / Rp)] *** Rp = 0.5 + 0.2 = 0.7 ***
            // = [403.92 * (0.4 / 0.7) * (100 / 100)] + [403.92 * (0.3 / 0.7)]
            // = [403.92 * 0.571 * 1] + [403.92 * 0.42857]
            // = 230.638 + 172.107
            // = 402.754

            Assert.AreEqual(402.754, currentYearViewItem.BelowGroundCarbonInput, 3);
        } 

        #endregion

        #region Annuals

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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            Assert.AreEqual(554.4, currentYearViewItem.BelowGroundCarbonInput, 1);
        }

        /// <summary>
        /// The current year's below ground inputs can't be greater than the previous year's inputs for perennials
        /// </summary>
        [TestMethod]
        public void CalculateBelowGroundCarbonInputForPerennialsWhenBelowGroundCarbonInputIsLessThanPreviousYear()
        {
            var perennialGroupId = Guid.NewGuid();

            var previousYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameMixed,
                PerennialStandGroupId = perennialGroupId,
                Yield = 1000,
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 2,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.5,
                BiomassCoefficientRoots = 0.4,
                BiomassCoefficientExtraroot = 0.3,
                CarbonInputFromRoots = 1800,        // The current year's below ground inputs should be the sum of the C_r + C_e in the previous year if the current year's inputs are greater than the previous year
                CarbonInputFromExtraroots = 1400,    // The current year's below ground inputs should be the sum of the C_r + C_e in the previous year if the current year's inputs are greater than the previous year
            };

            var currentYearViewItem = new CropViewItem()
            {
                CropType = CropType.TameMixed,
                PerennialStandGroupId = perennialGroupId,
                Yield = 0,                          // Set yield to 0 so we can simulate the situation where the current year's below ground inputs are less than the previous year's inputs
                PercentageOfRootsReturnedToSoil = 100,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 2,
                IrrigationType = IrrigationType.RainFed,
                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,
                HarvestMethod = HarvestMethods.CashCrop,
                BiomassCoefficientProduct = 0.5,
                BiomassCoefficientRoots = 0.4,
                BiomassCoefficientExtraroot = 0.3,
            };

            var farm = new Farm()
            {
                Province = Province.Manitoba,
                ClimateData =
                {
                    PrecipitationData =
                    {
                        May = 10,
                        June = 20,
                    },

                    EvapotranspirationData =
                    {
                        May = 15,
                        June = 21,
                    }
                },

                DefaultSoilData =
                {
                    SoilFunctionalCategory = SoilFunctionalCategory.Black,
                }
            };

            var expected = previousYearViewItem.CarbonInputFromRoots + previousYearViewItem.CarbonInputFromExtraroots;

            _sut.SetCarbonInputs(
                previousYearViewItem: previousYearViewItem,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm);

            Assert.AreEqual(expected, currentYearViewItem.BelowGroundCarbonInput, 1);
        }

        [TestMethod]
        public void CalculateCarbonInputFromRootsForPerennialsWhenCurrentYearInputsIsGreaterThanPreviousYearInputs()
        {
            var previousYearViewItem = new CropViewItem()
            {
                CarbonInputFromRoots = 10,
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

            Assert.AreEqual(26.7, result, 1);
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
            Assert.AreEqual(50, result);
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
            Assert.AreEqual(50, result);
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
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

            _sut.SetCarbonInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: nextYearViewItem,
                farm: farm);

            // Plant C in agricultural product: Cp = 594

            // Cag = CptoSoil           *** Cs is omitted in this situation ***
            // = Cp * (Sp / 100)
            // = 594 * (50 / 100)
            // = 297

            Assert.AreEqual(297, currentYearViewItem.AboveGroundCarbonInput, 1);
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

            _sut.SetCarbonInputs(
                previousYearViewItem: null,
                currentYearViewItem: currentYearViewItem,
                nextYearViewItem: null,
                farm: farm);

            // Plant C in agricultural product: Cp = 594

            // Cbg = Cr + Ce                *** Rp is the combined sum of Rp and Rs in this situation. So, Rp = 0.451 + 0.34 = 0.791 ***
            // = [Cp * (Rr / Rp) * (Sr / 100)] + [Cp * (Re / Rp)]
            // = [594 * (0.126 / 0.791) * 1] + [594 * (0.082 / 0.791)]
            // = 94.619 + 61.577
            // = 156.196

            Assert.AreEqual(156.196, currentYearViewItem.BelowGroundCarbonInput, 3);
        }

        /// <summary>
        /// The results from this test match closely with a farm set up in v3 that uses the same growing season precipitation, yield, and soil data
        ///
        /// See excel sheet to verify
        /// </summary>
        [TestMethod]
        public void CalculateCropN2OEmissions()
        {
            var fieldSystemComponent = new FieldSystemComponent();
            var cropViewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                TillageType = TillageType.Intensive,
                Area = 1,
                NitrogenFertilizerRate = 100, // There has to be an amount of fertilizer applied for the direct N emission to be larger than the indirect N emissions. If this is 0 then the direct and indirect will be almost equal
                Yield = 1000,
                CarbonConcentration = 0.45,

                // From residue table
                BiomassCoefficientProduct = 0.451,
                BiomassCoefficientStraw = 0.4,
                BiomassCoefficientRoots = 0.09,
                BiomassCoefficientExtraroot = 0.059,

                PercentageOfProductYieldReturnedToSoil = 2,
                PercentageOfStrawReturnedToSoil = 0,
                PercentageOfRootsReturnedToSoil = 100,

                AmountOfIrrigation = 0,
                MoistureContentOfCrop = 0.12,

                NitrogenContentInProduct = 0.019,
                NitrogenContentInStraw = 0.007,
                NitrogenContentInRoots = 0.01,
                NitrogenContentInExtraroot = 0.01,

                IrrigationType = IrrigationType.RainFed,
                HarvestMethod = HarvestMethods.CashCrop,
            };

            fieldSystemComponent.CropViewItems.Add(cropViewItem);

            var farm = new Farm()
            {
                Province = Province.Alberta,
            };

            var soilData = new SoilData()
            {
                EcodistrictId = 812,
                SoilTexture = SoilTexture.Fine,
                SoilFunctionalCategory = SoilFunctionalCategory.Black,
            };

            var geographicData = new GeographicData()
            {
                DefaultSoilData = soilData,
            };

            var climateData = new ClimateData();
            climateData.PrecipitationData.GrowingSeasonPrecipitation = 332.87;
            climateData.EvapotranspirationData.GrowingSeasonEvapotranspiration = 499.65;

            farm.GeographicData = geographicData;
            farm.ClimateData = climateData;

            cropViewItem.ClimateParameter = 1;
            cropViewItem.ManagementFactor = 1;

            _sut.SetCarbonInputs(null, cropViewItem, null, farm);

            // ICBM uses the previous year's inputs for residue calculations so we use the view item for both the current year and the previous year
            _sut.CalculateNitrogenAtInterval(cropViewItem, cropViewItem, null, farm, 0);

            var directN2OAsCO2 = cropViewItem.TotalDirectNitrousOxidePerHectare * cropViewItem.Area * CoreConstants.N2OToCO2eConversionFactor;
            var indirectN2OAsCO2 = cropViewItem.TotalIndirectNitrousOxidePerHectare * cropViewItem.Area * CoreConstants.N2OToCO2eConversionFactor;

            Assert.AreEqual(258.1501, directN2OAsCO2, 4);
            Assert.AreEqual(126.0155, indirectN2OAsCO2, 4);
        }

        #endregion

        #endregion
    }
}