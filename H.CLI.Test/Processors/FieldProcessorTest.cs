using H.CLI.Processors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.Core.Models;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using System.IO;

namespace H.CLI.Test.Processors
{
    [TestClass]
    public class FieldProccessorTest
    {
        private readonly string testPath = ".\\Test";

        [TestInitialize]
        public void Initialize()
        {
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

            var fieldProcessor = new FieldProcessor();

            Farm testFarm = new Farm();
            testFarm.Name = "Test Firm";

            Dictionary<string, ImperialUnitsOfMeasurement?> testComponentKeys = new Dictionary<string, ImperialUnitsOfMeasurement?>()
            {

            };
            FieldSystemComponent testFieldSystemComponent = new FieldSystemComponent();
            testFieldSystemComponent.Name = "Test";
            List<CropViewItem> testCropViewItemList = new List<CropViewItem>();

            fieldProcessor.SetTemplateCSVFileBasedOnExportedField(testFarm,
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

            var fieldProcessor = new FieldProcessor();

            Farm testFarm = new Farm();
            testFarm.Name = "Test Firm";

            Dictionary<string, ImperialUnitsOfMeasurement?> testComponentKeys = new Dictionary<string, ImperialUnitsOfMeasurement?>()
            {

            };
            FieldSystemComponent testFieldSystemComponent = new FieldSystemComponent();
            testFieldSystemComponent.Name = "Test\\Field";
            List<CropViewItem> testCropViewItemList = new List<CropViewItem>();

            fieldProcessor.SetTemplateCSVFileBasedOnExportedField(testFarm,
                testPath,
                testComponentKeys,
                testFieldSystemComponent,
                testCropViewItemList);


            Assert.IsTrue(File.Exists(expectedFileName));
            File.Delete(expectedFileName);
        }
    }
}