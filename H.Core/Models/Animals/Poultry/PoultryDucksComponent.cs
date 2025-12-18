using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryDucksComponent : AnimalComponentBase
    {
        public PoultryDucksComponent()
        {
            ComponentNameDisplayString = ComponentType.PoultryDucks.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.PoultryDucks;
        }
    }
}