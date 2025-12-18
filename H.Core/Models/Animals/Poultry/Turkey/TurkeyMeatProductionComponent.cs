using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Turkey
{
    /// <summary>
    /// Turkey meat production: poults arriving in the operation from a multiplier hatchery are raised to market weight after approximately 12 weeks
    /// (turkey broilers) or 16-22 weeks (heavy turkeys). Birds are grown to different weights depending on the end product: Broiler (turkey): < 5 kg;
    /// Light hens: 5-7 kg; Heavy hens: 7-9 kg; Light toms: 9-11 kg; Heavy toms: >11 kg (for processed products).
    /// </summary>
    public class TurkeyMeatProductionComponent : AnimalComponentBase
    {
        public TurkeyMeatProductionComponent()
        {
            this.ComponentNameDisplayString = ComponentType.TurkeyMeatProduction.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.TurkeyMeatProduction;

            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipTurkeyMeatProduction;
        }
    }
}