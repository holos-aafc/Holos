using System;
using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Models;
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

        #region GetYieldSource - the precedence, as a table

        /// <summary>
        /// Every combination of the four things that decide a yield, in one place. The precedence used to be written
        /// out separately by each consumer - the source label, the read-only rule, the Harvest tab summary - and they
        /// drifted: the label learned that grazing decides the yield while the read-only rule never asked about
        /// grazing, so a grazed field under a user-supplied yield showed an editable cell beside a tooltip saying the
        /// value was not the user's to set. This table is what stops that happening again.
        /// </summary>
        private static IEnumerable<object[]> PrecedenceMatrix()
        {
            // method, grazed, hayed, expected source, expected editable (outside advanced mode)
            yield return new object[] {YieldAssignmentMethod.Custom,        false, false, YieldSource.Entered,                     true};
            yield return new object[] {YieldAssignmentMethod.Custom,        false, true,  YieldSource.FromHarvest,                 false};
            yield return new object[] {YieldAssignmentMethod.Custom,        true,  false, YieldSource.GrazedEnteredAsTotalBiomass, true};
            yield return new object[] {YieldAssignmentMethod.Custom,        true,  true,  YieldSource.GrazedEnteredAsTotalBiomass, true};

            yield return new object[] {YieldAssignmentMethod.SmallAreaData, false, false, YieldSource.Estimated,                   false};
            yield return new object[] {YieldAssignmentMethod.SmallAreaData, false, true,  YieldSource.FromHarvest,                 false};
            yield return new object[] {YieldAssignmentMethod.SmallAreaData, true,  false, YieldSource.GrazedDerived,               false};
            yield return new object[] {YieldAssignmentMethod.SmallAreaData, true,  true,  YieldSource.GrazedDerived,               false};

            yield return new object[] {YieldAssignmentMethod.Average,       false, false, YieldSource.Estimated,                   false};
            yield return new object[] {YieldAssignmentMethod.Average,       false, true,  YieldSource.FromHarvest,                 false};
            yield return new object[] {YieldAssignmentMethod.Average,       true,  false, YieldSource.GrazedDerived,               false};
            yield return new object[] {YieldAssignmentMethod.Average,       true,  true,  YieldSource.GrazedDerived,               false};

            yield return new object[] {YieldAssignmentMethod.InputFile,     false, false, YieldSource.Estimated,                   false};
            yield return new object[] {YieldAssignmentMethod.InputFile,     false, true,  YieldSource.FromHarvest,                 false};
            yield return new object[] {YieldAssignmentMethod.InputFile,     true,  false, YieldSource.GrazedDerived,               false};
            yield return new object[] {YieldAssignmentMethod.InputFile,     true,  true,  YieldSource.GrazedDerived,               false};
        }

        private static CropViewItem Build(bool grazed, bool hayed)
        {
            var crop = new CropViewItem {CropType = CropType.TameGrass, Year = 1985};

            if (hayed)
            {
                crop.HarvestViewItems.Add(new HarvestViewItem
                    {Start = new DateTime(1985, 8, 1), ForageActivity = ForageActivities.Hayed});
            }

            if (grazed)
            {
                crop.GrazingViewItems.Add(new GrazingViewItem {Start = new DateTime(1985, 6, 1)});
            }

            return crop;
        }

        [TestMethod]
        public void EveryCombinationResolvesToOneSource()
        {
            foreach (var row in PrecedenceMatrix())
            {
                var method = (YieldAssignmentMethod)row[0];
                var crop = Build((bool)row[1], (bool)row[2]);
                var expected = (YieldSource)row[3];

                Assert.AreEqual(expected, YieldInputPolicy.GetYieldSource(crop, method),
                    $"method={method} grazed={row[1]} hayed={row[2]}");
            }
        }

        [TestMethod]
        public void ReadOnlyStateAgreesWithTheSourceForEveryCombination()
        {
            // The cell is editable exactly where the user supplies the value. Derived from the source rather than
            // restated, so these two can no longer disagree.
            foreach (var row in PrecedenceMatrix())
            {
                var method = (YieldAssignmentMethod)row[0];
                var crop = Build((bool)row[1], (bool)row[2]);
                var expectedEditable = (bool)row[4];

                Assert.AreEqual(expectedEditable, YieldInputPolicy.IsYieldReadOnly(crop, method, false) == false,
                    $"method={method} grazed={row[1]} hayed={row[2]}");
            }
        }

        [TestMethod]
        public void AdvancedInputEditingMakesEveryCombinationEditable()
        {
            foreach (var row in PrecedenceMatrix())
            {
                var crop = Build((bool)row[1], (bool)row[2]);

                Assert.IsFalse(YieldInputPolicy.IsYieldReadOnly(crop, (YieldAssignmentMethod)row[0], true),
                    $"method={row[0]} grazed={row[1]} hayed={row[2]}");
            }
        }

        [TestMethod]
        public void TheSummaryPanelDescribesTheSameSourceForEveryCombination()
        {
            // The third consumer. It used to re-derive the grazed branch itself, which is where the drift began.
            foreach (var row in PrecedenceMatrix())
            {
                var method = (YieldAssignmentMethod)row[0];
                var crop = Build((bool)row[1], (bool)row[2]);
                crop.Area = 1;

                var field = new FieldSystemComponent {YieldAssignmentMethod = method};
                crop.FieldSystemComponentGuid = field.Guid;
                var farm = new Farm {YieldAssignmentMethod = method};
                farm.Components.Add(field);

                var summary = FieldSummaryPolicy.Describe(crop, farm);

                Assert.IsFalse(string.IsNullOrWhiteSpace(summary.YieldSource),
                    $"no summary sentence for method={method} grazed={row[1]} hayed={row[2]}");

                // A grazed year must never be described as taking its yield from the cut.
                if ((bool)row[1])
                {
                    Assert.IsFalse(summary.YieldSource.IndexOf("bale", StringComparison.OrdinalIgnoreCase) >= 0,
                        $"grazed year described as coming from the cut: method={method} hayed={row[2]}");
                }
            }
        }

        #endregion
    }
}
