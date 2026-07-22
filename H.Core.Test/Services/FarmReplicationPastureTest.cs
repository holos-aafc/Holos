using System.Linq;
using H.Core.Calculators.Infrastructure;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services;
using H.Core.Services.Animals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;

namespace H.Core.Test.Services
{
    /// <summary>
    /// Replicating a farm must give the copy its own pasture, and must not alter the farm being copied.
    /// </summary>
    [TestClass]
    public class FarmReplicationPastureTest : UnitTestBase
    {
        private FarmResultsService _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new FarmResultsService(
                new EventAggregator(),
                _fieldResultsService,
                new ADCalculator(),
                new Mock<IManureService>().Object,
                new Mock<IAnimalService>().Object,
                _n2OEmissionFactorCalculator);
        }

        /// <summary>
        /// A farm with one pasture field and one animal group whose management period is housed on that pasture.
        /// </summary>
        private static Farm CreateFarmWithPasture(out FieldSystemComponent pasture, out ManagementPeriod managementPeriod)
        {
            var farm = new Farm { Name = "Original" };

            pasture = new FieldSystemComponent { Name = "Timothy pasture" };
            pasture.CropViewItems.Add(new CropViewItem { CropType = CropType.TameGrass, Year = 2024 });
            farm.Components.Add(pasture);

            managementPeriod = new ManagementPeriod { Name = "Summer pasture" };
            managementPeriod.HousingDetails.HousingType = HousingType.Pasture;
            managementPeriod.HousingDetails.PastureLocation = pasture;

            var group = new AnimalGroup { Name = "Cows" };
            group.ManagementPeriods.Add(managementPeriod);

            var animalComponent = new CowCalfComponent();
            animalComponent.Groups.Add(group);
            farm.Components.Add(animalComponent);

            return farm;
        }

        [TestMethod]
        public void ReplicateFarmPointsPastureAtTheCopiedField()
        {
            var farm = CreateFarmWithPasture(out var originalPasture, out _);

            var copy = _sut.ReplicateFarm(farm);

            var copiedPasture = copy.Components.OfType<FieldSystemComponent>().Single();
            var copiedManagementPeriod = copy.Components.OfType<AnimalComponentBase>()
                .Single().Groups.Single().ManagementPeriods.Single();

            var referenced = copiedManagementPeriod.HousingDetails.PastureLocation;

            Assert.IsNotNull(referenced, "the copy lost its pasture location");
            Assert.AreSame(copiedPasture, referenced,
                "the copy's pasture must be its OWN field, not the original farm's");
            Assert.AreNotEqual(originalPasture.Guid, referenced.Guid,
                "the copy's pasture must not still carry the original field's guid");
        }

        [TestMethod]
        public void ReplicateFarmDoesNotModifyTheFarmBeingCopied()
        {
            var farm = CreateFarmWithPasture(out var originalPasture, out var originalManagementPeriod);
            var guidBefore = originalPasture.Guid;

            _sut.ReplicateFarm(farm);

            Assert.AreEqual(guidBefore, originalPasture.Guid,
                "replicating a farm rewrote the identity of the original farm's field");
            Assert.AreSame(originalPasture, originalManagementPeriod.HousingDetails.PastureLocation,
                "the original farm's pasture reference was repointed");
        }
    }
}
