using H.CLI.Processors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.Core.Models;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using System.IO;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Nitrogen;
using H.Core.Providers.Climate;
using H.Core.Services.LandManagement;
using H.Core.Test;

namespace H.CLI.Test.Processors
{
    [TestClass]
    public class FieldProccessorTest : UnitTestBase
    {
        private readonly string testPath = ".\\Test";
        private FieldProcessor _sut;
        

        [TestInitialize]
        public void Initialize()
        {
            
            var iCBMSoilCarbonCalculator = new ICBMSoilCarbonCalculator(_climateProvider, _n2OEmissionFactorCalculator);
            var n2oEmissionFactorCalculator = new N2OEmissionFactorCalculator(_climateProvider);
            var ipcc = new IPCCTier2SoilCarbonCalculator(_climateProvider, n2oEmissionFactorCalculator);

            var fieldResultsService = new FieldResultsService(iCBMSoilCarbonCalculator, ipcc, n2oEmissionFactorCalculator, _initializationService);
            _sut = new FieldProcessor(fieldResultsService);

            Directory.CreateDirectory(testPath);
        }

        [TestCleanup]
        public void Cleanup() 
        { 
            Directory.Delete(testPath);
        }

        [TestMethod]
        public void TestSetTemplateCSVFileBasedOnExportedField()
        {
            var expectedFileName = testPath + "\\Test.csv";

            Farm testFarm = new Farm();
            testFarm.Name = "Test Firm";

            Dictionary<string, ImperialUnitsOfMeasurement?> testComponentKeys = new Dictionary<string, ImperialUnitsOfMeasurement?>()
            {

            };
            FieldSystemComponent testFieldSystemComponent = new FieldSystemComponent();
            testFieldSystemComponent.Name = "Test";
            List<CropViewItem> testCropViewItemList = new List<CropViewItem>();

            _sut.SetTemplateCSVFileBasedOnExportedField(testFarm,
                testPath,
                testComponentKeys,
                testFieldSystemComponent, 
                testCropViewItemList);


            Assert.IsTrue(File.Exists(expectedFileName));
            File.Delete(expectedFileName);
        }

        [TestMethod]
        public void TestSetTemplateCSVFileBasedOnExportedFieldSpecialCharacter()
        {
            var expectedFileName = testPath + "\\Test-Field.csv";

            Farm testFarm = new Farm();
            testFarm.Name = "Test Firm";

            Dictionary<string, ImperialUnitsOfMeasurement?> testComponentKeys = new Dictionary<string, ImperialUnitsOfMeasurement?>()
            {

            };
            FieldSystemComponent testFieldSystemComponent = new FieldSystemComponent();
            testFieldSystemComponent.Name = "Test\\Field";
            List<CropViewItem> testCropViewItemList = new List<CropViewItem>();

            _sut.SetTemplateCSVFileBasedOnExportedField(testFarm,
                testPath,
                testComponentKeys,
                testFieldSystemComponent,
                testCropViewItemList);


            Assert.IsTrue(File.Exists(expectedFileName));
            File.Delete(expectedFileName);
        }
    }
}