using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using H.Core.Models.Animals;
using H.Core.Services;
using H.Core.Services.Animals;

namespace H.Core.Test.Services
{
    [TestClass]
    public class AnimalComponentHelperTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        #region Tests
        
        [TestMethod]
        public void GetMonthlyBreakdownFromManagementPeriodTest()
        {
            var helper = new AnimalComponentHelper();

            var numberOfDays = 59;

            var managementPeriod = new ManagementPeriod() { };

            managementPeriod.Start = new DateTime(2020, 1, 1);
            managementPeriod.End = managementPeriod.Start.AddDays(numberOfDays);
            managementPeriod.Duration = managementPeriod.End.Subtract(managementPeriod.Start);
            managementPeriod.NumberOfDays = numberOfDays;
            managementPeriod.StartWeight = 100;
            managementPeriod.PeriodDailyGain = 1;

            var result = helper.GetMonthlyBreakdownFromManagementPeriod(managementPeriod).ToList();

            Assert.AreEqual(2, result.Count);

            var firstMonthData = result[0];
            Assert.AreEqual(100, firstMonthData.StartWeightInMonth);
            Assert.AreEqual(131, firstMonthData.EndWeightInMonth);

            var secondMonthData = result[1];
            Assert.AreEqual(131, secondMonthData.StartWeightInMonth);
            Assert.AreEqual(159, secondMonthData.EndWeightInMonth);
        }

        [TestMethod]
        public void GetMonthlyBreakdownFromManagementPeriodReturnsCorrectNumberOfItems()
        {
            var helper = new AnimalComponentHelper();

            var managementPeriod = new ManagementPeriod();
            managementPeriod.Start = new DateTime(2023, 1, 31);
            managementPeriod.End = managementPeriod.Start.AddDays(90);

            var result = helper.GetMonthlyBreakdownFromManagementPeriod(managementPeriod);
        }

        #endregion
    }
}
