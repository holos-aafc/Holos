using H.Infrastructure;

namespace H.Core.Models.Animals.Swine
{
    public class DrySowsComponent : AnimalComponentBase
    {
        #region Constructors

        public DrySowsComponent()
        {
            ComponentNameDisplayString = ComponentType.SwineDrySows.GetDescription();
            ComponentCategory = ComponentCategory.Swine;
            ComponentType = ComponentType.SwineDrySows;
        }

        #endregion
    }
}