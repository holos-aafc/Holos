﻿using H.Core.Converters;

namespace H.Core.Test.Converters;

[TestClass]
public class FertilizerBlendConverterTest
{
    #region Fields

    private FertilizerBlendConverter _converter;

    #endregion

    [TestMethod]
    public void TestMethod1()
    {
    }

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
        _converter = new FertilizerBlendConverter();
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion
}