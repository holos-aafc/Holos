using H.Core.Models;
using H.Core.Models.LandManagement.Shelterbelt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models
{
    [TestClass]
    public class ShelterbeltComponentTest
    {
        #region Mirros Year of Observation

        [TestMethod]
        [Ignore] // Ignore
        public void YearOfObservationConsistentOnInitialization()
        {
            ShelterbeltComponent shelterbelt = new ShelterbeltComponent();
            var row = shelterbelt.NewRowData();
            var treegroup = row.NewTreeGroupData();
            shelterbelt.BuildTrannums();
            bool one = true;
            one &= shelterbelt.YearOfObservation == row.YearOfObservation;
            one &= row.YearOfObservation == treegroup.YearOfObservation;
            one &= shelterbelt.TrannumData[0].YearOfObservation == shelterbelt.YearOfObservation;
            Assert.AreEqual(true, one);
        }

        [TestMethod]
        [Ignore] // Takes too long
        public void YearOfObservationConsistentOnChange()
        {
            ShelterbeltComponent shelterbelt = new ShelterbeltComponent();
            var row = shelterbelt.NewRowData();
            var treegroup = row.NewTreeGroupData();
            shelterbelt.BuildTrannums();
            shelterbelt.YearOfObservation = 10;
            bool one = true;
            one &= shelterbelt.YearOfObservation == row.YearOfObservation;
            one &= row.YearOfObservation == treegroup.YearOfObservation;
            //Trannums MUST not change...
            one &= shelterbelt.TrannumData[0].YearOfObservation != shelterbelt.YearOfObservation;
            Assert.AreEqual(true, one);
        }

        [TestMethod]
        [Ignore] // Takes too long
        public void YearOfObservationConsistentWhenCreatingNewStuff()
        {
            ShelterbeltComponent shelterbelt = new ShelterbeltComponent();
            shelterbelt.YearOfObservation = 15;
            var row = shelterbelt.NewRowData();
            var treegroup = row.NewTreeGroupData();
            shelterbelt.BuildTrannums();
            bool one = true;
            one &= shelterbelt.YearOfObservation == row.YearOfObservation;
            one &= row.YearOfObservation == treegroup.YearOfObservation;
            one &= shelterbelt.TrannumData[0].YearOfObservation == shelterbelt.YearOfObservation;
            Assert.AreEqual(true, one);
        }

        #endregion

        #region simple interactions

        [TestMethod]
        public void ShelterbeltComponentConstructorProducesEmptyRowDataCollection()
        {
            var a = new ShelterbeltComponent();
            Assert.IsNotNull(a.RowData);
            Assert.AreEqual(a.RowData.Count, 0);
        }

        [TestMethod]
        public void CanAddNewTreeGroups()
        {
            var a = new ShelterbeltComponent();
            var b = a.NewRowData();
            Assert.AreEqual(a.RowData.Count, 1);
        }

        [TestMethod]
        public void CanRemoveRowData()
        {
            var a = new ShelterbeltComponent();
            var c = a.NewRowData();
            var b = a.NewRowData();
            a.RemoveRowData(c);
            Assert.IsTrue(a.RowData.Contains(b));
            Assert.AreEqual(a.RowData.Count, 1);
        }

        #endregion
    }
}