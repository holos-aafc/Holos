using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Chicken
{
    public class ChickenPulletsComponent : AnimalComponentBase
    {
        public ChickenPulletsComponent()
        {
            this.ComponentNameDisplayString = ComponentType.ChickenPulletFarm.GetDescription();
            this.ComponentCategory = ComponentCategory.Poultry;
            this.ComponentType = ComponentType.ChickenPulletFarm;

            this.ComponentDescriptionString = H.Core.Properties.Resources.ToolTipPulletFarm;
        }
    }
}