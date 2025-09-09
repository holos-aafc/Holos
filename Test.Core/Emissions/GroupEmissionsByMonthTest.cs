using H.Core.Emissions.Results;
using H.Core.Models;

namespace H.Core.Test.Emissions;

[TestClass]
public class GroupEmissionsByMonthTest
{
    [TestMethod]
    public void GetDatesWhereDmiIsGreaterThanDmiMax()
    {
        var date1 = new DateTime(2022, 1, 1);
        var date2 = new DateTime(2022, 1, 2);

        var dailyEmissions = new List<GroupEmissionsByDay>
        {
            new()
            {
                DateTime = date1,
                DryMatterIntake = 10,
                DryMatterIntakeMax = 5
            },

            new()
            {
                DateTime = date2,
                DryMatterIntake = 2,
                DryMatterIntakeMax = 5
            }
        };

        var groupEmissionsByMonth = new GroupEmissionsByMonth(new MonthsAndDaysData(), dailyEmissions);

        Assert.AreEqual(1, groupEmissionsByMonth.GetDatesWhereDmiIsGreaterThanDmiMax().Count);
        Assert.AreEqual(date1, groupEmissionsByMonth.GetDatesWhereDmiIsGreaterThanDmiMax()[0].Date);
    }
}