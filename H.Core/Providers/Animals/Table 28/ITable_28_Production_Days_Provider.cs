using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Providers.Animals.Table_28
{
    public interface ITable_28_Production_Days_Provider
    {
        ProductionDaysData GetBackgroundingData();
        ProductionDaysData GetSwineData(AnimalType animalType, ComponentType componentType, ProductionStages? productionStage = null);
        ProductionDaysData GetPoultryData(AnimalType animalType);
    }
}