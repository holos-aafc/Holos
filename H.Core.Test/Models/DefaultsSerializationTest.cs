using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using H.Core;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H.Core.Test.Models
{
    /// <summary>
    /// A farm file written by an earlier version carries whatever properties <see cref="Defaults"/> had at the time.
    /// Loading one must not fail because the class has since lost a property.
    ///
    /// These read through <see cref="Storage.GetFarmsFromExportFile"/> rather than a serializer the test configures
    /// itself, so what they cover is the reader a farm file actually meets. A serializer setting that turned an
    /// unknown name into an error would break every saved farm, and that reader swallows the failure and returns no
    /// farms at all, so the break would be silent.
    /// </summary>
    [TestClass]
    public class DefaultsSerializationTest
    {
        #region Fields

        private string _folder;
        private Storage _storage;

        #endregion

        #region Initialization

        [TestInitialize]
        public void TestInitialize()
        {
            _folder = Path.Combine(Path.GetTempPath(), "holos-defaults-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_folder);

            _storage = new Storage();
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

        /// <summary>
        /// DefaultNitrogenFixation is stored on every farm saved before it was removed. Nitrogen fixation is a
        /// property of the crop, read from the view item.
        /// </summary>
        [TestMethod]
        public void AFarmSavedWithARemovedPropertyStillLoads()
        {
            var path = WriteExportFile(defaults => defaults["DefaultNitrogenFixation"] = 0.7);

            var farms = _storage.GetFarmsFromExportFile(path).ToList();

            Assert.AreEqual(1, farms.Count, "the farm did not load - the reader returns no farms when it fails");
        }

        /// <summary>
        /// The unknown name must be skipped rather than stopping the read, so the settings around it still arrive.
        /// </summary>
        [TestMethod]
        public void SettingsAroundARemovedPropertyAreStillRead()
        {
            var path = WriteExportFile(defaults =>
            {
                defaults["DefaultNitrogenFixation"] = 0.7;
                defaults["CarbonConcentration"] = 0.42;
                defaults["FTopo"] = 99.5;
            });

            var farm = _storage.GetFarmsFromExportFile(path).Single();

            Assert.AreEqual(0.42, farm.Defaults.CarbonConcentration, 0.0001);
            Assert.AreEqual(99.5, farm.Defaults.FTopo, 0.0001);
        }

        /// <summary>
        /// Nothing in the class answers to the removed name, so a farm saved now does not carry it.
        /// </summary>
        [TestMethod]
        public void ARemovedPropertyIsNotWrittenBackOut()
        {
            var json = JsonConvert.SerializeObject(new Defaults(), Settings());

            StringAssert.DoesNotMatch(json, new System.Text.RegularExpressions.Regex("DefaultNitrogenFixation"));
        }

        #endregion

        #region Private Methods

        private static JsonSerializerSettings Settings()
        {
            return new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Auto};
        }

        /// <summary>
        /// Writes a farm export file, letting the caller add or change entries under Defaults first so the file can
        /// hold names the current class does not define.
        /// </summary>
        private string WriteExportFile(Action<JObject> configureDefaults)
        {
            var json = JsonConvert.SerializeObject(new List<Farm>() {new Farm()}, Settings());

            var farms = JArray.Parse(json);
            var defaults = (JObject)farms[0]["Defaults"];

            configureDefaults(defaults);

            var path = Path.Combine(_folder, "export.json");
            File.WriteAllText(path, farms.ToString());

            return path;
        }

        #endregion
    }
}
