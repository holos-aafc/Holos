using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace H.Core.Test.Models.LandManagement.Fields
{
    [TestClass]
    public class HarvestViewItemTest
    {
        [TestMethod]
        public void BaleHasHasExpiredLifespanReturnsFalseForHayBales()
        {
            var viewItem = new HarvestViewItem();
            viewItem.ForageActivity = Enumerations.ForageActivities.Hayed;

            viewItem.Start = DateTime.Now.Subtract(TimeSpan.FromDays(30));

            var result = viewItem.BaleHasExpiredLifespan();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BaleHasHasExpiredLifespanReturnsTrueForHayBales()
        {
            var viewItem = new HarvestViewItem();
            viewItem.ForageActivity = Enumerations.ForageActivities.Hayed;

            viewItem.Start = DateTime.Now.Subtract(TimeSpan.FromDays(5 * 365 + 1)); // 1 day past expiry

            var result = viewItem.BaleHasExpiredLifespan();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BaleHasHasExpiredLifespanReturnsFalseForStrawBales()
        {
            var viewItem = new HarvestViewItem();
            viewItem.ForageActivity = Enumerations.ForageActivities.Silage;

            viewItem.Start = DateTime.Now.Subtract(TimeSpan.FromDays(30));

            var result = viewItem.BaleHasExpiredLifespan();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BaleHasHasExpiredLifespanReturnsTrueForStrawBales()
        {
            var viewItem = new HarvestViewItem();
            viewItem.ForageActivity = Enumerations.ForageActivities.Silage;

            viewItem.Start = DateTime.Now.Subtract(TimeSpan.FromDays(2 * 365 + 1)); // 1 day past expiry

            var result = viewItem.BaleHasExpiredLifespan();

            Assert.IsTrue(result);
        }
    }
}
