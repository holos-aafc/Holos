using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace H.Core.Test.Services.Animals
{
    /// <summary>
    /// Golden-master baseline for the PERENNIAL YIELD surface - the yield a field ends up with, the plant carbon derived
    /// from it, and the percentage of product returned to soil.
    ///
    /// This exists because the other baselines cannot see any of it. They call
    /// <see cref="FieldResultsService.CalculateFinalResults"/>, which deliberately does NOT rebuild the detail view items,
    /// while the perennial adjustments (UpdateYieldFromHarvestForCustomPerennials and UpdatePercentageReturnsForPerennials)
    /// run only inside CreateDetailViewItems. On top of that, no existing fixture farm has a single harvest, fertilizer or
    /// digestate application - every one of those collections is empty in Farm1, Farm2 and Farm3 - so the whole
    /// harvest-driven yield feature had no golden coverage at all.
    ///
    /// This test rebuilds the stage state (the GUI path) so those adjustments actually run, against a fixture that has a
    /// hayed perennial. Regenerate with HOLOS_UPDATE_BASELINES=1.
    /// </summary>
    [TestClass]
    public class PerennialYieldBaselineTest : UnitTestBase
    {
        #region Tests

        [TestMethod]
        public void Baseline_Farm4_PerennialYields_MatchesGolden()
        {
            RunBaseline("Farm4.json", "Farm4.perennial-yields.baseline.txt");
        }

        /// <summary>
        /// Farm3 has no harvests, but it does have a grazed perennial, so it pins the grazing side of the same surface -
        /// including the percentage returned now derived from utilization.
        /// </summary>
        [TestMethod]
        public void Baseline_Farm3_PerennialYields_MatchesGolden()
        {
            RunBaseline("Farm3.json", "Farm3.perennial-yields.baseline.txt");
        }

        #endregion

        #region Fixture generation

        /// <summary>
        /// Builds Farm4.json from Farm3.json: an extra field that is a hayed (not grazed) perennial under the Custom yield
        /// assignment method, carrying a fertilizer and a digestate application. Farm3's own fields are left untouched so
        /// the two fixtures stay comparable.
        ///
        /// Run deliberately with HOLOS_BUILD_FIXTURES=1; it is skipped otherwise so a normal test run never rewrites the
        /// fixture underneath the baseline.
        /// </summary>
        [TestMethod]
        public void BuildFarm4Fixture()
        {
            if (Environment.GetEnvironmentVariable("HOLOS_BUILD_FIXTURES") != "1")
            {
                Assert.Inconclusive("Set HOLOS_BUILD_FIXTURES=1 to regenerate Farm4.json.");
                return;
            }

            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath("Farm3.json")).Single();

            // The entered harvest is only the source of truth under Custom, which is the case this fixture exists to pin.
            farm.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            farm.Name = "Farm4 - hayed perennial with fertilizer and digestate";

            var sourceField = farm.FieldSystemComponents
                .Single(field => field.CropViewItems.Any(crop => crop.CropType == CropType.TameGrass));

            var helper = new FieldComponentHelper();
            var hayField = helper.Replicate(sourceField);
            hayField.Name = "Hayed perennial";

            foreach (var cropViewItem in hayField.CropViewItems)
            {
                // The replicated items still point at the field they were copied from.
                cropViewItem.FieldSystemComponentGuid = hayField.Guid;

                // This field is hayed, not grazed - the grazing path is already covered by Farm3's own field.
                cropViewItem.GrazingViewItems.Clear();

                cropViewItem.HarvestViewItems.Add(new HarvestViewItem()
                {
                    Start = new DateTime(cropViewItem.Year, 8, 1),
                    End = new DateTime(cropViewItem.Year, 8, 2),
                    ForageActivity = ForageActivities.Hayed,
                    TotalNumberOfBalesHarvested = 35,
                    BaleWeight = 500,
                    MoistureContentAsPercentage = 15,
                    HarvestLossPercentage = 35,
                    FieldGuid = hayField.Guid,
                });

                cropViewItem.FertilizerApplicationViewItems.Add(new FertilizerApplicationViewItem()
                {
                    DateCreated = new DateTime(cropViewItem.Year, 5, 15),
                    AmountOfNitrogenApplied = 75,
                });

                cropViewItem.DigestateApplicationViewItems.Add(new DigestateApplicationViewItem()
                {
                    DateCreated = new DateTime(cropViewItem.Year, 6, 10),
                    AmountAppliedPerHectare = 40,
                });
            }

            farm.Components.Add(hayField);

            // The field the hay field was copied from keeps its grazing animals and now also gets a hay cut, so the
            // fixture covers the grazed-and-hayed case as well - the one Eq. 11.3.2-5 / -7 combine, and the one no
            // fixture reached before.
            //
            // That case only arises under a modelled method, because IsNonSwathingGrazingScenario excludes Custom, so
            // the two fields are given different methods through field-level assignment. This keeps one fixture for
            // both cases and exercises the per-field method resolution at the same time.
            farm.UseFieldLevelYieldAssignement = true;
            hayField.YieldAssignmentMethod = YieldAssignmentMethod.Custom;
            sourceField.YieldAssignmentMethod = YieldAssignmentMethod.SmallAreaData;

            foreach (var cropViewItem in sourceField.CropViewItems.Where(crop => crop.CropType.IsPerennial()))
            {
                cropViewItem.HarvestViewItems.Add(new HarvestViewItem()
                {
                    Start = new DateTime(cropViewItem.Year, 8, 1),
                    End = new DateTime(cropViewItem.Year, 8, 2),
                    ForageActivity = ForageActivities.Hayed,
                    TotalNumberOfBalesHarvested = 20,
                    BaleWeight = 500,
                    MoistureContentAsPercentage = 15,
                    HarvestLossPercentage = 35,
                    FieldGuid = sourceField.Guid,
                });
            }

            // NOTE: a third field for the grazed-AND-hayed case under the CUSTOM method was tried and does not
            // work. FieldComponentHelper.Replicate copies the grazing view items, but InitializeGrazingViewItems
            // rebuilds a field's grazing from the animal components by matching PastureLocation against the field's
            // Guid, so a replicated field - which has a new Guid no management period points at - has its grazing
            // cleared and behaves as hayed-only. Grazing cannot be synthesised by copying; it would need an animal
            // component pointed at the new field. Since the animals point at Field #4, that field can be modelled-and-
            // grazed or custom-and-grazed but not both, and the modelled case is the one carrying the Eq. 11.3.2-5/-7
            // hay term. The custom-and-grazed branch is covered by unit tests in ICBMCarbonInputCalculatorTest instead.

            // Force the detail view items to be rebuilt from the component data when the baseline runs.
            farm.StageStates.Clear();

            var path = GetFixtureFilePath("Farm4.json");
            var serializer = new JsonSerializer() {TypeNameHandling = TypeNameHandling.Auto};
            using (var writer = new StreamWriter(path))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, new List<Farm>() {farm});
            }

            Assert.Inconclusive($"Fixture written to {path}. Commit it, then generate the baseline with HOLOS_UPDATE_BASELINES=1.");
        }

        #endregion

        #region Private Methods

        private void RunBaseline(string fixtureFileName, string baselineFileName)
        {
            var snapshot = BuildSnapshot(fixtureFileName);
            var goldenPath = GetFixtureFilePath(baselineFileName);

            var updating = Environment.GetEnvironmentVariable("HOLOS_UPDATE_BASELINES") == "1";
            if (updating || File.Exists(goldenPath) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(goldenPath));
                File.WriteAllText(goldenPath, snapshot);
                Assert.Inconclusive($"Baseline written to {goldenPath}. Commit it, then run again without HOLOS_UPDATE_BASELINES.");
                return;
            }

            var expected = File.ReadAllText(goldenPath).Replace("\r\n", "\n");
            var actual = snapshot.Replace("\r\n", "\n");
            if (expected == actual)
            {
                return;
            }

            var e = expected.Split('\n');
            var a = actual.Split('\n');
            var diff = new StringBuilder();
            var shown = 0;
            for (var i = 0; i < Math.Max(e.Length, a.Length) && shown < 25; i++)
            {
                var el = i < e.Length ? e[i] : "<missing>";
                var al = i < a.Length ? a[i] : "<missing>";
                if (el != al)
                {
                    diff.AppendLine($"  line {i + 1}: baseline [{el}]  current [{al}]");
                    shown++;
                }
            }

            File.WriteAllText(goldenPath + ".actual", snapshot);
            Assert.Fail($"Perennial yield snapshot differs from baseline ({fixtureFileName}).\n" +
                        $"Full current output at {goldenPath}.actual.\nFirst differences:\n{diff}");
        }

        /// <summary>
        /// Runs the GUI path - InitializeStageState rebuilds the detail view items through CreateDetailViewItems, which is
        /// where the perennial yield and percentage-returned adjustments live - then snapshots what each year ended up with.
        /// </summary>
        private string BuildSnapshot(string fixtureFileName)
        {
            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath(fixtureFileName)).Single();
            base._initializationService.ReInitializeFarms(new[] {farm});

            farm.ResetAnimalResults();
            var animalResults = new AnimalResultsService().GetAnimalResults(farm);

            var fieldResultsService = new FieldResultsService(
                base._iCbmSoilCarbonCalculator, base._ipcc, base._n2OEmissionFactorCalculator, base._initializationService)
            {
                AnimalResults = animalResults,
            };

            fieldResultsService.InitializeStageState(farm);

            var stageState = fieldResultsService.GetStageState(farm);

            var rows = new List<string>();
            foreach (var item in stageState.DetailsScreenViewCropViewItems)
            {
                var field = farm.GetFieldSystemComponent(item.FieldSystemComponentGuid);
                var fieldName = field != null ? field.Name : "<unknown-field>";
                var key = string.Format(CultureInfo.InvariantCulture, "field={0} | year={1} | crop={2}",
                    fieldName, item.Year, item.CropType);

                Add(rows, key, "Yield", item.Yield);
                Add(rows, key, "PlantCarbonInAgriculturalProduct", item.PlantCarbonInAgriculturalProduct);
                Add(rows, key, "PercentageOfProductYieldReturnedToSoil", item.PercentageOfProductYieldReturnedToSoil);
                Add(rows, key, "AboveGroundCarbonInput", item.AboveGroundCarbonInput);
                Add(rows, key, "BelowGroundCarbonInput", item.BelowGroundCarbonInput);
                Add(rows, key, "TotalCarbonInputs", item.TotalCarbonInputs);
            }

            rows.Sort(StringComparer.Ordinal);

            var sb = new StringBuilder();
            sb.Append("# Perennial yield golden baseline - " + fixtureFileName + "\n");
            sb.Append("# One line per field/year/crop value. Regenerate with HOLOS_UPDATE_BASELINES=1.\n");
            foreach (var row in rows)
            {
                sb.Append(row);
                sb.Append('\n');
            }

            return sb.ToString();
        }

        private static void Add(List<string> rows, string key, string name, double value)
        {
            rows.Add(string.Format(CultureInfo.InvariantCulture, "{0} | {1} = {2:F4}", key, name, value));
        }

        private static string GetFixtureFilePath(string fileName, [CallerFilePath] string callerFilePath = "")
        {
            return Path.Combine(Path.GetDirectoryName(callerFilePath), "Baselines", fileName);
        }

        #endregion
    }
}
