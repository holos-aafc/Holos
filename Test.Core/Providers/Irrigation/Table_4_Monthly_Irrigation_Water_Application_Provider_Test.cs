using H.Core.Enumerations;
using H.Core.Providers.Irrigation;

namespace H.Core.Test.Providers.Irrigation;

[TestClass]
public class Table_4_Monthly_Irrigation_Water_Application_Provider_Test
{
    #region Fields

    private static Table_4_Monthly_Irrigation_Water_Application_Provider _waterApplicationProvider;

    #endregion

    #region Initalization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _waterApplicationProvider = new Table_4_Monthly_Irrigation_Water_Application_Provider();
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
    public void GetMonthlyAverageIrrigationDataTest()
    {
        var irrigationWaterApplicationData =
            _waterApplicationProvider.GetMonthlyAverageIrrigationDataInstance(Months.April, Province.BritishColumbia);

        Assert.AreEqual(8, irrigationWaterApplicationData.IrrigationVolume);
    }

    [TestMethod]
    public void CheckNullReturnWrongMonth()
    {
        var irrigationWaterApplicationData =
            _waterApplicationProvider.GetMonthlyAverageIrrigationDataInstance(Months.January, Province.NewBrunswick);
        Assert.AreEqual(null, irrigationWaterApplicationData);
    }

    [TestMethod]
    public void CheckNullReturnWrongProvince()
    {
        var irrigationWaterApplicationData =
            _waterApplicationProvider.GetMonthlyAverageIrrigationDataInstance(Months.April, Province.Yukon);
        Assert.AreEqual(null, irrigationWaterApplicationData);
    }

    [TestMethod]
    public void CheckAllWrongInput()
    {
        var irrigationWaterApplicationData =
            _waterApplicationProvider.GetMonthlyAverageIrrigationDataInstance(Months.February, Province.Yukon);
        Assert.AreEqual(null, irrigationWaterApplicationData);
    }

    #endregion
}