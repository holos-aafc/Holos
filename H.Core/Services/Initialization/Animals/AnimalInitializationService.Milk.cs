using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Providers.Soil;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Method

        /// <summary>
        /// Reinitialize the MilkProduction value for each ManagementPeriod for each animalGroup in the DairyComponent of a <see cref="Farm"/> with new default values from table 21.
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> that will be reinitialized to new default value for the MilkProduction</param>
        public void InitializeMilkProduction(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeMilkProduction(managementPeriod, farm.DefaultSoilData);
                }
            }
        }

        public void InitializeMilkProduction(ManagementPeriod managementPeriod, SoilData soilData)
        {
            if (managementPeriod != null && soilData != null)
            {
                var province = soilData.Province;
                var year = managementPeriod.Start.Year;

                if (managementPeriod.AnimalType.IsLactatingType())
                {
                    if (managementPeriod.AnimalType.IsSheepType() && managementPeriod.ProductionStage == ProductionStages.Gestating)
                    {
                        managementPeriod.MilkProduction = 2;
                    }
                    else if(managementPeriod.AnimalType.IsDairyCattleType())
                    {
                        var milkProduction = _averageMilkProductionDairyCowsProvider.GetAverageMilkProductionForDairyCowsValue(year, province);
                        managementPeriod.MilkProduction = milkProduction;
                    }
                    else
                    {
                        // Other animal types (including beef cattle) but is lactating so use default
                        managementPeriod.MilkProduction = 8;
                    }
                }
                else
                {
                    managementPeriod.MilkProduction = 0;
                }
            }
        }

        #endregion
    }
}