using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Emissions.Results;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using H.Core.Properties;
using H.Core.Services.LandManagement;
using H.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace H.Core.Test.Services.LandManagement
{
    /// <summary>
    /// The results export builds its header row and its data rows from two separate lists that have to stay in step.
    /// Adding a column to one and not the other shifts every column after it, silently: the file still opens, and
    /// every value from that point on sits under the wrong heading.
    /// </summary>
    [TestClass]
    public class FieldResultsExportColumnTest : UnitTestBase
    {
        #region Fields

        private string _folder;
        private FieldResultsService _resultsService;

        #endregion

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            _folder = Path.Combine(Path.GetTempPath(), "holos-export-test-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_folder);

            var n2OEmissionFactorCalculator = new N2OEmissionFactorCalculator(base._climateProvider);

            _resultsService = new FieldResultsService(
                new ICBMSoilCarbonCalculator(base._climateProvider, n2OEmissionFactorCalculator),
                new IPCCTier2SoilCarbonCalculator(base._climateProvider, n2OEmissionFactorCalculator),
                n2OEmissionFactorCalculator,
                base._initializationService);

            _resultsService.AnimalResultsService = new Mock<IAnimalService>().Object;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                if (Directory.Exists(_folder))
                {
                    Directory.Delete(_folder, recursive: true);
                }
            }
            catch (IOException)
            {
                // A leftover temp folder is not worth failing a test over.
            }
        }

        #endregion

        #region Tests

        [TestMethod]
        public void EveryDataRowHasAValueForEveryHeading()
        {
            var lines = Export(BuildViewItem(), MeasurementSystemType.Metric);

            Assert.IsTrue(lines.Count >= 3, "expected a separator line, a heading line and a data row");

            var headings = CountFields(lines[1]);
            var values = CountFields(lines[2]);

            Assert.AreEqual(headings, values,
                $"the export has {headings} headings and {values} values, so the columns are shifted - a column was " +
                "added to the heading row or the data row without the other");
        }

        /// <summary>
        /// A count check alone would still pass if the column carried some other property, so this reads the value
        /// back from under its own heading.
        /// </summary>
        [TestMethod]
        public void TheOrganicNitrogenColumnCarriesTheOrganicNitrogenPool()
        {
            var viewItem = BuildViewItem();
            viewItem.OrganicNitrogenResiduesBeforeAdjustment = 46.25;

            var lines = Export(viewItem, MeasurementSystemType.Metric);

            var column = Fields(lines[1])
                .FindIndex(x => x.Contains(Resources.LabelOrganicNitrogenBeforeAdjustment));

            Assert.AreNotEqual(-1, column, "the organic nitrogen heading is not in the export");

            var exported = Fields(lines[2])[column].Trim('"', ' ');

            Assert.AreEqual(46.25, double.Parse(exported, CultureInfo.InvariantCulture), 0.0001,
                "the value under the organic nitrogen heading is not the organic nitrogen pool");
        }

        /// <summary>
        /// Every heading the export names has to have an imperial counterpart. A unit without one throws while the
        /// heading row is built, which is before any value is written, so a single unmapped unit means no file at all.
        /// </summary>
        [TestMethod]
        public void AnImperialExportWritesAFile()
        {
            var written = Export(BuildViewItem(), MeasurementSystemType.Imperial);

            Assert.IsTrue(written.Count >= 3, "the imperial export did not produce a heading row and a data row");
        }

        /// <summary>
        /// The unit a column is named with only changes the number on one path: an imperial export that did not come
        /// from the interface, which is the command line. Metric ignores the unit, and an imperial export from the
        /// interface arrives already converted.
        ///
        /// So this exports that one path and reads each corrected column back by its own heading. Naming the total
        /// unit instead of the areal one gives the column the heading "lb N" and leaves out the hectares to acres
        /// division, and both halves of that are asserted here.
        /// </summary>
        [TestMethod]
        public void TheArealNitrogenColumnsAreExportedPerAcre()
        {
            var viewItem = BuildViewItem();

            // A distinct value per column, so a column reading from the wrong property fails as well.
            var expected = new Dictionary<string, double>();
            var metricValue = 10.0;

            foreach (var column in ArealNitrogenColumns)
            {
                typeof(CropViewItem).GetProperty(column.Value).SetValue(viewItem, metricValue);
                expected.Add(column.Key, metricValue);
                metricValue += 3.0;
            }

            var lines = Export(viewItem, MeasurementSystemType.Imperial);
            var headings = Fields(lines[1]).Select(x => x.Trim('"', ' ')).ToList();
            var values = Fields(lines[2]);

            foreach (var column in ArealNitrogenColumns)
            {
                var heading = column.Key + " (" + PoundsNitrogenPerAcre + ")";
                var matches = headings.Select((text, index) => new {text, index})
                    .Where(x => x.text == heading)
                    .ToList();

                Assert.AreEqual(1, matches.Count,
                    $"expected exactly one \"{heading}\" column in the imperial export, found {matches.Count} - the " +
                    $"{column.Value} column is named with the total nitrogen unit, or two columns share a heading");

                var exported = double.Parse(values[matches[0].index].Trim('"', ' '), CultureInfo.InvariantCulture);

                // Pounds per acre multiplies by the kilograms to pounds factor and divides by hectares to acres.
                // Pounds alone only multiplies, so a column on the total unit reads about 2.47 times too high.
                Assert.AreEqual(expected[column.Key] * 2.205 / 2.4711, exported, 0.01,
                    $"the {column.Value} column was not converted per acre");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Every column the nitrogen budget reports per hectare, as its heading and the property behind it. The
        /// headings do not always match the property names, so the two are paired here rather than derived.
        /// </summary>
        private static Dictionary<string, string> ArealNitrogenColumns => new Dictionary<string, string>()
        {
            {Resources.LabelCropNitrogenDemand, nameof(CropViewItem.CropNitrogenDemand)},
            {Resources.LabelDifferenceBetweenInputsAndOutputs, nameof(CropViewItem.DifferenceBetweenInputsAndOutputs)},
            {Resources.LabelManureResiduePool_ManureN, nameof(CropViewItem.ManureResiduePool_ManureN)},
            {Resources.LabelMicrobeDeath, nameof(CropViewItem.MicrobeDeath)},
            {Resources.LabelMicrobeNitrogenPool_N_microbeN, nameof(CropViewItem.MicrobeNitrogenPool_N_microbeN)},
            {Resources.LabelMicrobePoolAfterCloseOfBudget, nameof(CropViewItem.MicrobialPoolAfterCloseOfBudget)},
            {Resources.LabelMicrobePoolAfterCropDemandReduction, nameof(CropViewItem.MicrobialPoolAfterCropDemandAdjustment)},
            {Resources.LabelMicrobePoolAfterOldPoolDemandReduction, nameof(CropViewItem.MicrobialPoolAfterOldPoolDemandAdjustment)},
            {Resources.LabelMicrobialNitrogenBalance, nameof(CropViewItem.MicrobialNitrogenBalance)},
            {Resources.LabelMineralNitrogenBalance, nameof(CropViewItem.MineralNitrogenBalance)},
            {Resources.LabelMineralNitrogenPool_N_mineralN, nameof(CropViewItem.MineralNitrogenPool_N_mineralN)},
            {Resources.LabelN_min_FromDecompositionOfOldCarbon, nameof(CropViewItem.N_min_FromDecompositionOfOldCarbon)},
            {Resources.LabelOldPoolNitrogenRequirement, nameof(CropViewItem.OldPoolNitrogenRequirement)},
            {Resources.LabelOverflow, nameof(CropViewItem.Overflow)},
            {Resources.LabelSumOfMineralAndMicrobialPools, nameof(CropViewItem.SumOfMineralAndMicrobialPools)},
            {Resources.LabelSyntheticInputsBeforeAdjustment, nameof(CropViewItem.SyntheticInputsBeforeAdjustment)},
            {Resources.LabelTotalNitrogenEmissions, nameof(CropViewItem.TotalNitrogenEmissions)},
            {Resources.LabelTotalNitrogenInputs, nameof(CropViewItem.TotalNitrogenInputs)},
            {Resources.LabelTotalNitrogenOutputs, nameof(CropViewItem.TotalNitrogenOutputs)},
            {Resources.LabelTotalUptake, nameof(CropViewItem.TotalUptake)},
        };

        private static string PoundsNitrogenPerAcre =>
            ImperialUnitsOfMeasurement.PoundsNitrogenPerAcre.GetDescription();

        #endregion

        #region Private Methods

        /// <summary>
        /// Exports one view item from the command line path and returns the written lines, blanks removed. The first
        /// is a separator directive, the second the headings and the third the only data row.
        /// </summary>
        private List<string> Export(CropViewItem viewItem, MeasurementSystemType measurementSystem)
        {
            // Not exported from the interface, so the path is treated as a prefix and the file name is built from
            // the farm name.
            var path = _folder + Path.DirectorySeparatorChar;

            var exported = _resultsService.ExportResultsToFile(
                results: new List<CropViewItem>() {viewItem},
                path: path,
                cultureInfo: CultureInfo.InvariantCulture,
                measurementSystemType: measurementSystem,
                languageAddOn: string.Empty,
                exportedFromGui: false,
                farm: new Farm());

            Assert.IsTrue(exported, "the export did not write a file");

            var written = Directory.GetFiles(_folder, "*.csv").SingleOrDefault();

            Assert.IsNotNull(written, "no csv was written to the export folder");

            return File.ReadAllLines(written).Where(x => string.IsNullOrWhiteSpace(x) == false).ToList();
        }

        private static CropViewItem BuildViewItem()
        {
            return new CropViewItem()
            {
                Year = 2026,
                CropType = CropType.Barley,
                Area = 1,
                CropEnergyResults = new CropEnergyResults(),
            };
        }

        /// <summary>
        /// Splits a line on its commas, ignoring commas inside a quoted value.
        /// </summary>
        private static List<string> Fields(string line)
        {
            var fields = new List<string>();
            var current = string.Empty;
            var inQuotes = false;

            foreach (var character in line)
            {
                if (character == '"')
                {
                    inQuotes = inQuotes == false;
                }
                else if (character == ',' && inQuotes == false)
                {
                    fields.Add(current);
                    current = string.Empty;
                    continue;
                }

                current += character;
            }

            fields.Add(current);

            return fields;
        }

        /// <summary>
        /// Counts comma separated fields, ignoring commas inside a quoted value.
        /// </summary>
        private static int CountFields(string line)
        {
            var count = 1;
            var inQuotes = false;

            foreach (var character in line)
            {
                if (character == '"')
                {
                    inQuotes = inQuotes == false;
                }
                else if (character == ',' && inQuotes == false)
                {
                    count++;
                }
            }

            return count;
        }

        #endregion
    }
}
