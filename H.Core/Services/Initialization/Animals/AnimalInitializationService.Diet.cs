using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        public List<DietAdditiveType> GetValidDietAdditiveTypes()
        {
            var result = new List<DietAdditiveType>()
            {
                DietAdditiveType.None,
                DietAdditiveType.TwoPercentFat,
                DietAdditiveType.FourPercentFat,
                DietAdditiveType.Inonophore,
                DietAdditiveType.InonophorePlusTwoPercentFat,
                DietAdditiveType.InonophorePlusFourPercentFat,
            };

            return result;
        }

        public void InitializeTotals(ManagementPeriod managementPeriod)
        {
            managementPeriod.SelectedDiet.UpdateTotals();
        }

        #endregion
    }
}