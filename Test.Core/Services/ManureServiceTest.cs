using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Services.Animals;
using Moq;

namespace H.Core.Test.Services;

[TestClass]
public class ManureServiceTest : UnitTestBase
{
    #region Fields

    private ManureService _sut;
    private Mock<IAnimalService> _mockAnimalService;
    private AnimalComponentEmissionsResults _componentResults;

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
        _mockAnimalService = new Mock<IAnimalService>();
        _sut = new ManureService();

        _componentResults = GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults();
        _mockAnimalService.Setup(x => x.GetAnimalResults(It.IsAny<AnimalType>(), It.IsAny<Farm>()))
            .Returns(new List<AnimalComponentEmissionsResults> { _componentResults });
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void InitializeTankTest()
    {
        var managementPeriod = new ManagementPeriod { Start = DateTime.Now };

        managementPeriod.NumberOfDays = 365 * 12;
        managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
        managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

        var group = new AnimalGroup();
        group.ManagementPeriods.Add(managementPeriod);

        var component = new FinishingComponent();
        component.Groups.Add(group);

        var farm = new Farm();
        farm.Components.Add(component);
    }

    [TestMethod]
    public void GetAmountAvailableForExportReturnsZeroWhenNoAnimalsPresentTest()
    {
        var result = _sut.GetVolumeAvailableForExport(DateTime.Now.Year);

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetAmountAvailableForExportReturnsNonZeroWhenAnimalsArePresentTest()
    {
        var farm = GetTestFarm();
        farm.Components.Add(GetTestFieldComponent());
        foreach (var allManagementPeriod in farm.GetAllManagementPeriods())
            allManagementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

        _sut.Initialize(farm, new List<AnimalComponentEmissionsResults> { _componentResults });
        var result = _sut.GetVolumeAvailableForExport(DateTime.Now.Year);

        Assert.IsTrue(result > 0);
    }

    [TestMethod]
    public void GetValidManureTypesReturnsCorrectCountTest()
    {
        var result = _sut.GetValidManureTypes();
        Assert.AreEqual(14, result.Count);
    }

    [TestMethod]
    public void GetAmountAvailableForExportReturnsNonZero()
    {
        var farm = GetTestFarm();
        farm.Components.Add(GetTestFieldComponent());
        foreach (var allManagementPeriod in farm.GetAllManagementPeriods())
            allManagementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

        _sut.Initialize(farm, new List<AnimalComponentEmissionsResults> { _componentResults });

        var result = _sut.GetVolumeAvailableForExport(DateTime.Now.Year);

        Assert.AreEqual(100000, result);
    }

    [TestMethod]
    public void GetTotalAmountOfManureExportedReturnsZeroWhenNoExportsCreated()
    {
        var farm = GetTestFarm();
        farm.ManureExportViewItems.Clear();

        var result = _sut.GetTotalVolumeOfManureExported(DateTime.Now.Year, farm, AnimalType.Beef);

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetTotalAmountOfManureExportedReturnsNonZeroWhenExportsCreated()
    {
        var farm = GetTestFarm();

        var result = _sut.GetTotalVolumeOfManureExported(DateTime.Now.Year, farm, AnimalType.Dairy);

        Assert.AreEqual(3000, result);
    }

    [TestMethod]
    public void GetValidManureStateTypesForImportedManureReturnsCorrectCount()
    {
        var result = _sut.GetValidManureStateTypes(new Farm(), ManureLocationSourceType.Imported, AnimalType.Beef);

        Assert.AreEqual(4, result.Count);
    }

    [TestMethod]
    public void GetValidManureStateTypesForOnFarmManureReturnsCorrectCount()
    {
        var farm = GetTestFarm();
        foreach (var allManagementPeriod in farm.GetAllManagementPeriods())
            allManagementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

        _sut.Initialize(farm, new List<AnimalComponentEmissionsResults> { _componentResults });
        var result = _sut.GetValidManureStateTypes(farm, ManureLocationSourceType.Livestock, AnimalType.Beef);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(ManureStateType.AnaerobicDigester, result[0]);
    }

    [TestMethod]
    public void GetManureTypesProducedOnFarm()
    {
        var farm = GetTestFarm();

        var result = _sut.GetManureCategoriesProducedOnFarm(farm);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetYearWithMostManureProducedTest()
    {
        var farm = GetTestFarm();
        _sut.Initialize(farm, new List<AnimalComponentEmissionsResults> { _componentResults });

        var result = _sut.GetYearHighestVolumeRemaining(AnimalType.Dairy);

        Assert.AreEqual(DateTime.Now.Year, result);
    }

    [TestMethod]
    public void GetManureCompositionDataReturnsNonNull()
    {
        var result = _sut.GetManureCompositionData(new Farm(), ManureStateType.DeepPit, AnimalType.Beef);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetManureCompositionDataReturnsNonNullWithNullArgument()
    {
        var result = _sut.GetManureCompositionData(null, ManureStateType.DeepPit, AnimalType.Beef);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetTotalNitrogenFromExportedManureReturnsZeroTest()
    {
        var farm = GetTestFarm();
        farm.ManureExportViewItems.Clear();


        var result = _sut.GetTotalNitrogenFromExportedManure(DateTime.Now.Year, farm);

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetTotalNitrogenFromExportedManureReturnsNonZeroTest()
    {
        var farm = GetTestFarm();
        farm.ManureExportViewItems.Clear();

        var manureExport = new ManureExportViewItem
        {
            DateOfExport = DateTime.Now,
            Amount = 100,
            DefaultManureCompositionData = new DefaultManureCompositionData { NitrogenContent = 0.5 }
        };

        farm.ManureExportViewItems.Add(manureExport);

        var result = _sut.GetTotalNitrogenFromExportedManure(DateTime.Now.Year, farm);

        Assert.AreEqual(50, result);
    }

    [TestMethod]
    public void GetTotalNitrogenFromExportedManureReturnsNonZeroForMultipleExportsTest()
    {
        var farm = GetTestFarm();
        farm.ManureExportViewItems.Clear();

        var manureExport = new ManureExportViewItem
        {
            DateOfExport = DateTime.Now,
            Amount = 100,
            DefaultManureCompositionData = new DefaultManureCompositionData { NitrogenContent = 0.5 }
        };

        var manureExport2 = new ManureExportViewItem
        {
            DateOfExport = DateTime.Now,
            Amount = 100,
            DefaultManureCompositionData = new DefaultManureCompositionData { NitrogenContent = 0.25 }
        };

        farm.ManureExportViewItems.Add(manureExport);
        farm.ManureExportViewItems.Add(manureExport2);

        var result = _sut.GetTotalNitrogenFromExportedManure(DateTime.Now.Year, farm);

        Assert.AreEqual(75, result);
    }

    [TestMethod]
    public void GetFractionOfTotalManureUsed()
    {
        var manureApplication = GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure();
        var farm = GetTestFarm();
        var cropViewItem = GetTestCropViewItem();
        var emissions = GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults();
        foreach (var allManagementPeriod in farm.GetAllManagementPeriods())
            allManagementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

        _sut.Initialize(farm, new List<AnimalComponentEmissionsResults> { emissions });

        var result = _sut.GetFractionOfTotalManureUsedFromLandApplication(cropViewItem, manureApplication);

        Assert.AreEqual(0.0005, result);
    }

    [TestMethod]
    public void GetAmountOfTanUsedDuringLandApplication()
    {
        var manureApplication = GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure();
        var farm = GetTestFarm();
        var cropViewItem = GetTestCropViewItem();
        var emissions = GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults();

        foreach (var allManagementPeriod in farm.GetAllManagementPeriods())
            allManagementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

        _sut.Initialize(farm, new List<AnimalComponentEmissionsResults> { emissions });

        var result = _sut.GetAmountOfTanUsedDuringLandApplication(cropViewItem, manureApplication);

        Assert.AreEqual(0.05, result);
    }

    [TestMethod]
    public void GetAmountOfTanUsedDuringLandApplications()
    {
        var manureApplication = GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure();
        var farm = GetTestFarm();
        var cropViewItem = GetTestCropViewItem();
        cropViewItem.ManureApplicationViewItems.Clear();
        cropViewItem.ManureApplicationViewItems.Add(GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure());
        cropViewItem.ManureApplicationViewItems.Add(GetTestBeefCattleManureApplicationViewItemUsingOnLivestockManure());

        var emissions = GetNonEmptyTestBeefCattleAnimalComponentEmissionsResults();

        foreach (var allManagementPeriod in farm.GetAllManagementPeriods())
            allManagementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

        _sut.Initialize(farm, new List<AnimalComponentEmissionsResults> { emissions });

        var result = _sut.GetAmountOfTanUsedDuringLandApplications(cropViewItem);

        Assert.AreEqual(0.05 + 0.05, result);
    }

    #endregion
}