﻿#region Imports

using H.Core.Providers.Climate;
using Test.Core;

#endregion

namespace H.Core.Test.Providers.Climate;

[TestClass]
public class CustomClimateDataProviderTest
{
    #region Fields

    private CustomFileClimateDataProvider _provider;

    #endregion

    #region Tests

    [TestMethod]
    public void ReadUserClimateFileReturnsNonEmptyList()
    {
        var lines = Resource1.custom_climate_data.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
            .ToList();

        var result = _provider.ParseFileLines(lines);

        Assert.AreEqual(38717, result.Count);
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
        _provider = new CustomFileClimateDataProvider();
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion
}