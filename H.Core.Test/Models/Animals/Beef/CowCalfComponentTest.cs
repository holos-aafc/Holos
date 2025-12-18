using System;
using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models.Animals.Beef
{
    [TestClass]
    public class CowCalfComponentTest
    {
        [TestMethod]
        public void GetAssociatedCalfGroupsTest()
        {
            var cowGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCow,
                GroupPairingNumber = 1,
            };

            var cowCalfComponent = new CowCalfComponent()
            {
                Groups = new ObservableCollection<AnimalGroup>()
                {
                    cowGroup,

                    new AnimalGroup()
                    {
                        GroupType = AnimalType.BeefCalf,
                        GroupPairingNumber = 1,
                    },

                    new AnimalGroup()
                    {
                        GroupType = AnimalType.BeefCalf,
                        GroupPairingNumber = 1,
                    },

                    new AnimalGroup()
                    {
                        GroupType = AnimalType.BeefCalf,
                        GroupPairingNumber = 2,
                    },
                }
            };

            var result = cowCalfComponent.GetAssociatedYoungAnimalGroups(cowGroup, AnimalType.BeefCalf);

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetAssociatedParentGroupTest()
        {
            var cowGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCow,
                GroupPairingNumber = 1,
            };

            var calfGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCalf,
                GroupPairingNumber = 1,
            };

            var cowCalfComponent = new CowCalfComponent()
            {
                Groups = new ObservableCollection<AnimalGroup>()
                {
                    cowGroup,
                    calfGroup,
                }
            };

            var result = cowCalfComponent.GetAssociatedParentGroup(calfGroup, AnimalType.BeefCow);

            Assert.AreEqual(result.Guid, cowGroup.Guid);
        }

        [TestMethod]
        public void GetAssociatedManagementPeriodOfParentGroup()
        {
            var managementPeriod = new ManagementPeriod()
            {
                Start = new DateTime(2010, 1, 20),
                End = new DateTime(2012, 1, 20),
            };

            var cowGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCow,
                GroupPairingNumber = 1,
                ManagementPeriods = new ObservableCollection<ManagementPeriod>()
                {
                   managementPeriod,
                }
            };

            var calfGroup = new AnimalGroup()
            {
                GroupType = AnimalType.BeefCalf,
                GroupPairingNumber = 1,
            };

            var cowCalfComponent = new CowCalfComponent()
            {
                Groups = new ObservableCollection<AnimalGroup>()
                {
                    cowGroup,
                    calfGroup,
                }
            };

            var result = cowCalfComponent.GetAssociatedManagementPeriodOfParentGroup(new DateTime(2011, 1, 1), calfGroup, AnimalType.BeefCow );

            Assert.AreEqual(managementPeriod.Guid, result.Guid);
        }
    }
}
