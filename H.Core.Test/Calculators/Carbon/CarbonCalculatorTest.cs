using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Calculators.Carbon;

namespace H.Core.Test.Calculators.Carbon
{
    [TestClass]
    public class CarbonCalculatorTest : UnitTestBase
    {
        #region Fields

        private ICarbonService _sut;

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
            _sut = new CarbonService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void CalculateCarbonLostFromHayExport()
        {
            var fieldId = Guid.NewGuid();

            var hayExportViewItem = new CropViewItem();
            hayExportViewItem.Year = DateTime.Now.Year;
            hayExportViewItem.PercentageOfProductYieldReturnedToSoil = 2;
            hayExportViewItem.HarvestViewItems.Add(new HarvestViewItem() { TotalNumberOfBalesHarvested = 20, Start = DateTime.Now });

            var exportingFieldComponent = new FieldSystemComponent();
            exportingFieldComponent.Guid = fieldId;
            exportingFieldComponent.Name = "Exporting field";
            exportingFieldComponent.CropViewItems.Add(hayExportViewItem);

            var importingViewItem = new CropViewItem();
            importingViewItem.HayImportViewItems.Add(new HayImportViewItem() { Date = DateTime.Now, FieldSourceGuid = fieldId, NumberOfBales = 5, MoistureContentAsPercentage = 20 });

            var importingFieldComponent = new FieldSystemComponent();
            importingFieldComponent.Name = "Importing field";
            importingFieldComponent.CropViewItems.Add(importingViewItem);

            var farm = new Farm();

            farm.Components.Add(importingFieldComponent);
            farm.Components.Add(exportingFieldComponent);

            _sut.CalculateCarbonLostFromHayExports(exportingFieldComponent, farm);

            Assert.AreEqual(5.5102040816326534, hayExportViewItem.TotalCarbonLossFromBaleExports);
        }
    }
}
