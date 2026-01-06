using H.Core.Enumerations;
using H.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Linq;
using H.Core.Models.Infrastructure;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.Animals.Swine;

namespace H.Core.Test.Services
{
    [TestClass]
    public class AnaerobicDigestionComponentHelperTest
    {
        #region Constructors

        private AnaerobicDigestionComponentHelper _ADHelper;

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
            _ADHelper = new AnaerobicDigestionComponentHelper();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void TestReplicateSingleManagementPeriodViewItems()
        {
            var farm = new Farm();
            var animalGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCow,
                Name = "Beef Cows"
            };

            var managementPeriod = new ManagementPeriod()
            {
                AnimalGroupGuid = animalGroup.Guid,
                Name = "Management Period #1"
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

            var result = _ADHelper.Replicate(componentToReplicate,  farm.AnimalComponents);

            Assert.IsFalse(result.Guid.Equals(componentToReplicate.Guid));
            Assert.IsTrue(result.ManagementPeriodViewItems.Count.Equals(1));

            Assert.IsTrue(result.ManagementPeriodViewItems[0].FlowRate.Equals(11));
            Assert.IsTrue(result.ManagementPeriodViewItems[0].AnimalGroup.Equals(animalGroup));
            Assert.IsTrue(result.ManagementPeriodViewItems[0].AnimalComponent.Equals(animalComponent));
            Assert.IsTrue(result.ManagementPeriodViewItems[0].ManagementPeriod.Equals(managementPeriod));

            Assert.IsTrue(result.AnaerobicDigestionViewItem.ManureSubstrateViewItems[0].FlowRate.Equals(11));
            Assert.IsTrue(result.AnaerobicDigestionViewItem.ManureSubstrateViewItems[0].ManureStateType.Equals(ManureStateType.SolidStorage));

            Assert.IsTrue(result.AnaerobicDigestionViewItem.CropResiduesSubstrateViewItems[0].FlowRate.Equals(11));
            Assert.IsTrue(result.AnaerobicDigestionViewItem.CropResiduesSubstrateViewItems[0].CropType.Equals(CropType.Barley));

            Assert.IsTrue(result.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems[0].FlowRate.Equals(11));
            Assert.IsTrue(result.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems[0].FarmResidueType.Equals(FarmResidueType.FoodWaste));
        }

        [TestMethod]
        public void TestReplicateMultipleManagementPeriodViewItems()
        {
            var farm = new Farm();
            var animalGroup1 = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCow,
                Name = "Beef Cows"
            };

            var managementPeriod1 = new ManagementPeriod()
            {
                AnimalGroupGuid = animalGroup1.Guid,
                Name = "Management Period #1"
            };

            animalGroup1.ManagementPeriods.Add(managementPeriod1);

            var animalComponent1 = new CowCalfComponent();
            animalComponent1.Groups.Add(animalGroup1);
            farm.Components.Add(animalComponent1);

            var animalGroup2 = new AnimalGroup()
            {
                GroupType = AnimalType.SwineBoar,
                Name = "Boar"
            };

            var managementPeriod2 = new ManagementPeriod()
            {
                AnimalGroupGuid = animalGroup2.Guid,
                Name = "Management Period #1"
            };

            animalGroup2.ManagementPeriods.Add(managementPeriod2);

            var animalComponent2 = new BoarComponent();
            animalComponent2.Groups.Add(animalGroup2);
            farm.Components.Add(animalComponent2);

            var componentToReplicate = new AnaerobicDigestionComponent()
            {
                ManagementPeriodViewItems = new ObservableCollection<ADManagementPeriodViewItem>()
                {
                    new ADManagementPeriodViewItem()
                    {
                        ManagementPeriod = managementPeriod1,
                        AnimalComponent = animalComponent1,
                        AnimalGroup = animalGroup1,
                        DailyPercentageOfManureAdded = 11,
                        FlowRate = 11,
                    },
                    new ADManagementPeriodViewItem()
                    {
                        ManagementPeriod = managementPeriod2,
                        AnimalComponent = animalComponent2,
                        AnimalGroup = animalGroup2,
                        DailyPercentageOfManureAdded = 11,
                        FlowRate = 12,
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

            var result = _ADHelper.Replicate(componentToReplicate, farm.AnimalComponents);

            Assert.IsFalse(result.Guid.Equals(componentToReplicate.Guid));
            Assert.IsTrue(result.ManagementPeriodViewItems.Count.Equals(2));

            Assert.IsTrue(result.ManagementPeriodViewItems[0].FlowRate.Equals(11));
            Assert.IsTrue(result.ManagementPeriodViewItems[0].AnimalGroup.Equals(animalGroup1));
            Assert.IsTrue(result.ManagementPeriodViewItems[0].AnimalComponent.Equals(animalComponent1));
            Assert.IsTrue(result.ManagementPeriodViewItems[0].ManagementPeriod.Equals(managementPeriod1));

            Assert.IsTrue(result.ManagementPeriodViewItems[1].FlowRate.Equals(12));
            Assert.IsTrue(result.ManagementPeriodViewItems[1].AnimalGroup.Equals(animalGroup2));
            Assert.IsTrue(result.ManagementPeriodViewItems[1].AnimalComponent.Equals(animalComponent2));
            Assert.IsTrue(result.ManagementPeriodViewItems[1].ManagementPeriod.Equals(managementPeriod2));

            Assert.IsTrue(result.AnaerobicDigestionViewItem.ManureSubstrateViewItems[0].FlowRate.Equals(11));
            Assert.IsTrue(result.AnaerobicDigestionViewItem.ManureSubstrateViewItems[0].ManureStateType.Equals(ManureStateType.SolidStorage));

            Assert.IsTrue(result.AnaerobicDigestionViewItem.CropResiduesSubstrateViewItems[0].FlowRate.Equals(11));
            Assert.IsTrue(result.AnaerobicDigestionViewItem.CropResiduesSubstrateViewItems[0].CropType.Equals(CropType.Barley));

            Assert.IsTrue(result.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems[0].FlowRate.Equals(11));
            Assert.IsTrue(result.AnaerobicDigestionViewItem.FarmResiduesSubstrateViewItems[0].FarmResidueType.Equals(FarmResidueType.FoodWaste));
        }

        [TestMethod]
        public void TestReplicateEmptyAD()
        {
            var farm = new Farm();
            var animalGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCow,
                Name = "Beef Cows"
            };

            var managementPeriod = new ManagementPeriod()
            {
                AnimalGroupGuid = animalGroup.Guid,
                Name = "Management Period #1"
            };

            animalGroup.ManagementPeriods.Add(managementPeriod);

            var animalComponent = new CowCalfComponent();
            animalComponent.Groups.Add(animalGroup);
            farm.Components.Add(animalComponent);

            var componentToReplicate = new AnaerobicDigestionComponent();

            var result = _ADHelper.Replicate(componentToReplicate, farm.AnimalComponents);

            Assert.IsFalse(result.ManagementPeriodViewItems.Any());
        }
        #endregion
    }
}
