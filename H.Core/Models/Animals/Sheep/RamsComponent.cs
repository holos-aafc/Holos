using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Sheep
{
    public class RamsComponent : AnimalComponentBase
    {
        public RamsComponent()
        {
            ComponentNameDisplayString = ComponentType.Rams.GetDescription();
            ComponentDescriptionString = Resources.ToolTipRamsComponent;
            ComponentCategory = ComponentCategory.Sheep;
            ComponentType = ComponentType.Rams;
        }
    }
}