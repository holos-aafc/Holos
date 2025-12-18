using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryLayersDryComponent : AnimalComponentBase
    {
        public PoultryLayersDryComponent()
        {
            ComponentNameDisplayString = ComponentType.PoultryLayersDry.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.PoultryLayersDry;
        }
    }
}