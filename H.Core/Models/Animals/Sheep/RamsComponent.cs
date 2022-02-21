using H.Infrastructure;

namespace H.Core.Models.Animals.Sheep
{
    public class RamsComponent : AnimalComponentBase
    {
        public RamsComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Rams.GetDescription();
            this.ComponentCategory = ComponentCategory.Sheep;
            this.ComponentType = ComponentType.Rams;
        }
    }
}