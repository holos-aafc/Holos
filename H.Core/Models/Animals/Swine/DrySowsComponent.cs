using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class DrySowsComponent : AnimalComponentBase
    {
        #region Constructors

        public DrySowsComponent()
        {
            this.ComponentNameDisplayString = ComponentType.SwineDrySows.GetDescription();
            this.ComponentCategory = ComponentCategory.Swine;
            this.ComponentType = ComponentType.SwineDrySows;
        }

        #endregion
    }
}
