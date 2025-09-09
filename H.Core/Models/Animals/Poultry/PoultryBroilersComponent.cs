using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryBroilersComponent : AnimalComponentBase
    {
        public PoultryBroilersComponent()
        {
            ComponentNameDisplayString = ComponentType.PoultryBroilers.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.PoultryBroilers;
        }
    }
}