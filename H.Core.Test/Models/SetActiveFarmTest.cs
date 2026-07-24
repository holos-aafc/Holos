using System.Linq;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models
{
    /// <summary>
    /// Opening a farm has to make it the active farm without disturbing the order farms are stored in.
    /// </summary>
    [TestClass]
    public class SetActiveFarmTest
    {
        [TestMethod]
        public void SetActiveFarmAssignsEvenWhenTheCurrentFarmSharesTheGuid()
        {
            var settings = new GlobalSettings();

            var farm = new Farm { Name = "Farm" };
            var separateInstanceWithSameGuid = new Farm { Name = "Copy", Guid = farm.Guid };

            settings.ActiveFarm = separateInstanceWithSameGuid;
            settings.SetActiveFarm(farm);

            Assert.AreSame(farm, settings.ActiveFarm,
                "the active farm must be the instance that was passed in, not the one it replaced");
        }

        [TestMethod]
        public void AssigningThePropertyDoesNotSwapInstancesSharingAGuid()
        {
            // Why SetActiveFarm exists. ModelBase compares by Guid, so SetProperty sees no change here.
            var settings = new GlobalSettings();

            var farm = new Farm { Name = "Farm" };
            var separateInstanceWithSameGuid = new Farm { Name = "Copy", Guid = farm.Guid };

            settings.ActiveFarm = separateInstanceWithSameGuid;
            settings.ActiveFarm = farm;

            Assert.AreSame(separateInstanceWithSameGuid, settings.ActiveFarm,
                "assigning the property is expected to leave the existing instance in place");
        }

        [TestMethod]
        public void MakingAFarmActiveLeavesTheFarmOrderAlone()
        {
            var data = new ApplicationData();

            var first = new Farm { Name = "First" };
            var second = new Farm { Name = "Second" };
            var third = new Farm { Name = "Third" };

            data.Farms.Add(first);
            data.Farms.Add(second);
            data.Farms.Add(third);

            // Open the middle farm, as the open farm dialog does.
            data.GlobalSettings.SetActiveFarm(second);

            Assert.AreSame(second, data.GlobalSettings.ActiveFarm);
            CollectionAssert.AreEqual(
                new[] { "First", "Second", "Third" },
                data.Farms.Select(x => x.Name).ToArray(),
                "opening a farm must not move it within the collection");
        }
    }
}
