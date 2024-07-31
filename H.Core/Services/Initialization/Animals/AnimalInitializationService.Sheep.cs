using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize the values for Ewes, ram and weanedLamb from Table 22
        /// </summary>
        /// <param name="farm">Contains the coefficient(s), weight(s) and wool production details that need to be reinitialized</param>
        public void InitializeLivestockCoefficientSheep(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods().Where(x => x.AnimalType.IsSheepType()))
                {
                    if (_sheepProvider.GetCoefficientsByAnimalType(managementPeriod.AnimalType) is Table_22_Livestock_Coefficients_Sheep_Data result)
                    {
                        managementPeriod.HousingDetails.BaselineMaintenanceCoefficient = result.BaselineMaintenanceCoefficient;
                        managementPeriod.WoolProduction = result.WoolProduction;
                        managementPeriod.GainCoefficientA = result.CoefficientA;
                        managementPeriod.GainCoefficientB = result.CoefficientB;
                        managementPeriod.StartWeight = result.DefaultInitialWeight;
                        managementPeriod.EndWeight = result.DefaultFinalWeight;
                    }
                }
            }
        }

        #endregion
    }
}