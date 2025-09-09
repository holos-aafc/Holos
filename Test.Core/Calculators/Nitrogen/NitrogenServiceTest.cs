using H.Core.Calculators.Nitrogen.NitrogenService;

namespace H.Core.Test.Calculators.Nitrogen;

[TestClass]
public class NitrogenServiceTest : UnitTestBase
{
    #region Fields

    private INitrogenService _sut;

    #endregion

    #region Tests

    [TestMethod]
    public void CalculateCropResidueExportNitrogen()
    {
    }

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
        _sut = new NitrogenService();
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion
}