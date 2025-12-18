namespace H.Core.Models.Animals.Sheep
{
    public class SheepComponent : AnimalComponentBase
    {
        public SheepComponent()
        {
            this.ComponentNameDisplayString = Properties.Resources.TitleSheepComponentDisplayString;
            this.ComponentCategory = ComponentCategory.Sheep;
            this.ComponentType = ComponentType.Sheep;
        }
    }
}