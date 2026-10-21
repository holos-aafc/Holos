using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Test.Models.LandManagement.Fields
{
    [TestClass]
    public class CropViewItemTest
    {
        #region Fields

        private CropViewItem _sut;

        #endregion

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new CropViewItem
            {
                Year = 2025,
                GrazingViewItems = new ObservableCollection<GrazingViewItem>()
            };
        }

        #endregion

        #region Tests

        [TestMethod]
        public void HasGrazingItemsForTheCurrentYear_WhenNoGrazingItems_ReturnsFalse()
        {
            var result = _sut.HasGrazingItemsForTheCurrentYear();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasGrazingItemsForTheCurrentYear_WhenGrazingItemMatchesYear_ReturnsTrue()
        {
            _sut.GrazingViewItems.Add(new GrazingViewItem
            {
                Start = new DateTime(_sut.Year, 6, 1)
            });

            var result = _sut.HasGrazingItemsForTheCurrentYear();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasGrazingItemsForTheCurrentYear_WhenGrazingItemIsFromDifferentYear_ReturnsFalse()
        {
            _sut.GrazingViewItems.Add(new GrazingViewItem
            {
                Start = new DateTime(_sut.Year - 1, 6, 1)
            });

            var result = _sut.HasGrazingItemsForTheCurrentYear();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasGrazingItemsForTheCurrentYear_WhenMultipleItemsOnlyOneMatchesYear_ReturnsTrue()
        {
            _sut.GrazingViewItems.Add(new GrazingViewItem { Start = new DateTime(_sut.Year - 2, 1, 1) });
            _sut.GrazingViewItems.Add(new GrazingViewItem { Start = new DateTime(_sut.Year, 8, 15) });

            var result = _sut.HasGrazingItemsForTheCurrentYear();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasGrazingItemsForTheCurrentYear_WhenMultipleItemsNoneMatchYear_ReturnsFalse()
        {
            _sut.GrazingViewItems.Add(new GrazingViewItem { Start = new DateTime(_sut.Year - 1, 1, 1) });
            _sut.GrazingViewItems.Add(new GrazingViewItem { Start = new DateTime(_sut.Year + 1, 1, 1) });

            var result = _sut.HasGrazingItemsForTheCurrentYear();

            Assert.IsFalse(result);
        }


        /// <summary>
        /// The harvest tab quotes this figure as the yield the Details screen will show, so it has to be the number the
        /// results set - and asking for it must not set anything, because that screen only reports.
        /// </summary>
        [TestMethod]
        public void GetYieldFromHayHarvests_MatchesWhatCalculateSetsWithoutChangingTheItem()
        {
            _sut.Area = 10;
            _sut.MoistureContentOfCrop = 0.6;
            _sut.HarvestViewItems.Add(new HarvestViewItem
            {
                Start = new DateTime(_sut.Year, 8, 1),
                ForageActivity = ForageActivities.Hayed,
                TotalNumberOfBalesHarvested = 6,
                BaleWeight = 500,
                MoistureContentAsPercentage = 15,
            });

            var yieldBefore = _sut.Yield;
            var dryYieldBefore = _sut.DryYield;
            var totalBiomassBefore = _sut.TotalBiomassHarvest;

            var reported = _sut.GetYieldFromHayHarvests();

            Assert.AreEqual(yieldBefore, _sut.Yield, "asking for the yield changed it");
            Assert.AreEqual(dryYieldBefore, _sut.DryYield, "asking for the yield changed the dry yield");
            Assert.AreEqual(totalBiomassBefore, _sut.TotalBiomassHarvest, "asking for the yield changed the total biomass");

            Assert.IsTrue(_sut.CalculateYieldFromHayHarvests());

            // 3,000 kg baled at 15% moisture is 2,550 kg dry matter: 255 kg per hectare, or 637.5 kg of standing crop
            // at 60% moisture.
            Assert.AreEqual(637.5, reported, 0.0001);
            Assert.AreEqual(_sut.Yield, reported, 0.0001);
            Assert.AreEqual(255, _sut.DryYield, 0.0001);
            Assert.AreEqual(3000, _sut.TotalBiomassHarvest, 0.0001);
        }

        [TestMethod]
        public void GetYieldFromHayHarvests_WithNoUsableHarvest_ReturnsZero()
        {
            _sut.Area = 10;
            _sut.MoistureContentOfCrop = 0.6;
            _sut.HarvestViewItems.Add(new HarvestViewItem
            {
                Start = new DateTime(_sut.Year, 8, 1),
                ForageActivity = ForageActivities.Hayed,
            });

            Assert.AreEqual(0, _sut.GetYieldFromHayHarvests());
        }

        #endregion
    }
}
