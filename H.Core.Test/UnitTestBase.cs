using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Test
{
    public abstract class UnitTestBase
    {
        #region Fields

        #endregion

        public Storage InitializeStorage()
        {
            var storage = new Storage
            {
                ApplicationData = new ApplicationData
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ActiveFarm = new Farm()
                    }
                }
            };

            return storage;
        }

        public Farm GetTestFarm()
        {
            var farm = new Farm();
            var backgroundingComponent = new BackgroundingComponent();
            var group = new AnimalGroup();
            @group.GroupType = AnimalType.BeefBackgrounderHeifer;

            var dairyComponent = new DairyComponent();
            var cowsGroup = new AnimalGroup();
            cowsGroup.GroupType = AnimalType.DairyLactatingCow;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.Start = DateTime.Now;
            managementPeriod.End = managementPeriod.Start.AddDays(30 * 2);

            managementPeriod.ManureDetails.StateType = ManureStateType.AnaerobicDigester;

            farm.Components.Add(backgroundingComponent);
            farm.Components.Add(dairyComponent);

            backgroundingComponent.Groups.Add(group);
            dairyComponent.Groups.Add(cowsGroup);

            group.ManagementPeriods.Add(managementPeriod);

            /*
             * Manure exports
             */

            farm.ManureExportViewItems.Add(new ManureExportViewItem() {DateOfExport = DateTime.Now, Amount = 1000, AnimalType = AnimalType.Dairy});
            farm.ManureExportViewItems.Add(new ManureExportViewItem() { DateOfExport = DateTime.Now, Amount = 2000, AnimalType = AnimalType.Dairy });

            return farm;
        }

        public FieldSystemDetailsStageState GetFieldStageState()
        {
            var stageState = new FieldSystemDetailsStageState();

            stageState.DetailsScreenViewCropViewItems = new ObservableCollection<CropViewItem>()
            {
                new CropViewItem()
                {
                    Year = 2023,
                }
            };

            return stageState;
        }

        public AnimalComponentEmissionsResults GetEmptyTestAnimalComponentEmissionsResults()
        {
            var results = new AnimalComponentEmissionsResults();

            var monthsAndDaysData = new MonthsAndDaysData();

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = new HousingDetails();
            monthsAndDaysData.ManagementPeriod = managementPeriod;

            var groupEmissionsByDay = new GroupEmissionsByDay();

            var groupEmissionsByMonth = new GroupEmissionsByMonth(monthsAndDaysData, new List<GroupEmissionsByDay>() { groupEmissionsByDay });

            var animalGroupResults = new AnimalGroupEmissionResults();
            animalGroupResults.GroupEmissionsByMonths = new List<GroupEmissionsByMonth>() {groupEmissionsByMonth};

            results.EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>() {animalGroupResults};

            return results;
        }

        public AnimalComponentEmissionsResults GetNonEmptyTestAnimalComponentEmissionsResults()
        {
            var results = new AnimalComponentEmissionsResults();

            var monthsAndDaysData = new MonthsAndDaysData();
            monthsAndDaysData.Year = DateTime.Now.Year;

            var managementPeriod = new ManagementPeriod();
            managementPeriod.HousingDetails = new HousingDetails();
            monthsAndDaysData.ManagementPeriod = managementPeriod;

            var groupEmissionsByDay = new GroupEmissionsByDay()
            {
                AdjustedAmountOfTanInStoredManureOnDay = 100,
                OrganicNitrogenCreatedOnDay = 50,
                TotalVolumeOfManureAvailableForLandApplication = 100,
            };

            var groupEmissionsByMonth = new GroupEmissionsByMonth(monthsAndDaysData, new List<GroupEmissionsByDay>() { groupEmissionsByDay });

            var animalGroupResults = new AnimalGroupEmissionResults();
            animalGroupResults.GroupEmissionsByMonths = new List<GroupEmissionsByMonth>() { groupEmissionsByMonth };

            results.EmissionResultsForAllAnimalGroupsInComponent = new List<AnimalGroupEmissionResults>() { animalGroupResults };

            return results;
        }

        public FieldSystemComponent GetTestFieldComponent()
        {
            var component = new FieldSystemComponent();

            var viewItem = new CropViewItem();
            viewItem.CropType = CropType.Wheat;
            viewItem.Year = DateTime.Now.Year;

            component.CropViewItems.Add(viewItem);

            return component;
        }
    }
}