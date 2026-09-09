using System;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.LandManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Core.Test.Services
{
    /// <summary>
    /// The field summary tells the user what Holos did with their field. Its value depends entirely on it being true,
    /// so each branch is pinned here rather than trusted to review.
    /// </summary>
    [TestClass]
    public class FieldSummaryPolicyTest
    {
        #region Private Methods

        private Farm CreateFarm(out CropViewItem viewItem, YieldAssignmentMethod method)
        {
            var farm = new Farm() {YieldAssignmentMethod = method};
            var field = new FieldSystemComponent();

            viewItem = new CropViewItem()
            {
                CropType = CropType.TameGrass,
                Year = 2026,
                Area = 10,
                FieldSystemComponentGuid = field.Guid,
                PercentageOfProductYieldReturnedToSoil = 35,
            };

            field.CropViewItems.Add(viewItem);
            farm.Components.Add(field);

            return farm;
        }

        private static void AddHayHarvest(CropViewItem viewItem, int bales = 35)
        {
            viewItem.HarvestViewItems.Add(new HarvestViewItem()
            {
                Start = new DateTime(viewItem.Year, 8, 1),
                ForageActivity = ForageActivities.Hayed,
                TotalNumberOfBalesHarvested = bales,
                BaleWeight = 500,
                MoistureContentAsPercentage = 15,
                HarvestLossPercentage = 35,
            });
        }

        private static void AddGrazing(CropViewItem viewItem, double utilization = 60)
        {
            viewItem.GrazingViewItems.Add(new GrazingViewItem()
            {
                Start = new DateTime(viewItem.Year, 6, 1),
                Utilization = utilization,
            });
        }

        #endregion

        #region Tests

        [TestMethod]
        public void DescribeSaysNothingForAnAnnualCrop()
        {
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.Custom);
            viewItem.CropType = CropType.Wheat;

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            Assert.IsFalse(summary.HasContent, "none of these rules apply to an annual crop");
        }

        [TestMethod]
        public void DescribeReportsAHayedFieldUnderCustom()
        {
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.Custom);
            AddHayHarvest(viewItem);

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            StringAssert.Contains(summary.Activity, "hayed in 2026");
            StringAssert.Contains(summary.YieldSource, "Your harvest is this field's yield");
            StringAssert.Contains(summary.ReturnedToSoil, "Your harvest sets how much");
        }

        [TestMethod]
        public void DescribeSaysTheHarvestDrivesTheYieldUnderAModelledMethodToo()
        {
            // There is no longer a case where a cut is entered and an estimate is used instead, so the panel no longer
            // has to explain one away. Under Small Area Data the cut still sets the yield and the panel says so.
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.SmallAreaData);
            AddHayHarvest(viewItem);

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            StringAssert.Contains(summary.YieldSource, "harvest");
            Assert.IsFalse(summary.YieldSource.Contains("not used"),
                "the panel must not claim the harvest is being ignored");
        }

        [TestMethod]
        public void DescribeReportsAGrazedFieldUnderAModelledMethod()
        {
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.SmallAreaData);
            AddGrazing(viewItem);
            viewItem.PercentageOfProductYieldReturnedToSoil = 40;

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            StringAssert.Contains(summary.Activity, "grazed in 2026");
            StringAssert.Contains(summary.YieldSource, "forage the animals ate");
            StringAssert.Contains(summary.ReturnedToSoil, "animals leave");
        }

        [TestMethod]
        public void DescribeSaysTheEnteredYieldIsTotalBiomassOnAGrazedCustomField()
        {
            // Under Custom with grazing the entered yield is taken to be everything that grew, so claiming it came from
            // the animals' intake would be wrong.
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.Custom);
            AddGrazing(viewItem);

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            StringAssert.Contains(summary.YieldSource, "all the biomass grown");
        }

        [TestMethod]
        public void DescribeReportsBothRemovalsOnAGrazedAndHayedField()
        {
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.SmallAreaData);
            AddGrazing(viewItem);
            AddHayHarvest(viewItem);
            viewItem.PercentageOfProductYieldReturnedToSoil = 38;

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            StringAssert.Contains(summary.Activity, "grazed and hayed in 2026");
            StringAssert.Contains(summary.ReturnedToSoil, "hay baled off");
        }

        [TestMethod]
        public void DescribeReportsAllProductReturnedWhenNothingRemovedIt()
        {
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.SmallAreaData);
            viewItem.PercentageOfProductYieldReturnedToSoil = 100;

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            StringAssert.Contains(summary.Activity, "neither grazed nor hayed");
            StringAssert.Contains(summary.ReturnedToSoil, "All of the product is returned");

            // With no harvest entered there is nothing to explain the fate of, so no offer to switch method.
            Assert.IsFalse(summary.YieldSource.Contains("Custom"));
        }

        [TestMethod]
        public void DescribeReportsAManualOverrideAheadOfEveryOtherRule()
        {
            // A pinned value beats everything, so the summary must not claim the harvest is driving the yield.
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.Custom);
            AddHayHarvest(viewItem);
            viewItem.DoNotRecalculateYield = true;
            viewItem.DoNotRecalculatePercentageReturnedToSoil = true;

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            StringAssert.Contains(summary.YieldSource, "no longer recalculates");
            StringAssert.Contains(summary.ReturnedToSoil, "no longer recalculates");
        }

        [TestMethod]
        public void DescribeIgnoresAHarvestFromAnotherYear()
        {
            // Everything here is scoped to the item's own year, as the calculation is.
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.Custom);
            AddHayHarvest(viewItem);
            viewItem.HarvestViewItems[0].Start = new DateTime(2020, 8, 1);

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            StringAssert.Contains(summary.Activity, "neither grazed nor hayed");
        }

        [TestMethod]
        public void DescribeNeverQuotesTheReturnedPercentage()
        {
            // UpdatePercentageReturnsForPerennials runs on the DETAIL view items, so a component-screen item carries
            // whatever initialization set rather than what the model computed. Quoting it here would show a stale
            // number that no amount of refreshing could fix, so the lines name the rule instead.
            var farm = CreateFarm(out var viewItem, YieldAssignmentMethod.Custom);
            AddHayHarvest(viewItem);
            viewItem.PercentageOfProductYieldReturnedToSoil = 12345;

            var summary = FieldSummaryPolicy.Describe(viewItem, farm);

            Assert.IsFalse(summary.ReturnedToSoil.Contains("12345"));
        }

        #endregion
    }
}
