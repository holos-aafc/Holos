using H.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Models
{
    /// <summary>
    /// Pins the computed setters on <see cref="StorageTankBase"/> that the map flagged as having no direct coverage:
    /// setting the "sum of applications made" properties recomputes the "remaining" / "after applications" properties.
    /// These are the subtle invariants a tank-accounting refactor (issue #451 follow-up) is most likely to disturb.
    /// Exercised through <see cref="ManureTank"/> (a concrete <see cref="StorageTankBase"/>).
    /// </summary>
    [TestClass]
    public class StorageTankBaseTest
    {
        [TestMethod]
        public void SettingVolumeSumOfApplications_RecomputesVolumeRemainingInTank()
        {
            var tank = new ManureTank
            {
                VolumeOfManureAvailableForLandApplication = 100,
            };

            tank.VolumeSumOfAllManureApplicationsMade = 30;

            // VolumeRemainingInTank = available - applied.
            Assert.AreEqual(70, tank.VolumeRemainingInTank, 1e-9);
        }

        [TestMethod]
        public void SettingNitrogenSumOfApplications_RecomputesNitrogenAfterApplications()
        {
            var tank = new ManureTank
            {
                TotalNitrogenAvailableForLandApplication = 50,
            };

            tank.NitrogenSumOfAllManureApplicationsMade = 20;

            // TotalNitrogenAvailableAfterAllLandApplications = available - applied.
            Assert.AreEqual(30, tank.TotalNitrogenAvailableAfterAllLandApplications, 1e-9);
        }

        /// <summary>
        /// <see cref="StorageTankBase.ResetTank"/> fully zeroes every value property, including VolumeRemainingInTank.
        /// (This previously left VolumeRemainingInTank at the pre-reset volume because the volume-sum setter recomputed
        /// it before the available volume was zeroed; the reset order was corrected during the tank-accounting refactor.)
        /// </summary>
        [TestMethod]
        public void ResetTank_ZeroesAllValueProperties()
        {
            var tank = new ManureTank
            {
                VolumeOfManureAvailableForLandApplication = 100,
                TotalNitrogenAvailableForLandApplication = 50,
                TotalAmountOfCarbonInStoredManure = 10,
            };
            tank.VolumeSumOfAllManureApplicationsMade = 30;
            tank.NitrogenSumOfAllManureApplicationsMade = 20;

            tank.ResetTank();

            Assert.AreEqual(0, tank.VolumeOfManureAvailableForLandApplication, 1e-9);
            Assert.AreEqual(0, tank.VolumeRemainingInTank, 1e-9);
            Assert.AreEqual(0, tank.VolumeSumOfAllManureApplicationsMade, 1e-9);
            Assert.AreEqual(0, tank.NitrogenSumOfAllManureApplicationsMade, 1e-9);
            Assert.AreEqual(0, tank.TotalNitrogenAvailableForLandApplication, 1e-9);
            Assert.AreEqual(0, tank.TotalNitrogenAvailableAfterAllLandApplications, 1e-9);
            Assert.AreEqual(0, tank.TotalAmountOfCarbonInStoredManure, 1e-9);
        }
    }
}
