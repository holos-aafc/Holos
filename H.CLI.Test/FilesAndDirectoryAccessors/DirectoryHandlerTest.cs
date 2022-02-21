using H.CLI.FileAndDirectoryAccessors;
using H.Core.Providers;
using H.Core.Providers.Soil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.CLI.Test.FilesAndDirectoryAccessors
{
    [TestClass]
    public class DirectoryHandlerTest
    {
        [TestMethod]
        public void TestInitializeDirectoriesAndFiles_ADirectoryPathIsNotValid_ThrowsExceptions()
        {
            var directoryHandler = new DirectoryHandler();
            Assert.ThrowsException<DirectoryNotFoundException>(() => directoryHandler.InitializeDirectoriesAndFilesForComponents("123123"));
        }

        [TestMethod]
        public void TestValidateDirectories_CreatesAllDirectories_ExpectDirectoryCountToBeTheNumberOfDirectoryKeys()
        {
            var directoryKeys = new DirectoryKeys();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestDirectoryHandler");
            var baseDirectoryPath = @"H.CLI.TestFiles\TestDirectoryHandler";
            var directoryHandler = new DirectoryHandler();
            directoryHandler.ValidateComponentDirectories(baseDirectoryPath);
            var numberOfDirectories = Directory.GetDirectories(baseDirectoryPath);
            Assert.AreEqual(directoryKeys.directoryWeights.Keys.Count(), numberOfDirectories.Count());

            foreach (var subdirectory in numberOfDirectories)
            {
                Directory.Delete(subdirectory);
            }

        }


        [TestMethod]
        public void TestValidateUserInputedDirectories_ExpectUserDirectoryToBeMadeBecauseItsNotThereInitially()
        {
            var directoryKeys = new DirectoryKeys();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestDirectoryHandlerUser");
            var directoryHandler = new DirectoryHandler();
            var userInputedPathForOutput = @"H.CLI.TestFiles\TestDirectoryHandlerUser\UserInputedPathForOutput";
            var farmName = "FarmExample1";
            directoryHandler.ValidateAndCreateLandManagementDirectories(userInputedPathForOutput, farmName);

            var tempDirectories = Directory.GetDirectories(userInputedPathForOutput);
            Assert.IsTrue(Directory.Exists(userInputedPathForOutput));
            foreach (var farmDirectory in tempDirectories)
            {
                var componentDirectories = Directory.GetDirectories(farmDirectory);
                foreach (var componentDirectory in componentDirectories)
                {
                    Directory.Delete(componentDirectory, true);
                }
                Directory.Delete(farmDirectory);
            }

            Directory.Delete(userInputedPathForOutput);

        }

        [TestMethod]
        public void TestPrioritizeComponentDirectoryPaths()
        {
            var directoryHandler = new DirectoryHandler();
            //Shelterbelts and Fields have a weight of 2, the other components have a weight of 1.
            var componentPathList = new List<string>()
            {
                @"C:\Holos\HolosCommandLineInterface\Farms\HolosExampleFarm1\Shelterbelts",
                @"C:\Holos\HolosCommandLineInterface\Farms\HolosExampleFarm1\Dairys",
                @"C:\Holos\HolosCommandLineInterface\Farms\HolosExampleFarm1\Swines",
                @"C:\Holos\HolosCommandLineInterface\Farms\HolosExampleFarm1\Poultrys",
                @"C:\Holos\HolosCommandLineInterface\Farms\HolosExampleFarm1\Fields",
            };

           var prioritizedComponentPathList = directoryHandler.prioritizeDirectoryKeys(componentPathList);
            Assert.AreEqual(prioritizedComponentPathList[0] == @"C:\Holos\HolosCommandLineInterface\Farms\HolosExampleFarm1\Shelterbelts"
                           ||
                            prioritizedComponentPathList[0] == @"C:\Holos\HolosCommandLineInterface\Farms\HolosExampleFarm1\Fields", true);
        }
    }
}
