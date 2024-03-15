#region Imports

using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Providers.Evapotranspiration;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Precipitation;
using H.Core.Providers.Soil;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GroupEmissionsByDay = H.Core.Emissions.Results.GroupEmissionsByDay;

#endregion

namespace H.Core.Test.Services
{
    [TestClass]
    public partial class FieldResultsServiceTest : UnitTestBase
    {
        #region Fields

        private FieldResultsService _resultsService;
        private Mock<IAnimalService> _mockAnimalResultsService;
        private N2OEmissionFactorCalculator _n2OEmissionFactorCalculator;
        private ICBMSoilCarbonCalculator _iCbmSoilCarbonCalculator;
        private IPCCTier2SoilCarbonCalculator _ipccSoilCarbonCalculator;

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
            _mockAnimalResultsService = new Mock<IAnimalService>();

            _n2OEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            _iCbmSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            _ipccSoilCarbonCalculator = new IPCCTier2SoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);

            _resultsService = new FieldResultsService(_iCbmSoilCarbonCalculator, _ipccSoilCarbonCalculator, _n2OEmissionFactorCalculator);
            _resultsService.AnimalResultsService = _mockAnimalResultsService.Object;
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        #region CalculateEquilibriumYear Tests

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

            var result = _resultsService.CalculateEquilibriumYear(
                detailViewItems: detailViewItems,
                farm: farm,
                componentId: fieldGuid);
        }

        #endregion

        #region UpdatePercentageReturnsForPerennials Tests

        [TestMethod]
        public void UpdatePercentageReturnsForPerennialsSetReturnTo100PercentWhenYieldIs0()
        {
            var crops = new List<CropViewItem>()
            {
                new CropViewItem()
                    {CropType = CropType.TameGrass, PercentageOfProductYieldReturnedToSoil = 35, Yield = 0},
                new CropViewItem()
                    {CropType = CropType.TameLegume, PercentageOfProductYieldReturnedToSoil = 35, Yield = 1000},
                new CropViewItem() {CropType = CropType.Barley, PercentageOfProductYieldReturnedToSoil = 2},
            };

            _resultsService.UpdatePercentageReturnsForPerennials(
                viewItems: crops);

            Assert.AreEqual(100,
                crops.Single(x => x.CropType == CropType.TameGrass)
                    .PercentageOfProductYieldReturnedToSoil); // This should have been set to 100 since there is no yield
            Assert.AreEqual(2,
                crops.Single(x => x.CropType == CropType.Barley)
                    .PercentageOfProductYieldReturnedToSoil); // This should remain the same since its not a perennial
            Assert.AreEqual(35,
                crops.Single(x => x.CropType == CropType.TameLegume)
                    .PercentageOfProductYieldReturnedToSoil); // This should remain the same since it has a non-zero yield
        }

        #endregion

        #region AssignPerennialStandIds Tests

        [TestMethod]
        public void AssignPerennialStandIds()
        {
            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2000},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2000, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2001},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2001, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.ForageForSeed, Year = 2002},
                new CropViewItem() {CropType = CropType.ForageForSeed, Year = 2002, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.ForageForSeed, Year = 2003},
                new CropViewItem() {CropType = CropType.ForageForSeed, Year = 2003, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.Barley, Year = 2004},
                new CropViewItem() {CropType = CropType.None, Year = 2004, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameMixed, Year = 2005},
                new CropViewItem() {CropType = CropType.TameMixed, Year = 2005, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameMixed, Year = 2006},
                new CropViewItem() {CropType = CropType.TameMixed, Year = 2006, IsSecondaryCrop = true},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            var result = _resultsService.AssignPerennialStandIds(crops, component);

            Assert.AreEqual(3, result.Count());

            // Ensure all view items in same stand have the same stand GUID
            foreach (var group in result)
            {
                var key = group.Key;

                Assert.IsTrue(group.All(x => x.PerennialStandGroupId == key));
            }
        }

        [TestMethod]
        public void AssignPerennialStandIdsAppliesCorrectIdsWhenUndersownCropsUsed()
        {
            // User has entered lentil, (haygrass (undersown)), (haygrass) as the sequence. Need to consider different stand ids when undersown crops
            // are used since we want different stand ids not the same stand id for all items in the sequence. The second HayGrass stand should have a different
            // id than the first stand.

            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2000},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2000, UnderSownCropsUsed = true, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2001},
                new CropViewItem() {CropType = CropType.None, Year = 2001, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.Lentils, Year = 2002},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2002, UnderSownCropsUsed = true, IsSecondaryCrop = true},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2003},
                new CropViewItem() {CropType = CropType.None, Year = 2003, IsSecondaryCrop = true},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            var result = _resultsService.AssignPerennialStandIds(crops, component);

            Assert.AreEqual(2, result.Count());

            // Ensure all view items in same stand have the same stand GUID
            foreach (var group in result)
            {
                var key = group.Key;

                Assert.IsTrue(group.All(x => x.PerennialStandGroupId == key));
            }
        }

        [TestMethod]
        public void AssignPerennialStandIdsConsidersCoverCrop()
        {
            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2019}, // Main crop grown
                new CropViewItem() {CropType = CropType.AlfalfaMedicagoSativaL, Year = 2019, IsSecondaryCrop = true}, // Cover crop used in same year
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2020},
                new CropViewItem() {CropType = CropType.None, Year = 2020, IsSecondaryCrop = true},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            var result = _resultsService.AssignPerennialStandIds(
                viewItems: crops, fieldSystemComponent: component);

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void AssignPerennialStandIdsConsidersOnlyPerennials()
        {
            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2019},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            var result = _resultsService.AssignPerennialStandIds(
                viewItems: crops, fieldSystemComponent: component);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void AssignPerennialStandPositionalYears()
        {
            var guid = Guid.NewGuid();

            var crops = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Wheat, Year = 2019, PerennialStandGroupId = guid},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2019, IsSecondaryCrop = true, PerennialStandGroupId = guid},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2020, PerennialStandGroupId = guid},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2020, IsSecondaryCrop = true, PerennialStandGroupId = guid},
                new CropViewItem() {CropType = CropType.TameLegume, Year = 2021, PerennialStandGroupId = guid},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(crops) };

            _resultsService.AssignPerennialStandPositionalYears(
                viewItems: crops, fieldSystemComponent: component);


        }

        #endregion

        #region CreateDetailViewItems Tests

        [TestMethod]
        public void CreateDetailViewItemsWhenUserSelectsToOrderFromStartYear()
        {
            var componentSelectionScreenViewItems = new List<CropViewItem>();

            componentSelectionScreenViewItems.Add(new CropViewItem()
            {
                CropType = CropType.Wheat,
                Year = 2016,
            });

            componentSelectionScreenViewItems.Add(new CropViewItem()
            {
                CropType = CropType.Oats,
                Year = 2017,
            });

            var fieldSystemComponent = new FieldSystemComponent();
            fieldSystemComponent.CropViewItems.AddRange(componentSelectionScreenViewItems);
            fieldSystemComponent.StartYear = 2014;
            fieldSystemComponent.BeginOrderingAtStartYearOfRotation = true;

            var farm = new Farm()
            {
                Defaults = new Defaults()
                {
                    CarbonModellingStrategy = CarbonModellingStrategies.ICBM,
                },

                GeographicData = new GeographicData()
                {
                    DefaultSoilData = new SoilData(),
                }
            };
            farm.Components.Add(fieldSystemComponent);

            _resultsService.CreateDetailViewItems(farm);

            var stageState = _resultsService.GetStageState(farm);

            var result = stageState.DetailsScreenViewCropViewItems;

            var itemAt2014 = result.Single(x => x.Year == 2014);
            var itemAt2015 = result.Single(x => x.Year == 2015);
            var itemAt2016 = result.Single(x => x.Year == 2016);
            var itemAt2017 = result.Single(x => x.Year == 2017);

            Assert.AreEqual(CropType.Wheat, itemAt2014.CropType);
            Assert.AreEqual(CropType.Oats, itemAt2015.CropType);
            Assert.AreEqual(CropType.Wheat, itemAt2016.CropType);
            Assert.AreEqual(CropType.Oats, itemAt2017.CropType);
        }

        [TestMethod]
        public void CreateDetailViewItemsWhenUserSelectsToOrderFromEndYearYear()
        {
            var componentSelectionScreenViewItems = new List<CropViewItem>();

            componentSelectionScreenViewItems.Add(new CropViewItem()
            {
                CropType = CropType.Wheat,
                Year = 2021,
            });

            componentSelectionScreenViewItems.Add(new CropViewItem()
            {
                CropType = CropType.Oats,
                Year = 2020,
            });

            componentSelectionScreenViewItems.Add(new CropViewItem()
            {
                CropType = CropType.Barley,
                Year = 2019,
            });

            var fieldSystemComponent = new FieldSystemComponent();
            fieldSystemComponent.CropViewItems.AddRange(componentSelectionScreenViewItems);
            fieldSystemComponent.StartYear = 2000;
            fieldSystemComponent.EndYear = 2021;
            fieldSystemComponent.BeginOrderingAtStartYearOfRotation = false;

            var farm = new Farm()
            {
                GeographicData = new GeographicData()
                {
                    DefaultSoilData = new SoilData(),
                }
            };
            farm.Components.Add(fieldSystemComponent);

            _resultsService.CreateDetailViewItems(farm);

            var stageState = _resultsService.GetStageState(farm);

            var result = stageState.DetailsScreenViewCropViewItems;

            var itemAt2021 = result.Single(x => x.Year == 2021);
            var itemAt2020 = result.Single(x => x.Year == 2020);
            var itemAt2019 = result.Single(x => x.Year == 2019);

            Assert.AreEqual(CropType.Wheat, itemAt2021.CropType);
            Assert.AreEqual(CropType.Oats, itemAt2020.CropType);
            Assert.AreEqual(CropType.Barley, itemAt2019.CropType);
        }

        [TestMethod]
        public void CreateDetailsViewItemsReturnsNonEmptyList()
        {
            const int StartYear = 1985;

            var farm = new Farm()
            {
                Defaults = new Defaults()
                {
                    CarbonModellingStrategy = CarbonModellingStrategies.ICBM,
                },

                Components = new ObservableCollection<ComponentBase>()
                {
                    new FieldSystemComponent()
                    {
                        StartYear = 1985,
                        CropViewItems = new ObservableCollection<CropViewItem>()
                        {
                            new CropViewItem() {CropType = CropType.Barley, Year = 2021},
                            new CropViewItem() {CropType = CropType.Oats, Year = 2020},
                        }
                    },
                }
            };

            _resultsService.CreateDetailViewItems(farm);

            var stageState = _resultsService.GetStageState(farm);

            var expectedNumberOfItems = DateTime.Now.Year - StartYear + 1;
            Assert.AreEqual(expectedNumberOfItems, stageState.DetailsScreenViewCropViewItems.Count);
        }

        #endregion

        #region CalculateAmountOfProductRequired Tests

        [TestMethod]
        public void CalculateAmountOfProductRequired()
        {
            var farm = new Farm();
            var viewItem = new CropViewItem()
            {
                CropType = CropType.Barley,
                MoistureContentOfCrop = 0.12,
                Yield = 1000,
                CarbonConcentration = 0.45,
                PercentageOfProductYieldReturnedToSoil = 2,
                NitrogenContentInProduct = 18.9 / 1000,
                NitrogenDepositionAmount = 0,
            };

            var fertilizerApplicationViewItem = new FertilizerApplicationViewItem()
            {
                FertilizerEfficiencyPercentage = 50,
                FertilizerBlendData = new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Data()
                {
                    PercentageNitrogen = 25,
                }
            };

            // Required N of plant = 18.849600000000002
            // Required amount of product = (18.849600000000002 / 25) * 100; 

            var result = _resultsService.CalculateAmountOfProductRequired(farm, viewItem, fertilizerApplicationViewItem);

            Assert.AreEqual(135.71712, result);
        }

        #endregion

        #region AssignYieldToDetailViewItems Tests

        [TestMethod]
        public void AssignYieldToDetailViewItems()
        {
            var farm = new Farm()
            {
                YieldAssignmentMethod = YieldAssignmentMethod.Average,
            };

            var fieldSystemComponent = new FieldSystemComponent();
            fieldSystemComponent.CropViewItems = new ObservableCollection<CropViewItem>()
            {
                new CropViewItem()
                {
                    CropType = CropType.Barley,
                    Yield = 100,
                },
                new CropViewItem()
                {
                    CropType = CropType.Wheat,
                    Yield = 200,
                }
            };

            farm.Components.Add(fieldSystemComponent);

            var stageState = new FieldSystemDetailsStageState();
            stageState.DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>()
            {
                new CropViewItem()
                {
                    Year = 1985,
                    CropType = CropType.Barley,
                },

                new CropViewItem()
                {
                    Year = 1986,
                    CropType = CropType.Wheat,
                },
            };

            farm.StageStates.Add(stageState);

            var detailsScreenViewItem = new CropViewItem();
            detailsScreenViewItem.CropType = CropType.Barley;
            detailsScreenViewItem.Year = 1985;
            detailsScreenViewItem.FieldSystemComponentGuid = fieldSystemComponent.Guid;

            _resultsService.AssignYieldToYear(farm, detailsScreenViewItem, fieldSystemComponent);

            Assert.AreEqual(150, detailsScreenViewItem.Yield);
        }

        #endregion

        #region CalculateCarbonDioxideEmissionsFromCroppingFuelUse Tests

        /// <summary>
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideEmissionsFromCroppingFuelUseReturnsCorrectValue()
        {
            var energyFromFuelUse = 36.875;
            var areaOfCrop = 117.125;
            var dieselConversion = 121.125;
            var result =
                _resultsService.CalculateCarbonDioxideEmissionsFromCroppingFuelUse(energyFromFuelUse, areaOfCrop,
                    dieselConversion);
            Assert.AreEqual(523136.982421875, result);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromFallowingFuelUseReturnsCorrectValue()
        {
            var energyFromFuelUse = 60.750;
            var areaOfFallow = 68.875;
            var dieselConversion = 89.000;
            var result =
                _resultsService.CalculateCarbonDioxideEmissionsFromCroppingFuelUse(energyFromFuelUse, areaOfFallow,
                    dieselConversion);
            Assert.AreEqual(372389.90625, result);
        }

        #endregion

        #region CalculateCarbonDioxideEmissionsFromCroppingHerbicideProductionReturnsCorrectValue Tests

        /// <summary>
        /// Equation 4.1.2-1
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideEmissionsFromCroppingHerbicideProductionReturnsCorrectValue()
        {
            var energyForHerbicideProduction = 12.125;
            var areaOfCrop = 27.375;
            var herbicideConversion = 240.625;
            var result =
                _resultsService.CalculateCarbonDioxideEmissionsFromCroppingHerbicideProduction(
                    energyForHerbicideProduction, areaOfCrop, herbicideConversion);
            Assert.AreEqual(79868.701171875, result);
        }

        #endregion


        /// <summary>
        ///  Equation 6.1.4-1 Test
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromIrrigationReturnsCorrectValueElectricPump()
        {
            var areaOfCropIrrigated = 87.375;
            var irrigationConversion = 93.625;

            var farm = new Farm();
            farm.Defaults.DefaultPumpType = PumpType.ElectricPump;


            var carboxDioxideEmission = _resultsService.CalculateTotalCarbonDioxideEmissionsFromIrrigation(areaOfCropIrrigated,
                                                                                                           irrigationConversion,
                                                                                                           farm.Defaults.PumpEmissionFactor);
            var result = System.Math.Round(carboxDioxideEmission, 2);
            Assert.AreEqual(2176.01, result);
        }


        /// <summary>
        ///  Equation 6.1.4-1 Test
        /// </summary>
        [TestMethod]
        public void CalculateTotalCarbonDioxideEmissionsFromIrrigationReturnsCorrectValueGasPump()
        {
            var areaOfCropIrrigated = 87.375;
            var irrigationConversion = 93.625;

            var farm = new Farm();
            farm.Defaults.DefaultPumpType = PumpType.NaturalGasPump;

            var carboxDioxideEmission = _resultsService.CalculateTotalCarbonDioxideEmissionsFromIrrigation(areaOfCropIrrigated,
                                                                                                           irrigationConversion,
                                                                                                           farm.Defaults.PumpEmissionFactor);
            var result = System.Math.Round(carboxDioxideEmission, 2);
            Assert.AreEqual(9366.65, result);
        }


        /// <summary>
        ///  Equation 4.1.5-1
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideEmissionsFromCroppingEnergyUseCorrectValue()
        {
            var result = _resultsService.CalculateCarbonDioxideEmissionsFromCroppingEnergyUse(1.23, 2.34, 3.45, 4.56, 5.67);
            var truncResult = System.Math.Round(result, 2);
            Assert.AreEqual(17.25, truncResult);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void CalculateCarbonDioxideEmissionsFromCroppingEnergyUseReturnsCorrectValue()
        {
            var totalCarbonDioxideEmissionsFromFallowingFuelUse = 101.625;
            var totalCarbonDioxideEmissionsFromCroppingHerbicideProduction = 99.375;
            var totalCarbonDioxideEmissionsFromFallowHerbicideProduction = 60.456;
            var totalCarbonDioxideEmissionsFromNitrogenFertilizerProduction = 246.000;
            var totalCarbonDioxideEmissionsFromP2O5FertilizerProduction = 94.750;
            var totalCarbonDioxideEmissionsFromPotassiumProduction = 92.460;
            var totalCarbonDioxideEmissionsFromIrrigation = 62.125;

            var result = _resultsService.CalculateTotalCarbonDioxideEmissionsFromCroppingEnergyUse(
                totalCarbonDioxideEmissionsFromFallowingFuelUse,
                totalCarbonDioxideEmissionsFromCroppingHerbicideProduction,
                totalCarbonDioxideEmissionsFromFallowHerbicideProduction,
                totalCarbonDioxideEmissionsFromNitrogenFertilizerProduction,
                totalCarbonDioxideEmissionsFromP2O5FertilizerProduction,
                totalCarbonDioxideEmissionsFromPotassiumProduction,
                totalCarbonDioxideEmissionsFromIrrigation);
            Assert.AreEqual(756.791, result);
        }

        [TestMethod]
        public void ApplyUserDefaults()
        {
            var viewItem = new CropViewItem() { CropType = CropType.Barley, };
            var cropDefault = new CropViewItem()
            { CropType = CropType.Barley, Yield = 777, EnableCustomUserDefaultsForThisCrop = true };

            var globalSettings = new GlobalSettings();
            globalSettings.CropDefaults.Add(cropDefault);

            _resultsService.AssignUserDefaults(viewItem, globalSettings);

            Assert.AreEqual(777, viewItem.Yield);
        }

        #region AssignCarbonInputs Tests

        [TestMethod]
        public void AssignCarbonInputsSetsCarbonInputWhenThereIsMissingYieldDataForFirstYear()
        {
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

            var orderedViewItems = new List<CropViewItem>()
            {
                new CropViewItem()
                {
                    Year = 1985,
                    CropType = CropType.TameGrass,
                    PercentageOfProductYieldReturnedToSoil = 35,
                    MoistureContentOfCrop = 0.12,
                    CarbonConcentration = 0.45,
                    PlantCarbonInAgriculturalProduct =
                        0, // Simulate a year when there is no yield input available (and thus no value for plant carbon in agricultural product either). We then can calculate that value using next year's data (but only when we are considering perennials)
                },

                new CropViewItem()
                {
                    Year = 1986,
                    CropType = CropType.TameGrass,
                    PercentageOfProductYieldReturnedToSoil = 35,
                    MoistureContentOfCrop = 0.12,
                    CarbonConcentration = 0.45,
                    PlantCarbonInAgriculturalProduct =
                        100, // This year does have a yield and therefore a C_p value that will be used to back-calculate the C_p for the previous year.
                },
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(orderedViewItems) };


            _resultsService.AssignCarbonInputs(orderedViewItems, farm, component);

            // Now we should have carbon input from product values for all items/years
            Assert.IsTrue(orderedViewItems.All(x => x.CarbonInputFromProduct > 0));
        }

        [TestMethod]
        public void AssignCarbonInputsSetsCarbonInputWhenThereIsMissingYieldDataForAYearInMiddleOfPerennialStand()
        {
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

            var orderedViewItems = new List<CropViewItem>()
            {
                new CropViewItem()
                {
                    Year = 1985,
                    CropType = CropType.TameGrass,
                    PercentageOfProductYieldReturnedToSoil = 35,
                    MoistureContentOfCrop = 0.12,
                    CarbonConcentration = 0.45,
                    PlantCarbonInAgriculturalProduct = 200,
                },

                new CropViewItem()
                {
                    Year = 1986,
                    CropType = CropType.TameGrass,
                    PercentageOfProductYieldReturnedToSoil = 35,
                    MoistureContentOfCrop = 0.12,
                    CarbonConcentration = 0.45,
                    PlantCarbonInAgriculturalProduct =
                        0, // Simulate a year when there is no yield input available (and thus no value for plant carbon in agricultural product either). We then can calculate that value using next year's data (but only when we are considering perennials)
                },

                new CropViewItem()
                {
                    Year = 1987,
                    CropType = CropType.TameGrass,
                    PercentageOfProductYieldReturnedToSoil = 35,
                    MoistureContentOfCrop = 0.12,
                    CarbonConcentration = 0.45,
                    PlantCarbonInAgriculturalProduct =
                        100, // This year does have a yield and therefore a C_p value that will be used to back-calculate the C_p for the previous year.
                },
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(orderedViewItems) };

            _resultsService.AssignCarbonInputs(orderedViewItems, farm, component);

            // Now we should have carbon input from product values for all items/years
            Assert.IsTrue(orderedViewItems.All(x => x.CarbonInputFromProduct > 0));
        }

        [TestMethod]
        public void AssignCarbonInputsWhenACoverCropIsUsed()
        {
            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2000},
                new CropViewItem() {CropType = CropType.AlfalfaMedicagoSativaL, Year = 2000},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            _resultsService.AssignCarbonInputs(viewItems, new Farm(), component);
        }

        [TestMethod]
        public void AssignCarbonInputsWhenThereIsMissingYieldDataForFinalYearOfPerennialStand()
        {
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

            var orderedViewItems = new List<CropViewItem>()
            {
                new CropViewItem()
                {
                    Year = 1985,
                    CropType = CropType.TameGrass,
                    PercentageOfProductYieldReturnedToSoil = 35,
                    MoistureContentOfCrop = 0.12,
                    CarbonConcentration = 0.45,
                    PlantCarbonInAgriculturalProduct = 200,
                },

                new CropViewItem()
                {
                    Year = 1986,
                    CropType = CropType.TameGrass,
                    PercentageOfProductYieldReturnedToSoil = 35,
                    MoistureContentOfCrop = 0.12,
                    CarbonConcentration = 0.45,
                    PlantCarbonInAgriculturalProduct = 100,
                },

                new CropViewItem()
                {
                    Year = 1987,
                    CropType = CropType.TameGrass,
                    PercentageOfProductYieldReturnedToSoil = 35,
                    MoistureContentOfCrop = 0.12,
                    CarbonConcentration = 0.45,
                    PlantCarbonInAgriculturalProduct =
                        0, // Simulate a year when there is no yield input available (and thus no value for plant carbon in agricultural product either). We then can calculate that value using next year's data (but only when we are considering perennials)
                },
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(orderedViewItems) };

            _resultsService.AssignCarbonInputs(orderedViewItems, farm, component);

            // Now we should have carbon input from product values for all items/years
            Assert.IsTrue(orderedViewItems.All(x => x.CarbonInputFromProduct > 0));
        }

        [TestMethod]
        public void AssignCarbonInputsUsingTier2StrategySetsInputs()
        {
            var guid = Guid.NewGuid();

            var farm = new Farm()
            {
                YieldAssignmentMethod = YieldAssignmentMethod.Custom,

                DefaultSoilData =
                {
                    ProportionOfSandInSoil = 0.25,
                },

                Defaults = new Defaults()
                {
                    CarbonModellingStrategy = CarbonModellingStrategies.IPCCTier2,
                    DefaultRunInPeriod = 2,
                }
            };

            var orderedViewItems = new List<CropViewItem>()
            {
                new CropViewItem()
                {
                    Year = 1985,
                    CropType = CropType.Barley,
                    FieldSystemComponentGuid = guid,
                    DryYield = 1000,
                },

                new CropViewItem()
                {
                    Year = 1986,
                    CropType = CropType.Barley,
                    FieldSystemComponentGuid = guid,
                    DryYield = 2000,
                },

                new CropViewItem()
                {
                    Year = 1987,
                    CropType = CropType.Barley,
                    FieldSystemComponentGuid = guid,
                    DryYield = 3000,
                },
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(orderedViewItems), Guid = guid };

            farm.Components.Add(component);

            _resultsService.AssignCarbonInputs(orderedViewItems, farm, component);

        }

        #endregion

        #region MergeDetailViewItems Tests

        [TestMethod]
        public void MergeDetailViewItems()
        {
            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {Year = 2000, CropType = CropType.Lentils},
                new CropViewItem() {Year = 2000, CropType = CropType.AlfalfaMedicagoSativaL}
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            var result = _resultsService.MergeDetailViewItems(viewItems, component);

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void MergeDetailViewItemsForSingleItem()
        {
            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {Year = 2000, CropType = CropType.Lentils},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            var result = _resultsService.MergeDetailViewItems(viewItems, component);

            Assert.AreEqual(1, result.Where(x => x.Year == 2000).Count());
        }

        [TestMethod]
        public void MergeDetailViewItemsForSingleItemWithFollowingYearViewItem()
        {
            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {Year = 2000, CropType = CropType.Lentils},
                new CropViewItem() {Year = 2001, CropType = CropType.AlfalfaMedicagoSativaL}
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            var result = _resultsService.MergeDetailViewItems(viewItems, component);

            Assert.AreEqual(1, result.Where(x => x.Year == 2000).Count());
            Assert.AreEqual(1, result.Where(x => x.Year == 2001).Count());
        }

        #endregion

        #region GetMainCropForYear Tests

        [TestMethod]
        public void GetMainCropForYearReturnsMainCrop()
        {
            var detailViewItems = new List<CropViewItem>()
            {
                new CropViewItem()
                {
                    CropType = CropType.Lentils, Year = 2000,
                },

                new CropViewItem()
                {
                    CropType = CropType.TameGrass, IsSecondaryCrop = true, Year = 2000
                }
            };

            var result = _resultsService.GetMainCropForYear(
                viewItems: detailViewItems,
                year: 2000,
                fieldSystemComponent: new FieldSystemComponent());

            Assert.AreEqual(CropType.Lentils, result.CropType);
        }

        [TestMethod]
        public void GetCoverCropForYearReturnsMainCrop()
        {
            var detailViewItems = new List<CropViewItem>()
            {
                new CropViewItem()
                {
                    CropType = CropType.Lentils,  Year = 2000,
                },

                new CropViewItem()
                {
                    //We set IsSecondaryCrop to false to test situations with old farms where this boolean would not have been set
                    CropType = CropType.TameGrass, IsSecondaryCrop = false, Year = 2000
                }
            };

            // Since the IsSecondaryCrop is FALSE we expect the first item in the list to be returned as a workaround for old farms
            var result = _resultsService.GetMainCropForYear(
                viewItems: detailViewItems,
                year: 2000,
                fieldSystemComponent: new FieldSystemComponent());

            Assert.AreEqual(CropType.Lentils, result.CropType);
        }

        #endregion

        #region CombineInputsForAllCropsInSameYear Tests

        [TestMethod]
        public void CombineInputsForAllCropsInSameYear()
        {
            var viewItem1 = new CropViewItem() { Name = "Main crop", CropType = CropType.Barley, Year = 2000, AboveGroundCarbonInput = 200, BelowGroundCarbonInput = 50 };
            var viewItem2 = new CropViewItem() { Name = "Undersown crop", CropType = CropType.TameGrass, Year = 2000, AboveGroundCarbonInput = 100, BelowGroundCarbonInput = 75, IsSecondaryCrop = true };

            // Inputs from this item should not be considered because it is for the following year
            var viewItem3 = new CropViewItem() { Name = "Main crop for following year", CropType = CropType.TameGrass, Year = 2001, AboveGroundCarbonInput = 1000, BelowGroundCarbonInput = 2000 };

            // Inputs from this item should not be considered because it is for the previous year
            var viewItem4 = new CropViewItem() { Name = "Main crop for previous year", CropType = CropType.TameGrass, Year = 1999, AboveGroundCarbonInput = 1000, BelowGroundCarbonInput = 2000 };

            var viewItems = new List<CropViewItem>()
            {
                viewItem2,
                viewItem1,
                viewItem3,
                viewItem4
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            _resultsService.CombineInputsForAllCropsInSameYear(viewItems, component);

            // The main crop gets the total inputs from all other crops added to its inputs since ICBM will use this main crop when calculating final results
            Assert.AreEqual(300, viewItem1.CombinedAboveGroundInput);
            Assert.AreEqual(100, viewItem2.AboveGroundCarbonInput);

            // Inputs from other crops in same year remain the same
            Assert.AreEqual(50, viewItem1.BelowGroundCarbonInput);
            Assert.AreEqual(75, viewItem2.BelowGroundCarbonInput);

            // These values should remain the same since there is only one item for 2001
            Assert.AreEqual(1000, viewItem3.AboveGroundCarbonInput);
            Assert.AreEqual(2000, viewItem3.BelowGroundCarbonInput);

            // These values should remain the same since there is only one item for 1999
            Assert.AreEqual(1000, viewItem4.AboveGroundCarbonInput);
            Assert.AreEqual(2000, viewItem4.BelowGroundCarbonInput);
        }

        [TestMethod]
        public void CombineInputsForAllCropsInSameYearForSingleItem()
        {
            var viewItem1 = new CropViewItem() { Name = "Main crop", CropType = CropType.Barley, Year = 2000, AboveGroundCarbonInput = 200, BelowGroundCarbonInput = 50 };

            var viewItems = new List<CropViewItem>()
            {
                viewItem1
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            _resultsService.CombineInputsForAllCropsInSameYear(viewItems, component);

            Assert.AreEqual(200, viewItem1.AboveGroundCarbonInput);
            Assert.AreEqual(50, viewItem1.BelowGroundCarbonInput);
        }

        #endregion

        #region CreateOrderedViewItems

        [TestMethod]
        public void CreateOrderedViewItemsReturnsCorrectOrderingWhenUserSelectsToOrderFromStartYear()
        {
            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Oats, Year = 1989},
                new CropViewItem() {CropType = CropType.Wheat, Year = 1990},
            };

            var component = new FieldSystemComponent()
            {
                StartYear = 1980,
                EndYear = 1990,
                BeginOrderingAtStartYearOfRotation = true,
            };

            var result = _resultsService.CreateOrderedViewItems(component, viewItems, new Farm() { Defaults = new Defaults { CarbonModellingStrategy = CarbonModellingStrategies.ICBM } });

            Assert.AreEqual(CropType.Oats, result.Single(x => x.Year == 1980).CropType);
            Assert.AreEqual(CropType.Wheat, result.Single(x => x.Year == 1981).CropType);
        }

        [TestMethod]
        public void CreateOrderedViewItemsReturnsCorrectOrderingWhenUserSelectsToOrderFromEndYear()
        {
            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Oats, Year = 1990},
                new CropViewItem() {CropType = CropType.Wheat, Year = 1989},
            };

            var component = new FieldSystemComponent()
            {
                StartYear = 1980,
                EndYear = 1990,
                BeginOrderingAtStartYearOfRotation = false,
            };

            var result = _resultsService.CreateOrderedViewItems(component, viewItems, new Farm());

            Assert.AreEqual(CropType.Oats, result.Single(x => x.Year == 1990).CropType);
            Assert.AreEqual(CropType.Wheat, result.Single(x => x.Year == 1989).CropType);
        }

        [TestMethod]
        public void CreateOrderedViewItemsBeginningFromStartYearForAnnualCrops()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                StartYear = 1985,
                EndYear = 1990,
                BeginOrderingAtStartYearOfRotation = true,
            };

            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 1989},
                new CropViewItem() {CropType = CropType.Oats, Year = 1990},
            };

            var result = _resultsService.CreateOrderedViewItems(fieldSystemComponent, viewItems, new Farm() { Defaults = new Defaults { CarbonModellingStrategy = CarbonModellingStrategies.ICBM } });

            Assert.IsTrue(result.Single(x => x.Year == 1985).CropType == CropType.Lentils);
            Assert.IsTrue(result.Single(x => x.Year == 1986).CropType == CropType.Oats);
            Assert.IsTrue(result.Single(x => x.Year == 1987).CropType == CropType.Lentils);
            Assert.IsTrue(result.Single(x => x.Year == 1988).CropType == CropType.Oats);
            Assert.IsTrue(result.Single(x => x.Year == 1989).CropType == CropType.Lentils);
            Assert.IsTrue(result.Single(x => x.Year == 1990).CropType == CropType.Oats);
        }

        [TestMethod]
        public void CreateOrderedViewItemsBeginningFromEndYearForAnnualCrops()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                StartYear = 1985,
                EndYear = 1990,
                BeginOrderingAtStartYearOfRotation = false,
            };

            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 1989},
                new CropViewItem() {CropType = CropType.Oats, Year = 1990},
            };

            var result = _resultsService.CreateOrderedViewItems(fieldSystemComponent, viewItems, new Farm());

            Assert.IsTrue(result.Single(x => x.Year == 1990).CropType == CropType.Oats);
            Assert.IsTrue(result.Single(x => x.Year == 1989).CropType == CropType.Lentils);
            Assert.IsTrue(result.Single(x => x.Year == 1988).CropType == CropType.Oats);
            Assert.IsTrue(result.Single(x => x.Year == 1987).CropType == CropType.Lentils);
            Assert.IsTrue(result.Single(x => x.Year == 1986).CropType == CropType.Oats);
            Assert.IsTrue(result.Single(x => x.Year == 1985).CropType == CropType.Lentils);
        }

        [TestMethod]
        public void CreateOrderedViewItemsBeginningFromStartYearForUndersownCrops()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                StartYear = 2000,
                EndYear = 2004,
                BeginOrderingAtStartYearOfRotation = true,
            };

            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Barley, Year = 2000},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2000},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2001},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2002},
            };

            var result = _resultsService.CreateOrderedViewItems(fieldSystemComponent, viewItems, new Farm() { Defaults = new Defaults { CarbonModellingStrategy = CarbonModellingStrategies.ICBM } });

            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.Barley));
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.TameGrass)); // Undersown from 2001
            Assert.IsTrue(result.Any(x => x.Year == 2001 && x.CropType == CropType.TameGrass));
            Assert.IsTrue(result.Any(x => x.Year == 2002 && x.CropType == CropType.TameGrass));
            Assert.IsTrue(result.Any(x => x.Year == 2003 && x.CropType == CropType.Barley));
            Assert.IsTrue(result.Any(x => x.Year == 2003 && x.CropType == CropType.TameGrass)); // Undersown from 2004
            Assert.IsTrue(result.Any(x => x.Year == 2004 && x.CropType == CropType.TameGrass));
        }

        [TestMethod]
        public void CreateOrderedViewItemsBeginningFromEndYearUndersownCrops()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                StartYear = 1998,
                EndYear = 2001,
                BeginOrderingAtStartYearOfRotation = false,
            };

            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Barley, Year = 2000},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2000},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2001},
            };

            var result = _resultsService.CreateOrderedViewItems(fieldSystemComponent, viewItems, new Farm());

            Assert.IsTrue(result.Any(x => x.Year == 2001 && x.CropType == CropType.TameGrass));
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.TameGrass)); // Undersown from 2001
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.Barley));
            Assert.IsTrue(result.Any(x => x.Year == 1999 && x.CropType == CropType.TameGrass));
            Assert.IsTrue(result.Any(x => x.Year == 1998 && x.CropType == CropType.TameGrass)); // Undersown from 1999
            Assert.IsTrue(result.Any(x => x.Year == 1998 && x.CropType == CropType.Barley));
        }

        #endregion

        #region AssignUndersownCropViewItemsDescription Tests

        [TestMethod]
        public void AssignUndersownCropViewItemsDescriptionSetsDescriptionForUndersownYearOnly()
        {
            var viewItem1 = new CropViewItem() { CropType = CropType.TameMixed, YearInPerennialStand = 1, UnderSownCropsUsed = true, IsSecondaryCrop = true };
            var viewItem2 = new CropViewItem() { CropType = CropType.TameMixed, YearInPerennialStand = 2, UnderSownCropsUsed = true, IsSecondaryCrop = true };

            var viewItems = new List<CropViewItem>()
            {
                viewItem1, viewItem2
            };

            _resultsService.AssignUndersownCropViewItemsDescription(viewItems);

            Assert.IsTrue(viewItem1.Description.IndexOf("undersown", StringComparison.InvariantCultureIgnoreCase) >= 0);
            Assert.IsTrue(string.IsNullOrWhiteSpace(viewItem2.Description));
        }

        #endregion

        #region PreProcessViewItems

        [TestMethod]
        public void PreProcessViewItemsConsidersSingleUndersownSequence()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                StartYear = 2000,
                EndYear = 2005,
                CropViewItems = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.Barley, Year = 2000},
                    new CropViewItem() {CropType = CropType.TameGrass, Year = 2001},
                },

                CoverCrops = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.TameGrass, Year = 2000},
                }
            };

            var result = _resultsService.PreProcessViewItems(fieldSystemComponent);

            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.Barley));
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.TameGrass));
            Assert.IsTrue(result.Any(x => x.Year == 2001 && x.CropType == CropType.TameGrass));
        }

        [TestMethod]
        public void PreProcessViewItemsConsidersMultipleUndersownSequences()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                CropViewItems = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.Barley, Year = 2000},
                    new CropViewItem() {CropType = CropType.TameGrass, Year = 2001},

                    new CropViewItem() {CropType = CropType.Lentils, Year = 2002},
                    new CropViewItem() {CropType = CropType.TameMixed, Year = 2003},
                },

                CoverCrops = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.TameGrass, Year = 2000 },   // Hay grass is undersown into the barley
                    new CropViewItem() {CropType = CropType.TameMixed, Year = 2002, },  // Hay mixed is undersown into the lentils
                }
            };

            var result = _resultsService.PreProcessViewItems(fieldSystemComponent);

            Assert.AreEqual(6, result.Count());

            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.Barley));
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.TameGrass));
            Assert.IsTrue(result.Any(x => x.Year == 2001 && x.CropType == CropType.TameGrass));
            Assert.IsTrue(result.Any(x => x.Year == 2002 && x.CropType == CropType.Lentils));
            Assert.IsTrue(result.Any(x => x.Year == 2003 && x.CropType == CropType.TameMixed));
            Assert.IsTrue(result.Any(x => x.Year == 2003 && x.CropType == CropType.TameMixed));
        }

        [TestMethod]
        public void PreProcessViewItemsAddsCoverCropItems()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                StartYear = 2000,
                EndYear = 2005,
                CropViewItems = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.Lentils, Year = 2000},
                },

                CoverCrops = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.AlfalfaMedicagoSativaL, Year = 2000},
                }
            };

            var result = _resultsService.PreProcessViewItems(fieldSystemComponent);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.Lentils));
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.AlfalfaMedicagoSativaL));
        }

        #endregion

        #region CreateItems Tests

        [TestMethod]
        public void CreateItemsForCoverCrops()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                StartYear = 2000,
                EndYear = 2001,
                CropViewItems = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.Barley, Year = 2000},
                },

                CoverCrops = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.AlfalfaMedicagoSativaL, Year = 2000}
                }
            };

            var result = _resultsService.CreateItems(fieldSystemComponent, new Farm() { Defaults = new Defaults { CarbonModellingStrategy = CarbonModellingStrategies.ICBM } });

            Assert.AreEqual(4, result.Count());

            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.Barley));
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.AlfalfaMedicagoSativaL));
            Assert.IsTrue(result.Any(x => x.Year == 2001 && x.CropType == CropType.Barley));
            Assert.IsTrue(result.Any(x => x.Year == 2001 && x.CropType == CropType.AlfalfaMedicagoSativaL));
        }

        [TestMethod]
        public void CreateItemsForUndersownCrops()
        {
            var fieldSystemComponent = new FieldSystemComponent()
            {
                StartYear = 2000,
                EndYear = 2001,
                CropViewItems = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.Barley, Year = 2000},
                    new CropViewItem() {CropType = CropType.TameGrass, Year = 2001}
                },

                CoverCrops = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() {CropType = CropType.TameGrass, Year = 2000}
                }
            };

            var result = _resultsService.CreateItems(fieldSystemComponent, new Farm() { Defaults = new Defaults { CarbonModellingStrategy = CarbonModellingStrategies.ICBM } });

            Assert.AreEqual(3, result.Count());

            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.Barley));
            Assert.IsTrue(result.Any(x => x.Year == 2000 && x.CropType == CropType.TameGrass));
            Assert.IsTrue(result.Any(x => x.Year == 2001 && x.CropType == CropType.TameGrass));
        }

        #endregion

        #region GetAdjoiningYears Test

        [TestMethod]
        public void GetAdjoiningYearsForPerennials()
        {
            var standId = Guid.NewGuid();

            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2000},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2000, UnderSownCropsUsed = true, PerennialStandGroupId = standId},

                new CropViewItem() {CropType = CropType.TameGrass, Year = 2001, PerennialStandGroupId = standId},
                new CropViewItem() {CropType = CropType.TameGrass, Year = 2002, PerennialStandGroupId = standId},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            // In the year 2000 we have two crops being grown, this is why careful consideration must be made so that we return the undersown perennial and not the annual being grown in the year 2000
            var result = _resultsService.GetAdjoiningYears(viewItems, 2001, component);

            Assert.AreEqual(CropType.TameGrass, result.PreviousYearViewItem.CropType);
            Assert.AreEqual(2000, result.PreviousYearViewItem.Year);

            Assert.AreEqual(CropType.TameGrass, result.CurrentYearViewItem.CropType);
            Assert.AreEqual(2001, result.CurrentYearViewItem.Year);

            Assert.AreEqual(CropType.TameGrass, result.NextYearViewItem.CropType);
            Assert.AreEqual(2002, result.NextYearViewItem.Year);
        }

        [TestMethod]
        public void GetAdjoiningYearsForAnnuals()
        {
            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2000},
                new CropViewItem() {CropType = CropType.Lentils, Year = 2001},
                new CropViewItem() {CropType = CropType.Lentils, Year = 2002},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            var result = _resultsService.GetAdjoiningYears(viewItems, 2001, component);

            Assert.IsNull(result.PreviousYearViewItem);

            Assert.AreEqual(CropType.Lentils, result.CurrentYearViewItem.CropType);
            Assert.AreEqual(2001, result.CurrentYearViewItem.Year);

            Assert.IsNull(result.NextYearViewItem);
        }

        /// <summary>
        /// Edge case test when there is only one view item
        /// </summary>
        [TestMethod]
        public void GetAdjoiningYearsForSingleYear()
        {
            var viewItems = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Lentils, Year = 2000},
            };

            var component = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>(viewItems) };

            var result = _resultsService.GetAdjoiningYears(viewItems, 2000, component);

            Assert.IsNull(result.PreviousYearViewItem);
            Assert.AreEqual(CropType.Lentils, result.CurrentYearViewItem.CropType);
            Assert.IsNull(result.NextYearViewItem);
        }

        #endregion

        #region CreateDetailViewItems Tests

        [TestMethod]
        public void CreateDetailViewItemsAddsMainCropsToStageState()
        {
            var farm = new Farm();

            var mainCrops = new List<CropViewItem>() { new CropViewItem() };

            var fieldComponent = new FieldSystemComponent()
            { CropViewItems = new ObservableCollection<CropViewItem>(mainCrops) };

            _resultsService.CreateDetailViewItems(
                fieldSystemComponent: fieldComponent,
                farm: farm);

            var stageState = farm.StageStates.OfType<FieldSystemDetailsStageState>().Single();

            Assert.IsTrue(stageState.DetailsScreenViewCropViewItems.Any());
        }

        #endregion

        #region GetRunInItems Tests

        [TestMethod]
        public void GetRunInItems()
        {
            var farm = new Farm()
            {
                Defaults = new Defaults()
                {
                    DefaultRunInPeriod = 3,
                }
            };

            const int startYear = 1985;

            var viewItemsInRotation = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Barley},
                new CropViewItem() {CropType = CropType.Wheat},
                new CropViewItem() {CropType = CropType.TameGrass},
                new CropViewItem() {CropType = CropType.TameGrass},
            };

            var viewItemsForField = new List<CropViewItem>()
            {
                new CropViewItem() {CropType = CropType.Barley, Year = startYear + 0},
                new CropViewItem() {CropType = CropType.Wheat, Year = startYear + 1},
                new CropViewItem() {CropType = CropType.TameGrass, Year = startYear + 2},
                new CropViewItem() {CropType = CropType.TameGrass, Year = startYear + 3},
            };

            var items = _resultsService.GetRunInPeriodItems(
                farm: farm,
                viewItemsForRotation: viewItemsInRotation,
                startYearOfField: startYear,
                viewItemsForField: viewItemsForField, fieldComponent: new FieldSystemComponent());

            // Assert that 3 items were created
            Assert.AreEqual(3, items.Count);

            // Assert that the correct years were assigned
            Assert.AreEqual(1982, items[0].Year);
            Assert.AreEqual(1983, items[1].Year);
            Assert.AreEqual(1984, items[2].Year);

            // Assert that the correct crop types were assigned. The history continues back in time so the result should be W-H-H
            Assert.AreEqual(items[0].CropType, CropType.Wheat);
            Assert.AreEqual(items[1].CropType, CropType.TameGrass);
            Assert.AreEqual(items[2].CropType, CropType.TameGrass);
        }

        #endregion


        [TestMethod]
        public void CalculateCarbonLostFromHayExport()
        {
            var fieldId = Guid.NewGuid();

            var hayExportViewItem = new CropViewItem();
            hayExportViewItem.Year = DateTime.Now.Year;
            hayExportViewItem.PercentageOfProductYieldReturnedToSoil = 2;
            hayExportViewItem.HarvestViewItems.Add(new HarvestViewItem() { TotalNumberOfBalesHarvested = 20, Start = DateTime.Now });

            var exportingFieldComponent = new FieldSystemComponent();
            exportingFieldComponent.Guid = fieldId;
            exportingFieldComponent.Name = "Exporting field";
            exportingFieldComponent.CropViewItems.Add(hayExportViewItem);

            var importingViewItem = new CropViewItem();
            importingViewItem.HayImportViewItems.Add(new HayImportViewItem() { Date = DateTime.Now, FieldSourceGuid = fieldId, NumberOfBales = 5, MoistureContentAsPercentage = 20 });

            var importingFieldComponent = new FieldSystemComponent();
            importingFieldComponent.Name = "Importing field";
            importingFieldComponent.CropViewItems.Add(importingViewItem);

            var farm = new Farm();
            farm.Components.Add(importingFieldComponent);
            farm.Components.Add(exportingFieldComponent);

            _resultsService.CalculateCarbonLostFromHayExports(exportingFieldComponent, farm);

            Assert.AreEqual(5.5102040816326534, hayExportViewItem.TotalCarbonLossFromBaleExports);
        }

        [TestMethod]
        public void CalculateCarbonUptakeByGrazingAnimals()
        {
            var farm = new Farm();
            var fieldSystemComponent = new FieldSystemComponent();
            var cropViewItem = new CropViewItem();

            var cowCalfComponent = new CowCalfComponent();
            var cowCalfComponentGuid = Guid.NewGuid();
            cowCalfComponent.Guid = cowCalfComponentGuid;

            var animalGroup = new AnimalGroup();
            var animalGroupGuid = Guid.NewGuid();
            animalGroup.Guid = animalGroupGuid;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = new HousingDetails();
            managementPeriod.HousingDetails.HousingType = HousingType.Pasture;
            managementPeriod.HousingDetails.PastureLocation = fieldSystemComponent;

            var managementPeriodGuid = Guid.NewGuid();
            managementPeriod.Guid = managementPeriodGuid;

            var grazingViewItem = new GrazingViewItem()
            {
                AnimalComponentGuid = cowCalfComponentGuid,
                AnimalGroupGuid = animalGroupGuid,
                ManagementPeriodGuid = managementPeriodGuid,
            };

            cropViewItem.GrazingViewItems.Add(grazingViewItem);
            fieldSystemComponent.CropViewItems.Add(cropViewItem);

            animalGroup.ManagementPeriods.Add(managementPeriod);
            cowCalfComponent.Groups.Add(animalGroup);

            farm.Components.Add(fieldSystemComponent);
            farm.Components.Add(cowCalfComponent);

            var animalResults = new AnimalGroupEmissionResults()
            {
                AnimalGroup = animalGroup,
                GroupEmissionsByMonths = new List<GroupEmissionsByMonth>()
                {
                    new GroupEmissionsByMonth(new MonthsAndDaysData() {ManagementPeriod = managementPeriod},
                        new List<GroupEmissionsByDay>()
                        {
                            new GroupEmissionsByDay()
                            {
                                TotalCarbonUptakeForGroup = 10,
                            },

                            new GroupEmissionsByDay()
                            {
                                TotalCarbonUptakeForGroup = 20,
                            },
                        })
                }
            };

            var animalComponentEmissionsResults = new ObservableCollection<AnimalComponentEmissionsResults>()
            {
                new AnimalComponentEmissionsResults()
                {
                    Component = cowCalfComponent,
                    EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>()
                    {
              animalResults,
                    }
                }
            };

            _mockAnimalResultsService.Setup(x => x.GetResultsForManagementPeriod(It.IsAny<AnimalGroup>(), It.IsAny<Farm>(), It.IsAny<AnimalComponentBase>(), It.IsAny<ManagementPeriod>())).Returns(animalResults);
            _resultsService.AnimalResultsService = _mockAnimalResultsService.Object;
            _resultsService.CalculateCarbonLostByGrazingAnimals(farm, fieldSystemComponent, animalComponentEmissionsResults);

            Assert.AreEqual(30, cropViewItem.TotalCarbonLossesByGrazingAnimals);
        }

        [TestMethod]
        public void CalculateAmmoniaEmissionFromLandAppliedManureForPoultry()
        {
            var date = DateTime.Now.Subtract(TimeSpan.FromDays(30));

            var animalComponentResults = new AnimalComponentEmissionsResults();
            animalComponentResults.EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>() { new AnimalGroupEmissionResults() };

            var animalGroupEmissionResults = new AnimalGroupEmissionResults();
            animalGroupEmissionResults.GroupEmissionsByMonths = new List<GroupEmissionsByMonth>();

            var dailyEmissions = new GroupEmissionsByDay();
            dailyEmissions.DateTime = date;
            dailyEmissions.AccumulatedTANAvailableForLandApplicationOnDay = 5;
            dailyEmissions.TotalVolumeOfManureAvailableForLandApplication = 200;

            var groupEmissionsByMonth = new GroupEmissionsByMonth(new MonthsAndDaysData(), new List<GroupEmissionsByDay>() { dailyEmissions });

            animalGroupEmissionResults.GroupEmissionsByMonths.Add(groupEmissionsByMonth);
            animalComponentResults.EmissionResultsForAllAnimalGroupsInComponent.Add(animalGroupEmissionResults);

            var field = new FieldSystemComponent();

            var crop = new CropViewItem();
            var manureApplication = new ManureApplicationViewItem();
            manureApplication.AnimalType = AnimalType.Poultry;
            manureApplication.DateOfApplication = date;
            manureApplication.ManureLocationSourceType = ManureLocationSourceType.Livestock;
            manureApplication.AmountOfManureAppliedPerHectare = 100;

            crop.ManureApplicationViewItems.Add(manureApplication);
            field.CropViewItems.Add(crop);

            var farm = new Farm();
            farm.Components.Add(field);
        }

        [TestMethod]
        public void CalculateCarbonUptakeByGrazingAnimalsWithOnePeriod()
        {
            var farm = base.GetTestFarm();
            var fieldComponent = base.GetTestFieldComponent();
            var cropViewItem = base.GetTestCropViewItem();
            fieldComponent.CropViewItems.Add(cropViewItem);

            var animalResults = base.GetTestGrazingBeefCattleAnimalGroupComponentEmissionsResults(fieldComponent);
            var animalComponentResults = base.GetTestGrazingBeefCattleAnimalComponentEmissionsResults(fieldComponent);
            var animalEmissionResults = new List<AnimalComponentEmissionsResults>() { animalComponentResults };

            _mockAnimalResultsService.Setup(x => x.GetResultsForManagementPeriod(It.IsAny<AnimalGroup>(), It.IsAny<Farm>(), It.IsAny<AnimalComponentBase>(), It.IsAny<ManagementPeriod>())).Returns(animalResults);
            _resultsService.AnimalResultsService = _mockAnimalResultsService.Object;

            _resultsService.CalculateCarbonLostByGrazingAnimals(farm, fieldComponent, animalEmissionResults);

            Assert.AreEqual(100, cropViewItem.TotalCarbonLossesByGrazingAnimals);
        }

        [TestMethod]
        public void SetSpinUpTillageTypeTest()
        {

        }

        #endregion
    }
}