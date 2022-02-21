using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.ComponentKeys;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.Handlers;
using H.CLI.TemporaryComponentStorage;
using H.CLI.UserInput;

namespace H.CLI.Test.Handlers
{
    
    [TestClass]
    public class GuidComponentHandlerTest
    { 
        [TestMethod]
        public void TestGuidComponentHandler_ExpectGuidsToBeProperlySet()
        {
            CLILanguageConstants.culture = CultureInfo.CreateSpecificCulture("en-CA");
            CLILanguageConstants.Delimiter = ",";

            var shelterBeltkeys = new ShelterBeltKeys();
            var excel = new ExcelInitializer();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\GuidComponentHandlerTest");
            var filePath = @"H.CLI.TestFiles\GuidComponentHandlerTest";
            var fileList = new string[]
            {
                @"H.CLI.TestFiles\GuidComponentHandlerTest\Shelterbelt1.csv"
            };

            excel.SetTemplateCSVFileForTesting(filePath, shelterBeltkeys.keys);

            var parser = new Parser.Parser();
            parser.ComponentTemporaryInput = new ShelterBeltTemporaryInput();
            parser.ComponentKey = new ShelterBeltKeys();
            var results = parser.Parse(fileList);

            var guidHandler = new GuidComponentHandler();
            var guidResults = guidHandler.GenerateComponentGUIDs(results[0]);

            //Line 1: Row ID = 1, Line 2: Row ID = 2, Line 3: Row ID = 1
            Assert.AreNotEqual(guidResults[0].Guid, guidResults[1].Guid);
            Assert.AreEqual(guidResults[0].Guid, guidResults[2].Guid);
        }

    }
    
}
