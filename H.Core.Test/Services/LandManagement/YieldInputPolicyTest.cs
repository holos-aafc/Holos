using System;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services.LandManagement
{
    [TestClass]
    public class YieldInputPolicyTest
    {
        #region Helpers

        private static CropViewItem HayedPerennial()
        {
            var crop = new CropViewItem {CropType = CropType.TameGrass, Year = 1985};
            crop.HarvestViewItems.Add(new HarvestViewItem
                {Start = new DateTime(1985, 8, 1), ForageActivity = ForageActivities.Hayed});
            return crop;
        }

        /// <summary>
        /// Grazed and hayed in the same year - the case that was mislabelled, because both are present and only one
        /// of them decides the yield.
        /// </summary>
        private static CropViewItem GrazedAndHayedPerennial()
        {
            var crop = HayedPerennial();
            crop.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 6, 1)});
            return crop;
        }

        private static CropViewItem PerennialNoHarvest()
        {
            return new CropViewItem {CropType = CropType.TameGrass, Year = 1985};
        }

        private static CropViewItem AnnualGrain()
        {
            return new CropViewItem {CropType = CropType.Barley, Year = 1985};
        }

        #endregion

        #region HasHayedHarvest

        [TestMethod]
        public void HasHayedHarvestTrueForHayedPerennial()
        {
            Assert.IsTrue(YieldInputPolicy.HasHayedHarvest(HayedPerennial()));
        }

        [TestMethod]
        public void HasHayedHarvestFalseWhenNoHarvest()
        {
            Assert.IsFalse(YieldInputPolicy.HasHayedHarvest(PerennialNoHarvest()));
        }

        [TestMethod]
        public void HasHayedHarvestFalseForSilageHarvest()
        {
            var crop = new CropViewItem {CropType = CropType.TameGrass, Year = 1985};
            crop.HarvestViewItems.Add(new HarvestViewItem
                {Start = new DateTime(1985, 8, 1), ForageActivity = ForageActivities.Silage});

            Assert.IsFalse(YieldInputPolicy.HasHayedHarvest(crop));
        }

        #endregion

        #region IsYieldReadOnly

        [TestMethod]
        public void YieldReadOnlyForCustomHay()
        {
            Assert.IsTrue(YieldInputPolicy.IsYieldReadOnly(HayedPerennial(), YieldAssignmentMethod.Custom, false));
        }

        [TestMethod]
        public void YieldEditableForCustomGrain()
        {
            Assert.IsFalse(YieldInputPolicy.IsYieldReadOnly(AnnualGrain(), YieldAssignmentMethod.Custom, false));
        }

        [TestMethod]
        public void YieldEditableForCustomPerennialWithoutHay()
        {
            Assert.IsFalse(YieldInputPolicy.IsYieldReadOnly(PerennialNoHarvest(), YieldAssignmentMethod.Custom, false));
        }

        [TestMethod]
        public void YieldReadOnlyForModelledMethod()
        {
            Assert.IsTrue(YieldInputPolicy.IsYieldReadOnly(AnnualGrain(), YieldAssignmentMethod.SmallAreaData, false));
        }

        [TestMethod]
        public void YieldEditableInAdvancedMode()
        {
            Assert.IsFalse(YieldInputPolicy.IsYieldReadOnly(HayedPerennial(), YieldAssignmentMethod.SmallAreaData, true));
        }

        #endregion

        #region IsPlantCarbonReadOnly

        [TestMethod]
        public void PlantCarbonReadOnlyInSimpleMode()
        {
            Assert.IsTrue(YieldInputPolicy.IsPlantCarbonReadOnly(false));
        }

        [TestMethod]
        public void PlantCarbonEditableInAdvancedMode()
        {
            Assert.IsFalse(YieldInputPolicy.IsPlantCarbonReadOnly(true));
        }

        #endregion

        #region IsPercentageReturnedReadOnly

        [TestMethod]
        public void PercentReturnedReadOnlyForPerennial()
        {
            Assert.IsTrue(YieldInputPolicy.IsPercentageReturnedReadOnly(PerennialNoHarvest(), false));
        }

        [TestMethod]
        public void PercentReturnedEditableForAnnual()
        {
            Assert.IsFalse(YieldInputPolicy.IsPercentageReturnedReadOnly(AnnualGrain(), false));
        }

        [TestMethod]
        public void PercentReturnedEditableInAdvancedMode()
        {
            Assert.IsFalse(YieldInputPolicy.IsPercentageReturnedReadOnly(PerennialNoHarvest(), true));
        }

        #endregion

        #region HasHarvestButUsingEstimate (mismatch nudge)

        [TestMethod]
        public void MismatchTrueForModelledWithHay()
        {
            Assert.IsTrue(YieldInputPolicy.HasHarvestButUsingEstimate(HayedPerennial(), YieldAssignmentMethod.SmallAreaData));
        }

        [TestMethod]
        public void MismatchFalseForCustomWithHay()
        {
            Assert.IsFalse(YieldInputPolicy.HasHarvestButUsingEstimate(HayedPerennial(), YieldAssignmentMethod.Custom));
        }

        [TestMethod]
        public void MismatchFalseForModelledWithoutHay()
        {
            Assert.IsFalse(YieldInputPolicy.HasHarvestButUsingEstimate(PerennialNoHarvest(), YieldAssignmentMethod.SmallAreaData));
        }

        #endregion

        #region GetYieldSource

        [TestMethod]
        public void SourceEstimatedForModelled()
        {
            Assert.AreEqual(YieldSource.Estimated, YieldInputPolicy.GetYieldSource(HayedPerennial(), YieldAssignmentMethod.Average));
        }

        [TestMethod]
        public void SourceFromHarvestForCustomHay()
        {
            Assert.AreEqual(YieldSource.FromHarvest, YieldInputPolicy.GetYieldSource(HayedPerennial(), YieldAssignmentMethod.Custom));
        }

        [TestMethod]
        public void SourceIsGrazedWhenTheYearIsGrazedEvenWithAHarvestUnderCustom()
        {
            // UpdateYieldFromHarvestForCustomPerennials skips any year with grazing on it, so calling this one
            // "from your harvest" described a derivation that never ran.
            Assert.AreEqual(YieldSource.Grazed,
                YieldInputPolicy.GetYieldSource(GrazedAndHayedPerennial(), YieldAssignmentMethod.Custom));
        }

        [TestMethod]
        public void SourceIsGrazedUnderAModelledMethodToo()
        {
            // Grazing is asked before the method because it holds under all of them.
            Assert.AreEqual(YieldSource.Grazed,
                YieldInputPolicy.GetYieldSource(GrazedAndHayedPerennial(), YieldAssignmentMethod.Average));
        }

        [TestMethod]
        public void SourceEnteredForCustomNoHay()
        {
            Assert.AreEqual(YieldSource.Entered, YieldInputPolicy.GetYieldSource(PerennialNoHarvest(), YieldAssignmentMethod.Custom));
        }

        #endregion
    }
}
