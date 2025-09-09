using H.Core.Calculators.Climate;

namespace H.Core.Test.Calculators.Climate;

[TestClass]
public class ClimateServiceTest
{
    #region Fields

    private IClimateService _climateService;

    #endregion

    #region Tests

    [TestMethod]
    public void TestMethod1()
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
        _climateService = new ClimateService();
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion
}