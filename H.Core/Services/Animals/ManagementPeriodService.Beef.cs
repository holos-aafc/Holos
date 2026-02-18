using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models;
using System;
using System.ComponentModel;

namespace H.Core.Services.Animals
{
    public partial class ManagementPeriodService
    {
        #region Public Methods
        // Finishing
        public void FinishingSteerGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            if (farm.IsBasicMode || farm.IsAdvancedMode)
            {
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
                managementPeriod.NumberOfAnimals = 20;
                FinisherManagementPeriodConfiguration(farm, managementPeriod);
            }
            else
            {
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
            }
        }

        public void FinishingHeiferGroupManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            if (farm.IsBasicMode || farm.IsAdvancedMode)
            {
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.NumberOfAnimals = 49;
            FinisherManagementPeriodConfiguration(farm, managementPeriod);
            managementPeriod.PeriodDailyGain = 1;
            }
            else
            {
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
            }
        }

        // Backgrounders 
        public void BackgrounderSteerGroupManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            if (farm.IsBasicMode || farm.IsAdvancedMode)
            {
                // First period
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.NumberOfAnimals = 50;

            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 10, 1);
            managementPeriod.NumberOfDays = 109;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            // Set start and end weight after setting the start and end dates
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);

            managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.MediumGrowth);
            managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
            managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;
            }
            else
            {
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
            }
        }

        public void BackgrounderHeifersGroupManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            if (farm.IsBasicMode || farm.IsAdvancedMode)
            {
                // First period
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
            managementPeriod.NumberOfAnimals = 50;

            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 10, 1);
            managementPeriod.NumberOfDays = 109;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            // Set start and end weight after setting the start and end dates
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);

            managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.MediumGrowth);
            managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
            managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;
            }
            else
            {
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
            }
        }

        // Cow calf
        public void CowCalfBullGroupManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            if (farm.IsBasicMode || farm.IsAdvancedMode)
            { 
                // Winter feeding period
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
                managementPeriod.NumberOfAnimals = 4;
                managementPeriod.Name = H.Core.Properties.Resources.LabelWinterFeeding;

                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 1, 1);
                managementPeriod.NumberOfDays = 119;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

                // Set start and end weight after setting the start and end dates
                _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);
                managementPeriod.PeriodDailyGain = (managementPeriod.EndWeight - managementPeriod.StartWeight) / managementPeriod.Duration.TotalDays;

                managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.MediumEnergyAndProtein);
                managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
                managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;

                // Summer grazing period
                managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
                managementPeriod.Name = H.Core.Properties.Resources.LabelSummerGrazing;
                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 5, 1);
                managementPeriod.NumberOfDays = 183;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
                managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.HighEnergyAndProtein);

                this.SetPastureIfPossible(farm, managementPeriod, managementPeriod.AnimalType, ManureStateType.DeepBedding, HousingType.ConfinedNoBarn);

                // Extended fall grazing period
                managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
                managementPeriod.Name = H.Core.Properties.Resources.LabelExtendedFallGrazing;
                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 11, 1);
                managementPeriod.NumberOfDays = 60;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
                managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.MediumEnergyAndProtein);

                this.SetPastureIfPossible(farm, managementPeriod, managementPeriod.AnimalType, ManureStateType.DeepBedding, HousingType.ConfinedNoBarn);
            }
        }

        public void CowCalfReplacementHeifersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.NumberOfAnimals = 20;

            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 1, 1);
            managementPeriod.NumberOfDays = 365;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            // Set start and end weight after setting the start and end dates
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);

            managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
        }

        public void CowCalfCalfGroupManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            if (farm.IsBasicMode || farm.IsAdvancedMode)
            {
                // Milk fed period
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
                managementPeriod.NumberOfAnimals = 102;
                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 3, 1);
                managementPeriod.NumberOfDays = 60;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

                managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.MediumEnergyAndProtein);

                // Set start and end weight after setting the start and end dates
                _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);
                managementPeriod.PeriodDailyGain = 1;
                managementPeriod.EndWeight = 260;

                //managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);
                managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
                managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;
                managementPeriod.AnimalsAreMilkFedOnly = true;

                // Grazing with cows - not milk fed
                managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);
                managementPeriod.NumberOfAnimals = 102;
                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 5, 1);
                managementPeriod.NumberOfDays = 152;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

                managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.HighEnergyAndProtein);
                managementPeriod.PeriodDailyGain = 1;
                managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
                managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;
                managementPeriod.AnimalsAreMilkFedOnly = false;
            }
            else
            {
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
            }
        }

        public void CowCalfCowGroupManagementPeriod(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            if (farm.IsBasicMode || farm.IsAdvancedMode)
            {
                // Winter feeding period
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
                managementPeriod.AnimalType = AnimalType.BeefCowDry;
                managementPeriod.MilkProteinContentAsPercentage = 3.38;
                managementPeriod.NumberOfAnimals = 120;

                managementPeriod.Name = H.Core.Properties.Resources.LabelWinterFeedingDry;
                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 1, 1);
                managementPeriod.NumberOfDays = 59;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

                // Set start and end weight after setting the start and end dates
                _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);

                managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType,
                    DietType.MediumEnergyAndProtein);
                managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
                managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;

                // Winter feeding period
                managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
                managementPeriod.AnimalType = AnimalType.BeefCowLactating;
                managementPeriod.MilkProteinContentAsPercentage = 3.38;
                managementPeriod.NumberOfAnimals = 120;

                _initializationService.InitializeMilkProduction(managementPeriod, farm.DefaultSoilData);

                managementPeriod.Name = H.Core.Properties.Resources.LabelWinterFeedingLactating;
                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 3, 1);
                managementPeriod.NumberOfDays = 61;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

                // Set start and end weight after setting the start and end dates
                _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);

                managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType,
                    DietType.MediumEnergyAndProtein);
                managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
                managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;

                // Summer grazing period
                managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
                managementPeriod.AnimalType = AnimalType.BeefCowLactating;
                managementPeriod.NumberOfAnimals = 120;
                managementPeriod.Name = H.Core.Properties.Resources.LabelSummerGrazing;
                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 5, 1);
                managementPeriod.NumberOfDays = 183;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

                _initializationService.InitializeMilkProduction(managementPeriod, farm.DefaultSoilData);

                managementPeriod.SelectedDiet =
                    GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.HighEnergyAndProtein);

                this.SetPastureIfPossible(farm, managementPeriod, managementPeriod.AnimalType, ManureStateType.DeepBedding, HousingType.ConfinedNoBarn);

                // Extended fall grazing period
                managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
                managementPeriod.AnimalType = AnimalType.BeefCowDry;
                managementPeriod.NumberOfAnimals = 120;
                managementPeriod.Name = H.Core.Properties.Resources.LabelExtendedFallGrazing;
                managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 11, 1);
                managementPeriod.NumberOfDays = 60;
                managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
                managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

                managementPeriod.MilkProduction = 0;

                managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType,
                    DietType.MediumEnergyAndProtein);

                this.SetPastureIfPossible(farm, managementPeriod, managementPeriod.AnimalType, ManureStateType.DeepBedding, HousingType.ConfinedNoBarn);
            }
            else
            {
                var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                    animalGroupOnPropertyChanged);
            }
        }

        private void SetPastureIfPossible(
            Farm farm, 
            ManagementPeriod managementPeriod, 
            AnimalType animalType,
            ManureStateType alternativeManureType, 
            HousingType alternativeHousingType)
        {

            var housingTypes = this.GetValidHousingTypes(farm, managementPeriod, animalType);
            if (housingTypes.Contains(HousingType.Pasture))
            {
                managementPeriod.HousingDetails.HousingType = HousingType.Pasture;
                managementPeriod.ManureDetails.StateType = ManureStateType.Pasture;
            }
            else
            {
                managementPeriod.HousingDetails.HousingType = alternativeHousingType;
                managementPeriod.ManureDetails.StateType = alternativeManureType;
            }
        }

        private ManagementPeriod FinisherManagementPeriodConfiguration(Farm farm, ManagementPeriod managementPeriod)
        {
            // First period
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 19);
            managementPeriod.NumberOfDays = 169;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            // Set start and end weight after setting the start and end dates
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);
            managementPeriod.PeriodDailyGain = (managementPeriod.EndWeight - managementPeriod.StartWeight) / managementPeriod.Duration.TotalDays;

            managementPeriod.SelectedDiet = GetDefaultDietForGroup(farm, managementPeriod.AnimalType, DietType.BarleyGrainBased);
            managementPeriod.HousingDetails.HousingType = HousingType.ConfinedNoBarn;
            managementPeriod.ManureDetails.StateType = ManureStateType.DeepBedding;
            return managementPeriod;
        }

        #endregion
    }
}
