﻿using H.Core.Enumerations;
using H.Core.Providers.Climate;

namespace H.Core.Test.Providers.Climate;

[TestClass]
public class Table_55_Global_Radiative_Forcing_Provider_Test
{
    #region Fields

    private static Table_55_Global_Radiative_Forcing_Provider _provider;

    #endregion

    #region Initialization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _provider = new Table_55_Global_Radiative_Forcing_Provider();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
    }

    [TestInitialize]
    public void TestInitialize()
    {
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void GetGlobalRadiativeForcingValue()
    {
        var data = _provider.GetGlobalRadiativeForcingInstance(1983, EmissionTypes.CH4);
        Assert.AreEqual(0.429, data.RadiativeForcingValue);
    }

    [TestMethod]
    public void InstanceNotFoundWrongEmission()
    {
        var data = _provider.GetGlobalRadiativeForcingInstance(1995, EmissionTypes.CO2e);
        Assert.AreEqual(null, data);
    }

    [TestMethod]
    public void InstanceNotFoundWrongYear()
    {
        var data = _provider.GetGlobalRadiativeForcingInstance(1950, EmissionTypes.N2O);
        Assert.AreEqual(null, data);
    }

    [TestMethod]
    public void DataInstanceNotFoundAllInputsWrong()
    {
        var data = _provider.GetGlobalRadiativeForcingInstance(3000, EmissionTypes.CropsDirectN20);
        Assert.AreEqual(null, data);
    }

    #endregion
}