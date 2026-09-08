using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;

namespace H.Core.Test.Models
{
    [TestClass]
    public class FarmTest : UnitTestBase
    {
        #region Tests
        
        [TestMethod]
        public void GetManureApplicationsForMonthTest()
        {
            var farm = new Farm();
            var canolaField = new FieldSystemComponent();
            var wheatField = new FieldSystemComponent();

            var wheatCrop = new CropViewItem();
            wheatField.CropViewItems.Add(wheatCrop);

            var manureApplicationForWheatField = new ManureApplicationViewItem();
            manureApplicationForWheatField.DateOfApplication = new DateTime(DateTime.Now.Year, 3, 10);
            manureApplicationForWheatField.AnimalType = AnimalType.Beef;
            manureApplicationForWheatField.Amount = 100;
            manureApplicationForWheatField.AmountOfManureAppliedPerHectare = 200;
            wheatCrop.ManureApplicationViewItems.Add(manureApplicationForWheatField);

            var canolaCrop = new CropViewItem();
            canolaField.CropViewItems.Add(canolaCrop);

            var manureApplicationForCanolaField = new ManureApplicationViewItem();
            manureApplicationForCanolaField.DateOfApplication = new DateTime(DateTime.Now.Year, 5, 15);
            manureApplicationForCanolaField.AnimalType = AnimalType.Sheep;
            canolaCrop.ManureApplicationViewItems.Add(manureApplicationForCanolaField);

            farm.Components.Add(canolaField);
            farm.Components.Add(wheatField);

            var result = farm.GetManureApplicationsForMonth(new MonthsAndDaysData() { StartDay = 9, Year = DateTime.Now.Year, Month = 3, DaysInMonth = 2 }, animalType: AnimalType.Beef);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TotalVolumeAppliedOfManureAppliedForMonth()
        {
            var farm = new Farm();
            var wheatField = new FieldSystemComponent();

            var wheatCrop = new CropViewItem();
            wheatCrop.Area = 5;
            wheatField.CropViewItems.Add(wheatCrop);

            farm.Components.Add(wheatField);

            var manureApplicationForWheatField = new ManureApplicationViewItem();
            manureApplicationForWheatField.DateOfApplication = new DateTime(DateTime.Now.Year, 3, 10);
            manureApplicationForWheatField.AnimalType = AnimalType.Beef;
            manureApplicationForWheatField.AmountOfManureAppliedPerHectare = 100;

            wheatCrop.ManureApplicationViewItems.Add(manureApplicationForWheatField);

            var monthsAndDaysData = new MonthsAndDaysData() { StartDay = 9, Year = DateTime.Now.Year, Month = 3, DaysInMonth = 2 };

            var result = farm.GetTotalVolumeAppliedOfManureAppliedForMonth(monthsAndDaysData, AnimalType.Beef);

            Assert.AreEqual(500, result);
        }

        [TestMethod]
        public void GetHayImportsUsingImportedHayFromSourceFieldReturnsEmptyList()
        {
            var farm = new Farm();

            var field = new FieldSystemComponent();

            var result = farm.GetHayImportsUsingImportedHayFromSourceField(field);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetHayImportsUsingImportedHayFromSourceFieldReturnsCorrectCount()
        {
            var farm = new Farm();

            var field = new FieldSystemComponent();
            field.Guid = Guid.NewGuid(); ;

            var cropViewItem = new CropViewItem();

            var hayImport = new HayImportViewItem();
            hayImport.FieldSourceGuid = field.Guid;

            cropViewItem.HayImportViewItems.Add(hayImport);
            field.CropViewItems.Add(cropViewItem);
            farm.Components.Add(field);

            var result = farm.GetHayImportsUsingImportedHayFromSourceField(field);

            Assert.AreEqual(1, result.Count);

            cropViewItem.HayImportViewItems.Add(hayImport);

            Assert.AreEqual(2, farm.GetHayImportsUsingImportedHayFromSourceField(field).Count);
        }

        /// <summary>
        /// Builds the smallest farm that satisfies IsNonSwathingGrazingScenario's other conditions, so each test below
        /// varies only the yield assignment method.
        /// </summary>
        private static Farm CreateGrazedFieldFarm(out CropViewItem viewItem, out FieldSystemComponent field)
        {
            var farm = new Farm();
            field = new FieldSystemComponent();

            viewItem = new CropViewItem()
            {
                CropType = CropType.TameGrass,
                Year = 1985,
                FieldSystemComponentGuid = field.Guid,
                TotalCarbonLossesByGrazingAnimals = 500,
            };
            viewItem.GrazingViewItems.Add(new GrazingViewItem() {Start = new DateTime(1985, 6, 1)});

            field.CropViewItems.Add(viewItem);
            farm.Components.Add(field);

            return farm;
        }

        [TestMethod]
        public void IsNonSwathingGrazingScenarioUsesFieldLevelMethodWhenEnabled()
        {
            // Field-level assignment on: the field says Custom, so the grazed field must take the custom path even
            // though the farm as a whole is set to a modelled method.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;
            farm.UseFieldLevelYieldAssignement = true;
            field.YieldAssignmentMethod = YieldAssignmentMethod.Custom;

            Assert.IsFalse(farm.IsNonSwathingGrazingScenario(viewItem));
        }

        [TestMethod]
        public void IsNonSwathingGrazingScenarioUsesFieldLevelModelledMethodWhenEnabled()
        {
            // The mirror case: the farm is Custom but this field is modelled, so the DMI-derived path applies.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            farm.UseFieldLevelYieldAssignement = true;
            field.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;

            Assert.IsTrue(farm.IsNonSwathingGrazingScenario(viewItem));
        }

        [TestMethod]
        public void IsNonSwathingGrazingScenarioFallsBackToFarmMethodWhenFieldLevelDisabled()
        {
            // Regression guard for the common case: with field-level assignment off, the field's own method is ignored
            // and the farm's method decides, exactly as before.
            var farm = CreateGrazedFieldFarm(out var viewItem, out var field);
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            farm.UseFieldLevelYieldAssignement = false;
            field.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;

            Assert.IsFalse(farm.IsNonSwathingGrazingScenario(viewItem));
        }

        #endregion
    }
}
