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

            _initializationService.InitializeDefaultEmissionFactors(_farm1, new CowCalfComponent(), managementPeriod);

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
                _initializationService.InitializeMilkProduction(farm);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            var dairyComponent = new DairyComponent();
            farm.Components.Add(dairyComponent);
            try
            {
                _initializationService.InitializeMilkProduction(farm);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
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

        #endregion
    }
}
