using H.Core.Enumerations;

namespace H.Core.Providers.Animals
{
    public interface IAnimalCoefficientDataProvider
    {
        AnimalCoefficientData GetCoefficientsByAnimalType(AnimalType animalType,
            ProductionStages? productionStage = null);
    }
}