using H.Core.Emissions.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace H.Core.Test.Emissions
{
    [TestClass]
    public class GroupEmissionsByMonthTest
    {
        [TestMethod]
        public void GetDatesWhereDmiIsGreaterThanDmiMax()
        {
            var date1 = new DateTime(2022, 1, 1);
            var date2 = new DateTime(2022, 1, 2);

            var dailyEmissions = new List<GroupEmissionsByDay>()
            {
                new GroupEmissionsByDay()
                {
                    DateTime = date1,
                    DryMatterIntake = 10,
                    DryMatterIntakeMax = 5,
                },

                new GroupEmissionsByDay()
                {
                    DateTime = date2,
                    DryMatterIntake = 2,
                    DryMatterIntakeMax = 5,
                },
            };

            var groupEmissionsByMonth = new GroupEmissionsByMonth(new Core.Models.MonthsAndDaysData(), dailyEmissions);

            Assert.AreEqual(1, groupEmissionsByMonth.GetDatesWhereDmiIsGreaterThanDmiMax().Count);
            Assert.AreEqual(date1, groupEmissionsByMonth.GetDatesWhereDmiIsGreaterThanDmiMax()[0].Date);
        }
    }
}
