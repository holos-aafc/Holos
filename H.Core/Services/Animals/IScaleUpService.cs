using H.Core.Enumerations;
using H.Core.Models;

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