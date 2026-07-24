using System;
using System.IO;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace H.Core.Test.Models
{
    /// <summary>
    /// A component must appear in the serialized farm once. Several members are derived views over
    /// <see cref="Farm.Components"/> or references into it; serializing them wrote a complete extra copy of every
    /// component they covered, which is what made saved farms grow to hundreds of megabytes.
    ///
    /// These tests fail if any of those members starts being serialized again.
    /// </summary>
    [TestClass]
    public class FarmSerializationDuplicationTest
    {
        private static JsonSerializer Serializer()
        {
            return new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };
        }

        private static string Write(Farm farm)
        {
            using (var writer = new StringWriter())
            {
                Serializer().Serialize(writer, farm, typeof(Farm));
                return writer.ToString();
            }
        }

        private static Farm Read(string json)
        {
            using (var reader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return Serializer().Deserialize<Farm>(jsonReader);
            }
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

        /// <summary>A farm with one field and one animal component grazing on it.</summary>
        private static Farm CreateFarm()
        {
            var farm = new Farm { Name = "Farm" };

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

            return farm;
        }

        [TestMethod]
        public void DerivedComponentViewsAreNotSerialized()
        {
            var json = Write(CreateFarm());

            // Each of these projects over Components; serializing one writes the matching components a second time.
            foreach (var derivedView in new[]
                     {
                         nameof(Farm.AnimalComponents),
                         nameof(Farm.DairyComponents),
                         nameof(Farm.FieldSystemComponents),
                         nameof(Farm.AnaerobicDigestionComponents),
                         nameof(Farm.BeefCattleComponents),
                         nameof(Farm.SwineComponents),
                         nameof(Farm.SheepComponents),
                         nameof(Farm.PoultryComponents),
                         nameof(Farm.OtherLivestockComponents),
                     })
            {
                Assert.AreEqual(0, Occurrences(json, "\"" + derivedView + "\":"),
                    $"{derivedView} is a derived view over Components and must not be serialized");
            }
        }

        [TestMethod]
        public void EachComponentIsWrittenOnce()
        {
            var json = Write(CreateFarm());

            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"Timothy pasture\""),
                "the field should be written once");
            Assert.AreEqual(1, Occurrences(json, "\"Name\":\"Cow-calf\""),
                "the animal component should be written once");
        }

        [TestMethod]
        public void PastureLocationIsWrittenAsAGuidNotAsACopyOfTheField()
        {
            var farm = CreateFarm();
            var json = Write(farm);

            Assert.AreEqual(0, Occurrences(json, "\"PastureLocation\":{"),
                "a pasture the farm owns must be stored by guid, not written inline");
            Assert.IsTrue(json.Contains("\"PastureLocationGuid\":"),
                "the pasture must still be identified by guid");

            // And it must survive the round trip as a reference into the farm's own components.
            var restored = Read(json);
            var restoredField = restored.Components.OfType<FieldSystemComponent>().Single();
            var restoredPasture = restored.Components.OfType<AnimalComponentBase>()
                .Single().Groups.Single().ManagementPeriods.Single().HousingDetails.PastureLocation;

            Assert.AreSame(restoredField, restoredPasture,
                "the pasture must be restored as a reference to the farm's field");
        }

        [TestMethod]
        public void CropViewItemsMaxThreeIsNotSerialized()
        {
            var json = Write(CreateFarm());

            Assert.AreEqual(0, Occurrences(json, "\"CropViewItemsMaxThree\":"),
                "CropViewItemsMaxThree is rebuilt from CropViewItems and must not be serialized");
        }

        [TestMethod]
        public void RoundTripPreservesTheFarm()
        {
            var farm = CreateFarm();
            var restored = Read(Write(farm));

            Assert.AreEqual(farm.Components.Count, restored.Components.Count);
            Assert.AreEqual(
                farm.Components.OfType<FieldSystemComponent>().Single().CropViewItems.Count,
                restored.Components.OfType<FieldSystemComponent>().Single().CropViewItems.Count);
            Assert.AreEqual(farm.GetAllManagementPeriods().Count, restored.GetAllManagementPeriods().Count);
        }
    }
}
