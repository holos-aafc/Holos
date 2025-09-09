using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Turkey
{
    /// <summary>
    ///     Multiplier breeder operation: represents the reproduction portion of bird production where birds that produce
    ///     eggs for reproduction are managed; these eggs are then sent to reproduction hatcheries to be hatched. In the
    ///     multiplier breeder operation, male and
    ///     female turkeys are housed separately for the entire life cycle.
    /// </summary>
    public class TurkeyMultiplierBreederComponent : AnimalComponentBase
    {
        public TurkeyMultiplierBreederComponent()
        {
            ComponentNameDisplayString = ComponentType.TurkeyMultiplierBreeder.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.TurkeyMultiplierBreeder;

            ComponentDescriptionString = Resources.ToolTipTurkeyMultiplierBreeder;
        }
    }
}