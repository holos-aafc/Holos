using H.Core.Enumerations;
using H.Core.Providers.Energy;

namespace H.Core.Test.Providers.Energy;

[TestClass]
public class Table_50_Fuel_Energy_Estimates_Provider_Test
{
    #region Fields

    private static Table_50_Fuel_Energy_Estimates_Provider _provider;

    #endregion

    [TestMethod]
    public void GetFuelEnergyValue()
    {
        var data = _provider.GetFuelEnergyEstimatesDataInstance(Province.NewBrunswick,
            SoilFunctionalCategory.EasternCanada, TillageType.NoTill, CropType.Potatoes);
        Assert.AreEqual(1.9, data.FuelEstimate);

        data = _provider.GetFuelEnergyEstimatesDataInstance(Province.BritishColumbia, SoilFunctionalCategory.Black,
            TillageType.NoTill, CropType.Oats);
        Assert.AreEqual(1.43, data.FuelEstimate);

        data = _provider.GetFuelEnergyEstimatesDataInstance(Province.BritishColumbia, SoilFunctionalCategory.Brown,
            TillageType.Reduced, CropType.Flax);
        Assert.AreEqual(1.78, data.FuelEstimate);

        data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Alberta, SoilFunctionalCategory.Black,
            TillageType.Reduced, CropType.CrimsonCloverTrifoliumIncarnatum);
        Assert.AreEqual(2.39, data.FuelEstimate);

        data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Saskatchewan, SoilFunctionalCategory.Brown,
            TillageType.Intensive, CropType.PigeonBean);
        Assert.AreEqual(2.02, data.FuelEstimate);
    }

    #region Initialization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _provider = new Table_50_Fuel_Energy_Estimates_Provider();
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
    public void GetFuelEnergyEstimateInstanceAlberta()
    {
        var data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Alberta, SoilFunctionalCategory.Black,
            TillageType.NoTill, CropType.CanarySeed);

        Assert.AreEqual(1.43, data.FuelEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyEstimateInstanceOntario()
    {
        var data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Ontario, SoilFunctionalCategory.EasternCanada,
            TillageType.Reduced, CropType.WinterTurnipRapeBrassicaRapaSppOleiferaLCVLargo);
        Assert.AreEqual(1.8, data.FuelEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyEstimateInstanceBritishColumbia()
    {
        var data = _provider.GetFuelEnergyEstimatesDataInstance(Province.BritishColumbia, SoilFunctionalCategory.Brown,
            TillageType.NoTill, CropType.ForageForSeed);
        Assert.AreEqual(1.42, data.FuelEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyEstimateInstanceNewfoundLand()
    {
        var data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Newfoundland,
            SoilFunctionalCategory.EasternCanada, TillageType.Intensive, CropType.Chickpeas);
        Assert.AreEqual(3.29, data.FuelEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyInstanceWrongProvince()
    {
        var data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Yukon, SoilFunctionalCategory.Brown,
            TillageType.NoTill, CropType.ForageForSeed);
        Assert.AreEqual(0, data.FuelEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyInstanceWrongSoil()
    {
        var data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Newfoundland, SoilFunctionalCategory.Organic,
            TillageType.NoTill, CropType.Durum);
        Assert.AreEqual(0, data.FuelEstimate);
    }

    [TestMethod]
    public void GetFuelEnergyInstanceWrongCrop()
    {
        var data = _provider.GetFuelEnergyEstimatesDataInstance(Province.Newfoundland,
            SoilFunctionalCategory.EasternCanada, TillageType.NoTill, CropType.LargeKabuliChickpea);
        Assert.AreEqual(0, data.FuelEstimate);
    }

    #endregion
}