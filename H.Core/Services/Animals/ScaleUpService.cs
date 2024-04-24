using System;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Animals.Table_28;

namespace H.Core.Services.Animals
{
    public class ScaleUpService : IScaleUpService
    {
        #region Fields

        private readonly ITable_28_Production_Days_Provider _table28ProductionDaysProvider;

        #endregion

        #region Constructors

        public ScaleUpService(ITable_28_Production_Days_Provider table28ProductionDaysProvider)
        {
            if (table28ProductionDaysProvider != null)
            {
                _table28ProductionDaysProvider = table28ProductionDaysProvider;
            }
            else
            {
                throw new ArgumentNullException(nameof(table28ProductionDaysProvider));
            }
        }

        #endregion

        #region Public Methods
        
        public bool ShouldScaleUp(bool isAnnualReport, AnimalType animalType, ProductionStages productionStage,
            ComponentType? componentType, Farm farm)
        {
            if (farm.Defaults.ScaleUpEmissionsEnabled == false)
            {
                return false;
            }

            if (isAnnualReport == false)
            {
                // Emissions are only scaled up when reporting annually
                return false;
            }

            return _table28ProductionDaysProvider.HasProductionCycle(animalType, productionStage, componentType);
        }

        /// <summary>
        /// Equation 3.5.1-1
        /// Equation 3.5.1-2
        /// </summary>
        public double ScaleUpEmissions(
            double emissions,
            double numberOfDaysRest, 
            double numberOfDaysInCycle)
        {
            var denominator = (numberOfDaysInCycle + numberOfDaysRest);
            if (denominator == 0)
            {
                return emissions;
            }

            var numberOfProductionCycles = CoreConstants.DaysInYear / denominator;

            return emissions * numberOfProductionCycles;
        }

        #endregion
    }
}