using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Sheep
{
    public class SheepFeedlotComponent : AnimalComponentBase
    {
        public SheepFeedlotComponent()
        {
            ComponentNameDisplayString = ComponentType.SheepFeedlot.GetDescription();
            ComponentDescriptionString = Resources.ToolTipSheepFeedlotComponent;
            ComponentCategory = ComponentCategory.Sheep;
            ComponentType = ComponentType.SheepFeedlot;
        }
    }
}