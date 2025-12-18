using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class HorsesComponent : AnimalComponentBase
    {
        public HorsesComponent()
        {
            ComponentNameDisplayString = ComponentType.Horses.GetDescription();
            ComponentDescriptionString = Resources.ToolTipHorsesComponent;
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.Horses;
        }
    }
}