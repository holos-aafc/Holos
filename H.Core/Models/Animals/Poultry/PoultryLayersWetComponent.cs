using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryLayersWetComponent : AnimalComponentBase
    {
        public PoultryLayersWetComponent()
        {
            this.ComponentNameDisplayString = ComponentType.PoultryLayersWet.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.PoultryLayersWet;
        }
    }
}

