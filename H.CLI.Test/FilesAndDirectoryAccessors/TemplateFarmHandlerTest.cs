using H.CLI.FileAndDirectoryAccessors;
using H.Core.Providers;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.Test.FilesAndDirectoryAccessors
{
    [TestClass]
    public class TemplateFarmHandlerTest
    {
        /*
        [TestMethod] 
        public void TestTemplateFarmHandler()
        {
            CLILanguageConstants.OutputLanguageAddOn = "-en-CA.csv";
            CLILanguageConstants.DisplayDataSeparator = ".";
            CLILanguageConstants.culture = CultureInfo.CreateSpecificCulture("en-CA");
            CLILanguageConstants.Delimiter = ",";
            var geographicDataProvider = new GeographicDataProvider(new MonthlyNormalsDataProvider(), new MonthlyNormalsDataProvider(), new MonthlyNormalsDataProvider(), new NationalSoilDataBaseProvider());
            geographicDataProvider.Initialize();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestTemplateFarmHandler\Farms");
            var pathToFarmsDirectory = @"H.CLI.TestFiles\TestTemplateFarmHandler\Farms";
            var templateFarmHandler = new TemplateFarmHandler();
          
            templateFarmHandler.CreateTemplateFarm(pathToFarmsDirectory, geographicDataProvider);
            var oneFarmMade = Directory.GetDirectories(pathToFarmsDirectory).Count();
            Assert.AreEqual(oneFarmMade, 1);
            var numberOfComponents = Directory.GetDirectories(pathToFarmsDirectory + @"\HolosExampleFarm");
            Assert.AreEqual(numberOfComponents.Count(), 8);
            Assert.IsTrue(File.Exists(pathToFarmsDirectory + @"\HolosExampleFarm\nameofsettingsfile.txt"));
        }
        */

    }
}
