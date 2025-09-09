using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryLayersWetComponent : AnimalComponentBase
    {
        public PoultryLayersWetComponent()
        {
            ComponentNameDisplayString = ComponentType.PoultryLayersWet.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.PoultryLayersWet;
        }
    }
}