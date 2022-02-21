using H.CLI.UserInput;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using H.CLI.ComponentKeys;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.TemporaryComponentStorage;

namespace H.CLI.Test.Parsers
{
    [TestClass]
    public class ParserTest
    {
        private static readonly Parser.Parser parser = new Parser.Parser();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestParser");
         
            parser.ComponentTemporaryInput = new ShelterBeltTemporaryInput();
            parser.ComponentKey = new ShelterBeltKeys();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }

        [TestMethod]
        public void TestParser_ExpectShelterbeltComponentFieldsToBeProperlySet()
        {
            CLILanguageConstants.culture = CultureInfo.CreateSpecificCulture("en-CA");
            CLILanguageConstants.Delimiter = ",";
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Metric;
        
            var excel = new ExcelInitializer();
            var shelterbeltKeys = new ShelterBeltKeys();
            excel.SetTemplateCSVFileForTesting(@"H.CLI.TestFiles\TestParser", shelterbeltKeys.keys);


            string[] fileList = new string[]
            {
                @"H.CLI.TestFiles\TestParser\Shelterbelt1.csv"
            };

      
            var results = parser.Parse(fileList);
            var tempShelterbeltList = new List<List<ShelterBeltTemporaryInput>>();
            foreach (var list in results)
            {
                tempShelterbeltList.Add(list.Cast<ShelterBeltTemporaryInput>().ToList());
            }
                Assert.AreEqual(tempShelterbeltList[0][0].HardinessZone, Core.Enumerations.HardinessZone.H4b);
                Assert.AreEqual(tempShelterbeltList[0][0].EcodistrictID, 754);
                Assert.AreEqual(tempShelterbeltList[0][0].Name, "Shelterbelt1");
                Assert.AreEqual(tempShelterbeltList[0][0].RowName, "Caragana");
                Assert.AreEqual(tempShelterbeltList[0][0].RowID, 1);
                Assert.AreEqual(tempShelterbeltList[0][0].RowLength, 100);
                Assert.AreEqual(tempShelterbeltList[0][0].PlantYear, 1996);
                Assert.AreEqual(tempShelterbeltList[0][0].CutYear, 2019);

                Assert.AreEqual(tempShelterbeltList[0][0].Species, Core.Enumerations.TreeSpecies.Caragana);
                Assert.AreEqual(tempShelterbeltList[0][0].PlantedTreeCount, 138);
                Assert.AreEqual(tempShelterbeltList[0][0].PlantedTreeSpacing, 113);
                Assert.AreEqual(tempShelterbeltList[0][0].LiveTreeCount, 0);
                Assert.AreEqual(tempShelterbeltList[0][0].AverageCircumference, 34.508);      
        }


        [TestMethod]
        public void TestParserHeaderExceptions_InvalidHeader_Expect_ThrowIndexOutOfRangeException()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));

            var excel = new ExcelInitializer();
            var filePath = @"H.CLI.TestFiles\TestParser\Shelterbelt1.csv";
   
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("NotAValidHeader" + ",");
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("H3b" + ",");
            File.WriteAllText(filePath, stringBuilder.ToString());

            string[] fileList = new string[]
            {
                filePath
            };

    
            Assert.ThrowsException<IndexOutOfRangeException>(() => parser.Parse(fileList));
        }

        
        [TestMethod]
        public void TestParserHeaderExceptions_HeaderIsWhiteSpace_Expect_ThrowIndexOutOfRangeException()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestParser\Shelterbelts");

            var excel = new ExcelInitializer();
            var filePath = @"H.CLI.TestFiles\TestParser\Shelterbelts\Shelterbelt1.csv";
           
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(" " + ",");
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("H3b" + ",");
            File.WriteAllText(filePath, stringBuilder.ToString());

            string[] fileList = new string[]
            {
                filePath
            };

     
            Assert.ThrowsException<IndexOutOfRangeException>(() => parser.Parse(fileList));
        }

    }
}
