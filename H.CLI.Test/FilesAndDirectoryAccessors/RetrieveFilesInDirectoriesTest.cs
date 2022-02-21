using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using H.CLI.ComponentKeys;
using H.CLI.FileAndDirectoryAccessors;

namespace H.CLI.Test.FilesAndDirectoryAccessors
{
    [TestClass]
    public class RetrieveFilesInDirectoriesTest
    {
        [TestMethod]
        public void TestGetSubDirectoryFiles_ExpectDirectoryNotFound_InvalidDirectoryPath()
        {
             var testDirectory = new RetrieveFilesInDirectories();
            Assert.ThrowsException<DirectoryNotFoundException>(() => testDirectory.GetSubDirectoryFiles("invalid path"));
        }

        [TestMethod]
        public void TestGetSubDirectoryFiles_ValidDirectory_ExpectListOfFilesInThatDirectory()
        {
            var excel = new ExcelInitializer();
            var directoryHandler = new DirectoryHandler();
            var shelterbeltKeys = new ShelterBeltKeys();
            Directory.CreateDirectory("H.CLI.TestFiles");
            Directory.CreateDirectory(@"H.CLI.TestFiles\TestRetrieveFilesInDirectory");
            directoryHandler.InitializeDirectoriesAndFilesForComponents(@"H.CLI.TestFiles\TestRetrieveFilesInDirectory");
            excel.SetTemplateCSVFileForTesting(@"H.CLI.TestFiles\TestRetrieveFilesInDirectory\Shelterbelts", shelterbeltKeys.keys);
            var testDirectory = new RetrieveFilesInDirectories();
            var result = testDirectory.GetSubDirectoryFiles(@"H.CLI.TestFiles\TestRetrieveFilesInDirectory\Shelterbelts");
            Assert.AreEqual(result.Length, 2);
        }

    }
}
