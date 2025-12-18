using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class DeerComponent : AnimalComponentBase
    {
        public DeerComponent()
        {
            ComponentNameDisplayString = ComponentType.Deer.GetDescription();
            ComponentDescriptionString = Resources.ToolTipDeerComponent;
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.Deer;
        }
    }
}