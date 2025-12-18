using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Models;

namespace H.Core.Test.Models
{
    [TestClass]
    public class MonthsAndDaysDataTest
    {
        #region Tests
        
        [TestMethod]
        public void DateIsInMonthReturnsTrue()
        {
            var daysAndMonthsData = new MonthsAndDaysData()
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                StartDay = 2,
                DaysInMonth = 20,
            };

            // The manure application date is within the target period
            var manureApplicationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10);

            var result = daysAndMonthsData.DateIsInMonth(manureApplicationDate);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DateIsInMonthReturnsFalse()
        {
            var daysAndMonthsData = new MonthsAndDaysData()
            {
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                StartDay = 2,
                DaysInMonth = 20,
            };

            // The manure application date is past the target period
            var manureApplicationDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 28);

            var result = daysAndMonthsData.DateIsInMonth(manureApplicationDate);

            Assert.IsFalse(result);
        } 
        
        #endregion
    }
}
