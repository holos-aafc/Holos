using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.Animals.Sheep;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Climate;
using H.Core.Providers.Feed;
using H.Core.Providers.Soil;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace H.Core.Test.Services
{
    [TestClass]
    public class FarmResultsServiceTest : UnitTestBase
    {
        #region Fields

        private FarmResultsService _farmResultsService;

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
            _farmResultsService = new FarmResultsService(new EventAggregator(), _fieldResultsService, new ADCalculator(), new Mock<IManureService>().Object, new Mock<IAnimalService>().Object, _n2OEmissionFactorCalculator);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void TestReplicateFarm()
        {
            #region Init Farm
            var geographicDataProvider = new GeographicDataProvider();
            geographicDataProvider.Initialize();

            var farmToReplicate = new Farm()
            {
                Name = "Farm 1",
                StageStates = new List<StageStateBase>()
                {
                    new FieldSystemDetailsStageState()
                    {
                        DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>()
                        {
                            new CropViewItem()
                            {
                                Year = 1985,
                                CropType = CropType.Barley,
                                CropEconomicData =
                                {
                                    ExpectedMarketPrice = 0.88,
                                }
                            }
                        }
                    }
                },
                Longitude = -112,
                Latitude = 49,
                ClimateData = _climateProvider.Get(49, -112, TimeFrame.NineteenNinetyToTwoThousand),
                GeographicData = geographicDataProvider.GetGeographicalData(793011),
                PolygonId = 793011,
            };
            farmToReplicate.GeographicData.CustomYieldData = new List<CustomUserYieldData> { new CustomUserYieldData() };
            farmToReplicate.Components.Add(new SheepComponent());
            farmToReplicate.Components.Add(new FieldSystemComponent()
            {
                CropViewItems = new ObservableCollection<CropViewItem>()
                {
                    new CropViewItem()
                    {
                        Year = 1985,
                        CropType = CropType.Barley,
                        CropEconomicData =
                        {
                            ExpectedMarketPrice = 0.88,
                        }
                    }
                }
                
            });
            #endregion

            // if farmToReplicate.ClimateData is null, there was an exception thrown (i.e. 502 Gateway Error) or etc. during the NASA Api Call 
            // return early to prevent unhandled exceptions or errors that lead to a failed test
            if (farmToReplicate.ClimateData == null)
            {
                return;
            } 

            var result = _farmResultsService.ReplicateFarm(farmToReplicate);

            //GUID
            Assert.AreNotEqual(result.Guid, farmToReplicate.Guid);

            //Defaults
            farmToReplicate.Defaults.CarbonConcentration = 55;
            Assert.AreNotEqual(result.Defaults.CarbonConcentration, 55, "Assert that a copy of the defaults are made (they don't reference same object");

            //Stage States
            Assert.AreEqual(result.StageStates.OfType<FieldSystemDetailsStageState>().Single().DetailsScreenViewCropViewItems.Count, 1, "Assert that stage state was copied");

            //climate data
            Assert.AreEqual(result.ClimateData.TemperatureData.GetMeanAnnualTemperature(), farmToReplicate.ClimateData.TemperatureData.GetMeanAnnualTemperature());
            farmToReplicate.ClimateData = _climateProvider.Get(50, -105, TimeFrame.NineteenNinetyToTwoThousand); 

            if (farmToReplicate.ClimateData == null)
            {
                return;
            }

            Assert.AreNotEqual(result.ClimateData.TemperatureData.GetMeanAnnualTemperature(), farmToReplicate.ClimateData.TemperatureData.GetMeanAnnualTemperature(), "Assert that climate data was copied");

            //DailyClimateData
            Assert.AreNotEqual(result.ClimateData.DailyClimateData[0].MeanDailyAirTemperature, farmToReplicate.ClimateData.DailyClimateData[0].MeanDailyAirTemperature);

            //latitude & Longitude
            Assert.AreEqual(result.Latitude, farmToReplicate.Latitude);
            farmToReplicate.Latitude = 11;
            farmToReplicate.Longitude = 11;
            Assert.AreNotEqual(result.Latitude, farmToReplicate.Latitude);
            Assert.AreNotEqual(result.Longitude, farmToReplicate.Longitude);

            //DefaultSoilData
            Assert.AreEqual(result.GeographicData.DefaultSoilData.BulkDensity, farmToReplicate.GeographicData.DefaultSoilData.BulkDensity);
            farmToReplicate.GeographicData.DefaultSoilData.BulkDensity = 22;
            Assert.AreNotEqual(result.GeographicData.DefaultSoilData.BulkDensity, farmToReplicate.GeographicData.DefaultSoilData.BulkDensity);

            //CustomUserYieldData
            Assert.AreEqual(result.GeographicData.CustomYieldData[0].Yield, farmToReplicate.GeographicData.CustomYieldData[0].Yield);
            farmToReplicate.GeographicData.CustomYieldData[0].Yield = 2323;
            Assert.AreNotEqual(result.GeographicData.CustomYieldData[0].Yield, farmToReplicate.GeographicData.CustomYieldData[0].Yield);

            //Components
            Assert.AreEqual(result.Components.Count, farmToReplicate.Components.Count);
            Assert.AreEqual(result.Components[0].ComponentCategory, farmToReplicate.Components[0].ComponentCategory);
            farmToReplicate.Components.Add(new CowCalfComponent());
            Assert.AreNotEqual(result.Components.Count, farmToReplicate.Components.Count);

            //economics
            //check the economics copied over correctly
            Assert.AreEqual(
                farmToReplicate.FieldSystemComponents.ElementAt(0).CropViewItems[0].CropEconomicData
                    .ExpectedMarketPrice,
                result.FieldSystemComponents.ElementAt(0).CropViewItems[0].CropEconomicData.ExpectedMarketPrice);
            result.FieldSystemComponents.ElementAt(0).CropViewItems[0].CropEconomicData.ExpectedMarketPrice = 99;
            //true copy won't affect the original
            Assert.AreNotEqual(
                farmToReplicate.FieldSystemComponents.ElementAt(0).CropViewItems[0].CropEconomicData
                    .ExpectedMarketPrice,
                result.FieldSystemComponents.ElementAt(0).CropViewItems[0].CropEconomicData.ExpectedMarketPrice);
        }

        [TestMethod]
        public void TestCopiedAnimalGroupsInReplicatedFarm()
        {
            Farm farm = new Farm()
            {
                Components = new ObservableCollection<ComponentBase>()
                {
                    new DairyComponent()
                },
                ClimateData = new ClimateData(),
                GeographicData = new GeographicData(),
            };
            var farmDairyComponent = (DairyComponent)farm.Components[0];
            var farmGroups = farmDairyComponent.Groups;
            farmGroups.Add(new AnimalGroup()
            {
                GroupType = AnimalType.Dairy,
            });

            var replicateFarm = _farmResultsService.ReplicateFarm(farm);

            var replicateComponent = (DairyComponent)replicateFarm.Components[0];
            var replicateGroups = replicateComponent.Groups;
            Assert.AreEqual(farmGroups.Count, replicateGroups.Count);
        }

        [TestMethod]
        public void TestCalculateAdResults()
        {
            var farm = new Farm();
            var animalGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCow,
                Name = "Test Beef Cows",
            };

            var managementPeriod = new ManagementPeriod()
            {
                AnimalGroupGuid = animalGroup.Guid,
                Name = "Test Management Period #1",
            };

            animalGroup.ManagementPeriods.Add(managementPeriod);

            var animalComponent = new CowCalfComponent();
            animalComponent.Groups.Add(animalGroup);
            farm.Components.Add(animalComponent);

            var componentToReplicate = new AnaerobicDigestionComponent()
            {
                ManagementPeriodViewItems = new ObservableCollection<ADManagementPeriodViewItem>()
                {
                    new ADManagementPeriodViewItem()
                    {
                        ManagementPeriod = managementPeriod,
                        AnimalComponent = animalComponent,
                        AnimalGroup = animalGroup,
                        DailyPercentageOfManureAdded = 11,
                        FlowRate = 11,
                    },
                },
                AnaerobicDigestionViewItem = new AnaerobicDigestionViewItem()
                {
                    ManureSubstrateViewItems = new ObservableCollection<ManureSubstrateViewItem>()
                    {
                        new ManureSubstrateViewItem()
                        {
                            FlowRate = 11,
                            ManureStateType = ManureStateType.SolidStorage,
                        }
                    },
                    CropResiduesSubstrateViewItems = new ObservableCollection<CropResidueSubstrateViewItem>()
                    {
                        new CropResidueSubstrateViewItem()
                        {
                            CropType = CropType.Barley,
                            FlowRate = 11,
                        }
                    },
                    FarmResiduesSubstrateViewItems = new ObservableCollection<FarmResiduesSubstrateViewItem>()
                    {
                        new FarmResiduesSubstrateViewItem()
                        {
                            FlowRate = 11,
                            FarmResidueType = FarmResidueType.FoodWaste,
                        }
                    },
                }
            };

            farm.Components.Add(componentToReplicate);

            var replicateFarm = _farmResultsService.ReplicateFarm(farm);

            // Check that replicated animal components have been copied in to new AD component correctly and old animal components were not modified
            Assert.IsTrue(farm.AnimalComponents.Contains(animalComponent));

            // Check that animal component in the AD Component is the same as the animal component in the farm
            Assert.IsTrue(replicateFarm.AnimalComponents.Contains(replicateFarm.GetAnaerobicDigestionComponent().ManagementPeriodViewItems[0].AnimalComponent));

            // Ensure that the animal component in the original AD component has different GUID to the animal component in the replicated AD component
            Assert.AreNotEqual(farm.GetAnaerobicDigestionComponent().ManagementPeriodViewItems[0].AnimalComponent.Guid, replicateFarm.GetAnaerobicDigestionComponent().ManagementPeriodViewItems[0].AnimalComponent.Guid);
        }

        #endregion
    }
}
