using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry
{
    public class PoultryTurkeysComponent : AnimalComponentBase
    {
        public PoultryTurkeysComponent()
        {
            this.ComponentNameDisplayString = ComponentType.PoultryTurkeys.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.PoultryTurkeys;
        }
    }
}
