﻿using H.Core.Enumerations;
using H.Core.Providers.Plants;

namespace H.Core.Test.Providers.Plants;

[TestClass]
public class LumCMax_KValues_Perennial_Cropping_Change_Provider_Test
{
    private LumCMax_KValues_Perennial_Cropping_Change_Provider _forPerennialsAndGrasslandProvider;

    [TestInitialize]
    public void TestInitialize()
    {
        _forPerennialsAndGrasslandProvider = new LumCMax_KValues_Perennial_Cropping_Change_Provider();
    }

    [TestMethod]
    public void GetLumCMax()
    {
        var result = _forPerennialsAndGrasslandProvider.GetLumCMax(Ecozone.AtlanticMaritimes, SoilTexture.Coarse,
            PerennialCroppingChangeType.DecreaseInPerennialCroppingArea);
        Assert.AreEqual(result, -3769);
    }

    [TestMethod]
    public void GetKValue()
    {
        var result = _forPerennialsAndGrasslandProvider.GetKValue(Ecozone.BorealShieldWest, SoilTexture.Medium,
            PerennialCroppingChangeType.IncreaseInPerennialCroppingArea);
        Assert.AreEqual(result, 0.0253);
    }
}