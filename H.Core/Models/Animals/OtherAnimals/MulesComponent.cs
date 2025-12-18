using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class MulesComponent : AnimalComponentBase
    {
        public MulesComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Mules.GetDescription();
            this.ComponentDescriptionString = Properties.Resources.ToolTipMulesComponent;
            this.ComponentCategory = ComponentCategory.OtherLivestock;
            this.ComponentType = ComponentType.Mules;
        }
    }
}