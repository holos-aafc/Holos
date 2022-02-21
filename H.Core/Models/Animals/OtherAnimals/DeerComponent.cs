using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class DeerComponent : AnimalComponentBase
    {
        public DeerComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Deer.GetDescription();
            this.ComponentCategory = ComponentCategory.OtherLivestock;
            this.ComponentType = ComponentType.Deer;
        }
    }
}