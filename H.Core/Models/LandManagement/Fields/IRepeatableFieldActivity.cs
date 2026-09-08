namespace H.Core.Models.LandManagement.Fields
{
    /// <summary>
    /// Management the user enters against a field that Holos carries into every year of the simulation, and which they
    /// can mark as belonging to one year only.
    ///
    /// The algorithm document builds the historical period from the rotation the user specifies once, so repeating is
    /// the default and an entry happening in a single year is the exception. Implementers are the collections copied by
    /// <see cref="H.Core.Services.LandManagement.FieldResultsService"/> when it creates the detail view items.
    ///
    /// Manure and digestate applications implement this but are NOT yet given the checkbox in the interface, so they
    /// keep the repeating default and behave as they always have. Which year one of them belongs to is currently
    /// answered two different ways - the carbon and N2O calculations resolve it from the date on the application, while
    /// a couple of other paths simply use whatever copies a year holds - and an application dated outside its own crop
    /// row's year is counted by each of them in a different year. The control cannot mean anything on those two tables
    /// until that is settled, so it is deliberately absent rather than present and inert.
    ///
    /// Grazing view items deliberately do NOT implement this. They are a derived mirror of the animal components'
    /// management periods, rebuilt by InitializeGrazingViewItems, so which years they apply to is already answered by
    /// the animal data - a flag here would be a second, competing source of truth, and would be wiped on the next
    /// rebuild besides.
    /// </summary>
    public interface IRepeatableFieldActivity
    {
        /// <summary>
        /// True when this entry describes management that recurs whenever the crop is grown, so it is carried into every
        /// year of the simulation. False marks an entry that happened in a single year only.
        /// </summary>
        bool RepeatsInEveryYear { get; set; }
    }
}
