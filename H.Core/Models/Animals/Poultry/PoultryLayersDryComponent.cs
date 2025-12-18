using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryLayersDryComponent : AnimalComponentBase
    {
        public PoultryLayersDryComponent()
        {
            this.ComponentNameDisplayString = ComponentType.PoultryLayersDry.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.PoultryLayersDry;
        }
    }
}