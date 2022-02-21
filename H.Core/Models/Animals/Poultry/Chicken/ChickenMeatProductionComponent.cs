using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Chicken
{
    /// <summary>
    /// Chicken meat production/broiler operation: chicks arriving in the operation from a multiplier hatchery
    /// are raised to market weight (1-4 kg, depending on bird type and end product) after approximately 30-56 days
    /// (depending on the bird type and rearing system)
    /// </summary>
    public class ChickenMeatProductionComponent : AnimalComponentBase
    {
        public ChickenMeatProductionComponent()
        {
            this.ComponentNameDisplayString = ComponentType.ChickenMeatProduction.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.ChickenMeatProduction;

            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipChickenMeatProduction;
        }
    }
}