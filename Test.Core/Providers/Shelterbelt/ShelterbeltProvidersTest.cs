﻿using H.Core.Enumerations;
using H.Core.Providers.Shelterbelt;

namespace H.Core.Test.Providers.Shelterbelt;

[TestClass]
public class ShelterbeltProvidersTest
{
    [TestMethod]
    public void SlcToHardinessZoneOffsetsCorrect()
    {
        var provider = new SlcToHardinessZoneProvider();
        var table = provider.GetSlcToHardinessZone();
        Assert.AreEqual(244007, table[0].SLCID);
    }

    [TestMethod]
    public void ShelterbeltAllometricTableOffsetsCorrect()
    {
        var table = ShelterbeltAllometricTableProvider.GetShelterbeltAllometricTable();
        Assert.AreEqual(TreeSpecies.HybridPoplar, table[0].TreeSpecies);
    }

    [TestMethod]
    public void ShelterbeltEcodistrictLookupTableOffsetsCorrect()
    {
        var table = ShelterbeltEcodistrictLookupTableProvider.GetShelterbeltEcodistrictLookupTable();
        Assert.AreEqual(709, table[0].EcodistrictID);
    }
}