using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.Animals.OtherAnimals;
using H.Core.Models.Animals.Sheep;
using H.Core.Models.Animals.Swine;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Core.Providers.Fertilizer;
using H.Core.Services.Initialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services.Initialization
{
    [TestClass]
    public class InitializationServiceTest
    {
        #region Fields

        private IInitializationService _initializationService;
        private List<Farm> _farms;
        private Farm _farm1;
        private Farm _farm2;

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
            _farms = new List<Farm>();

            _farm1 = new Farm();
            _farm2 = new Farm();

            _farms.Add(_farm1);
            _farms.Add(_farm2);

            _initializationService = new InitializationService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

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

            var result = _initializationService.CalculateAmountOfProductRequired(farm, viewItem, fertilizerApplicationViewItem);

            Assert.AreEqual(135.77142857142857, result);
        }

        [TestMethod]
        public void ReInitializeFarmsTest()
        {
            _farm1.ClimateData.BarnTemperatureData.January = 30;

            _initializationService.ReInitializeFarms(_farms);

            Assert.AreEqual(_farm1.ClimateData.BarnTemperatureData.January, 2.767742149);
        }

        [TestMethod]
        public void InitializeDefaultEmissionFactorsUpdatesDefault()
        {
            var managementPeriod = new ManagementPeriod()
            {
                ManureDetails = new ManureDetails()
                {
                    StateType = ManureStateType.SolidStorage,
                    MethaneConversionFactor = 4,
                }
            };

            _initializationService.InitializeDefaultEmissionFactors(_farm1, managementPeriod);

            Assert.AreEqual(expected: 0.02, actual: managementPeriod.ManureDetails.MethaneConversionFactor);
        }

        [TestMethod]
        public void InitializeManureMineralizationFractionsUpdatedDefaultValues()
        {
            var managementPeriod = new ManagementPeriod()
            {
                AnimalType = AnimalType.Beef,
                ManureDetails = new ManureDetails()
                {
                    StateType = ManureStateType.CompostIntensive,
                    FractionOfOrganicNitrogenNitrified = 10,
                }
            };

            _initializationService.InitializeManureMineralizationFractions(managementPeriod);

            Assert.AreEqual(expected: 0.25, actual: managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified);
        }

        [TestMethod]
        public void InitializeManureCompositionDataAssignsNewValueToManureDetails()
        {
            var managementPeriod = new ManagementPeriod()
            {
                ManureDetails = new ManureDetails()
                {
                    FractionOfPhosphorusInManure = 100,
                }
            };

            var manureComposition = new DefaultManureCompositionData
            {
                PhosphorusFraction = 20
            };

            _initializationService.InitializeManureCompositionData(managementPeriod, manureComposition);

            Assert.AreEqual(expected: 20, actual: managementPeriod.ManureDetails.FractionOfPhosphorusInManure);
        }

        [TestMethod]
        public void ReinitializeAverageMilkProduction()
        {
            var dairycow = new AnimalGroup()
            {
                GroupType = AnimalType.DairyLactatingCow,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        AnimalType = AnimalType.DairyLactatingCow,
                        Start = new DateTime(1990,  4, 25),
                        MilkProduction = 200,

                    }
                },
            };
            var dairyComponent = new DairyComponent();
            dairyComponent.Groups.Add(dairycow);
            _farm1.Components.Add(dairyComponent);

            _farm1.DairyComponents.Cast<DairyComponent>().First().Groups.Add(dairycow);
            _farm1.DefaultSoilData.Province = Province.BritishColumbia;
            _initializationService.InitializeMilkProduction(_farm1);
            Assert.AreEqual(24.3, dairycow.ManagementPeriods.First().MilkProduction);

        }
        [TestMethod]
        public void ReinitializeAverageMilkProductionYearNotFound()
        {
            var mp =
                new ManagementPeriod
                {
                    AnimalType = AnimalType.DairyLactatingCow,
                    Start = new DateTime(1989, 4, 25), //invalid year
                    MilkProduction = 200,

                };
            
        

        var dairycow2 = new AnimalGroup()
            {
                GroupType = AnimalType.DairyLactatingCow,
            };
            var dairyComponent2 = new DairyComponent();
            dairycow2.ManagementPeriods.Add(mp);
            dairyComponent2.Groups.Add(dairycow2);
            _farm2.Components.Add(dairyComponent2);
            _farm2.DairyComponents.Cast<DairyComponent>().First().Groups.Add(dairycow2);
            _farm2.DefaultSoilData.Province = Province.BritishColumbia;

            _initializationService.InitializeMilkProduction(_farm2);

            Assert.AreEqual(24.3, mp.MilkProduction);
        }
        [TestMethod]
        public void ReinitializeAverageMilkProductionIncorrectAnimalGroup()
        {
            var beef = new AnimalGroup()
            {
                GroupType = AnimalType.Beef,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        Start = new DateTime(1990,  4, 25),
                        MilkProduction = 1000,

                    }
                },
            };
            var dairyComponent = new DairyComponent();
            dairyComponent.Groups.Add(beef);
            _farm1.Components.Add(dairyComponent);

            _farm1.DairyComponents.Cast<DairyComponent>().First().Groups.Add(beef);
            _farm1.DefaultSoilData.Province = Province.BritishColumbia;
            _initializationService.InitializeMilkProduction(_farm1);
            Assert.AreEqual(0, beef.ManagementPeriods.First().MilkProduction);
        }
        [TestMethod]
        public void ReinitializationAverageMilkProductionNullArgument()
        {
            Farm farm = new Farm();
            try
            {
                _initializationService.InitializeMilkProduction(farm);//passing null farm
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            var dairyComponent = new DairyComponent();
            farm.Components.Add(dairyComponent);
            try
            {
                _initializationService.InitializeMilkProduction(farm);// passing farm with null dairyComponent
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod]
        public void ReinitializeFuelEnergyUpdatesDefaultTwoArguments()
        {
            var viewItem = new CropViewItem();
            viewItem.FuelEnergy = 12;
            _farm1.DefaultSoilData.Province = Province.BritishColumbia;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.Brown;
            viewItem.TillageType = TillageType.Reduced;
            viewItem.CropType = CropType.Flax;
            _initializationService.InitializeFuelEnergy(_farm1, viewItem);
            Assert.AreEqual(expected: 1.78, actual: viewItem.FuelEnergy);
        }
        [TestMethod]
        public void ReinitializeFuelEnergyUpdatesDefaultSingleArgument()
        {
            var cropViewItem = new CropViewItem();
            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();

            cropViewItem.TillageType = TillageType.NoTill;
            cropViewItem.CropType = CropType.Buckwheat;
            cropViewItemCollection.Add(cropViewItem);
            _farm1.DefaultSoilData.Province = Province.Ontario;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada;
            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);

            _initializationService.InitializeFuelEnergy(_farm1);

            var cropViewItemsPostInitialization = _farm1.GetCropDetailViewItems();
            Assert.AreEqual(expected: 1.34, actual: cropViewItemsPostInitialization[0].FuelEnergy);
        }
        [TestMethod]
        public void ReinitializeFuelEnergyUpdatesDefaultSingleArgumentMultipleCropViewItems()
        {
            var cropViewItemOne = new CropViewItem();
            var cropViewItemTwo = new CropViewItem();
            var cropViewItemThree = new CropViewItem();
            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();

            cropViewItemOne.TillageType = TillageType.NoTill;
            cropViewItemOne.CropType = CropType.Buckwheat;
            cropViewItemTwo.TillageType = TillageType.NoTill;
            cropViewItemTwo.CropType = CropType.GrainCorn;
            cropViewItemThree.TillageType = TillageType.NoTill;
            cropViewItemThree.CropType = CropType.PulseCrops;
            cropViewItemCollection.Add(cropViewItemOne);
            cropViewItemCollection.Add(cropViewItemTwo);
            cropViewItemCollection.Add(cropViewItemThree);
            _farm1.DefaultSoilData.Province = Province.Ontario;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada;
            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);

            _initializationService.InitializeFuelEnergy(_farm1);

            var cropViewItemsPostInitialization = _farm1.GetCropDetailViewItems();
            Assert.AreEqual(expected: 1.34, actual: cropViewItemsPostInitialization[0].FuelEnergy);
            Assert.AreEqual(expected: 1.9, actual: cropViewItemsPostInitialization[1].FuelEnergy);
            Assert.AreEqual(expected: 1.72, actual: cropViewItemsPostInitialization[2].FuelEnergy);
        }

        [TestMethod]
        public void ReinitializeFuelEnergyNullArgument()
        {
            var farm = new Farm();
            try
            {
                _initializationService.InitializeFuelEnergy(farm);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod]
        public void ReinitializeFuelEnergyNoCropViewItems()
        {
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();
            _farm1.DefaultSoilData.Province = Province.Ontario;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);
            try
            {
                _initializationService.InitializeFuelEnergy(_farm1);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        [TestMethod]
        public void ReinitializeFuelEnergyMissingFarmProperties()
        {
            var cropViewItem = new CropViewItem();
            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();

            cropViewItem.TillageType = TillageType.NoTill;
            cropViewItem.CropType = CropType.Buckwheat;
            cropViewItemCollection.Add(cropViewItem);

            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);

            try
            {
                _initializationService.InitializeFuelEnergy(_farm1);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        [TestMethod]
        public void ReinitializeHerbicideEnergyUpdatesDefaultTwoArguments()
        {
            var viewItem = new CropViewItem();
            viewItem.HerbicideEnergy = 12;
            _farm1.DefaultSoilData.Province = Province.Alberta;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.Black;
            viewItem.TillageType = TillageType.Intensive;
            viewItem.CropType = CropType.Fallow;
            _initializationService.InitializeHerbicideEnergy(_farm1, viewItem);
            Assert.AreEqual(expected: 0.06, actual: viewItem.HerbicideEnergy);
        }
        [TestMethod]
        public void ReinitializeHerbicideEnergyUpdatesDefaultSingleArgument()
        {
            var cropViewItem = new CropViewItem();
            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();

            cropViewItem.TillageType = TillageType.Intensive;
            cropViewItem.CropType = CropType.Fallow;
            cropViewItemCollection.Add(cropViewItem);
            _farm1.DefaultSoilData.Province = Province.Alberta;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.Black;
            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);

            _initializationService.InitializeHerbicideEnergy(_farm1);

            var cropViewItemsPostInitialization = _farm1.GetCropDetailViewItems();
            Assert.AreEqual(expected: 0.06, actual: cropViewItemsPostInitialization[0].HerbicideEnergy);
        }
        [TestMethod]
        public void ReinitializeHerbicideEnergyUpdatesDefaultSingleArgumentMultipleCropViewItems()
        {
            var cropViewItemOne = new CropViewItem();
            var cropViewItemTwo = new CropViewItem();
            var cropViewItemThree = new CropViewItem();
            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();

            cropViewItemOne.TillageType = TillageType.Reduced;
            cropViewItemOne.CropType = CropType.SmallGrainCereals;
            cropViewItemTwo.TillageType = TillageType.Reduced;
            cropViewItemTwo.CropType = CropType.GrainCorn;
            cropViewItemThree.TillageType = TillageType.Reduced;
            cropViewItemThree.CropType = CropType.RangelandNative;
            cropViewItemCollection.Add(cropViewItemOne);
            cropViewItemCollection.Add(cropViewItemTwo);
            cropViewItemCollection.Add(cropViewItemThree);
            _farm1.DefaultSoilData.Province = Province.Ontario;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada;
            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);

            _initializationService.InitializeHerbicideEnergy(_farm1);

            var cropViewItemsPostInitialization = _farm1.GetCropDetailViewItems();
            Assert.AreEqual(expected: 0.24, actual: cropViewItemsPostInitialization[0].HerbicideEnergy);
            Assert.AreEqual(expected: 0.12, actual: cropViewItemsPostInitialization[1].HerbicideEnergy);
            Assert.AreEqual(expected: 0, actual: cropViewItemsPostInitialization[2].HerbicideEnergy);
        }
        [TestMethod]
        public void ReinitializeHerbicideEnergyNullArgument()
        {
            var farm = new Farm();
            try
            {
                _initializationService.InitializeHerbicideEnergy(farm);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod]
        public void ReinitializeHerbicideEnergyNoCropViewItems()
        {
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();
            _farm1.DefaultSoilData.Province = Province.Ontario;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.EasternCanada;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);
            try
            {
                _initializationService.InitializeHerbicideEnergy(_farm1);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        [TestMethod]
        public void ReinitializeHerbicideEnergyMissingFarmProperties()
        {
            var cropViewItem = new CropViewItem();
            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();

            cropViewItem.TillageType = TillageType.NoTill;
            cropViewItem.CropType = CropType.Buckwheat;
            cropViewItemCollection.Add(cropViewItem);

            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);

            try
            {
                _initializationService.InitializeHerbicideEnergy(_farm1);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        [TestMethod]
        public void InitializeBeddingMaterialSetsTotalCarbon()
        {
            var housingDetails = new HousingDetails();
            housingDetails.TotalCarbonKilogramsDryMatterForBedding = 100;
            housingDetails.BeddingMaterialType = BeddingMaterialType.Straw;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = housingDetails;
            managementPeriod.AnimalType = AnimalType.BeefBackgrounderHeifer;

            var animalGroup = new AnimalGroup();
            animalGroup.ManagementPeriods.Add(managementPeriod);

            var component = new CowCalfComponent();
            component.Groups.Add(animalGroup);

            _farm1.Components.Add(component);

            _initializationService.InitializeBeddingMaterial(_farm1);

            Assert.AreEqual(0.447, housingDetails.TotalCarbonKilogramsDryMatterForBedding);
        }

        [TestMethod]
        public void InitializeAnnualEntericMethaneRateSetsNewDefault()
        {
            var managementPeriod = new ManagementPeriod()
            {
                AnimalType = AnimalType.Bison,
                ManureDetails = new ManureDetails()
                {
                    YearlyEntericMethaneRate = 10,
                }
            };

            _initializationService.InitializeAnnualEntericMethaneEmissionRate(managementPeriod);

            Assert.AreEqual(64, managementPeriod.ManureDetails.YearlyEntericMethaneRate);
        }

        [TestMethod]
        public void InitializeManureExcretionRateSetsNewExcretionRate()
        {
            var managementPeriod = new ManagementPeriod()
            {
                AnimalType = AnimalType.Horses,
                ManureDetails = new ManureDetails()
                {
                    ManureExcretionRate = 10,
                }
            };

            _initializationService.InitializeManureExcretionRate(managementPeriod);

            Assert.AreEqual(23, managementPeriod.ManureDetails.ManureExcretionRate);
        }
        [TestMethod]
        public void InitializeMethaneProducingCapacitySetsValue()
        {
            var managementPeriod = new ManagementPeriod()
            {
                AnimalType = AnimalType.Beef,

                ManureDetails = new ManureDetails()
                {
                    MethaneProducingCapacityOfManure = 10,
                }
            };

            _initializationService.InitializeMethaneProducingCapacityOfManure(managementPeriod);

            Assert.AreEqual(0.19, managementPeriod.ManureDetails.MethaneProducingCapacityOfManure);
        }

        [TestMethod]
        public void InitializeCattleFeedingActivityCoefficient()
        {
            Farm farm = new Farm();
            var beef = new AnimalGroup()
            {
                GroupType = AnimalType.Beef,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        AnimalType = AnimalType.Dairy,
                        HousingDetails =
                        {
                            HousingType = HousingType.EnclosedPasture,
                            ActivityCeofficientOfFeedingSituation = 100.0,
                        }

                    }
                },
            };
            var dairyComponent = new DairyComponent();
            dairyComponent.Groups.Add(beef);
            farm.Components.Add(dairyComponent);

            _initializationService.InitializeFeedingActivityCoefficient(farm);
            Assert.AreEqual(0.17, beef.ManagementPeriods.First().HousingDetails.ActivityCeofficientOfFeedingSituation);
        }

        [TestMethod]
        public void InitializeCattleFeedingActivityCoefficientNullArguments()
        {
            Farm farm = new Farm();
            try
            {
                _initializationService.InitializeFeedingActivityCoefficient(farm); //passing null farm
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void InitializeCattleFeedingActivityCoefficientNullComponent(){
            var farm = new Farm();
            var dairyComponent = new DairyComponent();
            farm.Components.Add(dairyComponent);
            try
            {
                _initializationService.InitializeFeedingActivityCoefficient(farm);// passing farm with null dairyComponent
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void InitializeStartAndEndWeightsForCattleManagementPeriods()
        {
            var managementPeriod1 = new ManagementPeriod()
            {
                AnimalType = AnimalType.BeefFinishingSteer,
            };
            var managementPeriod2 = new ManagementPeriod()
            {
                AnimalType = AnimalType.DairyCalves,
            };
            var managementPeriod3 = new ManagementPeriod()
            {
                AnimalType = AnimalType.SwineBoar,
            };

            var farm = new Farm();

            var beefGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefFinishingSteer,
            };
            beefGroup.ManagementPeriods.Add(managementPeriod1);
            var beefComponent = new FinishingComponent();
            beefComponent.Groups.Add(beefGroup);

            var dairyGroup = new AnimalGroup()
            {
                GroupType = AnimalType.DairyCalves,
            };
            dairyGroup.ManagementPeriods.Add(managementPeriod2);
            var dairyComponent = new DairyComponent();
            dairyComponent.Groups.Add(dairyGroup);

            var swineGroup = new AnimalGroup()
            {
                GroupType = AnimalType.SwineBoar,
            };
            swineGroup.ManagementPeriods.Add(managementPeriod3);
            var swineComponent = new FarrowToWeanComponent();
            swineComponent.Groups.Add(swineGroup);
            
            farm.Components.Add(beefComponent);
            farm.Components.Add(dairyComponent);
            farm.Components.Add(swineComponent);

            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod1);
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod2);
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod3);

            Assert.AreEqual(beefGroup.ManagementPeriods.ElementAt(0).StartWeight, 310, 0.01);
            Assert.AreEqual(beefGroup.ManagementPeriods.ElementAt(0).EndWeight, 610, 0.01);

            Assert.AreEqual(dairyGroup.ManagementPeriods.ElementAt(0).StartWeight, 45, 0.01);
            Assert.AreEqual(dairyGroup.ManagementPeriods.ElementAt(0).EndWeight, 127, 0.01);

            Assert.AreEqual(swineGroup.ManagementPeriods.ElementAt(0).StartWeight, 0, 0.01);
            Assert.AreEqual(swineGroup.ManagementPeriods.ElementAt(0).EndWeight, 0, 0.01);
        }

        [TestMethod]
        public void InitializeStartAndEndWeightsForCattleFarm()
        {
            var managementPeriod1 = new ManagementPeriod()
            {
                AnimalType = AnimalType.BeefFinishingSteer,
            };
            var managementPeriod2 = new ManagementPeriod()
            {
                AnimalType = AnimalType.DairyCalves,
            };
            var managementPeriod3 = new ManagementPeriod()
            {
                AnimalType = AnimalType.SwineBoar,
            };

            var farm = new Farm();

            var beefGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefFinishingSteer,
            };
            beefGroup.ManagementPeriods.Add(managementPeriod1);
            var beefComponent = new FinishingComponent();
            beefComponent.Groups.Add(beefGroup);

            var dairyGroup = new AnimalGroup()
            {
                GroupType = AnimalType.DairyCalves,
            };
            dairyGroup.ManagementPeriods.Add(managementPeriod2);
            var dairyComponent = new DairyComponent();
            dairyComponent.Groups.Add(dairyGroup);

            var swineGroup = new AnimalGroup()
            {
                GroupType = AnimalType.SwineBoar,
            };
            swineGroup.ManagementPeriods.Add(managementPeriod3);
            var swineComponent = new FarrowToWeanComponent();
            swineComponent.Groups.Add(swineGroup);

            farm.Components.Add(beefComponent);
            farm.Components.Add(dairyComponent);
            farm.Components.Add(swineComponent);

            _initializationService.InitializeStartAndEndWeightsForCattle(farm);

            Assert.AreEqual(beefGroup.ManagementPeriods.ElementAt(0).StartWeight, 310, 0.01);
            Assert.AreEqual(beefGroup.ManagementPeriods.ElementAt(0).EndWeight, 610, 0.01);

            Assert.AreEqual(dairyGroup.ManagementPeriods.ElementAt(0).StartWeight, 45, 0.01);
            Assert.AreEqual(dairyGroup.ManagementPeriods.ElementAt(0).EndWeight, 127, 0.01);

            Assert.AreEqual(swineGroup.ManagementPeriods.ElementAt(0).StartWeight, 0, 0.01);
            Assert.AreEqual(swineGroup.ManagementPeriods.ElementAt(0).EndWeight, 0, 0.01);
        }

        [TestMethod]
        public void InitializeLivestockCoefficientSheep()
        {
            var farm = new Farm();
            var ram = new AnimalGroup()
            {
                GroupType = AnimalType.Ram,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        AnimalType =AnimalType.Ram,
                        GainCoefficientA = 0,
                        GainCoefficientB = 0,
                        WoolProduction = 0,
                        StartWeight = 0,
                        EndWeight = 0,
                        HousingDetails = new HousingDetails{ BaselineMaintenanceCoefficient = 0,},
                    }
                },
            };
            var ramComponent = new RamsComponent();
            ramComponent.Groups.Add(ram);

            farm.Components.Add(ramComponent);

            _initializationService.InitializeLivestockCoefficientSheep(farm);
            Assert.AreEqual(4, ram.ManagementPeriods.First().WoolProduction);
            Assert.AreEqual(125, ram.ManagementPeriods.First().StartWeight);
            Assert.AreEqual(125, ram.ManagementPeriods.First().EndWeight);
            Assert.AreEqual(2.5, ram.ManagementPeriods.First().GainCoefficientA);
            Assert.AreEqual(0.35, ram.ManagementPeriods.First().GainCoefficientB);
            Assert.AreEqual(0.25, ram.ManagementPeriods.First().HousingDetails.BaselineMaintenanceCoefficient);
        }

        [TestMethod]
        public void InitializeLivestockCoefficientSheepFeedLot()
        {
            var farm = new Farm();
            var sheepFeedLot = new AnimalGroup()
            {
                GroupType = AnimalType.SheepFeedlot,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        AnimalType =AnimalType.SheepFeedlot,
                        GainCoefficientA = 0,
                        GainCoefficientB = 0,
                        WoolProduction = 0,
                        StartWeight = 0,
                        EndWeight = 0,
                        HousingDetails = new HousingDetails{ BaselineMaintenanceCoefficient = 0,},
                    }
                },
            };
            var ramComponent = new RamsComponent();
            ramComponent.Groups.Add(sheepFeedLot);

            farm.Components.Add(ramComponent);

            _initializationService.InitializeLivestockCoefficientSheep(farm);
            Assert.AreEqual(4, sheepFeedLot.ManagementPeriods.First().WoolProduction);
        }

        [TestMethod]
        public void InitializeLivestockCoefficientMultipleComponents()
        {
            var farm = new Farm();
            var ewe = new AnimalGroup()
            {
                GroupType = AnimalType.Ewes,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        AnimalType =AnimalType.Ewes,
                        GainCoefficientA = 0,
                        GainCoefficientB = 0,
                        WoolProduction = 0,
                        StartWeight = 0,
                        EndWeight = 0,
                        HousingDetails = new HousingDetails{ BaselineMaintenanceCoefficient = 0,},
                    }
                },
            };
            var weanedLamb = new AnimalGroup()
            {
                GroupType = AnimalType.WeanedLamb,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        AnimalType =AnimalType.WeanedLamb,
                        GainCoefficientA = 0,
                        GainCoefficientB = 0,
                        WoolProduction = 0,
                        StartWeight = 0,
                        EndWeight = 0,
                        HousingDetails = new HousingDetails{ BaselineMaintenanceCoefficient = 0,},
                    }
                },
            };
            var eweAndLambsComponent = new EwesAndLambsComponent();
            eweAndLambsComponent.Groups.Add(ewe);
            eweAndLambsComponent.Groups.Add(weanedLamb);

            farm.Components.Add(eweAndLambsComponent);

            _initializationService.InitializeLivestockCoefficientSheep(farm);
            Assert.AreEqual(0.45, ewe.ManagementPeriods.First().GainCoefficientB);
            Assert.AreEqual(0.385, weanedLamb.ManagementPeriods.First().GainCoefficientB);
        }

        [TestMethod]
        public void InitializeLivestockCoefficientNullArguments()
        {
            Farm farm = new Farm();
            try
            {
                _initializationService.InitializeLivestockCoefficientSheep(farm); //passing null farm
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod]
        public void InitializeSwineVsExcretionForDietsDefaultValue()
        {
            _farm1.DefaultSoilData.Province = Province.Alberta;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.AnimalType = AnimalType.SwineDrySow;
            
            var animalGroup = new AnimalGroup();
            animalGroup.ManagementPeriods.Add(managementPeriod);
            animalGroup.GroupType = AnimalType.SwineDrySow;

            var drySowsComponent = new DrySowsComponent();
            drySowsComponent.Groups.Add(animalGroup);

            _farm1.Components.Add(drySowsComponent);

            _initializationService.InitializeVolatileSolidsExcretion(_farm1);
            Assert.AreEqual(expected: 0.1228, actual: managementPeriod.ManureDetails.VolatileSolidExcretion);
        }

        [TestMethod]
        public void InitializeSwineVsExcretionForDietsDefaultValuesNullArgument()
        {
            Farm farm = new Farm();
            try
            {
                _initializationService.InitializeVolatileSolidsExcretion(farm);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod]
        public void InitializeSwineVsExcretionForDietsDefaultValueMultipleSwineTypes()
        {
            _farm1.DefaultSoilData.Province = Province.Alberta;

            var managementPeriodOne = new ManagementPeriod();
            managementPeriodOne.AnimalType = AnimalType.SwineDrySow;
            var animalGroupOne = new AnimalGroup();
            animalGroupOne.GroupType = AnimalType.SwineDrySow;
            animalGroupOne.ManagementPeriods.Add(managementPeriodOne);
            var drySowsComponent = new DrySowsComponent();
            drySowsComponent.Groups.Add(animalGroupOne);

            var managementPeriodTwo = new ManagementPeriod();
            managementPeriodTwo.AnimalType = AnimalType.SwineFinisher;
            var animalGroupTwo = new AnimalGroup();
            animalGroupTwo.GroupType = AnimalType.SwineFinisher;
            animalGroupTwo.ManagementPeriods.Add(managementPeriodTwo);
            var swineFinisherComponent = new SwineFinishersComponent();
            swineFinisherComponent.Groups.Add(animalGroupTwo);

            var managementPeriodThree = new ManagementPeriod();
            managementPeriodThree.AnimalType = AnimalType.SwineStarter;
            var animalGroupThree = new AnimalGroup();
            animalGroupThree.GroupType = AnimalType.SwineStarter;
            animalGroupThree.ManagementPeriods.Add(managementPeriodThree);
            var swineStarterComponent = new SwineFinishersComponent();
            swineStarterComponent.Groups.Add(animalGroupThree);

            _farm1.Components.Add(drySowsComponent);
            _farm1.Components.Add(swineFinisherComponent);
            _farm1.Components.Add(swineStarterComponent);

            _initializationService.InitializeVolatileSolidsExcretion(_farm1);
            Assert.AreEqual(expected: 0.1228, actual: managementPeriodOne.ManureDetails.VolatileSolidExcretion);
            Assert.AreEqual(expected: 0.1389, actual: managementPeriodTwo.ManureDetails.VolatileSolidExcretion);
            Assert.AreEqual(expected: 0.1504, actual: managementPeriodThree.ManureDetails.VolatileSolidExcretion);
        }

        [TestMethod]
        public void InitializeSwineVsExcretionForDietNullManagementPeriod()
        {
            var province = Province.Alberta;
            var managementPeriod = new ManagementPeriod();
            try
            {
                _initializationService.InitializeVolatileSolidsExcretion(managementPeriod, province);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void InitializeOtherLivestockCh4EmissionFactor()
        {
            var farm = new Farm();
            var deer = new AnimalGroup()
            {
                GroupType = AnimalType.Deer,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        AnimalType = AnimalType.Deer,
                        ManureDetails = new ManureDetails
                        {
                             
                            DailyManureMethaneEmissionRate = 0.0,
                        }
                    }
                },
            };
            var deerComponent = new DeerComponent();
            deerComponent.Groups.Add(deer);

            farm.Components.Add(deerComponent);

            _initializationService.InitializeDailyManureMethaneEmissionRate(farm);
            Assert.AreEqual(0.000603, deer.ManagementPeriods.First().ManureDetails.DailyManureMethaneEmissionRate);
        }

        [TestMethod]
        public void InitializeIrrigationWaterApplicationTwoArguments()
        {
            _farm1.DefaultSoilData.Province = Province.Alberta;
            var climateData = new ClimateData();
            var cropViewItemOne = new CropViewItem()
            {
                Year = 2022,
            };
            var cropViewItemTwo = new CropViewItem()
            {
                Year = 2002
            };

            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();
            cropViewItemCollection.Add(cropViewItemOne);
            cropViewItemCollection.Add(cropViewItemTwo);
            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);

            climateData.PrecipitationData.April = 1;
            climateData.PrecipitationData.May = 1;
            climateData.PrecipitationData.June = 1;
            climateData.PrecipitationData.July = 1;
            climateData.PrecipitationData.August = 1;
            climateData.PrecipitationData.September = 1;
            climateData.PrecipitationData.October = 1;
            climateData.EvapotranspirationData.April = 2;
            climateData.EvapotranspirationData.May = 2;
            climateData.EvapotranspirationData.June = 2;
            climateData.EvapotranspirationData.July = 2;
            climateData.EvapotranspirationData.August = 2;
            climateData.EvapotranspirationData.September = 2;
            climateData.EvapotranspirationData.October = 2;

            _farm1.ClimateData = climateData;

            _initializationService.InitializeIrrigationWaterApplication(_farm1);

            Assert.AreEqual(expected: 7, actual: cropViewItemOne.AmountOfIrrigation);
            Assert.AreEqual(expected: 96.93, cropViewItemOne.GrowingSeasonIrrigation, 0.01);
            Assert.AreEqual(expected: 7, actual: cropViewItemTwo.AmountOfIrrigation);
            Assert.AreEqual(expected: 96.93, cropViewItemTwo.GrowingSeasonIrrigation, 0.01);
        }

        [TestMethod]
        public void InitializeIrrigationWaterApplicationSingleArgument()
        {
            _farm1.DefaultSoilData.Province = Province.Ontario;

            var cropViewItem = new CropViewItem()
            {
                Year = 2022
            };
            
            var climateData = new ClimateData();
            climateData.PrecipitationData.April = 1;
            climateData.PrecipitationData.May = 1;
            climateData.PrecipitationData.June = 1;
            climateData.PrecipitationData.July = 1;
            climateData.PrecipitationData.August = 1;
            climateData.PrecipitationData.September = 1;
            climateData.PrecipitationData.October = 1;
            climateData.EvapotranspirationData.April = 2;
            climateData.EvapotranspirationData.May = 2;
            climateData.EvapotranspirationData.June = 2;
            climateData.EvapotranspirationData.July = 2;
            climateData.EvapotranspirationData.August = 2;
            climateData.EvapotranspirationData.September = 2;
            climateData.EvapotranspirationData.October = 2;

            _farm1.ClimateData = climateData;

            _initializationService.InitializeIrrigationWaterApplication(_farm1, cropViewItem);

            Assert.AreEqual(expected: 7, actual: cropViewItem.AmountOfIrrigation);
            Assert.AreEqual(expected: 96.18, cropViewItem.GrowingSeasonIrrigation, 0.01);
        }

        [TestMethod]
        public void InitializeIrrigationWaterApplicationLessTranspiration()
        {
            _farm1.DefaultSoilData.Province = Province.Ontario;
            var cropViewItem = new CropViewItem()
            {
                Year = 2022
            };
            var climateData = new ClimateData();
            climateData.PrecipitationData.April = 1;
            _farm1.ClimateData = climateData;
            _initializationService.InitializeIrrigationWaterApplication(_farm1, cropViewItem);

            Assert.AreEqual(expected: 0, actual: cropViewItem.AmountOfIrrigation);
        }
        [TestMethod]
        public void InitializeIrrigationWaterApplicationTwoArgumentsNullCropViewItem()
        {
            _farm1.Province = Province.Alberta;
            var cropViewItem = new CropViewItem();
            try
            {
                _initializationService.InitializeIrrigationWaterApplication(_farm1, cropViewItem);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void InitializeIrrigationWaterApplicationNullFarm()
        {
            var cropViewItem = new CropViewItem()
            {
                Year = 2022
            };
            var farm = new Farm(); try
            {
                _initializationService.InitializeIrrigationWaterApplication(_farm1, cropViewItem);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    
        [TestMethod]
        public void InitializeOtherLivestockCh4EmissionFactorNullArguments()
        {
            Farm farm = new Farm();
            try
            {
                _initializationService.InitializeDailyManureMethaneEmissionRate(farm);// passing farm with null dairyComponent
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void InitializeDefaultMoistureContent()
        {
            var stageState = new FieldSystemDetailsStageState()
            {
                DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem() { HarvestMethod = HarvestMethods.Silage},
                    new CropViewItem() { HarvestMethod = HarvestMethods.GreenManure},
                    new CropViewItem() { HarvestMethod = HarvestMethods.Swathing},
                    new CropViewItem() { CropType = CropType.SilageCorn},
                }
            };
            Farm farm = new Farm();
            farm.StageStates.Add(stageState);
            _initializationService.InitializeMoistureContent(farm);
            foreach (var item in farm.StageStates.OfType<FieldSystemDetailsStageState>().First().DetailsScreenViewCropViewItems)
            {
                Assert.AreEqual(65, item.MoistureContentOfCropPercentage);
            }
        }

        [TestMethod]
        public void InitializeDefaultMoistureContentNullArguments()
        {
            Farm farm = new Farm();
            try
            {
                _initializationService.InitializeMoistureContent(farm);// passing farm with null dairyComponent
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod]
        public void InitializePercentageReturnsSetsDefaultsForPerennials()
        {
            var viewItem = new CropViewItem() { CropType = CropType.TameGrass };

            _initializationService.InitializePercentageReturns(new Farm(), viewItem);

            Assert.AreEqual(35, viewItem.PercentageOfProductYieldReturnedToSoil);
            Assert.AreEqual(0, viewItem.PercentageOfStrawReturnedToSoil);
            Assert.AreEqual(100, viewItem.PercentageOfRootsReturnedToSoil);
        }

        [TestMethod]
        public void InitializePercentageReturnsSetsDefaultsForSilageCrop()
        {
            var viewItem = new CropViewItem() { CropType = CropType.SilageCorn };

            _initializationService.InitializePercentageReturns(new Farm(), viewItem);

            Assert.AreEqual(2, viewItem.PercentageOfProductYieldReturnedToSoil);
            Assert.AreEqual(0, viewItem.PercentageOfStrawReturnedToSoil);
            Assert.AreEqual(100, viewItem.PercentageOfRootsReturnedToSoil);
        }

        [TestMethod]
        public void InitializePercentageReturnsSetsDefaultsForRootCrop()
        {
            var viewItem = new CropViewItem() { CropType = CropType.Potatoes };

            _initializationService.InitializePercentageReturns(new Farm(), viewItem);

            Assert.AreEqual(0, viewItem.PercentageOfProductYieldReturnedToSoil);
            Assert.AreEqual(100, viewItem.PercentageOfStrawReturnedToSoil);
        }

        [TestMethod]
        public void InitializePercentageReturnsSetsDefaultsForCoverCrop()
        {
            var viewItem = new CropViewItem() { CropType = CropType.PigeonBean };

            _initializationService.InitializePercentageReturns(new Farm(), viewItem);

            Assert.AreEqual(100, viewItem.PercentageOfProductYieldReturnedToSoil);
            Assert.AreEqual(100, viewItem.PercentageOfStrawReturnedToSoil);
            Assert.AreEqual(100, viewItem.PercentageOfRootsReturnedToSoil);
        }

        [TestMethod]
        public void InitializePercentageReturnsSetsDefaultsForSilageHarvestMethod()
        {
            var viewItem = new CropViewItem() { CropType = CropType.PigeonBean, HarvestMethod = HarvestMethods.Silage};

            _initializationService.InitializePercentageReturns(new Farm(), viewItem);

            Assert.AreEqual(2, viewItem.PercentageOfProductYieldReturnedToSoil);
            Assert.AreEqual(0, viewItem.PercentageOfStrawReturnedToSoil);
            Assert.AreEqual(100, viewItem.PercentageOfRootsReturnedToSoil);
        }

        [TestMethod]
        public void InitializePercentageReturnsSetsDefaultsForSwathingHarvestMethod()
        {
            var viewItem = new CropViewItem() { CropType = CropType.PigeonBean, HarvestMethod = HarvestMethods.Swathing };

            _initializationService.InitializePercentageReturns(new Farm(), viewItem);

            Assert.AreEqual(30, viewItem.PercentageOfProductYieldReturnedToSoil);
            Assert.AreEqual(0, viewItem.PercentageOfStrawReturnedToSoil);
            Assert.AreEqual(100, viewItem.PercentageOfRootsReturnedToSoil);
        }

        [TestMethod]
        public void InitializePercentageReturnsSetsDefaultsForGreenManureHarvestMethod()
        {
            var viewItem = new CropViewItem() { CropType = CropType.PigeonBean, HarvestMethod = HarvestMethods.GreenManure };

            _initializationService.InitializePercentageReturns(new Farm(), viewItem);

            Assert.AreEqual(100, viewItem.PercentageOfProductYieldReturnedToSoil);
            Assert.AreEqual(0, viewItem.PercentageOfStrawReturnedToSoil);
            Assert.AreEqual(100, viewItem.PercentageOfRootsReturnedToSoil);
        }

        [TestMethod]
        public void InitializeCarbonConcentrationSingleArgument()
        {
            var cropViewItem = new CropViewItem();
            cropViewItem.CarbonConcentration = 0;
            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();
            cropViewItemCollection.Add(cropViewItem);
            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);

            _initializationService.InitializeCarbonConcentration(_farm1);

            Assert.AreEqual(expected: 0.45, actual: cropViewItem.CarbonConcentration);
        }

        [TestMethod]
        public void InitializeCarbonConcentrationTwoArgument()
        {
            var cropViewItem = new CropViewItem();
            var defaults = new Defaults();
            cropViewItem.CarbonConcentration = 0;

            _initializationService.InitializeCarbonConcentration(cropViewItem, defaults);

            Assert.AreEqual(expected: 0.45, actual: cropViewItem.CarbonConcentration); 
        }

        [TestMethod]
        public void InitializeNitrogenFixationNonLegumousCrop()
        {
            var viewItem = new CropViewItem();
            viewItem.NitrogenFixationPercentage = 15;
            _initializationService.InitializeNitrogenFixation(viewItem);
            Assert.AreEqual(expected: 0, actual: viewItem.NitrogenFixationPercentage);
        }

        [TestMethod]
        public void InitializeNitrogenFixationLegumousCrop()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.PulseCrops;
            _initializationService.InitializeNitrogenFixation(viewItem);
            Assert.AreEqual(expected: 70, actual: viewItem.NitrogenFixationPercentage);
        }

        [TestMethod]
        public void InitializeNitrogenFixationFarmArgumentTwoCropViewItems()
        {
            var cropViewItemOne = new CropViewItem();
            var cropViewItemTwo = new CropViewItem();
            cropViewItemOne.CropType = CropType.AlfalfaMedicagoSativaL;
            cropViewItemTwo.CropType = CropType.PulseCrops;
            var cropViewItemCollection = new ObservableCollection<CropViewItem>();
            var fieldSystemDetailsStageState = new FieldSystemDetailsStageState();
            cropViewItemCollection.Add(cropViewItemOne);
            cropViewItemCollection.Add(cropViewItemTwo);
            fieldSystemDetailsStageState.DetailsScreenViewCropViewItems = cropViewItemCollection;
            _farm1.StageStates.Add(fieldSystemDetailsStageState);
            _initializationService.InitializeNitrogenFixation(_farm1);
            Assert.AreEqual(expected: 0, actual: cropViewItemOne.NitrogenFixationPercentage);
            Assert.AreEqual(expected: 70, actual: cropViewItemTwo.NitrogenFixationPercentage);
        }

        [TestMethod]
        public void InitializeNitrogenFixationNullFarm()
        {
            var farm = new Farm();
            try
            {
                _initializationService.InitializeNitrogenFixation(farm);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }


        [TestMethod]
        public void InitializeDefaultMoistureContentWithNullResidueData()
        {
            var cropViewItem = new CropViewItem()
            {
                MoistureContentOfCropPercentage = 0,
            };
            _initializationService.InitializeMoistureContent(null, cropViewItem);
            Assert.AreEqual(12, cropViewItem.MoistureContentOfCropPercentage);
        }
        [TestMethod]
        public void InitializeDefaultMoistureContentWithEmptyResidueData()
        {
            var cropViewItem = new CropViewItem()
            {
                MoistureContentOfCropPercentage = 0,
            };
            _initializationService.InitializeMoistureContent(new Table_7_Relative_Biomass_Information_Data(), cropViewItem);
            Assert.AreEqual(12, cropViewItem.MoistureContentOfCropPercentage);
        }

        [TestMethod]
        public void InitializeDefaultMoistureContentWithResidueData()
        {
            Table_7_Relative_Biomass_Information_Data residueData = new Table_7_Relative_Biomass_Information_Data();
            residueData.MoistureContentOfProduct = 40;
            var cropViewItem = new CropViewItem()
            {
                MoistureContentOfCropPercentage = 0,
            };
            _initializationService.InitializeMoistureContent(residueData, cropViewItem);
            Assert.AreEqual(40, cropViewItem.MoistureContentOfCropPercentage);
        }

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

            _initializationService.InitializeYieldForYear(farm, detailsScreenViewItem, fieldSystemComponent);

            Assert.AreEqual(150, detailsScreenViewItem.Yield);
        }

        [TestMethod]
        public void InitializeTillageTypeForProvinceWithPerennialSelected()
        {
            var viewItem = new CropViewItem() { CropType = CropType.TameGrass };

            _initializationService.InitializeTillageType(viewItem, _farm1);

            Assert.AreEqual(TillageType.NoTill, viewItem.TillageType);
        }

        [TestMethod]
        public void InitializeTillageTypeForProvinceWithAnnualSelected()
        {
            _farm1.DefaultSoilData.Province = Province.BritishColumbia;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.Black;
            var viewItem = new CropViewItem() { CropType = CropType.Barley };

            _initializationService.InitializeTillageType(viewItem, _farm1);

            Assert.AreEqual(TillageType.Reduced, viewItem.TillageType);
        }

        [TestMethod]
        public void InitializeFallowSetTillageType()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.Fallow;
            viewItem.TillageType = TillageType.Reduced;

            _initializationService.InitializeFallow(viewItem, new Farm());

            Assert.AreEqual(TillageType.NoTill, viewItem.TillageType);
        }

        [TestMethod]
        public void InitializeFallowDoesNotSetTillageTypeWhenCropIsNotFallow()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.Barley;
            viewItem.TillageType = TillageType.Reduced;

            _initializationService.InitializeFallow(viewItem, new Farm());

            Assert.AreEqual(TillageType.Reduced, viewItem.TillageType);
        }

        [TestMethod]
        public void InitializeHarvestMethodSetsCashCrop()
        {
            var viewItem = new CropViewItem();

            _initializationService.InitializeHarvestMethod(viewItem);

            Assert.AreEqual(HarvestMethods.CashCrop, viewItem.HarvestMethod);
        }

        [TestMethod]
        public void InitializeHarvestMethodSetsSilage()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.SilageCorn;

            _initializationService.InitializeHarvestMethod(viewItem);

            Assert.AreEqual(HarvestMethods.Silage, viewItem.HarvestMethod);
        }

        [TestMethod]
        public void InitializeLigninContentForSilage()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.SilageCorn;
            _farm1.DefaultSoilData.Province = Province.BritishColumbia;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.Black;

            _initializationService.InitializeLigninContent(viewItem, _farm1);

            Assert.AreEqual(0.11, viewItem.LigninContent);
        }

        [TestMethod]
        public void InitializeLigninContentForAnnual()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.Barley;
            _farm1.DefaultSoilData.Province = Province.BritishColumbia;
            _farm1.DefaultSoilData.SoilFunctionalCategory = SoilFunctionalCategory.Black;

            _initializationService.InitializeLigninContent(viewItem, _farm1);

            Assert.AreEqual(0.046, viewItem.LigninContent);
        }

        [TestMethod]
        public void InitializePerennialsForTameGrass()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.TameGrass;

            _initializationService.InitializePerennialDefaults(viewItem, new Farm());

            Assert.AreEqual(60, viewItem.ForageUtilizationRate);
        }

        [TestMethod]
        public void InitializePerennialsForRangeland()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.RangelandNative;

            _initializationService.InitializePerennialDefaults(viewItem, new Farm());

            Assert.AreEqual(40, viewItem.ForageUtilizationRate);
        }

        [TestMethod]
        public void InitializeSoilProperties()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.Barley;
            _farm1.DefaultSoilData.ProportionOfSandInSoil = 50;

            _initializationService.InitializeSoilProperties(viewItem, _farm1);

            Assert.AreEqual(50, viewItem.Sand);
        }

        [TestMethod]
        public void InitializeUserDefaultsSetsYield()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.Wheat;
            var globalSettings = new GlobalSettings();

            var defaultCrop = new CropViewItem();
            defaultCrop.EnableCustomUserDefaultsForThisCrop = true;
            defaultCrop.CropType = CropType.Wheat;
            defaultCrop.Yield = 555;
            globalSettings.CropDefaults.Add(defaultCrop);

            _initializationService.InitializeUserDefaults(viewItem, globalSettings);

            Assert.AreEqual(555, viewItem.Yield);
        }

        [TestMethod]
        public void InitializeUserDefaultsDoesNotSetYield()
        {
            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.Wheat;
            viewItem.Yield = 100;
            var globalSettings = new GlobalSettings();

            var defaultCrop = new CropViewItem();
            defaultCrop.EnableCustomUserDefaultsForThisCrop = false;
            defaultCrop.CropType = CropType.Wheat;
            defaultCrop.Yield = 555;
            globalSettings.CropDefaults.Add(defaultCrop);

            _initializationService.InitializeUserDefaults(viewItem, globalSettings);

            Assert.AreEqual(100, viewItem.Yield);
        }

        #endregion
    }
}
