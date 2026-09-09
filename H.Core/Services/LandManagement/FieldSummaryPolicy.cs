using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Properties;

namespace H.Core.Services.LandManagement
{
    /// <summary>
    /// The plain-language summary shown for a field: what it did this year, where its yield came from, and what
    /// returned to the soil. Null lines are simply not shown.
    /// </summary>
    public class FieldSummary
    {
        public string Activity { get; set; }
        public string YieldSource { get; set; }
        public string ReturnedToSoil { get; set; }

        public bool HasContent => this.Activity != null;
    }

    /// <summary>
    /// Describes, in plain language, what Holos did with a perennial field in a given year.
    ///
    /// A field's carbon depends on four things the user sets in different places - the yield assignment method, hay
    /// harvests, grazing animals, and any manual override - and the rules combining them are not discoverable from the
    /// screens. Several are counter-intuitive: a harvest changes the yield under Custom and not otherwise, a grazed
    /// field's yield is worked out backwards from what the animals ate, grazing takes precedence over haying for the
    /// return to soil, and a field with neither returns all of its product.
    ///
    /// Every sentence here is derived from the same rules the calculation uses, never written independently. If a rule
    /// changes and this is not updated, it still tells the truth, because it asks the question the calculation asks.
    /// </summary>
    public static class FieldSummaryPolicy
    {
        public static FieldSummary Describe(CropViewItem viewItem, Farm farm)
        {
            var summary = new FieldSummary();

            // None of these rules apply to an annual crop; the panel keeps its ordinary content there.
            if (viewItem == null || farm == null || viewItem.CropType.IsPerennial() == false)
            {
                return summary;
            }

            var field = farm.GetFieldSystemComponent(viewItem.FieldSystemComponentGuid);
            var method = farm.GetYieldAssignmentMethod(field);

            var isGrazed = viewItem.HasGrazingItemsForTheCurrentYear();
            var isHayed = viewItem.IsHarvested();

            summary.Activity = GetActivity(viewItem, isGrazed, isHayed);
            summary.YieldSource = GetYieldSource(viewItem, method);
            summary.ReturnedToSoil = GetReturnedToSoil(viewItem, isGrazed, isHayed);

            return summary;
        }

        private static string GetActivity(CropViewItem viewItem, bool isGrazed, bool isHayed)
        {
            if (isGrazed && isHayed)
            {
                return string.Format(Resources.SummaryActivityGrazedAndHayed, viewItem.Year);
            }

            if (isGrazed)
            {
                return string.Format(Resources.SummaryActivityGrazed, viewItem.Year);
            }

            if (isHayed)
            {
                return string.Format(Resources.SummaryActivityHayed, viewItem.Year);
            }

            return string.Format(Resources.SummaryActivityNeither, viewItem.Year);
        }

        private static string GetYieldSource(CropViewItem viewItem, YieldAssignmentMethod method)
        {
            // A pinned value beats every other rule - nothing recalculates it.
            if (viewItem.DoNotRecalculateYield)
            {
                return Resources.SummaryYieldOverridden;
            }

            // Every case below comes from YieldInputPolicy.GetYieldSource, which is the one place the precedence lives.
            // This used to re-derive the grazed branch itself, which is how the two drifted apart.
            switch (YieldInputPolicy.GetYieldSource(viewItem, method))
            {
                case LandManagement.YieldSource.GrazedEnteredAsTotalBiomass:
                    return Resources.SummaryYieldEnteredAsTotalBiomass;

                case LandManagement.YieldSource.GrazedDerived:
                    return Resources.SummaryYieldFromGrazing;

                case LandManagement.YieldSource.FromHarvest:
                    var bales = viewItem.GetHayHarvests()
                        .Where(harvest => harvest.ForageActivity == ForageActivities.Hayed)
                        .Sum(harvest => harvest.TotalNumberOfBalesHarvested);

                    return string.Format(Resources.SummaryYieldFromHarvest, bales);

                case LandManagement.YieldSource.Entered:
                    return Resources.SummaryYieldEntered;

                default:
                    // Only reachable with no cut for this year. A cut sets the yield under every method now, so there
                    // is no longer a case where a harvest was entered and the estimate was used instead.
                    return Resources.SummaryYieldEstimate;
            }
        }

        private static string GetReturnedToSoil(CropViewItem viewItem, bool isGrazed, bool isHayed)
        {
            if (viewItem.DoNotRecalculatePercentageReturnedToSoil)
            {
                return Resources.SummaryReturnedOverridden;
            }

            // These name the rule rather than quoting a percentage. UpdatePercentageReturnsForPerennials runs on the
            // DETAIL view items, so a component-screen item carries whatever initialization set, not what the model
            // computed - quoting it here would show a stale number that no amount of refreshing could fix. The actual
            // values are on the same screen anyway, in the harvest loss column and on the grazing tab.
            if (isGrazed && isHayed)
            {
                return Resources.SummaryReturnedBothRemovals;
            }

            if (isGrazed)
            {
                return Resources.SummaryReturnedAfterGrazing;
            }

            if (isHayed)
            {
                return Resources.SummaryReturnedAfterCut;
            }

            return Resources.SummaryReturnedAll;
        }
    }
}
