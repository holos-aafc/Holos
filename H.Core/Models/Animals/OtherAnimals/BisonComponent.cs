using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class BisonComponent : AnimalComponentBase
    {
        public BisonComponent()
        {
            this.ComponentNameDisplayString = ComponentType.Bison.GetDescription();
            this.ComponentDescriptionString = Properties.Resources.ToolTipBisonComponent;
            this.ComponentCategory = ComponentCategory.OtherLivestock;
            this.ComponentType = ComponentType.Bison;
        }
    }
}
