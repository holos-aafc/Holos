using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryGeeseComponent : AnimalComponentBase
    {
        public PoultryGeeseComponent()
        {
            this.ComponentNameDisplayString = ComponentType.PoultryGeese.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.PoultryGeese;
        }
    }
}