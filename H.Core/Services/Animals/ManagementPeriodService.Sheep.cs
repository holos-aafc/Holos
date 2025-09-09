using System;
using System.ComponentModel;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Properties;

namespace H.Core.Services.Animals
{
    public partial class ManagementPeriodService
    {
        #region Private Methods

        private ManagementPeriod AddSheepManagementPeriodToAnimalGroup(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            _initializationService.InitializeLivestockCoefficientSheep(managementPeriod);

            return managementPeriod;
        }

        #endregion

        #region Public Methods

        public void LambsAndEwesEwesManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            // Pregnancy period
            var managementPeriod = AddSheepManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = Resources.LabelPregnancy;
            managementPeriod.ProductionStage = ProductionStages.Gestating;
            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 1, 1);
            managementPeriod.NumberOfDays = 147;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.PeriodDailyGain = 0;

            _initializationService.InitializeMilkProduction(managementPeriod, farm.DefaultSoilData);

            // Post pregnancy period (following lambing)
            managementPeriod = AddSheepManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.ProductionStage = ProductionStages.Lactating;
            managementPeriod.Name = Resources.LabelLactationDiet;
            managementPeriod.NumberOfDays = 218;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void LambsAndEwesLambsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddSheepManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.ProductionStage = ProductionStages.Weaning;
        }

        public void RamsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddSheepManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.PeriodDailyGain = 0;
        }

        public void SheepFeedlotManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddSheepManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.PeriodDailyGain = 0;
        }

        #endregion
    }
}