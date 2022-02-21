using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using H.CLI.ComponentKeys;
using H.CLI.FileAndDirectoryAccessors;
using H.Content;
using H.CLI.UserInput;
using H.Core.Enumerations;

namespace H.CLI.Test.FilesAndDirectoryAccessors
{
    [TestClass]
    public class ExcelInitializerTest
    {
        string[][] fileLines;

        [TestInitialize]
        public void Initialize()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            var excel = new ExcelInitializer();
            var shelterbeltKeys = new ShelterBeltKeys();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\ExcelInitializerTest\ReadExcelFileTest\Shelterbelts");
            var filePath = @"H.CLI.TestFiles\ExcelInitializerTest\ReadExcelFileTest";
            excel.SetTemplateCSVFileForTesting(filePath, shelterbeltKeys.keys);
            fileLines = excel.ReadExcelFile(filePath + @"\Shelterbelt1.csv").ToArray();

        }
        [TestMethod]
        public void TestExcelInitializer__ReadExcelFileEnglish_ExpectStringArrayOfValidFileLinesFromCSVFileSplitOnComma()
        {
      
            Assert.AreEqual(fileLines[0][0], "Hardiness Zone");
            Assert.AreEqual(fileLines[1][0], "H4b");
            Assert.AreEqual(fileLines[0][1], "Ecodistrict ID");
            Assert.AreEqual(fileLines[1][1], "754");      
        }

        [TestMethod]
        public void TestExcelInitializer__TestSetTemplateFileForShelterbeltsMetric()
        {
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Metric;

            var excel = new ExcelInitializer();
            var shelterbeltKeys = new ShelterBeltKeys();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\ExcelInitializerTest\Shelterbelts");
            var filePath = @"H.CLI.TestFiles\ExcelInitializerTest\Shelterbelts";
            excel.SetTemplateFile(filePath, "Shelterbelts", shelterbeltKeys.keys);
            var doesTemplateFileExist = File.Exists(filePath + @"\Shelterbelts_Example-en-CA.csv");

            using (File.OpenRead(filePath + @"\Shelterbelts_Example-en-CA.csv"))
            {
                var filelines = File.ReadAllLines(filePath + @"\Shelterbelts_Example-en-CA.csv");
                var splitHeaders = filelines[0].Split(',');
                Assert.AreEqual(splitHeaders[0], "Hardiness Zone");
                Assert.AreEqual(splitHeaders[1], "Ecodistrict ID");
                Assert.AreEqual(splitHeaders[2], "Year Of Observation");
                Assert.AreEqual(splitHeaders[3], "Name");
                Assert.AreEqual(splitHeaders[4], "Row Name");
                Assert.AreEqual(splitHeaders[5], "Row ID");
                Assert.AreEqual(splitHeaders[6], "Row Length(m)");
                Assert.AreEqual(splitHeaders[7], "Plant Year");
                Assert.AreEqual(splitHeaders[8], "Cut Year");
                Assert.AreEqual(splitHeaders[9], "Species");
                Assert.AreEqual(splitHeaders[10], "Planted Tree Count");
                Assert.AreEqual(splitHeaders[11], "Live Tree Count");
                Assert.AreEqual(splitHeaders[12], "Planted Tree Spacing(m)");
                Assert.AreEqual(splitHeaders[13], "Average Circumference(cm)");

            }
            
            File.Delete(filePath + @"\Shelterbelts_Example-en-CA.csv");
        }       

        [TestMethod]
        public void TestExcelInitializer__TestSetTemplateFileForShelterbeltsImperial()
        {
       
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Imperial;

            var excel = new ExcelInitializer();
            var shelterbeltKeys = new ShelterBeltKeys();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\ExcelInitializerTest\Shelterbelts");
            var filePath = @"H.CLI.TestFiles\ExcelInitializerTest\Shelterbelts";
            excel.SetTemplateFile(filePath, "Shelterbelts", shelterbeltKeys.keys);
            var doesTemplateFileExist = File.Exists(filePath + @"\Shelterbelts_Example-en-CA.csv");

            using (File.OpenRead(filePath + @"\Shelterbelts_Example-en-CA.csv"))
            {
                var filelines = File.ReadAllLines(filePath + @"\Shelterbelts_Example-en-CA.csv");
                var splitHeaders = filelines[0].Split(',');
                Assert.AreEqual(splitHeaders[0], "Hardiness Zone");
                Assert.AreEqual(splitHeaders[1], "Ecodistrict ID");
                Assert.AreEqual(splitHeaders[2], "Year Of Observation");
                Assert.AreEqual(splitHeaders[3], "Name");
                Assert.AreEqual(splitHeaders[4], "Row Name");
                Assert.AreEqual(splitHeaders[5], "Row ID");
                Assert.AreEqual(splitHeaders[6], "Row Length(yards)");
                Assert.AreEqual(splitHeaders[7], "Plant Year");
                Assert.AreEqual(splitHeaders[8], "Cut Year");
                Assert.AreEqual(splitHeaders[9], "Species");
                Assert.AreEqual(splitHeaders[10], "Planted Tree Count");
                Assert.AreEqual(splitHeaders[11], "Live Tree Count");
                Assert.AreEqual(splitHeaders[12], "Planted Tree Spacing(yards)");
                Assert.AreEqual(splitHeaders[13], "Average Circumference(in)");
            }

            File.Delete(filePath + @"\Shelterbelts_Example-en-CA.csv");
        }


    }
}