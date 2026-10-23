using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services.Animals
{
    /// <summary>
    /// The IPCC Tier 2 run-in period's average inputs set the starting soil carbon. Its yields are assigned by
    /// FieldResultsService.AssignRunInPeriodYields, according to Defaults.RunInPeriodYieldSource. Farm4 is switched to
    /// Small Area Data so that a harvested field's own years and the regional estimate differ.
    /// </summary>
    [TestClass]
    public class RunInPeriodYieldTest : UnitTestBase
    {
        #region Tests

        /// <summary>
        /// A field whose history records hay harvests: by default the run-in years take the harvested yield the
        /// simulation's own years use, so soil carbon does not start from regional productivity and then move away
        /// from it. The other setting keeps the regional estimate.
        /// </summary>
        [TestMethod]
        public void RunInYearsTakeTheHarvestedYieldByDefaultAndTheEstimateOtherwise()
        {
            RunInFor(RunInPeriodYieldSource.FieldHistory, out var rows, out var runInItems, out _, out _);
            var harvested = WithSource(runInItems, rows).Where(x => IsFromHarvest(x.Item2)).ToList();

            Assert.IsTrue(harvested.Any(), "Farm4 no longer has a harvested perennial, so this test shows nothing");
            foreach (var x in harvested)
            {
                Assert.AreEqual(x.Item2.Yield, x.Item1.Yield, 1e-9, $"run-in {x.Item1.Year} did not take the harvested yield of {x.Item2.Year}");
            }

            RunInFor(RunInPeriodYieldSource.YieldAssignmentMethod, out var regionalRows, out var regionalItems, out _, out _);
            Assert.IsTrue(WithSource(regionalItems, regionalRows).Where(x => IsFromHarvest(x.Item2)).Any(x => Math.Abs(x.Item1.Yield - x.Item2.Yield) > 1),
                "the yield assignment method gave the harvested yield too, so the two settings were not told apart");
        }

        /// <summary>
        /// Where a run-in year repeats a field history year whose yield is the method's own estimate, it keeps the
        /// estimate for its own year rather than taking the history year's.
        /// </summary>
        [TestMethod]
        public void RunInYearsWithAnEstimatedYieldKeepTheirOwnYearsEstimate()
        {
            RunInFor(RunInPeriodYieldSource.FieldHistory, out var rows, out var runInItems, out var farm, out var service);

            var estimated = WithSource(runInItems, rows)
                .Where(x => x.Item2.DoNotRecalculateYield == false &&
                            YieldInputPolicy.GetYieldSource(x.Item2, YieldAssignmentMethod.SmallAreaData) == YieldSource.Estimated)
                .ToList();

            Assert.IsTrue(estimated.Any(), "Farm4 has no estimated-yield field, so this test shows nothing");

            foreach (var x in estimated)
            {
                var field = farm.GetFieldSystemComponent(x.Item1.FieldSystemComponentGuid);
                var ownYear = service.MapDetailsScreenViewItemFromComponentScreenViewItem(x.Item2, x.Item1.Year);
                ownYear.Year = x.Item1.Year;
                base._initializationService.InitializeYieldForAllYears(new List<CropViewItem>() { ownYear }, farm, field);

                Assert.AreEqual(ownYear.Yield, x.Item1.Yield, 1e-9, $"run-in {x.Item1.Year} {x.Item1.CropType} did not keep its own year's estimate");
            }

            Assert.IsTrue(estimated.Any(x => Math.Abs(x.Item1.Yield - x.Item2.Yield) > 1),
                "every run-in year's estimate equalled its history year's, so this test cannot tell them apart");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Builds Farm4's details screen rows and every field's run-in items, with their yields assigned, under the given setting.
        /// </summary>
        private void RunInFor(RunInPeriodYieldSource source, out List<CropViewItem> rows, out List<CropViewItem> runInItems, out Farm farmOut, out FieldResultsService serviceOut)
        {
            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath("Farm4.json")).Single();
            farm.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;
            farm.UseFieldLevelYieldAssignement = false;
            farm.Defaults.RunInPeriodYieldSource = source;

            base._initializationService.ReInitializeFarms(new[] { farm });
            farm.ResetAnimalResults();

            var service = new FieldResultsService(base._iCbmSoilCarbonCalculator, base._ipcc, base._n2OEmissionFactorCalculator, base._initializationService)
            {
                AnimalResults = new AnimalResultsService().GetAnimalResults(farm),
            };

            service.InitializeStageState(farm);

            rows = new List<CropViewItem>();
            runInItems = new List<CropViewItem>();
            foreach (var field in farm.FieldSystemComponents.OrderBy(x => x.Name))
            {
                var fieldRows = service.GetStageState(farm).DetailsScreenViewCropViewItems
                    .Where(x => x.FieldSystemComponentGuid == field.Guid)
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.IsSecondaryCrop)
                    .ToList();

                var items = service.GetRunInPeriodItems(farm, field.CropViewItems, field.StartYear, fieldRows, field);
                service.AssignRunInPeriodYields(farm, items, fieldRows, field);

                rows.AddRange(fieldRows);
                runInItems.AddRange(items);
            }

            farmOut = farm;
            serviceOut = service;
        }

        /// <summary>
        /// Each run-in item paired with the field history row it repeats.
        /// </summary>
        private static List<Tuple<CropViewItem, CropViewItem>> WithSource(List<CropViewItem> runInItems, List<CropViewItem> rows)
        {
            return runInItems.Select(item => Tuple.Create(item, rows.Single(row => row.Guid == item.DetailViewItemToComponentSelectionViewItemMap))).ToList();
        }

        private static bool IsFromHarvest(CropViewItem row)
        {
            return YieldInputPolicy.GetYieldSource(row, YieldAssignmentMethod.SmallAreaData) == YieldSource.FromHarvest;
        }

        private static string GetFixtureFilePath(string fileName, [CallerFilePath] string thisFile = "")
        {
            return Path.Combine(Path.GetDirectoryName(thisFile), "Baselines", fileName);
        }

        #endregion
    }
}
