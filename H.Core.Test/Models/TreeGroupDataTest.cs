using System;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Shelterbelt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models
{
    [TestClass]
    public class TreeGroupDataTest
    {
        [TestMethod]
        public void ModifyingPlantYearUpdatesPlantDateToCorrectYear()
        {
            var a = new TreeGroupData();
            a.PlantYear = 1996;
            Assert.AreEqual(a.PlantDate.Year, 1996);
        }

        [TestMethod]
        public void ModifyingCutYearUpdatesCutDateToCorrectYear()
        {
            var a = new TreeGroupData();
            a.CutYear = 1996;
            Assert.AreEqual(a.CutDate.Year, 1996);
        }

        [TestMethod]
        [Ignore]
        public void ModifyingCutYearAndPlantYearCausesCorrectChangeInLifespan()
        {
            var a = new TreeGroupData();
            a.PlantYear = 2000;
            a.CutYear = 2001;
            Assert.AreEqual(365, a.Lifespan.TotalDays);
        }

        #region simple interactions

        private bool WithinTolerance(double a, double b)
        {
            return a < b + 0.00000000000001 && a > b - 0.00000000000001;
        }

        [TestMethod]
        public void ARetainsSetValue()
        {
            var a = new TreeGroupData();
            a.A = 5;
            Assert.AreEqual(a.A, 5);
        }

        [TestMethod]
        public void BRetainsSetValue()
        {
            var a = new TreeGroupData();
            a.B = 5;
            Assert.AreEqual(a.B, 5);
        }

        [TestMethod]
        public void PlantedTreeCountRetainsSetValue()
        {
            var a = new TreeGroupData();
            a.PlantedTreeCount = 5;
            Assert.AreEqual(a.PlantedTreeCount, 5);
        }

        [TestMethod]
        public void LiveTreeCountRetainsSetValue()
        {
            var a = new TreeGroupData();
            a.LiveTreeCount = 5;
            Assert.AreEqual(a.LiveTreeCount, 5);
        }

        [TestMethod]
        public void TreeSpeciesRetainsSetValue()
        {
            var a = new TreeGroupData();
            a.TreeSpecies = TreeSpecies.ManitobaMaple;
            Assert.AreEqual(a.TreeSpecies, TreeSpecies.ManitobaMaple);
        }

        [TestMethod]
        public void PlantedTreeSpacingRetainsSetValue()
        {
            var a = new TreeGroupData();
            a.PlantedTreeSpacing = 5;
            Assert.AreEqual(a.PlantedTreeSpacing, 5);
        }

        [TestMethod]
        public void LiveTreeSpacingRetainsSetValue()
        {
            var a = new TreeGroupData();
            a.LiveTreeSpacing = 5;
            Assert.AreEqual(a.LiveTreeSpacing, 5);
        }

        [TestMethod]
        public void PlantYearRetainsSetValue()
        {
            var a = new TreeGroupData();
            a.PlantYear = 5;
            Assert.AreEqual(a.PlantYear, 5);
        }

        [TestMethod]
        public void CutYearRetainsSetValue()
        {
            var a = new TreeGroupData();
            a.CutYear = 5;
            Assert.AreEqual(a.CutYear, 5);
        }

        [TestMethod]
        public void ParentRowDataRetainsSetValue()
        {
            var a = new TreeGroupData();
            var guid = Guid.NewGuid();
            a.ParentRowData = guid;
            Assert.AreEqual(a.ParentRowData, guid);
        }

        [TestMethod]
        public void GrandParentShelterbeltComponentRetainsSetValue()
        {
            var a = new TreeGroupData();
            var guid = Guid.NewGuid();
            a.GrandParentShelterbeltComponent = guid;
            Assert.AreEqual(a.GrandParentShelterbeltComponent, guid);
        }

        #endregion
    }
}