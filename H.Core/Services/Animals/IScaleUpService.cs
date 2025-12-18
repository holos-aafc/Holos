using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals.Table_28;

namespace H.Core.Services.Animals
{
    public interface IScaleUpService
    {
        #region Public Methods

        bool ShouldScaleUp(bool isAnnualReport, AnimalType animalType, ProductionStages productionStage,
            ComponentType? componentType, Farm farm);
        double ScaleUpEmissions(double emissions, double numberOfDaysRest, double numberOfDaysInCycle);

        #endregion
    }
}