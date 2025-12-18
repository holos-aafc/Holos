using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.OtherAnimals
{
    public class MulesComponent : AnimalComponentBase
    {
        public MulesComponent()
        {
            ComponentNameDisplayString = ComponentType.Mules.GetDescription();
            ComponentDescriptionString = Resources.ToolTipMulesComponent;
            ComponentCategory = ComponentCategory.OtherLivestock;
            ComponentType = ComponentType.Mules;
        }
    }
}