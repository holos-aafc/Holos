using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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

            var farm = new Farm() {Province = Province.Ontario};
            var crop = new CropViewItem() {AmountOfIrrigation = 50, Year = 2017};

            var result = _sut.AddIrrigationToDailyPrecipitations(dailyPrecipitation, farm, crop);

            // Create dictionary for ON, and monthly percentages then check in loop as the assert.

            var julianDay = 1;
            foreach (var d in result)
            {
                // Get month


                julianDay++;
            }
        }
    }
}
