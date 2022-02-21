using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using H.CLI.ComponentKeys;
using H.CLI.FileAndDirectoryAccessors;
using H.CLI.UserInput;
using H.Core.Models;
using H.Core.Models.LandManagement.Shelterbelt;

namespace H.CLI.Test.FilesAndDirectoryAccessors
{
    [TestClass]
    public class ReadSettingsFileTest
    {
    
        [TestMethod]
        public void TestReadGlobalSettings_ExpectGlobalSettingsToBeRead()
        {
            var excel = new ExcelInitializer();
            var directoryHandler = new DirectoryHandler();
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\ReadGlobalSettingsTest");
            directoryHandler.GenerateGlobalSettingsFile(@"H.CLI.TestFiles\ReadGlobalSettingsTest", new Farm());
            var settingsfilePath = @"H.CLI.TestFiles\ReadGlobalSettingsTest\farm.settings";
            var reader = new ReadSettingsFile();
            reader.ReadGlobalSettings(settingsfilePath);
            Assert.AreEqual(reader.GlobalSettingsDictionary.Count, 92);
            Assert.AreEqual(reader.GlobalSettingsDictionary["Ripening Day"],"197");
            Assert.AreEqual(reader.GlobalSettingsDictionary["Alfa"], "0.7");
        }

        [TestMethod] 
        public void TestParseFarmComponentPaths_ValidPathToFarmAnd1FileToBeParsed_ExpectFarmComponentsListToHave1Component()
        {
            CLILanguageConstants.SetCulture(CultureInfo.CreateSpecificCulture("en-CA"));

            var directoryHandler = new DirectoryHandler();
            var excel = new ExcelInitializer();
            var shelterbeltKeys = new ShelterBeltKeys();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestFarms");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestFarms\Farm1\");

            var farmPath = @"H.CLI.TestFiles\TestFarms\Farm1\";
           // directoryHandler.GenerateGlobalSettingsFile(farmPath);
            directoryHandler.InitializeDirectoriesAndFilesForComponents(farmPath);
            excel.SetTemplateCSVFileForTesting(farmPath + "Shelterbelts", shelterbeltKeys.keys);

            var dataHandler = new DataInputHandler();
            var farm = dataHandler.ProcessDataInputFiles(farmPath);

            Assert.AreEqual(farm.Components.Count, 1);
            Assert.IsInstanceOfType(farm.Components[0], typeof(ShelterbeltComponent));
        }
   
    }
}
