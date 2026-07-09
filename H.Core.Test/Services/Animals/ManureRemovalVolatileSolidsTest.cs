using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;

namespace H.Core.Test.Services.Animals
{
    /// <summary>
    /// Unit tests for the reduction of volatile solids (VS) remaining in liquid manure storage when
    /// manure is removed (land application, export). Covers Equation 4.1.3-6, the manure-removed-on-day aggregation,
    /// the clamped removal fraction, and the net-of-removals volume carryover.
    /// </summary>
    [TestClass]
    public class ManureRemovalVolatileSolidsTest : UnitTestBase
    {
        #region Fields

        private DairyCattleResultsService _resultsService;
        private ManureService _manureService;
        private TestableResultsService _testableService;

        #endregion

        #region Set Up

        // The two removal helpers on the base service are protected; this subclass exposes them so they can be
        // exercised directly instead of only through the full daily pipeline.
        private class TestableResultsService : DairyCattleResultsService
        {
            public double GetFraction(GroupEmissionsByDay emissionsForDay)
            {
                return base.GetFractionOfManureRemovedFromStorage(emissionsForDay);
            }

            public void UpdateNetVolume(GroupEmissionsByDay daily, GroupEmissionsByDay previous, ManagementPeriod managementPeriod, Farm farm)
            {
                base.AdvanceManureStorageByOneDay(daily, previous, managementPeriod, farm);
            }

            public double AmountInStorageNet(double netPrevious, double inflow, double fractionRemovedPreviousDay)
            {
                return base.CalculateAmountInStorageNetOfRemovals(netPrevious, inflow, fractionRemovedPreviousDay);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _resultsService = new DairyCattleResultsService();
            _manureService = new ManureService();
            _testableService = new TestableResultsService();
        }

        #endregion

        #region CalculateVolatileSolidsAvailable (Equation 4.1.3-5 / 4.1.3-6)

        /// <summary>
        /// With no removal (fraction 0), VS available is today's loaded VS plus the full previous-day carryover -
        /// the unchanged base equation.
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsAvailable_NoRemoval_CarriesOverFully()
        {
            // Arrange + Act: nothing was removed yesterday, so the removal fraction is 0.
            var result = _resultsService.CalculateVolatileSolidsAvailable(
                volatileSolidsLoaded: 10,
                volatileSolidsAvailableFromPreviousDay: 100,
                volatileSolidsConsumedFromPreviousDay: 30,
                fractionOfManureRemovedFromStorageOnPreviousDay: 0);

            // Assert: yesterday's leftover (100 - 30) carries over in full and today's loaded VS is added on top.
            // This is the original behaviour for a day on which no manure was removed.
            Assert.AreEqual(80, result, 1e-9); // 10 + (100 - 30)
        }

        /// <summary>
        /// A removal scales down only the previous-day carryover by the removed fraction; today's loaded VS is
        /// still added in full.
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsAvailable_WithRemoval_ReducesCarryoverButKeepsLoaded()
        {
            // Arrange + Act: half of yesterday's stored manure was hauled out (fraction 0.5).
            var result = _resultsService.CalculateVolatileSolidsAvailable(10, 100, 30, 0.5);

            // Assert: only the carried-over VS is halved; today's loaded VS (10) is still added in full.
            Assert.AreEqual(45, result, 1e-9); // 10 + (70 * 0.5)
        }

        /// <summary>
        /// Even at the maximum 95% removal, the day's loaded VS is fully retained - only the carryover is reduced.
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsAvailable_LoadedAlwaysRetained_EvenAtMaxRemoval()
        {
            // Arrange + Act: the tank is emptied at the maximum allowed rate (95%).
            var result = _resultsService.CalculateVolatileSolidsAvailable(40, 100, 0, 0.95);

            // Assert: 95% of the carryover is gone, but the day's loaded VS (40) is fully retained - the loaded
            // term is never reduced, so the current day's fresh manure is always kept in storage.
            Assert.AreEqual(45, result, 1e-9); // 40 (loaded) + 100 * 0.05
        }

        /// <summary>
        /// A removal fraction greater than 1 (more removed than the tank held) is clamped to the 0.95 emptying cap,
        /// so 5% of the carryover always remains.
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsAvailable_FractionAboveCap_ClampedTo95Percent()
        {
            // Arrange + Act: a fraction above 1.0 means more was "removed" than the tank held (the streams aren't
            // mutually exclusive). It must be capped at 0.95 so the carryover is never fully wiped out.
            var result = _resultsService.CalculateVolatileSolidsAvailable(0, 100, 0, 1.5);

            // Assert: clamp 1.5 -> 0.95, so 5% of the carryover survives.
            Assert.AreEqual(5, result, 1e-9); // 100 * (1 - 0.95)
        }

        /// <summary>
        /// A negative removal fraction is clamped up to 0, leaving the carryover untouched.
        /// </summary>
        [TestMethod]
        public void CalculateVolatileSolidsAvailable_NegativeFraction_TreatedAsZero()
        {
            // Arrange + Act: a nonsensical negative fraction is clamped up to 0 (treated as "no removal").
            var result = _resultsService.CalculateVolatileSolidsAvailable(10, 100, 30, -0.5);

            // Assert: the carryover is kept in full, same as the no-removal case.
            Assert.AreEqual(80, result, 1e-9); // 10 + (100 - 30)
        }

        #endregion

        #region ManureService.GetTotalVolumeOfManureRemovedFromStorageOnDay

        // Builds a farm with exactly one land application (5 kg/ha over a 10 ha field = 50 kg) and one 30 kg export,
        // both for the given animal type on the given day. The application's source (on-farm vs imported) is varied
        // by the tests.
        private Farm BuildFarmWithRemoval(DateTime day, AnimalType animalType, ManureLocationSourceType applicationSource)
        {
            var farm = base.GetTestFarm();
            farm.Components.Clear();            // start from a known-empty farm so only our records count
            farm.ManureExportViewItems.Clear();

            var crop = new CropViewItem { Area = 10 };
            crop.ManureApplicationViewItems.Add(new ManureApplicationViewItem
            {
                DateOfApplication = day,
                AmountOfManureAppliedPerHectare = 5, // kg ha^-1 -> x 10 ha = 50 kg
                AnimalType = animalType,
                ManureLocationSourceType = applicationSource,
            });

            var field = new FieldSystemComponent();
            field.CropViewItems.Add(crop);
            farm.Components.Add(field);

            farm.ManureExportViewItems.Add(new ManureExportViewItem
            {
                DateOfExport = day,
                Amount = 30,
                AnimalType = animalType,
            });

            return farm;
        }

        /// <summary>
        /// The day's removed volume is the sum of on-farm land application and off-farm export for the animal type.
        /// </summary>
        [TestMethod]
        public void GetTotalVolumeOfManureRemovedFromStorageOnDay_SumsLandApplicationAndExport()
        {
            // Arrange: on-farm application (50 kg) + export (30 kg) for dairy on the same day.
            var day = new DateTime(2025, 6, 1);
            var farm = BuildFarmWithRemoval(day, AnimalType.DairyLactatingCow, ManureLocationSourceType.Livestock);

            // Act: total manure that left storage that day for dairy.
            var result = _manureService.GetTotalVolumeOfManureRemovedFromStorageOnDay(day, farm, AnimalType.DairyLactatingCow);

            // Assert: both removal streams are summed.
            Assert.AreEqual(80, result, 1e-9); // 5 * 10 (applied) + 30 (exported)
        }

        /// <summary>
        /// Imported (off-farm) manure applications are excluded, because that manure was never held in this farm's storage.
        /// </summary>
        [TestMethod]
        public void GetTotalVolumeOfManureRemovedFromStorageOnDay_ExcludesImportedManureApplication()
        {
            // Arrange: the land application is flagged as imported (sourced off-farm), so that manure never sat in
            // this farm's storage and must not be treated as a removal from it.
            var day = new DateTime(2025, 6, 1);
            var farm = BuildFarmWithRemoval(day, AnimalType.DairyLactatingCow, ManureLocationSourceType.Imported);

            // Act
            var result = _manureService.GetTotalVolumeOfManureRemovedFromStorageOnDay(day, farm, AnimalType.DairyLactatingCow);

            // Assert: the imported application is skipped; only the 30 kg export counts.
            Assert.AreEqual(30, result, 1e-9);
        }

        /// <summary>
        /// A day with no application or export returns zero removed volume.
        /// </summary>
        [TestMethod]
        public void GetTotalVolumeOfManureRemovedFromStorageOnDay_DifferentDay_ReturnsZero()
        {
            // Arrange: removals exist on `day`...
            var day = new DateTime(2025, 6, 1);
            var farm = BuildFarmWithRemoval(day, AnimalType.DairyLactatingCow, ManureLocationSourceType.Livestock);

            // Act: ...but we query the following day, which has none.
            var result = _manureService.GetTotalVolumeOfManureRemovedFromStorageOnDay(day.AddDays(1), farm, AnimalType.DairyLactatingCow);

            // Assert
            Assert.AreEqual(0, result, 1e-9);
        }

        #endregion

        #region GetFractionOfManureRemovedFromStorage (clamped fraction)

        /// <summary>
        /// The removal fraction is the volume removed divided by the net volume remaining in storage.
        /// </summary>
        [TestMethod]
        public void GetFractionOfManureRemovedFromStorage_DividesRemovedByNetVolume()
        {
            // Arrange: 50 kg removed from a tank holding 200 kg.
            var day = new GroupEmissionsByDay
            {
                AccumulatedVolumeNetOfRemovals = 200,
                VolumeOfManureRemovedFromStorageOnDay = 50,
            };

            // Act + Assert: fraction is removed / net volume.
            Assert.AreEqual(0.25, _testableService.GetFraction(day), 1e-9); // 50 / 200
        }

        /// <summary>
        /// A missing previous day (the first day of the run) yields a zero fraction instead of throwing.
        /// </summary>
        [TestMethod]
        public void GetFractionOfManureRemovedFromStorage_NullDay_ReturnsZero()
        {
            // On the first day there is no previous day to read; the helper must return 0 rather than throw.
            Assert.AreEqual(0, _testableService.GetFraction(null), 1e-9);
        }

        /// <summary>
        /// The divide-by-zero guard: an empty tank (zero net volume) yields a zero fraction.
        /// </summary>
        [TestMethod]
        public void GetFractionOfManureRemovedFromStorage_ZeroNetVolume_ReturnsZero()
        {
            // Arrange: an empty tank (net volume 0) - the divide-by-zero guard must return 0, not divide.
            var day = new GroupEmissionsByDay
            {
                AccumulatedVolumeNetOfRemovals = 0,
                VolumeOfManureRemovedFromStorageOnDay = 50,
            };

            // Act + Assert
            Assert.AreEqual(0, _testableService.GetFraction(day), 1e-9);
        }

        /// <summary>
        /// Removing more than the tank holds is capped at the 0.95 emptying limit.
        /// </summary>
        [TestMethod]
        public void GetFractionOfManureRemovedFromStorage_AboveCap_ClampedTo95Percent()
        {
            // Arrange: 300 kg "removed" from a 200 kg tank (1.5x) - removals can exceed the tank, so this must cap.
            var day = new GroupEmissionsByDay
            {
                AccumulatedVolumeNetOfRemovals = 200,
                VolumeOfManureRemovedFromStorageOnDay = 300,
            };

            // Act + Assert: capped at the 95% emptying limit.
            Assert.AreEqual(0.95, _testableService.GetFraction(day), 1e-9);
        }

        #endregion

        #region AdvanceManureStorageByOneDay (net storage volume carryover)

        /// <summary>
        /// On the first day (no previous day) the net storage volume is simply today's manure inflow.
        /// </summary>
        [TestMethod]
        public void AdvanceManureStorageByOneDay_FirstDay_NetVolumeEqualsTodaysInflow()
        {
            // Arrange: first day (no previous day), and an empty farm so nothing is removed today.
            var managementPeriod = new ManagementPeriod { AnimalType = AnimalType.DairyLactatingCow };

            var farm = base.GetTestFarm();
            farm.Components.Clear();
            farm.ManureExportViewItems.Clear();

            var daily = new GroupEmissionsByDay
            {
                DateTime = new DateTime(2025, 6, 1),
                TotalVolumeOfManureAvailableForLandApplication = 0.1, // 1000 kg/L units -> x1000 = 100 kg inflow
            };

            // Act
            _testableService.UpdateNetVolume(daily, previous: null, managementPeriod: managementPeriod, farm: farm);

            // Assert: with no carryover, the net volume is just today's fresh manure; nothing was removed.
            Assert.AreEqual(100, daily.AccumulatedVolumeNetOfRemovals, 1e-9);
            Assert.AreEqual(0, daily.VolumeOfManureRemovedFromStorageOnDay, 1e-9);
        }

        /// <summary>
        /// The carryover recursion: yesterday's net volume is reduced by yesterday's removal fraction, then today's
        /// inflow is added on top.
        /// </summary>
        [TestMethod]
        public void AdvanceManureStorageByOneDay_DebitsPreviousDayRemovalThenAddsInflow()
        {
            // Arrange: yesterday the tank held 200 kg and 50 kg was removed (fraction 0.25); today's inflow is 100 kg.
            var managementPeriod = new ManagementPeriod { AnimalType = AnimalType.DairyLactatingCow };

            var farm = base.GetTestFarm();
            farm.Components.Clear();
            farm.ManureExportViewItems.Clear();

            var previous = new GroupEmissionsByDay
            {
                AccumulatedVolumeNetOfRemovals = 200,
                VolumeOfManureRemovedFromStorageOnDay = 50,
            };

            var daily = new GroupEmissionsByDay
            {
                DateTime = new DateTime(2025, 6, 2),
                TotalVolumeOfManureAvailableForLandApplication = 0.1, // 100 kg inflow
            };

            // Act
            _testableService.UpdateNetVolume(daily, previous, managementPeriod, farm);

            // Assert: yesterday's tank is debited by its removal fraction, then today's inflow is added.
            Assert.AreEqual(250, daily.AccumulatedVolumeNetOfRemovals, 1e-9); // 200 * (1 - 0.25) + 100
        }

        #endregion

        #region End-to-end multi-day carryover (removal -> lower VS available)

        // Drives the real daily recursion the same way the dairy/swine services do (fraction from the previous day,
        // VS carryover via Eq. 4.1.3-6, then the net-volume update), using a fixed climate factor so the only thing
        // that varies between runs is the manure removal taken from the farm. Returns VS available for each day.
        private double[] RunDailyCarryover(Farm farm, ManagementPeriod managementPeriod, int numberOfDays, DateTime startDate)
        {
            const double dailyVolatileSolidsLoaded = 10;   // fresh VS added to storage each day
            const double dailyInflowInKilograms = 100;     // fresh manure volume added to storage each day
            const double climateFactor = 0.1;              // fraction of available VS consumed (methanogenesis) each day

            var volatileSolidsAvailablePerDay = new double[numberOfDays];
            GroupEmissionsByDay previous = null;

            for (var i = 0; i < numberOfDays; i++)
            {
                // 1) Fraction of storage removed *yesterday* (0 on the first day).
                var fractionRemovedPreviousDay = _testableService.GetFraction(previous);

                var daily = new GroupEmissionsByDay
                {
                    DateTime = startDate.AddDays(i),
                    VolatileSolidsLoaded = dailyVolatileSolidsLoaded,
                    TotalVolumeOfManureAvailableForLandApplication = dailyInflowInKilograms / 1000.0,
                };

                // 2) Today's VS available: today's loaded VS + yesterday's carryover reduced by what was removed.
                daily.VolatileSolidsAvailable = _resultsService.CalculateVolatileSolidsAvailable(
                    daily.VolatileSolidsLoaded,
                    previous == null ? 0 : previous.VolatileSolidsAvailable,
                    previous == null ? 0 : previous.VolatileSolidsConsumed,
                    fractionRemovedPreviousDay);

                // 3) Today's VS consumed (this is what drives the methane number downstream).
                daily.VolatileSolidsConsumed = _resultsService.CalculateVolatileSolidsConsumed(climateFactor, daily.VolatileSolidsAvailable);

                // 4) Update the running net tank level and record today's removal for the next day to read.
                _testableService.UpdateNetVolume(daily, previous, managementPeriod, farm);

                volatileSolidsAvailablePerDay[i] = daily.VolatileSolidsAvailable;
                previous = daily;
            }

            return volatileSolidsAvailablePerDay;
        }

        // Builds a farm whose only removal is a single land application on `applicationDay` (1 ha field, so the
        // per-hectare amount equals the kilograms removed).
        private Farm BuildFarmWithApplicationOnDay(DateTime applicationDay, AnimalType animalType, double amountRemovedInKilograms)
        {
            var farm = base.GetTestFarm();
            farm.Components.Clear();
            farm.ManureExportViewItems.Clear();

            var crop = new CropViewItem { Area = 1 };
            crop.ManureApplicationViewItems.Add(new ManureApplicationViewItem
            {
                DateOfApplication = applicationDay,
                AmountOfManureAppliedPerHectare = amountRemovedInKilograms, // Area 1 ha -> amountRemovedInKilograms kg
                AnimalType = animalType,
                ManureLocationSourceType = ManureLocationSourceType.Livestock,
            });

            var field = new FieldSystemComponent();
            field.CropViewItems.Add(crop);
            farm.Components.Add(field);

            return farm;
        }

        /// <summary>
        /// End-to-end over six days: a mid-period removal leaves the days up to it unchanged but lowers VS available
        /// from the following day onward, compared with a no-removal baseline.
        /// </summary>
        [TestMethod]
        public void EndToEnd_RemovalLowersSubsequentVolatileSolidsAvailable()
        {
            var managementPeriod = new ManagementPeriod { AnimalType = AnimalType.DairyLactatingCow };
            var start = new DateTime(2025, 1, 1);

            // Baseline: an empty farm, so nothing is ever removed.
            var noRemovalFarm = base.GetTestFarm();
            noRemovalFarm.Components.Clear();
            noRemovalFarm.ManureExportViewItems.Clear();

            // Comparison: the same setup but with a single removal on day index 2 (the third day).
            var removalFarm = BuildFarmWithApplicationOnDay(start.AddDays(2), AnimalType.DairyLactatingCow, 50);

            // Act: run both for six days.
            var baseline = RunDailyCarryover(noRemovalFarm, managementPeriod, 6, start);
            var withRemoval = RunDailyCarryover(removalFarm, managementPeriod, 6, start);

            // Assert (a): days 0-2 are identical - a removal on day 2 only reduces the carryover from day 3 onward.
            for (var i = 0; i <= 2; i++)
            {
                Assert.AreEqual(baseline[i], withRemoval[i], 1e-9, $"day {i} should be unchanged");
            }

            // Assert (b): from day 3 on, less VS remains in storage, so the stored methane it drives would be lower
            // (the intent of the removal adjustment, Eq. 4.1.3-6).
            for (var i = 3; i < 6; i++)
            {
                Assert.IsTrue(withRemoval[i] < baseline[i], $"day {i}: expected {withRemoval[i]} < {baseline[i]}");
            }
        }

        /// <summary>
        /// End-to-end: a larger removal takes a larger fraction of the tank, leaving less VS available the following
        /// day than a smaller removal.
        /// </summary>
        [TestMethod]
        public void EndToEnd_LargerRemovalLeavesLessInStorage()
        {
            var managementPeriod = new ManagementPeriod { AnimalType = AnimalType.DairyLactatingCow };
            var start = new DateTime(2025, 1, 1);

            // Two identical runs except for the size of the day-2 removal (20 kg vs 80 kg).
            var smallRemoval = RunDailyCarryover(BuildFarmWithApplicationOnDay(start.AddDays(2), AnimalType.DairyLactatingCow, 20), managementPeriod, 6, start);
            var largeRemoval = RunDailyCarryover(BuildFarmWithApplicationOnDay(start.AddDays(2), AnimalType.DairyLactatingCow, 80), managementPeriod, 6, start);

            // The bigger haul-out removes a bigger fraction of the tank, so day 3 has less VS left.
            Assert.IsTrue(largeRemoval[3] < smallRemoval[3], $"expected {largeRemoval[3]} < {smallRemoval[3]}");
        }

        /// <summary>
        /// End-to-end: an over-removal (more than the tank holds) is capped at 95%, so the next day still retains the
        /// loaded VS and VS available never goes negative on any day.
        /// </summary>
        [TestMethod]
        public void EndToEnd_RemovalExceedingTank_StaysAtCapAndNeverGoesNegative()
        {
            var managementPeriod = new ManagementPeriod { AnimalType = AnimalType.DairyLactatingCow };
            var start = new DateTime(2025, 1, 1);

            // Remove far more than the tank holds on day 2 - exercises the 95% cap through the full daily flow.
            var result = RunDailyCarryover(BuildFarmWithApplicationOnDay(start.AddDays(2), AnimalType.DairyLactatingCow, 100000), managementPeriod, 6, start);

            // Even after an over-removal, the next day still holds that day's loaded VS (10) because only the
            // carryover is reduced (and only by 95%)...
            Assert.IsTrue(result[3] >= 10 - 1e-9, $"day 3 should retain the loaded VS, was {result[3]}");

            // ...and VS available never goes negative on any day.
            for (var i = 0; i < 6; i++)
            {
                Assert.IsTrue(result[i] >= 0, $"day {i} VS available should never be negative, was {result[i]}");
            }
        }

        #endregion

        #region Net-of-removals carryover helper (shared by every stored constituent)

        /// <summary>
        /// With no removal (fraction 0), the net stored amount is today's inflow plus the full previous-day carryover.
        /// </summary>
        [TestMethod]
        public void CalculateAmountInStorageNetOfRemovals_NoRemoval_CarriesOverFully()
        {
            var result = _testableService.AmountInStorageNet(netPrevious: 100, inflow: 10, fractionRemovedPreviousDay: 0);

            Assert.AreEqual(110, result, 1e-9); // 10 + 100
        }

        /// <summary>
        /// A removal reduces only the carried-over amount by the removed fraction; today's inflow is added in full.
        /// </summary>
        [TestMethod]
        public void CalculateAmountInStorageNetOfRemovals_WithRemoval_ReducesCarryoverButKeepsInflow()
        {
            var result = _testableService.AmountInStorageNet(100, 10, 0.5);

            Assert.AreEqual(60, result, 1e-9); // 10 + (100 * 0.5)
        }

        #endregion
    }
}
