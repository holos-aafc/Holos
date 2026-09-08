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
