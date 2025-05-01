using H.Core.Models.Animals;
using H.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;
using H.Core.Services.Initialization;
using System.ComponentModel;
using H.Infrastructure;

namespace H.Core.Services.Animals
{
    public partial class ManagementPeriodService
    {
        #region Fields
        const int DefaultLitterSize = 9;
        const double PeriodDailyGain = 0.25;
        #endregion

        #region Public Methods
        public void FarrowToFinishBoarsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelHeatDetection;
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Boars);
            managementPeriod.NumberOfDays = 365;
            managementPeriod.PeriodDailyGain = 0;
            managementPeriod.StartWeight = 198;
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);
            managementPeriod.ProductionStage = ProductionStages.BreedingStock;
        }

        public void FarrowToFinishGiltsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            animalGroup.WeightOfPigletsAtBirth = 1.4;
            animalGroup.WeightOfWeanedAnimals = 6;

            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelOpenGilts;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GiltDeveloperDiet);
            managementPeriod.ProductionStage = ProductionStages.BreedingStock;
            managementPeriod.PeriodDailyGain = 0;
            managementPeriod.NumberOfDays = 5;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 198;
            managementPeriod.EndWeight = 198;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBredGilts + " " + H.Core.Properties.Resources.LabelStage + " #1";
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.ProductionStage = ProductionStages.Gestating;
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBredGilts + " " + H.Core.Properties.Resources.LabelStage + " #2";
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.ProductionStage = ProductionStages.Gestating;
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelBredGilts + " " + H.Core.Properties.Resources.LabelStage + " #3";
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.ProductionStage = ProductionStages.Gestating;
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelFarrowingGilts;
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Lactation);
            managementPeriod.ProductionStage = ProductionStages.Lactating;
            managementPeriod.NumberOfDays = 21;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);
        }

        public void FarrowToFinishSowsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            animalGroup.WeightOfPigletsAtBirth = 1.4;
            animalGroup.WeightOfWeanedAnimals = 6;

            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelOpenSows;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 5;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.PeriodDailyGain = 0;
            managementPeriod.StartWeight = 198;
            managementPeriod.EndWeight = 198;
            managementPeriod.ProductionStage = ProductionStages.Weaning;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredSows + " " + Properties.Resources.LabelStage + " #1";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);
            managementPeriod.ProductionStage = ProductionStages.Open;
            animalGroup.LitterSizeOfBirthingAnimal = DefaultLitterSize;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredSows + " " + Properties.Resources.LabelStage + " #2";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);
            managementPeriod.ProductionStage = ProductionStages.Gestating;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredSows + " " + Properties.Resources.LabelStage + " #3";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);
            managementPeriod.ProductionStage = ProductionStages.Gestating;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = Properties.Resources.LabelFarrowingLactatingSows;
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Lactation);
            managementPeriod.NumberOfDays = 21;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);
            managementPeriod.ProductionStage = ProductionStages.Lactating;
        }

        public void FarrowToFinishHogsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelGrowerFinisherHogs + " " + H.Core.Properties.Resources.LabelStage + " #1";
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GrowerFinisherDiet1);
            managementPeriod.PeriodDailyGain = PeriodDailyGain;
            managementPeriod.NumberOfDays = 26;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 30;
            managementPeriod.EndWeight = 50;
            managementPeriod.ProductionStage = ProductionStages.GrowingAndFinishing;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelGrowerFinisherHogs + " " + H.Core.Properties.Resources.LabelStage + " #2";
            managementPeriod.NumberOfDays = 17;
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GrowerFinisherDiet2);
            managementPeriod.End = managementPeriod.Start.AddDays(80);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 50;
            managementPeriod.EndWeight = 65;
            managementPeriod.ProductionStage = ProductionStages.GrowingAndFinishing;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelGrowerFinisherHogs + " " + H.Core.Properties.Resources.LabelStage + " #3";
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GrowerFinisherDiet3);
            managementPeriod.NumberOfDays = 27;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 65;
            managementPeriod.EndWeight = 90;
            managementPeriod.ProductionStage = ProductionStages.GrowingAndFinishing;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelGrowerFinisherHogs + " " + H.Core.Properties.Resources.LabelStage + " #4";
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GrowerFinisherDiet4);
            managementPeriod.NumberOfDays = 45;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 90;
            managementPeriod.EndWeight = 130;
            managementPeriod.ProductionStage = ProductionStages.GrowingAndFinishing;
        }

        public void FarrowToFinishPigletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelSucklingPiglets;
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.None); // No diet because they are still suckling
            managementPeriod.NumberOfDays = 21;
            managementPeriod.StartWeight = 1.4;
            managementPeriod.PeriodDailyGain = 0.23;
            managementPeriod.EndWeight = 6;
            managementPeriod.ProductionStage = ProductionStages.Weaning;
        }

        public void FarrowToFinishWeanedPigletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelWeanedPiglets + " " + H.Core.Properties.Resources.LabelStage + " #1";
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.NurseryWeanersStarter1);
            managementPeriod.PeriodDailyGain = 0.754;
            managementPeriod.NumberOfDays = 19;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 6.23;
            managementPeriod.ProductionStage = ProductionStages.Weaned;
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = H.Core.Properties.Resources.LabelWeanedPiglets + " " + H.Core.Properties.Resources.LabelStage + " #2";
            managementPeriod.NumberOfDays = 16;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 20.765;
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.NurseryWeanersStarter2);
            managementPeriod.PeriodDailyGain = 0.629;
            managementPeriod.ProductionStage = ProductionStages.Weaned;
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.Duration.TotalDays * managementPeriod.PeriodDailyGain);
        }

        public void IsoWeanPigletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.NurseryWeanersStarter1);
            managementPeriod.StartWeight = 6;
            managementPeriod.Name = H.Core.Properties.Resources.LabelWeanedPiglets + " " + H.Core.Properties.Resources.LabelStage + " #1";
            managementPeriod.PeriodDailyGain = 0.23;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 19;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.ProductionStage = ProductionStages.Weaned;
            managementPeriod.EndWeight = 20;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.NurseryWeanersStarter1);
            managementPeriod.StartWeight = 20;
            managementPeriod.Name = H.Core.Properties.Resources.LabelWeanedPiglets + " " + H.Core.Properties.Resources.LabelStage + " #2";
            managementPeriod.NumberOfDays = 16;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.ProductionStage = ProductionStages.Weaned;
            managementPeriod.EndWeight = 30;
        }

        public void FarrowToWeanPigletsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.None); // No diet because they are still suckling
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.Name = H.Core.Properties.Resources.LabelSucklingPiglets;
            managementPeriod.NumberOfDays = 21;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.PeriodDailyGain = PeriodDailyGain;
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Weaning;
        }

        public void FarrowToWeanGiltsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            animalGroup.WeightOfPigletsAtBirth = 1.4;
            animalGroup.WeightOfWeanedAnimals = 6;

            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GiltDeveloperDiet);
            managementPeriod.Name = Properties.Resources.LabelOpenGilts;
            managementPeriod.PeriodDailyGain = 0;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 5;
            managementPeriod.StartWeight = 198;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Open;
            managementPeriod.EndWeight = 198;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredGilts + " (" + Core.Properties.Resources.LabelStage + " #1)";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Gestating;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredGilts + " (" + Core.Properties.Resources.LabelStage + " #2)";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Gestating;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredGilts + " (" + Core.Properties.Resources.LabelStage + " #3)";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Gestating;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Lactation);
            managementPeriod.Name = Properties.Resources.LabelFarrowingGilts;
            managementPeriod.NumberOfDays = 21;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Lactating;
        }

        public void FarrowToWeanSowsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            animalGroup.WeightOfPigletsAtBirth = 1.4;
            animalGroup.WeightOfWeanedAnimals = 6;

            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelOpenSows;
            managementPeriod.PeriodDailyGain = 0;
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.NumberOfDays = 5;
            managementPeriod.StartWeight = 198;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.EndWeight = 198;
            managementPeriod.ProductionStage = ProductionStages.Open;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredSows + " (" + Core.Properties.Resources.LabelStage + " #1)";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Gestating;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredSows + " (" + Core.Properties.Resources.LabelStage + " #2)";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Gestating;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Gestation);
            managementPeriod.Name = Properties.Resources.LabelBredSows + " (" + Core.Properties.Resources.LabelStage + " #3)";
            managementPeriod.NumberOfDays = 38;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Gestating;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Name = Core.Properties.Resources.LabelFarrowingLactatingSows;
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Lactation);
            managementPeriod.NumberOfDays = 21;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.Lactating;
        }

        public void FarrowToWeanBoarsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.Boars);
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.Name = AnimalType.SwineBoar.GetDescription();
            managementPeriod.NumberOfDays = 365;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 198;
            managementPeriod.PeriodDailyGain = 0;
            managementPeriod.EndWeight = managementPeriod.StartWeight + (managementPeriod.PeriodDailyGain * managementPeriod.Duration.TotalDays);
            managementPeriod.ProductionStage = ProductionStages.BreedingStock;
        }

        public void GrowerToFinishHogsManagementPeriod(Farm farm, AnimalGroup animalGroup,
            ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.Start = new DateTime(DateTime.Now.Year, 1, 1);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GrowerFinisherDiet1);
            managementPeriod.Name = H.Core.Properties.Resources.LabelStage + " #1";
            managementPeriod.NumberOfDays = 26;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 30;
            managementPeriod.EndWeight = 50;
            managementPeriod.PeriodDailyGain = (managementPeriod.EndWeight - managementPeriod.StartWeight) / managementPeriod.Duration.TotalDays;
            managementPeriod.ProductionStage = ProductionStages.GrowingAndFinishing;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GrowerFinisherDiet2);
            managementPeriod.Name = H.Core.Properties.Resources.LabelStage + " #2";
            managementPeriod.NumberOfDays = 17;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 50;
            managementPeriod.EndWeight = 65;
            managementPeriod.PeriodDailyGain = (managementPeriod.EndWeight - managementPeriod.StartWeight) / managementPeriod.Duration.TotalDays;
            managementPeriod.ProductionStage = ProductionStages.GrowingAndFinishing;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GrowerFinisherDiet3);
            managementPeriod.Name = H.Core.Properties.Resources.LabelStage + " #3";
            managementPeriod.NumberOfDays = 27;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.StartWeight = 65;
            managementPeriod.EndWeight = 90;
            managementPeriod.PeriodDailyGain = (managementPeriod.EndWeight - managementPeriod.StartWeight) / managementPeriod.Duration.TotalDays;
            managementPeriod.ProductionStage = ProductionStages.GrowingAndFinishing;

            managementPeriod = AddSwineManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged, managementPeriodOnPropertyChanged);
            managementPeriod.SelectedDiet = farm.GetDietByName(DietType.GrowerFinisherDiet4);
            managementPeriod.NumberOfDays = 45;
            managementPeriod.End = managementPeriod.Start.AddDays(managementPeriod.NumberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.Name = H.Core.Properties.Resources.LabelStage + " #4";
            managementPeriod.StartWeight = 90;
            managementPeriod.EndWeight = 130;
            managementPeriod.PeriodDailyGain = (managementPeriod.EndWeight - managementPeriod.StartWeight) / managementPeriod.Duration.TotalDays;
            managementPeriod.ProductionStage = ProductionStages.GrowingAndFinishing;
        }
        #endregion

        #region Private Methods

        private ManagementPeriod AddSwineManagementPeriodToAnimalGroup(Farm farm, AnimalGroup animalGroup, ManagementPeriod bindingManagementPeriod, PropertyChangedEventHandler animalGroupOnPropertyChanged, PropertyChangedEventHandler managementPeriodOnPropertyChanged)
        {
            var managementPeriod = this.AddManagementPeriodToAnimalGroup(farm, animalGroup, bindingManagementPeriod, animalGroupOnPropertyChanged);

            _initializationService.InitializeVolatileSolidsExcretion(managementPeriod, farm.DefaultSoilData.Province);

            managementPeriod.PropertyChanged -= managementPeriodOnPropertyChanged;
            managementPeriod.PropertyChanged += managementPeriodOnPropertyChanged;

            return managementPeriod;
        }

        #endregion
    }
}
