#region Imports

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Animals;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace H.Core.Test.Calculators.Carbon
{
    [TestClass]
    public class ICBMSoilCarbonCalculatorTest : UnitTestBase
    {
        #region Fields

        private ICBMSoilCarbonCalculator _sut;
        private IICBMCarbonInputCalculator _soilCarbonInputCalculator;

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
            var iCBMSoilCarbonCalculator = new ICBMSoilCarbonCalculator(base._climateProvider, base._n2OEmissionFactorCalculator);
            
            

            _sut = iCBMSoilCarbonCalculator;
            _soilCarbonInputCalculator = new ICBMCarbonInputCalculator();
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

            var result = _sut.CalculateManureCarbonInputPerHectare(cropViewItem);

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

            var result = _sut.CalculateManureCarbonInputPerHectare(cropViewItem);

            Assert.AreEqual(25, result);
        }

        #endregion

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

            farm.Components.Add(fieldSystemComponent);

            farm.StageStates.Add(new FieldSystemDetailsStageState() { DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>() { cropViewItem } });

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
            cropViewItem.FieldSystemComponentGuid = fieldSystemComponent.Guid;

            _soilCarbonInputCalculator.SetCarbonInputs(null, cropViewItem, null, farm);

            // ICBM uses the previous year's inputs for residue calculations so we use the view item for both the current year and the previous year
            _sut.CalculateNitrogenAtInterval(cropViewItem, cropViewItem, null, farm, 0);

            var directN2OAsCO2 = cropViewItem.TotalDirectNitrousOxidePerHectare * cropViewItem.Area * CoreConstants.N2OToCO2eConversionFactor;
            var indirectN2OAsCO2 = cropViewItem.TotalIndirectNitrousOxidePerHectare * cropViewItem.Area * CoreConstants.N2OToCO2eConversionFactor;

            Assert.AreEqual(258.1501, directN2OAsCO2, 4);
            Assert.AreEqual(152.337506663593, indirectN2OAsCO2, 4);
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

        #region CalculateInputsFromSupplementalHayFedToGrazingAnimals



        #endregion



        [TestMethod]
        public void CalculateEquilibriumYear()
        {
            var detailViewItems = new List<CropViewItem>()
            {
                new CropViewItem()
                {
                    CropType = CropType.Barley, SizeOfFirstRotationForField = 2, Year = 2000,
                    AboveGroundCarbonInput = 10, BelowGroundCarbonInput = 20, ClimateParameter = 1,
                    ManagementFactor = 0.9
                },
                new CropViewItem()
                {
                    CropType = CropType.Oats, SizeOfFirstRotationForField = 2, Year = 2001, AboveGroundCarbonInput = 20,
                    BelowGroundCarbonInput = 40, ClimateParameter = 1, ManagementFactor = 0.9
                },
                new CropViewItem()
                {
                    CropType = CropType.Barley, SizeOfFirstRotationForField = 2, Year = 2002,
                    AboveGroundCarbonInput = 30, BelowGroundCarbonInput = 80, ClimateParameter = 1,
                    ManagementFactor = 0.9
                },
                new CropViewItem()
                {
                    CropType = CropType.Oats, SizeOfFirstRotationForField = 2, Year = 2003, AboveGroundCarbonInput = 40,
                    BelowGroundCarbonInput = 100, ClimateParameter = 1, ManagementFactor = 0.9
                },
            };


            var farm = new Farm()
            {
                UseCustomStartingSoilOrganicCarbon = false,
                Defaults =
                {
                    EquilibriumCalculationStrategy = EquilibriumCalculationStrategies.Default,
                }
            };
            var fieldGuid = Guid.NewGuid();

            var fieldComponent = new FieldSystemComponent()
            {
                Guid = fieldGuid,
            };

            fieldComponent.CropViewItems.AddRange(detailViewItems);

            farm.Components.Add(fieldComponent);

            var result = _sut.CalculateEquilibriumYear(
                detailViewItems: detailViewItems,
                farm: farm,
                componentId: fieldGuid);
        }


        #endregion
    }
}