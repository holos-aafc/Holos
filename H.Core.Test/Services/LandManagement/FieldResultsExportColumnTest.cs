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
using H.Core.Services.LandManagement;
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
            // Not exported from the interface, so the path is treated as a prefix and the file name is built from
            // the farm name.
            var path = _folder + Path.DirectorySeparatorChar;

            var exported = _resultsService.ExportResultsToFile(
                results: new List<CropViewItem>() {new CropViewItem() {Year = 2026, CropType = CropType.Barley, Area = 1, CropEnergyResults = new CropEnergyResults()}},
                path: path,
                cultureInfo: CultureInfo.InvariantCulture,
                measurementSystemType: MeasurementSystemType.Metric,
                languageAddOn: string.Empty,
                exportedFromGui: false,
                farm: new Farm());

            Assert.IsTrue(exported, "the export did not write a file");

            var written = Directory.GetFiles(_folder, "*.csv").SingleOrDefault();

            Assert.IsNotNull(written, "no csv was written to the export folder");

            // The first line is a separator directive, the second the headings, the third the only data row.
            var lines = File.ReadAllLines(written).Where(x => string.IsNullOrWhiteSpace(x) == false).ToList();

            Assert.IsTrue(lines.Count >= 3, "expected a separator line, a heading line and a data row");

            var headings = CountFields(lines[1]);
            var values = CountFields(lines[2]);

            Assert.AreEqual(headings, values,
                $"the export has {headings} headings and {values} values, so the columns are shifted - a column was " +
                "added to the heading row or the data row without the other");
        }

        #endregion

        #region Private Methods

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
