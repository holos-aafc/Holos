using System.IO;
using System.Linq;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace H.Core.Test.Models
{
    /// <summary>
    /// The active farm and the farms selected for comparison are references into <see cref="ApplicationData.Farms"/>.
    /// They must be persisted as guids and restored as references, not written as extra copies.
    /// </summary>
    [TestClass]
    public class ApplicationDataFarmReferenceTest
    {
        private static JsonSerializer Serializer()
        {
            return new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };
        }

        private static string Write(ApplicationData data)
        {
            using (var writer = new StringWriter())
            {
                Serializer().Serialize(writer, data, typeof(ApplicationData));
                return writer.ToString();
            }
        }

        private static ApplicationData Read(string json)
        {
            using (var reader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return Serializer().Deserialize<ApplicationData>(jsonReader);
            }
        }

        private static ApplicationData CreateApplicationData()
        {
            var data = new ApplicationData();

            var first = new Farm { Name = "First" };
            var second = new Farm { Name = "Second" };
            var third = new Farm { Name = "Third" };

            data.Farms.Add(first);
            data.Farms.Add(second);
            data.Farms.Add(third);

            data.GlobalSettings.ActiveFarm = second;
            data.GlobalSettings.FarmsForComparison.Add(first);
            data.GlobalSettings.FarmsForComparison.Add(third);

            return data;
        }

        [TestMethod]
        public void ActiveFarmSurvivesAsAReference()
        {
            var restored = Read(Write(CreateApplicationData()));

            Assert.AreEqual("Second", restored.GlobalSettings.ActiveFarm.Name,
                "the farm that was active must still be active");
            Assert.IsTrue(restored.Farms.Any(x => ReferenceEquals(x, restored.GlobalSettings.ActiveFarm)),
                "the active farm must be one of the farms, not a copy");
        }

        [TestMethod]
        public void FarmsForComparisonSurviveAsReferences()
        {
            var restored = Read(Write(CreateApplicationData()));
            var comparison = restored.GlobalSettings.FarmsForComparison;

            CollectionAssert.AreEqual(
                new[] { "First", "Third" },
                comparison.Select(x => x.Name).ToArray(),
                "the comparison selection must be preserved, in order");

            foreach (var farm in comparison)
            {
                Assert.IsTrue(restored.Farms.Any(x => ReferenceEquals(x, farm)),
                    $"'{farm.Name}' must be a reference into Farms, not a copy");
            }
        }

        [TestMethod]
        public void FarmsAreNotWrittenMoreThanOnce()
        {
            var json = Write(CreateApplicationData());

            // Each farm object carries a Name; a farm written once appears once.
            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"Second\""),
                "the active farm was written more than once");
            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"First\""),
                "a farm selected for comparison was written more than once");
            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"Third\""),
                "a farm selected for comparison was written more than once");
        }

        private static int Occurrences(string text, string needle)
        {
            var count = 0;
            for (var i = text.IndexOf(needle, System.StringComparison.Ordinal); i >= 0;
                 i = text.IndexOf(needle, i + 1, System.StringComparison.Ordinal))
            {
                count++;
            }

            return count;
        }
    }
}
