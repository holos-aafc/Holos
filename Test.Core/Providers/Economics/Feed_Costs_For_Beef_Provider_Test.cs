using H.Core.Enumerations;
using H.Core.Providers.Economics;

namespace H.Core.Test.Providers.Economics;

[TestClass]
public class Feed_Costs_For_Beef_Provider_Test
{
    #region Fields

    private Feed_Costs_For_Beef_Provider _provider;

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
        _provider = new Feed_Costs_For_Beef_Provider();
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Test Methods

    [TestMethod]
    public void GetFeedCostsData()
    {
        var data = _provider.FeedCostsForBeefData;

        Assert.AreEqual(7, data.Count);
    }

    [TestMethod]
    public void GetFeedCostDataByDietType()
    {
        var data = _provider.GetFeedCostByDietType(DietType.MediumGrowth);
        Assert.AreEqual(0.133, data.Cost);

        data = _provider.GetFeedCostByDietType(DietType.Corn);
        Assert.AreEqual(0.198, data.Cost);
    }

    [TestMethod]
    public void TestIncorrectDietInput()
    {
        var data = _provider.GetFeedCostByDietType(DietType.FarOffDry);
        Assert.IsNull(data);
    }

    #endregion
}