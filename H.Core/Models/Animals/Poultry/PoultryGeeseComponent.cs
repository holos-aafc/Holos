using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryGeeseComponent : AnimalComponentBase
    {
        public PoultryGeeseComponent()
        {
            ComponentNameDisplayString = ComponentType.PoultryGeese.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.PoultryGeese;
        }
    }
}