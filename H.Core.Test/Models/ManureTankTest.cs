using System;
using H.Core.Converters;
using H.Core.Emissions.Results;
using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models
{
    [TestClass]
    public class ManureTankTest : UnitTestBase
    {
        #region Fields

        private ManureTank _sut;

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
            _sut = new ManureTank();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        /// <summary>
        /// Issue #451 worked example on the tank itself: two management periods feed the tank 100 kg and 50 kg on the
        /// same day (summed to 150), with 30 kg removed. The shared net-of-removals volume is 150 and the removal
        /// fraction is 30 / 150 = 0.20 - not 30/100 or 30/50.
        /// </summary>
        [TestMethod]
        public void ComputeDailyStorage_SummedInflow_UsesSharedTankDenominator()
        {
            var day = new DateTime(2025, 6, 1);
            _sut.AddDailyInflow(day, 100);
            _sut.AddDailyInflow(day, 50);

            _sut.ComputeDailyStorage(date => 30);

            Assert.AreEqual(150, _sut.NetOfRemovalsByDate[day.Date], 1e-9);
            Assert.AreEqual(0.20, _sut.RemovalFractionByDate[day.Date], 1e-9);
        }

        /// <summary>
        /// The net-of-removals volume carries forward: yesterday's remaining volume (debited by yesterday's removal
        /// fraction) plus today's inflow.
        /// </summary>
        [TestMethod]
        public void ComputeDailyStorage_CarriesVolumeForwardAcrossDays()
        {
            var day1 = new DateTime(2025, 6, 1);
            var day2 = new DateTime(2025, 6, 2);
            _sut.AddDailyInflow(day1, 100);
            _sut.AddDailyInflow(day2, 50);

            // No removal on day 1, 20 kg removed on day 2.
            _sut.ComputeDailyStorage(date => date == day2 ? 20 : 0);

            Assert.AreEqual(100, _sut.NetOfRemovalsByDate[day1], 1e-9);       // inflow only, nothing removed yet
            Assert.AreEqual(0, _sut.RemovalFractionByDate[day1], 1e-9);
            Assert.AreEqual(150, _sut.NetOfRemovalsByDate[day2], 1e-9);       // 50 + 100 * (1 - 0)
            Assert.AreEqual(20.0 / 150.0, _sut.RemovalFractionByDate[day2], 1e-9);
        }

        /// <summary>
        /// A removal larger than the tank is capped at the daily maximum removal fraction.
        /// </summary>
        [TestMethod]
        public void ComputeDailyStorage_RemovalExceedingTank_CapsFraction()
        {
            var day = new DateTime(2025, 6, 1);
            _sut.AddDailyInflow(day, 100);

            _sut.ComputeDailyStorage(date => 1000); // far more than the 100 in the tank

            Assert.AreEqual(ManureStorageMath.MaximumRemovalFraction, _sut.RemovalFractionByDate[day], 1e-9);
        }

        #endregion
    }
}