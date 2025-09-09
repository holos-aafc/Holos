﻿using H.Core.Emissions.Results;
using H.Core.Models;
using H.Core.Models.Animals;

namespace H.Core.Test.Emissions.Results;

[TestClass]
public class AnimalGroupEmissionsResultTest
{
    [TestMethod]
    public void AnimalsOverDmiMaxDuringManagementPeriodReturnsFalse()
    {
        var animalGroupEmissionResults = new AnimalGroupEmissionResults
        {
            GroupEmissionsByMonths = new List<GroupEmissionsByMonth>
            {
                new(new MonthsAndDaysData(), new List<GroupEmissionsByDay>
                {
                    new()
                    {
                        DryMatterIntake = 10,
                        DryMatterIntakeMax = 20,
                        DateTime = DateTime.Now
                    }
                })
            }
        };

        var result = animalGroupEmissionResults.IsDmiOverDmiMaxForPeriod(new ManagementPeriod
        {
            Start = DateTime.Now.Subtract(TimeSpan.FromDays(30)), End = DateTime.Now.Subtract(TimeSpan.FromDays(20))
        });

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void AnimalsOverDmiMaxDuringManagementPeriodReturnsTrue()
    {
        var animalGroupEmissionResults = new AnimalGroupEmissionResults
        {
            GroupEmissionsByMonths = new List<GroupEmissionsByMonth>
            {
                new(new MonthsAndDaysData(), new List<GroupEmissionsByDay>
                {
                    new()
                    {
                        DryMatterIntake = 60,
                        DryMatterIntakeMax = 20,
                        DateTime = DateTime.Now
                    }
                })
            }
        };

        var result = animalGroupEmissionResults.IsDmiOverDmiMaxForPeriod(new ManagementPeriod
            { Start = DateTime.Now.Subtract(TimeSpan.FromDays(30)), End = DateTime.Now.Add(TimeSpan.FromDays(20)) });

        Assert.IsTrue(result);
    }
}