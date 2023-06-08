using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Providers.Animals.Table_28
{
    public class ProductionDaysData
    {
        #region Properties

        public ComponentCategory? ComponentCategory { get; set; }
        public ComponentType? ComponentType { get; set; }
        public AnimalType AnimalType { get; set; }
        public int NumberOfDaysInProductionCycle { get; set; }
        public int NumberOfNonProductionDaysBetweenCycles { get; set; }
        public ProductionStages? ProductionStage { get; set; }

        #endregion
    }
}