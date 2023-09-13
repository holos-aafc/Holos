using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using System;
using System.Collections.Generic;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Test
{
    public abstract class UnitTestBase
    {
        public Farm GetTestFarm()
        {
            var farm = new Farm();
            var component = new BackgroundingComponent();
            var group = new AnimalGroup();
            var managementPeriod = new ManagementPeriod();
            managementPeriod.Start = DateTime.Now;
            managementPeriod.End = managementPeriod.Start.AddDays(30 * 2);

            farm.Components.Add(component);
            component.Groups.Add(group);

            group.ManagementPeriods.Add(managementPeriod);

            return farm;
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