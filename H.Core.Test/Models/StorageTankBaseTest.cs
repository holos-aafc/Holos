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
        /// Characterizes the CURRENT behaviour of <see cref="StorageTankBase.ResetTank"/>, including a known ordering
        /// quirk we are pinning before the tank-accounting refactor: because ResetTank sets
        /// VolumeSumOfAllManureApplicationsMade to 0 (which recomputes VolumeRemainingInTank from the not-yet-zeroed
        /// VolumeOfManureAvailableForLandApplication) BEFORE it zeroes VolumeOfManureAvailableForLandApplication (a plain
        /// setter that does not re-trigger the recompute), VolumeRemainingInTank is left at the pre-reset volume rather
        /// than 0. The nitrogen side has the opposite (correct) order and does reach 0. If the refactor changes this,
        /// this test flags it so the change is a conscious decision.
        /// </summary>
        [TestMethod]
        public void ResetTank_ZeroesMostProperties_ButLeavesVolumeRemainingAtPreResetVolume()
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

            // Fully reset:
            Assert.AreEqual(0, tank.VolumeOfManureAvailableForLandApplication, 1e-9);
            Assert.AreEqual(0, tank.VolumeSumOfAllManureApplicationsMade, 1e-9);
            Assert.AreEqual(0, tank.NitrogenSumOfAllManureApplicationsMade, 1e-9);
            Assert.AreEqual(0, tank.TotalNitrogenAvailableForLandApplication, 1e-9);
            Assert.AreEqual(0, tank.TotalNitrogenAvailableAfterAllLandApplications, 1e-9);
            Assert.AreEqual(0, tank.TotalAmountOfCarbonInStoredManure, 1e-9);

            // Known quirk (pinned): NOT reset to 0 — left at the pre-reset VolumeOfManureAvailableForLandApplication.
            Assert.AreEqual(100, tank.VolumeRemainingInTank, 1e-9);
        }
    }
}
