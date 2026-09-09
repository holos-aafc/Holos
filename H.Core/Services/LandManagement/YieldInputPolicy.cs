using System.Linq;
using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Services.LandManagement
{
    /// <summary>
    /// What decided a field's yield for one year. This is the precedence itself, in one place: every consumer - the
    /// cell's read-only state, its tooltip, the Harvest tab summary - answers from this rather than restating the rule.
    ///
    /// Each case was previously re-derived independently, which is how they drifted apart: the tooltip learned that
    /// grazing decides the yield while the read-only rule never asked about grazing at all, so a grazed field under a
    /// user-supplied yield showed an editable cell beside a tooltip saying the value was not the user's to set.
    /// </summary>
    public enum YieldSource
    {
        /// <summary>Regional/modelled estimate (Small Area Data, Average, Input file).</summary>
        Estimated,

        /// <summary>Derived from the user's entered hay cut, whichever yield assignment method is selected.</summary>
        FromHarvest,

        /// <summary>Typed directly by the user (Custom method, no hay cut and no grazing to derive from).</summary>
        Entered,

        /// <summary>
        /// Grazed, with the yield worked backwards from what the animals ate and what was baled (Eq. 11.3.2-9). An
        /// output, not an input.
        /// </summary>
        GrazedDerived,

        /// <summary>
        /// Grazed, but the user supplied the yield, which the algorithm document then takes to be the total aboveground
        /// biomass - what the animals ate plus what they left - with no gross-up (note under Eq. 2.1.2-1). Still the
        /// user's own input, so the cell stays editable; it simply means something different from an ordinary yield.
        /// </summary>
        GrazedEnteredAsTotalBiomass,
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
        /// Yield is read-only unless it is a genuine typed input. Rather than restate the precedence, this asks
        /// <see cref="GetYieldSource"/> what decided the yield and edits only what the user actually supplies: a value
        /// typed under the Custom method, with or without grazing. Everything else - an estimate, a value derived from
        /// a cut, a value worked backwards from grazing - is an output and is shown read-only.
        ///
        /// Deriving it this way is what keeps the cell's state and its tooltip from disagreeing. The rule used to be
        /// written out separately here and asked only about the method and the cut, so a grazed field under a
        /// user-supplied yield offered an editable cell while its tooltip said the value was not the user's to set.
        /// </summary>
        public static bool IsYieldReadOnly(CropViewItem viewItem, YieldAssignmentMethod method, bool advancedEditing)
        {
            if (advancedEditing)
            {
                return false;
            }

            var source = GetYieldSource(viewItem, method);

            return source != YieldSource.Entered && source != YieldSource.GrazedEnteredAsTotalBiomass;
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
            // so labelling such a year "from your harvest" would describe a derivation that did not run. Which of the
            // two grazed cases applies is decided by the method - the two columns of the grazed-and-hayed chart.
            if (viewItem.HasGrazingItemsForTheCurrentYear())
            {
                return method == YieldAssignmentMethod.Custom
                    ? YieldSource.GrazedEnteredAsTotalBiomass
                    : YieldSource.GrazedDerived;
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
