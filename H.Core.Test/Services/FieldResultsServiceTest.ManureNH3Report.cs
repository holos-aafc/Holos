using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services
{
    public partial class FieldResultsServiceTest
    {
        #region Manure land-application NH3-N emission factor report

        private static ManureApplicationViewItem PoultryApplication(int month, double nitrogenPerHectare)
        {
            return new ManureApplicationViewItem
            {
                AnimalType = AnimalType.Broilers,
                ManureLocationSourceType = ManureLocationSourceType.Livestock,
                ManureStateType = ManureStateType.Solid,
                DateOfApplication = new System.DateTime(2024, month, 15),
                AmountOfNitrogenAppliedPerHectare = nitrogenPerHectare,
            };
        }

        /// <summary>The report gives every manure application its own row, so several applications on one field-year are not merged.</summary>
        [TestMethod]
        public void ManureReport_WritesOneRowPerApplication()
        {
            // No rain, so the poultry EF is the base tillage/month value with no modifier.
            _n2OEmissionFactorCalculator.ClimateProvider = base._mockClimateProviderObject;

            var farm = new Farm { Name = "Test farm" };
            var viewItem = new CropViewItem
            {
                Name = "Barley",
                Year = 2024,
                Area = 10,
                TillageType = TillageType.Reduced,
            };
            viewItem.ManureApplicationViewItems.Add(PoultryApplication(month: 5, nitrogenPerHectare: 30));  // May  -> 0.42
            viewItem.ManureApplicationViewItems.Add(PoultryApplication(month: 9, nitrogenPerHectare: 40));  // Sept -> 0.47

            var report = _resultsService.BuildManureLandApplicationEmissionFactorReport(
                new[] { viewItem }, farm, CultureInfo.InvariantCulture);

            var lines = report.Split('\n').Where(x => x.Trim().Length > 0).ToList();

            // sep line + header + two data rows.
            var dataRows = lines.Where(x => x.Contains("Test farm")).ToList();
            Assert.AreEqual(2, dataRows.Count, "each application must have its own row");
        }

        [TestMethod]
        public void ManureReport_ReportsTheFinalEmissionFactorForEachApplication()
        {
            _n2OEmissionFactorCalculator.ClimateProvider = base._mockClimateProviderObject;

            var farm = new Farm { Name = "Test farm" };
            var viewItem = new CropViewItem
            {
                Name = "Barley",
                Year = 2024,
                Area = 10,
                TillageType = TillageType.Reduced,
            };
            viewItem.ManureApplicationViewItems.Add(PoultryApplication(month: 5, nitrogenPerHectare: 30));  // May  -> 0.42
            viewItem.ManureApplicationViewItems.Add(PoultryApplication(month: 9, nitrogenPerHectare: 40));  // Sept -> 0.47

            var report = _resultsService.BuildManureLandApplicationEmissionFactorReport(
                new[] { viewItem }, farm, CultureInfo.InvariantCulture);

            var dataRows = report.Split('\n').Where(x => x.Contains("Test farm")).ToList();

            var mayRow = dataRows.Single(x => x.Contains("2024-05-15"));
            var septemberRow = dataRows.Single(x => x.Contains("2024-09-15"));

            // Poultry EF acts on TAN, so the basis column must say so, and the final EF must match the box values.
            Assert.IsTrue(mayRow.Contains("0.4200"), "May application (tilled, May-Aug) must report 0.42");
            Assert.IsTrue(septemberRow.Contains("0.4700"), "September application (tilled, Apr/Sep/Oct) must report 0.47");
            Assert.IsTrue(mayRow.Contains(Properties.Resources.LabelPerKilogramTan),
                "a poultry emission factor is applied per kg TAN");
        }

        [TestMethod]
        public void ManureReport_HasAHeaderAndNoDataRowsWhenThereAreNoApplications()
        {
            var farm = new Farm { Name = "Test farm" };
            var viewItem = new CropViewItem { Name = "Barley", Year = 2024, Area = 10 };

            var report = _resultsService.BuildManureLandApplicationEmissionFactorReport(
                new[] { viewItem }, farm, CultureInfo.InvariantCulture);

            Assert.IsTrue(report.Contains(Properties.Resources.LabelFinalAmmoniaEmissionFactorForLandApplication),
                "the header must always be written");
            Assert.IsFalse(report.Contains("Test farm"),
                "a field-year with no manure applications produces no rows");
        }

        #endregion
    }
}
