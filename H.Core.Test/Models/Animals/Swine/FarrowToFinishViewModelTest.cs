using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Swine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace H.Core.Test.Models.Animals.Swine
{
    [TestClass]
    public class FarrowToFinishViewModelTest
    {
        [TestMethod]
        public void GetAverageLitterSizeTest()
        {
            var farrowToFinishComponent = new FarrowToFinishComponent();
            var pigletGroup = new AnimalGroup() { GroupType = AnimalType.SwinePiglets };
            pigletGroup.ManagementPeriods = new ObservableCollection<ManagementPeriod>() { new ManagementPeriod { NumberOfAnimals = 100, ProductionStage = ProductionStages.Weaning} };

            var giltsGroup = new AnimalGroup() { GroupType = AnimalType.SwineGilts };
            giltsGroup.ManagementPeriods = new ObservableCollection<ManagementPeriod> { new ManagementPeriod { NumberOfAnimals = 50, ProductionStage = ProductionStages.Gestating } };

            var sowsGroup = new AnimalGroup() { GroupType = AnimalType.SwineSows };
            sowsGroup.ManagementPeriods = new ObservableCollection<ManagementPeriod> { new ManagementPeriod { NumberOfAnimals = 40, ProductionStage = ProductionStages.Gestating } };

            farrowToFinishComponent.Groups.Add(pigletGroup);
            farrowToFinishComponent.Groups.Add(giltsGroup);
            farrowToFinishComponent.Groups.Add(sowsGroup);

            var result = farrowToFinishComponent.CalculateAverageLitterSize(
                youngAnimalTypeAndStage: new Tuple<AnimalType, ProductionStages>(AnimalType.SwinePiglets, ProductionStages.Weaning),
                otherAnimalsAndStages: new List<Tuple<AnimalType, ProductionStages>>()
                        {
                            new Tuple<AnimalType, ProductionStages>(AnimalType.SwineGilts, ProductionStages.Gestating),
                            new Tuple<AnimalType, ProductionStages>(AnimalType.SwineSows, ProductionStages.Gestating),
                        });

            Assert.AreEqual(0.34, 1.11, 2);
        }
    }
}
