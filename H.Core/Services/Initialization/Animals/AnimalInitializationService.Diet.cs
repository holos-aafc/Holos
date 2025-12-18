using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        public List<DietAdditiveType> GetValidDietAdditiveTypes()
        {
            var result = new List<DietAdditiveType>
            {
                DietAdditiveType.None,
                DietAdditiveType.TwoPercentFat,
                DietAdditiveType.FourPercentFat,
                DietAdditiveType.Inonophore,
                DietAdditiveType.InonophorePlusTwoPercentFat,
                DietAdditiveType.InonophorePlusFourPercentFat
            };

            return result;
        }

        public void InitializeTotals(ManagementPeriod managementPeriod)
        {
            managementPeriod.SelectedDiet.UpdateTotals();
        }

        public void InitializeStartAndEndWeightsForCattle(Farm farm)
        {
            if (farm != null)
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                    InitializeStartAndEndWeightsForCattle(managementPeriod);
        }

        public void InitializeStartAndEndWeightsForCattle(ManagementPeriod managementPeriod)
        {
            if (!(managementPeriod.AnimalType.GetCategory() == AnimalType.Beef ||
                  managementPeriod.AnimalType.GetCategory() == AnimalType.Dairy)) return;
            var coefficients =
                _livestockCoefficientsBeefAndDairyCattleProvider.GetCoefficientsByAnimalType(
                    managementPeriod.AnimalType);
            // If PeriodDailyGain has not been set or is 0, set start and end weights then calculate PeriodDailyGain
            if (managementPeriod.PeriodDailyGain == 0)
            {
                managementPeriod.StartWeight = coefficients.DefaultInitialWeight;
                managementPeriod.EndWeight = coefficients.DefaultFinalWeight;
                managementPeriod.PeriodDailyGain = (managementPeriod.EndWeight - managementPeriod.StartWeight) /
                                                   managementPeriod.Duration.TotalDays;
            }
            else
            {
                managementPeriod.StartWeight = coefficients.DefaultInitialWeight;
                managementPeriod.EndWeight = managementPeriod.StartWeight +
                                             managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain;
            }
        }

        #endregion
    }
}