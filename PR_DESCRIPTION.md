# Make the entered hay harvest the source of a perennial field's yield

Branches: `feature/hay-yield-single-source` (this repo) and
`feature/hay-yield-single-source-gui` (views). Both are needed; the views branch will not build
against `main` alone.

## The problem

A perennial hay field carried two unrelated yields, and nothing reconciled them.

1. A **modelled yield** assigned by the yield provider (Small area data, Average, or typed by the user).
2. The **hay the user actually baled**, entered on the Harvest tab as bales x bale weight.

Only the first drove the carbon calculation. A user could enter a season's harvest in detail and see
no effect on soil carbon, with nothing on screen to say the number was being ignored. Alongside that,
the Harvest tab's "Harvest loss %" was a live input wired to nothing, and a perennial year with no
harvest and no grazing kept the 35% default return when the algorithm document says all of the product
stays on the field.

## What changes

**Under the Custom yield assignment method, the entered harvest becomes the field's yield.** The
modelled methods are untouched and still use their estimate - the harvest is only the source of truth
where the user has said they are supplying the numbers.

Everything derived from that yield is now presented as derived: the yield, plant carbon in product,
and percentage of product returned to soil are read-only, greyed, and explained. An **Advanced input
editing** switch makes them editable for a user who needs to override one; an overridden value shows
in amber, survives recalculation, and can be undone with **Reset overrides**.

## Correctness fixes

- **A perennial with no harvest and no grazing returns 100% of its product to soil**, not the 35%
  default. Section 2.1.2.6.2 of the algorithm document asked the user to make this change by hand;
  Holos now does it.
- **The hayed harvest's "Harvest loss %" drives the percentage of product returned to soil**,
  biomass-weighted across multiple cuts. The document defines one parameter, S_p, for both the
  harvest-loss gross-up and the returned fraction; Holos had two independent numbers.
- **A grazed perennial's return is derived from grazing utilization** and written back, so the value
  shown is the value used. Previously the carbon calculator derived it privately and the screen showed
  a default that disagreed.
- **The harvest-derived yield is put on the crop's moisture basis.** A bale is dried to around 15%
  moisture while Yield means the standing crop at around 80%, and C_p multiplies Yield by 1 minus the
  crop's moisture. Feeding the bale's wet weight in directly understated a hayed field's carbon about
  fourfold.
- **A hay harvest is carried across the simulation**, which is what the document describes - the
  historical period is built from the rotation the user specifies once. The year stamp was landing on
  `DateCreated` while the code that reads harvests filters on `Start.Year`, so a harvest only ever
  applied to the year it was entered. A per-harvest **Repeats every year** checkbox marks the exception,
  and the same control is offered on the Fertilizer and supplemental hay tables.
- **Cached values are recomputed with the collections they summarise.** A crop view item caches the
  N-P-K-S rates and the "has any" flags for its collections, and the nitrogen calculations read the
  cached rate rather than summing the fertilizer applications. A year kept out of a repeat therefore
  still carried its full nitrogen rate into the synthetic N2O, and a year with no harvest made
  `CalculateHarvest` (Eq. 11.4.4-1) sum an empty table instead of falling back to the year's yield.
- **The hay cut off a grazed field is accounted for.** For a field both grazed and hayed, the carbon in
  the baled hay was in neither what the field produced nor what was removed from it, so the residue the
  cut leaves behind was never counted as an input. Eq. 11.3.2-8 subtracts both removals - the modelling
  team confirmed the document's sign error.
- **The yield assignment method is resolved per field** in the grazing paths, which read the farm-level
  method directly and would have ignored field-level assignment.
- **The fertilizer and digestate copy loops added the original item rather than the mapped copy**, so
  every year shared one instance and an edit in one year changed all of them.
- **Plant carbon was clamped to 100** by a `Maximum` on its cell editor, though it is kg C/ha and runs
  into the thousands.

## Behaviour change for existing farms

Farms with a hayed perennial under the Custom method will produce different soil carbon after this
change. Both the moisture-basis correction and the harvest now applying across the stand move the
numbers, and they move in opposite directions, so the net effect depends on how the entered harvest
compares with the modelled estimate. In the test fixture, total carbon inputs on the hayed field fall
from 2617 to 741 kg C/ha because the entered harvest is much smaller than the regional estimate.

Grandfathering old farms into the previous behaviour was considered and rejected: it would fork
results silently on file creation date and leave the wrong behaviour as the permanent default. **A
version bump and release note are still outstanding and should land before release.**

## Testing

`H.Core` is green at 1328 passed / 0 failed / 14 skipped.

The existing golden baselines could not see any of this work - they call `CalculateFinalResults`,
which does not rebuild the detail view items, while these adjustments run inside
`CreateDetailViewItems`; and no fixture farm had a single harvest, fertilizer or digestate
application. `PerennialYieldBaselineTest` and the `Farm4` fixture were added to close that, running
the GUI path and snapshotting yield, plant carbon, percentage returned and carbon inputs per field and
year. Regenerate the fixture with `HOLOS_BUILD_FIXTURES=1` and the baselines with
`HOLOS_UPDATE_BASELINES=1`.

Every GUI change has been exercised in the running application.

## Deliberately not included

**Repeats every year is not offered on the Manure or Digestate tables.** Holos answers "which year does
this application belong to?" two different ways: the carbon and N2O calculations resolve it from the
date on the application, while `ManureCarbonInputsFromManureOnly` and a recalculation in `CarbonService`
use whichever years hold a copy of it. An application dated outside its own crop row's year is counted
in a different year by each - one fixture field has an imported application on a 2026 row dated
2025-01-01. No default preserves existing results and makes the checkbox honest at the same time, so the
control is withheld rather than shipped inert. The question is written up for the modelling team.

## Still outstanding

- Version bump to 4.0.1.135. The changelog entry is written; the `H.csproj` bump is not in this branch.
- A changelog line for the Fertilizer and supplemental hay toggles - the existing entry covers only the
  Harvest tab.
- French: `UserGuide_French.md` and `fr-CA` strings for everything added here.
- Four corrections for the algorithm document authors are written up separately, the substantive one
  being that Eq. 2.1.2-1 states the harvest-loss gross-up as an addition where its own Eq. 2.1.2-20 and
  Eq. 11.3.2-5 require a division.
