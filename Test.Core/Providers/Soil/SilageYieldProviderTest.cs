﻿using H.Core.Enumerations;
using H.Core.Providers.Soil;

namespace H.Core.Test.Providers.Soil;

[TestClass]
public class SilageYieldProviderTest
{
    #region Fields

    private static SilageYieldProvider _provider;

    #endregion

    #region Initialization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _provider = new SilageYieldProvider();
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
    public void GetOntarioYieldValue()
    {
        var data = _provider.GetDataInstance(2001, Province.Ontario);

        Assert.AreEqual(23850, data.Yield);
    }

    [TestMethod]
    public void GetMonitobaYieldValue()
    {
        var data = _provider.GetDataInstance(2015, Province.Manitoba);

        Assert.AreEqual(38310, data.Yield);
    }

    [TestMethod]
    public void TestWrongProvinceInput()
    {
        var data = _provider.GetDataInstance(2001, Province.Yukon);

        Assert.AreEqual(null, data);
    }

    [TestMethod]
    public void TestWrongYearInput()
    {
        var data = _provider.GetDataInstance(3154, Province.Ontario);

        Assert.AreEqual(null, data);
    }

    [TestMethod]
    public void TestValueNotAvailable()
    {
        var data = _provider.GetDataInstance(2018, Province.Alberta);

        Assert.AreEqual(0, data.Yield);
    }

    #endregion
}