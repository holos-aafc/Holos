using H.Core.Enumerations;
using H.Core.Providers.Nitrogen;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Providers.Nitrogen
{
    /// <summary>
    /// The values these tests pin come from Resources\NitrogenFixationByCropType.csv. Revising that file is expected
    /// to fail these tests; update them to match rather than working around them.
    /// </summary>
    [TestClass]
    public class NitrogenFixationProviderTest
    {
        #region Fields

        private NitogenFixationProvider _provider;

        #endregion

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new NitogenFixationProvider();
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetNitrogenFixationResultReturnsValuesFromFile()
        {
            Assert.AreEqual(0.55, _provider.GetNitrogenFixationResult(CropType.Soybeans).Fixation, 0.0001);
            Assert.AreEqual(0.54, _provider.GetNitrogenFixationResult(CropType.DryPeas).Fixation, 0.0001);
            Assert.AreEqual(0.54, _provider.GetNitrogenFixationResult(CropType.FieldPeas).Fixation, 0.0001);
            Assert.AreEqual(0.40, _provider.GetNitrogenFixationResult(CropType.BeansDryField).Fixation, 0.0001);
            Assert.AreEqual(0.53, _provider.GetNitrogenFixationResult(CropType.Lentils).Fixation, 0.0001);
            Assert.AreEqual(0.52, _provider.GetNitrogenFixationResult(CropType.Chickpeas).Fixation, 0.0001);
            Assert.AreEqual(0.51, _provider.GetNitrogenFixationResult(CropType.PulseCrops).Fixation, 0.0001);
            Assert.AreEqual(0.66, _provider.GetNitrogenFixationResult(CropType.TameLegume).Fixation, 0.0001);
            Assert.AreEqual(0.66, _provider.GetNitrogenFixationResult(CropType.TameMixed).Fixation, 0.0001);
        }

        /// <summary>
        /// The file states percentages; every caller expects a fraction.
        /// </summary>
        [TestMethod]
        public void GetNitrogenFixationResultReturnsAFractionNotAPercentage()
        {
            foreach (CropType cropType in System.Enum.GetValues(typeof(CropType)))
            {
                var fixation = _provider.GetNitrogenFixationResult(cropType).Fixation;

                Assert.IsTrue(fixation >= 0 && fixation <= 1, $"{cropType} returned {fixation}");
            }
        }

        [TestMethod]
        public void GetNitrogenFixationResultReturnsZeroForACropThatIsNotListed()
        {
            Assert.AreEqual(0, _provider.GetNitrogenFixationResult(CropType.Barley).Fixation, 0.0001);
            Assert.AreEqual(0, _provider.GetNitrogenFixationResult(CropType.FabaBeans).Fixation, 0.0001);
            Assert.AreEqual(0, _provider.GetNitrogenFixationResult(CropType.WhiteBeans).Fixation, 0.0001);
        }

        /// <summary>
        /// Every row in the file must name a crop the converter recognises, otherwise the row is silently dropped and
        /// that crop quietly fixes nothing.
        /// </summary>
        [TestMethod]
        public void EveryRowInTheFileWasParsed()
        {
            var lines = H.Content.CsvResourceReader.GetFileLines(H.Content.CsvResourceNames.NitrogenFixationByCropType);
            var rowCount = 0;

            // Deliberately spelled out rather than calling ProviderBase.IsComment. If that method ever mistook a data
            // row for a comment, sharing it here would make the test skip the same rows as the reader and still pass,
            // while those crops quietly fixed nothing - which is the very thing this test exists to catch.
            foreach (var line in lines)
            {
                if (line.Length > 0 &&
                    string.IsNullOrWhiteSpace(line[0]) == false &&
                    line[0].TrimStart().StartsWith("#") == false)
                {
                    rowCount++;
                }
            }

            // The header is counted above, so the file holds rowCount - 1 crops.
            var cropsInFile = rowCount - 1;
            var cropsParsed = 0;

            foreach (CropType cropType in System.Enum.GetValues(typeof(CropType)))
            {
                if (_provider.GetNitrogenFixationResult(cropType).Fixation > 0)
                {
                    cropsParsed++;
                }
            }

            Assert.AreEqual(cropsInFile, cropsParsed);
        }

        #endregion
    }
}
