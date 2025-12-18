using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class BisonComponent : AnimalComponentBase
    {
        public BisonComponent()
        {
            ComponentNameDisplayString = ComponentType.Bison.GetDescription();
            ComponentDescriptionString = Resources.ToolTipBisonComponent;
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.Bison;
        }
    }
}