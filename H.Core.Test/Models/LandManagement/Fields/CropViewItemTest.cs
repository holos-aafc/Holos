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

        #endregion
    }
}
