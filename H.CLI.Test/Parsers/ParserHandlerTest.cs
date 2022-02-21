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
using H.CLI.Parser;
using H.CLI.TemporaryComponentStorage;
using H.CLI.UserInput;

namespace H.CLI.Test.Parsers
{
    [TestClass]
    public class ParserHandlerTest
    {

        [TestMethod]
        public void InitializeParser_ExpectParsedResultsToBe1()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            var parserHandler = new ParserHandler();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestParserHandler");

            var excel = new ExcelInitializer();
            var shelterBeltKeys = new ShelterBeltKeys();
            var componentDirectoryPath = @"H.CLI.TestFiles\TestParserHandler";
        
            excel.SetTemplateCSVFileForTesting(componentDirectoryPath, shelterBeltKeys.keys);
   
            parserHandler.InitializeParser("Shelterbelts");

            var files = Directory.GetFiles(componentDirectoryPath);
            var parsedComponentList = parserHandler.StartParsing(files);
            Assert.IsInstanceOfType(parsedComponentList[0][0], typeof(ShelterBeltTemporaryInput));
        }

    }
}
