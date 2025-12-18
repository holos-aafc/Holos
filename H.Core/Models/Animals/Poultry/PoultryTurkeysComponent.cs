using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryTurkeysComponent : AnimalComponentBase
    {
        public PoultryTurkeysComponent()
        {
            ComponentNameDisplayString = ComponentType.PoultryTurkeys.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.PoultryTurkeys;
        }
    }
}