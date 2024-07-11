using H.Core.Emissions.Results;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals;
using H.Core.Services;
using System.Linq;
using H.Core.Models.Animals.Dairy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using System.Collections.ObjectModel;

namespace H.Core.Test.Services
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
                ManureDetails = new ManureDetails()
                {
                    FractionOfOrganicNitrogenNitrified = 10,
                }
            };

            _initializationService.InitializeManureMineralizationFractions(managementPeriod, new FractionOfOrganicNitrogenMineralizedData() {FractionNitrified = 2});

            Assert.AreEqual(expected: 2, actual: managementPeriod.ManureDetails.FractionOfOrganicNitrogenNitrified);
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
            var dairycow2 = new AnimalGroup()
            {
                GroupType = AnimalType.DairyLactatingCow,
                ManagementPeriods =
                {
                    new ManagementPeriod
                    {
                        Start = new DateTime(1989,  4, 25),//invalid year
                        MilkProduction = 200,

                    }
                },
            };
            var dairyComponent2 = new DairyComponent();
            dairyComponent2.Groups.Add(dairycow2);
            _farm2.Components.Add(dairyComponent2);
            _farm2.DairyComponents.Cast<DairyComponent>().First().Groups.Add(dairycow2);
            _farm2.DefaultSoilData.Province = Province.BritishColumbia;

            Assert.ThrowsException<NullReferenceException>(() => _initializationService.InitializeMilkProduction(_farm2));
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
            Assert.AreEqual(1000, beef.ManagementPeriods.First().MilkProduction);
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

            _initializationService.ReinitializeBeddingMaterial(_farm1);

            Assert.AreEqual(0.447, housingDetails.TotalCarbonKilogramsDryMatterForBedding);
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

            _initializationService.InitializeMethaneProducingCapacity(managementPeriod);

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

            _initializationService.InitializeCattleFeedingActivity(farm);
            Assert.AreEqual(0.17, beef.ManagementPeriods.First().HousingDetails.ActivityCeofficientOfFeedingSituation);
        }

        [TestMethod]
        public void InitializeCattleFeedingActivityCoefficientNullArguments()
        {
            Farm farm = new Farm();
            try
            {
                _initializationService.InitializeCattleFeedingActivity(farm); //passing null farm
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
                _initializationService.InitializeCattleFeedingActivity(farm);// passing farm with null dairyComponent
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}
