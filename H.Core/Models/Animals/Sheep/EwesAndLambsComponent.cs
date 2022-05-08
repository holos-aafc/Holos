using H.Infrastructure;

namespace H.Core.Models.Animals.Sheep
{
    public class EwesAndLambsComponent : AnimalComponentBase
    {
        public EwesAndLambsComponent()
        {
            this.ComponentNameDisplayString = ComponentType.LambsAndEwes.GetDescription();
            this.ComponentDescriptionString = Properties.Resources.ToolTipEwesAndLambsComponent;
            this.ComponentCategory = ComponentCategory.Sheep;
            this.ComponentType = ComponentType.LambsAndEwes;
        }
    }
}