using H.Core.Enumerations;
using H.Core.Providers.Plants;

namespace H.Core.Test.Providers.Plants;

[TestClass]
public class Table_9_Nitrogen_Lignin_Content_In_Crops_Provider_Test
{
    #region Fields

    private static Table_9_Nitrogen_Lignin_Content_In_Crops_Provider _provider;

    #endregion

    #region Initialization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _provider = new Table_9_Nitrogen_Lignin_Content_In_Crops_Provider();
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
    public void GetContentsInstance()
    {
        var data = _provider.GetDataByCropType(CropType.TameLegume);
        Assert.AreEqual(0.025, data.NitrogenContentResidues);
    }

    [TestMethod]
    public void GetInstanceWrongCrop()
    {
        var data = _provider.GetDataByCropType(CropType.KabuliChickpea);
        Assert.AreEqual(0, data.LigninContentResidues);
    }

    [TestMethod]
    public void CheckMoistureContent()
    {
        var data = _provider.GetDataByCropType(CropType.SugarBeets);
        Assert.AreEqual(80.0, data.MoistureContent);
    }

    [TestMethod]
    public void CheckLigninContentOfResidues()
    {
        var data = _provider.GetDataByCropType(CropType.BeansDryField);
        Assert.AreEqual(0.085, data.LigninContentResidues);
    }

    [TestMethod]
    public void CheckRootToShootRatioOfCrop()
    {
        var data = _provider.GetDataByCropType(CropType.Rice);
        Assert.AreEqual(0.0, data.RSTRatio);
    }

    [TestMethod]
    public void CheckSlopeValueOfCrop()
    {
        var data = _provider.GetDataByCropType(CropType.MixedGrains);
        Assert.AreEqual(0.029, data.SlopeValue);
    }

    [TestMethod]
    public void CheckInterceptValueOfCrop()
    {
        var data = _provider.GetDataByCropType(CropType.WhiteBeans);
        Assert.AreEqual(0.279, data.InterceptValue);
    }

    [TestMethod]
    public void GetBiomethaneData()
    {
        var data = _provider.GetAllCrops();
    }

    #endregion
}