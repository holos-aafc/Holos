using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Models.Animals.Poultry.Chicken
{
    public class ChickenPulletsComponent : AnimalComponentBase
    {
        public ChickenPulletsComponent()
        {
            ComponentNameDisplayString = ComponentType.ChickenPulletFarm.GetDescription();
            ComponentCategory = ComponentCategory.Poultry;
            ComponentType = ComponentType.ChickenPulletFarm;

            ComponentDescriptionString = Resources.ToolTipPulletFarm;
        }
    }
}