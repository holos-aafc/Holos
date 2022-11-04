using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.LandManagement;

namespace H.Core.Test.Services.LandManagement
{
    [TestClass]
    public class IrrigationServiceTest
    {
        #region Fields

        private IrrigationService _sut;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new IrrigationService();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        [TestMethod]
        public void GetTotalDailyPrecipitationTest()
        {
           var result = _sut.GetTotalDailyPrecipitation(5, 6, Province.Manitoba, 2021, 100);

           // Should be equal to precipitation since no irrigation is used in January
           Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void AddIrrigationToDailyPrecipitationsTest()
        {
            var dailyPrecipitation = new List<double>();

            for (int i = 0; i < 365; i++)
            {
                dailyPrecipitation.Add(10);
            }

            var farm = new Farm() {Province = Province.BritishColumbia};
            var crop = new CropViewItem() {AmountOfIrrigation = 50, Year = 2017};

            var result = _sut.AddIrrigationToDailyPrecipitations(dailyPrecipitation, farm, crop);

            // Create dictionary for ON, and monthly percentages then check in loop as the assert.
            var table = new Dictionary<Months, double>()
            {
                {Months.January, 0},
                {Months.February, 0},
                {Months.March, 0},
                {Months.April, 8},
                {Months.May, 8},
                {Months.June, 19.54},
                {Months.July, 27.77},
                {Months.August, 25.66},
                {Months.September, 5.51},
                {Months.October, 5.51},
                {Months.November, 0},
                {Months.December, 0},
            };

            var julianDay = 1;
            foreach (var d in result)
            {
                // Get month
                var month = _sut.GetMonthFromJulianDay(julianDay);
                var percentage = table[month];


                var precipitation = dailyPrecipitation.ElementAt(julianDay - 1);
                
                var totalPrecipitation = _sut.GetTotalDailyPrecipitation(julianDay, precipitation, farm.Province,crop.Year, crop.AmountOfIrrigation);
                var expected = precipitation + ( crop.AmountOfIrrigation * (percentage / 100.0) / DateTime.DaysInMonth(crop.Year, (int)month));

                Assert.AreEqual(expected, totalPrecipitation);

                julianDay++;
            }
        }
    }
}
