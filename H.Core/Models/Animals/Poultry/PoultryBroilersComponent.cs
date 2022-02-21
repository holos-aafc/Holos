using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryBroilersComponent : AnimalComponentBase
    {
        public PoultryBroilersComponent()
        {
            this.ComponentNameDisplayString = ComponentType.PoultryBroilers.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.PoultryBroilers;
        }
    }
}
