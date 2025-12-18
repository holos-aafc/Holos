using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Chicken
{
    /// <summary>
    /// Multiplier hatchery: Hatcheries receive fertile eggs from multiplier breeder farms, hatch them and then deliver the
    /// hatchlings (day old chicks or poults) to pullet farms (female chicks only), to pullet barns on egg-laying operations, or
    /// to meat production operations (male and female chicks, for chicken and/or turkey).
    /// </summary>
    public class ChickenMultiplierHatcheryComponent : AnimalComponentBase
    {
        public ChickenMultiplierHatcheryComponent()
        {
            this.ComponentNameDisplayString = ComponentType.ChickenMultiplierHatchery.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.ChickenMultiplierHatchery;

            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipChickenMultiplierHatchery;
        }
    }
}