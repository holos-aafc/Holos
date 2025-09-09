﻿using H.Core.Enumerations;
using H.Core.Providers.Plants;

namespace H.Core.Test.Providers.Plants;

[TestClass]
public class LumCMax_KValues_Tillage_Practice_Change_Provider_Test
{
    private LumCMax_KValues_Tillage_Practice_Change_Provider _provider;

    [TestInitialize]
    public void TestInitialize()
    {
        _provider = new LumCMax_KValues_Tillage_Practice_Change_Provider();
    }

    [TestMethod]
    public void GetLumCMax()
    {
        var result = _provider.GetLumCMax(Ecozone.AtlanticMaritimes, SoilTexture.Coarse,
            TillagePracticeChangeType.IntenseToReduced);
        Assert.AreEqual(result, 232);
    }

    [TestMethod]
    public void GetKValue()
    {
        var result = _provider.GetKValue(Ecozone.BorealShieldWest, SoilTexture.Medium,
            TillagePracticeChangeType.IntenseToNone);
        Assert.AreEqual(result, 0.0311);
    }
}