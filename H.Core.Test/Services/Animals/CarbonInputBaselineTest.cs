using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services.Animals
{
    /// <summary>
    /// Golden-master (characterization) baseline for the CARBON pipeline's manure-carbon output - the exact surface the
    /// planned carbon-calculator unification (issue #451 follow-up) will refactor. It runs the real field/carbon model on
    /// the fixture farms and snapshots, per field-view-item, the manure carbon inputs that
    /// <see cref="H.Core.Calculators.Carbon.CarbonInputCalculatorBase.AssignManureCarbonInputs"/> produces
    /// (ManureCarbonInputsPerHectare / ManureCarbonInputsFromManureOnly / TotalCarbonInputs) plus the resulting SoilCarbon.
    ///
    /// This pins the branch logic inside AssignManureCarbonInputs (GUI/CLI split, run-in-period skip, pasture add-on) and
    /// its dependence on the manure-tank totals - NOT just the ManureService query surface. When the carbon calculators are
    /// rewired to share the per-run <see cref="ManureTankStore"/>, a byte-identical match here proves no carbon number moved.
    /// Regenerate with HOLOS_UPDATE_BASELINES=1.
    /// </summary>
    [TestClass]
    public class CarbonInputBaselineTest : UnitTestBase
    {
        [TestMethod]
        public void Baseline_Farm1_FieldManureCarbonInputs_MatchesGolden()
        {
            RunBaseline("Farm1.json", "Farm1.carbon-inputs.baseline.txt");
        }

        [TestMethod]
        public void Baseline_Farm2_FieldManureCarbonInputs_MatchesGolden()
        {
            RunBaseline("Farm2.json", "Farm2.carbon-inputs.baseline.txt");
        }

        /// <summary>
        /// Farm3 is the defense-in-depth fixture built to exercise the carbon-manure branches the single-field Farm1/Farm2
        /// leave dark: 5 fields of different areas (per-field area allocation), livestock manure across multiple years,
        /// imported manure (separate branch + the created==0/imported>0 short-circuit), same-year export+application, a
        /// grazed tame-grass field (the pasture manure-carbon add-on), and a native-grassland field (the early return 0).
        /// IPCC Tier 2. See [[holos-451-status]] for the build spec.
        /// </summary>
        [TestMethod]
        public void Baseline_Farm3_FieldManureCarbonInputs_MatchesGolden()
        {
            RunBaseline("Farm3.json", "Farm3.carbon-inputs.baseline.txt");
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
            Assert.Fail($"Field manure-carbon snapshot differs from baseline ({fixtureFileName}).\n" +
                        $"Full current output at {goldenPath}.actual.\nFirst differences:\n{diff}");
        }

        private string BuildSnapshot(string fixtureFileName)
        {
            var farm = new Storage().GetFarmsFromExportFile(GetFixtureFilePath(fixtureFileName)).Single();
            base._initializationService.ReInitializeFarms(new[] { farm });

            // Mirror the production sequence (FarmResultsService.CalculateFarmEmissionResults): the animal results feed the
            // field carbon model's manure-carbon inputs. The carbon calculators build their own manure tanks from these
            // today; the committed baseline is generated on that path, so after the unification a byte-identical match
            // proves the carbon numbers are unchanged.
            farm.ResetAnimalResults();
            var animalResults = new AnimalResultsService().GetAnimalResults(farm);

            var fieldResultsService = new FieldResultsService(
                base._iCbmSoilCarbonCalculator, base._ipcc, base._n2OEmissionFactorCalculator, base._initializationService)
            {
                AnimalResults = animalResults,
            };

            // The GUI-saved fixture already carries its detail view items in the stage state, so we do NOT rebuild them
            // (CreateDetailViewItems double-processes an already-populated farm). CalculateFinalResults re-runs the carbon
            // model over those items - including AssignManureCarbonInputs, the method the unification will refactor.
            var finalResults = fieldResultsService.CalculateFinalResults(farm);

            var rows = new List<string>();
            foreach (var item in finalResults)
            {
                var field = farm.GetFieldSystemComponent(item.FieldSystemComponentGuid);
                var fieldName = field != null ? field.Name : "<unknown-field>";
                var key = string.Format(CultureInfo.InvariantCulture, "field={0} | year={1} | crop={2}",
                    fieldName, item.Year, item.CropType);

                Add(rows, key, "ManureCarbonInputsPerHectare", item.ManureCarbonInputsPerHectare);
                Add(rows, key, "ManureCarbonInputsFromManureOnly", item.ManureCarbonInputsFromManureOnly);
                Add(rows, key, "TotalCarbonInputs", item.TotalCarbonInputs);
                Add(rows, key, "SoilCarbon", item.SoilCarbon);
            }

            rows.Sort(StringComparer.Ordinal);

            var sb = new StringBuilder();
            sb.Append("# Field manure-carbon golden baseline (issue #451 carbon-pipeline unification) - " + fixtureFileName + "\n");
            sb.Append("# One line per field/year/crop carbon field. Regenerate with HOLOS_UPDATE_BASELINES=1.\n");
            foreach (var row in rows)
            {
                sb.Append(row).Append('\n');
            }

            return sb.ToString();
        }

        // Emits every value (including zero) so a field dropping to/from zero is caught; NaN/Infinity are recorded verbatim.
        private static void Add(List<string> rows, string key, string field, double value)
        {
            string formatted;
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                formatted = value.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                formatted = value.ToString("F4", CultureInfo.InvariantCulture);
            }

            rows.Add(string.Format(CultureInfo.InvariantCulture, "{0} | {1} = {2}", key, field, formatted));
        }

        private static string GetFixtureFilePath(string fileName, [CallerFilePath] string thisFile = "")
        {
            return Path.Combine(Path.GetDirectoryName(thisFile), "Baselines", fileName);
        }
    }
}
