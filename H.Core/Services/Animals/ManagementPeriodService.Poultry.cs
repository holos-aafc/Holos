using H.Core.Models;
using H.Core.Models.Animals;
using System;
using System.ComponentModel;

namespace H.Core.Services.Animals
{
    public partial class ManagementPeriodService
    {
        #region Public Methods

        public void ChickenEggProductionHensManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelEggLaying;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 358;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void PulletFarmPulletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBroodingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 14;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelRearingStage;
            managementPeriod.NumberOfDays = 119;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void ChickenMultiplierBreederLayersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBroodingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 14;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelRearingStage;
            managementPeriod.NumberOfDays = 140;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBreedingStage;
            managementPeriod.NumberOfDays = 344;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void ChickenMultiplierBreederBroilersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBroodingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 14;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelRearingStage;
            managementPeriod.NumberOfDays = 140;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBreedingStage;
            managementPeriod.NumberOfDays = 294;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void ChickenMeatProductionBroilersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBroodingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 14;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelRearingStage;
            managementPeriod.NumberOfDays = 14;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelRearingStage;
            managementPeriod.NumberOfDays = 14;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void TurkeyMultiplierBreederYoungTomsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBroodingAndRearingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 1, 1);
            managementPeriod.NumberOfDays = 210;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void TurkeyMultiplierBreederTomsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBreedingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 7, 30);
            managementPeriod.NumberOfDays = 168;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void TurkeyMultiplierBreederYoungTurkeyHensManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBroodingAndRearingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 1, 1);
            managementPeriod.NumberOfDays = 210;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void TurkeyMultiplierBreederTurkeyHensManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBreedingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 7, 30);
            managementPeriod.NumberOfDays = 168;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void TurkeyMeatProductionYoungTomsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBroodingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 21;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelTurkeyBroilers;
            managementPeriod.NumberOfDays = 63;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelHeavyTurkeys;
            managementPeriod.NumberOfDays = 112;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void TurkeyMeatProductionYoungTurkeyHensManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBroodingStage;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 14;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelTurkeyBroilers;
            managementPeriod.NumberOfDays = 56;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelHeavyTurkeys;
            managementPeriod.NumberOfDays = 84;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void ChickenMultiplierHatcheryChicksManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelIncubation;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 18;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelHatching;
            managementPeriod.NumberOfDays = 3;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelServicing;
            managementPeriod.NumberOfDays = 1;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelStorage;
            managementPeriod.NumberOfDays = 1;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        public void ChickenMultiplierHatcheryPoultsManagementPeriod(Farm farm, AnimalGroup animalGroup,
    ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelIncubation;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 25;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelHatching;
            managementPeriod.NumberOfDays = 3;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelServicing;
            managementPeriod.NumberOfDays = 1;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddPoultryManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelStorage;
            managementPeriod.NumberOfDays = 1;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }

        #endregion

        #region Private Methods

        private ManagementPeriod AddPoultryManagementPeriodToAnimalGroup(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);

            _initializationService.InitializeDailyTanExcretion(managementPeriod);
            _initializationService.InitializeAmmoniaEmissionFactorForManureStorage(managementPeriod);
            _initializationService.InitializeAmmoniaEmissionFactorForLandApplication(managementPeriod);
            _initializationService.InitializeAmmoniaEmissionFactorForHousing(managementPeriod);

            return managementPeriod;
        }

        #endregion
    }
}