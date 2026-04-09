using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Providers.Animals.Table_28
{
    public interface ITable_28_Production_Days_Provider
    {
        bool HasProductionCycle(AnimalType animalType, ProductionStages productionStage,
            ComponentType? componentType = null);

        ProductionDaysData GetData(AnimalType animalType,
            ProductionStages productionStage, ComponentType? componentType = null);
    }
}