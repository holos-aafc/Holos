using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class GoatsComponent : AnimalComponentBase
    {
        public GoatsComponent()
        {
            ComponentNameDisplayString = ComponentType.Goats.GetDescription();
            ComponentDescriptionString = Resources.ToolTipGoatsComponent;
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.Goats;
        }
    }
}