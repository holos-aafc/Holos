﻿using H.Core.Enumerations;
using H.Core.Providers.Energy;

namespace H.Core.Test.Providers.Energy;

[TestClass]
public class Table_51_Herbicide_Energy_Estimates_Provider_Test
{
    #region Fields

    private static Table_51_Herbicide_Energy_Estimates_Provider _provider;

    #endregion

    #region Initialization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _provider = new Table_51_Herbicide_Energy_Estimates_Provider();
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
    public void GetHerbicideEnergyEstimateInstanceOntario()
    {
        var data = _provider.GetHerbicideEnergyDataInstance(Province.Ontario, SoilFunctionalCategory.EasternCanada,
            TillageType.Reduced, CropType.BeansDryField);
        Assert.AreEqual(0.12, data.HerbicideEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyEstimateInstanceBritishColumbia()
    {
        var data = _provider.GetHerbicideEnergyDataInstance(Province.BritishColumbia, SoilFunctionalCategory.Brown,
            TillageType.NoTill, CropType.ForageForSeed);
        Assert.AreEqual(0.46, data.HerbicideEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyEstimateInstanceNewfoundLand()
    {
        var data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.EasternCanada,
            TillageType.Intensive, CropType.Chickpeas);
        Assert.AreEqual(0.08, data.HerbicideEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyInstanceWrongProvince()
    {
        var data = _provider.GetHerbicideEnergyDataInstance(Province.Yukon, SoilFunctionalCategory.Brown,
            TillageType.NoTill, CropType.ForageForSeed);
        Assert.AreEqual(0, data.HerbicideEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyInstanceWrongSoil()
    {
        var data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.Organic,
            TillageType.NoTill, CropType.Durum);
        Assert.AreEqual(0, data.HerbicideEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyInstanceWrongCrop()
    {
        var data = _provider.GetHerbicideEnergyDataInstance(Province.Newfoundland, SoilFunctionalCategory.EasternCanada,
            TillageType.NoTill, CropType.LargeKabuliChickpea);
        Assert.AreEqual(0, data.HerbicideEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyValue()
    {
        var data = _provider.GetHerbicideEnergyDataInstance(Province.NewBrunswick, SoilFunctionalCategory.EasternCanada,
            TillageType.NoTill, CropType.Potatoes);
        Assert.AreEqual(0.12, data.HerbicideEstimate);

        data = _provider.GetHerbicideEnergyDataInstance(Province.Quebec, SoilFunctionalCategory.EasternCanada,
            TillageType.Reduced, CropType.Tobacco);
        Assert.AreEqual(0.24, data.HerbicideEstimate);

        data = _provider.GetHerbicideEnergyDataInstance(Province.Saskatchewan, SoilFunctionalCategory.Brown,
            TillageType.Intensive, CropType.BerseemCloverTrifoliumAlexandriumL);
        Assert.AreEqual(0.16, data.HerbicideEstimate);

        data = _provider.GetHerbicideEnergyDataInstance(Province.Alberta, SoilFunctionalCategory.Black,
            TillageType.NoTill, CropType.ForageForSeed);
        Assert.AreEqual(0.46, data.HerbicideEstimate);
    }

    #endregion
}