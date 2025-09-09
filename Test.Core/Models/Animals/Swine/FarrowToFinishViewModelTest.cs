using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Swine;

namespace H.Core.Test.Models.Animals.Swine;

[TestClass]
public class FarrowToFinishViewModelTest
{
    [TestMethod]
    public void GetAverageLitterSizeTest()
    {
        var farrowToFinishComponent = new FarrowToFinishComponent();
        var pigletGroup = new AnimalGroup { GroupType = AnimalType.SwinePiglets };
        pigletGroup.ManagementPeriods = new ObservableCollection<ManagementPeriod>
            { new() { NumberOfAnimals = 100, ProductionStage = ProductionStages.Weaning } };

        var giltsGroup = new AnimalGroup { GroupType = AnimalType.SwineGilts };
        giltsGroup.ManagementPeriods = new ObservableCollection<ManagementPeriod>
            { new() { NumberOfAnimals = 50, ProductionStage = ProductionStages.Gestating } };

        var sowsGroup = new AnimalGroup { GroupType = AnimalType.SwineSows };
        sowsGroup.ManagementPeriods = new ObservableCollection<ManagementPeriod>
            { new() { NumberOfAnimals = 40, ProductionStage = ProductionStages.Gestating } };

        farrowToFinishComponent.Groups.Add(pigletGroup);
        farrowToFinishComponent.Groups.Add(giltsGroup);
        farrowToFinishComponent.Groups.Add(sowsGroup);

        var result = farrowToFinishComponent.CalculateAverageLitterSize(
            new Tuple<AnimalType, ProductionStages>(AnimalType.SwinePiglets, ProductionStages.Weaning),
            new List<Tuple<AnimalType, ProductionStages>>
            {
                new(AnimalType.SwineGilts, ProductionStages.Gestating),
                new(AnimalType.SwineSows, ProductionStages.Gestating)
            });

        Assert.AreEqual(0.34, 1.11, 2);
    }
}