using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryDucksComponent : AnimalComponentBase
    {
        public PoultryDucksComponent()
        {
            this.ComponentNameDisplayString = ComponentType.PoultryDucks.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.PoultryDucks;
        }   
    }
}