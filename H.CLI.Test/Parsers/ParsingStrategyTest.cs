using H.CLI.UserInput;
using H.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using H.CLI.ComponentKeys;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Parser;
using H.CLI.TemporaryComponentStorage;

namespace H.CLI.Test.Parsers
{
    [TestClass]
    public class ParsingStrategyTest
    {
        [TestMethod]
        public void TestSetComponentKey_SetToShelterbeltKeys_ExpectComponentKeyToBeTypeOfShelterBeltKeys()
        {
            var parsingStrategy = new ParsingStrategy();
            parsingStrategy.SetComponentKey(new ShelterBeltKeys());
            Assert.IsInstanceOfType(parsingStrategy._parser.ComponentKey, typeof(ShelterBeltKeys));
        }

        [TestMethod]
        public void TestSetComponentTemporaryInput_SetToShelterbeltTemporaryInput_ExpectTemporaryInputToBeTypeOfShelterBeltTemporaryInput()
        {
            var parsingStrategy = new ParsingStrategy();
            parsingStrategy.SetComponentTemporaryInput(new ShelterBeltTemporaryInput());
            Assert.IsInstanceOfType(parsingStrategy._parser.ComponentTemporaryInput, typeof(ShelterBeltTemporaryInput));
        }


        [TestMethod]
        public void TestGetParsedComponentList_ExpectShelterbeltInList()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            CLIUnitsOfMeasurementConstants.measurementSystem = MeasurementSystemType.Metric;

            var excel = new ExcelInitializer();
            var shelterbeltKeys = new ShelterBeltKeys();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestParsingStrategy");
            var filePath = @"H.CLI.TestFiles\TestParsingStrategy";
            excel.SetTemplateCSVFileForTesting(filePath, shelterbeltKeys.keys);
            var files = Directory.GetFiles(filePath);

            var parserStrategy = new ParsingStrategy();
            parserStrategy.SetComponentTemporaryInput(new ShelterBeltTemporaryInput());
            parserStrategy.SetComponentKey(new ShelterBeltKeys());
            var result = parserStrategy.GetParsedComponentList(files);

            Assert.AreEqual(result.Count, 1);

        }

     
    }
}
