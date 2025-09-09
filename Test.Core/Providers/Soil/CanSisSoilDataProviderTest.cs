﻿#region Imports

using H.Core.Enumerations;
using H.Core.Providers.Soil;

#endregion

namespace H.Core.Test.Providers.Soil;

[TestClass]
public class CanSisSoilDataProviderTest
{
    #region Fields

    private const int PolygonId = 729003;

    private static ISoilDataProvider _sut;

    #endregion

    #region Initialization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _sut = new NationalSoilDataBaseProvider();
        _sut.Initialize();
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
    public void GetSoilDataByPolygonIdReturnsValues()
    {
        var result = _sut.GetPredominantSoilDataByPolygonId(PolygonId);

        Assert.AreEqual(SoilGreatGroupType.BlackChernozem, result.SoilGreatGroup);
        Assert.AreEqual(5.9, result.SoilPh);
        Assert.AreEqual(1.25, result.BulkDensity);
        Assert.AreEqual(4, result.ProportionOfSoilOrganicCarbon);
        Assert.AreEqual(0.42, result.ProportionOfSandInSoil);
        Assert.AreEqual(0.20, result.ProportionOfClayInSoil);
        Assert.AreEqual(200, result.TopLayerThickness);
    }

    [TestMethod]
    public void GetPolygons()
    {
        var soils = new List<SoilData>();
        var result = _sut.GetPolygonIdList();
        foreach (var i in result)
        {
            var a = _sut.GetPredominantSoilDataByPolygonId(i);
            if (a != null) soils.Add(a);
        }

        Assert.IsTrue(soils.Count > 2000);
    }
}

#endregion