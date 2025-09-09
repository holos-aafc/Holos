﻿using H.Core.Enumerations;
using H.Core.Providers.Animals;

namespace H.Core.Test.Providers.Animals;

[TestClass]
public class Table_31_Swine_VS_Excretion_For_Diets_Provider_Test
{
    private Table_31_Swine_VS_Excretion_For_Diets_Provider _provider;

    [TestInitialize]
    public void TestInitialize()
    {
        _provider = new Table_31_Swine_VS_Excretion_For_Diets_Provider();
    }

    [TestMethod]
    public void GetByProvince()
    {
        var result = _provider.GetByProvince(Province.Alberta);
        Assert.AreEqual(0.1504, result[AnimalType.SwineStarter]);
        Assert.AreEqual(0.1389, result[AnimalType.SwineGrower]);
        Assert.AreEqual(0.1389, result[AnimalType.SwineFinisher]);
        Assert.AreEqual(0.1228, result[AnimalType.SwineDrySow]);
        Assert.AreEqual(0.1228, result[AnimalType.SwineLactatingSow]);
    }
}