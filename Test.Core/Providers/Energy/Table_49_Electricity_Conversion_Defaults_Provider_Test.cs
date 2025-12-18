using H.Core.Enumerations;
using H.Core.Providers.Energy;

namespace H.Core.Test.Providers.Energy;

[TestClass]
public class Table_49_Electricity_Conversion_Defaults_Provider_Test
{
    #region Fields

    private static Table_49_Electricity_Conversion_Defaults_Provider _provider;
    private const int FirstYear = 1990;
    private const int LastYear = 2018;

    #endregion

    #region Initialization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _provider = new Table_49_Electricity_Conversion_Defaults_Provider();
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
    public void GetElectricityConversionDataInstance()
    {
        var data = _provider.GetElectricityConversionData(1994, Province.Alberta);
        Assert.AreEqual(0.98, data.ElectricityValue);
    }

    [TestMethod]
    public void TestWrongYearElectricityDataInstance()
    {
        var data = _provider.GetElectricityConversionData(2075, Province.PrinceEdwardIsland);
        Assert.AreEqual(0, data.ElectricityValue);
    }

    [TestMethod]
    public void TestWrongProvinceElectricityDataInstance()
    {
        var data = _provider.GetElectricityConversionData(1994, Province.Yukon);
        Assert.AreEqual(0, data.ElectricityValue);
    }

    [TestMethod]
    public void TestAllWrongInputElectricityDataInstance()
    {
        var data = _provider.GetElectricityConversionData(2075, Province.Yukon);
        Assert.AreEqual(0, data.ElectricityValue);
    }


    [TestMethod]
    public void CheckStartYearLessThanData()
    {
        var averageElectricityValue = _provider.GetElectricityConversionValue(1985, Province.Ontario);
        Assert.AreEqual(0.166, averageElectricityValue);
    }

    [TestMethod]
    public void CheckEndYearGreaterThanData()
    {
        var averageElectricityValue = _provider.GetElectricityConversionValue(2019, Province.NovaScotia);
        Assert.AreEqual(0.694, averageElectricityValue);
    }

    [TestMethod]
    public void GetValueOfYearWithinRange()
    {
        var electricityValue = _provider.GetElectricityConversionValue(2005, Province.Saskatchewan);
        Assert.AreEqual(0.78, electricityValue);
    }

    [TestMethod]
    public void GetValueAtUpperBoundary()
    {
        var electricityValue = _provider.GetElectricityConversionValue(FirstYear, Province.Quebec);
        Assert.AreEqual(0.013, electricityValue);
    }

    [TestMethod]
    public void GetValueAtLowerBoundary()
    {
        var electricityValue = _provider.GetElectricityConversionValue(LastYear, Province.Alberta);
        Assert.AreEqual(0.63, electricityValue);
    }

    #endregion
}