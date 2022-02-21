using H.Infrastructure;

namespace H.Core.Models.Animals.Sheep
{
    public class SheepFeedlotComponent : AnimalComponentBase
    {
        public SheepFeedlotComponent()
        {
            this.ComponentNameDisplayString = ComponentType.SheepFeedlot.GetDescription();
            this.ComponentCategory = ComponentCategory.Sheep;
            this.ComponentType = ComponentType.SheepFeedlot;
        }
    }
}