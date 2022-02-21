using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Test.Models
{
    [TestClass]
    public class FarmTest
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

        #endregion
    }
}
