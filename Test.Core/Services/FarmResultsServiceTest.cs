using H.Core.Calculators.Infrastructure;
using H.Core.Services;
using H.Core.Services.Animals;
using Moq;

namespace H.Core.Test.Services;

[TestClass]
public class FarmResultsServiceTest : UnitTestBase
{
    #region Fields

    private FarmResultsService _farmResultsService;

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
        _farmResultsService = new FarmResultsService(new EventAggregator(), _fieldResultsService, new ADCalculator(),
            new Mock<IManureService>().Object, new Mock<IAnimalService>().Object, _n2OEmissionFactorCalculator);
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion
}