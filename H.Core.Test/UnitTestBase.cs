using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Beef;
using System;
using System.Collections.Generic;
using H.Core.Emissions.Results;

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

        public AnimalComponentEmissionsResults GetTestAnimalComponentEmissionsResults()
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
    }
}