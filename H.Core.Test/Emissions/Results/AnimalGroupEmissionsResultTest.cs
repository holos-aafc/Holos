using H.Core.Emissions.Results;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace H.Core.Test.Emissions.Results
{
    [TestClass]
    public class AnimalGroupEmissionsResultTest
    {
        [TestMethod]
        public void AnimalsOverDmiMaxDuringManagementPeriodReturnsFalse()
        {
            var animalGroupEmissionResults = new AnimalGroupEmissionResults()
            {
                GroupEmissionsByMonths = new List<GroupEmissionsByMonth>
                {
                    new GroupEmissionsByMonth(new MonthsAndDaysData(), new List<GroupEmissionsByDay>()
                    {
                        new GroupEmissionsByDay()
                        {
                            DryMatterIntake = 10,
                            DryMatterIntakeMax = 20,
                        }
                    })
                }
            };

            var result = animalGroupEmissionResults.IsDmiOverDmiMaxForPeriod();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AnimalsOverDmiMaxDuringManagementPeriodReturnsTrue()
        {
            var animalGroupEmissionResults = new AnimalGroupEmissionResults()
            {
                GroupEmissionsByMonths = new List<GroupEmissionsByMonth>
                {
                    new GroupEmissionsByMonth(new MonthsAndDaysData(), new List<GroupEmissionsByDay>()
                    {
                        new GroupEmissionsByDay()
                        {
                            DryMatterIntake = 60,
                            DryMatterIntakeMax = 20,
                        }
                    })
                }
            };

            var result = animalGroupEmissionResults.IsDmiOverDmiMaxForPeriod();

            Assert.IsTrue(result);
        }
    }
}
