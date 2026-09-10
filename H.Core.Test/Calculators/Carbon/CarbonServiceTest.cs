using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using System.Collections.Generic;

namespace H.Core.Test.Calculators.Carbon
{
    [TestClass]
    public class CarbonServiceTest : UnitTestBase
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

        #region Tests

        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void CalculateCarbonLostFromHayExportForField()
        {
            var fieldId = Guid.NewGuid();

            var hayExportViewItem = new CropViewItem();
            hayExportViewItem.FieldSystemComponentGuid = fieldId;
            hayExportViewItem.Year = DateTime.Now.Year;
            hayExportViewItem.PercentageOfProductYieldReturnedToSoil = 2;
            hayExportViewItem.HarvestViewItems.Add(new HarvestViewItem() { TotalNumberOfBalesHarvested = 20, Start = DateTime.Now });

            var exportingFieldComponent = new FieldSystemComponent();
            exportingFieldComponent.Guid = fieldId;
            exportingFieldComponent.Name = "Exporting field";
            exportingFieldComponent.CropViewItems.Add(hayExportViewItem);

            var importingViewItem = new CropViewItem();
            importingViewItem.Year = DateTime.Now.Year;
            importingViewItem.HayImportViewItems.Add(new HayImportViewItem() { Start = DateTime.Now, FieldSourceGuid = fieldId, NumberOfBales = 5, MoistureContentAsPercentage = 20, BaleWeight = 100});

            var importingFieldComponent = new FieldSystemComponent();
            importingFieldComponent.Guid = Guid.NewGuid();
            importingFieldComponent.Name = "Importing field";
            importingFieldComponent.CropViewItems.Add(importingViewItem);
            importingViewItem.FieldSystemComponentGuid = importingFieldComponent.Guid;

            var farm = new Farm();

            farm.Components.Add(importingFieldComponent);
            farm.Components.Add(exportingFieldComponent);

            _sut.CalculateCarbonLostFromHayExports(farm, hayExportViewItem);

            // Amount of hay harvested is 10,000 kg, amount imported onto another field is 480 kg. Carbon lost is (10000 - 480) * 0.45
            Assert.AreEqual((10000 - 480) * 0.45, hayExportViewItem.TotalCarbonLossFromBaleExports);
        }

        [TestMethod]
        public void CalculateTotalDryWeightLossFromResiduesTestReturnsNonZeroWhenExportingFieldIsTheSameAsImportingField()
        {
            var field = new FieldSystemComponent();
            field.Guid = Guid.NewGuid();

            var farm = new Farm();

            var hayHarvest = new HarvestViewItem();
            hayHarvest.Start = DateTime.Now;
            hayHarvest.TotalNumberOfBalesHarvested = 20;
            hayHarvest.MoistureContentAsPercentage = 50;
            hayHarvest.BaleWeight = 100;

            var exportingCropViewItem = new CropViewItem();
            exportingCropViewItem.FieldSystemComponentGuid = field.Guid;
            exportingCropViewItem.Year = DateTime.Now.Year;
            exportingCropViewItem.HarvestViewItems.Add(hayHarvest);

            field.CropViewItems.Add(exportingCropViewItem);

            farm.Components.Add(field);

            var hayImportViewItem = new HayImportViewItem();
            hayImportViewItem.Start = DateTime.Now;
            hayImportViewItem.FieldSourceGuid = field.Guid;
            hayImportViewItem.BaleWeight = 100;
            hayImportViewItem.NumberOfBales = 10;
            hayImportViewItem.MoistureContentAsPercentage = 50;

            exportingCropViewItem.HayImportViewItems.Add(hayImportViewItem);

            var result = _sut.CalculateTotalDryMatterLossFromResidueExports(exportingCropViewItem, farm);

            // Total harvested amount is 1000 kg. Total imported onto same field is 750 kg. Total residues exported off farm is 250 kg
            Assert.AreEqual(250, result);
        }

        [TestMethod]
        public void CalculateTotalDryWeightLossFromResiduesTestReturnsNonZeroWhenExportingFieldIsNotTheSameAsImportingField()
        {
            var exportingField = new FieldSystemComponent();
            exportingField.Guid = Guid.NewGuid();

            var farm = new Farm();

            var hayHarvest = new HarvestViewItem();
            hayHarvest.Start = DateTime.Now;
            hayHarvest.TotalNumberOfBalesHarvested = 20;
            hayHarvest.MoistureContentAsPercentage = 50;
            hayHarvest.BaleWeight = 100;

            var exportingCropViewItem = new CropViewItem();
            exportingCropViewItem.FieldSystemComponentGuid = exportingField.Guid;
            exportingCropViewItem.Year = DateTime.Now.Year;
            exportingCropViewItem.HarvestViewItems.Add(hayHarvest);

            exportingField.CropViewItems.Add(exportingCropViewItem);

            farm.Components.Add(exportingField);

            var hayImportViewItem = new HayImportViewItem();
            hayImportViewItem.Start = DateTime.Now;
            hayImportViewItem.FieldSourceGuid = exportingField.Guid;
            hayImportViewItem.BaleWeight = 20;
            hayImportViewItem.NumberOfBales = 10;
            hayImportViewItem.MoistureContentAsPercentage = 50;

            var importingField = new FieldSystemComponent();
            importingField.Guid = Guid.NewGuid();
            farm.Components.Add(importingField);

            var importingViewItem = new CropViewItem();
            importingViewItem.FieldSystemComponentGuid = importingField.Guid;
            importingField.CropViewItems.Add(importingViewItem);

            importingViewItem.HayImportViewItems.Add(hayImportViewItem);

            var result = _sut.CalculateTotalDryMatterLossFromResidueExports(exportingCropViewItem, farm);

            // Total harvested amount is 1000 kg. Total imported onto a different field is 150 kg. Total residues exported off farm is 250 kg
            Assert.AreEqual(850, result);
        }

        /// <summary>
        /// The real-world shape of a hay import: the user sets Date, and nothing ever assigns Start. The netting used
        /// to filter on Start, which therefore sat at DateTime.MinValue and matched no simulation year, so hay fed back
        /// onto the field it was cut from was still counted as having left the farm.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDryMatterLossFromResidueExportsNetsImportThatHasNoStartDateAssigned()
        {
            var farm = BuildFarmWithHayCut(out var field, out var exportingCropViewItem);
            var withoutImport = _sut.CalculateTotalDryMatterLossFromResidueExports(exportingCropViewItem, farm);

            var hayImportViewItem = new HayImportViewItem
            {
                FieldSourceGuid = field.Guid,
                BaleWeight = 100,
                NumberOfBales = 10,
                MoistureContentAsPercentage = 50,
                Date = new DateTime(exportingCropViewItem.Year, 10, 30),
            };

            Assert.AreEqual(1, hayImportViewItem.Start.Year, "this test is only meaningful while Start is left unassigned");

            exportingCropViewItem.HayImportViewItems.Add(hayImportViewItem);

            var withImport = _sut.CalculateTotalDryMatterLossFromResidueExports(exportingCropViewItem, farm);

            Assert.IsTrue(withImport < withoutImport,
                $"hay fed back onto its own field must reduce what is counted as exported ({withImport} vs {withoutImport})");
        }

        /// <summary>
        /// The import's own date decides which year it is netted against. Repeating management is already expanded
        /// into one copy per year before this runs, so matching on the repeat flag as well would count every year's
        /// copy against every year.
        /// </summary>
        [TestMethod]
        public void CalculateTotalDryMatterLossFromResidueExportsIgnoresImportFromAnotherYear()
        {
            var farm = BuildFarmWithHayCut(out var field, out var exportingCropViewItem);
            var withoutImport = _sut.CalculateTotalDryMatterLossFromResidueExports(exportingCropViewItem, farm);

            var hayImportViewItem = new HayImportViewItem
            {
                FieldSourceGuid = field.Guid,
                BaleWeight = 100,
                NumberOfBales = 10,
                MoistureContentAsPercentage = 50,
                Date = new DateTime(exportingCropViewItem.Year - 3, 10, 30),
            };

            exportingCropViewItem.HayImportViewItems.Add(hayImportViewItem);

            Assert.AreEqual(withoutImport, _sut.CalculateTotalDryMatterLossFromResidueExports(exportingCropViewItem, farm));

            // ... and is netted once it is dated to the year being calculated.
            hayImportViewItem.Date = new DateTime(exportingCropViewItem.Year, 10, 30);

            Assert.IsTrue(_sut.CalculateTotalDryMatterLossFromResidueExports(exportingCropViewItem, farm) < withoutImport);
        }

        private static Farm BuildFarmWithHayCut(out FieldSystemComponent field, out CropViewItem exportingCropViewItem)
        {
            field = new FieldSystemComponent { Guid = Guid.NewGuid() };

            exportingCropViewItem = new CropViewItem
            {
                FieldSystemComponentGuid = field.Guid,
                Year = DateTime.Now.Year,
            };

            exportingCropViewItem.HarvestViewItems.Add(new HarvestViewItem
            {
                Start = new DateTime(exportingCropViewItem.Year, 8, 3),
                TotalNumberOfBalesHarvested = 20,
                MoistureContentAsPercentage = 50,
                BaleWeight = 100,
            });

            field.CropViewItems.Add(exportingCropViewItem);

            var farm = new Farm();
            farm.Components.Add(field);

            return farm;
        }

        #endregion
    }
}
