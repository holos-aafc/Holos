using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Chicken
{
    /// <summary>
    /// Multiplier breeder operation: represents the reproduction portion of bird production where birds that produce eggs for reproduction are managed;
    /// these eggs are then sent to reproduction hatcheries to be hatched. In the multiplier breeder operation, adult males and females are housed together.
    /// </summary>
    public class ChickenMultiplierBreederComponent : AnimalComponentBase
    {
        public ChickenMultiplierBreederComponent()
        {
            this.ComponentNameDisplayString = ComponentType.ChickenMultiplierBreeder.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.ChickenMultiplierBreeder;

            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipChickenMultiplierBreeder;
        }
    }
}