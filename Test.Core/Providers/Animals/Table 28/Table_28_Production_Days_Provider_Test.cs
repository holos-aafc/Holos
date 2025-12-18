using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals.Table_28;

namespace H.Core.Test.Providers.Animals.Table_28;

[TestClass]
public class Table_28_Production_Days_Provider_Test
{
    #region Fields

    private ITable_28_Production_Days_Provider _sut;

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
        _sut = new Table_28_Production_Days_Provider();
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void HasProductionDataReturnsTrueTest()
    {
        var result = _sut.HasProductionCycle(AnimalType.BeefBackgrounderHeifer, ProductionStages.GrowingAndFinishing,
            ComponentType.Backgrounding);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void HasProductionDataReturnsFalseTest()
    {
        var result = _sut.HasProductionCycle(AnimalType.BeefFinishingHeifer, ProductionStages.GrowingAndFinishing,
            ComponentType.Finishing);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HasProductionDataReturnsFalseForCowsTest()
    {
        var result = _sut.HasProductionCycle(AnimalType.BeefCowLactating, ProductionStages.Lactating,
            ComponentType.CowCalf);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetBackgroundingDataTest()
    {
        var result = _sut.GetData(AnimalType.BeefBackgrounderHeifer, ProductionStages.GrowingAndFinishing,
            ComponentType.Backgrounding);

        Assert.AreEqual(109, result.NumberOfDaysInProductionCycle);
    }

    [TestMethod]
    public void GetSwineDataTest()
    {
        var result = _sut.GetData(AnimalType.SwineSows, ProductionStages.BreedingStock, ComponentType.FarrowToFinish);

        Assert.AreEqual(140, result.NumberOfDaysInProductionCycle);
    }

    [TestMethod]
    public void GetSwineHogsDataTest()
    {
        var result = _sut.GetData(AnimalType.SwineBoar, ProductionStages.BreedingStock, ComponentType.FarrowToFinish);

        Assert.AreEqual(114, result.NumberOfDaysInProductionCycle);
    }

    [TestMethod]
    public void GetSwineWeanerDataTest()
    {
        var result = _sut.GetData(AnimalType.SwinePiglets, ProductionStages.Weaning, ComponentType.IsoWean);

        Assert.AreEqual(35, result.NumberOfDaysInProductionCycle);
    }

    [TestMethod]
    public void GetPoultryDataTest()
    {
        var result = _sut.GetData(AnimalType.ChickenPullets, ProductionStages.BreedingStock);

        Assert.AreEqual(133, result.NumberOfDaysInProductionCycle);
    }

    #endregion
}