using H.Core.Enumerations;
using H.Core.Providers.Animals.Table_69;
using H.Core.Providers.Animals.Table_70;

namespace H.Core.Test.Providers.Animals;

[TestClass]
public class Table_62_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider_Test
{
    #region Fields

    private IVolatilizationFractionsFromLandAppliedManureProvider _sut;

    #endregion

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
        _sut = new Table_62_Volatilization_Fractions_From_Land_Applied_Swine_Manure_Provider();
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void GetDataReturnsZeroWhenIncorrectAnimalTypeIsUsed()
    {
        var result = _sut.GetData(AnimalType.Dairy, Province.Alberta, 2000);

        Assert.AreEqual(0, result.ImpliedEmissionFactor);
    }

    [TestMethod]
    public void GetDataReturnsNonZeroWhenForProvinceAndYear()
    {
        var result = _sut.GetData(AnimalType.Swine, Province.Quebec, 2017);

        Assert.AreEqual(0.24, result.ImpliedEmissionFactor);
    }

    #endregion
}