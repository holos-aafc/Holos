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
    /// A farm copied before the replication fix references the original farm's field. On load that reference is
    /// repaired to the farm's own field of the same name - but only when the match is unambiguous.
    /// </summary>
    [TestClass]
    public class PastureLocationRepairTest
    {
        private static JsonSerializer Serializer()
        {
            return new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };
        }

        private static Farm RoundTrip(Farm farm)
        {
            string json;
            using (var writer = new StringWriter())
            {
                Serializer().Serialize(writer, farm, typeof(Farm));
                json = writer.ToString();
            }

            using (var reader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return Serializer().Deserialize<Farm>(jsonReader);
            }
        }

        /// <summary>
        /// A farm owning <paramref name="ownFieldNames"/>, whose single management period is housed on
        /// <paramref name="strayPasture"/> - a field belonging to some other farm.
        /// </summary>
        private static Farm CreateFarmWithStrayPasture(FieldSystemComponent strayPasture, params string[] ownFieldNames)
        {
            var farm = new Farm { Name = "Copy" };

            foreach (var name in ownFieldNames)
            {
                var field = new FieldSystemComponent { Name = name };
                field.CropViewItems.Add(new CropViewItem { CropType = CropType.TameGrass, Year = 2024 });
                farm.Components.Add(field);
            }

            var managementPeriod = new ManagementPeriod { Name = "Summer pasture" };
            managementPeriod.HousingDetails.HousingType = HousingType.Pasture;
            managementPeriod.HousingDetails.PastureLocation = strayPasture;

            var group = new AnimalGroup { Name = "Cows" };
            group.ManagementPeriods.Add(managementPeriod);

            var animalComponent = new CowCalfComponent();
            animalComponent.Groups.Add(group);
            farm.Components.Add(animalComponent);

            return farm;
        }

        private static FieldSystemComponent OtherFarmsField(string name)
        {
            var field = new FieldSystemComponent { Name = name };
            field.CropViewItems.Add(new CropViewItem { CropType = CropType.TameGrass, Year = 2024 });

            return field;
        }

        private static ManagementPeriod SingleManagementPeriod(Farm farm)
        {
            return farm.Components.OfType<AnimalComponentBase>().Single().Groups.Single().ManagementPeriods.Single();
        }

        [TestMethod]
        public void RepairsAStrayPastureToTheFarmsOwnFieldOfTheSameName()
        {
            var stray = OtherFarmsField("Timothy pasture");
            var farm = CreateFarmWithStrayPasture(stray, "Timothy pasture", "Barley");

            var restored = RoundTrip(farm);

            var ownPasture = restored.Components.OfType<FieldSystemComponent>()
                .Single(x => x.Name == "Timothy pasture");
            var repaired = SingleManagementPeriod(restored).HousingDetails.PastureLocation;

            Assert.AreSame(ownPasture, repaired, "the pasture should point at this farm's own field");
            Assert.AreNotEqual(stray.Guid, repaired.Guid, "it should no longer reference the other farm's field");
        }

        [TestMethod]
        public void LeavesTheReferenceAloneWhenNoFieldMatchesByName()
        {
            var stray = OtherFarmsField("Timothy pasture");
            var farm = CreateFarmWithStrayPasture(stray, "Barley", "Canola");

            var restored = RoundTrip(farm);
            var pasture = SingleManagementPeriod(restored).HousingDetails.PastureLocation;

            Assert.IsNotNull(pasture, "the reference must not be dropped");
            Assert.AreEqual(stray.Guid, pasture.Guid, "with no match, the original reference must be preserved");
        }

        [TestMethod]
        public void LeavesTheReferenceAloneWhenSeveralFieldsShareTheName()
        {
            var stray = OtherFarmsField("Timothy pasture");
            var farm = CreateFarmWithStrayPasture(stray, "Timothy pasture", "Timothy pasture");

            var restored = RoundTrip(farm);
            var pasture = SingleManagementPeriod(restored).HousingDetails.PastureLocation;

            Assert.IsNotNull(pasture, "the reference must not be dropped");
            Assert.AreEqual(stray.Guid, pasture.Guid, "an ambiguous match must not be guessed at");
        }

        [TestMethod]
        public void DoesNotDisturbAPastureThatIsAlreadyTheFarmsOwnField()
        {
            var farm = new Farm { Name = "Healthy" };

            var pasture = new FieldSystemComponent { Name = "Timothy pasture" };
            pasture.CropViewItems.Add(new CropViewItem { CropType = CropType.TameGrass, Year = 2024 });
            farm.Components.Add(pasture);

            var managementPeriod = new ManagementPeriod { Name = "Summer pasture" };
            managementPeriod.HousingDetails.PastureLocation = pasture;

            var group = new AnimalGroup { Name = "Cows" };
            group.ManagementPeriods.Add(managementPeriod);

            var animalComponent = new CowCalfComponent();
            animalComponent.Groups.Add(group);
            farm.Components.Add(animalComponent);

            var restored = RoundTrip(farm);

            var restoredPasture = restored.Components.OfType<FieldSystemComponent>().Single();
            var referenced = SingleManagementPeriod(restored).HousingDetails.PastureLocation;

            Assert.AreSame(restoredPasture, referenced);
            Assert.AreEqual(pasture.Guid, referenced.Guid, "a healthy reference must keep its identity");
        }
    }
}
