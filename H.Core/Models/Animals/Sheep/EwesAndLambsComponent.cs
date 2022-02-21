using H.Infrastructure;

namespace H.Core.Models.Animals.Sheep
{
    public class EwesAndLambsComponent : AnimalComponentBase
    {
        public EwesAndLambsComponent()
        {
            this.ComponentNameDisplayString = ComponentType.LambsAndEwes.GetDescription();
            this.ComponentCategory = ComponentCategory.Sheep;
            this.ComponentType = ComponentType.LambsAndEwes;
        }
    }
}