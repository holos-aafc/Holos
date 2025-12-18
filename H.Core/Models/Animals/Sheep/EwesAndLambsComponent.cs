using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Sheep
{
    public class EwesAndLambsComponent : AnimalComponentBase
    {
        public EwesAndLambsComponent()
        {
            ComponentNameDisplayString = ComponentType.LambsAndEwes.GetDescription();
            ComponentDescriptionString = Resources.ToolTipEwesAndLambsComponent;
            ComponentCategory = ComponentCategory.Sheep;
            ComponentType = ComponentType.LambsAndEwes;
        }
    }
}