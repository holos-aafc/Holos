using H.CLI.FileAndDirectoryAccessors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.UserInput;

namespace H.CLI.Test.FilesAndDirectoryAccessors
{
    [TestClass]
    public class TemplateFileHandlerTest
    {
        [TestMethod]
        public void TestCheckFileNameForComponentType_ExpectShelterBelt()
        {
            var handler = new TemplateFileHandler();
            var directoryKeys = new DirectoryKeys();

            var result = handler.checkFileNameForComponentType(@"H.CLI.Test\TemplateFileHandlerTest\Shelterbelts");
            Assert.AreEqual("Shelterbelts", result);
        }

        [TestMethod]
        public void TestCheckFileNameForComponentType_CaseSensitivity_ExpectShelterBelt()
        {
            var handler = new TemplateFileHandler();
            var directoryKeys = new DirectoryKeys();

            var result = handler.checkFileNameForComponentType(@"H.CLI.Test\TemplateFileHandlerTest\SHElterbelts");
            Assert.AreEqual("Shelterbelts", result);
        }

        [TestMethod]
        public void TestCheckFileNameForComponentType_FilePathContainsShelterbelt_ExpectShelterBelt()
        {
            var handler = new TemplateFileHandler();
            var directoryKeys = new DirectoryKeys();

            var result = handler.checkFileNameForComponentType(@"H.CLI.Test\TemplateFileHandlerTest\HolosShelterbelts");
            Assert.AreEqual("Shelterbelts", result);
        }

        [TestMethod]
        public void TestCheckFileNameForComponentType()
        {
            var handler = new TemplateFileHandler();
            var directoryKeys = new DirectoryKeys();

            var result = handler.checkFileNameForComponentType(@"H.CLI.Test\TemplateFileHandlerTest\1545Shelterbelt67");
            Assert.AreEqual("Shelterbelts", result);
        }

        [TestMethod]
        public void TestValidateTemplateFiles()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestTemplateFileHandler");
            var pathToTemplateFileHandlerFolder = @"H.CLI.TestFiles\TestTemplateFileHandler";
            var directoryHandler = new DirectoryHandler();
            directoryHandler.InitializeDirectoriesAndFilesForComponents(pathToTemplateFileHandlerFolder);
     
            var templateFileHandler = new TemplateFileHandler();
            var validDirectoryPaths = new List<string>()
            {
                @"H.CLI.TestFiles\TestTemplateFileHandler\Shelterbelts",
                @"H.CLI.TestFiles\TestTemplateFileHandler\Beef",                
                @"H.CLI.TestFiles\TestTemplateFileHandler\Dairy",
                @"H.CLI.TestFiles\TestTemplateFileHandler\Fields",
                @"H.CLI.TestFiles\TestTemplateFileHandler\Other Livestock",
                @"H.CLI.TestFiles\TestTemplateFileHandler\Poultry",
                @"H.CLI.TestFiles\TestTemplateFileHandler\Swine",
            };

            templateFileHandler.validateTemplateFiles(validDirectoryPaths);
            Assert.IsTrue(File.Exists(@"H.CLI.TestFiles\TestTemplateFileHandler\Shelterbelts\Shelterbelts_Example-en-CA.csv"));
            Assert.IsTrue(File.Exists(@"H.CLI.TestFiles\TestTemplateFileHandler\Beef\Beef_Example-en-CA.csv"));
            Assert.IsTrue(File.Exists(@"H.CLI.TestFiles\TestTemplateFileHandler\Dairy\Dairy_Example-en-CA.csv"));
            Assert.IsTrue(File.Exists(@"H.CLI.TestFiles\TestTemplateFileHandler\Fields\Fields_Example-en-CA.csv"));
            Assert.IsTrue(File.Exists(@"H.CLI.TestFiles\TestTemplateFileHandler\Other Livestock\Other Livestock_Example-en-CA.csv"));
            Assert.IsTrue(File.Exists(@"H.CLI.TestFiles\TestTemplateFileHandler\Poultry\Poultry_Example-en-CA.csv"));
            Assert.IsTrue(File.Exists(@"H.CLI.TestFiles\TestTemplateFileHandler\Swine\Swine_Example-en-CA.csv"));
        }
            

    }
}
