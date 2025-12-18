using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Test.Models.LandManagement.Fields;

[TestClass]
public class HarvestViewItemTest
{
    #region Tests

    [TestMethod]
    public void CalculateDryMatterOfBalesFiftyPercent()
    {
        var viewItem = new HarvestViewItem();
        viewItem.MoistureContentAsPercentage = 50;
        viewItem.BaleWeight = 1000;
        viewItem.TotalNumberOfBalesHarvested = 1;

        Assert.AreEqual(500, viewItem.AboveGroundBiomassDryWeight);
    }

    [TestMethod]
    public void CalculateDryMatterOfBalesTwentyFivePercent()
    {
        var viewItem = new HarvestViewItem();
        viewItem.MoistureContentAsPercentage = 25;
        viewItem.BaleWeight = 1000;
        viewItem.TotalNumberOfBalesHarvested = 1;

        Assert.AreEqual(750, viewItem.AboveGroundBiomassDryWeight);
    }

    [TestMethod]
    public void CalculateWetWeightOfBales()
    {
        var viewItem = new HarvestViewItem();
        viewItem.BaleWeight = 1000;
        viewItem.TotalNumberOfBalesHarvested = 1;

        Assert.AreEqual(1000, viewItem.AboveGroundBiomass);
    }

    [TestMethod]
    public void CalculateWetWeightOfTwoBales()
    {
        var viewItem = new HarvestViewItem();
        viewItem.BaleWeight = 1000;
        viewItem.TotalNumberOfBalesHarvested = 2;

        Assert.AreEqual(2000, viewItem.AboveGroundBiomass);
    }

    [TestMethod]
    public void BaleHasHasExpiredLifespanReturnsFalseForHayBales()
    {
        var viewItem = new HarvestViewItem();
        viewItem.ForageActivity = ForageActivities.Hayed;

        viewItem.Start = DateTime.Now.Subtract(TimeSpan.FromDays(30));

        var result = viewItem.BaleHasExpiredLifespan();

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void BaleHasHasExpiredLifespanReturnsTrueForHayBales()
    {
        var viewItem = new HarvestViewItem();
        viewItem.ForageActivity = ForageActivities.Hayed;

        viewItem.Start = DateTime.Now.Subtract(TimeSpan.FromDays(5 * 365 + 1)); // 1 day past expiry

        var result = viewItem.BaleHasExpiredLifespan();

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void BaleHasHasExpiredLifespanReturnsFalseForStrawBales()
    {
        var viewItem = new HarvestViewItem();
        viewItem.ForageActivity = ForageActivities.Silage;

        viewItem.Start = DateTime.Now.Subtract(TimeSpan.FromDays(30));

        var result = viewItem.BaleHasExpiredLifespan();

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void BaleHasHasExpiredLifespanReturnsTrueForStrawBales()
    {
        var viewItem = new HarvestViewItem();
        viewItem.ForageActivity = ForageActivities.Silage;

        viewItem.Start = DateTime.Now.Subtract(TimeSpan.FromDays(2 * 365 + 1)); // 1 day past expiry

        var result = viewItem.BaleHasExpiredLifespan();

        Assert.IsTrue(result);
    }

    #endregion
}