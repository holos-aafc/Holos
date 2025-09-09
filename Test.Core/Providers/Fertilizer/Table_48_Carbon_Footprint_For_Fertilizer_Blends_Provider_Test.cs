﻿using H.Core.Enumerations;
using H.Core.Providers.Fertilizer;

namespace H.Core.Test.Providers.Fertilizer;

[TestClass]
public class Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider_Test
{
    #region Fields

    private Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider _provider;

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
        _provider = new Table_48_Carbon_Footprint_For_Fertilizer_Blends_Provider();
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void GetDataReturnsNonEmptyList()
    {
        var result = _provider.GetData();

        Assert.AreEqual(19, result.Count());
    }

    [TestMethod]
    public void GetFertilizerBlendValues()
    {
        var data = _provider.GetData(FertilizerBlends.Ammonia);
        Assert.AreEqual(2.49, data.CarbonDioxideEmissionsAtTheGate);
        Assert.AreEqual(82, data.PercentageNitrogen);

        data = _provider.GetData(FertilizerBlends.CalciumNitrate);
        Assert.AreEqual(1.58, data.CarbonDioxideEmissionsAtTheGate);

        data = _provider.GetData(FertilizerBlends.Urea);
        Assert.AreEqual(0.73, data.ApplicationEmissions);

        data = _provider.GetData(FertilizerBlends.NpkMixedAcid);
        Assert.AreEqual(15, data.PercentagePotassium);
    }

    #endregion
}