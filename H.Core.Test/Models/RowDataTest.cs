using System;
using H.Core.Models.LandManagement.Shelterbelt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models
{
    [TestClass]
    public class RowDataTest
    {
        #region simple interactions

        [TestMethod]
        public void LengthRetainsSetValue()
        {
            var a = new RowData();
            a.Length = 5;
            Assert.AreEqual(a.Length, 5);
        }

        [TestMethod]
        public void ParentRowDataRetainsSetValue()
        {
            var a = new RowData();
            var guid = Guid.NewGuid();
            a.ParentShelterbeltComponent = guid;
            Assert.AreEqual(a.ParentShelterbeltComponent, guid);
        }

        [TestMethod]
        public void RowDataConstructorProducesEmptyTreeGroupDataCollection()
        {
            var a = new RowData();
            Assert.IsNotNull(a.TreeGroupData);
            Assert.AreEqual(a.TreeGroupData.Count, 0);
        }

        [TestMethod]
        public void CanAddNewTreeGroups()
        {
            var a = new RowData();
            var b = a.NewTreeGroupData();
            Assert.AreEqual(a.TreeGroupData.Count, 1);
        }

        [TestMethod]
        public void CanRemoveTreeGroupData()
        {
            var a = new RowData();
            var c = a.NewTreeGroupData();
            var b = a.NewTreeGroupData();
            a.RemoveTreeGroupData(c);
            Assert.IsTrue(a.TreeGroupData.Contains(b));
            Assert.AreEqual(a.TreeGroupData.Count, 1);
        }

        #endregion
    }
}