using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.LandManagement
{
    /// <summary>
    /// Where a field's yield comes from (used to label the yield in the UI).
    /// </summary>
    public enum YieldSource
    {
        /// <summary>Regional/modelled estimate (Small Area Data, Average, etc.).</summary>
        Estimated,

        /// <summary>Derived from the user's entered hay cut, whichever yield assignment method is selected.</summary>
        FromHarvest,

        /// <summary>Typed directly by the user (Custom method, no hay cut to derive from).</summary>
        Entered,

        /// <summary>
        /// The year is grazed, so the harvest does not set the yield whatever else the user has entered. Grazing takes
        /// precedence over a hay cut: the derivation skips any year with grazing on it, because a grazed field's yield
        /// stands for the whole standing crop. Without this case the label claimed the yield came from a cut that was
        /// never used.
        /// </summary>
        Grazed,
    }

    /// <summary>
    /// Pure, testable rules for which of the interdependent yield inputs (yield, plant carbon, percentage returned to
    /// soil) are user inputs vs. derived outputs on the field details screen, plus how the yield's source is labelled.
    ///
    /// This is the logic behind the "harvest is the single source of truth" redesign: in the default (simple) view the
    /// derived quantities are read-only; an "advanced input editing" mode makes them editable again. The GUI binds to
    /// these rules through a value converter - the rules live here so they can be unit tested without the GUI.
    /// </summary>
    public static class YieldInputPolicy
    {
        /// <summary>
        /// True when the field has at least one hayed harvest for its year (silage / swath / grazed harvests do not count).
        /// A hayed harvest is what lets the yield be derived from the entered biomass.
        /// </summary>
        public static bool HasHayedHarvest(CropViewItem viewItem)
        {
            return viewItem != null &&
                   viewItem.GetHayHarvests().Any(harvest => harvest.ForageActivity == ForageActivities.Hayed);
        }

        /// <summary>
        /// Yield is read-only unless it is a genuine typed input: it stays editable only in advanced mode, or under the
        /// Custom method for a field whose yield is NOT derived from a hay cut (e.g. grain, or a perennial with no cut
        /// that year). A cut derives the yield under every method now, so a cut makes the cell read-only regardless of
        /// the method, as does any modelled method supplying an estimate.
        /// </summary>
        public static bool IsYieldReadOnly(CropViewItem viewItem, YieldAssignmentMethod method, bool advancedEditing)
        {
            if (advancedEditing)
            {
                return false;
            }

            return method != YieldAssignmentMethod.Custom || HasHayedHarvest(viewItem);
        }

        /// <summary>
        /// Plant carbon in agricultural product (C_p) is always a derived intermediate, so it is read-only unless the
        /// user has turned on advanced input editing.
        /// </summary>
        public static bool IsPlantCarbonReadOnly(bool advancedEditing)
        {
            return advancedEditing == false;
        }

        /// <summary>
        /// Percentage of product returned to soil is derived for every perennial (from harvest loss when hayed, from
        /// utilization when grazed, or 100% when neither), so it is read-only for perennials outside advanced mode. For
        /// annual crops it remains an editable residue parameter.
        /// </summary>
        public static bool IsPercentageReturnedReadOnly(CropViewItem viewItem, bool advancedEditing)
        {
            if (advancedEditing)
            {
                return false;
            }

            return viewItem != null && viewItem.CropType.IsPerennial();
        }

        /// <summary>
        /// Where the field's yield comes from, for labelling: a modelled estimate, the user's entered harvest, or a
        /// directly typed value.
        /// </summary>
        public static YieldSource GetYieldSource(CropViewItem viewItem, YieldAssignmentMethod method)
        {
            // Grazing first, because it holds under every method: a grazed year's yield is never taken from a hay cut,
            // so labelling such a year "from your harvest" would describe a derivation that did not run.
            if (viewItem.HasGrazingItemsForTheCurrentYear())
            {
                return YieldSource.Grazed;
            }

            // Then the cut, also under every method. The assignment method only supplies the years without one.
            if (HasHayedHarvest(viewItem))
            {
                return YieldSource.FromHarvest;
            }

            return method == YieldAssignmentMethod.Custom ? YieldSource.Entered : YieldSource.Estimated;
        }
    }
}
