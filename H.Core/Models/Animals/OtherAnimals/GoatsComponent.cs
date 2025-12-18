using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class GoatsComponent : AnimalComponentBase
    {
        public GoatsComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Goats.GetDescription();
            this.ComponentDescriptionString = Properties.Resources.ToolTipGoatsComponent;
            this.ComponentCategory = ComponentCategory.OtherLivestock;
            this.ComponentType = ComponentType.Goats;
        }
    }
}