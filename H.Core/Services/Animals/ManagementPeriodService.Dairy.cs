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
        public void DairyCalvesGroupManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = Resources.LabelMilkFedManagementPeriod;
            managementPeriod.ManureDetails.StateType = ManureStateType.SolidStorage;
            managementPeriod.ProductionStage = ProductionStages.Weaning;
            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 1, 1);
            managementPeriod.NumberOfDays = 3 * 30; // Milk fed for 3 months
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);

            // Set start and end weight after setting the start and end dates
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);
        }

        public void DairyReplacementHeifersManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.ManureDetails.StateType = ManureStateType.SolidStorage;
            managementPeriod.GainCoefficient = 0.8;

            // Set start and end weight after setting the start and end dates
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);
        }

        public void DairyDryManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = Resources.LabelDryPeriodName;
            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 11, 5);
            managementPeriod.NumberOfDays = 60;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.ManureDetails.StateType = ManureStateType.SolidStorage;
            managementPeriod.GainCoefficient = 0.8;

            _initializationService.InitializeBaselineCoefficient(managementPeriod);

            managementPeriod.MilkProduction = 0; // No milk produced during this period

            // Set start and end weight after setting the start and end dates
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);
        }

        public void DairyLactatingManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged)
        {
            var managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = Resources.LabelEarlyLactationPeriodName;
            managementPeriod.Start = new DateTime(DateTime.Now.Year - 1, 1, 1);
            managementPeriod.NumberOfDays = 150;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.ManureDetails.StateType = ManureStateType.SolidStorage;

            _initializationService.InitializeMilkProduction(managementPeriod, farm.DefaultSoilData);

            managementPeriod.MilkFatContent = 3.71;
            managementPeriod.MilkProteinContentAsPercentage = 3.5;

            // Set start and end weight after setting the start and end dates
            _initializationService.InitializeStartAndEndWeightsForCattle(managementPeriod);

            managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = Resources.LabelMidLactationPeriodName;
            managementPeriod.NumberOfDays = 60;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);

            managementPeriod = AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod,
                animalGroupOnPropertyChanged);
            managementPeriod.Name = Resources.LabelLateLactationPeriodName;
            managementPeriod.NumberOfDays = 95;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
        }
    }
}