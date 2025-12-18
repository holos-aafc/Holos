using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Chicken
{
    /// <summary>
    /// Egg production (layers): once pullets reach approximately 18-20 weeks of age they are transferred from either separate
    /// pullet operations or pullet barns on an egg production operation to egg-laying barns. These hens will produce eggs for up to 71 weeks of age.		
    /// </summary>
    public class ChickenEggProductionComponent : AnimalComponentBase
    {
        public ChickenEggProductionComponent()
        {
            this.ComponentNameDisplayString = ComponentType.ChickenEggProduction.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.ChickenEggProduction;

            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipChickenEggProduction;
        }
    }
}