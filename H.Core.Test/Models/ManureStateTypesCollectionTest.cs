using System.IO;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace H.Core.Test.Models
{
    /// <summary>
    /// The valid-state-type collections are seeded in their constructors, so without ObjectCreationHandling.Replace
    /// Json.NET adds the values from file on top of the seed and the collection grows by one on every load and save.
    /// </summary>
    [TestClass]
    public class ManureStateTypesCollectionTest
    {
        private static JsonSerializer Serializer()
        {
            return new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };
        }

        private static T RoundTrip<T>(T value)
        {
            string json;
            using (var writer = new StringWriter())
            {
                Serializer().Serialize(writer, value, typeof(T));
                json = writer.ToString();
            }

            using (var reader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return Serializer().Deserialize<T>(jsonReader);
            }
        }

        [TestMethod]
        public void ManureApplicationViewItemStateTypesDoNotGrowOnReload()
        {
            var item = new ManureApplicationViewItem();
            item.ValidManureStateTypesForSelectedTypeOfAnimalManure.Clear();
            item.ValidManureStateTypesForSelectedTypeOfAnimalManure.Add(ManureStateType.DeepBedding);
            item.ValidManureStateTypesForSelectedTypeOfAnimalManure.Add(ManureStateType.Liquid);

            var before = item.ValidManureStateTypesForSelectedTypeOfAnimalManure.ToList();

            var reloaded = RoundTrip(item);

            CollectionAssert.AreEqual(before,
                reloaded.ValidManureStateTypesForSelectedTypeOfAnimalManure.ToList(),
                "the collection must come back exactly as saved, with no seeded entry added");
        }

        [TestMethod]
        public void ManureSubstrateViewItemStateTypesDoNotGrowOnReload()
        {
            var item = new ManureSubstrateViewItem();
            item.ValidManureStateTypesForSelectedTypeOfAnimalManure.Clear();
            item.ValidManureStateTypesForSelectedTypeOfAnimalManure.Add(ManureStateType.DeepBedding);

            var before = item.ValidManureStateTypesForSelectedTypeOfAnimalManure.ToList();

            var reloaded = RoundTrip(item);

            CollectionAssert.AreEqual(before,
                reloaded.ValidManureStateTypesForSelectedTypeOfAnimalManure.ToList(),
                "the collection must come back exactly as saved, with no seeded entry added");
        }

        [TestMethod]
        public void RepeatedReloadsDoNotAccumulateEntries()
        {
            var item = new ManureApplicationViewItem();
            item.ValidManureStateTypesForSelectedTypeOfAnimalManure.Clear();
            item.ValidManureStateTypesForSelectedTypeOfAnimalManure.Add(ManureStateType.Solid);

            var once = RoundTrip(item);
            var twice = RoundTrip(once);
            var thrice = RoundTrip(twice);

            Assert.AreEqual(1, thrice.ValidManureStateTypesForSelectedTypeOfAnimalManure.Count,
                "the collection grew across repeated load and save cycles");
        }
    }
}
