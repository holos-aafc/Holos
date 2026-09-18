using System;
using System.IO;
using H.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test
{
    /// <summary>
    /// Coverage of <see cref="Storage.WriteExceptionToFile"/> against a data folder that does not exist yet, which is
    /// the state of a machine Holos has not run on. Failing there would raise a second exception out of the handler
    /// reporting the first, so the folder has to be created rather than assumed.
    ///
    /// Storage is redirected to a temporary folder for these. It must never run against the real one.
    /// </summary>
    [TestClass]
    public class StorageLogFileTest
    {
        #region Fields

        private string _folder;

        #endregion

        #region Inner Classes

        /// <summary>Storage writing to a throwaway folder instead of the user's AppData.</summary>
        private class RedirectedStorage : Storage
        {
            private readonly string _folder;

            public RedirectedStorage(string folder)
            {
                _folder = folder;
            }

            protected override string GetUserFolderPath(bool isBackupFolder = false)
            {
                return isBackupFolder ? Path.Combine(_folder, "backups") : _folder;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Deliberately does not create the folder - the point of these tests is what happens when it is absent.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _folder = Path.Combine(Path.GetTempPath(), "holos-logfile-test-" + Guid.NewGuid().ToString("N"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                if (Directory.Exists(_folder))
                {
                    Directory.Delete(_folder, recursive: true);
                }
            }
            catch (IOException)
            {
                // A leftover temp folder is not worth failing a test over.
            }
        }

        #endregion

        #region Tests

        [TestMethod]
        public void WriteExceptionToFileCreatesTheDataFolderWhenItDoesNotExist()
        {
            var storage = new RedirectedStorage(_folder);

            Assert.IsFalse(Directory.Exists(_folder), "the folder must be absent for this test to mean anything");

            storage.WriteExceptionToFile(new Exception("this is a test exception"));

            Assert.IsTrue(File.Exists(Path.Combine(_folder, "logfile.txt")));
        }

        [TestMethod]
        public void WriteExceptionToFileRecordsTheException()
        {
            var storage = new RedirectedStorage(_folder);

            storage.WriteExceptionToFile(new Exception("first failure"));

            var contents = File.ReadAllText(Path.Combine(_folder, "logfile.txt"));

            StringAssert.Contains(contents, "first failure");
        }

        /// <summary>
        /// The log accumulates - a second exception must not replace the first.
        /// </summary>
        [TestMethod]
        public void WriteExceptionToFileAppendsToAnExistingLog()
        {
            var storage = new RedirectedStorage(_folder);

            storage.WriteExceptionToFile(new Exception("first failure"));
            storage.WriteExceptionToFile(new Exception("second failure"));

            var contents = File.ReadAllText(Path.Combine(_folder, "logfile.txt"));

            StringAssert.Contains(contents, "first failure");
            StringAssert.Contains(contents, "second failure");
        }

        #endregion
    }
}
