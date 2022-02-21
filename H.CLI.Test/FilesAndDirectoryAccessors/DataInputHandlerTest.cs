using H.CLI.FileAndDirectoryAccessors;
using H.Core.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.CLI.ComponentKeys;
using H.CLI.UserInput;
using H.Core.Models;

namespace H.CLI.Test.FilesAndDirectoryAccessors
{
    [TestClass]
    public class DataInputHandlerTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestDataInputHandler\ExampleFarm\Shelterbelts");
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            var componentFiles = Directory.GetFiles(@"H.CLI.TestFiles\TestDataInputHandler\ExampleFarm\Shelterbelts");
            foreach (var file in componentFiles)
            {
                File.Delete(file);
            }
           
        }

        [TestMethod]
        public void TestProcessDataInputFiles_OneFarmWithShelterbeltComponent_ExpectFarmToContainOneShelterbeltComponent()
        {
            CLILanguageConstants.SetCulture(CultureInfo.GetCultureInfo("en-CA"));
            var farmDirectoryPath = @"H.CLI.TestFiles\TestDataInputHandler\ExampleFarm";
            var testFilePath = @"H.CLI.TestFiles\TestDataInputHandler\ExampleFarm\Shelterbelts";

            var dataHandler = new DataInputHandler();
            var excel = new ExcelInitializer();
            excel.SetTemplateCSVFileForTesting(testFilePath, new ShelterBeltKeys().keys);
            var result = dataHandler.ProcessDataInputFiles(farmDirectoryPath);
            Assert.AreEqual(result.Components.Count(), 1);
            Assert.AreEqual(result.Components[0].ComponentType, ComponentType.Shelterbelt);
        }

        [TestMethod]
        public void TestProcessDataInputFiles_NoFiles_ExpectEmptyFarm()
        {
            CLILanguageConstants.SetCulture(CultureInfo.GetCultureInfo("en-CA"));
            var farmDirectoryPath = @"H.CLI.TestFiles\TestDataInputHandler\ExampleFarm";

            var dataHandler = new DataInputHandler();
            var excel = new ExcelInitializer();

            var result = dataHandler.ProcessDataInputFiles(farmDirectoryPath);
            Assert.AreEqual(result.Components.Count(), 0);

        }
    }
}
