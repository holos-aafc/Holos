using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Chicken
{
    /// <summary>
    ///     Multiplier hatchery: Hatcheries receive fertile eggs from multiplier breeder farms, hatch them and then deliver the
    ///     hatchlings (day old chicks or poults) to pullet farms (female chicks only), to pullet barns on egg-laying
    ///     operations, or
    ///     to meat production operations (male and female chicks, for chicken and/or turkey).
    /// </summary>
    public class ChickenMultiplierHatcheryComponent : AnimalComponentBase
    {
        public ChickenMultiplierHatcheryComponent()
        {
            ComponentNameDisplayString = ComponentType.ChickenMultiplierHatchery.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.ChickenMultiplierHatchery;

            ComponentDescriptionString = Resources.ToolTipChickenMultiplierHatchery;
        }
    }
}