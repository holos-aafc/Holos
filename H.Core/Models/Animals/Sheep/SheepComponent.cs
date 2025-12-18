using H.Core.Properties;

namespace H.Core.Models.Animals.Sheep
{
    public class SheepComponent : AnimalComponentBase
    {
        public SheepComponent()
        {
            ComponentNameDisplayString = Resources.TitleSheepComponentDisplayString;
            ComponentCategory = ComponentCategory.Sheep;
            ComponentType = ComponentType.Sheep;
        }
    }
}