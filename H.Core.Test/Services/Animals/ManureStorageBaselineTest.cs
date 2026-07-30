using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using H.Core;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services.Animals
{
    /// <summary>
    /// Golden-master (characterization) baseline for the manure-storage refactor of issue #451. It loads a fixed
    /// exported dairy farm ("Farm #1"), runs the real animal-results pipeline, and snapshots every numeric daily
    /// emission field (summed per management period per month) plus the shared-tank removal fraction. The snapshot is
    /// compared against a committed baseline so that, once the tank-level denominator fix (Option A or B) lands, any
    /// change OUTSIDE the shared dairy liquid tank is caught immediately, and the intended change inside it is visible
    /// as a reviewable diff.
    ///
    /// The farm is deliberately chosen so the shared LiquidWithNaturalCrust tank (dairy heifers + lactating cows, 2026)
    /// is drawn down by a single land application on 2026-05-22: today every contributing period clamps to the 0.95
    /// removal cap, whereas the correct shared-tank fraction is ~0.7544.
    ///
    /// To regenerate the baseline after an intended change, set the environment variable HOLOS_UPDATE_BASELINES=1 and
    /// run this test once; it rewrites the committed file and passes.
    /// </summary>
    [TestClass]
    public class ManureStorageBaselineTest : UnitTestBase
    {
        [TestMethod]
        public void Baseline_Farm1_DairyAnimalResults_MatchesGolden()
        {
            var snapshot = BuildSnapshot();
            var goldenPath = GetBaselineFilePath();

            var updating = Environment.GetEnvironmentVariable("HOLOS_UPDATE_BASELINES") == "1";
            if (updating || File.Exists(goldenPath) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(goldenPath));
                File.WriteAllText(goldenPath, snapshot);
                Assert.Inconclusive($"Baseline written to {goldenPath} ({snapshot.Split('\n').Length} lines). " +
                                    "Commit it, then run again without HOLOS_UPDATE_BASELINES to compare.");
                return;
            }

            var expected = File.ReadAllText(goldenPath).Replace("\r\n", "\n");
            var actual = snapshot.Replace("\r\n", "\n");
            if (expected == actual)
            {
                return;
            }

            // Produce a focused diff of the first differing lines so an unintended change is easy to spot.
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
                    diff.AppendLine($"  line {i + 1}:");
                    diff.AppendLine($"    baseline: {el}");
                    diff.AppendLine($"    current : {al}");
                    shown++;
                }
            }

            var actualPath = goldenPath + ".actual";
            File.WriteAllText(actualPath, snapshot);
            Assert.Fail($"Animal-results snapshot differs from baseline.\nFull current output written to {actualPath}.\n" +
                        $"First differing lines:\n{diff}");
        }

        private string BuildSnapshot()
        {
            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath("Farm1.json")).Single();
            base._initializationService.ReInitializeFarms(new[] { farm });

            var dairy = farm.Components.OfType<AnimalComponentBase>().First();
            var results = new DairyCattleResultsService().CalculateResultsForAnimalComponents(new[] { dairy }, farm);

            // Every readable double property on a daily emission record - a complete, future-proof field set.
            var numericProps = typeof(GroupEmissionsByDay)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0 && p.PropertyType == typeof(double))
                .OrderBy(p => p.Name, StringComparer.Ordinal)
                .ToList();

            var rows = new List<string>();
            foreach (var componentResult in results)
            foreach (var groupResult in componentResult.EmissionResultsForAllAnimalGroupsInComponent)
            foreach (var month in groupResult.GroupEmissionsByMonths)
            {
                var mp = month.MonthsAndDaysData.ManagementPeriod;
                var key = $"{mp?.AnimalType}|{mp?.Name}|{mp?.ManureDetails?.StateType}|M{month.MonthsAndDaysData.Month:00}";
                foreach (var prop in numericProps)
                {
                    var sum = 0d;
                    foreach (var day in month.DailyEmissions)
                    {
                        try
                        {
                            sum += (double)prop.GetValue(day);
                        }
                        catch
                        {
                            // Some getters may not be valid in every state; treat as no contribution.
                        }
                    }

                    if (Math.Abs(sum) > 1e-12)
                    {
                        rows.Add(string.Format(CultureInfo.InvariantCulture, "{0} | {1} = {2:F4}", key, prop.Name, sum));
                    }
                }
            }

            rows.Sort(StringComparer.Ordinal);

            var sb = new StringBuilder();
            sb.Append("# Manure-storage golden baseline (issue #451) - Farm #1 dairy animal results\n");
            sb.Append("# Sum of each numeric daily emission field, per management period per month. Regenerate with HOLOS_UPDATE_BASELINES=1.\n");
            foreach (var row in rows)
            {
                sb.Append(row).Append('\n');
            }

            // Shared-tank removal-day detail (the value the fix changes): per-period denominator/fraction on 2026-05-22.
            sb.Append("#\n# Shared LiquidWithNaturalCrust tank on 2026-05-22 (the removal day)\n");
            var removalDay = new DateTime(2026, 5, 22);
            var removalRows = new List<string>();
            foreach (var componentResult in results)
            foreach (var groupResult in componentResult.EmissionResultsForAllAnimalGroupsInComponent)
            foreach (var month in groupResult.GroupEmissionsByMonths)
            {
                var mp = month.MonthsAndDaysData.ManagementPeriod;
                foreach (var d in month.DailyEmissions.Where(x => x.DateTime.Date == removalDay))
                {
                    removalRows.Add(string.Format(CultureInfo.InvariantCulture,
                        "{0}|{1}: net={2:F4} removed={3:F4}",
                        mp?.AnimalType, mp?.Name, d.AccumulatedVolumeNetOfRemovals, d.VolumeOfManureRemovedFromStorageOnDay));
                }
            }

            removalRows.Sort(StringComparer.Ordinal);
            foreach (var row in removalRows)
            {
                sb.Append(row).Append('\n');
            }

            return sb.ToString();
        }

        private static string GetBaselineFilePath()
        {
            return GetFixtureFilePath("Farm1.dairy-animal-results.baseline.txt");
        }

        // Fixtures live next to this test's source (a committed 'Baselines' folder), resolved from the compiled-in
        // source path so the same files are used whether run locally or from the build output.
        private static string GetFixtureFilePath(string fileName, [CallerFilePath] string thisFile = "")
        {
            return Path.Combine(Path.GetDirectoryName(thisFile), "Baselines", fileName);
        }
    }
}
