﻿using H.Core.Enumerations;
using H.Core.Providers.Plants;

namespace H.Core.Test.Providers.Plants;

[TestClass]
public class LumCMax_KValues_Fallow_Practice_Change_Provider_Test
{
    private LumCMax_KValues_Fallow_Practice_Change_Provider _provider;

    [TestInitialize]
    public void TestInitialize()
    {
        _provider = new LumCMax_KValues_Fallow_Practice_Change_Provider();
    }

    [TestMethod]
    public void GetLumCMax()
    {
        var result = _provider.GetLumCMax(Ecozone.AtlanticMaritimes, SoilTexture.Coarse,
            FallowPracticeChangeType.ContinousToFallowCropping);
        Assert.AreEqual(result, -1314);
    }

    [TestMethod]
    public void GetKValue()
    {
        var result = _provider.GetKValue(Ecozone.BorealShieldWest, SoilTexture.Medium,
            FallowPracticeChangeType.FallowCroppingToContinous);
        Assert.AreEqual(result, 0.0305);
    }
}