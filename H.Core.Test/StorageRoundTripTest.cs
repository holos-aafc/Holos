using System;
using System.IO;
using System.Linq;
using H.Core;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test
{
    /// <summary>
    /// End-to-end coverage of <see cref="Storage.Save"/> and <see cref="Storage.Load"/>. The other serialization tests
    /// drive a <c>JsonSerializer</c> directly, so nothing else checks that Storage's own reader and writer are
    /// configured the same way.
    ///
    /// Storage is redirected to a temporary folder for these. It must never run against the real one - Save overwrites
    /// the user's data file and Load deletes it when a file cannot be read.
    /// </summary>
    [TestClass]
    public class StorageRoundTripTest
    {
        private string _folder;

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

        [TestInitialize]
        public void TestInitialize()
        {
            _folder = Path.Combine(Path.GetTempPath(), "holos-storage-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_folder);
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

        /// <summary>Two farms; the second is active and has an animal group grazing on its own field.</summary>
        private static ApplicationData CreateApplicationData()
        {
            var data = new ApplicationData();

            var other = new Farm { Name = "Other farm" };
            data.Farms.Add(other);

            var farm = new Farm { Name = "Main farm" };

            var field = new FieldSystemComponent { Name = "Timothy pasture" };
            field.CropViewItems.Add(new CropViewItem { CropType = CropType.TameGrass, Year = 2024 });
            farm.Components.Add(field);

            var managementPeriod = new ManagementPeriod { Name = "Summer pasture" };
            managementPeriod.HousingDetails.HousingType = HousingType.Pasture;
            managementPeriod.HousingDetails.PastureLocation = field;

            var group = new AnimalGroup { Name = "Cows" };
            group.ManagementPeriods.Add(managementPeriod);

            var animalComponent = new CowCalfComponent { Name = "Cow-calf" };
            animalComponent.Groups.Add(group);
            farm.Components.Add(animalComponent);

            data.Farms.Add(farm);

            data.GlobalSettings.ActiveFarm = farm;
            data.GlobalSettings.FarmsForComparison.Add(other);

            return data;
        }

        private ApplicationData SaveThenLoad()
        {
            var saving = new RedirectedStorage(_folder) { ApplicationData = CreateApplicationData() };
            saving.Save();

            var loading = new RedirectedStorage(_folder);
            loading.Load();

            Assert.IsTrue(loading.IsDataLoaded, "Storage reported the data was not loaded");

            return loading.ApplicationData;
        }

        [TestMethod]
        public void SaveThenLoadPreservesTheFarms()
        {
            var loaded = SaveThenLoad();

            CollectionAssert.AreEqual(
                new[] { "Other farm", "Main farm" },
                loaded.Farms.Select(x => x.Name).ToArray(),
                "the farms should come back unchanged, in order");

            var farm = loaded.Farms.Single(x => x.Name == "Main farm");
            Assert.AreEqual(2, farm.Components.Count);
            Assert.AreEqual(1, farm.Components.OfType<FieldSystemComponent>().Single().CropViewItems.Count);
            Assert.AreEqual(1, farm.GetAllManagementPeriods().Count);
        }

        [TestMethod]
        public void SaveThenLoadRestoresTheReferences()
        {
            var loaded = SaveThenLoad();
            var farm = loaded.Farms.Single(x => x.Name == "Main farm");

            Assert.AreSame(farm, loaded.GlobalSettings.ActiveFarm,
                "the active farm must be the instance held in Farms");

            Assert.AreSame(loaded.Farms.Single(x => x.Name == "Other farm"),
                loaded.GlobalSettings.FarmsForComparison.Single(),
                "a farm selected for comparison must be the instance held in Farms");

            Assert.AreSame(farm.Components.OfType<FieldSystemComponent>().Single(),
                farm.GetAllManagementPeriods().Single().HousingDetails.PastureLocation,
                "the pasture must be the field held in the farm's components");
        }

        [TestMethod]
        public void TheSavedFileDoesNotDuplicateFarmsOrComponents()
        {
            var saving = new RedirectedStorage(_folder) { ApplicationData = CreateApplicationData() };
            saving.Save();

            var json = File.ReadAllText(saving.GetFullPathToStorageFile());

            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"Main farm\""),
                "the active farm was written more than once");
            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"Other farm\""),
                "the farm selected for comparison was written more than once");
            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"Timothy pasture\""),
                "the field was written more than once");
            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"Cow-calf\""),
                "the animal component was written more than once");
        }

        [TestMethod]
        public void SavingTwiceDoesNotGrowTheFile()
        {
            var storage = new RedirectedStorage(_folder) { ApplicationData = CreateApplicationData() };

            storage.Save();
            var first = new FileInfo(storage.GetFullPathToStorageFile()).Length;

            // Load and save again, as opening and saving the file in the application would.
            var reopened = new RedirectedStorage(_folder);
            reopened.Load();
            reopened.Save();
            var second = new FileInfo(reopened.GetFullPathToStorageFile()).Length;

            var thirdStorage = new RedirectedStorage(_folder);
            thirdStorage.Load();
            thirdStorage.Save();
            var third = new FileInfo(thirdStorage.GetFullPathToStorageFile()).Length;

            Assert.AreEqual(first, second, "the file grew after being loaded and saved once");
            Assert.AreEqual(second, third, "the file grew after being loaded and saved twice");
        }

        private static int Occurrences(string text, string needle)
        {
            var count = 0;
            for (var i = text.IndexOf(needle, StringComparison.Ordinal); i >= 0;
                 i = text.IndexOf(needle, i + 1, StringComparison.Ordinal))
            {
                count++;
            }

            return count;
        }
    }
}
