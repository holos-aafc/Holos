using System.Collections.ObjectModel;
using System.ComponentModel;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.Animals.OtherAnimals;
using H.Core.Models.Animals.Poultry.Chicken;
using H.Core.Models.Animals.Poultry.Turkey;
using H.Core.Models.Animals.Sheep;
using H.Core.Models.Animals.Swine;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Feed;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.Initialization;
using Moq;

namespace H.Core.Test.Services.Animals;

[TestClass]
public class ManagementPeriodServiceTest : UnitTestBase
{
    #region Fields

    private readonly DietProvider dietProvider = new();
    private IManagementPeriodService _managementPeriodService;
    private Farm _farm;
    private ManagementPeriod _managementPeriod;
    private Mock<IInitializationService> _mockInitializationService;
    private Mock<IAnimalComponentHelper> _mockanimalComponentHelper;

    private void ManagementPeriodOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
    }

    private void AnimalGroupOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
    }

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
        _farm = new Farm();
        _farm.Diets.AddRange(dietProvider.GetDiets());
        _mockInitializationService = new Mock<IInitializationService>();


        _mockanimalComponentHelper = new Mock<IAnimalComponentHelper>();
        _mockanimalComponentHelper.SetupSequence(x => x.GetUniqueManagementPeriodName(It.IsAny<AnimalGroup>()))
            .Returns("1").Returns("2").Returns("3").Returns("4");

        var managementPeriod1 = new ManagementPeriod
        {
            Name = "1",
            AnimalType = AnimalType.BeefBulls,
            HousingDetails =
            {
                HousingType = HousingType.FreeStallBarnSolidLitter,
                BeddingMaterialType = BeddingMaterialType.Straw
            },
            ManureDetails =
            {
                StateType = ManureStateType.SolidStorage
            }
        };

        var managementPeriod2 = new ManagementPeriod
        {
            Name = "2",
            AnimalType = AnimalType.BeefBulls,
            HousingDetails =
            {
                HousingType = HousingType.FreeStallBarnSolidLitter,
                BeddingMaterialType = BeddingMaterialType.None
            },
            ManureDetails =
            {
                StateType = ManureStateType.SolidStorage
            }
        };

        var managementPeriod3 = new ManagementPeriod
        {
            Name = "3",
            HousingDetails =
            {
                HousingType = HousingType.Pasture,
                BeddingMaterialType = BeddingMaterialType.None
            },
            ManureDetails =
            {
                StateType = ManureStateType.SolidStorage
            }
        };

        var managementPeriod4 = new ManagementPeriod
        {
            Name = "4"
        };
        _mockanimalComponentHelper.SetupSequence(x => x.ReplicateManagementPeriod(It.IsAny<ManagementPeriod>()))
            .Returns(managementPeriod1).Returns(managementPeriod2).Returns(managementPeriod3)
            .Returns(managementPeriod4);

        _mockInitializationService.Setup(x => x.InitializeStartAndEndWeightsForCattle(It.IsAny<ManagementPeriod>()))
            .Callback(
                (ManagementPeriod managementPeriod) =>
                {
                    managementPeriod.StartWeight = 1;
                    managementPeriod.EndWeight = 2;
                });

        _mockInitializationService.Setup(x => x.InitializeLivestockCoefficientSheep(It.IsAny<ManagementPeriod>()))
            .Callback(
                (ManagementPeriod managementPeriod) =>
                {
                    managementPeriod.HousingDetails.BaselineMaintenanceCoefficient = 2;
                    managementPeriod.WoolProduction = 3;
                    managementPeriod.GainCoefficientA = 4;
                    managementPeriod.GainCoefficientB = 5;
                    managementPeriod.StartWeight = 6;
                    managementPeriod.EndWeight = 7;
                    managementPeriod.NumberOfAnimals = 100;
                    managementPeriod.EnergyRequiredForWool = 24;
                    managementPeriod.EnergyRequiredForMilk = 4.6;
                });

        _managementPeriodService =
            new ManagementPeriodService(_mockInitializationService.Object, _mockanimalComponentHelper.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void FinishingSteerGroupManagementPeriodDefault()
    {
        var steerGroup = new AnimalGroup
        {
            GroupType = AnimalType.BeefFinishingSteer
        };
        var steerComponent = new FinishingComponent();
        steerComponent.Groups.Add(steerGroup);
        _farm.Components.Add(steerComponent);

        _managementPeriodService.FinishingSteerGroupManagementPeriod(_farm, steerGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.BarleyGrainBased));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.NumberOfAnimals.Equals(20)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.NumberOfDays.Equals(169)));
        Assert.IsTrue(_farm.GetAllManagementPeriods()
            .All(x => x.HousingDetails.HousingType.Equals(HousingType.ConfinedNoBarn)));
        Assert.IsTrue(_farm.GetAllManagementPeriods()
            .All(x => x.ManureDetails.StateType.Equals(ManureStateType.DeepBedding)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.StartWeight.Equals(1)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.EndWeight.Equals(2)));
    }

    [TestMethod]
    public void FinishingHeiferGroupManagementPeriodDefault()
    {
        var HeiferGroup = new AnimalGroup
        {
            GroupType = AnimalType.BeefFinishingHeifer
        };
        var HeiferComponent = new DairyComponent();
        HeiferComponent.Groups.Add(HeiferGroup);
        _farm.Components.Add(HeiferComponent);

        _managementPeriodService.FinishingHeiferGroupManagementPeriod(_farm, HeiferGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.BarleyGrainBased));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.NumberOfAnimals.Equals(49)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.NumberOfDays.Equals(169)));
        Assert.IsTrue(_farm.GetAllManagementPeriods()
            .All(x => x.HousingDetails.HousingType.Equals(HousingType.ConfinedNoBarn)));
        Assert.IsTrue(_farm.GetAllManagementPeriods()
            .All(x => x.ManureDetails.StateType.Equals(ManureStateType.DeepBedding)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.StartWeight.Equals(1)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.EndWeight.Equals(2)));
    }

    [TestMethod]
    public void BackgrounderSteerGroupManagementPeriodDefault()
    {
        var steerGroup = new AnimalGroup
        {
            GroupType = AnimalType.BeefBackgrounderSteer
        };
        var steerComponent = new BackgroundingComponent();
        steerComponent.Groups.Add(steerGroup);
        _farm.Components.Add(steerComponent);

        _managementPeriodService.BackgrounderSteerGroupManagementPeriod(_farm, steerGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType.Equals(DietType.MediumGrowth));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.NumberOfAnimals.Equals(50)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.NumberOfDays.Equals(109)));
        Assert.IsTrue(_farm.GetAllManagementPeriods()
            .All(x => x.HousingDetails.HousingType.Equals(HousingType.ConfinedNoBarn)));
        Assert.IsTrue(_farm.GetAllManagementPeriods()
            .All(x => x.ManureDetails.StateType.Equals(ManureStateType.DeepBedding)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.StartWeight.Equals(1)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.EndWeight.Equals(2)));
    }

    [TestMethod]
    public void BackgroundersHeiferGroupManagementPeriodDefault()
    {
        var HeiferGroup = new AnimalGroup
        {
            GroupType = AnimalType.BeefBackgrounderHeifer
        };
        var HeiferComponent = new BackgroundingComponent();
        HeiferComponent.Groups.Add(HeiferGroup);
        _farm.Components.Add(HeiferComponent);

        _managementPeriodService.BackgrounderHeifersGroupManagementPeriod(_farm, HeiferGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType.Equals(DietType.MediumGrowth));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.NumberOfAnimals.Equals(50)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.NumberOfDays.Equals(109)));
        Assert.IsTrue(_farm.GetAllManagementPeriods()
            .All(x => x.HousingDetails.HousingType.Equals(HousingType.ConfinedNoBarn)));
        Assert.IsTrue(_farm.GetAllManagementPeriods()
            .All(x => x.ManureDetails.StateType.Equals(ManureStateType.DeepBedding)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.StartWeight.Equals(1)));
        Assert.IsTrue(_farm.GetAllManagementPeriods().All(x => x.EndWeight.Equals(2)));
    }

    [TestMethod]
    public void CowCalfBullGroupManagementPeriodDefault()
    {
        var cowCalfGroup = new AnimalGroup
        {
            GroupType = AnimalType.BeefBulls
        };
        var cowCalfComponent = new CowCalfComponent();
        cowCalfComponent.Groups.Add(cowCalfGroup);
        _farm.Components.Add(cowCalfComponent);
        _farm.Components.Add(new FieldSystemComponent());

        _managementPeriodService.CowCalfBullGroupManagementPeriod(_farm, cowCalfGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(3));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Winter feeding"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.MediumEnergyAndProtein));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(4));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(119));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.ConfinedNoBarn));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.DeepBedding));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Summer grazing"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType
            .Equals(DietType.HighEnergyAndProtein));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(183));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.Pasture));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ManureDetails.StateType
            .Equals(ManureStateType.Pasture));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Extended fall grazing"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).SelectedDiet.DietType
            .Equals(DietType.MediumEnergyAndProtein));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(60));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.Pasture));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ManureDetails.StateType
            .Equals(ManureStateType.Pasture));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(2));
    }

    [TestMethod]
    public void CowCalfReplacementHeiferManagementPeriodDefault()
    {
        var replacmentHeiferGroup = new AnimalGroup
        {
            GroupType = AnimalType.BeefReplacementHeifers
        };
        var replacementHeiferComponent = new CowCalfComponent();
        replacementHeiferComponent.Groups.Add(replacmentHeiferGroup);
        _farm.Components.Add(replacementHeiferComponent);

        _managementPeriodService.CowCalfReplacementHeifersManagementPeriod(_farm, replacmentHeiferGroup,
            _managementPeriod, AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.LowEnergyAndProtein));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(365));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.ConfinedNoBarn));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.DeepBedding));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(2));
    }

    [TestMethod]
    public void CowCalfCalfGroupManagementPeriodDefault()
    {
        var calfGroup = new AnimalGroup
        {
            GroupType = AnimalType.CowCalf
        };
        var calfComponent = new CowCalfComponent();
        calfComponent.Groups.Add(calfGroup);
        _farm.Components.Add(calfComponent);

        _managementPeriodService.CowCalfCalfGroupManagementPeriod(_farm, calfGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(102));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(60));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.ConfinedNoBarn));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.DeepBedding));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(260));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("2"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(102));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(152));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.ConfinedNoBarn));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ManureDetails.StateType
            .Equals(ManureStateType.DeepBedding));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(260));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(260));
    }

    [TestMethod]
    public void CowCalfCowGroupManagementPeriodDefault()
    {
        var cowGroup = new AnimalGroup
        {
            GroupType = AnimalType.BeefCow
        };
        var cowComponent = new CowCalfComponent();
        cowComponent.Groups.Add(cowGroup);
        _farm.Components.Add(cowComponent);
        _farm.Components.Add(new FieldSystemComponent());

        _managementPeriodService.CowCalfCowGroupManagementPeriod(_farm, cowGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(4));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Winter feeding - dry"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.MediumEnergyAndProtein));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(120));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(59));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.ConfinedNoBarn));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.DeepBedding));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Winter feeding - lactating"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType
            .Equals(DietType.MediumEnergyAndProtein));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(120));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(61));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.ConfinedNoBarn));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.Straw));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ManureDetails.StateType
            .Equals(ManureStateType.DeepBedding));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Summer grazing"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).SelectedDiet.DietType
            .Equals(DietType.HighEnergyAndProtein));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(120));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(183));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.Pasture));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ManureDetails.StateType
            .Equals(ManureStateType.Pasture));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Extended fall grazing"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).SelectedDiet.DietType
            .Equals(DietType.MediumEnergyAndProtein));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfAnimals.Equals(120));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(60));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).HousingDetails.HousingType
            .Equals(HousingType.Pasture));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ManureDetails.StateType
            .Equals(ManureStateType.Pasture));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).StartWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).EndWeight.Equals(2));
    }

    [TestMethod]
    public void DairyCalvesGroupManagementPeriod()
    {
        var cowGroup = new AnimalGroup
        {
            GroupType = AnimalType.DairyCalves
        };
        var cowComponent = new DairyComponent();
        cowComponent.Groups.Add(cowGroup);
        _farm.Components.Add(cowComponent);

        _managementPeriodService.DairyCalvesGroupManagementPeriod(_farm, cowGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));


        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Milk-fed dairy calves"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.SolidStorage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Weaning));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(90));
    }

    [TestMethod]
    public void DairyReplacementHeifersGroupManagementPeriod()
    {
        var cowGroup = new AnimalGroup
        {
            GroupType = AnimalType.DairyHeifers
        };
        var cowComponent = new DairyComponent();
        cowComponent.Groups.Add(cowGroup);
        _farm.Components.Add(cowComponent);

        _managementPeriodService.DairyReplacementHeifersManagementPeriod(_farm, cowGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.LegumeForageBased));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.SolidStorage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
    }

    [TestMethod]
    public void DairyDryGroupManagementPeriod()
    {
        var cowGroup = new AnimalGroup
        {
            GroupType = AnimalType.DairyDryCow
        };
        var cowComponent = new DairyComponent();
        cowComponent.Groups.Add(cowGroup);
        _farm.Components.Add(cowComponent);

        _managementPeriodService.DairyDryManagementPeriod(_farm, cowGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Dry period"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.LegumeForageBased));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.SolidStorage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(60));
    }

    [TestMethod]
    public void DairyLactatingGroupManagementPeriod()
    {
        var cowGroup = new AnimalGroup
        {
            GroupType = AnimalType.DairyLactatingCow
        };
        var cowComponent = new DairyComponent();
        cowComponent.Groups.Add(cowGroup);
        _farm.Components.Add(cowComponent);

        _managementPeriodService.DairyLactatingManagementPeriod(_farm, cowGroup, _managementPeriod,
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(3));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Early lactation"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.LegumeForageBased));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.SolidStorage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(150));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Mid lactation"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.LegumeForageBased));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.Straw));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ManureDetails.StateType
            .Equals(ManureStateType.SolidStorage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(60));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Late lactation"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.LegumeForageBased));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ManureDetails.StateType
            .Equals(ManureStateType.SolidStorage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(95));
    }

    [TestMethod]
    public void FarrowToFinishBoarsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwineBoar
        };
        var swineComponent = new FarrowToFinishComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToFinishBoarsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Heat detection"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType.Equals(DietType.Boars));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.BeddingMaterialType
            .Equals(BeddingMaterialType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ManureDetails.StateType
            .Equals(ManureStateType.CompostedInVessel));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.BreedingStock));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(365));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(198));
    }

    [TestMethod]
    public void FarrowToFinishGiltsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwineGilts
        };
        var swineComponent = new FarrowToFinishComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToFinishGiltsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(5));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Open gilts"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.GiltDeveloperDiet));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.BreedingStock));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(5));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Bred gilts Stage #1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Bred gilts Stage #2"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Bred gilts Stage #3"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).Name.Equals("Farrowing gilts"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).SelectedDiet.DietType.Equals(DietType.Lactation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).ProductionStage
            .Equals(ProductionStages.Lactating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).NumberOfDays.Equals(21));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).EndWeight.Equals(198));
    }


    [TestMethod]
    public void FarrowToFinishSowsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwineSows
        };
        var swineComponent = new FarrowToFinishComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToFinishSowsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(5));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Open sows"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Weaning));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(5));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Bred sows Stage #1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage
            .Equals(ProductionStages.Open));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Bred sows Stage #2"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Bred sows Stage #3"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).Name.Equals("Farrowing lactating sows"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).SelectedDiet.DietType.Equals(DietType.Lactation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).ProductionStage
            .Equals(ProductionStages.Lactating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).NumberOfDays.Equals(21));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).EndWeight.Equals(198));
    }

    [TestMethod]
    public void FarrowToFinishHogsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwineBoar
        };
        var swineComponent = new FarrowToFinishComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToFinishHogsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(4));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Grower-finisher hogs Stage #1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.GrowerFinisherDiet1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.GrowingAndFinishing));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(26));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(30));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(50));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Grower-finisher hogs Stage #2"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType
            .Equals(DietType.GrowerFinisherDiet2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage
            .Equals(ProductionStages.GrowingAndFinishing));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(17));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(50));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(65));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Grower-finisher hogs Stage #3"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).SelectedDiet.DietType
            .Equals(DietType.GrowerFinisherDiet3));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage
            .Equals(ProductionStages.GrowingAndFinishing));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(27));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(65));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(90));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Grower-finisher hogs Stage #4"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).SelectedDiet.DietType
            .Equals(DietType.GrowerFinisherDiet4));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ProductionStage
            .Equals(ProductionStages.GrowingAndFinishing));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(45));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).StartWeight.Equals(90));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).EndWeight.Equals(130));
    }

    [TestMethod]
    public void FarrowToFinishPigletsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwinePiglets
        };
        var swineComponent = new FarrowToFinishComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToFinishPigletsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Suckling piglets"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Weaning));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(21));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(1.4));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(6));
    }

    [TestMethod]
    public void FarrowToFinishWeanedPigletsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwinePiglets
        };
        var swineComponent = new FarrowToFinishComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToFinishWeanedPigletsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Weaned piglets Stage #1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.NurseryWeanersStarter1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Weaned));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(19));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(6.23));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(20.556));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Weaned piglets Stage #2"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType
            .Equals(DietType.NurseryWeanersStarter2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage
            .Equals(ProductionStages.Weaned));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(16));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(20.765));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(30.829));
    }

    [TestMethod]
    public void IsoWeanPigletsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwinePiglets
        };
        var swineComponent = new IsoWeanComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.IsoWeanPigletsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Weaned piglets Stage #1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.NurseryWeanersStarter1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Weaned));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(19));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(6));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(20));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Weaned piglets Stage #2"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType
            .Equals(DietType.NurseryWeanersStarter1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage
            .Equals(ProductionStages.Weaned));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(16));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(30));
    }

    [TestMethod]
    public void FarrowToWeanPigletsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwinePiglets
        };
        var swineComponent = new FarrowToWeanComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToWeanPigletsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Suckling piglets"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType.Equals(DietType.None));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Weaning));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(21));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(0));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(5.25));
    }

    [TestMethod]
    public void FarrowToWeanGiltsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwineGilts
        };
        var swineComponent = new FarrowToWeanComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToWeanGiltsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(5));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Open gilts"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.GiltDeveloperDiet));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Open));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(5));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Bred gilts (Stage #1)"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Bred gilts (Stage #2)"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Bred gilts (Stage #3)"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).Name.Equals("Farrowing gilts"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).SelectedDiet.DietType.Equals(DietType.Lactation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).ProductionStage
            .Equals(ProductionStages.Lactating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).NumberOfDays.Equals(21));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).EndWeight.Equals(198));
    }

    [TestMethod]
    public void FarrowToWeanSowsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwineSows
        };
        var swineComponent = new FarrowToWeanComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToWeanSowsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(5));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Open sows"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Open));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(5));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Bred sows (Stage #1)"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Bred sows (Stage #2)"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Bred sows (Stage #3)"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).SelectedDiet.DietType.Equals(DietType.Gestation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(38));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).EndWeight.Equals(198));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).Name.Equals("Farrowing lactating sows"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).SelectedDiet.DietType.Equals(DietType.Lactation));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).ProductionStage
            .Equals(ProductionStages.Lactating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).NumberOfDays.Equals(21));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(4).EndWeight.Equals(198));
    }

    [TestMethod]
    public void FarrowToWeanBoarsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwineBoar
        };
        var swineComponent = new FarrowToWeanComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.FarrowToWeanBoarsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Boars"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType.Equals(DietType.Boars));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.BreedingStock));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(365));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(198));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(198));
    }

    [TestMethod]
    public void GrowerToFinishHogsManagementPeriod()
    {
        var swineGroup = new AnimalGroup
        {
            GroupType = AnimalType.SwineGrower
        };
        var swineComponent = new GrowerToFinishComponent();
        swineComponent.Groups.Add(swineGroup);
        _farm.Components.Add(swineComponent);

        _managementPeriodService.GrowerToFinishHogsManagementPeriod(_farm, swineGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged,
            ManagementPeriodOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(4));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Stage #1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.GrowerFinisherDiet1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.GrowingAndFinishing));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(26));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(30));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(50));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Stage #2"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).SelectedDiet.DietType
            .Equals(DietType.GrowerFinisherDiet2));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage
            .Equals(ProductionStages.GrowingAndFinishing));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(17));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).StartWeight.Equals(50));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).EndWeight.Equals(65));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Stage #3"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).SelectedDiet.DietType
            .Equals(DietType.GrowerFinisherDiet3));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage
            .Equals(ProductionStages.GrowingAndFinishing));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(27));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).StartWeight.Equals(65));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).EndWeight.Equals(90));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Stage #4"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).SelectedDiet.DietType
            .Equals(DietType.GrowerFinisherDiet4));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ProductionStage
            .Equals(ProductionStages.GrowingAndFinishing));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(45));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).StartWeight.Equals(90));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).EndWeight.Equals(130));
    }

    [TestMethod]
    public void LambsAndEwesLambsManagementPeriod()
    {
        var sheepGroup = new AnimalGroup
        {
            GroupType = AnimalType.Lambs
        };
        var sheepComponent = new EwesAndLambsComponent();
        sheepComponent.Groups.Add(sheepGroup);
        _farm.Components.Add(sheepComponent);

        _managementPeriodService.LambsAndEwesLambsManagementPeriod(_farm, sheepGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Weaning));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(100));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(6));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(7));
    }

    [TestMethod]
    public void LambsAndEwesEwesManagementPeriod()
    {
        var sheepGroup = new AnimalGroup
        {
            GroupType = AnimalType.Ewes
        };
        var sheepComponent = new EwesAndLambsComponent();
        sheepComponent.Groups.Add(sheepGroup);
        _farm.Components.Add(sheepComponent);

        _managementPeriodService.LambsAndEwesEwesManagementPeriod(_farm, sheepGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Pregnancy"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.GoodQualityForage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(100));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(147));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(6));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(7));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Lactation"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.GoodQualityForage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage
            .Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(100));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(147));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(6));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(7));
    }

    [TestMethod]
    public void RamsRamsManagementPeriod()
    {
        var sheepGroup = new AnimalGroup
        {
            GroupType = AnimalType.Ram
        };
        var sheepComponent = new RamsComponent();
        sheepComponent.Groups.Add(sheepGroup);
        _farm.Components.Add(sheepComponent);

        _managementPeriodService.RamsManagementPeriod(_farm, sheepGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.GoodQualityForage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(100));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(6));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(7));
    }

    [TestMethod]
    public void SheepFeedlotManagementPeriod()
    {
        var sheepGroup = new AnimalGroup
        {
            GroupType = AnimalType.SheepFeedlot
        };
        var sheepComponent = new SheepFeedlotComponent();
        sheepComponent.Groups.Add(sheepGroup);
        _farm.Components.Add(sheepComponent);

        _managementPeriodService.SheepFeedlotManagementPeriod(_farm, sheepGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).SelectedDiet.DietType
            .Equals(DietType.GoodQualityForage));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(100));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).StartWeight.Equals(6));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).EndWeight.Equals(7));
    }

    [TestMethod]
    public void ChickenEggProductionHensManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.ChickenHens
        };
        var poultryComponent = new ChickenEggProductionComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.ChickenEggProductionHensManagementPeriod(_farm, poultryGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Egg laying"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(358));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void PulletFarmPulletsManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.ChickenPullets
        };
        var poultryComponent = new ChickenPulletsComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.PulletFarmPulletsManagementPeriod(_farm, poultryGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(2));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Brooding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(14));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Rearing stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(119));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
    }

    [TestMethod]
    public void ChickenMultiplierBreederLayersManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.Layers
        };
        var poultryComponent = new ChickenMultiplierBreederComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.ChickenMultiplierBreederLayersManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(3));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Brooding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(14));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Rearing stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(140));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Breeding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(344));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
    }

    [TestMethod]
    public void ChickenMultiplierBreederBroilersManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.Broilers
        };
        var poultryComponent = new ChickenMultiplierBreederComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.ChickenMultiplierBreederBroilersManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(3));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Brooding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(14));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Rearing stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(140));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Breeding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(294));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
    }

    [TestMethod]
    public void ChickenMeatProductionBroilersManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.Broilers
        };
        var poultryComponent = new ChickenMeatProductionComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.ChickenMeatProductionBroilersManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(3));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Brooding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(14));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Rearing stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(14));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Rearing stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(14));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
    }

    [TestMethod]
    public void TurkeyMultiplierBreederYoungTomsManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.YoungTom
        };
        var poultryComponent = new TurkeyMultiplierBreederComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.TurkeyMultiplierBreederYoungTomsManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Brooding and rearing stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(210));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void TurkeyMultiplierBreederTomsManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.YoungTurkeyHen
        };
        var poultryComponent = new TurkeyMultiplierBreederComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.TurkeyMultiplierBreederTomsManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Breeding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(168));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void TurkeyMultiplierBreederYoungTurkeyHensManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.YoungTurkeyHen
        };
        var poultryComponent = new TurkeyMultiplierBreederComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.TurkeyMultiplierBreederYoungTurkeyHensManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Brooding and rearing stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(210));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void TurkeyMultiplierBreederTurkeyHensManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.YoungTurkeyHen
        };
        var poultryComponent = new TurkeyMultiplierBreederComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.TurkeyMultiplierBreederTurkeyHensManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Breeding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(168));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void TurkeyMeatProductionYoungTomsManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.Broilers
        };
        var poultryComponent = new TurkeyMeatProductionComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.TurkeyMeatProductionYoungTomsManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(3));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Brooding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(21));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Turkey broilers"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(63));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Heavy turkeys"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(112));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
    }

    [TestMethod]
    public void TurkeyMeatProductionYoungTurkeyHensManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.YoungTurkeyHen
        };
        var poultryComponent = new TurkeyMeatProductionComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.TurkeyMeatProductionYoungTurkeyHensManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(3));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Brooding stage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(14));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Turkey broilers"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(56));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Heavy turkeys"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(84));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));
    }

    [TestMethod]
    public void ChickenMultiplierHatcheryChicksManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.Chicks
        };
        var poultryComponent = new ChickenMultiplierHatcheryComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.ChickenMultiplierHatcheryChicksManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(4));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Incubation"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(18));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Hatching"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(3));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Servicing"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Storage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).HousingDetails.HousingType
            .Equals(HousingType.Pasture));
    }

    [TestMethod]
    public void ChickenMultiplierHatcheryPoultsManagementPeriod()
    {
        var poultryGroup = new AnimalGroup
        {
            GroupType = AnimalType.Poults
        };
        var poultryComponent = new ChickenMultiplierHatcheryComponent();
        poultryComponent.Groups.Add(poultryGroup);
        _farm.Components.Add(poultryComponent);

        _managementPeriodService.ChickenMultiplierHatcheryPoultsManagementPeriod(_farm, poultryGroup,
            new ManagementPeriod(), AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(4));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("Incubation"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(25));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).Name.Equals("Hatching"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).NumberOfDays.Equals(3));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(1).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).Name.Equals("Servicing"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).NumberOfDays.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(2).HousingDetails.HousingType
            .Equals(HousingType.FreeStallBarnSolidLitter));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).Name.Equals("Storage"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).NumberOfDays.Equals(1));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(3).HousingDetails.HousingType
            .Equals(HousingType.Pasture));
    }

    [TestMethod]
    public void GoatsManagementPeriod()
    {
        var goatsGroup = new AnimalGroup
        {
            GroupType = AnimalType.Goats
        };
        var goatsComponent = new GoatsComponent();
        goatsComponent.Groups.Add(goatsGroup);
        _farm.Components.Add(goatsComponent);

        _managementPeriodService.GoatsManagementPeriod(_farm, goatsGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void BisonManagementPeriod()
    {
        var bisonGroup = new AnimalGroup
        {
            GroupType = AnimalType.Bison
        };
        var bisonComponent = new BisonComponent();
        bisonComponent.Groups.Add(bisonGroup);
        _farm.Components.Add(bisonComponent);

        _managementPeriodService.BisonManagementPeriod(_farm, bisonGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void MulesManagementPeriod()
    {
        var mulesGroup = new AnimalGroup
        {
            GroupType = AnimalType.Mules
        };
        var mulesComponent = new MulesComponent();
        mulesComponent.Groups.Add(mulesGroup);
        _farm.Components.Add(mulesComponent);

        _managementPeriodService.MulesManagementPeriod(_farm, mulesGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void HorsesManagementPeriod()
    {
        var horseGroup = new AnimalGroup
        {
            GroupType = AnimalType.Horses
        };
        var horseComponent = new HorsesComponent();
        horseComponent.Groups.Add(horseGroup);
        _farm.Components.Add(horseComponent);

        _managementPeriodService.HorsesManagementPeriod(_farm, horseGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void DeerManagementPeriod()
    {
        var deerGroup = new AnimalGroup
        {
            GroupType = AnimalType.Deer
        };
        var deerComponent = new DeerComponent();
        deerComponent.Groups.Add(deerGroup);
        _farm.Components.Add(deerComponent);

        _managementPeriodService.DeerManagementPeriod(_farm, deerGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    [TestMethod]
    public void LlamaManagementPeriod()
    {
        var deerGroup = new AnimalGroup
        {
            GroupType = AnimalType.Deer
        };
        var deerComponent = new DeerComponent();
        deerComponent.Groups.Add(deerGroup);
        _farm.Components.Add(deerComponent);

        _managementPeriodService.LlamaManagementPeriod(_farm, deerGroup, new ManagementPeriod(),
            AnimalGroupOnPropertyChanged);

        Assert.IsTrue(_farm.GetAllManagementPeriods().Count.Equals(1));

        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).Name.Equals("1"));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).ProductionStage.Equals(ProductionStages.Gestating));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfAnimals.Equals(20));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).NumberOfDays.Equals(31));
        Assert.IsTrue(_farm.GetAllManagementPeriods().ElementAt(0).HousingDetails.HousingType
            .Equals(HousingType.HousedInBarn));
    }

    #endregion
}