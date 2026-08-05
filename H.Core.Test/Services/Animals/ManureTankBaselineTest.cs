using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using H.Core;
using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services.Animals
{
    /// <summary>
    /// Golden-master (characterization) baseline for the manure-tank accounting in <see cref="ManureService"/> - the
    /// tank build (Initialize) and every public tank-derived query the rest of the pipeline reads through (volumes,
    /// nitrogen/TAN/carbon created, amounts remaining, amounts applied, export availability). Pinning this whole query
    /// surface protects the downstream consumers (indirect N2O, soil-carbon inputs, exports) by construction: they read
    /// the tanks only through these methods, so identical outputs here mean identical inputs there.
    ///
    /// This is the safety net for the planned unification of the tank accounting onto the ManureTank domain object
    /// (issue #451 follow-up). Regenerate with HOLOS_UPDATE_BASELINES=1.
    /// </summary>
    [TestClass]
    public class ManureTankBaselineTest : UnitTestBase
    {
        private static readonly AnimalType[] Categories =
        {
            AnimalType.Dairy, AnimalType.Beef, AnimalType.Swine,
            AnimalType.Sheep, AnimalType.Poultry, AnimalType.OtherLivestock,
        };

        [TestMethod]
        public void Baseline_Farm1_ManureServiceTankTotals_MatchesGolden()
        {
            RunBaseline("Farm1.json", "Farm1.manure-tank-totals.baseline.txt");
        }

        [TestMethod]
        public void Baseline_Farm2_ManureServiceTankTotals_MatchesGolden()
        {
            RunBaseline("Farm2.json", "Farm2.manure-tank-totals.baseline.txt");
        }

        /// <summary>
        /// Demonstrates the unification (issue #451): after the shared-store run, a single ManureTank carries BOTH the
        /// daily storage series produced by the animal results AND the whole-year totals added by ManureService - one
        /// source of truth, rather than two disconnected tank worlds.
        /// </summary>
        [TestMethod]
        public void SharedStore_DairyLiquidTank_CarriesBothDailyStorageAndTotals()
        {
            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath("Farm2.json")).Single();
            base._initializationService.ReInitializeFarms(new[] { farm });

            var store = new ManureTankStore();
            var animalResults = new AnimalResultsService().GetAnimalResults(farm, store);
            new ManureService().Initialize(farm, animalResults, store);

            var dairyLiquidTank = store.Tanks.Single(t =>
                t.AnimalType.GetCategory() == AnimalType.Dairy &&
                t.Year == 2025 &&
                t.ManureStateType == ManureStateType.LiquidWithNaturalCrust);

            // Daily storage from the animal phase:
            Assert.IsTrue(dairyLiquidTank.NetOfRemovalsByDate.Count > 0, "tank should carry the animal-phase daily net-of-removals series");
            Assert.IsTrue(dairyLiquidTank.RemovalFractionByDate.Count > 0, "tank should carry the animal-phase removal fraction series");

            // Whole-year totals from ManureService, on the SAME tank object:
            Assert.IsTrue(dairyLiquidTank.VolumeOfManureAvailableForLandApplication > 0, "tank should carry the ManureService totals");
        }

        /// <summary>
        /// Confirms the field/indirect-N2O path uses the shared store when it is set (issue #451): the N2O calculator
        /// builds its manure tanks into the shared store rather than a private one.
        /// </summary>
        [TestMethod]
        public void N2OCalculator_WithSharedStore_PopulatesThatStoreInsteadOfRebuilding()
        {
            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath("Farm2.json")).Single();
            base._initializationService.ReInitializeFarms(new[] { farm });

            var store = new ManureTankStore();
            var animalResults = new AnimalResultsService().GetAnimalResults(farm, store);

            var n2oCalculator = new N2OEmissionFactorCalculator(base._climateProvider) { SharedManureTankStore = store };
            n2oCalculator.Initialize(farm, animalResults);

            Assert.IsTrue(store.Tanks.Any(t => t.VolumeOfManureAvailableForLandApplication > 0),
                "the N2O calculator should build its manure tanks into the shared store when it is set");
        }

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
            Assert.Fail($"ManureService tank-total snapshot differs from baseline ({fixtureFileName}).\n" +
                        $"Full current output at {goldenPath}.actual.\nFirst differences:\n{diff}");
        }

        private string BuildSnapshot(string fixtureFileName)
        {
            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath(fixtureFileName)).Single();
            base._initializationService.ReInitializeFarms(new[] { farm });

            // Exercise the unified shared-store path (issue #451): the animal results populate the tanks' daily storage
            // into the shared store, then ManureService adds the totals onto the same tanks. The committed baseline was
            // generated via the old private-store path, so a byte-identical match proves the unification changed no totals.
            var manureTankStore = new ManureTankStore();
            var animalResults = new AnimalResultsService().GetAnimalResults(farm, manureTankStore);

            var manureService = new ManureService();
            manureService.Initialize(farm, animalResults, manureTankStore);

            var years = farm.GetYearsWithAnimals().OrderBy(y => y).ToList();
            var rows = new List<string>();

            foreach (var year in years)
            {
                // Year-level tank totals (read by exports and the farm-wide N/volume-remaining queries).
                Record(rows, $"year={year} | GetTotalVolumeCreated", () => manureService.GetTotalVolumeCreated(year));
                Record(rows, $"year={year} | GetTotalTANCreated", () => manureService.GetTotalTANCreated(year));
                Record(rows, $"year={year} | GetTotalNitrogenCreated", () => manureService.GetTotalNitrogenCreated(year));
                Record(rows, $"year={year} | GetTotalCarbonCreated", () => manureService.GetTotalCarbonCreated(year));
                Record(rows, $"year={year} | GetVolumeAvailableForExport", () => manureService.GetVolumeAvailableForExport(year));
                Record(rows, $"year={year} | GetTotalNitrogenAppliedToAllFields", () => manureService.GetTotalNitrogenAppliedToAllFields(year));
                Record(rows, $"year={year} | GetTotalManureNitrogenRemainingForFarmAndYear", () => manureService.GetTotalManureNitrogenRemainingForFarmAndYear(year, farm));
                Record(rows, $"year={year} | GetTotalVolumeRemainingForFarmAndYear", () => manureService.GetTotalVolumeRemainingForFarmAndYear(year, farm));

                // Per-animal-category totals (read by the per-category N2O / carbon-input paths).
                foreach (var category in Categories)
                {
                    Record(rows, $"year={year} cat={category} | GetTotalVolumeCreated", () => manureService.GetTotalVolumeCreated(year, category));
                    Record(rows, $"year={year} cat={category} | GetTotalTANCreated", () => manureService.GetTotalTANCreated(year, category));
                    Record(rows, $"year={year} cat={category} | GetTotalNitrogenCreated", () => manureService.GetTotalNitrogenCreated(year, category));
                    Record(rows, $"year={year} cat={category} | GetTotalNitrogenAppliedToAllFields", () => manureService.GetTotalNitrogenAppliedToAllFields(year, category));
                    Record(rows, $"year={year} cat={category} | GetVolumeAvailableForExport", () => manureService.GetVolumeAvailableForExport(year, farm, category));
                }
            }

            foreach (var category in Categories)
            {
                Record(rows, $"cat={category} | GetYearHighestVolumeRemaining", () => manureService.GetYearHighestVolumeRemaining(category));
            }

            rows.Sort(StringComparer.Ordinal);

            var sb = new StringBuilder();
            sb.Append("# ManureService tank-total golden baseline (issue #451 unification) - " + fixtureFileName + "\n");
            sb.Append("# One line per non-zero tank-derived query. Regenerate with HOLOS_UPDATE_BASELINES=1.\n");
            foreach (var row in rows)
            {
                sb.Append(row).Append('\n');
            }

            return sb.ToString();
        }

        // Emits a line only for non-zero results (keeps absent categories out of the snapshot); records exceptions so a
        // change in throwing behaviour is also caught.
        private static void Record(List<string> rows, string label, Func<double> query)
        {
            try
            {
                var value = query();
                if (Math.Abs(value) > 1e-9)
                {
                    rows.Add(string.Format(CultureInfo.InvariantCulture, "{0} = {1:F4}", label, value));
                }
            }
            catch (Exception e)
            {
                rows.Add($"{label} = THREW:{e.GetType().Name}");
            }
        }

        private static string GetFixtureFilePath(string fileName, [CallerFilePath] string thisFile = "")
        {
            return Path.Combine(Path.GetDirectoryName(thisFile), "Baselines", fileName);
        }
    }
}
