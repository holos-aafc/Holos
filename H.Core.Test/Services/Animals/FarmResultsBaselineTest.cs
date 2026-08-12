using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using H.Core;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.Nitrogen;
using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.Initialization;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;

namespace H.Core.Test.Services.Animals
{
    /// <summary>
    /// End-to-end whole-farm golden master over <see cref="FarmResultsService.CalculateFarmEmissionResults"/> - the top
    /// orchestrator - snapshotting the ENTIRE numeric output surface via a reflective walk. This is the safety net for the
    /// planned per-run scoped-composition refactor (Option 2), whose risk is changing service lifetimes and re-wiring the
    /// composition roots. It complements the finer-grained tank/carbon baselines (which localize a regression) by pinning
    /// every output at once, and it drives the SAME shared-instance wiring production uses (one N2O calc across the field
    /// and farm services) so a wiring change is detectable. Regenerate with HOLOS_UPDATE_BASELINES=1.
    /// </summary>
    [TestClass]
    public class FarmResultsBaselineTest : UnitTestBase
    {
        [TestMethod]
        public void Baseline_Farm1_WholeFarmResults_MatchesGolden()
        {
            RunBaseline("Farm1.json", "Farm1.whole-farm.baseline.txt.gz");
        }

        [TestMethod]
        public void Baseline_Farm2_WholeFarmResults_MatchesGolden()
        {
            RunBaseline("Farm2.json", "Farm2.whole-farm.baseline.txt.gz");
        }

        [TestMethod]
        public void Baseline_Farm3_WholeFarmResults_MatchesGolden()
        {
            RunBaseline("Farm3.json", "Farm3.whole-farm.baseline.txt.gz");
        }

        // ----------------------------------------------------------------------------------------------------------------
        // Lifetime / multi-run tests: the net for the scoped-composition refactor (Option 2). They target the signature
        // failure mode of a singleton -> per-run lifetime change - state silently carried across farm runs. A single-run
        // golden master cannot see this; these can. They must already pass on current code (it is run-independent), which
        // is exactly the property Option 2 must preserve.
        // ----------------------------------------------------------------------------------------------------------------

        [TestMethod]
        public void Lifetime_SameFarmTwiceOnOnePipeline_IsIdentical()
        {
            var pipeline = BuildFarmResultsService();

            var first = SnapshotVia(pipeline, "Farm3.json");
            var second = SnapshotVia(pipeline, "Farm3.json");

            Assert.AreEqual(first, second,
                "Running the same farm twice through one reused pipeline must be identical - no state may accumulate between runs.");
        }

        [TestMethod]
        public void Lifetime_DifferentFarmsBackToBack_EachMatchesSolo()
        {
            // Solo references: a fresh pipeline per farm (the "clean" single-run result).
            var solo1 = SnapshotVia(BuildFarmResultsService(), "Farm1.json");
            var solo3 = SnapshotVia(BuildFarmResultsService(), "Farm3.json");

            // Now run both through ONE reused pipeline, in sequence, and again in the opposite order.
            var forward = BuildFarmResultsService();
            var forward1 = SnapshotVia(forward, "Farm1.json");
            var forward3 = SnapshotVia(forward, "Farm3.json");

            var reverse = BuildFarmResultsService();
            var reverse3 = SnapshotVia(reverse, "Farm3.json");
            var reverse1 = SnapshotVia(reverse, "Farm1.json");

            Assert.AreEqual(solo1, forward1, "Farm1 first on a reused pipeline must match its solo result.");
            Assert.AreEqual(solo3, forward3, "Farm3 after Farm1 must match its solo result - Farm1 must not bleed into Farm3.");
            Assert.AreEqual(solo3, reverse3, "Farm3 first on a reused pipeline must match its solo result.");
            Assert.AreEqual(solo1, reverse1, "Farm1 after Farm3 must match its solo result - Farm3 must not bleed into Farm1.");
        }

        // ----------------------------------------------------------------------------------------------------------------
        // Composition (mirrors H.CLI/Program.cs + ComponentResultsProcessor, and the GUI via CoreModule): FarmResultsService
        // takes a field-results-service factory and builds a FRESH field graph (one N2O calc shared across the ICBM/IPCC
        // soil-carbon calculators and the field service) per run. Exercising the real factory is what lets the lifetime
        // tests verify runs are independent (Option 2 / scoped composition).
        // ----------------------------------------------------------------------------------------------------------------
        private FarmResultsService BuildFarmResultsService()
        {
            var factory = new FieldResultsServiceFactory(base._climateProvider, new InitializationService());

            return new FarmResultsService(
                new EventAggregator(),
                factory,
                new ADCalculator(),
                new ManureService(),
                new AnimalResultsService());
        }

        private FarmEmissionResults CalculateFarm(FarmResultsService service, string fixtureFileName)
        {
            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath(fixtureFileName)).Single();
            base._initializationService.ReInitializeFarms(new[] { farm });

            return service.CalculateFarmEmissionResults(farm);
        }

        // ----------------------------------------------------------------------------------------------------------------
        // Reflective snapshot: walk the result graph, emit every numeric/enum/bool leaf as a sorted "path = value" line.
        // ----------------------------------------------------------------------------------------------------------------
        private string BuildSnapshot(string fixtureFileName)
        {
            // A fresh pipeline per farm - the "clean" single-run reference the golden master pins.
            return SnapshotVia(BuildFarmResultsService(), fixtureFileName);
        }

        private string SnapshotVia(FarmResultsService service, string fixtureFileName)
        {
            var results = CalculateFarm(service, fixtureFileName);

            var rows = new List<string>();
            Walk("FarmEmissionResults", results, rows, new HashSet<object>(RefComparer.Instance), 0);
            rows.Sort(StringComparer.Ordinal);

            var sb = new StringBuilder();
            sb.Append("# Whole-farm end-to-end golden baseline (Option 2 scoped-composition safety net) - " + fixtureFileName + "\n");
            sb.Append("# One line per numeric/enum/bool result leaf, reflective walk. Regenerate with HOLOS_UPDATE_BASELINES=1.\n");
            foreach (var row in rows)
            {
                sb.Append(row).Append('\n');
            }

            return sb.ToString();
        }

        private static void Walk(string path, object obj, List<string> rows, HashSet<object> visited, int depth)
        {
            if (obj == null || depth > 30)
            {
                return;
            }

            var type = obj.GetType();

            // Non-deterministic or non-numeric leaves: skip.
            if (obj is string || obj is DateTime || obj is Guid || obj is TimeSpan || obj is char)
            {
                return;
            }

            // Numeric / bool / enum leaves.
            if (obj is bool boolean) { rows.Add(path + " = " + boolean); return; }
            if (obj is double dbl) { rows.Add(path + " = " + Fmt(dbl)); return; }
            if (obj is float flt) { rows.Add(path + " = " + Fmt(flt)); return; }
            if (obj is decimal dec) { rows.Add(path + " = " + Fmt((double)dec)); return; }
            if (type.IsEnum) { rows.Add(path + " = " + obj); return; }
            if (type.IsPrimitive) { rows.Add(path + " = " + Convert.ToString(obj, CultureInfo.InvariantCulture)); return; }

            // Reference-type cycle guard.
            if (!type.IsValueType)
            {
                if (visited.Contains(obj))
                {
                    return;
                }

                visited.Add(obj);
            }

            // Do not recurse into input aggregates (Farm, components, climate/soil) - they are inputs, not outputs, and
            // would balloon the walk. Results reference them only as back-pointers.
            if (IsPrunedInput(type))
            {
                return;
            }

            if (obj is IDictionary dictionary)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    Walk(path + "[" + entry.Key + "]", entry.Value, rows, visited, depth + 1);
                }

                return;
            }

            if (obj is IEnumerable enumerable)
            {
                var i = 0;
                foreach (var item in enumerable)
                {
                    Walk(path + "[" + i++ + "]", item, rows, visited, depth + 1);
                }

                return;
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanRead == false || property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                // Skip the per-DAY emission detail: it is ~90% of the volume, and the monthly aggregate getters we keep are
                // the lazy sums of exactly these days, so any daily change still surfaces monthly. A composition/lifetime
                // refactor cannot redistribute values between days while preserving the monthly sum, so nothing is lost.
                if (property.Name == "DailyEmissions")
                {
                    rows.Add(path + "." + property.Name + ".Count = " + CountOf(property, obj));
                    continue;
                }

                object value;
                try
                {
                    value = property.GetValue(obj);
                }
                catch (Exception e)
                {
                    // A change in throwing behaviour is itself a regression worth catching.
                    rows.Add(path + "." + property.Name + " = THREW:" + (e.InnerException ?? e).GetType().Name);
                    continue;
                }

                Walk(path + "." + property.Name, value, rows, visited, depth + 1);
            }
        }

        // Result objects hold back-references to their INPUT configuration (an emission result -> its AnimalGroup ->
        // ManagementPeriods -> Diet -> Ingredients, etc.). Walking those echoes the entire input and balloons the
        // snapshot without adding output coverage - any input that mattered already shows up in the emission numbers we
        // keep. Prune the input model/provider trees; keep the result namespaces and CropViewItem (which carries the
        // per-field soil-carbon / N2O results).
        private static bool IsPrunedInput(Type type)
        {
            if (type.Name == "CropViewItem")
            {
                return false;
            }

            var ns = type.Namespace ?? string.Empty;
            if (ns.StartsWith("H.Core.Models.Animals") || ns.StartsWith("H.Core.Providers"))
            {
                return true;
            }

            // Input line-items hanging off a result CropViewItem (manure applications, harvests, fertilizer, grazing, hay,
            // digestate) - pure input echoes. CropViewItem itself is kept (handled above); prune the rest of this namespace.
            if (ns.StartsWith("H.Core.Models.LandManagement"))
            {
                return true;
            }

            if (type.Name.EndsWith("Component"))
            {
                return true;
            }

            switch (type.Name)
            {
                case "Farm":
                case "ClimateData":
                case "GeographicData":
                case "SoilData":
                case "Defaults":
                case "MonthsAndDaysData":
                    return true;
                default:
                    return false;
            }
        }

        private static string CountOf(PropertyInfo property, object obj)
        {
            try
            {
                var value = property.GetValue(obj);
                if (value is ICollection collection)
                {
                    return collection.Count.ToString(CultureInfo.InvariantCulture);
                }

                if (value is IEnumerable enumerable)
                {
                    return enumerable.Cast<object>().Count().ToString(CultureInfo.InvariantCulture);
                }

                return "0";
            }
            catch
            {
                return "THREW";
            }
        }

        private static string Fmt(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            return value.ToString("F4", CultureInfo.InvariantCulture);
        }

        private sealed class RefComparer : IEqualityComparer<object>
        {
            public static readonly RefComparer Instance = new RefComparer();
            bool IEqualityComparer<object>.Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }

        // ----------------------------------------------------------------------------------------------------------------
        // Golden-master diff harness (same mechanism as the other baselines).
        // ----------------------------------------------------------------------------------------------------------------
        private void RunBaseline(string fixtureFileName, string baselineFileName)
        {
            var snapshot = BuildSnapshot(fixtureFileName);
            var goldenPath = GetFixtureFilePath(baselineFileName);

            var updating = Environment.GetEnvironmentVariable("HOLOS_UPDATE_BASELINES") == "1";
            if (updating || File.Exists(goldenPath) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(goldenPath));
                WriteGzip(goldenPath, snapshot);
                Assert.Inconclusive($"Baseline written to {goldenPath}. Commit it, then run again without HOLOS_UPDATE_BASELINES.");
                return;
            }

            // Stored gzipped (the reflective whole-farm snapshot is multi-MB of text); compare the decompressed content.
            var expected = ReadGzip(goldenPath).Replace("\r\n", "\n");
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
            Assert.Fail($"Whole-farm result snapshot differs from baseline ({fixtureFileName}).\n" +
                        $"Full current output at {goldenPath}.actual.\nFirst differences:\n{diff}");
        }

        private static void WriteGzip(string path, string content)
        {
            using (var file = File.Create(path))
            using (var gzip = new GZipStream(file, CompressionLevel.Optimal))
            using (var writer = new StreamWriter(gzip, new UTF8Encoding(false)))
            {
                writer.Write(content);
            }
        }

        private static string ReadGzip(string path)
        {
            using (var file = File.OpenRead(path))
            using (var gzip = new GZipStream(file, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzip, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private static string GetFixtureFilePath(string fileName, [CallerFilePath] string thisFile = "")
        {
            return Path.Combine(Path.GetDirectoryName(thisFile), "Baselines", fileName);
        }
    }
}
